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
    [SerializeField] private WJBtn StartBtn;
    [SerializeField] private WJBtn EndBtn;
    [SerializeField] private Image CharactorImg;
    [SerializeField] private TMP_Text StageInfoTxt;
    [SerializeField] private WJBtn NextStageBtn;
    [SerializeField] private WJBtn BeforeStageBtn;

    private void OnEnable()
    {
        StartBtn.BindOnClickBtn(OnClickStartBtn);
        EndBtn.BindOnClickBtn(OnClickEndBtn);
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
        StageInfoTxt.text = $"{stageInfo}";
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
                NextStageBtn.gameObject.SetActive(false);
                break;
            case WJRobyBtnActiveType.BeforeBtnDeActive:
                BeforeStageBtn.gameObject.SetActive(false);
                break;
            case WJRobyBtnActiveType.NextBtnActive:
                NextStageBtn.gameObject.SetActive(true);
                NextStageBtn.BindOnClickBtnOnlyOne(OnClickNextStageBtn);
                break;
            case WJRobyBtnActiveType.BeforeBtnActive:
                BeforeStageBtn.gameObject.SetActive(true);
                BeforeStageBtn.BindOnClickBtnOnlyOne(OnClickBeforeStageBtn);
                break;
        }
    }
}
