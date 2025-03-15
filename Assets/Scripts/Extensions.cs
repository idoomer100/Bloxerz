using UnityEngine;

public static class Extensions
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

    public static bool IsRotationAt90DegreeSteps(this Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;

        bool isXValid = Mathf.Round(euler.x / 90) * 90 == Mathf.Round(euler.x);
        bool isYValid = Mathf.Round(euler.y / 90) * 90 == Mathf.Round(euler.y);
        bool isZValid = Mathf.Round(euler.z / 90) * 90 == Mathf.Round(euler.z);

        return isXValid && isYValid && isZValid;
    }

    public static bool IsSnappedToGrid(this Vector3 vector)
    {
        Vector3 roundedVector = vector.RoundPositionToTile();
        return Vector3.Distance(vector, roundedVector) < 0.3f;
    }
}
