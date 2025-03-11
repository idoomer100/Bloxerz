using UnityEngine;

public class GoalTile : Tile
{
    public override void OnPlayerStand(Transform player, int height)
    {
        print("YOU WINNNN"); //TODO: WIN
    }
}
