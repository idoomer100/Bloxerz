using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] [Range(1f, 5f)] private float _fallSpeed = 4;
    [SerializeField] [Range(1f, 10f)] private float _slideSpeed = 4;
    [SerializeField] [Range(0, 20)] private float _pushSpeed = 10;
    [SerializeField] private LayerMask collisionLayer;

    public MovableState state;

    protected (List<Movable>, int) DetectPushables(Vector3 direction, Movable pushingObject, int depth=0)
    {
        List<Movable> pushList = new List<Movable>();

        int totalMass = 0;

        if (depth > 4) return (pushList, totalMass);

        if (depth == 0 && transform.localScale.y > 1 && state != MovableState.SLIDING)
        {
            if (pushingObject.IsGoingToStand(direction))
            {
                RaycastHit hit;
                if (Physics.Raycast(pushingObject.transform.position,
                    direction,
                    out hit,
                    transform.localScale.y / 2 + 0.5f,
                    collisionLayer))
                {
                    return (pushList, 100);
                }
                return (pushList, totalMass);
            }
            if (pushingObject.IsStanding())
            {
                RaycastHit hit;
                if (Physics.Raycast(pushingObject.transform.position - Vector3.up * (transform.localScale.y / 2 - 0.5f),
                    direction,
                    out hit,
                    transform.localScale.y,
                    collisionLayer))
                {
                    return (pushList, 100);
                }
                return (pushList, totalMass);
            }
        }
        
 
        Collider[] hits = Physics.OverlapBox(
            pushingObject.transform.position + direction * 0.5f,
            pushingObject.transform.localScale / 2 - new Vector3(0.1f, 0.1f, 0.1f),
            pushingObject.transform.rotation,
            collisionLayer);

        foreach (Collider col in hits)
        {
            Movable m = col.GetComponent<Movable>();
            if (col != null && m == null)
            {
                // Hit a wall
                return (pushList, 100);
            }
            if (m != null && m != pushingObject)
            {
                totalMass += (int)m.transform.localScale.y;

                pushList.Add(m);

                (List<Movable> chainPushables, int chainMass) = DetectPushables(direction, m, depth+1);

                totalMass += chainMass;
                pushList.AddRange(chainPushables);

            }
        }

        return (pushList, totalMass);
    }

    protected bool IsStanding()
    {
        return Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)) > 0.6f;
    }

    protected bool IsGoingToStand(Vector3 moveInput)
    {
        return Mathf.Abs(Vector3.Dot(transform.up, moveInput)) > 0.6f;
    }

    public void Push(Vector3 direction)
    { 
        StartCoroutine(Pushed(direction));
    }

    private IEnumerator Pushed(Vector3 direction)
    {
        state = MovableState.PUSHED;

        Vector3 startPosition = transform.position;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * _pushSpeed;
            Vector3 updatedStartPosition = new Vector3(startPosition.x, transform.position.y, startPosition.z);
            Vector3 updatedEndPosition = updatedStartPosition + direction;
            transform.position = Vector3.Lerp(updatedStartPosition, updatedEndPosition, time);
            yield return null;
        }

        CheckGround(direction);
    }

    public void CheckGround(Vector3 moveInput)
    {
        List<Tile> steppedTiles = new List<Tile>();
        Vector3 fallDirection = Vector3.zero;

        bool isStanding = IsStanding();

        if (isStanding)
        {
            Vector3 groundPos = transform.position + Vector3.down * (transform.localScale.y * 0.5f - 0.5f);
            Tile detectedTile = DetectTile(groundPos);
            if (detectedTile != null)
            {
                steppedTiles.Add(detectedTile);
                if (transform.localScale.y > 1)
                {
                    detectedTile.OnPlayerStand(transform, (int)transform.localScale.y);
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.localScale.y; i++)
            {
                Vector3 tileCheckworldPos = GetSubblockWorldPosition(i, (int)transform.localScale.y);
                Tile detectedTile = DetectTile(tileCheckworldPos);
                if (detectedTile != null)
                {
                    steppedTiles.Add(detectedTile);
                }
                else
                {
                    fallDirection += new Vector3(transform.position.x - tileCheckworldPos.x, 0, transform.position.z - tileCheckworldPos.z);
                }
            }
        }

        bool inAir = !steppedTiles.Any();
        bool notFullySupportedByGround = steppedTiles.Count < transform.localScale.y * 0.5f + 0.01f;
        // If height=3 and lying on the ground where only 1 tile is directly below it's, is supported and shouldn't fall:
        notFullySupportedByGround &= !(transform.localScale.y == 3 && 
            steppedTiles.Count == 1 &&
            Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(steppedTiles[0].transform.position.x, steppedTiles[0].transform.position.z)
                ) < 0.1f);

        if ((!IsStanding() && notFullySupportedByGround) ||
            (IsStanding() && inAir))
        {
            StartCoroutine(Fall(fallDirection, moveInput, inAir));
        }
        else if (steppedTiles.All(tile => tile is IceTile))
        {
            StartCoroutine(Slide(moveInput));
        }
        else
        {
            state = MovableState.IDLE;
        }
    }

    Vector3 GetSubblockWorldPosition(int index, int total)
    {
        float localYPos = (-(total - 1) * 0.5f) + index;
        return transform.TransformPoint(new Vector3(0, localYPos / transform.localScale.y, 0));
    }

    Tile DetectTile(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 2))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }

    protected IEnumerator Slide(Vector3 moveInput)
    {
        state = MovableState.SLIDING;

        //Check collision before sliding
        (List<Movable> pushables, int pushbalesWeight) = DetectPushables(moveInput, this);
        if (pushbalesWeight > 0)
        {
            state = MovableState.IDLE;
            yield break;
        }


        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + moveInput;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * _slideSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
        }

        transform.position = endPosition;

        AfterSlideChecks();

        CheckGround(moveInput);
    }

    protected virtual void AfterSlideChecks()
    {
        return;
    }

    private IEnumerator Fall(Vector3 fallDirection, Vector3 moveInput, bool inAir)
    {
        MovableState lastState = state;
        state = MovableState.FALLING;

        fallDirection = inAir ? -moveInput : fallDirection.EpsilonRound().normalized;

        bool firstWhirl = true;

        while (true)
        {
            Vector3 axis = Vector3.Cross(fallDirection, Vector3.up).normalized;

            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.AngleAxis(90, axis) * startRotation;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = transform.position + Vector3.down;

            // Add extra push to the fall, only if is actively rolling or half supported by ground, and only for first whirl
            bool doWhirl = lastState == MovableState.ROLLING || !inAir;
            if (firstWhirl && doWhirl)
                endPosition -= fallDirection;

            float spinFallTime = 0;

            while (spinFallTime < 1)
            {
                spinFallTime += Time.deltaTime * _fallSpeed;

                transform.position = Vector3.Lerp(startPosition, endPosition, spinFallTime);
                if (doWhirl)
                {
                    transform.rotation = Quaternion.Lerp(startRotation, endRotation, spinFallTime);
                }
                yield return null;
            }

            firstWhirl = false;
        }
    }
}

public enum MovableState
{
    IDLE,
    ROLLING,
    FALLING,
    SLIDING,
    PUSHED
}