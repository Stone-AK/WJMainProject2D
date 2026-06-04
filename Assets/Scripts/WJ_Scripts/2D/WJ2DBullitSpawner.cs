using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WJ2DBullitSpawner : MonoBehaviour
{
    public static WJ2DBullitSpawner Inst { get; set; }
    
    private int _bullitInstIdNum = 0;

    private List<string> _playerHaveBullitLvIdList = new List<string>();
    private Dictionary<string, float> _bullitIdAndFireCurTimeList = new Dictionary<string, float>();

    private List<WJ2DBullit> _bullitPool = new List<WJ2DBullit>();
    private List<WJ2DMeleeAtk> _meleePool = new List<WJ2DMeleeAtk>();

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
        Dictionary<string, int> poolingNeedList = new Dictionary<string, int>();
        List<string> pollingBullitIdList = new List<string>();
        
        foreach(var data in DaniTechGameDataManager.Instance.WJBullitLvDataList)
        {
            string prefabPath = data.Value._bullitPrefabPath;
            if (poolingNeedList.ContainsKey(prefabPath)) continue;

            int poolCount = data.Value._pollCount;

            pollingBullitIdList.Add(data.Value.Id);
            poolingNeedList.Add(prefabPath, poolCount);
        }

        int pollingBullitLvListCount = 0;
        foreach (string poolPrefabPath in poolingNeedList.Keys)
        {
            int poolCount = poolingNeedList[poolPrefabPath];

            for (int i = 0; i < poolCount; i++)
            {
                GameObject bullit = Instantiate(GetBullitPrefab(pollingBullitIdList[pollingBullitLvListCount]), this.transform);
                bullit.gameObject.SetActive(false);

                if (pollingBullitIdList[pollingBullitLvListCount].Contains("Bullit_Base"))
                {
                    _bullitPool.Add(bullit.GetComponent<WJ2DBullit>());
                }
                else if (pollingBullitIdList[pollingBullitLvListCount].Contains("Melee_Base"))
                {
                    _meleePool.Add(bullit.GetComponent<WJ2DMeleeAtk>());
                }
            }
            pollingBullitLvListCount++;
        }
    }

    private GameObject GetBullitFromPool(int firedUnitId, string bullitLvDataId)
    {
        foreach (WJ2DBullit bullit in _bullitPool)
        {
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

    public void DestroyBullit(int bullitInstId)
    {
        WJObjectManager.Inst.RemoveBullitToBullitList(bullitInstId);
    }

    private GameObject GetMeleeFromPool(int firedUnitId, string bullitLvDataId)
    {
        foreach (WJ2DMeleeAtk melee in _meleePool)
        {
            if (melee.gameObject.activeSelf == false)
            {
                _bullitInstIdNum++;

                melee.InitBullitStat(_bullitInstIdNum, firedUnitId, bullitLvDataId);

                WJObjectManager.Inst.AddMeleeAtkToMeleeAtkList(_bullitInstIdNum, melee);

                return melee.gameObject;
            }
        }

        return null;
    }

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

    public void ShootBulitOnUpdate(WJ2DUnit targetUnit, int firedUnitId, WJ2DUnit shootingUnit)
    {
        foreach (string checkBullitLvId in _playerHaveBullitLvIdList)
        {
            BullitFnuncType bullitFuncVal = CheckShootingBullit(checkBullitLvId);
            if (bullitFuncVal == BullitFnuncType.None) continue;

            switch (bullitFuncVal)
            {
                case BullitFnuncType.Basic:
                    ShootBasic(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.DoubleFiring:
                    ShootDouble(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.TripleFiring:
                    ShootTriple(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.Bigger:
                    SootBiggerBullit(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.Gigantamax:
                    SootGigantamaxBullit(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.MeleeBasic:
                    ShootMeleeAttack(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.MeleeDoubleSize:
                    ShootMeleeDoubleSizeAttack(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
                case BullitFnuncType.MeleeTripleSize:
                    ShootMeleeTripleSizeAttack(targetUnit, shootingUnit, firedUnitId, checkBullitLvId);
                    break;
            }
        }
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

    private void ShootDouble(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bulliLvId)
    {
        float barrelGap = 0.25f;
        int wantedShootCount = 2;

        for (int i = 0; i < wantedShootCount; i++)
        {
            GameObject bullitObj = GetBullitFromPool(firedUnitId, bulliLvId);

            if (bullitObj == null)
            {
                Debug.LogError("bullet List가 비어 있습니다.");
                return;
            }

            CheckTargetTransformAndLoadBullit(targetUnit, shootingUnit, bullitObj);

            float offsetAmount = (i - (wantedShootCount - 1) / 2f) * barrelGap;
            Vector3 sideOffset = bullitObj.transform.up * offsetAmount;

            bullitObj.transform.position += sideOffset;
            bullitObj.SetActive(true);
        }

        _bullitIdAndFireCurTimeList[bulliLvId] = 0;
    }

    private void ShootTriple(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bulliLvId)
    {
        float barrelGap = 0.25f;
        int wantedShootCount = 3;

        for (int i = 0; i < wantedShootCount; i++)
        {
            GameObject bullitObj = GetBullitFromPool(firedUnitId, bulliLvId);

            if (bullitObj == null)
            {
                Debug.LogError("bullet List가 비어 있습니다.");
                return;
            }

            CheckTargetTransformAndLoadBullit(targetUnit, shootingUnit, bullitObj);

            float offsetAmount = (i - (wantedShootCount - 1) / 2f) * barrelGap;
            Vector3 sideOffset = bullitObj.transform.up * offsetAmount;

            bullitObj.transform.position += sideOffset;
            bullitObj.SetActive(true);
        }

        _bullitIdAndFireCurTimeList[bulliLvId] = 0;
    }

    private void SootBiggerBullit(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bulliLvId)
    {
        float bullitScale = 2f;

        GameObject bullitObj = GetBullitFromPool(firedUnitId, bulliLvId);

        if (bullitObj == null)
        {
            Debug.LogError("bullet List가 비어 있습니다.");
            return;
        }

        CheckTargetTransformAndLoadBullit(targetUnit, shootingUnit, bullitObj);

        bullitObj.transform.localScale = new Vector3(bullitScale, bullitScale, 1f);
        bullitObj.SetActive(true);

        _bullitIdAndFireCurTimeList[bulliLvId] = 0;
    }

    private void SootGigantamaxBullit(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bulliLvId)
    {
        float bullitScale = 3f;

        GameObject bullitObj = GetBullitFromPool(firedUnitId, bulliLvId);

        if (bullitObj == null)
        {
            Debug.LogError("bullet List가 비어 있습니다.");
            return;
        }

        CheckTargetTransformAndLoadBullit(targetUnit, shootingUnit, bullitObj);

        bullitObj.transform.localScale = new Vector3(bullitScale, bullitScale, 1f);
        bullitObj.SetActive(true);

        _bullitIdAndFireCurTimeList[bulliLvId] = 0;
    }

    private void ShootMeleeAttack(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bullitLvId)
    {
        GameObject meleeObj = GetMeleeFromPool(firedUnitId, bullitLvId);

        if (meleeObj == null)
        {
            Debug.LogError("근접 공격 오브젝트를 가져오지 못했습니다.");
            return;
        }

        Vector2 attackDir;

        if (targetUnit != null)
            attackDir = targetUnit.transform.position - shootingUnit.transform.position;
        else
            attackDir = shootingUnit.transform.right;

        if (attackDir == Vector2.zero)
            attackDir = Vector2.right;

        attackDir.Normalize();

        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float meleeOffset = 0.8f;

        meleeObj.transform.position = shootingUnit.transform.position + (Vector3)(attackDir * meleeOffset);
        meleeObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
        meleeObj.SetActive(true);

        _bullitIdAndFireCurTimeList[bullitLvId] = 0f;
    }

    private void ShootMeleeDoubleSizeAttack(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bullitLvId)
    {
        GameObject meleeObj = GetMeleeFromPool(firedUnitId, bullitLvId);

        if (meleeObj == null)
        {
            Debug.LogError("근접 공격 오브젝트를 가져오지 못했습니다.");
            return;
        }

        Vector2 attackDir;

        if (targetUnit != null)
            attackDir = targetUnit.transform.position - shootingUnit.transform.position;
        else
            attackDir = shootingUnit.transform.right;

        if (attackDir == Vector2.zero)
            attackDir = Vector2.right;

        attackDir.Normalize();

        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float meleeOffset = 0.8f;

        meleeObj.transform.position = shootingUnit.transform.position + (Vector3)(attackDir * meleeOffset);
        meleeObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
        meleeObj.transform.localScale = new Vector3(2f, 2f, 1f);
        meleeObj.SetActive(true);

        _bullitIdAndFireCurTimeList[bullitLvId] = 0f;
    }

    private void ShootMeleeTripleSizeAttack(WJ2DUnit targetUnit, WJ2DUnit shootingUnit, int firedUnitId, string bullitLvId)
    {
        GameObject meleeObj = GetMeleeFromPool(firedUnitId, bullitLvId);

        if (meleeObj == null)
        {
            Debug.LogError("근접 공격 오브젝트를 가져오지 못했습니다.");
            return;
        }

        Vector2 attackDir;

        if (targetUnit != null)
            attackDir = targetUnit.transform.position - shootingUnit.transform.position;
        else
            attackDir = shootingUnit.transform.right;

        if (attackDir == Vector2.zero)
            attackDir = Vector2.right;

        attackDir.Normalize();

        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float meleeOffset = 0.8f;

        meleeObj.transform.position = shootingUnit.transform.position + (Vector3)(attackDir * meleeOffset);
        meleeObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
        meleeObj.transform.localScale = new Vector3(3f, 3f, 1f);
        meleeObj.SetActive(true);

        _bullitIdAndFireCurTimeList[bullitLvId] = 0f;
    }

}
