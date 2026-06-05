using System.Collections.Generic;
using UnityEngine;

public class WJItemManager : MonoBehaviour
{
    public static WJItemManager Inst;

    private Transform _liveRootTransform;

    private int _itemInstId;

    private List<WJDropItem> _dropItemPool = new List<WJDropItem>();
    private string _dropItemPrefabPath = "Prefabs/2D/WJ2DDropItem";
    private int _dropItemPollCount = 200;


    private void Awake()
    {
        Inst = this;
        _itemInstId = 0;
    }

    public void SetLiveRootTransform(Transform liveTransform)
    {
        _liveRootTransform = liveTransform;
    }

    public void BackToRobyThenClearAll()
    {
        WJObjectManager.Inst.RemoveAllDropItemList();
        _dropItemPool.Clear();
    }

    private GameObject GetItemPrefab()
    {
        GameObject loadedObj = (GameObject)Resources.Load(_dropItemPrefabPath);
        if (loadedObj == null) return null;
        return loadedObj;
    }

    public void CreateItemPool()
    {
        for (int i = 0; i < _dropItemPollCount; i++)
        {
            GameObject dropItem = Instantiate(GetItemPrefab(), _liveRootTransform);
            dropItem.gameObject.SetActive(false);
            if(dropItem.TryGetComponent<WJDropItem>(out WJDropItem itemInToDropItemPool))
                _dropItemPool.Add(itemInToDropItemPool);
        }
    }

    private GameObject GetDropItemFromPool(float enemyHaveExp)
    {
        foreach (WJDropItem dropItem in _dropItemPool)
        {
            if (dropItem.gameObject.activeSelf == false)
            {
                _itemInstId++;
                dropItem.InitDropItemStat(enemyHaveExp, _itemInstId);
                WJObjectManager.Inst.AddDropItemToDropItemList(_itemInstId, dropItem);
                return dropItem.gameObject;
            }
        }
        return null;
    }

    public void DropTheDropItem(Transform deadEnemyTransform, float InitExp)
    {
        GameObject dropItemObj = GetDropItemFromPool(InitExp);

        dropItemObj.transform.position = deadEnemyTransform.position;
        dropItemObj.transform.rotation = dropItemObj.transform.rotation;
        dropItemObj.SetActive(true);
        return;
    }

}
