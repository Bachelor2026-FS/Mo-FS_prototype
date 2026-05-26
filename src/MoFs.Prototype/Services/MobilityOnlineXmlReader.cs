using System.Xml.Serialization;
using MoFs.Prototype.Models;

namespace MoFs.Prototype.Services;

public class MobilityOnlineXmlReader
{
    public MobilityOnlineExport ReadFromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Filsti kan ikke være tom.", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Fant ikke XML-filen.", path);
        }

        var serializer = new XmlSerializer(typeof(MobilityOnlineExport));
        using var stream = File.OpenRead(path);
        var result = serializer.Deserialize(stream) as MobilityOnlineExport;

        return result ?? throw new InvalidOperationException("Kunne ikke deserialisere XML-filen til MobilityOnlineExport.");
    }
}
