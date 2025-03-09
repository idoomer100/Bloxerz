using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Range(100f, 300f)] private float _rollSpeed = 3f;
    [SerializeField] [Range(1f, 5f)] private float _fallSpeed = 4;

    BloxerController[] bloxerz;

    private Vector3 _moveInput;
    private int activeBloxer;

    private void Start()
    {
        bloxerz = GetComponentsInChildren<BloxerController>();
        foreach (BloxerController bloxer in bloxerz)
        {
            bloxer._rollSpeed = _rollSpeed;
            bloxer._fallSpeed = _fallSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (_moveInput != Vector3.zero)
        {
            bloxerz[activeBloxer].Move(_moveInput);
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        // Don't allow diagonal input, transform to Vector3
        _moveInput = Mathf.Abs(input.x) > Mathf.Abs(input.y) ? new Vector3(input.x, 0, 0) : new Vector3(0, 0, input.y);
    }

    public void OnSwitch(InputValue value)
    {
        activeBloxer++;
        if (activeBloxer >= bloxerz.Length)
        {
            activeBloxer = 0;
        }
    }
}
