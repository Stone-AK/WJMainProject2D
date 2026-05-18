using UnityEngine;

public enum Player2DAnimeState
{
    None = 0,
    Idle,
    LeftWalk,
    RightWalk
}

public class WJ2DPlayer : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private Rigidbody2D PlayerRigidBody;

    [Header("점프 설정")]
    [SerializeField] private float jumpForce = 8f;

    [Header("바닥 체크")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("상태")]
    [SerializeField] private float _moveInput;
    [SerializeField] private bool _isGround;
    [SerializeField] private WJ2DPlayerAnimation PlayerAnime;


    [Header("애니메이션 컨트롤 스크립트")]
    [SerializeField] private WJ2DPlayerAnimation PlayerAnimaController;

    [Header("UI")]
    [SerializeField] private PlayerStatUI PlayerStatUI;

    private int _hp;
    private int _curHP;
    private float moveSpeed = 5f;

    private void Update()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");

        _isGround = CheckCharactorIsGround();

        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            JumpCharactor();
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.JumpStart);
        }
        MoveCharactor();

        if (_isGround)
        {
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.JumpEnd);
            if (_moveInput > 0)
                PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.RightWalk);
            else if (_moveInput < 0)
                PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.LeftWalk);
            else if (_moveInput == 0)
                PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.Idle);
        }
        if (!_isGround)
            PlayerAnimaController.ChangeMoveAnimation(Player2DAnimStat.JumpLoop);
    }

    private void MoveCharactor()
    {
        PlayerRigidBody.linearVelocity = new Vector2(
            _moveInput * moveSpeed,
            PlayerRigidBody.linearVelocity.y
        );
    }

    private void JumpCharactor()
    {
        PlayerRigidBody.linearVelocity = new Vector2(PlayerRigidBody.linearVelocity.x, jumpForce);
    }

    private bool CheckCharactorIsGround()
    {
        bool isGround;
        isGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
        return isGround;
    }

    public void TakeDamage(int damage)
    {
        _curHP -= damage;
        PlayerStatUI.ShowHP(_hp, _curHP);
        if (0 >= _curHP)
        {
            DaniTechGameManager.Inst.GameOver();
        }
    }

    public void SetPlayerData(int setHp, float setMoveSpeed)
    {
        _hp = setHp;
        moveSpeed = setMoveSpeed;
        _curHP = _hp;
        PlayerStatUI.InitUI(_hp, _curHP);
    }
    public void PrintStat()
    {
        Debug.Log($"{this.gameObject.name}의 HP는 {_curHP}");
        Debug.Log($"{this.gameObject.name}의 이동속도는 {moveSpeed}");
    }
}
