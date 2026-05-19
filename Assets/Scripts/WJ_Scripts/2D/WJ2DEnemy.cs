using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WJ2DEnemy : WJ2DUnit
{
    [Header("테스트 용")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector2 dir;

    [Header("Enemy 애니메이션")]
    [SerializeField] private WJ2DEnemyAnimation EnemyAnimation;
    private Enemy2DAnimeStat _enemyAniStat;
    private int _enemyDamage = 5;
    private float _damagePerTime = 2.5f;
    private float _damageTimer;


    private void Start()
    {
        InitStat();
        InitPlayerPosition();
        _damageTimer = _damagePerTime;
    }

    private void Update()
    {
        FollowPlayer();
        EnemyAnimation.ChangeAnime(_enemyAniStat);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.gameObject.TryGetComponent<WJ2DPlayer>(out WJ2DPlayer _player))
            {
                _damageTimer += Time.deltaTime;
                if (_player == null) return;
                if (_damageTimer >= _damagePerTime)
                {
                    _player.DecreaseCurrentHp(_enemyDamage);
                    _damageTimer = 0f;
                }
            }
        }
    }

    public void InitStat()
    {
        _hp = 5;
        _curHP = _hp;
        _moveSpeed = 1f;
    }

    private void FollowPlayer()
    {
        dir = (_playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)(dir * _moveSpeed * Time.deltaTime);

        if (dir.x < 0)
            _enemyAniStat = Enemy2DAnimeStat.LeftMove;
        else if(dir.x > 0)
            _enemyAniStat = Enemy2DAnimeStat.RightMove;
    }

    public void InitPlayerPosition()
    {
        _playerTransform = DaniTechGameManager.Inst.ReturnPlayerTransform();
    }

    private void DieEnemy()
    {
        WJ2DEnemySpawner.Inst._currentEnemy--;
        this.gameObject.SetActive(false);
        InitStat();
    }

    public override void DecreaseCurrentHp(int dmg)
    {
        base.DecreaseCurrentHp(dmg);

        if(_curHP <= 0)
        {
            DieEnemy();
        }
    }
    
}
