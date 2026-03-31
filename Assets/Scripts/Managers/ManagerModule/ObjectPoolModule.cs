using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectPoolModule
{
    //오브젝트 하나를 관리하는 하청 모듈
    PoolSetting _setting;
    public PoolSetting Setting => _setting;

    Transform rootTransform;

    //"대기열" => 선입 선출. 먼저 온 애가 먼저 나간다.
    Queue<GameObject> prepareQueue = new();

    //생성할 때 정보를 넣어주기
    //생성자! 반환값 => 본인, 이름 => 본인
    public ObjectPoolModule(PoolSetting newSetting)
    {
        _setting = newSetting;
    }

    public void Initialize()
    {
        rootTransform = new GameObject(Setting.poolName).transform;

        //풀링하려고 하는 원본 프리팹에 "PooledObject"라고 하는게 안들어있으면 추가해 줄 필요가 있다
        Setting.target?.TryAddComponent<PooledObject>();
        

        //게임 하면서 미니언 30개 쓸꺼니까 그만큼 준비해야지
        PrepareObjects(Setting.countInitial);
        
    }

    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = CreateFromPrefab();
        EnqueueObject(result);
        return result;
    }

    //uint => 마이너스가 존재하면 안됨
    //unsigned => 부호가 없다
    void PrepareObjects(uint count)
    {
        if (!Setting.target) return;
        for (uint i = 0; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }
    void PrepareObjects(uint count, out GameObject activeObject)
    {
        if (!Setting.target)
        {
            activeObject = null;
            return;
        }

        activeObject = CreateFromPrefab();

        for (uint i = 1; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }

    public GameObject CreateFromPrefab()
    {
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        
        if (result)
        {
            result.name = Setting.poolName;
            if(result.TryGetComponent(out PooledObject pool))
            {
                pool.OnEnqueueEvent -= DestroyObject;
                pool.OnEnqueueEvent += DestroyObject;
            }
        }
        return result;
    }

    //오브젝트 생성 부탁
    public GameObject CreateObject(Transform parent = null)
    {
        //대기자중에 꺼내보기
        GameObject result;
        //대기열에 아무도 없을때
        if(!prepareQueue.TryDequeue(out result))
        {
            //새로 대기자를 뽑아오기
            PrepareObjects(Setting.countAdditional, out result);
        }

        if (result) //만들어 졌는지 체크
        {
            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnDequeue();
            }

            result.transform.SetParent(parent);
            result.SetActive(true);
        }

        return result;
    }

    //오브젝트 제거 부탁
    public void DestroyObject(GameObject target)
    {
        EnqueueObject(target);
        if (target)
        {
            target.transform.SetParent(rootTransform);
        }
    }

    public void EnqueueObject(GameObject target)
    {
        if(!target) return;

        target.SetActive(false);
        //부모 - 자식 관계 만들기
        //폴더처럼 쓸 수 있는 것을 만들어보자
        prepareQueue.Enqueue(target);
    }
}
