using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    ControllerBase _controller;
    public ControllerBase Controller => _controller;

    protected Vector3 _lookRotation;
    public Vector3 LookRotation => _lookRotation;

    public virtual string DisplayName => "Character";

    public virtual void OnPossessed(ControllerBase newController) {}
    public ControllerBase Possessed(ControllerBase from)
    {
        if (Controller) Unpossessed();

        _controller = from;
        OnPossessed(Controller);
        return Controller;
    }

    public virtual void OnUnpossessed(ControllerBase oldController) {}
    public void Unpossessed()
    {
        if(Controller) OnUnpossessed(Controller);
        _controller = null;
    }
    public bool Unpossessed(ControllerBase oldController)
    {
        if (Controller != oldController) return false;

        Unpossessed();
        return true;
    }
}
