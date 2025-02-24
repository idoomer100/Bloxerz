using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] [Range(0.5f, 5f)] private float _rollSpeed = 1f;
    [SerializeField] [Range(1f, 10f)] private float _fallTorqueForce = 5f;
    [SerializeField] [Range(1f, 10f)] private float _fallGravityForce = 5f;

    private int _playerHeight = 2;

    private bool _isMoving;
    private Vector3 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_isMoving && _moveInput != Vector3.zero)
        {
            StartMovement();
        }
    }

    private void StartMovement()
    {
        Vector3 anchor = transform.position + (Vector3.down + _moveInput) * 0.5f + RollOffset(_moveInput);
        Vector3 axis = Vector3.Cross(Vector3.up, _moveInput);
        StartCoroutine(Roll(anchor, axis));
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        // Don't allow diagonal input, transform to Vector3
        _moveInput = Mathf.Abs(input.x) > Mathf.Abs(input.y) ? new Vector3(input.x, 0, 0) : new Vector3(0, 0, input.y);
    }

    private Vector3 RollOffset(Vector3 direction)
    {
        Vector3 offset = Vector3.zero;

        Ray ray = new Ray(this.transform.position.RoundedPos() + new Vector3(0.25f, 0.0f, 0.25f), Vector3.down);
        RaycastHit hit;

        float offsetAmount = _playerHeight * 0.5f;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.distance > 1)
            {
                offset = Vector3.down * offsetAmount * 0.5f;
            }
            else
            {
                Collider[] checkOffset = Physics.OverlapSphere(this.transform.position + direction, 0.1f);

                foreach (Collider item in checkOffset)
                {
                    if (item.transform.position == this.transform.position) {
                        offset = direction * offsetAmount * 0.5f;
                    }
                }
            }
        }

        return offset;
    }

    private IEnumerator Roll(Vector3 anchor, Vector3 axis)
    {
        _isMoving = true;
        for (var i = 0; i < 90 / _rollSpeed; i++)
        {
            transform.RotateAround(anchor, axis, _rollSpeed);
            yield return new WaitForSeconds(0.001f);
        }
        transform.position = transform.position.RoundedPos();

        CheckGround();

        _isMoving = false;
        //_rb.isKinematic = true;
    }

    private void CheckGround()
    {
        bool isYAligned = Mathf.Abs(transform.up.x) < 0.001f && Mathf.Abs(transform.up.z) < 0.001f;

        //List<Tile> steppedTiles = new List<Tiles>() TODO: Implement Tile Interface, detect them by this class and store them in this array.
        List<string> steppedTiles = new List<string>(); // Meanwhile that will do.
        Vector3 fallDirection = Vector3.zero;

        if (isYAligned)
        {
            string detectedTile = DetectTile(GetLowestSubblockPosition());
            if (detectedTile != null)
            {
                steppedTiles.Add(detectedTile);
            }
        }
        else
        {
            for (int i = 0; i < _playerHeight; i++)
            {
                Vector3 TileCheckworldPos = GetSubblockWorldPosition(i, _playerHeight);
                string detectedTile = DetectTile(TileCheckworldPos);
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

        if ((isYAligned && !steppedTiles.Any()) ||
            (!isYAligned && steppedTiles.Count < _playerHeight * 0.5f + 0.01f))
        {
            Lose(fallDirection.normalized);
        }
    }

    private void Lose(Vector3 fallDirection)
    {
        _rb.isKinematic = false;

        Vector3 rotationAxis = Vector3.Cross(fallDirection, Vector3.up).normalized;
        _rb.AddTorque(rotationAxis * _fallTorqueForce, ForceMode.VelocityChange);

        _rb.AddForce(Vector3.down * _fallGravityForce, ForceMode.VelocityChange);
    }

    Vector3 GetSubblockWorldPosition(int index, int total)
    {
        float localYPos = (-(total - 1) * 0.5f) + index;
        return transform.TransformPoint(new Vector3(0, localYPos / _playerHeight, 0));
    }

    Vector3 GetLowestSubblockPosition()
    {
        return transform.position + Vector3.down * (_playerHeight * 0.5f - 0.5f);
    }

    // TODO: Change return type to Tile
    string DetectTile(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 1))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return "tile";
            // TODO: return different types of tiles.
        }

        return null;
    }
}