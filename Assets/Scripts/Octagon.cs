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

                    if (main.tryClaimOctagon(findAttacksNeededToClaim()))
                    {

                        setOwner(attackingPlayer);

                    }

                }
                else if (owner != attackingPlayer)
                {

                    if (main.tryAttackOctagon(owner, findAttacksNeededToAttack()))
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

    // Find number of attacks needed to claim/attack
    // Changed by if surrounded by a bunker owned by the enemy
    public int findAttacksNeededToClaim()
    {

        int attacksNeeded = 1;

        // Neighbor squares with bunkers
        Square[] neighborBunkers = findNeighborBuilding(Main.BUNKER);

        // True if there is a neighboring bunker owned by an enemy
        bool enemyBunker = false;

        foreach (Square square in neighborBunkers)
        {

            if (square.getOwner() == (main.getCurrentTurn() ? Main.RIGHT_PLAYER : Main.LEFT_PLAYER))
            {

                enemyBunker = true;
                break;

            }

        }

        if (enemyBunker)
        {

            attacksNeeded++;

        }

        return attacksNeeded;

    }

    public int findAttacksNeededToAttack()
    {

        int attacksNeeded = 1;

        // Neighbor squares with bunkers
        Square[] neighborBunkers = findNeighborBuilding(Main.BUNKER);

        // True if there is a neighboring bunker owned by an enemy
        bool enemyBunker = false;

        foreach (Square square in neighborBunkers)
        {

            if (square.getOwner() == (main.getCurrentTurn() ? Main.RIGHT_PLAYER : Main.LEFT_PLAYER))
            {

                enemyBunker = true;
                break;

            }

        }

        if (enemyBunker)
        {

            attacksNeeded++;

        }

        return attacksNeeded;

    }

    // Return any Squares that have the building being looked for
    // @param:  building = Integer building
    // @return: Array of squares that have the building
    public Square[] findNeighborBuilding(int building)
    {

        Square[] foundSquares = new Square[4];
        int numFoundSquares = 0;

        foreach (int square in squareNeighbors)
        {

            if (((Square)neighbors[square]).getBuilding() == building)
            {

                foundSquares[numFoundSquares++] = ((Square)neighbors[square]);

            }

        }

        Square[] squares = new Square[numFoundSquares];

        for (int i = 0; i < numFoundSquares; i++)
        {

            squares[i] = foundSquares[i];

        }

        return squares;

    }

}
