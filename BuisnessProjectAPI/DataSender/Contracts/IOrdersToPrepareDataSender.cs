using Models.DataModels;

namespace BuisnessProjectAPI.DataSender.Contracts
{
    public interface IOrdersToPrepareDataSender
    {
        void SendOrdersData(List<Order> orders);
    }
}
