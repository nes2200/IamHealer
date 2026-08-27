using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button_UnitSelect : UIBase
{
    //버튼을 누르면 유닛 프리팹과 비용이 임시로 저장됨
    //바닥을 클릭하면 프리팹 복사본을 생성
    //추가될 비용이 최대 코스트 제한을 넘으면 생성 안됨

    [Header("유닛 프리팹")]
    [SerializeField] UnitDefinition unitDefinition;
    
    UnitStatus status;

    [Header("오브젝트 구성 요소")]
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] TextMeshProUGUI unitCostText;
    [SerializeField] Image unitImage;

    public void Initialize(UnitDefinition newUnitDefinition)
    {
        if(!newUnitDefinition || !newUnitDefinition.IsValid)
        {
            Debug.LogError("[UI_Button_UnitSelect] 유효한 유닛 정의가 필요합니다.");
            return;
        }

        unitDefinition = newUnitDefinition;
        status = unitDefinition.Status;

        unitNameText.text = status.unitName;
        unitCostText.text = status.cost.ToString();
    }

    public void OnClickUnitSelect()
    {
        if(unitDefinition && status)
        {
            InputManager.InvokeUnitSelect(unitDefinition);
        }
    }
}
