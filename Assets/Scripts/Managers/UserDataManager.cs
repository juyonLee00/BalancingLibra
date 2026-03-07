using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BalancingLibra.Data;

public class UserDataManager : SingletonBehaviour<UserDataManager>
{
    private UserGoodsData _goodsData;
    private UserSettingData _settingData;

    private List<IUserData> _userDataList;

    public UserGoodsData Goods => _goodsData;
    public UserSettingData Setting => _settingData;

    protected override void Init()
    {
        base.Init();

        _goodsData = new UserGoodsData();
        _settingData = new UserSettingData();

        _userDataList = new List<IUserData>
        {
            _settingData,
            _goodsData
        };

        LoadAllData();
    }

    public void LoadAllData()
    {
        Logger.Log("UserDataManage : Load All Data", this);

        foreach(var data in _userDataList)
        {
            data.LoadData();
        }
    }

    public void SaveAllData()
    {
        Logger.Log("UserDataManager : Save All Data", this);

        foreach(var data in _userDataList)
        {
            data.SaveData();
        }
    }

    public void ResetAllData()
    {
        Logger.Log("UserDataManager : Reset All Data", this);

        foreach(var data in _userDataList)
        {
            data.SetDefaultData();
        }
        SaveAllData();
    }

    public void SaveSpecificData(IUserData targetData)
    {
        if(targetData == null)
        {
            Logger.Log("SaveSpecificData : Target is null");
            return;
        }

        bool success = targetData.SaveData();
        if(success)
            Logger.Log($"SaveSpecificData : Success ({targetData.GetType().Name})");
        else
            Logger.Log($"SaveSpecificData : Failed ({targetData.GetType().Name})");
    }

    public void DeleteSpecificData(IUserData targetData)
    {
        if(targetData == null)
        {
            Logger.Log("DeleteSpecificData : Target is null");
            return;
        }

        targetData.DeleteData();
        Logger.Log($"DeleteSpecificData : Deleted ({targetData.GetType().Name})");
    }

    private void OnApplicationQuit() 
    {
        SaveAllData();
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if(pauseStatus)
        {
            SaveAllData();
        }
    }
    
}
