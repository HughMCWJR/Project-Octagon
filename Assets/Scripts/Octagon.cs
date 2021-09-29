using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octagon : Tile
{

    // Called every frame mouse if over, used for checking is clicked
    void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {

            if (main.tryClaimOctagon())
            {



            }

        }

    }

}
