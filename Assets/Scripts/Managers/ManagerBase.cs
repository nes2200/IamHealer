using System.Collections;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    GameManager _connectedManager;

    public virtual int LoadCount => 1;

    public IEnumerator Connect(GameManager newManager)
    {
        if (_connectedManager != null) Disconnect(); //이미 연결된 애가 있으면 끊고 간다

        _connectedManager = newManager;
        yield return Onconnected(newManager);
    }

    public void Disconnect()
    {
        _connectedManager = null;
        OnDisconnected();
    }

    protected abstract IEnumerator Onconnected(GameManager newManager);
    protected abstract void OnDisconnected();
}
