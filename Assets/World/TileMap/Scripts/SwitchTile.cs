using System.Collections.Generic;
using UnityEngine;

public class SwitchTile : Tile
{
    [SerializeField] List<MovingTile> gateTiles = new List<MovingTile>();
    [SerializeField] Material pressedMaterial;

    private MeshRenderer mesh;
    private Material defaultMaterial;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        defaultMaterial = mesh.material;
    }

    private void OpenGates()
    {
        foreach (MovingTile gateTile in gateTiles)
        {
            gateTile.Open();
        }
    }

    public override void OnPlayerStand(Transform bloxer, int height)
    {
        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        OpenGates();
        mesh.material = pressedMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        OpenGates();
        mesh.material = defaultMaterial;
    }
}
