namespace MoFs.Prototype.Models;

public class MobilityOnlineStudent
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Dette er råverdien fra MO som brukes som grunnlag for FS-fødselsnummer.
    // I noen datasett kan dette være dato, i andre kan det være fødselsnummer-lignende verdi.
    public string? BirthValue { get; set; }

    public string? FromDate { get; set; }
    public string? Semester { get; set; }
    public string? StudyYear { get; set; }
    public string? StudyProgram { get; set; }
}
