using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Tile
{
    [SerializeField] List<MovingTile> gateTiles = new List<MovingTile>();
    [SerializeField] bool isSwitch = false;

    bool pressed = false;

    private void OpenGates()
    {
        foreach (MovingTile gateTile in gateTiles)
        {
            gateTile.Open();
        }
    }

    public override void OnPlayerStand(Transform bloxer, int height)
    {
        if (!isSwitch && !pressed)
        {
            pressed = true;

            OpenGates();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSwitch)
        {
            OpenGates();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isSwitch)
        {
            OpenGates();
        }
    }
}
