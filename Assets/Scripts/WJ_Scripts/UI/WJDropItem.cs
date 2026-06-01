using UnityEngine;

public class WJDropItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _itemSprite;

    public float _exp { get; private set;}
    [SerializeField] private int _InstId;

    public void InitDropItemStat(float exp, int instId)
    {
        _InstId = instId;
        _exp = exp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 실행될 메서드 호출
            if (collision.TryGetComponent<WJ2DUnit>(out WJ2DUnit catchingPlayer))
            {
                CatchedToPlayer(catchingPlayer);

            }
        }
    }

    private void CatchedToPlayer(WJ2DUnit catchingPlayer)
    {
        WJInteractionUtil.CatchExpInteraction(_InstId, catchingPlayer._instId);
        FinishGetItem();
    }

    private void FinishGetItem()
    {
        // 파괴, 리스트 삭제 등
        WJObjectManager.Inst.RemoveDropItemToDropItemList(_InstId);
        this.gameObject.SetActive(false);
    }
}
