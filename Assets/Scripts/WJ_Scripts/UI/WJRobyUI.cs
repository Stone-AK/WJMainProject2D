using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WJRobyBtnActiveType
{
    None,
    NextBtnActive,
    NextBtnDeActive,
    BeforeBtnActive,
    BeforeBtnDeActive
}

public class WJRobyUI : DaniTechUIBase
{
    [SerializeField] private WJBtn _startBtn;
    [SerializeField] private WJBtn _endBtn;
    [SerializeField] private Image _charactorImg;
    [SerializeField] private TMP_Text _stageInfoTxt;
    [SerializeField] private WJBtn _nextStageBtn;
    [SerializeField] private WJBtn _beforeStageBtn;
    [SerializeField] private WJBtn _saveBtn;
    [SerializeField] private WJBtn _loadBtn;

    private void OnEnable()
    {
        _startBtn.BindOnClickBtn(OnClickStartBtn);
        _endBtn.BindOnClickBtn(OnClickEndBtn);
        _saveBtn.BindOnClickBtn(OnClickSaveBtn);
        _loadBtn.BindOnClickBtn(OnClickLoadBtn);
    }

    private void OnClickStartBtn()
    {
        Debug.Log("게임 시작이 눌렸습니다.");
        DaniTechUIManager.Instance.CloseWJRobyUI();
        DaniTechGameManager.Inst.StartGame();
        DaniTechUIManager.Instance.OpenWJGameUI();
    }

    private void OnClickEndBtn()
    {
        Debug.Log("종료버튼이 눌렸습니다.");
        DaniTechGameManager.Inst.EndGame();
    }

    public void PrintStageInfo(string stageInfo)
    {
        _stageInfoTxt.text = $"{stageInfo}";
    }

    private void OnClickNextStageBtn()
    {
        DaniTechGameManager.Inst.NextStageSetting();
    }

    private void OnClickBeforeStageBtn()
    {
        DaniTechGameManager.Inst.BeforeStageSetting();
    }

    public void SettingActiveBtn(WJRobyBtnActiveType setActiveBtnType)
    {
        switch(setActiveBtnType)
        {
            case WJRobyBtnActiveType.NextBtnDeActive:
                _nextStageBtn.gameObject.SetActive(false);
                break;
            case WJRobyBtnActiveType.BeforeBtnDeActive:
                _beforeStageBtn.gameObject.SetActive(false);
                break;
            case WJRobyBtnActiveType.NextBtnActive:
                _nextStageBtn.gameObject.SetActive(true);
                _nextStageBtn.BindOnClickBtnOnlyOne(OnClickNextStageBtn);
                break;
            case WJRobyBtnActiveType.BeforeBtnActive:
                _beforeStageBtn.gameObject.SetActive(true);
                _beforeStageBtn.BindOnClickBtnOnlyOne(OnClickBeforeStageBtn);
                break;
        }
    }

    public void OnClickSaveBtn()
    {
        DaniTechGameManager.Inst.SaveWJPlayerData();
    }

    public void OnClickLoadBtn()
    {
        DaniTechGameManager.Inst.LoadWJPlayerData();
    }
}
