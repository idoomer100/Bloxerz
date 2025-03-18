using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Tile
{
    [SerializeField] List<MovingTile> gateTiles = new List<MovingTile>();
    [SerializeField] Transform pushedPart;
    [SerializeField] Material pressedMaterial;
    
    MeshRenderer mesh;

    bool pressed = false;

    private void Start()
    {
        mesh = pushedPart.GetComponent<MeshRenderer>();    
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
        if (!pressed)
        {
            pressed = true;

            if (pushedPart != null)
            {
                pushedPart.transform.position = transform.position;
            }

            OpenGates();

            mesh.material = pressedMaterial;
        }
    }
}
