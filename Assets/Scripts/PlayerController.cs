using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _rollSpeed = 1;
    private bool _isMoving;

    private void Update()
    {
        if (_isMoving) return;

        if (Input.GetKey(KeyCode.A)) Assemble(Vector3.left);
        else if (Input.GetKey(KeyCode.D)) Assemble(Vector3.right);
        else if (Input.GetKey(KeyCode.W)) Assemble(Vector3.forward);
        else if (Input.GetKey(KeyCode.S)) Assemble(Vector3.back);

        void Assemble(Vector3 direction)
        {
            Vector3 anchor = transform.position + (Vector3.down + direction) * 0.5f + RollOffset(direction);
            // Temporary movement- Only works for a cube.
            Vector3 axis = Vector3.Cross(Vector3.up, direction);
            StartCoroutine(Roll(anchor, axis));
        }
    }

    private Vector3 RollOffset(Vector3 direction)
    {
        Vector3 offset = Vector3.zero;

        Ray ray = new Ray(this.transform.position.RoundedPos() + new Vector3(0.25f, 0.0f, 0.25f), Vector3.down);
        RaycastHit hit;

        float offsetAmount = transform.localScale.y * 0.5f; // height * 0.5
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
        _isMoving = false;
        transform.position = transform.position.RoundedPos();
    }
}