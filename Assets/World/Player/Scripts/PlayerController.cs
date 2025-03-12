using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject bloxer1;

    [SerializeField] [Range(100f, 300f)] private float _rollSpeed = 3f;
    [SerializeField] [Range(1f, 5f)] private float _fallSpeed = 4;
    [SerializeField] [Range(1f, 10f)] private float _slideSpeed = 4;

    BloxerController[] bloxerz;

    CinemachineCamera camera;

    private Vector3 _moveInput;
    private int activeBloxer;

    private void Start()
    {
        CinemachineBrain brain = CinemachineBrain.GetActiveBrain(0);

        if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera camera)
        {
            this.camera = camera;
        }

        DetectBloxers();
    }

    private void FixedUpdate()
    {
        if (_moveInput != Vector3.zero)
        {
            if(bloxerz[activeBloxer] != null)
            {
                bloxerz[activeBloxer].Move(_moveInput);
            }
        }
    }

    public void DetectBloxers()
    {
        StartCoroutine(StartDetectBloxers());
    }

    public IEnumerator StartDetectBloxers()
    {
        yield return null;

        bloxerz = GetComponentsInChildren<BloxerController>();

        foreach (BloxerController bloxer in bloxerz)
        {
            bloxer._rollSpeed = _rollSpeed;
            bloxer._fallSpeed = _fallSpeed;
            bloxer._slideSpeed = _slideSpeed;
        }

        activeBloxer = 0;

        camera.Follow = bloxerz[activeBloxer].transform;
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

        camera.Follow = bloxerz[activeBloxer].transform;
    }

    private void ActivateBloxer(BloxerController bloxer)
    {
        for (int i = 0; i < bloxerz.Length; i++)
        {
            if (ReferenceEquals(bloxerz[activeBloxer], bloxer))
            {
                return;
            }
            
            SwitchActiveBloxer();
        }
    }

    public void MergeBloxerz(Transform bloxer1ToMerge, Transform bloxer2ToMerge)
    {
        Vector3 distancesDirection = (bloxer2ToMerge.position - bloxer1ToMerge.position).normalized;
        
        int height = (int)Mathf.Round(Vector3.Distance(bloxer1ToMerge.position, bloxer2ToMerge.position) * 2);
        
        Vector3 spawnPosition = bloxer1ToMerge.position + distancesDirection * bloxer2ToMerge.localScale.y * 0.5f;
        
        Quaternion rotation = Quaternion.Euler(new Vector3(distancesDirection.z * 90, 0, distancesDirection.x * 90));

        GameObject spawnedBloxer = Instantiate(bloxer1, spawnPosition, rotation, transform);
        
        spawnedBloxer.transform.localScale = new Vector3(1, height, 1);
        
        Destroy(bloxer1ToMerge.gameObject);
        Destroy(bloxer2ToMerge.gameObject);
                
        DetectBloxers();

        ActivateBloxer(spawnedBloxer.GetComponent<BloxerController>());
    }

    public void SpawnBloxer(Vector3 spawnLocation)
    {
        Instantiate(bloxer1, spawnLocation, Quaternion.identity, transform);
    }
}
