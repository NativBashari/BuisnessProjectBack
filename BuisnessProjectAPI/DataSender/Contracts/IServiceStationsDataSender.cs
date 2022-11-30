using Models.DataModels;

namespace BuisnessProjectAPI.DataSender.Contracts
{
    public interface IServiceStationsDataSender
    {
        void SendServiceStationsData(List<ServiceStation> serviceStations);
    }
}
