using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Tile
{
    [SerializeField] List<MovingTile> gateTiles = new List<MovingTile>();
    [SerializeField] bool isSwitch = false;

    private bool pressed = false;

    public override void OnPlayerStand(Transform player)
    {
        if (isSwitch || !pressed)
        {
            pressed = true;

            foreach (MovingTile gateTile in gateTiles)
            {
                gateTile.Open();
            }
        }
    }
}
