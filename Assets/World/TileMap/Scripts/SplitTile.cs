using System.Collections;
using UnityEngine;

public class SplitTile : Tile
{
    [SerializeField] GameObject bloxer1;
    [SerializeField] Vector3 spawnLocation1;
    [SerializeField] Vector3 spawnLocation2;
    [SerializeField] Vector3 spawnLocation3;

    public override void OnPlayerStand(Transform bloxer, int height)
    {
        PlayerController playerController = bloxer.parent.GetComponent<PlayerController>();

        InstansiateBloxer(playerController, spawnLocation1);
        InstansiateBloxer(playerController, spawnLocation2);
        if (height == 3)
        {
            InstansiateBloxer(playerController, spawnLocation3);
        }

        Destroy(bloxer.gameObject);

        playerController.ScheduleBloxerzDetection();
    }

    private void InstansiateBloxer(PlayerController player, Vector3 spawnLocation)
    {
        // If there is already cube in the spawn location, increase it's height by 1
        RaycastHit hit;
        if (Physics.Raycast(spawnLocation + Vector3.down * 0.6f, Vector3.up, out hit, 1, LayerMask.GetMask("Bloxer")))
        {
            hit.transform.rotation = Quaternion.identity;
            hit.transform.localScale = new Vector3(1, 2, 1);
            hit.transform.position = hit.transform.position + Vector3.up * 0.5f;
            hit.transform.GetComponent<BloxerController>();
        }
        else
        {
            player.SpawnBloxer(spawnLocation);
        }
    }
}
