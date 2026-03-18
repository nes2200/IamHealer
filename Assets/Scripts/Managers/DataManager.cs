using System.Collections;
using UnityEngine;

public class DataManager : ManagerBase
{
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        yield return null;

    }

    protected override void OnDisconnected()
    {
    }
}
