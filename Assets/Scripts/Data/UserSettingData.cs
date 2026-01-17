using UnityEngine;

namespace BalancingLibra.Data
{
    public class UserSettingData : IUserData
    {
        private const string SOUND_KEY = "Setting_Sound";
        
        public bool Sound { get; set; }

        public void SetDefaultData()
        {
            Logger.Log($"{nameof(UserSettingData)} :: SetDefaultData");
            Sound = true;
        }

        public bool LoadData()
        {
            Logger.Log($"{nameof(UserSettingData)} :: LoadData");

            try
            {
                if (PlayerPrefs.HasKey(SOUND_KEY))
                {
                    Sound = PlayerPrefs.GetInt(SOUND_KEY) == 1;
                }
                else
                {
                    SetDefaultData();
                }

                Logger.Log($"Sound : {Sound}");
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
            Logger.Log($"{nameof(UserSettingData)} : SaveData");

            try
            {
                PlayerPrefs.SetInt(SOUND_KEY, Sound ? 1 : 0);
                PlayerPrefs.Save(); // 명시적 저장 호출 (Unity 버전에 따라 자동 저장되나, 확실한 타이밍 제어 필요)
                
                Logger.Log($"Sound : {Sound}");
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