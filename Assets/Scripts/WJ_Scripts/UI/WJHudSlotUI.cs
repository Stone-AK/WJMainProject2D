using UnityEngine;
using UnityEngine.UI;

public class WJHudSlotUI : MonoBehaviour
{
    [SerializeField] private Slider _sliderHp;

    private int _instanceId;
    private Transform _targetTransform;

    [SerializeField] private float _extraYOffset = 0.5f;
    private Collider2D _targetCollider;

    public void InitSlot(int instanceId)
    {
        _instanceId = instanceId;
        _targetTransform = WJObjectManager.Inst.GetUnitToUnitList(instanceId).transform;
        TrybindStatChangedEvent(instanceId);
        // 생성 위치 지정
        var unit = WJObjectManager.Inst.GetUnitToUnitList(instanceId);

        if (unit == null)
            return;

        _targetTransform = unit.transform;
        _targetCollider = unit.GetComponent<Collider2D>();

    }

    public void Update()
    {
        if (_targetTransform == null)
            return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentRect = rectTransform.parent as RectTransform;

        Vector3 targetWorldPos = _targetTransform.position;

        if (_targetCollider != null)
        {
            float halfHeight = _targetCollider.bounds.size.y * 0.5f;
            targetWorldPos.y += halfHeight + _extraYOffset;
        }
        else
        {
            targetWorldPos.y += _extraYOffset;
        }

        Vector2 screenPos = Camera.main.WorldToScreenPoint(targetWorldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPos,
            null,
            out Vector2 localPos
        );

        rectTransform.anchoredPosition = localPos;
    }

    private void TrybindStatChangedEvent(int instanceId)
    {
        // 이벤트 구독, 스탯이 변경될 경우 이게 실행이 됨.
        var unit = WJObjectManager.Inst.GetUnitToUnitList(instanceId);

        if (unit != null)
        {
            unit.BindOnStatChangedEvent(OnTargetEntityHpChanged);

            return;
        }
    }

    private void OnTargetEntityHpChanged(int curHp, int maxHp)
    {
        _sliderHp.value = (curHp / (float)maxHp);
    }
}
