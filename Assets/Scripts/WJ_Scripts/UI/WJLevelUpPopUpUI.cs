using UnityEngine;

public class WJLevelUpPopUpUI : DaniTechUIBase
{
    [SerializeField] private WJLvUpSlotBtn SlotBtnOne;

    public void SetSlot(string imgPath, string title, string lvTxt, string comment)
    {
        SlotBtnOne.SetSlotImg(imgPath);
        SlotBtnOne.SetSlotTitle(title);
        SlotBtnOne.SetSlotBullitLv(lvTxt);
        SlotBtnOne.SetSlotComment(comment);
    }
}
