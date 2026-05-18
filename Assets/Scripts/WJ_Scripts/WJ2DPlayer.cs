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

    private void Start()
    {
        InitStat();
    }
    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        MoveCharactorOnUpdate();
        AnimationControllerOnUpdate();
    }

    public void InitStat()
    {
        _hp = 10;
        _curHP = _hp;
        _moveSpeed = 5f;
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

    public void TakeDamage(int damage)
    {
        _curHP -= damage;
    }

    public void SetPlayerData(int setHp, float setMoveSpeed)
    {
        _hp = setHp;
        _moveSpeed = setMoveSpeed;
        _curHP = _hp;
    }
    public void PrintStat()
    {
        Debug.Log($"{this.gameObject.name}의 HP는 {_curHP}");
        Debug.Log($"{this.gameObject.name}의 이동속도는 {_moveSpeed}");
    }
}
