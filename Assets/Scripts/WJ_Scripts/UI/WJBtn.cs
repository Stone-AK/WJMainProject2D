using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WJBtn : MonoBehaviour
{
    [SerializeField] private Button Btn_Base;
    [SerializeField] private TMP_Text Txt_Base;
    [SerializeField] private Image Img_Base;

    private void Awake()
    {
        InitUIBtn();
    }

    private void OnDisable()
    {
        Btn_Base.onClick.RemoveAllListeners();
    }

    private void InitUIBtn()
    {
        if (Btn_Base != null)
        {
            return;
        }

        var button = this.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this.Btn_Base = button;
        }
    }

    private string GetHierarchyPath(Transform target)
    {
        string path = target.name;

        while (target.parent != null)
        {
            target = target.parent;
            path = target.name + "/" + path;
        }

        return path;
    }

    public void BindOnClickBtn(Action onClickAction)
    {
        if(Btn_Base ==  null)
        {
            Debug.LogError($"{gameObject.name}의 Btn_Base가 null 입니다. 경로: {GetHierarchyPath(transform)}");
            return;
        }

        // C#의 Action(System.Action)에서 UnityAction으로의 형변환
        Btn_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickAction));
    }

    public void UnBindOnClickBtn(Action onClickAction)
    {
        if (Btn_Base == null)
        {
            Debug.LogError($"{this.gameObject.name}의 버튼이 null 입니다.(UnBindOnClickBtn애서 발생)");
            return;
        }

        // C#의 Action(System.Action)에서 UnityAction으로의 형변환
        Btn_Base.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickAction));
    }

    public void ChangeButtonText(string buttonStr)
    {
        if (Txt_Base == null) return;

        Txt_Base.text = buttonStr;
    }
}
