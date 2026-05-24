using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WJ2DBullitSpawner : MonoBehaviour
{
    public static WJ2DBullitSpawner Inst { get; set; }
    // 테스트 직접 할당
    [Header("테스트 직접 할당")] 
    [SerializeField] private WJ2DBullit _Prefab;
    [SerializeField] private int _PollCount = 10;
    private List<WJ2DBullit> _bullitPool = new List<WJ2DBullit>();
    private int _bullitInstIdNum = 0;
    private Dictionary<int, WJ2DBullit> _bullitObjectList = new Dictionary<int, WJ2DBullit>();


    [Header("플레이어")]
    [SerializeField] private WJ2DPlayer Player;

    private WJ2DEnemy closestEnemy = null;
    private float _bullitOne_coolDown = 0f;

    private void Awake()
    {
        Inst = this;
    }

    private void OnDisable()
    {
        _bullitPool.Clear();
        _bullitObjectList.Clear();
        _bullitInstIdNum = 0;
    }

    private void Start()
    {
        Player = DaniTechGameManager.Inst.ReturnPlayerTransform().gameObject.GetComponent<WJ2DPlayer>();
        CreateBulletPool();
    }

    private void Update()
    {
        closestEnemy = Player.GetClosestEnemy();
        ShootBulit();
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

    private GameObject GetBulletFromPool()
    {
        foreach (WJ2DBullit bullet in _bullitPool)
        {
            if (bullet.gameObject.activeSelf == false)
            {
                _bullitInstIdNum++;
                bullet.InitBullitStat(_bullitInstIdNum/*총알 데이터 Id를 통해서 총알을 변경할 수 있음*/);
                _bullitObjectList.Add(_bullitInstIdNum, bullet);
                return bullet.gameObject;
            }
        }
        return null;
    }

    private void ShootBulit()
    {
        _bullitOne_coolDown -= Time.deltaTime;

        if (_bullitOne_coolDown > 0f)
            return;

        GameObject bullitObj = GetBulletFromPool();

        if (closestEnemy == null)
        {
            if (bullitObj == null)
            {
                Debug.LogError("가까운 적도 없고 bullet List가 비어 있습니다.");
                return;
            }
            bullitObj.transform.position = DaniTechGameManager.Inst.ReturnPlayerTransform().position;
            bullitObj.transform.rotation = _Prefab.transform.rotation;
            bullitObj.SetActive(true);

            _bullitOne_coolDown = bullitObj.GetComponent<WJ2DBullit>().CollTime;
            return;
        }

        Vector2 dir = closestEnemy.transform.position - DaniTechGameManager.Inst.ReturnPlayerTransform().position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        if (bullitObj == null)
        {
            Debug.LogError("bullet List가 비어 있습니다.");
            return;
        }
        bullitObj.transform.position = DaniTechGameManager.Inst.ReturnPlayerTransform().position;
        bullitObj.transform.rotation = rot;
        bullitObj.gameObject.SetActive(true);

        _bullitOne_coolDown = bullitObj.GetComponent<WJ2DBullit>().CollTime;
    }

    public void DestroyBullit(int bullitInstId)
    {
        _bullitObjectList.Remove(bullitInstId);
    }

}
