using TMPro;
using UnityEngine;

public class WJGameUI : DaniTechUIBase
{
    [SerializeField] private TMP_Text Timer;
    [SerializeField] private TMP_Text MonsterCount;
    [SerializeField] private WJBtn PauseBtn;

    private void OnEnable()
    {
        PauseBtn.BindOnClickBtn(DaniTechGameManager.Inst.PauseGame);
        PauseBtn.BindOnClickBtn(DaniTechUIManager.Instance.OpenWJPopUpUI);
    }

    public void SetCatchMonsterCount(int monsterCount)
    {
        MonsterCount.text = $"잡은 몬스터 수 : {monsterCount}";
    }
}
