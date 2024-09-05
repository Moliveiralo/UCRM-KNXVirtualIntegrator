using Knx.Falcon;

namespace KNX_Virtual_Integrator.Model.Interfaces;

public interface IGroupCommunication
{
    Task GroupValueWriteOnAsync();

    Task GroupValueWriteOffAsync();

    public Task GroupValueWriteAsync(GroupAddress addr, GroupValue value);
    public Task<GroupValue> MaGroupValueReadAsync(GroupAddress toRead);
}