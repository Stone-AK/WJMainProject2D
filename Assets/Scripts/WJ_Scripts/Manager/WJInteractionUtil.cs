using System;
using System.Collections.Generic;
using UnityEngine;

public static class WJInteractionUtil
{ 
    public static void CatchExpInteraction(int dropExpInstId, int unitInstId)
    {
        WJDropItem dropItem = WJObjectManager.Inst.GetDropItemToDropItemList(dropExpInstId);
        if (dropItem == null) return;
        WJ2DUnit catchingUnit = WJObjectManager.Inst.GetUnitToUnitList(unitInstId);
        if (catchingUnit == null) return;

        if(catchingUnit is WJ2DPlayer player)
        {
            player.IncreaseExp(dropItem._exp);
        }
    }
}
