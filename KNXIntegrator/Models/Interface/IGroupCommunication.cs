using Knx.Falcon;
namespace KNXIntegrator.Models;

public interface IGroupCommunication
{
    public Task WriteListAsync(List<(GroupAddress addr,GroupValue value)> toWrite);

    public Task<List<(GroupAddress addr,GroupValue value)>> ReadListAsync(List<GroupAddress> toRead);
}