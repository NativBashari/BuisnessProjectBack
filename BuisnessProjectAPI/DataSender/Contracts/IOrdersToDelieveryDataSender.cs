using Models.DataModels;

namespace BuisnessProjectAPI.DataSender.Contracts
{
    public interface IOrdersToDelieveryDataSender
    {
        void SendDelieveryData(List<Order> orders);
    }
}
