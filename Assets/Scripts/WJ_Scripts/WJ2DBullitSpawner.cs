using System.Collections.Generic;
using UnityEngine;

public class WJ2DBullitSpawner : MonoBehaviour
{
    // 테스트 직접 할당
    [Header("테스트 직접 할당")] 
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private int m_PollCount = 5;
    private List<GameObject> _bulletPool = new List<GameObject>();

    private List<WJ2DEnemy> _enemies = new List<WJ2DEnemy>();
    private WJ2DEnemy closestEnemy = null;
    private float _bullitOne_coolDown = 0f;

    private void Start()
    {
        CreateBulletPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out WJ2DEnemy enemy))
        {
            _enemies.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out WJ2DEnemy enemy))
        {
            _enemies.Remove(enemy);
        }
    }

    private void Update()
    {
        FindClosEnemy();
        ShootBulit();
    }

    private void CreateBulletPool()
    {
        for (int i = 0; i < m_PollCount; i++)
        {
            GameObject bullet = Instantiate(m_Prefab);

            bullet.SetActive(false);

            _bulletPool.Add(bullet);
        }
    }

    private GameObject GetBulletFromPool()
    {
        foreach (GameObject bullet in _bulletPool)
        {
            if (bullet.activeSelf == false)
            {
                return bullet;
            }
        }

        return null;
    }

    private void ShootBulit()
    {
        if (m_Prefab.TryGetComponent(out WJ2DBullit bullit))
        {
            _bullitOne_coolDown -= Time.deltaTime;

            if (_bullitOne_coolDown > 0f)
                return;

            GameObject bullet = GetBulletFromPool();

            if (closestEnemy == null)
            {
                if(bullet == null)
                {
                    Debug.LogError("bullet List가 비어 있습니다.");
                    return;
                }
                bullet.transform.position = this.gameObject.transform.position;
                bullet.transform.rotation = m_Prefab.transform.rotation;
                bullet.SetActive(true);

                _bullitOne_coolDown = bullit.CollTime;
                return;
            }

            Vector2 dir = closestEnemy.transform.position - this.gameObject.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            if (bullet == null)
            {
                Debug.LogError("bullet List가 비어 있습니다.");
                return;
            }
            bullet.transform.position = this.gameObject.transform.position;
            bullet.transform.rotation = rot;
            bullet.SetActive(true);

            _bullitOne_coolDown = bullit.CollTime;
        }
    }

    private void FindClosEnemy()
    {
        float minDistance = float.MaxValue;

        foreach (WJ2DEnemy enemy in _enemies)
        {
            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }
    }

}
