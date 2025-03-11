using System.Collections;
using UnityEngine;

public class SplitTile : Tile
{
    [SerializeField] GameObject bloxer1;
    [SerializeField] Vector3 spawnLocation1;
    [SerializeField] Vector3 spawnLocation2;
    [SerializeField] Vector3 spawnLocation3;

    public override void OnPlayerStand(Transform player, int height)
    {
        StartCoroutine(SplitBloxer(player, height));
    }

    private IEnumerator SplitBloxer(Transform player, int height)
    {
        InstansiateBloxer(player, spawnLocation1);
        InstansiateBloxer(player, spawnLocation2);
        if (height == 3)
        {
            InstansiateBloxer(player, spawnLocation3);
        }

        PlayerController playerController = player.parent.GetComponent<PlayerController>();

        Destroy(player.gameObject);

        yield return null;

        playerController.DetectBloxers();
    }

    private void InstansiateBloxer(Transform player, Vector3 spawnLocation)
    {
        // If there is already cube in the spawn location, increase it's height by 1
        RaycastHit hit;
        if (Physics.Raycast(spawnLocation + Vector3.down * 0.6f, Vector3.up, out hit, 0.5f, LayerMask.GetMask("Bloxer")))
        {
            hit.transform.rotation = Quaternion.identity;
            hit.transform.localScale = new Vector3(1, 2, 1);
            hit.transform.position = hit.transform.position + Vector3.up * 0.5f;
        }
        else
        {
            Instantiate(bloxer1, spawnLocation, Quaternion.identity, player.parent);
        }
    }
}
