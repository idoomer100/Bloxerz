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
            MeshRenderer currPlayer = GetComponent<MeshRenderer>();
            Vector3 size = currPlayer.bounds.size;

            Debug.Log(size.x + " " + size.y +  " " + size.z);

            var anchor = transform.position + (Vector3.down + direction) * 0.5f;
            // Temporary movement- Only works for a cube.
            var axis = Vector3.Cross(Vector3.up, direction);
            StartCoroutine(Roll(anchor, axis));
        }
    }

    private bool isAlive()
    {
        Ray ray = new Ray(this.transform.position.RoundedPos(), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3))
        {
            return true;
        }
        return false;
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

        Debug.Log("is alive: " + isAlive());
    }
}