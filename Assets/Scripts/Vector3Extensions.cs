using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 RoundedPos(this Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x * 10f) * 0.1f, Mathf.Round(pos.y * 10f) * 0.1f, Mathf.Round(pos.z * 10f) * 0.1f);
    }
}
