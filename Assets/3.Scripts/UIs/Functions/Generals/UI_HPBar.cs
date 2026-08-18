using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] Slider hpSlider;

    CharacterBase owner;
    Transform hpbarAnchor;
    HitPointModule hpModule;
    Camera mainCamera;

    public void Initialize(CharacterBase newOwner)
    {
        owner = newOwner;
        hpbarAnchor = owner.HPBarAnchor;
        hpModule = newOwner.GetModule<HitPointModule>();
        mainCamera = GameManager.Camera.MainCamera;
        UpdateScreenPosition(0f);

        GameManager.OnUpdateCharacter -= UpdateScreenPosition;
        GameManager.OnUpdateCharacter += UpdateScreenPosition;

        hpModule.OnHPChanged -= ChangeSlider;
        hpModule.OnHPChanged += ChangeSlider;
        ChangeSlider();
    }
    public void Remove()
    {
        GameManager.OnUpdateCharacter -= UpdateScreenPosition;
        if (hpModule) hpModule.OnHPChanged -= ChangeSlider;

        owner = null;
        hpbarAnchor = null;
        hpModule = null;
        mainCamera = null;
    }

    public void UpdateScreenPosition(float _)
    {
        if (!mainCamera || !hpbarAnchor) return;

        Vector3 anchorScreenPosition = mainCamera.WorldToScreenPoint(hpbarAnchor.position);
        transform.position = anchorScreenPosition;
    }

    public void ChangeSlider()
    {
        if (!hpSlider || !hpModule) return;

        hpSlider.value = hpModule.HPPercent;
    }
}
