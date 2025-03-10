using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BloxerController : MonoBehaviour
{
    [HideInInspector] public float _rollSpeed = 1f;
    [HideInInspector] public float _fallSpeed = 4;

    private int _height = 2;

    private bool _isMoving;
    bool _isFalling = false;

    private void Start()
    {
        _height = (int)transform.localScale.y;
    }

    public void Move(Vector3 moveInput)
    {
        if (!_isMoving && !_isFalling)
        {
            StartCoroutine(Roll(moveInput));
        }
    }

    private IEnumerator Roll(Vector3 moveInput)
    {
        float horizontalOffset = (Mathf.Abs(Vector3.Dot(transform.up, moveInput)) > 0.5f) ? _height / 2f : 0.5f;
        float verticalOffset = (Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)) > 0.5f) ? _height / 2f : 0.5f;
        Vector3 pivot = transform.position + moveInput * horizontalOffset - Vector3.up * verticalOffset;
        Vector3 rollAxis = Vector3.Cross(Vector3.up, moveInput);

        if (DetectCollision(moveInput, pivot))
        {
            print("ERRRR collision!");
            yield break;
        }

        _isMoving = true;

        float rotatedAngle = 0f;
        while (rotatedAngle < 90f)
        {
            float step = _rollSpeed * Time.deltaTime;
            if (rotatedAngle + step > 90f)
                step = 90f - rotatedAngle;
            transform.RotateAround(pivot, rollAxis, step);
            rotatedAngle += step;
            yield return null;
        }

        RoundPositionToTile();

        CheckGround(moveInput);

        _isMoving = false;
    }

    private bool IsStanding()
    {
        return Mathf.Abs(transform.up.x) < 0.001f && Mathf.Abs(transform.up.z) < 0.001f;
    }

    private void RoundPositionToTile()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x * 2f) / 2f,
            Mathf.Round(transform.position.y * 2f) / 2f + 0.05f,
            Mathf.Round(transform.position.z * 2f) / 2f
            );
    }

    private void CheckGround(Vector3 moveInput)
    {
        List<Tile> steppedTiles = new List<Tile>();
        Vector3 fallDirection = Vector3.zero;

        bool isStanding = IsStanding();

        if (isStanding)
        {
            Tile detectedTile = DetectTile(GetLowestSubblockPosition());
            if (detectedTile != null)
            {
                steppedTiles.Add(detectedTile);
                if (_height > 1)
                {
                    detectedTile.OnPlayerStand(transform);
                }
            }
        }
        else
        {
            for (int i = 0; i < _height; i++)
            {
                Vector3 TileCheckworldPos = GetSubblockWorldPosition(i, _height);
                Tile detectedTile = DetectTile(TileCheckworldPos);
                if (detectedTile != null)
                {
                    steppedTiles.Add(detectedTile);
                }
                else
                {
                    fallDirection += new Vector3(transform.position.x - TileCheckworldPos.x, 0, transform.position.z - TileCheckworldPos.z);
                }
            }
        }

        bool inAir = !steppedTiles.Any();
        bool notFullySupportedOnGround = steppedTiles.Count < _height * 0.5f + 0.01f;
        if ((isStanding && inAir) ||
            (!isStanding && notFullySupportedOnGround))
        {
            StartCoroutine(Fall(inAir ? -moveInput : fallDirection.EpsilonRound().normalized));
        }
    }

    Tile DetectTile(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 1))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }

    Vector3 GetSubblockWorldPosition(int index, int total)
    {
        float localYPos = (-(total - 1) * 0.5f) + index;
        return transform.TransformPoint(new Vector3(0, localYPos / _height, 0));
    }

    Vector3 GetLowestSubblockPosition()
    {
        return transform.position + Vector3.down * (_height * 0.5f - 0.5f);
    }

    private IEnumerator Fall(Vector3 fallDirection)
    {
        _isFalling = true;

        bool firstFall = true;

        while (true)
        {
            Vector3 axis = Vector3.Cross(fallDirection, Vector3.up).normalized;

            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.AngleAxis(90, axis) * startRotation;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = transform.position + Vector3.down;
            if (firstFall)
                endPosition -= fallDirection;

            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime * _fallSpeed;
                transform.position = Vector3.Lerp(startPosition, endPosition, time);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);
                yield return null;
            }

            firstFall = false;
        }
    }

    private bool DetectCollision(Vector3 direction, Vector3 offset)
    {
        RaycastHit hit;
        if (Physics.Raycast(offset + Vector3.up * 0.5f - direction * 0.01f,
            direction,
            out hit,
            IsStanding() ? _height - 0.01f : 1 - 0.01f,
            1 << gameObject.layer))
        {
            return true;
        }

        return false;
    }
}