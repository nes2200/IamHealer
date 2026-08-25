using UnityEngine;

public class UI_Button_UnitRemove : MonoBehaviour
{
    UnitPlaceIndicator indicator;

    public void Connect(UnitPlaceIndicator newIndicator)
    {
        indicator = newIndicator;
    }
    public void Disconnect()
    {
        indicator = null;
    }

    public void ToggleRemoveMode()
    {
        if (!indicator) return;

        indicator.ToggleMode();
    }
}
