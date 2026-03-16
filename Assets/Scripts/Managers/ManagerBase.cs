using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    GameManager _connectedManager;

    public void Connect(GameManager newManager)
    {
        if (_connectedManager != null) Disconnect(); //이미 연결된 애가 있으면 끊고 간다

        _connectedManager = newManager;
        Onconnected(newManager);
    }

    public void Disconnect()
    {
        _connectedManager = null;
        OnDisconnected();
    }

    protected abstract void Onconnected(GameManager newManager);
    protected abstract void OnDisconnected();
}
