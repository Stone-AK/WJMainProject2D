using System.Collections.Generic;
using UnityEngine;

public class WJHudUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_HudSlot;
    [SerializeField] private Transform Transform_SlotRoot;

    private Dictionary<int, WJHudSlotUI> _hudSlotList = new Dictionary<int, WJHudSlotUI>();

    public void AddHudSlotOnWJHudUI(int instancId)
    {
        CreateHudSlot(instancId);
    }

    private void CreateHudSlot(int instancId)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotcomponet = gObj.GetComponent<WJHudSlotUI>();
        if(slotcomponet == null) return;

        slotcomponet.InitSlot(instancId);
        _hudSlotList.Add(instancId, slotcomponet);
    }

    public void RemoveHudSlotOnWJHudUI(int instancId)
    {
        if(_hudSlotList.ContainsKey(instancId) == true)
        {
            var slot = _hudSlotList[instancId];
            Destroy(slot.gameObject);
            _hudSlotList.Remove(instancId);
        }
    }
}
