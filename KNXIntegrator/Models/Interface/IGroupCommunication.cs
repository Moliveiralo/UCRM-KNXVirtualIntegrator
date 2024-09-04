using Knx.Falcon;
namespace KNXIntegrator.Models;

public interface IGroupCommunication
{
    public Task GroupValueWriteAsync((GroupAddress addr,GroupValue value) toWrite);

    public Task<GroupValue> MaGroupValueReadAsync(GroupAddress toRead);
}