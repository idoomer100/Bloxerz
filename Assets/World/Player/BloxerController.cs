using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BloxerController : Movable
{
    [SerializeField] [Range(100f, 300f)] private float rollSpeed = 3f;
    [SerializeField] [Range(0, 2)] private float collisionEffectDuration = 1f;

    PlayerController _playerController;
    Material material;

    bool playingCollisionEffect;

    private void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
        material = GetComponent<MeshRenderer>().material;

        CheckGround(Vector3.zero);
    }

    public void Move(Vector3 moveInput)
    {
        if (state == MovableState.IDLE)
        {
            StartCoroutine(Roll(moveInput));
        }
    }

    private IEnumerator Roll(Vector3 moveInput)
    {
        float horizontalOffset = IsGoingToStand(moveInput) ? transform.localScale.y / 2f : 0.5f;
        float verticalOffset = IsStanding() ? transform.localScale.y / 2f : 0.5f;
        Vector3 pivot = transform.position + moveInput * horizontalOffset - Vector3.up * verticalOffset;
        Vector3 rollAxis = Vector3.Cross(Vector3.up, moveInput);

        (List<Movable> pushables, int pushbalesWeight) = DetectPushables(moveInput, this);
        if (transform.localScale.y > 1 && (IsGoingToStand(moveInput) || IsStanding()))
        {
            if (pushbalesWeight > 0)
            {
                if (!playingCollisionEffect)
                    StartCoroutine(TriggerCollisionEffect(moveInput));
                yield break;
            }
        }
        else
        {
            if (pushbalesWeight > transform.localScale.y)
            {
                if (!playingCollisionEffect)
                    StartCoroutine(TriggerCollisionEffect(moveInput));
                yield break;
            }
            else
            {
                foreach (Movable movable in pushables)
                {
                    movable.Push(moveInput);
                }
            }
        }

        state = MovableState.ROLLING;

        float rotatedAngle = 0f;
        while (rotatedAngle < 90f)
        {
            float step = rollSpeed * Time.deltaTime;
            if (rotatedAngle + step > 90f)
                step = 90f - rotatedAngle;
            transform.RotateAround(pivot, rollAxis, step);
            rotatedAngle += step;
            yield return null;
        }

        transform.position = transform.position.RoundPositionToTile();

        CheckGround(moveInput);

        HandleMerge();
    }

    IEnumerator TriggerCollisionEffect(Vector3 collisionDirection)
    {
        playingCollisionEffect = true;

        float elapsed = 0;
        
        material.SetVector("_CollisionDirection", collisionDirection);

        while (elapsed < collisionEffectDuration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Sin((elapsed / collisionEffectDuration) * Mathf.PI); 

            material.SetFloat("_CollisionEffect", intensity);

            yield return null;
        }

        material.SetFloat("_CollisionEffect", 0);

        playingCollisionEffect = false;
    }

    private BloxerController CheckMerge(Vector3 direction, Vector3 offset)
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

    protected override void AfterSlideChecks()
    {
        HandleMerge();
    }

    public BloxerController HandleMerge()
    {
        if (transform.localScale.y == 2)
        {
            if (!IsStanding())
            {
                BloxerController hitBloxer1 = CheckMerge(transform.up, transform.position + transform.up * transform.localScale.y * 0.5f);

                if (hitBloxer1 != null && hitBloxer1.transform.localScale.y == 1)
                {
                    return _playerController.MergeBloxerz(transform, hitBloxer1.transform);
                }
                else 
                {
                    BloxerController hitBloxer2 = CheckMerge(-transform.up, transform.position - transform.up * transform.localScale.y * 0.5f);
                    if (hitBloxer2 != null && hitBloxer2.transform.localScale.y == 1)
                    {
                        return _playerController.MergeBloxerz(transform, hitBloxer2.transform);
                    }
                }
            }
        }
        else if (transform.localScale.y == 1)
        {
            Vector3[] directions = new Vector3[4] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

            for (int i = 0; i < 4; i++)
            {
                BloxerController hitBloxer = CheckMerge(directions[i], transform.position + directions[i] * 0.5f);

                if (hitBloxer != null)
                {
                    if (hitBloxer.transform.localScale.y == 1 || Mathf.Abs(Vector3.Dot(directions[i], hitBloxer.transform.up)) > 0.9f)
                    {
                        return _playerController.MergeBloxerz(transform, hitBloxer.transform);
                    }
                }
            }
            
        }

        return this;
    }
}