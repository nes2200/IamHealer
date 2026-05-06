using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (target == null) return null;
        else return target.gameObject.TryAddComponent<T>();
    }

    //Action : 반환값이 없는 함수
    //Func : 반환값이 있는 함수
    //왼쪽 오른쪽 친구를 가지고 비교를 해서 그 결과가 bool로 나오는 형태의 함수를 Comparison이라고 한다
    public static T GetExtreme<T>(this IEnumerable targetList, float defaultScore,
     System.Func<T, float> Evaluator,
     System.Func<float, float, bool> Comparison)
    {
        T result = default;
        float extremeScore = defaultScore;

        //foreach에 in 역할을 넣을 수 있는 모든 것을 담을 수 있어야 한다. Array, List, Dictionary 등등
        foreach (T currentTarget in targetList)
        {
            float currentScore = Evaluator(currentTarget);

            if (Comparison(currentScore, extremeScore))
            {
                extremeScore = currentScore;
                result = currentTarget;
            }
        }
        return result;
    }
    public static T GetMaximum<T>(this IEnumerable targetList, System.Func<T, float> Evaluator)
     => GetExtreme(targetList, float.MinValue, Evaluator, (a,b) => a > b);
    public static T GetMinimum<T>(this IEnumerable targetList, System.Func<T, float> Evaluator)
      => GetExtreme(targetList, float.MaxValue, Evaluator, (a,b) => a < b);

    public static IEnumerator WaitForTask(this Task targetTask)
    {
        //WaitWhile : treu인 동안 작동함
        //WaitUntil : false인 동안 작동함 -> true가 될 떄까지 기다림
        //                기다린다         타겟 작업 끝나기 전까지
        yield return new WaitUntil(() => targetTask.IsCompleted);
        //작업을 제거하다
        targetTask.Dispose();
    }

    public static float GetPenetratedDistance(float aHalf, float bHalf, float aPos, float bPos)
    {
        float absAHalf = Mathf.Abs(aHalf);
        float absBHalf = Mathf.Abs(bHalf);
        //겹쳤다면, 만약에 원래 안 겹쳤을 때에 있을 수 있는 공간
        float minSpace = absAHalf + absBHalf;
        //둘 사이의 거리
        float distance = aPos - bPos;
        //x최소 거리와 둘 사이의 거리 차이
        float penetration = minSpace - Mathf.Abs(distance);
        //어느 방향으로 묻혀있는지 확인
        return penetration *= Mathf.Sign(distance);
    }

    //A와 B가 얼마나 깊게 묻혀 있는지 확인
    public static Vector2 AABB(this Rect A, Rect B)
    {
        Vector2 result = Vector2.zero;
        Vector2 aMin = A.min;
        Vector2 aMax = A.max;
        Vector2 aHalf = A.size * 0.5f;
        Vector2 bMin = B.min;
        Vector2 bMax = B.max;
        Vector2 bHalf = B.size * 0.5f;

        //한 쪽의 최대 위치가 다른 쪽의 최소 위치보다 높아야 함
        if (aMax.x > bMin.x && bMax.x > aMin.x)
        {
            result.x = GetPenetratedDistance(aHalf.x, bHalf.x, A.position.x, B.position.x);
        }
        if (aMax.y > bMin.y && bMax.y > aMin.y)
        {
            result.y = GetPenetratedDistance(aHalf.y, bHalf.y, A.position.y, B.position.y);
        }
        return result;
    }

    public static float GetOutBoundDistance(float inMin, float outMin, float inMax, float outMax)
    {
        float result = 0f;

        //전체 맵보다 카메라가 클 경우
        bool leftOut = inMin < outMin;
        bool rightOut = inMax > outMax;

        if (leftOut ^ rightOut)
        {
            if(leftOut) result = outMin - inMin;
            if(rightOut) result = outMax - inMax;
        }
        return result;
    }

    //삐져 나온 양을 체크하는 방법
    public static Vector2 InversedAABB(this Rect target, Rect bound)
    {
        Vector2 result = Vector2.zero;
        result.x = GetOutBoundDistance(target.xMin, bound.xMin, target.xMax, bound.xMax);
        result.y = GetOutBoundDistance(target.yMin, bound.yMin, target.yMax, bound.yMax);
        return result;
    }
}
