using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Tile
{
    [SerializeField] List<GateTile> gateTiles = new List<GateTile>();

    private bool pressed = false;

    public override void OnPlayerStand(Transform player)
    {
        if (!pressed)
        {
            pressed = true;

            foreach (GateTile gateTile in gateTiles)
            {
                gateTile.Open();
            }
        }
    }
}
