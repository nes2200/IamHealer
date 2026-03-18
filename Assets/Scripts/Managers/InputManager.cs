using System.Collections;
using UnityEngine;

public class InputManager : ManagerBase
{
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        yield return null;

    }

    protected override void OnDisconnected()
    {
    }
}
