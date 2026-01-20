namespace BalancingLibra.Data
{
    public interface IUserData
    {
        void SetDefaultData();
        bool LoadData();
        bool SaveData();
        void DeleteData();
    }
}