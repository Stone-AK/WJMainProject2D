using TMPro;
using UnityEngine;

public class WJGameEndPopUp : DaniTechUIBase
{
    [SerializeField] private TMP_Text GameEndMessage;
    [SerializeField] private WJBtn BackRobyBtn;

    private void OnEnable()
    {
        BackRobyBtn.BindOnClickBtn(OnClickBackRoby);
    }

    private void OnClickBackRoby()
    {
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.WJGameEndPopUp);
        DaniTechGameManager.Inst.BackToRoby();
    }

    public void SetGemeEndStatus(WJ2DGameStat gameStat)
    {
        switch(gameStat)
        {
            case WJ2DGameStat.Clear:
                GameEndMessage.text = "게임 클리어";
                break;
            case WJ2DGameStat.Over:
                GameEndMessage.text = "게임 오버";
                break;
        }
    }
}
