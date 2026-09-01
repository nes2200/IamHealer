using Unity.Cinemachine;
using UnityEngine;

public abstract class AIController : ControllerBase
{
    GameObject _focusTarget = null;
    public GameObject FocusTarget => _focusTarget;

    protected abstract void Think(float deltaTime);

    protected virtual void SearchFocusTarget()
    {

    }

    public GameObject SetFocusTarget(GameObject newTarget)
    {
        if (!IsFocussable(newTarget))
        {
            return _focusTarget;
        }

        GameObject oldTarget = _focusTarget;
        _focusTarget = newTarget;
        OnFocusTargetChanged(oldTarget, _focusTarget);
        
        return _focusTarget;
    }
    protected virtual bool IsFocussable(GameObject target) => target != FocusTarget;
    protected virtual void OnFocusTargetChanged(GameObject oldTarget, GameObject newTarget)
    {
    }
}
