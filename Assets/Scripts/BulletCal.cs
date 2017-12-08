using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletCal{

    private const float DegToRad = Mathf.PI / 180;

    public static Vector3 VectorRotate(Vector3 _vector, float _degrees) // vector를 degree만큼 회전.
    {
        return _vector.RotateRadians(_degrees * DegToRad);
    }

    public static Vector3 RotateRadians(this Vector3 _vector, float _radians) // vector를 radian 만큼 회전 this 키워드는 확장 메소드 검색 static에서만 사용됨
    {
        var ca = Mathf.Cos(_radians);
        var sa = Mathf.Sin(_radians);
        return new Vector3(ca * _vector.x - sa * _vector.y, sa * _vector.x + ca * _vector.y,0);
    }

    public static Quaternion GetRotFromVector(Vector3 _vector) // vector방향으로 회전하는 Quaternion 계산
    {
        Quaternion newRotation = Quaternion.LookRotation(_vector, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        return newRotation;
    }
}
