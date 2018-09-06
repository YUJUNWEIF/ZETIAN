using UnityEngine;

//namespace geniusbaby
//{
public static class MathTrans
{
    public static FEVector2D V2(this Vector2 v2) { return new FEVector2D(v2.x, v2.y); }
    public static FEVector2D V2(this Vector3 v3) { return new FEVector2D(v3.x, v3.y); }
    public static FEVector3D V3(this Vector2 v2) { return new FEVector3D(v2.x, v2.y, 0); }
    public static FEVector3D V3(this Vector3 v3) { return new FEVector3D(v3.x, v3.y, v3.z); }
    public static Vector2 V2(this FEVector2D v2) { return new Vector2(v2.x, v2.y); }
    public static Vector2 V2(this FEVector3D v3) { return new Vector2(v3.x, v3.y); }
    public static Vector3 V3(this FEVector2D v2) { return new Vector3(v2.x, v2.y, 0); }
    public static Vector3 V3(this FEVector3D v3) { return new Vector3(v3.x, v3.y, v3.z); }
    public static Quaternion QU(this FEQuaternion qf) { return new Quaternion((float)qf.x, (float)qf.y, (float)qf.z, (float)qf.w); }
    public static FEQuaternion QF(this Quaternion qu) { return new FEQuaternion(qu.w, qu.x, qu.y, qu.z); }
    public static FEMatrix4 MF(this Matrix4x4 m)
    {
        return new FEMatrix4(
            m.m00, m.m01, m.m02, m.m03,
            m.m10, m.m11, m.m12, m.m13,
            m.m20, m.m21, m.m22, m.m23,
            m.m30, m.m31, m.m32, m.m33);
    }
}
//}
