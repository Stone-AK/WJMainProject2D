using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WJLvUpSlotBtn : MonoBehaviour
{
    private Button Btn_Base;
    [SerializeField] private TMP_Text Txt_ChooseTitle;
    [SerializeField] private TMP_Text Txt_ChooseBullitLv;
    [SerializeField] private TMP_Text Txt_ChooseComment;
    [SerializeField] private Image Img_ChooseIcon;

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
        if (Btn_Base == null)
        {
            Debug.LogError($"{gameObject.name}의 Btn_Base가 null 입니다. 경로: {GetHierarchyPath(transform)}");
            return;
        }

        // C#의 Action(System.Action)에서 UnityAction으로의 형변환
        Btn_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickAction));
    }

    public void BindOnClickBtnOnlyOne(Action onClickAction)
    {
        Btn_Base.onClick.RemoveAllListeners();
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

    public void SetSlotImg(string path)
    {
        Img_ChooseIcon.sprite = Resources.Load<Sprite>(path);
    }

    public void SetSlotTitle(string title)
    {
        Txt_ChooseTitle.text = title;
    }

    public void SetSlotBullitLv(string lvText)
    {
        Txt_ChooseBullitLv.text = lvText;
    }

    public void SetSlotComment(string Comment)
    {
        Txt_ChooseComment.text = Comment;
    }    
}
