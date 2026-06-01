using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WJ2DBullitSpawner : MonoBehaviour
{
    public static WJ2DBullitSpawner Inst { get; set; }
    
    private int _PollCount = 200;
    private int _bullitInstIdNum = 0;

    private List<string> _playerHaveBullitLvIdList = new List<string>();
    private Dictionary<string, float> _bullitIdAndFireCurTimeList = new Dictionary<string, float>();

    private List<WJ2DBullit> _bullitPool = new List<WJ2DBullit>();
    private string _BasicBullitType = "Prefabs/2D/Bullit";

    private void Awake()
    {
        Inst = this;
    }

    private void OnDisable()
    {
        _bullitPool.Clear();
        WJObjectManager.Inst.RemoveAllBullitList();
        _bullitInstIdNum = 0;
    }

    private void Start()
    {
        CreateBullitPool();
    }

    private GameObject GetBullitPrefab(string bullitDataId)
    {
        string path = DaniTechGameDataManager.Instance.GetWJBullitLvData(bullitDataId)._bullitPrefabPath;
        GameObject loadedObj = (GameObject)Resources.Load(path);
        if (loadedObj == null)
        {
            Debug.Log($"Bullit path({bullitDataId})가 정확하지 않습니다.");
            return null;
        }
        return loadedObj;
    }

    private void CreateBullitPool()
    {
        foreach (string bullitId in _playerHaveBullitLvIdList)
        {
            for (int i = 0; i < _PollCount; i++)
            {
                string bullitPath = DaniTechGameDataManager.Instance.GetWJBullitLvData(bullitId)._bullitPrefabPath;
                GameObject bullit = Instantiate(GetBullitPrefab(bullitId), this.transform);
                bullit.gameObject.SetActive(false);

                if(bullitPath == _BasicBullitType)
                {
                    _bullitPool.Add(bullit.GetComponent<WJ2DBullit>());
                }
            }
        }
    }

    private GameObject GetBullitFromPool(int firedUnitId, string bullitLvDataId)
    {
        foreach (WJ2DBullit bullit in _bullitPool)
        {
            //if(bullit is WJ2DBullit )
            if (bullit.gameObject.activeSelf == false)
            {
                _bullitInstIdNum++;
                bullit.InitBullitStat(_bullitInstIdNum, firedUnitId, bullitLvDataId);
                WJObjectManager.Inst.AddBullitToBullitList(_bullitInstIdNum, bullit);
                return bullit.gameObject;
            }
        }
        return null;
    }

    public void ShootBulitOnUpdate(WJ2DUnit targetUnit, int firedUnitId, WJ2DUnit shootingUnit)
    {
        // 수정 중
        foreach (string checkBullitLvId in _playerHaveBullitLvIdList)
        {
            BullitFnuncType bullitFuncVal = CheckShootingBullit(checkBullitLvId);
            if (bullitFuncVal == BullitFnuncType.None) continue;

            switch (bullitFuncVal)
            {
                case BullitFnuncType.Basic:
                    ShootBasic(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
            }
        }
    }

    private BullitFnuncType CheckShootingBullit(string checkBullitLvId)
    {
        float collTime = DaniTechGameDataManager.Instance.GetWJBullitLvData(checkBullitLvId)._collTime;
        _bullitIdAndFireCurTimeList[checkBullitLvId] += Time.deltaTime;

        if (collTime <= _bullitIdAndFireCurTimeList[checkBullitLvId])
        {
            return Enum.Parse<BullitFnuncType>(DaniTechGameDataManager.Instance.GetWJBullitLvData(checkBullitLvId)._bullitfunc);
        }

        return BullitFnuncType.None;
    }

    private void CheckTargetTransformAndLoadBullit(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, GameObject bullitObj)
    {
        if (targetUnit == null)
        {
            bullitObj.transform.position = shootingUnit.gameObject.transform.position;
            bullitObj.transform.rotation = bullitObj.transform.rotation;
            return;
        }

        Vector2 dir = targetUnit.gameObject.transform.position - shootingUnit.gameObject.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        bullitObj.transform.position = shootingUnit.gameObject.transform.position;
        bullitObj.transform.rotation = rot;
    }

    private void ShootBasic(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bulliLvId)
    {
        GameObject bullitObj = GetBullitFromPool(firedUnitId, bulliLvId);
        if (bullitObj == null)
        {
            Debug.LogError("bullet List가 비어 있습니다.");
            return;
        }

        if (targetUnit == null)
        {
            CheckTargetTransformAndLoadBullit(targetUnit, shootingUnit, bullitObj);
            bullitObj.SetActive(true);

            _bullitIdAndFireCurTimeList[bulliLvId] = 0;
            return;
        }

        CheckTargetTransformAndLoadBullit(targetUnit, shootingUnit, bullitObj);
        bullitObj.gameObject.SetActive(true);

        _bullitIdAndFireCurTimeList[bulliLvId] = 0;
    }

    public void DestroyBullit(int bullitInstId)
    {
        WJObjectManager.Inst.RemoveBullitToBullitList(bullitInstId);
    }


    // 
    public void GetPlayerHadBullitInfo(Dictionary<string, int> playerHadBullitList)
    {
        foreach (var keyValPair in playerHadBullitList)
        {
            string bullitId = keyValPair.Key;
            int bullitLv = keyValPair.Value;
            string bullitLvDataId = DaniTechGameDataManager.Instance.GetWJBullitObjectData(bullitId)._bullitLvList[bullitLv];
            if (bullitLvDataId == null) return;
            var bullitLvData = DaniTechGameDataManager.Instance.GetWJBullitLvData(bullitLvDataId);
            if (bullitLvData == null) return;

            if(_playerHaveBullitLvIdList.Contains(bullitLvDataId))
            {
                _playerHaveBullitLvIdList.Remove(bullitLvDataId);
                _bullitIdAndFireCurTimeList.Remove(bullitLvDataId);
                _playerHaveBullitLvIdList.Add(bullitLvDataId);
                _bullitIdAndFireCurTimeList.Add(bullitLvDataId, 0f);
                return;
            }
            _playerHaveBullitLvIdList.Add(bullitLvDataId);
            _bullitIdAndFireCurTimeList.Add(bullitLvDataId , 0f);
        }
    }
}
