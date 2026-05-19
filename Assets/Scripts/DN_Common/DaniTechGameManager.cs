using System.Collections.Generic;
using UnityEngine;

public class DaniTechGameManager : MonoBehaviour
{
    public static DaniTechGameManager Inst { get; set; }

    // 플레이 중에 저장되어야 하는 정보들이 있는 위치
    private DaniTechPlayerModel _playerModel = new DaniTechPlayerModel();

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        // LoadSaveData();

        GameObject rootObj = GameObject.Find("GameRoot");

        if (rootObj != null)
        {
            GameRoot = rootObj;
        }
        else
        {
            Debug.LogError("GameRoot를 찾지 못함");
        }
    }

    public void SaveData()
    {
        DaniTechNetworkManager.Inst.RequstSaveData(_playerModel);
    }

    public void SaveAndEndGame()
    {
        SaveData();
        Application.Quit();
    }

    private void LoadSaveData()
    {
        _playerModel = DaniTechNetworkManager.Inst.RequstLoadSaveData();
    }

    public void IncreasePlayerExp(int exp)
    {
        // 추후에 한곳에서 관리할 수 있게 익스텐션으로 빼도 된다
        _playerModel.PlayerTotalExp += exp;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        // 저장할때 고유값 ID를 부여하기 위해 사용
        long uniqueId = DaniTechGameUtil.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new DaniTechItemModel();
        newItem.ItemUniqueId = uniqueId;
        newItem.ItemDataId = itemDataId;
        newItem.ItemStackCount = addItemCount;

        _playerModel.ItemList.Add(newItem);
    }

    public List<DaniTechItemModel> GetPlayerItemList()
    {
        // _playerModel이 Private이므로 외부에서 ItemList를 받아올 수 있게 Get함수를 사용한다
        return _playerModel.ItemList;
    }

    // #################################################################################################################

    // Start에서 Scene에 있는 GameRoot(위치 잡기용)을 찾도록(find)하도록 해놓았음
    [SerializeField] private GameObject GameRoot;
    [SerializeField] private WJ2DPlayer PlayerObject;

    private void CreateObj(WJObjectRootType objectRootType, WJObjectType objectType)
    {
        string path = this.Get2DWJPath(objectType, objectRootType);
        GameObject loadedObj = (GameObject)Resources.Load(path);
        Transform root = GetRootTransform(objectRootType);
        GameObject gObj = Instantiate(loadedObj, root);

        if (gObj.gameObject.TryGetComponent<WJ2DPlayer>(out WJ2DPlayer player))
        {
            PlayerObject = player;
        }
    }

    private Transform GetRootTransform(WJObjectRootType objRootType)
    {
        Transform root = null;
        switch (objRootType)
        {
            case WJObjectRootType.None:
                root = GameRoot.transform;
                break;
        }
        return root;
    }

    public void MakeMap()
    {
        CreateObj(WJObjectRootType.None, WJObjectType.WJ2DMap);
    }

    public void MakePlayer()
    {
        CreateObj(WJObjectRootType.None, WJObjectType.WJ2DPlayer);
    }

    public void MakeEnemySpawner()
    {
        CreateObj(WJObjectRootType.None, WJObjectType.WJEnemySpawner);
    }


    public Transform ReturnPlayerTransform()
    {
        if(PlayerObject == null)
        {
            Debug.LogError("플레이어가 할당되어 있지 않습니다.");
            return null;
        }
        Transform playerTransform = PlayerObject.transform;
        return playerTransform;
    }

    
}
