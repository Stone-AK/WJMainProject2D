using System.Collections.Generic;
using UnityEngine;

public class WJObjectManager : MonoBehaviour
{
    public static WJObjectManager Inst;

    private Dictionary<int, WJ2DUnit> _unitList = new Dictionary<int, WJ2DUnit>();   
    private Dictionary<int, WJ2DBullit> _bullitList = new Dictionary<int, WJ2DBullit>();

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
}
