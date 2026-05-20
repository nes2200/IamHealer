using UnityEngine;

public class UI_StageScreen : UI_ScreenBase
{
    [SerializeField] GameObject placementController;

    private void OnEnable()
    {
        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;

        placementController.SetActive(true);
    }
    private void OnDisable()
    {
        InputManager.OnCancel -= ToggleMenu;
    }

    public void ToggleMenu(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Menu);

        bool isMenuOpen = UIManager.ClaimCheckOpen(UIType.Menu, out _);
        if (isMenuOpen)
        {
            GameManager.Pause();
        }
        else
        {
            GameManager.UnPause();
        }
    }
        
    public override void Open()
    {
        base.Open();
        GameManager.Instance.Camera.AddCameraController();
    }
    public override void Close()
    {
        base.Close();
        GameManager.Instance.Camera.RemoveCameraController();
    }
}
