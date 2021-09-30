using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octagon : Tile
{

    [SerializeField] private Sprite[] sprites;

    private List<int> octagonNeighbors = new List<int>();
    private List<int> squareNeighbors  = new List<int>();

    // Called every frame mouse if over, used for checking if clicked
    void OnMouseOver()
    {

        // If clicked, try and attack octagon if available
        if (terrain == Main.DESERT && Input.GetMouseButtonDown(0))
        {

            // Current player attacking
            int attackingPlayer = main.getCurrentTurn() ? Main.LEFT_PLAYER : Main.RIGHT_PLAYER;

            // Check that attacking player has octagon that can reach this one
            bool hasNeighber = false;

            foreach (int octagon in octagonNeighbors)
            {

                if (neighbors[octagon].getOwner() == attackingPlayer)
                {

                    hasNeighber = true;
                    break;

                }

            }

            if (hasNeighber)
            {

                if (owner == Main.NO_ONE)
                {

                    if (main.tryClaimOctagon())
                    {

                        setOwner(attackingPlayer);

                    }

                }
                else if (owner != attackingPlayer)
                {

                    if (main.tryAttackOctagon(owner))
                    {

                        setOwner(Main.NO_ONE);

                    }

                }

            }

        }

    }

    // Add neighbor to neighbors and add index to appropriate arrays for squares and octagons
    public override void addNeighbor(int direction, Tile tile)
    {

        neighbors.Add(direction, tile);

        if (tile is Octagon)
        {

            octagonNeighbors.Add(direction);

        } else
        {

            squareNeighbors.Add(direction);

        }

    }

    public override void setTerrain(int terrain)
    {
        this.terrain = terrain;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[terrain];
    }

    // Set owner of this tile and change tint of child tile
    public override void setOwner(int newOwner, bool firstTurn = false)
    {

        owner = newOwner;

        SpriteRenderer srend = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

        switch (owner)
        {
            case Main.NO_ONE:
                srend.color = Color.white;
                break;
            case Main.RIGHT_PLAYER:
                srend.color = Color.red;
                break;
            case Main.LEFT_PLAYER:
                srend.color = Color.blue;
                break;
        }

        // Updates surrounding squares to see if ownership changes
        foreach (int square in squareNeighbors) {

            ((Square)neighbors[square]).checkOwnership();

        }

    }

}
