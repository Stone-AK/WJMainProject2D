using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class WJ2DEnemySpawner : MonoBehaviour
{
    public static WJ2DEnemySpawner Inst { get; set; }
    // [Todo] 지금은 Enemy형식이 한개밖에 없어서 직접할당도 되지만 나중에는 동적 생성 필요한 부분
    // 급한거 아님
    [SerializeField] private GameObject _enemyPrefab;
    // [Todo] Dictionary부분 잘되면 poll 수를 좀 많이 늘려놓아야함(데이터 드리븐으로 정해줘도 좋음)
    // 급한거 아님
    private int _enemyPollCount = 50;
    [SerializeField] private List<GameObject> SpawnLocation;

    Vector2 randomOffset;

    private List<WJ2DEnemy> _enemyPool = new List<WJ2DEnemy>();
    private Dictionary<string, int> _curEnemyCountList = new Dictionary<string, int>();
    private Dictionary<string, int> _maxEnemyCountList = new Dictionary<string, int>();
    private int _curFieldEnemyCount;
    private int _maxFieldEnemyCount = 200;
    private int _enemyInstanceId;

    private string _curSpawnerWaveId;
    private bool _isChangingWave = true;


    private void OnDisable()
    {
        WJObjectManager.Inst.RemoveAllUnitList();
    }

    private void Awake()
    {
        Inst = this;
    }

    private void OnEnable()
    {
        _enemyInstanceId = 0;
        _curFieldEnemyCount = 0;
    }

    private void Start()
    {
        CreateEnemyPool();
    }

    private void Update()
    {
        if (_isChangingWave) return;

        if(_curFieldEnemyCount <=  _maxFieldEnemyCount)
        {
            CreateEnemyOnUpdate();
        }
    }

    private void CreateEnemyPool()
    {
        for (int i = 0; i < _enemyPollCount; i++)
        {
            GameObject enemyGobj = Instantiate(_enemyPrefab, this.transform);

            enemyGobj.SetActive(false);

            var enemyComponent = enemyGobj.GetComponent<WJ2DEnemy>();
            if (enemyComponent == null) return;

            _enemyPool.Add(enemyComponent);
        }
    }

    private WJ2DEnemy GetEnemyFromPool(string EnemyId)
    {
        foreach (WJ2DEnemy enemy in _enemyPool)
        {
            if (enemy.gameObject.activeSelf == false)
            {
                _enemyInstanceId++;
                WJObjectManager.Inst.AddUnitToUnitList(_enemyInstanceId, enemy);
                enemy.InitEnemy(_enemyInstanceId, EnemyId);
                return enemy;
            }
        }
        return null;
    }

    private void CreateEnemyOnUpdate()
    {
        foreach (string enemyId in _curEnemyCountList.Keys.ToList())
        {
            if (_curEnemyCountList.TryGetValue(enemyId, out int curCount) == false)
                continue;

            if (_maxEnemyCountList.TryGetValue(enemyId, out int maxCount) == false)
                continue;

            if (curCount < maxCount)
            {
                int randomLocationNum = Random.Range(0, SpawnLocation.Count);
                randomOffset = UnityEngine.Random.insideUnitCircle * 1.5f;

                WJ2DEnemy enemy = GetEnemyFromPool(enemyId);
                if(enemy == null) return;

                enemy.transform.position =
                SpawnLocation[randomLocationNum].transform.position + (Vector3)randomOffset;
                enemy.transform.rotation = _enemyPrefab.transform.rotation;
                enemy.gameObject.SetActive(true);
                _curEnemyCountList[enemyId]++;
                _curFieldEnemyCount++;
            }
        }
    }

    public void SetSpwanerWave(string waveId)
    {
        _curSpawnerWaveId = waveId;
        SetSpawnEnemyId(waveId);
    }

    private void SetSpawnEnemyId(string waveId)
    {
        _isChangingWave = true;
        ResetEnemyCountDictionary();
        string waveEnemyDataId = DaniTechGameDataManager.Instance.GetWJWaveData(waveId).SpawnEnemyDataId;
        if (waveEnemyDataId == null) return;

        List<string> waveEnemyIdListData 
            = DaniTechGameDataManager.Instance.GetWJWaveEnemyData(waveEnemyDataId).EnemyIdList;
        List<int> waveEnemyMaxCountListData
            = DaniTechGameDataManager.Instance.GetWJWaveEnemyData(waveEnemyDataId).EnemyMaxCountList;

        int waveEnemyCountSeq = 0;
        foreach (string waveEnemyId in waveEnemyIdListData)
        {
            if (waveEnemyId == null) return;

            _curEnemyCountList.Add(waveEnemyId, 0);
            _maxEnemyCountList.Add(waveEnemyId, waveEnemyMaxCountListData[waveEnemyCountSeq]);
            waveEnemyCountSeq++;
        }
        _isChangingWave = false;
    }

    private void ResetEnemyCountDictionary()
    {
        _curEnemyCountList.Clear();
        _maxEnemyCountList.Clear();
    }

    public void DieEnemyOnSpanwer(string enemyId)
    {
        _curEnemyCountList[enemyId]--;
        _curFieldEnemyCount--;
    }
}
