namespace MoFs.Prototype.Models;

public class MoStudentSelection
{
    public string SourceApplicationId { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? StudentNumber { get; init; }
    public string? BirthDateRaw { get; init; }
    public string? FoedselsnummerRaw { get; init; }
    public string? Email { get; init; }
    public string? Nationality { get; init; }
    public string? StudyProgramName { get; init; }
    public string? StudyYear { get; init; }
    public string? Semester { get; init; }
    public string? HostInstitutionCode { get; init; }
    public string? HostInstitutionName { get; init; }
    public string? MobilityProgram { get; init; }
    public DateOnly? MobilityStartDate { get; init; }
    public DateOnly? MobilityEndDate { get; init; }
}
