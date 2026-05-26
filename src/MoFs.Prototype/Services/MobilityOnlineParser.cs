using MoFs.Prototype.Models;

namespace MoFs.Prototype.Services;

public class MobilityOnlineParser
{
    public MobilityOnlineStudent Parse(MobilityOnlineRow row)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }

        return new MobilityOnlineStudent
        {
            FirstName = row.FirstName?.Trim(),
            LastName = row.LastName?.Trim(),
            BirthValue = !string.IsNullOrWhiteSpace(row.SocialSecurityNumber)
                ? row.SocialSecurityNumber.Trim()
                : row.BirthDate?.Trim(),
            FromDate = row.MobilityStartDate?.Trim(),
            Semester = row.Semester?.Trim(),
            StudyYear = row.StudyYear?.Trim(),
            StudyProgram = row.StudyProgramName?.Trim()
        };
    }
}
