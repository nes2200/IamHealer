using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] Slider hpSlider;

    [Header("Visual")]
    [SerializeField] GameObject visualRoot;
    [SerializeField] Image fillImage;
    [SerializeField] Color allyColor = Color.green;
    [SerializeField] Color enemyColor = Color.red;


    CharacterBase owner;
    Transform hpbarAnchor;
    HitPointModule hpModule;
    Camera mainCamera;

    public void Initialize(CharacterBase newOwner)
    {
        owner = newOwner;
        if (visualRoot) visualRoot.SetActive(true);
        hpbarAnchor = owner.HPBarAnchor;
        hpModule = newOwner.GetModule<HitPointModule>();
        mainCamera = GameManager.Camera.MainCamera;
        UpdateScreenPosition(0f);

        GameManager.OnUpdateUI -= UpdateScreenPosition;
        GameManager.OnUpdateUI += UpdateScreenPosition;

        hpModule.OnHPChanged -= ChangeSlider;
        hpModule.OnHPChanged += ChangeSlider;

        owner.OnFaint -= HideOnFaint;
        owner.OnFaint += HideOnFaint;

        UpdateBarColor();
        ChangeSlider();
    }
    public void Remove()
    {
        GameManager.OnUpdateUI -= UpdateScreenPosition;
        if (hpModule) hpModule.OnHPChanged -= ChangeSlider;
        if (owner) owner.OnFaint -= HideOnFaint;

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

    public void UpdateBarColor()
    {
        fillImage.color = owner.Team switch
        {
            TeamID.TeamA => allyColor,
            TeamID.TeamB => enemyColor,
            _ => Color.gray
        };
    }

    public void ChangeSlider()
    {
        if (!hpSlider || !hpModule) return;

        hpSlider.value = hpModule.HPPercent;
    }

    public void HideOnFaint()
    {
        if (visualRoot) visualRoot.SetActive(false);
        gameObject.SetActive(false);
    }
}
