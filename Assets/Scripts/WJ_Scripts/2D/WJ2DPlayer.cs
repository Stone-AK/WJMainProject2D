using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum Player2DAnimeState
{
    None = 0,
    Idle,
    LeftWalk,
    RightWalk
}

public class WJ2DPlayer : WJ2DUnit
{
    [Header("이동 설정")]
    [SerializeField] private Rigidbody2D PlayerRigidBody;

    [Header("상태")]
    [SerializeField] private float _horizontalInput;
    [SerializeField] private float _verticalInput;
    [SerializeField] private WJ2DPlayerAnimation PlayerAnime;

    [Header("애니메이션 컨트롤 스크립트")]
    [SerializeField] private WJ2DPlayerAnimation PlayerAnimaController;

    [Header("Enemy탐지 오브젝트")]
    [SerializeField] private CircleCollider2D DetectEnemyCollider;
    [SerializeField] private LayerMask _enemyLayer;

    private readonly Collider2D[] _enemyResults = new Collider2D[30];
    private WJ2DUnit _closestUnit;

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        MoveCharactorOnUpdate();
        AnimationControllerOnUpdate();
        GetClosestEnemy();
        WJ2DBullitSpawner.Inst.ShootBulitOnUpdate(_closestUnit, _instId, this);
    }

    public void InitStat(int instId)
    {
        _instId = instId;
        // 현재 플레이어 데이터Id 하드 코딩
        _hp = DaniTechGameDataManager.Instance.GetWJUnitObjectData("Unit_Player_1")._hp;
        _curHP = _hp;
        _moveSpeed = DaniTechGameDataManager.Instance.GetWJUnitObjectData("Unit_Player_1")._moveSpeed;
    }

    private void MoveCharactorOnUpdate()
    {
        Vector2 moveDir = new Vector2(_horizontalInput, _verticalInput).normalized;

        PlayerRigidBody.linearVelocity = moveDir * _moveSpeed;
    }

    private void AnimationControllerOnUpdate()
    {
        if (_horizontalInput > 0)
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.RightWalk);
        else if (_horizontalInput < 0)
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.LeftWalk);
        else if (_verticalInput < 0 || _verticalInput > 0)
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.Move);
        else if (_horizontalInput == 0 && _verticalInput == 0)
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.Idle);
    }
    private void DiePlayer()
    {
        this.gameObject.SetActive(false);
        Debug.Log("플레이어가 죽었습니다.");
        DaniTechGameManager.Inst.EndGameOnOver();
    }
    public override void DecreaseCurrentHp(int dmg)
    {
        base.DecreaseCurrentHp(dmg);

        if (_curHP <= 0)
        {
            DiePlayer();
        }
    }

    public void GetClosestEnemy()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_enemyLayer);
        filter.useTriggers = true;

        float radius = DetectEnemyCollider.radius * DetectEnemyCollider.transform.lossyScale.x;

        int count = Physics2D.OverlapCircle(
            DetectEnemyCollider.transform.position,
            radius,
            filter,
            _enemyResults
        );

        WJ2DEnemy closestEnemy = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (_enemyResults[i].TryGetComponent(out WJ2DEnemy enemy))
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        _closestUnit = closestEnemy;
    }
}
