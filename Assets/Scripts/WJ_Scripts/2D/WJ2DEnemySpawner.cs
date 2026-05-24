using System.Collections.Generic;
using UnityEngine;

public class WJ2DEnemySpawner : MonoBehaviour
{
    public static WJ2DEnemySpawner Inst { get; set; }
    // 테스트 직접 할당
    [Header("테스트 직접 할당")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _enemyPollCount = 5;
    [SerializeField] private List<GameObject> SpawnLocation;
    [SerializeField] private int _maximumEnemy = 5;
     public int _currentEnemy { get; set; } = 0;
    Vector2 randomOffset;

    private List<WJ2DEnemy> _enemyPool = new List<WJ2DEnemy>();
    // 인스턴스 아이디와 오브젝트 풀을 매칭 시켜주는 역할
    private Dictionary<int, WJ2DEnemy> _objectIdList = new Dictionary<int, WJ2DEnemy>();
    private int _enemyInstanceId;
    private string _enemyDataIdSetting;

    private void OnDisable()
    {
        // 키가 쌓이는것을 방지
        _objectIdList.Clear();
    }

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        CreateEnemyPool();
    }

    private void Update()
    {
        // 아래 Todo는 매개변수로써 들어가도 되고 아니면 생성 기밍에서 특수한 경우 설정을 해줘도 됨
        SetEnemyDataId(/*[Todo]나중에 여기에 어떻게 EnemyDataId를 넣을지 고민해 볼것*/);
        CreateEnemyOnUpdate(_enemyDataIdSetting);
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

    // [Todo] 5월 23일 점심먹고 여기서부터
    private WJ2DEnemy GetEnemyFromPool(string EnemyId)
    {
        foreach (WJ2DEnemy enemy in _enemyPool)
        {
            if (enemy.gameObject.activeSelf == false)
            {
                _enemyInstanceId++;
                _objectIdList.Add(_enemyInstanceId, enemy);
                // [ToDo] string 공백은 몬스터 ID로 바꿀것 
                enemy.InitEnemy(_enemyInstanceId, EnemyId);
                return enemy;
            }
        }
        return null;
    }

    public void ResetObjectFromPool(int instanceId)
    {
        if(_objectIdList.ContainsKey(instanceId))
        {
            _objectIdList.Remove(instanceId);
        }
    }

    // 지금 사용하것은 아니지만 오브젝트 풀링 기반일 경우 딕셔너리를 두어 ID에 매칭되는 인덱스
    // 를 관리하는 경우 오브젝트를 가져오는 메서드
    // 
    private WJ2DEnemy GetEnemyByInstanceId(int InstanceID)
    {
        if(_objectIdList.ContainsKey(InstanceID))
        {
            return _objectIdList[InstanceID];
        }
        Debug.LogError($"존재하지 않는 오브젝트입니다.");
        return null;
    }

    private void CreateEnemyOnUpdate(string dataId)
    {
        if(_currentEnemy <= _maximumEnemy)
        {
            int randomLocationNum = Random.Range(0, SpawnLocation.Count);
            randomOffset = UnityEngine.Random.insideUnitCircle * 1.5f;
            // 나중에 
            WJ2DEnemy enemy = GetEnemyFromPool(dataId);
            if (enemy == null)
            {
                Debug.LogError($"{this.name}에 생성할 enemy가 없다.");
                return;
            }

            enemy.transform.position = 
                SpawnLocation[randomLocationNum].transform.position + (Vector3)randomOffset;
            enemy.transform.rotation = _enemyPrefab.transform.rotation;
            enemy.gameObject.SetActive(true);
            _currentEnemy++;
        }
    }

    private void SetEnemyDataId(string setDataIdValue = "Unit_Enemy_1")
    {
        _enemyDataIdSetting = setDataIdValue;
    }
}
