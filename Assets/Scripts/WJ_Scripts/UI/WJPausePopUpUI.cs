using UnityEngine;

public class WJPausePopUpUI : DaniTechUIBase
{
    [SerializeField] private WJBtn BackGroundCloseUIBtn;
    [SerializeField] private WJBtn ContinueBtn;
    [SerializeField] private WJBtn BackRobyBtn;

    private void OnEnable()
    {
        BackGroundCloseUIBtn.BindOnClickBtn(OnClickRsumeGame);
        ContinueBtn.BindOnClickBtn(OnClickRsumeGame);
        BackRobyBtn.BindOnClickBtn(OnClickBackRoby);
    }

    private void OnClickRsumeGame()
    {
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.WJPausePopUpUI);
        DaniTechGameManager.Inst.ResumeGame();
    }

    private void OnClickBackRoby()
    {
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.WJPausePopUpUI);
        DaniTechGameManager.Inst.BackToRoby();
        DaniTechGameManager.Inst.ResumeGame();
    }
}
