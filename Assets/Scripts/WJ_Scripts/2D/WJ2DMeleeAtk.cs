using System;
using System.Collections.Generic;
using UnityEngine;

public class WJ2DMeleeAtk : MonoBehaviour
{
    private int _createUnitInstId;
    private int _bullitInstId;
    private int _power;
    // _knockBackPower = MoveSpeed값
    private float _knockBackPower;

    [SerializeField] private SpriteRenderer _meleeSprite;
    [SerializeField] private Animator _meleeAnimator;
    private BullitFnuncType _curBullitFunc;

    private HashSet<int> _hitEnemyIdSet = new HashSet<int>();

    private void OnEnable()
    {
        _hitEnemyIdSet.Clear();

        CancelInvoke();

        float lifeTime = GetAttackAnimationLength();

        Invoke(nameof(DestroyBullit), lifeTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TouchEnemy(collision, _createUnitInstId);
        }
    }

    private float GetAttackAnimationLength()
    {
        if (_meleeAnimator == null)
            return 0.5f;

        RuntimeAnimatorController controller = _meleeAnimator.runtimeAnimatorController;

        if (controller == null)
            return 0.5f;

        AnimationClip[] clips = controller.animationClips;

        if (clips == null || clips.Length == 0)
            return 0.5f;

        return clips[0].length;
    }

    public void InitBullitStat(int bullitInstId, int unitThatFired, string bullitDataId)
    {
        WJBullitLv bullitData = DaniTechGameDataManager.Instance.GetWJBullitLvData(bullitDataId);
        SetBullitStat(bullitData);
        _bullitInstId = bullitInstId;
        _createUnitInstId = unitThatFired;
        _meleeSprite.sprite = Resources.Load<Sprite>(bullitData._spritePath);
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(bullitData._animatorPath);
        if (controller != null)
        {
            _meleeAnimator.runtimeAnimatorController = controller;
        }
        else
        {
            Debug.LogError($"근접 공격 AnimatorController 로드 실패: {bullitData._animatorPath}");
        }
        string bullitFuncTypeString = bullitData._bullitfunc;
        _curBullitFunc = Enum.Parse<BullitFnuncType>(bullitFuncTypeString);
    }

    private void SetBullitStat(WJBullitLv bullitData)
    {
        _power = bullitData._power;
        // 근접의 경우 moveSpeed가 넉백 강도임.
        _knockBackPower = bullitData._moveSpeed;
    }

    private void TouchEnemy(Collider2D collision, int attckunitInstId)
    {
        Debug.Log("적과 충돌");

        DamageUnit(collision);
    }

    private void DamageUnit(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<WJ2DUnit>(out WJ2DUnit unit))
        {
            if (unit._instId == _createUnitInstId)
                return;

            if (_hitEnemyIdSet.Contains(unit._instId))
                return;

            _hitEnemyIdSet.Add(unit._instId);

            var damageUnit = WJObjectManager.Inst.GetUnitToUnitList(unit._instId);
            if (damageUnit == null) return;

            damageUnit.DecreaseCurrentHp(_power);
            PushEnemy(_createUnitInstId, unit._instId);
        }
    }

    public void PushEnemy(int attackUnitInstId, int attackedUnitInstId)
    {
        WJ2DUnit attackUnit = WJObjectManager.Inst.GetUnitToUnitList(attackUnitInstId);
        WJ2DUnit attackedUnit = WJObjectManager.Inst.GetUnitToUnitList(attackedUnitInstId);

        if (attackUnit == null || attackedUnit == null) return;

        Vector2 pushDir = attackedUnit.transform.position - attackUnit.transform.position;

        if (pushDir == Vector2.zero)
            pushDir = Vector2.right;

        pushDir.Normalize();

        attackedUnit.transform.position += (Vector3)(pushDir * _knockBackPower);
    }

    private void DestroyBullit()
    {
        transform.localScale = Vector3.one;

        this.gameObject.SetActive(false);
        WJ2DBullitSpawner.Inst.DestroyBullit(_bullitInstId);
    }
}
