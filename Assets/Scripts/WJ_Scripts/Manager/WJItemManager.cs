using System.Collections.Generic;
using UnityEngine;

public class WJItemManager : MonoBehaviour
{
    public static WJItemManager Inst;

    private int _itemInstId;

    private List<WJDropItem> _dropItemPool = new List<WJDropItem>();
    private string _dropItemPrefabPath = "Prefabs/2D/WJ2DDropItem";
    private int _dropItemPollCount = 50;


    private void Awake()
    {
        Inst = this;
        _itemInstId = 0;
    }

    private void Start()
    {
        CreateItemPool();
    }

    public void BackToRobyThenClearAll()
    {
        foreach(var item in _dropItemPool)
        {
            Destroy(item.gameObject);
        }
        WJObjectManager.Inst.RemoveAllDropItemList();
    }

    private GameObject GetItemPrefab()
    {
        GameObject loadedObj = (GameObject)Resources.Load(_dropItemPrefabPath);
        if (loadedObj == null) return null;
        return loadedObj;
    }

    private void CreateItemPool()
    {
        for (int i = 0; i < _dropItemPollCount; i++)
        {
            GameObject dropItem = Instantiate(GetItemPrefab(), this.transform);
            dropItem.gameObject.SetActive(false);
            if(dropItem.TryGetComponent<WJDropItem>(out WJDropItem itemInToDropItemPool))
                _dropItemPool.Add(itemInToDropItemPool);
        }
    }

    private GameObject GetDropItemFromPool(int enemyHaveExp)
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

    public void DropTheDropItem(Transform deadEnemyTransform, int InitExp)
    {
        GameObject dropItemObj = GetDropItemFromPool(InitExp);

        dropItemObj.transform.position = deadEnemyTransform.position;
        dropItemObj.transform.rotation = dropItemObj.transform.rotation;
        dropItemObj.SetActive(true);
        return;
    }

}
