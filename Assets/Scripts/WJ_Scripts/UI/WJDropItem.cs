using UnityEngine;

public class WJDropItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _itemSprite;

    [SerializeField] private int _exp;
    [SerializeField] private int _InstId;

    public void InitDropItemStat(int exp, int instId)
    {
        _InstId = instId;
        _exp = exp;
    }
}
