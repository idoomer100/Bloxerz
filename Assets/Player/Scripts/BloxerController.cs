using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BloxerController : MonoBehaviour
{
    [HideInInspector] public float _rollSpeed = 1f;
    [HideInInspector] public float _fallSpeed = 4;

    private bool _isMoving;
    bool _isFalling = false;

    PlayerController _playerController;

    private void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
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
        float horizontalOffset = (Mathf.Abs(Vector3.Dot(transform.up, moveInput)) > 0.5f) ? transform.localScale.y / 2f : 0.5f;
        float verticalOffset = (Mathf.Abs(Vector3.Dot(transform.up, Vector3.up)) > 0.5f) ? transform.localScale.y / 2f : 0.5f;
        Vector3 pivot = transform.position + moveInput * horizontalOffset - Vector3.up * verticalOffset;
        Vector3 rollAxis = Vector3.Cross(Vector3.up, moveInput);

        if (DetectCollision(moveInput, pivot + Vector3.up * 0.5f) != null)
        {
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

        transform.position = transform.position.RoundPositionToTile();

        CheckGround(moveInput);

        CheckMerge();

        _isMoving = false;
    }

    private bool IsStanding()
    {
        return Mathf.Abs(transform.up.x) < 0.001f && Mathf.Abs(transform.up.z) < 0.001f;
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
                Vector3 TileCheckworldPos = GetSubblockWorldPosition(i, (int)transform.localScale.y);
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
        bool notFullySupportedOnGround = steppedTiles.Count < transform.localScale.y * 0.5f + 0.01f;
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
        return transform.TransformPoint(new Vector3(0, localYPos / transform.localScale.y, 0));
    }

    Vector3 GetLowestSubblockPosition()
    {
        return transform.position + Vector3.down * (transform.localScale.y * 0.5f - 0.5f);
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

    private BloxerController DetectCollision(Vector3 direction, Vector3 offset)
    {
        BloxerController hitBloxer = null;

        RaycastHit hit;
        if (Physics.Raycast(
            offset - direction * 0.01f,
            direction,
            out hit,
            IsStanding() ? transform.localScale.y - 0.01f : 1 - 0.01f,
            1 << gameObject.layer))
        {
            if (hit.transform.TryGetComponent<BloxerController>(out hitBloxer))
            {
                return hitBloxer;
            }
        }

        return hitBloxer;
    }

    private void CheckMerge()
    {
        if (transform.localScale.y == 2)
        {
            if (!IsStanding())
            {
                BloxerController hitBloxer1 = DetectCollision(transform.up, transform.position + transform.up * transform.localScale.y * 0.5f);

                if (hitBloxer1 != null && hitBloxer1.transform.localScale.y == 1)
                {
                    _playerController.MergeBloxerz(this, hitBloxer1);
                }
                else 
                {
                    BloxerController hitBloxer2 = DetectCollision(-transform.up, transform.position - transform.up * transform.localScale.y * 0.5f);
                    if (hitBloxer2 != null && hitBloxer2.transform.localScale.y == 1)
                    {
                        _playerController.MergeBloxerz(this, hitBloxer2);
                    }
                }
            }
        }
        else if (transform.localScale.y == 1)
        {
            Vector3[] directions = new Vector3[4] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

            for (int i = 0; i < 4; i++)
            {
                BloxerController hitBloxer = DetectCollision(directions[i], transform.position + directions[i] * 0.5f);

                if (hitBloxer != null)
                {
                    if (hitBloxer.transform.localScale.y == 1 || Mathf.Abs(Vector3.Dot(directions[i], hitBloxer.transform.up)) > 0.9f)
                    {
                        _playerController.MergeBloxerz(this, hitBloxer);
                        break;
                    }
                }
            }
            
        }
    }
}