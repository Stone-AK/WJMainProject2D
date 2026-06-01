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
    private float _horizontalInput;
    private float _verticalInput;

    [Header("Sprite 및 이동 관련 할당")]
    [SerializeField] private WJ2DPlayerAnimation PlayerAnimaController;
    [SerializeField] private Rigidbody2D PlayerRigidBody;

    [Header("감지범위 Collider 할당")]
    [SerializeField] private CircleCollider2D DetectEnemyCollider;
    [SerializeField] private LayerMask _enemyLayer;

    // 보유(발사)가능한 총알의 종류를 보관하는 Dictionary(총알 데이터 ID, 총알 레벨 데이터 id)
    private Dictionary<string, int> _playerHaveBuliit = new Dictionary<string, int>();

    private readonly Collider2D[] _enemyResults = new Collider2D[30];
    private WJ2DUnit _closestUnit;

    [Header("레벨_Test")]
    [SerializeField] private int _playerLv = 0;
    [SerializeField] private float _curExp = 0;
    [SerializeField] private float _requireNextLvUpExp = 10;

    private void Start()
    {
        InvokeStatChangedEvent();
    }

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

    public void InitHaveBullitList()
    {
        var savedList = DaniTechGameManager.Inst.GetSavedBullitList();
        if (savedList.Count == 0)
        {
            _playerHaveBuliit.Add("Bullit_Base_1", 0);
        }
        else
        {
            foreach (var dataPair in savedList)
            {
                string haveBullitLvId = dataPair.Key;
                int haveBullitLvCount = dataPair.Value;

                _playerHaveBuliit.Add(haveBullitLvId, haveBullitLvCount);
            }
        }

        // 위에까지는 플레이어가 가진 총알을 초기화 해주는 부분이고 아래부터는 Spawner에 어떠한 총알을 가지고 있는지
        // 전달해주는 부분
        WJ2DBullitSpawner.Inst.GetPlayerHadBullitInfo(_playerHaveBuliit);
        DaniTechGameManager.Inst.RenewLvUpChooseList(_playerHaveBuliit);
    }

    public void RenewHaveBullitList(string upBullitId, int lvCount)
    {
        if(_playerHaveBuliit.ContainsKey(upBullitId))
        {
            _playerHaveBuliit.Remove(upBullitId);
            _playerHaveBuliit.Add(upBullitId, lvCount);
            WJ2DBullitSpawner.Inst.GetPlayerHadBullitInfo(_playerHaveBuliit);
            DaniTechGameManager.Inst.RenewLvUpChooseList(_playerHaveBuliit);
        }
        _playerHaveBuliit.Remove(upBullitId);
        _playerHaveBuliit.Add(upBullitId, lvCount);
        WJ2DBullitSpawner.Inst.GetPlayerHadBullitInfo(_playerHaveBuliit);
        DaniTechGameManager.Inst.RenewLvUpChooseList(_playerHaveBuliit);
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
        DaniTechUIManager.Instance.RemoveHudSlot(_instId);
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

    public void IncreaseExp(float getExp)
    {
        _curExp += getExp;

        while (_curExp >= _requireNextLvUpExp)
        {
            _curExp -= _requireNextLvUpExp;

            _playerLv++;

            _requireNextLvUpExp *= 1.2f;
            // 레벨 업 시 나오는 무기 팝업 UI 출력 필요
            DaniTechGameManager.Inst.LvUpChoosePhase();
        }
    }
    
    public Dictionary<string, int> ClearGameThenSaveHaveBullitList()
    {
        return _playerHaveBuliit;
    }
}
