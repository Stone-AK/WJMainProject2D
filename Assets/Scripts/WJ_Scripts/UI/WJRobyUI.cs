using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WJRobyUI : DaniTechUIBase
{
    [SerializeField] private WJBtn StartBtn;
    [SerializeField] private WJBtn EndBtn;
    [SerializeField] private Image CharactorImg;
    [SerializeField] private TMP_Text StageInfoTxt;


    private void OnEnable()
    {
        StartBtn.BindOnClickBtn(OnClickStartBtn);
        EndBtn.BindOnClickBtn(OnClickEndBtn);
        
    }

    private void Start()
    {
        
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
}
