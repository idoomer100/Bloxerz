using System.Collections;
using UnityEngine;

public class GateTile : Tile
{
    [SerializeField] Vector2Int moveAmount;

    public override void OnPlayerStand(Transform Player)
    {
        return;
    }

    public void Open()
    {
        StartCoroutine(Move());
    }

    const float MOVE_SPEED = 10;

    private IEnumerator Move()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + new Vector3(moveAmount.x, 0, moveAmount.y);

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * MOVE_SPEED;
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
        }
    }
}
