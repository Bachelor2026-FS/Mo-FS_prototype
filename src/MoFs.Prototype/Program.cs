using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MoFs.Prototype.Configuration;
using MoFs.Prototype.Models;
using MoFs.Prototype.Services;
using System.Xml;

if (args.Length == 0)
{
    Console.WriteLine("Bruk: dotnet run -- <sti-til-xml>");
    return;
}

var xmlPath = args[0];

if (!File.Exists(xmlPath))
{
    Console.WriteLine($"Fant ikke XML-filen: {xmlPath}");
    return;
}

try
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("Configuration/appsettings.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    var fsGraphQlOptions =
        configuration.GetSection("FsGraphQl").Get<FsGraphQlOptions>() ?? new FsGraphQlOptions();

    var integrationOptions =
        configuration.GetSection("FsIntegration").Get<FsIntegrationOptions>() ?? new FsIntegrationOptions();

    var xmlReader = new MobilityOnlineXmlReader();
    var parser = new MobilityOnlineParser();
    var foedselsnummerService = new FoedselsnummerService();
    var transformer = new FsTransformationService(integrationOptions, foedselsnummerService);
    var httpClient = new HttpClient();
    var graphQlClient = new FsGraphQlClient(httpClient, fsGraphQlOptions);

    var exportData = xmlReader.ReadFromFile(xmlPath);

    var records = new List<FsInputRecord>();

    foreach (var row in exportData.Rows)
    {
        try
        {
            var parsedStudent = parser.Parse(row);
            var transformed = transformer.Transform(parsedStudent);
            records.Add(transformed);
        }
        catch (Exception ex)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("KUNNE IKKE BYGGE STUDENT FRA XML-RAD");
            Console.WriteLine($"Feil: {ex.Message}");
            Console.WriteLine("========================================");
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Antall studenter klare for sending: {records.Count}");
    Console.WriteLine();

    var sentOk = new List<string>();
    var failed = new List<string>();

    foreach (var studentRecord in records)
    {
        var navn = $"{studentRecord.Student.Fornavn} {studentRecord.Student.Etternavn}";
        var fnr = studentRecord.Student.Fodselsnummer;

        var singlePayload = new FsPayload();
        singlePayload.Input.Add(studentRecord);

        Console.WriteLine("========================================");
        Console.WriteLine("PRØVER Å SENDE STUDENT");
        Console.WriteLine($"Navn: {navn}");
        Console.WriteLine($"Fødselsnummer: {fnr}");
        Console.WriteLine($"StudentgrunnlagId: {studentRecord.Student.StudentgrunnlagId}");
        Console.WriteLine($"KullId: {studentRecord.Studierett.KullId}");
        Console.WriteLine($"FraDato: {studentRecord.Studierett.FraDato}");
        Console.WriteLine($"FraTermin: {studentRecord.Studierett.FraTermin.Ar} / {studentRecord.Studierett.FraTermin.Betegnelse}");
        Console.WriteLine("========================================");

        try
        {
            var result = await graphQlClient.OpprettStudenterAsync(singlePayload);

            Console.WriteLine("SVAR FRA FS:");
            Console.WriteLine(result);

            if (result.Contains("\"errors\":null"))
            {
                Console.WriteLine($"SENDT OK: {navn}");
                sentOk.Add($"{navn} | {fnr}");
            }
            else
            {
                Console.WriteLine($"IKKE SENDT: {navn}");
                failed.Add($"{navn} | {fnr} | FS svarte med feil");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IKKE SENDT: {navn}");
            Console.WriteLine($"Feil: {ex.Message}");
            failed.Add($"{navn} | {fnr} | {ex.Message}");
        }

        Console.WriteLine();
    }

    Console.WriteLine("========================================");
    Console.WriteLine("OPPSUMMERING");
    Console.WriteLine("========================================");

    Console.WriteLine();
    Console.WriteLine($"SENDT OK ({sentOk.Count}):");
    if (sentOk.Count == 0)
    {
        Console.WriteLine("Ingen");
    }
    else
    {
        foreach (var item in sentOk)
        {
            Console.WriteLine($"- {item}");
        }
    }

    Console.WriteLine();
    Console.WriteLine($"IKKE SENDT ({failed.Count}):");
    if (failed.Count == 0)
    {
        Console.WriteLine("Ingen");
    }
    else
    {
        foreach (var item in failed)
        {
            Console.WriteLine($"- {item}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("FERDIG");
}
catch (InvalidOperationException ex) when (ex.InnerException is XmlException xmlEx)
{
    Console.WriteLine("Ugyldig XML.");
    Console.WriteLine($"Linje: {xmlEx.LineNumber}, posisjon: {xmlEx.LinePosition}");
    Console.WriteLine(xmlEx.Message);
}
catch (Exception ex)
{
    Console.WriteLine("Uventet feil:");
    Console.WriteLine(ex.Message);
}
