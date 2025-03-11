using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 EpsilonRound(this Vector3 vector) 
    {
        return new Vector3(
            Mathf.Abs(vector.x) < 0.01f ? 0 : vector.x,
            Mathf.Abs(vector.y) < 0.01f ? 0 : vector.y,
            Mathf.Abs(vector.z) < 0.01f ? 0 : vector.z
            );
    }

    public static Vector3 RoundPositionToTile(this Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.x * 2f) / 2f,
            Mathf.Round(vector.y * 2f) / 2f,
            Mathf.Round(vector.z * 2f) / 2f
            );
    }
}
