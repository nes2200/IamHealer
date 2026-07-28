using UnityEngine;

public static class SerializationUtility
{
    //소숫점 셋째 자리까지 반올림 해주는 함수
    public static float RoundToThreeDecimals(float value)
    {
        return Mathf.Round(value * 1000f) / 1000f;
    }
}

//Vectro3를 직렬화하여 저장할 컨테이너
[System.Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    //변환이 쉽게 하기 위한 생성자
    public SerializableVector3(Vector3 v3)
    {
        x = SerializationUtility.RoundToThreeDecimals(v3.x);
        y = SerializationUtility.RoundToThreeDecimals(v3.y);
        z = SerializationUtility.RoundToThreeDecimals(v3.z);
    }

    //형변환을 쉽게 하기 위한 연산자
    public static implicit operator SerializableVector3(Vector3 v3) => new SerializableVector3(v3);
    public static implicit operator Vector3(SerializableVector3 v3) => new Vector3(v3.x, v3.y, v3.z);
}
//회전 오차 방지를 위한 Quaternion 저장 컨테이너
[System.Serializable]
public struct SerializableQuaternion
{
    public float x, y, z, w;

    //변환이 쉽게 하기 위한 생성자
    public SerializableQuaternion(Quaternion q)
    {
        x = SerializationUtility.RoundToThreeDecimals(q.x);
        y = SerializationUtility.RoundToThreeDecimals(q.y);
        z = SerializationUtility.RoundToThreeDecimals(q.z);
        w = SerializationUtility.RoundToThreeDecimals(q.w);
    }

    //형변환을 쉽게 하기 위한 연산자
    public static implicit operator SerializableQuaternion(Quaternion q) => new SerializableQuaternion(q);
    public static implicit operator Quaternion(SerializableQuaternion sq) => new Quaternion(sq.x, sq.y, sq.z, sq.w);
}

