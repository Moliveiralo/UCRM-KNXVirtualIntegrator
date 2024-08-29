using Knx.Falcon;
namespace KNXIntegrator.Models;

public interface IGroupCommunication
{
    public Task WriteAsync((GroupAddress addr,GroupValue value) toWrite);

    public Task<(GroupValue value)> ReadAsync(GroupAddress toRead);
}