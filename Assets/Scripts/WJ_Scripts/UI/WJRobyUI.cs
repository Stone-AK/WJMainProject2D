using UnityEngine;
using UnityEngine.UI;

public class WJRobyUI : DaniTechUIBase
{
    [SerializeField] private WJBtn StartBtn;
    [SerializeField] private WJBtn EndBtn;
    [SerializeField] private Image CharactorImg;

    private void Start()
    {
        StartBtn.BindOnClickBtn(OnClickStartBtn);
        EndBtn.BindOnClickBtn(OnClickEndBtn);
    }
    
    private void OnClickStartBtn()
    {
        Debug.Log("게임 시작이 눌렸습니다.");
        DaniTechUIManager.Instance.CloseWJRobyUI();
        DaniTechGameManager.Inst.StartGame();
    }

    private void OnClickEndBtn()
    {
        Debug.Log("종료버튼이 눌렸습니다.");
    }
}
