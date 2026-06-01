using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WJLevelUpPopUpUI : DaniTechUIBase
{
    [SerializeField] private List<WJLvUpSlotBtn> _chooseSlotList = new List<WJLvUpSlotBtn>();



    public void SetSlot(Dictionary<string, int> lvUpAbleBullitList)
    {
        int a = 0;

        foreach(var dataAndLevelPair in lvUpAbleBullitList)
        {
            string dataId = dataAndLevelPair.Key;
            int nextLv = dataAndLevelPair.Value;

            var data = DaniTechGameDataManager.Instance.GetWJBullitObjectData(dataId);
            if (data == null) continue;

            string iconPath = data._bullitIconPath;
            string title = data._bullitName;
            string lvText = data._bullitShowUILv[nextLv];
            string comment = data._bullitDecription;

            SetSlotInLevelUpUI(_chooseSlotList[a], iconPath, title, lvText, comment);
            a++;
        }

        if (a < 3)
        {
            for(int i = a; i <= 3; i++)
            {
                SetSlotInLevelUpUI(_chooseSlotList[i]);
            }
        }
    }

    private void SetSlotInLevelUpUI(
        WJLvUpSlotBtn slotBtn,
        string iconPath = "Icon/CancelIcon",
        string title = "",
        string lvText = "",
        string comment = "더 이상 업그레이드 할게 없습니다.")
    {
        slotBtn.SetSlotImg(iconPath);
        slotBtn.SetSlotTitle(title);
        slotBtn.SetSlotBullitLv(lvText);
        slotBtn.SetSlotComment(comment);
    }
}
