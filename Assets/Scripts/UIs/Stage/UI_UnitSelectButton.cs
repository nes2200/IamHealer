using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitSelectButton : MonoBehaviour
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

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        status = unitPrefab.GetComponent<CharacterBase>().Status;
        unitNameText.text = status.unitName;
        unitCostText.text = status.cost.ToString();
    }

    public void SelectUnit()
    {
        InputManager.OnUnitSelected -= SelectUnit;
        InputManager.OnUnitSelected += SelectUnit;
    }

    private void SelectUnit(GameObject selectedUnit)
    {

    }
}
