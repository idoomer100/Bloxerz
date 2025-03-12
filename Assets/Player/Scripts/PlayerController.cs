using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject bloxer1;

    [SerializeField] [Range(100f, 300f)] private float _rollSpeed = 3f;
    [SerializeField] [Range(1f, 5f)] private float _fallSpeed = 4;
    [SerializeField] [Range(1f, 10f)] private float _slideSpeed = 4;

    BloxerController[] bloxerz;

    private Vector3 _moveInput;
    private int activeBloxer;

    private void Start()
    {
        DetectBloxers();
    }

    private void FixedUpdate()
    {
        if (_moveInput != Vector3.zero)
        {
            bloxerz[activeBloxer].Move(_moveInput);
        }
    }

    public void DetectBloxers()
    {
        bloxerz = GetComponentsInChildren<BloxerController>();
        
        foreach (BloxerController bloxer in bloxerz)
        {
            bloxer._rollSpeed = _rollSpeed;
            bloxer._fallSpeed = _fallSpeed;
            bloxer._slideSpeed = _slideSpeed;
        }
        
        activeBloxer = 0;
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        // Don't allow diagonal input, transform to Vector3
        if (input.x != 0 && input.x != _moveInput.x)
        {
            _moveInput = new Vector3(input.x, 0, 0);
        }
        else if (input.y != 0 && input.y != _moveInput.y) 
        {
            _moveInput = new Vector3(0, 0, input.y);
        }
        else
        {
            _moveInput = Vector3.zero;
        }
    }

    public void OnSwitch(InputValue value)
    {
        SwitchActiveBloxer();
    }

    private void SwitchActiveBloxer()
    {
        activeBloxer++;
        if (activeBloxer >= bloxerz.Length)
        {
            activeBloxer = 0;
        }
    }

    private void ActivateBloxer(BloxerController bloxer)
    {
        int tries = 4;
        while (!ReferenceEquals(bloxerz[activeBloxer], bloxer) && tries > 0)
        {
            SwitchActiveBloxer();
            tries--;
        }
    }

    public void MergeBloxerz(BloxerController bloxer1ToMerge, BloxerController bloxer2ToMerge)
    {
        StartCoroutine(MergeBloxerz(bloxer1ToMerge.transform, bloxer2ToMerge.transform));
    }

    private IEnumerator MergeBloxerz(Transform bloxer1ToMerge, Transform bloxer2ToMerge)
    {
        Vector3 distancesDirection = (bloxer2ToMerge.position - bloxer1ToMerge.position).normalized;
        
        int height = (int)Mathf.Round(Vector3.Distance(bloxer1ToMerge.position, bloxer2ToMerge.position) * 2);
        
        Vector3 spawnPosition = bloxer1ToMerge.position + distancesDirection * bloxer2ToMerge.localScale.y * 0.5f;
        
        Quaternion rotation = Quaternion.Euler(new Vector3(distancesDirection.z * 90, 0, distancesDirection.x * 90));

        GameObject spawnedBloxer = Instantiate(bloxer1, spawnPosition, rotation, transform);
        
        spawnedBloxer.transform.localScale = new Vector3(1, height, 1);
        
        Destroy(bloxer1ToMerge.gameObject);
        Destroy(bloxer2ToMerge.gameObject);
        
        yield return null;
        
        DetectBloxers();

        ActivateBloxer(spawnedBloxer.GetComponent<BloxerController>());
    }
}
