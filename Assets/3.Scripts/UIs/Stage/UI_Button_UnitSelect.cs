using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button_UnitSelect : UIBase
{
    //버튼을 누르면 유닛 프리팹과 비용이 임시로 저장됨
    //바닥을 클릭하면 프리팹 복사본을 생성
    //추가될 비용이 최대 코스트 제한을 넘으면 생성 안됨

    [Header("유닛 프리팹")]
    [SerializeField] GameObject unitPrefab;
    
    UnitStatus status;

    [Header("오브젝트 구성 요소")]
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] TextMeshProUGUI unitCostText;
    [SerializeField] Image unitImage;

    public void Initialize(GameObject newUnitPrefab)
    {
        if(!newUnitPrefab)
        {
            Debug.LogError("[UI_Button_UnitSelect] 유닛 프리팹이 없습니다.");
            return;
        }
        CharacterBase character = newUnitPrefab.GetComponent<CharacterBase>();
        if (!character || !character.Status)
        {
            Debug.LogError($"[UI_Button_UnitSelect] '{newUnitPrefab.name}'의 캐릭터 정보를 찾지 못했습니다.");
            return;
        }

        unitPrefab = newUnitPrefab;
        status = character.Status;

        unitNameText.text = status.unitName;
        unitCostText.text = status.cost.ToString();
    }

    public void OnClickUnitSelect()
    {
        if(unitPrefab && status)
        {
            InputManager.InvokeUnitSelect(unitPrefab, status.cost);
        }
    }
}
