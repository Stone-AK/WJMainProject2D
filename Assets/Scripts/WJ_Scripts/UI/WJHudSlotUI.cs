using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class WJHudSlotUI : MonoBehaviour
{
    [SerializeField] private Slider _sliderHp;

    private int _instanceId;
    private Transform _targetTransform;


    public void InitSlot(int instanceId)
    {
        _instanceId = instanceId;
        _targetTransform = WJObjectManager.Inst.GetUnitToUnitList(instanceId).transform;
    }

    public void Update()
    {
        if(_targetTransform != null)
        {
            this.gameObject.transform.position = _targetTransform.position;

            // GUI을 월드로 변환하는 방식
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransform.position);

            // canvas에서가 아닌 오브젝트 영역에서 보이기 위해서는 rectTransform으로 변환
            var rectTransfrom = this.GetComponent<RectTransform>();
            if (rectTransfrom != null)
            {
                rectTransfrom.anchoredPosition = screenPos;
            }

        }
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
        _sliderHp.value = (curHp / maxHp);
    }
}
