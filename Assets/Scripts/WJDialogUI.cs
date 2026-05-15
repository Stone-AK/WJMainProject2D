using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 여기는 DialogUI이지만 해당 스크립트는 데이터 드리븐을 보여주는 것이 목적임
// 아래 DaniTechUIBase의 경우 MonoBehavior를 상속 받은 클래스로 UI들을 묶기 위한 부모 클래스
public class WJDialogUI : DaniTechUIBase
{
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_Next;

    private string _currentdialougeId = string.Empty;
    private Queue<string> _descriptionQueue = new Queue<string>();

    private void OnEnable()
    {
        Button_Next.BindOnClickButtonEvent(OnClick_Next);
    }

    public void OnClick_Next()
    {
        // 페이지가 넘어갈때 들어갈 메서드들을 넣기 위함
        // 넘어갈때 글뿐만 아니라, 소리, 이미지 등이 바뀔 가능성이 있기 때문
        SetNextPage();
    }

    public void SetNextPage()
    {
        bool isNextDescriptionOnened = CheckAndSetDescription();

        if(isNextDescriptionOnened)
        {
            return;
        }
        // 해당 Dialogue는 Manager를 통해서 열린것이므로 Destroy나 Disable을 통해서 하는것이 아닌
        // Manager의 닫기를 사용해야함.
        // DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.WJDialogueUI);
    }

    // 이 UI가 오픈 될때 어떤 dialogue의 Id를 받아와서 정보를 꺼낼 수 있는 메서드를 먼저 만들어야함
    // UIManager.Inst.OpenDialogue("dialogeu_mainstream_1_1_100");
    // UIManager -> OpenDialogue() 열렸을거고, 클래스 형변환해서 -> Startdialogue()
    // instanceId가 전달되는 경우는 게임내에서 생성되고 휘발되는 게임 오브젝트들(채집물, 몬스터)
    // uniqueId가 전달되는 경우(플레이어, 아이템) uniqueId와 dataId는 연동해야만한다.
    // 왜냐하면 해당 아이템이 변경되는 경우 dataId와 연관되어 있어야 같이 변경되기 때문
    // dataId는 레코드 Id(엑셀 > Json > DataClass 순서로 만들어진 ID)
    public void StartDialogue(string dialogueDataId)
    {
        // 애초에 데이터가 없으면 시작도 하면 안됨.
        var dialougueData = DaniTechGameDataManager.Instance.GetDNDialogueData(dialogueDataId);
        if( dialougueData == null )
        {
            Debug.LogWarning($"{dialogueDataId}다이얼로그 데이터가 존재하지 않습니다.");
            return;
        }

        _currentdialougeId = dialogueDataId;

        // Description에 split(분할) 문자가 있는경우 해당 문자별로 나누기
        if(dialougueData.Description.Contains("<np>"))
        {
            string[] dialogueDescArr = dialougueData.Description.Split("<np>");
            foreach (string desc in dialogueDescArr)
            {
                _descriptionQueue.Enqueue(desc);
            }
            CheckAndSetDescription();
        }
        else
        {
            Text_Description.text = dialougueData.Description;
        }
    }

    private bool CheckAndSetDescription()
    {
        // 큐에 남은 문장이 있다면 출력하는 부분
        bool isNextDescriptionExsit = (_descriptionQueue.Count > 0);
        if(isNextDescriptionExsit)
        {
            string desc = _descriptionQueue.Dequeue();
            Text_Description.text = desc;
        }

        return isNextDescriptionExsit;
    }

    // 2) 다음 다이어로그가 있는가 체크하는 메서드(기능)을 제작
    // 3) 선택지가 있는 것인 경우 버튼을 생성(또는 Active)해주는 메서드(기능) 제작
    // 4) 선택지에 따른 다이어로그를 띄우는 메서드(기능) 제작
    // 다이어로그를 통해 기능이 추가되는 경우 위의 순서에 따라 기능 하나하나를 만들고
    // 실제로 되는지 하나하나 테스트 해보는 것을 해야한다.
    // 그리고 테스트가 성공하면 다음으로 넘어가는 식으로 계속 개발해야한다.
    // 절대 한번에 개발하지 말것


}
