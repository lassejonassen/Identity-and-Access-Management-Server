namespace Domain.Modules.Devices;

public interface IDeviceFlowRepository
{
    Task AddAsync(DeviceFlow deviceFlow);
    Task<DeviceFlow?> GetByUserCodeAsync(string userCode);
    Task<int> UserInteractionComplete(DeviceFlow deviceFlow);
    Task<DeviceFlow> GetAsync(string deviceCode, string clientId);
}
