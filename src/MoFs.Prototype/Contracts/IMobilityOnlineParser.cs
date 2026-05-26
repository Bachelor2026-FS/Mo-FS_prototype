using MoFs.Prototype.Models;

namespace MoFs.Prototype.Contracts;

public interface IMobilityOnlineParser
{
    MoStudentSelection ExtractRelevantData(MobilityOnlineRow row);
}
