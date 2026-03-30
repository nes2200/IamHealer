using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolSetting
{
    public string poolName;
    public GameObject target;
    public int countInitial; //처음에 준비할 개수
    public int countAdditional; //부족하면 추가할 개수
}

public class ObjectManager : ManagerBase
{
    [SerializeField] PoolSetting[] testSetting;

    //PoolRequest는 얼마나 자주 추가될까? => 로딩할 떄 즈음?
    //로딩되는 회수보다 대상에 개수가 부족하면 새로 추가하거나 하는 일
    List<PoolRequest> loadedPoolRequests = new();

    //해당하는 이름의 대상으로 불러주기 위해서
    //[이름 - 게임오브젝트] 자료구조
    Dictionary<string, ObjectPoolModule> poolDictionary = new();

    protected override IEnumerator Onconnected(GameManager newManager)
    {
        RegistrationPool("GlobalCharacterPool");
        RegistrationPool("GlobalControllerPool");
        RegistrationPool("GlobalEffectPool");
        RegistrationPool("GlobalObjectPool");
        RegistrationPool("GlobalUIPool");

        InitializePool();

        yield return null;
    }

    protected override void OnDisconnected()
    {
    }

    //오브젝트 만들고 등록하기
    public static GameObject CreateObject(GameObject prefab, Transform parent = null)
    {
        if (prefab == null) return null;

        GameObject result = Instantiate(prefab, parent);
        RegistrationObject(result);
        return result;
    }
    public static GameObject CreatObject(GameObject prefab, Vector3 position)
    {
        GameObject result = CreateObject(prefab);
        if (result) result.transform.position = position;
        return result;
    }
    public static GameObject CreatObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
        }
        return result;
    }
    public static GameObject CreatObject(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;
        }
        return result;
    }
    public static GameObject CreatObject(GameObject prefab, Transform parent, Vector3 position, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    break;
            }
            result.transform.position = position;
        }
        return result;
    }
    public static GameObject CreatObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    break;
            }
        }
        return result;
    }
    public static GameObject CreatObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale,Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale;
                    //"진짜 크기"는 "부모 크기"와 비교해서 값을 구한다
                    //로컬 => 월드    로컬 * (월드 / 로컬) = 월드
                    //월드 => 로컬    월드 * (로컬 / 월드) = 로컬
                    //float scaledScaleX = scale.x * (result.transform.localScale.x / result.transform.lossyScale.x);
                    //float scaledScaleY = scale.y * (result.transform.localScale.y / result.transform.lossyScale.y);
                    //float scaledScaleZ = scale.z * (result.transform.localScale.z / result.transform.lossyScale.z);
                    //result.transform.localScale = new Vector3(scaledScaleX, scaledScaleY, scaledScaleZ);
                    
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    result.transform.localScale = scale;
                    break;
            }
        }
        return result;
    }

    //변수로 받은 오브젝트 등록하기
    public static void RegistrationObject(GameObject target)
    {
        if (target)
        {
            // 오브젝트에 'IFunctionable'을 가진 모든 '컴포넌트'를 다 가져와야한다.
            foreach (var current in target.GetComponentsInChildren<IFunctionable>())
            {
                current.RegistrationFunctions();
            }
        }
    }

    public static void DestroyObject(GameObject target)
    {
        if (!target) return;
        UnRegistrationObject(target);
        Destroy(target);

    }
    public static void UnRegistrationObject(GameObject target)
    {
        if (target)
        {
            // 오브젝트에 'IFunctionable'을 가진 모든 '컴포넌트'를 다 가져와야한다.
            foreach (var current in target.GetComponentsInChildren<IFunctionable>())
            {
                current.UnregistrationFunctions();
            }
        }
    }

    public void RegistrationPool(string poolName)
    {
        PoolRequest currentRequest = DataManager.LoadDataFile<PoolRequest>(poolName);
        if (currentRequest == null) return;
        loadedPoolRequests.Add(currentRequest);
        //애들마다 하나씩
        //         학생         다음학생    in     3학년4반
        foreach(PoolSetting currentSetting in currentRequest.settings)
        {
            string currentName = currentSetting.poolName;
            GameObject currentPrefab = currentSetting.target;
            //다음학생이 오늘 안왔대요! => 다음 학생을 불러야 한다
            if (currentPrefab == null) continue;
            //이름에 문제가 있을 수 있음
            if (poolDictionary.ContainsKey(currentName)) continue;
            //여기까지 왔으면 추가하기
            poolDictionary.Add(currentName, new(currentSetting));
        }
    }

    public void InitializePool()
    {
        foreach(ObjectPoolModule currentPool in poolDictionary.Values)
        {
            currentPool?.Initialize();
        }
    }
}
