using UnityEngine;

public class SplitTile : Tile
{
    [SerializeField] GameObject bloxer1;
    [SerializeField] Vector3[] spawnLocations = new Vector3[3];
    [SerializeField] float dashSpeed;
    [SerializeField] GameObject dot;


    private LineRenderer[] dashes;
    private MeshRenderer[] dots;

    private void Start()
    {
        dashes = GetComponentsInChildren<LineRenderer>();
        
        for (int i = 0; i < 3; i++)
        {
            dashes[i].SetPosition(0, transform.position + Vector3.up * 0.1f);
            dashes[i].SetPosition(1, transform.position + spawnLocations[i] + Vector3.up * 0.1f);
            Instantiate(dot, transform.position + spawnLocations[i] + Vector3.up * 0.11f, Quaternion.identity, transform);
        }
        
    }

    public override void OnPlayerStand(Transform bloxer, int height)
    {
        PlayerController playerController = bloxer.parent.GetComponent<PlayerController>();

        InstansiateBloxer(playerController, transform.position + spawnLocations[0]  + Vector3.up * 0.5f);
        InstansiateBloxer(playerController, transform.position + spawnLocations[1] + Vector3.up * 0.5f);
        if (height == 3)
        {
            InstansiateBloxer(playerController, transform.position + spawnLocations[2] + Vector3.up * 0.5f);
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

    private float textureOffset;
    private void Update()
    {
        textureOffset -= Time.deltaTime * dashSpeed;

        for (int i = 0; i < 3; i++)
        {
            dashes[i].material.SetTextureOffset("_BaseMap", new Vector2(textureOffset, 0));
        }
    }
}
