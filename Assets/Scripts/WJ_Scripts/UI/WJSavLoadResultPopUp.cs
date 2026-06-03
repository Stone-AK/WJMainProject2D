using TMPro;
using UnityEngine;

public class WJSavLoadResultPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text _resultSaveOrLoad;
    [SerializeField] private WJBtn CloseBtn;

    private void OnEnable()
    {
        CloseBtn.BindOnClickBtn(OnClickBackRoby);
    }

    private void OnClickBackRoby()
    {
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.WJSaveAndLoadResultPopUp);
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
