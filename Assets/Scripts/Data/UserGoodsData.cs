using System.IO;
using UnityEngine;

namespace BalancingLibra.Data
{
    [System.Serializable]
    public class UserGoodsData : IUserData
    {
        [SerializeField] private long gold;

        public long Gold { get => gold; set => gold = value; }

        private string SavePath => Path.Combine(Application.persistentDataPath, "UserGoodsData.json");

        public void SetDefaultData()
        {
            Logger.Log($"{nameof(UserGoodsData)} : SetDefaultData");
            gold = 0;
        }

        public bool LoadData()
        {
            Logger.Log($"{nameof(UserGoodsData)} : LoadData");

            if(!File.Exists(SavePath))
            {
                Logger.Log("Save file not found. Initializing default data.");
                SetDefaultData();
                return true;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                JsonUtility.FromJsonOverwrite(json, this);
                
                Logger.Log($"Gold Loaded : {Gold}");
                return true;
            }
            catch (System.Exception e)
            {
                Logger.Log($"Load failed ({e.Message})");
                return false;
            }
        }

        public bool SaveData()
        {
            Logger.Log($"{nameof(UserGoodsData)} : SaveData");

            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(SavePath, json);

                Logger.Log($"Gold Saved : {Gold}");
                return true;
            }
            catch (System.Exception e)
            {
                Logger.Log($"Save failed ({e.Message})");
                return false;
            }
        }
    }
}
