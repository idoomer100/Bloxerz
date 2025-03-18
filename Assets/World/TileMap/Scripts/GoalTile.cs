using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GoalTile : Tile
{
    [SerializeField] [Range(0, 5)] float riseSpeed;
    [SerializeField] Material pressedMaterial;

    CinemachineCamera cmCamera;

    private void Start()
    {
        CinemachineBrain brain = CinemachineBrain.GetActiveBrain(0);

        if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera cmCamera)
        {
            this.cmCamera = cmCamera;
        }
    }

    public override void OnPlayerStand(Transform bloxer, int height)
    {
        GetComponent<MeshRenderer>().material = pressedMaterial;

        StartCoroutine(Win(bloxer));
    }

    const float RISE_DISTANCE = 10;
    private IEnumerator Win(Transform player)
    {
        cmCamera.GetComponent<LockCameraY>().enabled = false;

        Vector3 tileStartPosition = transform.position;
        Vector3 playerStartPosition = player.position;
        Vector3 endPosition = transform.position + Vector3.up * RISE_DISTANCE;
        Vector3 playerEndPosition = player.position + Vector3.up * RISE_DISTANCE;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * riseSpeed;
            transform.position = Vector3.Lerp(tileStartPosition, endPosition, time);
            player.position = Vector3.Lerp(playerStartPosition, playerEndPosition, time);
            yield return null;
        }
    }
}
