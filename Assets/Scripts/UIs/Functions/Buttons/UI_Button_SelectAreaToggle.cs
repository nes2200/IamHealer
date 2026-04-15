using UnityEngine;

public class UI_Button_SelectAreaToggle : MonoBehaviour
{
    [SerializeField] GameObject unitSelectArea;
    [SerializeField] GameObject weaponSelectArea;

    public void SwapToUnitSelectArea()
    {
        unitSelectArea.SetActive(true);
        weaponSelectArea.SetActive(false);
    }
    public void SwapToWeaponSelectArea()
    {
        unitSelectArea.SetActive(false);
        weaponSelectArea.SetActive(true);
    }

}
