using Knx.Falcon;
namespace KNXIntegrator.Models;

public interface IGroupCommunication
{
    Task WriteListAsync(List<(GroupAddress addr,GroupValue value)>);

    Task<List<(GroupAddress addr,GroupValue value)>> ReadListAsync(List<(GroupAddress addr)>);
}