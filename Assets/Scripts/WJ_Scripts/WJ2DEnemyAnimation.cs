using UnityEngine;

public enum Enemy2DAnimeStat
{
    None = 0,
    RightMove,
    LeftMove
}

public class WJ2DEnemyAnimation : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    Enemy2DAnimeStat _enemyStat;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeAnime(Enemy2DAnimeStat ChangeStat)
    {
        _enemyStat = ChangeStat;
        switch(_enemyStat)
        {
            case Enemy2DAnimeStat.RightMove:
                _spriteRenderer.flipX = false;
                break;
            case Enemy2DAnimeStat.LeftMove:
                _spriteRenderer.flipX = true;
                break;
        }
    }

}
