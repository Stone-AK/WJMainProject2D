using System.IO;
using UnityEngine;

public class WJ2DBullit : MonoBehaviour
{
    private int _createUnitInstId;
    private int _bullitInstId;
    private int _power = 10;
    private float _moveSpeed = 4f;
    // 해당 프로퍼티 변수로 고치고 나중에 ID를 통해서 접근 가능하도록 변경
    public float CollTime { get; private set; } = 5f;

    [SerializeField] private SpriteRenderer BullitSprite;

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

    public void InitBullitStat(int bullitInstId, int unitThatFired, string bullitDataId = "Bullit_Base_2")
    {
        WJBullit bullitData = DaniTechGameDataManager.Instance.GetWJBullitObjectData(bullitDataId);
        SetBullitStat(bullitData);
        _bullitInstId = bullitInstId;
        _createUnitInstId = unitThatFired;
        BullitSprite.sprite = Resources.Load<Sprite>(bullitData._spritePath);
    }

    private void SetBullitStat(WJBullit bullitData)
    {
        _power = bullitData._power;
        _moveSpeed = bullitData._moveSpeed;
        CollTime = bullitData._collTime;
    }

    private void MoveBullit()
    {
        transform.position += transform.right * _moveSpeed * Time.deltaTime;
    }

    private void TouchWall()
    {
        Debug.Log("벽과 충돌");


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
