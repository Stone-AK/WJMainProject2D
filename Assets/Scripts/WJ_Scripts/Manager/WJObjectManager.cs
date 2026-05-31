using System.Collections.Generic;
using UnityEngine;

public class WJObjectManager : MonoBehaviour
{
    public static WJObjectManager Inst;

    private Dictionary<int, WJ2DUnit> _unitList = new Dictionary<int, WJ2DUnit>();   
    private Dictionary<int, WJ2DBullit> _bullitList = new Dictionary<int, WJ2DBullit>();
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
        return _unitList[unitInstNum];
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
        _dropItemList.Clear();
    }
}
