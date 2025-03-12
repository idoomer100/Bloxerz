using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    private bool waitingToApply = false;
    protected Transform bloxer = null;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bloxer"))
        {
            waitingToApply = true;
            bloxer = other.transform;
        }
    }

    private void FixedUpdate()
    {
        if (waitingToApply && bloxer != null)
        {
            if (bloxer.rotation.IsRotationAt90DegreeSteps())
            {
                ApplyPowerUp();
            }
        }
    }

    abstract public void ApplyPowerUp();
}
