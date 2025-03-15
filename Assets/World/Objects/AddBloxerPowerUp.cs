using UnityEngine;

public class AddBloxerPowerUp : PowerUp
{
    public override void ApplyPowerUp()
    {
        if (bloxer.localScale.y == 1)
        {
            bloxer.rotation = Quaternion.identity;
            bloxer.localScale = new Vector3(1, 2, 1);
            bloxer.position = bloxer.position + Vector3.up * 0.5f;

            Destroy(gameObject);
        }
        else if (bloxer.localScale.y == 2)
        {
            bloxer.localScale = new Vector3(1, 3, 1);

            Vector3 addDirection = transform.position - bloxer.position;
            addDirection.y = 0;
            addDirection.Normalize();

            bloxer.position = bloxer.position + addDirection * 0.5f;

            Destroy(gameObject);
        }
    }
}
