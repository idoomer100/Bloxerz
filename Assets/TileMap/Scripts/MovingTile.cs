using System.Collections;
using UnityEngine;

public class MovingTile : Tile
{
    [SerializeField] Vector2Int moveAmount;

    private bool moving = false;
    private int state = 1;
    
    public override void OnPlayerStand(Transform Player)
    {
        return;
    }

    public void Open()
    {
        if (!moving)
        {
            StartCoroutine(Move());
        }
    }

    const float MOVE_SPEED = 10;

    private IEnumerator Move()
    {
        moving = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + state * new Vector3(moveAmount.x, 0, moveAmount.y);

        RaycastHit hit;
        Transform detectedBloxer = null;
        Vector3 bloxerStartPosition = Vector3.zero;
        Vector3 bloxerEndPosition = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1))
        {
            detectedBloxer = hit.transform;
            bloxerStartPosition = detectedBloxer.position;
            bloxerEndPosition = detectedBloxer.position + new Vector3(moveAmount.x, 0, moveAmount.y);
        }

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * MOVE_SPEED;
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            if (detectedBloxer != null)
            {
                detectedBloxer.position = Vector3.Lerp(bloxerStartPosition, bloxerEndPosition, time);
            }
            yield return null;
        }

        state = -state;
        moving = false;
    }
}
