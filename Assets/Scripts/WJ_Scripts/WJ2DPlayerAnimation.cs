using UnityEngine;

public enum Player2DAnimStat
{
    None = 0,
    Idle,
    RightWalk,
    LeftWalk,
    Move
}

public class WJ2DPlayerAnimation : MonoBehaviour
{
    [Header("애니메이터")]
    [SerializeField] private Animator CharactorAnimator;

    [Header("상태")]
    [SerializeField] private Player2DAnimStat _CharactorAnimeState;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void ChangeMoveAnimation(Player2DAnimStat ChangeStat)
    {
        _CharactorAnimeState = ChangeStat;
        switch (_CharactorAnimeState)
        {
            case Player2DAnimStat.Idle:
                ResetAllAnimParameters();
                break;
            case Player2DAnimStat.RightWalk:
                _spriteRenderer.flipX = false;
                CharactorAnimator.SetBool("isMove", true);
                break;
            case Player2DAnimStat.LeftWalk:
                _spriteRenderer.flipX = true;
                CharactorAnimator.SetBool("isMove", true);
                break;
            case Player2DAnimStat.Move:
                CharactorAnimator.SetBool("isMove", true);
                break;
            default:
                Debug.Log("지정된 애니메이션이 없습니다.");
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        CharactorAnimator.SetBool("isMove", false);
    }
}
