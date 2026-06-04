using System.Collections.Generic;
using UnityEngine;

public class WJObjectManager : MonoBehaviour
{
    public static WJObjectManager Inst;

    private Dictionary<int, WJ2DUnit> _unitList = new Dictionary<int, WJ2DUnit>();   
    private Dictionary<int, WJ2DBullit> _bullitList = new Dictionary<int, WJ2DBullit>();
    private Dictionary<int, WJ2DMeleeAtk> _meleeList = new Dictionary<int, WJ2DMeleeAtk>();
    private Dictionary<int, WJDropItem> _dropItemList = new Dictionary<int, WJDropItem>();

    private void Awake()
    {
        Inst = this;
    }

    public void AddUnitToUnitList(int unitInstNum, WJ2DUnit unitObject)
    {
        _unitList.Add(unitInstNum, unitObject);
    }

    public WJ2DUnit GetUnitToUnitList(int unitInstNum)
    {
        if (_unitList.TryGetValue(unitInstNum, out WJ2DUnit unit))
        {
            return unit;
        }

        return null;
    }

    public void RemoveUnitToUnitList(int unitInstNum)
    {
        if(_unitList.ContainsKey(unitInstNum))
        {
            _unitList.Remove(unitInstNum);
        }
    }

    public void RemoveAllUnitList()
    {
        _unitList.Clear();
    }

    public void AddBullitToBullitList(int bullitInstNum, WJ2DBullit bullitObject)
    {
        _bullitList.Add(bullitInstNum, bullitObject);
    }

    public WJ2DBullit GetBullitToBullitList(int bullitInstNum)
    {
        return _bullitList[bullitInstNum];
    }

    public void RemoveBullitToBullitList(int bullitInstNum)
    {
        if(_bullitList.ContainsKey(bullitInstNum))
        {
            _bullitList.Remove(bullitInstNum);
        }
    }

    public void RemoveAllBullitList()
    {
        _bullitList.Clear();
    }

    public void AddMeleeAtkToMeleeAtkList(int dropItemInstNum, WJ2DMeleeAtk dropItemObject)
    {
        _meleeList.Add(dropItemInstNum, dropItemObject);
    }

    public WJ2DMeleeAtk GetMeleeAtkToMeleeAtkList(int dropItemInstNum)
    {
        return _meleeList[dropItemInstNum];
    }

    public void RemoveMeleeAtkToMeleeAtkList(int bullitInstNum)
    {
        if (_meleeList.ContainsKey(bullitInstNum))
        {
            _meleeList.Remove(bullitInstNum);
        }
    }

    public void RemoveAllMeleeAtkList()
    {
        _meleeList.Clear();
    }

    public void AddDropItemToDropItemList(int dropItemInstNum, WJDropItem dropItemObject)
    {
        _dropItemList.Add(dropItemInstNum, dropItemObject);
    }

    public WJDropItem GetDropItemToDropItemList(int dropItemInstNum)
    {
        return _dropItemList[dropItemInstNum];
    }

    public void RemoveDropItemToDropItemList(int bullitInstNum)
    {
        if (_dropItemList.ContainsKey(bullitInstNum))
        {
            _dropItemList.Remove(bullitInstNum);
        }
    }

    public void RemoveAllDropItemList()
    {
        _meleeList.Clear();
    }

    
}
