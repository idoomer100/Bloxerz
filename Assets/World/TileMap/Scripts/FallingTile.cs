using System;
using System.Collections;
using UnityEngine;

public class FallingTile : Tile
{
    [SerializeField] [Range(0, 5)] float fallSpeed;

    public override void OnPlayerStand(Transform bloxer, int height)
    {
        StartCoroutine(Fall(bloxer));
        print("you lose!!"); //TODO: LOSE;
    }

    const float FALL_DISTANCE = 10;
    private IEnumerator Fall(Transform player)
    {
        Vector3 tileStartPosition = transform.position;
        Vector3 playerStartPosition = transform.position;
        Vector3 endPosition = transform.position + Vector3.down * FALL_DISTANCE;
        Vector3 playerEndPosition = player.position + Vector3.down * FALL_DISTANCE;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * fallSpeed;
            transform.position = Vector3.Lerp(tileStartPosition, endPosition, time);
            player.position = Vector3.Lerp(playerStartPosition, playerEndPosition, time);
            yield return null;
        }
    }
}
