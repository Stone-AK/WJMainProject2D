using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum Enemy2DAnimeStat
{
    None = 0,
    RightMove,
    LeftMove
}

public class WJ2DEnemyAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    Enemy2DAnimeStat _enemyStat;

    private AsyncOperationHandle<Sprite> _spriteHandle;
    private AsyncOperationHandle<RuntimeAnimatorController> _animatorHandle;

    public void SetSpriteAndAnimator(string spritePath, string animatorPath)
    {
        SetEnemySpriteAndAnimator(spritePath, animatorPath).Forget();
    }

    private async UniTask SetEnemySpriteAndAnimator(string spritePath, string animatorPath)
    {
        _spriteHandle = Addressables.LoadAssetAsync<Sprite>(spritePath);
        await _spriteHandle.Task.AsUniTask();

        if (_spriteHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _spriteRenderer.sprite = _spriteHandle.Result;
        }
        else
        {
            Debug.LogError($"Sprite 로드 실패: {spritePath}");
        }

        _animatorHandle = Addressables.LoadAssetAsync<RuntimeAnimatorController>(animatorPath);
        await _animatorHandle.Task.AsUniTask();

        if (_animatorHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _animator.runtimeAnimatorController = _animatorHandle.Result;
        }
        else
        {
            Debug.LogError($"AnimatorController 로드 실패: {animatorPath}");
        }
    }

    private void OnDestroy()
    {
        if (_spriteHandle.IsValid())
        {
            Addressables.Release(_spriteHandle);
        }

        if (_animatorHandle.IsValid())
        {
            Addressables.Release(_animatorHandle);
        }
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
