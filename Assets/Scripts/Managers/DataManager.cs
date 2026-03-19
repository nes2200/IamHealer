using System.Collections;
using UnityEngine;

public class DataManager : ManagerBase
{
    public override int LoadCount => 100;

    protected override IEnumerator Onconnected(GameManager newManager)
    {
        // 로딩 진해율 => 최대 몇개를 로딩해야 하는지, 몇개까지 했는지
        //                현재 / 최대
        UIBase loading = UIManager.ClaimGetUI(UIType.Loading);
        IProgress<int> progressUI = loading as IProgress<int>;
        IStatus<string> statusUI = loading as IStatus<string>;

        //if (TryGetFildFromResources("Prefabs/Square", out Sprite trash)) Debug.Log(trash);

        // Interface : 어떤 기능이 있을거란 약속.

        for(int i = 0; i < LoadCount; i += 7)
        {
            progressUI?.AddCurrent(7);
            statusUI?.SetCurrentStatus($"Loading {i + 1}/{LoadCount}");
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    protected override void OnDisconnected()
    {
    }

    //Resources => 유니티에서 Resources라는 폴더를 만들면 사용 가능
    bool TryGetFildFromResources<T>(string path, out T result) where T : Object
    {
        result = Resources.Load<T>(path);
        return result != null;
    }

    bool TryGetFileFromAssetBundle()
    {
        return false; 
    }
}
