using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

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

        //게임 하면서 미니언 30개 쓸꺼니까 그만큼 준비해야지
        for (int i = 0; i < _setting.countInitial; i++)
        {
            PrepareObject();
        }
    }

    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        if (result)
        {
            result.SetActive(false);
            //부모 - 자식 관계 만들기
            //폴더처럼 쓸 수 있는 것을 만들어보자
            result.name = Setting.poolName;
        }
        return result;
    }

    //오브젝트 생성 부탁
    public GameObject CreateObject()
    {
        return null;
    }

    //오브젝트 제거 부탁
    public void DestroyObject(GameObject target)
    {

    }
}
