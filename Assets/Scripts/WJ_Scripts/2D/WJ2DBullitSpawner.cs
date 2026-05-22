using System.Collections.Generic;
using UnityEngine;

public class WJ2DBullitSpawner : MonoBehaviour
{
    // 테스트 직접 할당
    [Header("테스트 직접 할당")] 
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private int m_PollCount = 10;
    private List<GameObject> _bulletPool = new List<GameObject>();


    [Header("플레이어")]
    [SerializeField] private WJ2DPlayer Player;

    private WJ2DEnemy closestEnemy = null;
    private float _bullitOne_coolDown = 0f;

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

    private void CreateBulletPool()
    {
        for (int i = 0; i < m_PollCount; i++)
        {
            GameObject bullet = Instantiate(m_Prefab, this.transform);

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
                bullet.transform.position = DaniTechGameManager.Inst.ReturnPlayerTransform().position;
                bullet.transform.rotation = m_Prefab.transform.rotation;
                bullet.SetActive(true);

                _bullitOne_coolDown = bullit.CollTime;
                return;
            }

            Vector2 dir = closestEnemy.transform.position - DaniTechGameManager.Inst.ReturnPlayerTransform().position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            if (bullet == null)
            {
                Debug.LogError("bullet List가 비어 있습니다.");
                return;
            }
            bullet.transform.position = DaniTechGameManager.Inst.ReturnPlayerTransform().position;
            bullet.transform.rotation = rot;
            bullet.SetActive(true);

            _bullitOne_coolDown = bullit.CollTime;
        }
    }

    

}
