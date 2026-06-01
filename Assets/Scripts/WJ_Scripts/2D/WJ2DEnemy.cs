using System.Runtime.CompilerServices;
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
    private int _enemyDamage;
    private float _damagePerTime;
    private float _damageTimer;
    private string _enemyDataId;
    private float _exp;

    private void Start()
    {
        InitPlayerPosition();
    }

    private void Update()
    {
        FollowPlayer();
        EnemyAnimation.ChangeAnime(_enemyAniStat);
        _damageTimer += Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(collision.gameObject.TryGetComponent<WJ2DUnit>(out WJ2DUnit unitObj))
            {
                if (_damageTimer >= _damagePerTime)
                {
                    var unit = WJObjectManager.Inst.GetUnitToUnitList(unitObj._instId);
                    unit.DecreaseCurrentHp(_enemyDamage);
                    _damageTimer = 0f;
                }
            }
        }
    }

    public void InitEnemy(int instId, string dataId)
    {
        _instId = instId;
        var enemyData = DaniTechGameDataManager.Instance.GetWJUnitObjectData(dataId);
        InitStat(enemyData);
    }

    public int GetEnemyInstId()
    {
        return _instId;
    }

    public void InitStat(WJUnit initData)
    {
        _enemyDataId = initData.Id;
        _hp = initData._hp;
        _curHP = _hp;
        _moveSpeed = initData._moveSpeed;
        _damagePerTime = initData._damagePerTime;
        _enemyDamage = initData._damage;
        _exp = initData._exp;
        EnemyAnimation.SetSpriteAndAnimator(
            initData._spritePath,
            initData._animePath);
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
        WJItemManager.Inst.DropTheDropItem(this.transform, _exp);
        WJ2DEnemySpawner.Inst.DieEnemyOnSpanwer(_enemyDataId);
        DaniTechGameManager.Inst.IncreasCatchEnemyCount();
        this.gameObject.SetActive(false);
        WJObjectManager.Inst.RemoveUnitToUnitList(_instId);
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
