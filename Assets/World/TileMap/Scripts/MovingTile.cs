using System;
using System.Collections;
using UnityEngine;

public class MovingTile : Tile
{
    [SerializeField] [Tooltip("Diagonal Movement is not recommended (x xor y).")] Vector2Int moveAmount;
    [SerializeField] [Range(0,10)] float moveSpeed = 3;

    private bool moving = false;
    private int state = 1;
    
    public override void OnPlayerStand(Transform bloxer, int height)
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

    private IEnumerator Move()
    {
        moving = true;

        Movable movable = DetectPassanger();
        if (movable != null)
        {
            movable.state = MovableState.PUSHED;
        }

        Vector3 endPosition = transform.position + state * new Vector3(moveAmount.x, 0, moveAmount.y);

        while (Vector3.Distance(transform.position, endPosition) > 0.1f)
        {
            Vector3 stepStartPosition = transform.position;
            Vector3 stepEndPosition = transform.position + state * new Vector3(Math.Sign(moveAmount.x), 0, Math.Sign(moveAmount.y));
            Vector3 movableStepStartPosition = Vector3.zero;
            Vector3 movableStepEndPosition = Vector3.zero;

            if (movable != null)
            {
                movableStepStartPosition = movable.transform.position;
                movableStepEndPosition = movable.transform.position + state * new Vector3(Math.Sign(moveAmount.x), 0, Math.Sign(moveAmount.y));
            }
         
            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime * moveSpeed;

                transform.position = Vector3.Lerp(stepStartPosition, stepEndPosition, time);

                if (movable != null)
                {
                    movable.transform.position = Vector3.Lerp(movableStepStartPosition, movableStepEndPosition, time);
                }

                yield return null;
            }

            if (movable is BloxerController)
            {
                BloxerController bloxer = (BloxerController)movable;
                bloxer = bloxer.HandleMerge();
                movable = bloxer;
                bloxer.CheckGround(Vector3.zero);
            }
        }

        state = -state;
        moving = false;
    }

    private Movable DetectPassanger()
    {
        RaycastHit hit;
        Movable movable = null;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1))
        {
            hit.transform.TryGetComponent<Movable>(out movable);
        }

        return movable;
    }
}
