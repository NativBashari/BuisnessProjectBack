using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public interface IMainLogic
    {

        void PauseCustomers();
        void ContinueCustomers();
        void CloseServiceStation(int id);
        void SetBuisnsess(Buisness buisness);
        void StartLogic();
    }
}
