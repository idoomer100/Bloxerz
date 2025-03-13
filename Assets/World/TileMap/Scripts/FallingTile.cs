using System;
using System.Collections;
using UnityEngine;

public class FallingTile : Tile
{
    public override void OnPlayerStand(Transform bloxer, int height)
    {
        StartCoroutine(Fall(bloxer));
        print("you lose!!"); //TODO: LOSE;
    }

    const float MOVE_SPEED = 1;
    const float FALL_DISTANCE = 10;
    private IEnumerator Fall(Transform player)
    {
        Vector3 tileStartPosition = transform.position;
        Vector3 playerStartPosition = transform.position;
        Vector3 endPosition = transform.position + Vector3.down * FALL_DISTANCE;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * MOVE_SPEED;
            transform.position = Vector3.Lerp(tileStartPosition, endPosition, time);
            player.position = Vector3.Lerp(playerStartPosition, endPosition, time);
            yield return null;
        }
    }
}
