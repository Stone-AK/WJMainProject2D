using System;
using System.IO;
using UnityEngine;

public enum BullitFnuncType
{
    None,
    Basic,
    DoubleFiring,
    TripleFiring
}

public class WJ2DBullit : MonoBehaviour
{
    private int _createUnitInstId;
    private int _bullitInstId;
    private int _power;
    private float _moveSpeed;
    [SerializeField] private SpriteRenderer BullitSprite;
    private BullitFnuncType _curBullitFunc;

    private void FixedUpdate()
    {
        MoveBullit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Map_Wall"))
        {
            TouchWall();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TouchEnemy(collision);
        }
    }

    public void InitBullitStat(int bullitInstId, int unitThatFired, string bullitDataId)
    {
        WJBullitLv bullitData = DaniTechGameDataManager.Instance.GetWJBullitLvData(bullitDataId);
        SetBullitStat(bullitData);
        _bullitInstId = bullitInstId;
        _createUnitInstId = unitThatFired;
        BullitSprite.sprite = Resources.Load<Sprite>(bullitData._spritePath);
        string bullitFuncTypeString = bullitData._bullitfunc;
        _curBullitFunc = Enum.Parse<BullitFnuncType>(bullitFuncTypeString);
    }

    private void SetBullitStat(WJBullitLv bullitData)
    {
        _power = bullitData._power;
        _moveSpeed = bullitData._moveSpeed;
    }

    private void MoveBullit()
    {
        transform.position += transform.right * _moveSpeed * Time.deltaTime;
    }

    private void TouchWall()
    {
        DestroyBullit();
    }

    private void TouchEnemy(Collider2D collision)
    {
        Debug.Log("적과 충돌");

        DamageUnit(collision);
        DestroyBullit();
    }

    private void DamageUnit(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<WJ2DUnit>(out WJ2DUnit unit))
        {
            if(unit._instId != _createUnitInstId)
            {
                var damageUnit = WJObjectManager.Inst.GetUnitToUnitList(unit._instId);
                if (damageUnit == null) return;

                damageUnit.DecreaseCurrentHp(_power);
            }
        }
    }

    private void DestroyBullit()
    {
        this.gameObject.SetActive(false);
        WJ2DBullitSpawner.Inst.DestroyBullit(_bullitInstId);
    }
}
