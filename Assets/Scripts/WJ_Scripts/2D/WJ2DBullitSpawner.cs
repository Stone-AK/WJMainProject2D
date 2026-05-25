using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WJ2DBullitSpawner : MonoBehaviour
{
    public static WJ2DBullitSpawner Inst { get; set; }
    // 테스트 직접 할당
    [Header("테스트 직접 할당")] 
    [SerializeField] private int _PollCount = 10;

    private List<WJ2DBullit> _bullitPool = new List<WJ2DBullit>();
    private int _bullitInstIdNum = 0;

    private float _bullitOne_coolDown = 0f;

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
        CreateBulletPool();
    }

    private GameObject GetBullitPrefab(string bullitDataId = "Bullit_Base_1")
    {
        string path = DaniTechGameDataManager.Instance.GetWJBullitObjectData(bullitDataId)._path;
        GameObject loadedObj = (GameObject)Resources.Load(path);
        if (loadedObj == null)
        {
            Debug.Log($"Bullit path({bullitDataId})가 정확하지 않습니다.");
            return null;
        }
        return loadedObj;
    }

    private void CreateBulletPool(string bullitDataId = "Bullit_Base_1")
    {
        for (int i = 0; i < _PollCount; i++)
        {
            GameObject bullit = Instantiate(GetBullitPrefab(bullitDataId), this.transform);
            bullit.gameObject.SetActive(false);

            _bullitPool.Add(bullit.GetComponent<WJ2DBullit>());
        }
    }

    private GameObject GetBulletFromPool(int firedUnitId)
    {
        foreach (WJ2DBullit bullet in _bullitPool)
        {
            if (bullet.gameObject.activeSelf == false)
            {
                _bullitInstIdNum++;
                bullet.InitBullitStat(_bullitInstIdNum, firedUnitId/*총알 데이터 Id를 통해서 총알을 변경할 수 있음*/);
                WJObjectManager.Inst.AddBullitToBullitList(_bullitInstIdNum, bullet);
                return bullet.gameObject;
            }
        }
        return null;
    }

    public void ShootBulitOnUpdate(WJ2DUnit targetUnit, int firedUnitId, WJ2DUnit shootingUnit)
    {
        _bullitOne_coolDown -= Time.deltaTime;

        if (_bullitOne_coolDown > 0f)
            return;

        GameObject bullitObj = GetBulletFromPool(firedUnitId);

        if (targetUnit == null)
        {
            if (bullitObj == null)
            {
                Debug.LogError("가까운 유닛도 없고 bullet List가 비어 있습니다.");
                return;
            }
            bullitObj.transform.position = shootingUnit.gameObject.transform.position;
            bullitObj.transform.rotation = bullitObj.transform.rotation;
            bullitObj.SetActive(true);

            _bullitOne_coolDown = bullitObj.GetComponent<WJ2DBullit>().CollTime;
            return;
        }

        Vector2 dir = targetUnit.gameObject.transform.position - shootingUnit.gameObject.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        if (bullitObj == null)
        {
            Debug.LogError("bullet List가 비어 있습니다.");
            return;
        }
        bullitObj.transform.position = shootingUnit.gameObject.transform.position;
        bullitObj.transform.rotation = rot;
        bullitObj.gameObject.SetActive(true);

        _bullitOne_coolDown = bullitObj.GetComponent<WJ2DBullit>().CollTime;
    }

    public void DestroyBullit(int bullitInstId)
    {
        WJObjectManager.Inst.RemoveBullitToBullitList(bullitInstId);
    }

}
