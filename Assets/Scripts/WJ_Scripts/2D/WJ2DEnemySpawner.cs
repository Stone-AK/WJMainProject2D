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

    private List<GameObject> _enemyPool = new List<GameObject>();

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
        CreateEnemyOnUpdate();
    }

    private void CreateEnemyPool()
    {
        for (int i = 0; i < _enemyPollCount; i++)
        {
            GameObject enemy = Instantiate(_enemyPrefab);

            enemy.SetActive(false);

            _enemyPool.Add(enemy);
        }
    }

    private GameObject GetEnemyFromPool()
    {
        foreach (GameObject enemy in _enemyPool)
        {
            if (enemy.activeSelf == false)
            {
                return enemy;
            }
        }
        return null;
    }

    private void CreateEnemyOnUpdate()
    {
        if(_currentEnemy <= _maximumEnemy)
        {
            int randomLocationNum = Random.Range(0, SpawnLocation.Count);
            randomOffset = UnityEngine.Random.insideUnitCircle * 1.5f;
            GameObject enemy = GetEnemyFromPool();
            if(enemy == null)
            {
                Debug.LogError($"{this.name}에 생성할 enemy가 없다.");
                return;
            }

            enemy.transform.position = 
                SpawnLocation[randomLocationNum].transform.position + (Vector3)randomOffset;
            enemy.transform.rotation = _enemyPrefab.transform.rotation;
            enemy.SetActive(true);
            _currentEnemy++;
        }
    }
}
