using System.Collections;
using UnityEngine;

public class RemoveBloxerPowerUp : PowerUp
{
    public override void ApplyPowerUp()
    {
        PlayerController playerController = bloxer.parent.GetComponent<PlayerController>();

        // If player has height of 1, destory it instead of decreasing it's height.
        if (bloxer.localScale.y == 1)
        {
            Destroy(bloxer.gameObject);
            playerController.ScheduleBloxerzDetection();
        }
        else
        {
            // If player standing remove top blox.
            if (Mathf.Abs(Vector3.Dot(bloxer.up, Vector3.up)) > 0.8f)
            {
                bloxer.localScale = new Vector3(1, bloxer.localScale.y - 1, 1);
                bloxer.position = bloxer.transform.position + Vector3.down * 0.5f;
            }
            // If height not 1 and power-up on same position as the player then it means player height is 3 and
            // it encountered the power-up at it's middle, in this case it should split into 2 bloxerz1.
            else if (Vector3.Distance(transform.position, bloxer.position) < 0.1f)
            {
                playerController.SpawnBloxer(bloxer.position + bloxer.up);
                playerController.SpawnBloxer(bloxer.position - bloxer.up);

                Destroy(bloxer.gameObject);

                playerController.ScheduleBloxerzDetection();
            }
            // Else, height is 2 or 3 so just change height and offset accordingly.
            else
            {
                bloxer.localScale = new Vector3(1, bloxer.localScale.y - 1, 1);

                Vector3 removeDirection = bloxer.position - transform.position;
                removeDirection.y = 0;
                removeDirection.Normalize();

                bloxer.position = bloxer.transform.position + removeDirection * 0.5f;
            }
        }

        Destroy(gameObject);
    }
}
