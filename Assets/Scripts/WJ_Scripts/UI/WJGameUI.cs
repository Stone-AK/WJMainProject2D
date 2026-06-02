using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WJGameUI : DaniTechUIBase
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _monsterCount;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private WJBtn _pauseBtn;
    [SerializeField] private Slider _expBar;

    private void OnEnable()
    {
        _pauseBtn.BindOnClickBtn(DaniTechGameManager.Inst.PauseGame);
        _pauseBtn.BindOnClickBtn(DaniTechUIManager.Instance.OpenWJPopUpUI);
        SetCatchMonsterCount(0);
    }

    public void SetCatchMonsterCount(int monsterCount)
    {
        _monsterCount.text = $"잡은 몬스터 수 : {monsterCount}";
    }

    public void SetTextTimer(float curTime)
    {
        int minute = (int)curTime / 60;
        int second = (int)curTime % 60;
        _timer.text = $"{minute:00} : {second:00}";
    }

    public void SetWaveText(string waveName)
    {
        _waveText.text = $"{waveName}";
    }

    public void OnTargetEntityHpChanged(float curExp, float maxExp)
    {
        _expBar.value = (curExp / maxExp);
    }
}
