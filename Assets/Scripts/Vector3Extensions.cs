using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 RoundedPos(this Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x * 10f) * 0.1f, Mathf.Round(pos.y * 10f) * 0.1f, Mathf.Round(pos.z * 10f) * 0.1f);
    }

    public static Vector3 EpsilonRound(this Vector3 vector) 
    {
        return new Vector3(
            Mathf.Abs(vector.x) < 0.01f ? 0 : vector.x,
            Mathf.Abs(vector.y) < 0.01f ? 0 : vector.y,
            Mathf.Abs(vector.z) < 0.01f ? 0 : vector.z
            );
    }
}
