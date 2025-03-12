using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public abstract void OnPlayerStand(Transform player, int height);
}
