using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
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
        SetGameStat(WJ2DGameStat.Roby);
        LoadStageInfo();
        InitStageRecord();
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
    [SerializeField] private Transform LiveRoot;
    [SerializeField] private WJ2DPlayer PlayerObject;
    [SerializeField] private WJ2DGameStat CurGameStat;


    private WJStage _curStageData;
    private WJWave _curWaveData;

    private int _enemyCatchCount;

    private float _gameMaxSecond;
    private float _gameCurSecond;
    private float _gameNextWaveSecond;

    private int _clearStage;
    private int _maxtStage;
    private Dictionary<int,int> _stageClearCatchEnemyCount;

    private int _curWaveCount = 0;

    private string _stageName;
    private string _waveName;

    // BullitDataId(string), 다음 레벨(int)인 Dictionary. 최대 레벨인 Bullit 종류는 제외
    private Dictionary<string, int> _lvUpAbleBullitList = new Dictionary<string, int>();
    private Dictionary<string, int> _forDeliverBullitList = new Dictionary<string, int>();

    private void Update()
    {
        switch(CurGameStat)
        {
            case WJ2DGameStat.Start:
                DecreaseTime();
                CheckTime();
                DaniTechUIManager.Instance.SetGameUITimeToUIManager(_gameCurSecond);
                break;
        }
    }

    public void StartGame()
    {
        GameObject liveRoot = new GameObject("LiveRoot");
        SetGameStat(WJ2DGameStat.Start);
        liveRoot.transform.SetParent(ReturnGameRootTransfrom());
        liveRoot.transform.localPosition = Vector3.zero;
        liveRoot.transform.localRotation = Quaternion.identity;
        liveRoot.transform.localScale = Vector3.one;
        SetLiveRoot(liveRoot.transform);
        // 게임 시작 시 만들 것들을 할당
        MakeMap();
        MakePlayer();
        MakeEnemySpawner();
        MakeBulitSpawner();
        InitCatchEnemyCount();
        DaniTechUIManager.Instance.AddHudSlot(0);
        InitPlayerBullitList();
        // stage, wave 정보 초기화 설정
        if (_stageName == "스테이지 1")
            LoadStageInfo();
        else
            LoadStageInfo(_curStageData.Id);
        InitCurTime();
    }

    public void LoadStageInfo(string stageId = "Stage_1")
    {
        _curStageData = DaniTechGameDataManager.Instance.GetWJStageData(stageId);

        if (_curStageData != null)
        {
            _gameMaxSecond = _curStageData.StageLimitTime;
            string waveId = _curStageData.WaveDataIdList[_curWaveCount];
            LoadWaveInfo(waveId);
            LoadStageName();
        }

        if (CurGameStat == WJ2DGameStat.Roby)
            CheckStageBtnActivate();
    }

    public void LoadWaveInfo(string waveId)
    {
        if (CurGameStat == WJ2DGameStat.Roby) return;

        _curWaveData = DaniTechGameDataManager.Instance.GetWJWaveData(waveId);
        if(_curWaveCount + 1 < _curStageData.WaveDataIdList.Count)
        {
            var nextWaveId = _curStageData.WaveDataIdList[_curWaveCount + 1];
            _gameNextWaveSecond = DaniTechGameDataManager.Instance.GetWJWaveData(nextWaveId).StartRemainTime;
        }
        else
        {
            _gameNextWaveSecond = -1f;
        }
        WJ2DEnemySpawner.Inst.SetSpwanerWave(waveId);
        LoadWaveName();
        _curWaveCount++;
    }

    private void LoadStageName()
    {
        _stageName = _curStageData.Name;
        DaniTechUIManager.Instance.SetStageUIToUIManager(_stageName);
    }

    private void LoadWaveName()
    {
        _waveName = _curWaveData.Name;
        DaniTechUIManager.Instance.SetWaveToUIManager(_waveName);
    }

    public void CheckTime()
    {
        if (_gameNextWaveSecond < 0)
            return;

        if (_gameCurSecond <= _gameNextWaveSecond)
        {
            string waveId = _curStageData.WaveDataIdList[_curWaveCount];
            LoadWaveInfo(waveId);
        }
    }

    public void InitCurTime()
    {
        _gameCurSecond = _gameMaxSecond;
    }

    private void DecreaseTime()
    {
        _gameCurSecond -= Time.deltaTime;

        if(_gameCurSecond <= 0)
        {
            EndGameOnClear();
        }
    }

    public Transform ReturnGameRootTransfrom()
    {
        return GameRoot.transform;
    }

    public void SetLiveRoot(Transform root)
    {
        LiveRoot = root;
    }

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

    public void EndGame()
    {
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    private Transform GetRootTransform(WJObjectRootType objRootType)
    {
        Transform root = null;
        switch (objRootType)
        {
            case WJObjectRootType.None:
                root = LiveRoot;
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
        // 플레이어 인스턴스Id 0으로 하드코딩
        PlayerObject.InitStat(0);
        WJObjectManager.Inst.AddUnitToUnitList(0, PlayerObject);
    }

    public void InitPlayerBullitList()
    {
        PlayerObject.InitHaveBullitList();
    }

    public void MakeEnemySpawner()
    {
        CreateObj(WJObjectRootType.None, WJObjectType.WJEnemySpawner);
    }

    public void MakeBulitSpawner()
    {
        CreateObj(WJObjectRootType.None, WJObjectType.WJ2DBullitSpawner);
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

    public void PauseGame()
    {
        SetGameStat(WJ2DGameStat.Pause);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        SetGameStat(WJ2DGameStat.Start);
        Time.timeScale = 1f;
    }

    public void BackToRoby()
    {
        _curWaveCount = 0;
        if(CurGameStat == WJ2DGameStat.Clear)
        {
            SetGameStat(WJ2DGameStat.Roby);
            ClearGameThenRenewStage();
        }
        else
        {
            SetGameStat(WJ2DGameStat.Roby);
        }
        Destroy(LiveRoot.gameObject);
        DaniTechUIManager.Instance.CloseWJGameUI();
        DaniTechUIManager.Instance.OpenWJRobyUI();
        Time.timeScale = 1f;
        DaniTechUIManager.Instance.ResetHudSlot();
        CheckStageBtnActivate();
    }

    public void EndGameOnOver()
    {
        Time.timeScale = 0f;
        SetGameStat(WJ2DGameStat.Over);
        DaniTechUIManager.Instance.OpenWJGameEndPopUpUI(CurGameStat);
    }

    public void EndGameOnClear()
    {
        Time.timeScale = 0f;
        SetGameStat(WJ2DGameStat.Clear);
        DaniTechUIManager.Instance.OpenWJGameEndPopUpUI(CurGameStat);
    }

    public void ClearGameThenRenewStage()
    {
        string nextStageId = _curStageData.NextStageId;
        if (_clearStage < _curStageData.StageCount)
        {
            _clearStage = _curStageData.StageCount;
        }
        if(nextStageId == "" )
        {
            return;
        }
        LoadStageInfo(nextStageId);
    }

    public void InitCatchEnemyCount()
    {
        _enemyCatchCount = 0;
    }

    public void IncreasCatchEnemyCount()
    {
        _enemyCatchCount++;
        DaniTechUIManager.Instance.SetGameUITextToUIManager();
    }

    public int GetEnemyCatchCount()
    {
        return _enemyCatchCount;
    }

    public void SetGameStat(WJ2DGameStat curGameStat)
    {
        CurGameStat = curGameStat;
    }

    private void InitStageRecord(int clearStageNo = 0)
    {
        _clearStage = clearStageNo;
        _maxtStage = DaniTechGameDataManager.Instance.WJStageDataList.Count;
    }

    public void NextStageSetting()
    {
        string nextStageId = _curStageData.NextStageId;
        if(nextStageId == null) return;

        LoadStageInfo(nextStageId);
    }

    public void BeforeStageSetting()
    {
        string beforeStageId = _curStageData.BeforeStageId;
        if (beforeStageId == null) return;

        LoadStageInfo(beforeStageId);
    }

    public void CheckStageBtnActivate()
    {
        if ((_curStageData.NextStageId == "") || (_curStageData.StageCount > _clearStage))
        {
            DaniTechUIManager.Instance.WJRobyBtnActiveSetting(WJRobyBtnActiveType.NextBtnDeActive);
        }
        else
        {
            DaniTechUIManager.Instance.WJRobyBtnActiveSetting(WJRobyBtnActiveType.NextBtnActive);
        }

        if (_curStageData.BeforeStageId == "")
        {
            DaniTechUIManager.Instance.WJRobyBtnActiveSetting(WJRobyBtnActiveType.BeforeBtnDeActive);
        }
        else
        {
            DaniTechUIManager.Instance.WJRobyBtnActiveSetting(WJRobyBtnActiveType.BeforeBtnActive);
        }
    }

    public void LvUpChoosePhase()
    {
        Time.timeScale = 0f;
        _forDeliverBullitList.Clear();

        int lvUpAbleeBullitCount = _lvUpAbleBullitList.Count;

        if(lvUpAbleeBullitCount < 3)
        {
            foreach(var deliverData in _lvUpAbleBullitList)
            {
                string deliverDataId = deliverData.Key;
                int deliverpPlayerHaveLv = deliverData.Value;

                _forDeliverBullitList.Add(deliverDataId, deliverpPlayerHaveLv);
            }
        }

        RandomOutPutBullitData(lvUpAbleeBullitCount);
        

        DaniTechUIManager.Instance.OpenLvUpPopUp(_forDeliverBullitList);
    }

    public void RenewLvUpChooseList(Dictionary<string, int> playerHaveBullitLvList)
    {
        _lvUpAbleBullitList.Clear();
        foreach (WJBullit bullitData in DaniTechGameDataManager.Instance.WJBullitDataList.Values)
        {
            string bullitDataId = bullitData.Id;

            if (playerHaveBullitLvList.ContainsKey(bullitDataId))
            {
                // 현재 가지고 있는 bullit의 레벨이 최대치인지 알기 위해 최댓값 2에 리스트의 사이즈와 같은지 체크하기
                // 위해서 +1해줌. 또한 다음 레벨 값을 저장하기 위해서 +1을 해줘야했음
                int haveBullitLvMaxChecking = (playerHaveBullitLvList[bullitDataId] + 1);
                if (bullitData._bullitLvList.Count <= haveBullitLvMaxChecking)
                    continue;
                _lvUpAbleBullitList.Add(bullitDataId, haveBullitLvMaxChecking);
            }
            else
            {
                _lvUpAbleBullitList.Add(bullitDataId, 0);
            }
        }
    }

    private void RandomOutPutBullitData(int maxValue)
    {
        HashSet<int> randomNumbers = new HashSet<int>();

        while (randomNumbers.Count < 3)
        {
            randomNumbers.Add(Random.Range(0, maxValue));
        }

        foreach(int num in randomNumbers)
        {
            var data = _lvUpAbleBullitList.ElementAt(num);

            _forDeliverBullitList.Add(data.Key, data.Value);
        }
    }
}
