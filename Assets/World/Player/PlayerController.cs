using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject bloxerPrefab;

    BloxerController[] bloxerz;

    CinemachineCamera cmCamera;

    private Vector3 _moveInput;
    private int activeBloxer;

    private bool needToDetect = false;
    private BloxerController lastSpawnedBloxer = null;

    private void Start()
    {
        CinemachineBrain brain = CinemachineBrain.GetActiveBrain(0);

        if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera cmCamera)
        {
            this.cmCamera = cmCamera;
        }

        ScheduleBloxerzDetection();
    }

    private void FixedUpdate()
    {
        if (needToDetect)
        {
            DetectBloxerz();
        }

        if (_moveInput != Vector3.zero)
        {
            if(bloxerz[activeBloxer] != null)
            {
                bloxerz[activeBloxer].Move(_moveInput);
            }
        }
    }

    public void ScheduleBloxerzDetection()
    {
        needToDetect = true;
    }

    public void DetectBloxerz()
    {
        bloxerz = GetComponentsInChildren<BloxerController>();

        activeBloxer = 0;

        for (int i = 0; i < bloxerz.Length; i++)
        {
            if (lastSpawnedBloxer != null)
            {
                if (bloxerz[i] == lastSpawnedBloxer)
                {
                    activeBloxer = i;
                    lastSpawnedBloxer = null;
                }
            }
        }

        cmCamera.Follow = bloxerz[activeBloxer].transform;

        needToDetect = false;
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

        cmCamera.Follow = bloxerz[activeBloxer].transform;
    }

    public BloxerController MergeBloxerz(Transform bloxer1ToMerge, Transform bloxer2ToMerge)
    {
        Vector3 distancesDirection = (bloxer2ToMerge.position - bloxer1ToMerge.position).normalized;
        
        int height = (int)Mathf.Round(Vector3.Distance(bloxer1ToMerge.position, bloxer2ToMerge.position) * 2);
        
        Vector3 spawnPosition = bloxer1ToMerge.position + distancesDirection * bloxer2ToMerge.localScale.y * 0.5f;
        
        Quaternion rotation = Quaternion.Euler(new Vector3(distancesDirection.z * 90, 0, distancesDirection.x * 90));

        BloxerController spawnedBloxer = SpawnBloxer(spawnPosition, rotation);
        
        spawnedBloxer.transform.localScale = new Vector3(1, height, 1);
        
        Destroy(bloxer1ToMerge.gameObject);
        Destroy(bloxer2ToMerge.gameObject);

        ScheduleBloxerzDetection();

        return spawnedBloxer;
    }

    public BloxerController SpawnBloxer(Vector3 spawnLocation)
    {
        return SpawnBloxer(spawnLocation, Quaternion.identity);
    }

    public BloxerController SpawnBloxer(Vector3 spawnLocation, Quaternion rotation)
    {
        GameObject spawnedBloxer = Instantiate(bloxerPrefab, spawnLocation, rotation, transform);

        lastSpawnedBloxer = spawnedBloxer.transform.GetComponent<BloxerController>();

        return lastSpawnedBloxer;
    }
}
