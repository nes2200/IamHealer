using UnityEditor.Tilemaps;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public virtual void Registration(UIManager manager)
    {

    }
    public virtual void Unregistration(UIManager manager)
    {

    }

    public GameObject SetChild(GameObject newChild)
    {
        if (!newChild) return null;
        newChild.transform.SetParent(transform);

        return OnSetChild(newChild);
    }

    protected virtual GameObject OnSetChild(GameObject newChild)
    {
        return newChild;
    }

    public void UnsetChild(GameObject oldChild)
    {
        if (!oldChild) return;
        //나를 부모라고 생각하고 있다면
        if(oldChild.transform.parent == transform)
        {
            //부모 해제하기
            oldChild.transform.SetParent(null);
        }
        OnUnsetChild(oldChild);
    }

    protected virtual void OnUnsetChild(GameObject oldChild)
    {

    }
}
