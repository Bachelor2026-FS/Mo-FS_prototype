using MoFs.Prototype.Models;

namespace MoFs.Prototype.Contracts;

public interface IFsTransformationService
{
    FsInputRecord Transform(MobilityOnlineStudent source);
}
