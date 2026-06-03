using TMPro;
using UnityEngine;
public enum WJResultSaveAndLoad
{
    None,
    SaveFail,
    SaveSuccess,
    LoadFail,
    LoadSuccess
}

public class WJSavLoadResultPopUp : DaniTechUIBase
{
    [SerializeField] private TMP_Text _resultSaveOrLoad;
    [SerializeField] private WJBtn CloseBtn;

    private void OnEnable()
    {
        CloseBtn.BindOnClickBtn(OnClickConfirmBtn);
    }

    private void OnClickConfirmBtn()
    {
        DaniTechUIManager.Instance.CloseUI(DaniTechUIRootType.PopupUI, DaniTechUIType.WJSaveAndLoadResultPopUp);
    }

    public void SetGemeEndStatus(WJResultSaveAndLoad resultStat)
    {
        switch (resultStat)
        {
            case WJResultSaveAndLoad.SaveFail:
                _resultSaveOrLoad.text = "세이브 실패";
                break;
            case WJResultSaveAndLoad.SaveSuccess:
                _resultSaveOrLoad.text = "세이브 성공";
                break;
            case WJResultSaveAndLoad.LoadFail:
                _resultSaveOrLoad.text = "로드 실패";
                break;
            case WJResultSaveAndLoad.LoadSuccess:
                _resultSaveOrLoad.text = "로드 성공";
                break;
        }
    }
}
