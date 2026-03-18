using System.Collections;
using UnityEngine;

public class CameraManager : ManagerBase
{
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        yield return null;

    }

    protected override void OnDisconnected()
    {
    }
}
