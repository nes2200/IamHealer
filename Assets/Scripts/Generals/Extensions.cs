using UnityEngine;

//확장 메소드를 가지고 있을 클래스
//객체화가 필요할까?
//원본의 기능을 확장하기 때문에 원본이 있다면 객체화할 필요가 없다
// => 하나만 있어도 된다
public static class Extensions
{
    //Normalize - 본인을 1로 바꿔버림
    //normalized - 크기가 1인 값을 돌려주는 것

    //                            내가 함수를 넣고 싶은 대상에 this
    public static float normalized(this float target)
    {
        
        if (target > 0) return 1f;
        else if (target < 0) return -1f;
        else return 0f;
    }

    //Try Add Component => 추가를 시도 => 있는지 확인 => 없으면 추가
    public static T TryAddComponent<T>(this GameObject target) where T : Component
    {
        T result = null;
        if (target == null) return result;

        result = target.GetComponent<T>() ?? target.AddComponent<T>();

        //result = target.GetComponent<T>();
        //result ??= target.AddComponent<T>();

        //result = target.GetComponent<T>();
        //if (result is null) result = target.AddComponent<T>();

        return result;
    }
    
    public static T TryAddComponent<T>(this Component target) where T : Component
    {
        if(target == null) return null;
        else return target.gameObject.TryAddComponent<T>();
    }
}
