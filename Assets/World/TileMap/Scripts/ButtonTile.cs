using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Tile
{
    [SerializeField] List<MovingTile> gateTiles = new List<MovingTile>();
    [SerializeField] bool isSwitch = false;
    [SerializeField] Transform lever;

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

            if (lever != null)
            {
                lever.transform.position = transform.position;
            }

            OpenGates();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSwitch)
        {
            OpenGates();
            if (lever != null)
            {
                lever.transform.position = transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isSwitch)
        {
            OpenGates();
            if (lever != null)
            {
                lever.transform.position = transform.position + Vector3.up * transform.localScale.y * 0.5f;
            }
        }
    }
}
