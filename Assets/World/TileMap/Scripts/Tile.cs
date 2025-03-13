using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public abstract void OnPlayerStand(Transform bloxer, int height);
}
