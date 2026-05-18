using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WJ2DEnemy : MonoBehaviour
{
    [Header("테스트 용")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector2 dir;

    [Header("Enemy 애니메이션")]
    [SerializeField] private WJ2DEnemyAnimation EnemyAnimation;
    private Enemy2DAnimeStat _enemyAniStat;

    private float enemySpeed = 0.1f;


    private void Awake()
    {
        
    }

    private void Update()
    {
        FollowPlayer();
        EnemyAnimation.ChangeAnime(_enemyAniStat);
    }

    private void FollowPlayer()
    {
        /*Vector2*/ dir = (_playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)(dir * enemySpeed * Time.deltaTime);

        if (dir.x < 0)
            _enemyAniStat = Enemy2DAnimeStat.LeftMove;
        else if(dir.x > 0)
            _enemyAniStat = Enemy2DAnimeStat.RightMove;
    }

    // 플레이어 Transform 받아 메서드
    public void InitPlayerPosition(Transform player)
    {
        _playerTransform = player;
    }
}
