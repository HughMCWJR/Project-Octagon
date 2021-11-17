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

        // If clicked, see what action is trying to be taken
        if (Input.GetMouseButtonDown(0))
        {

            if (main.getCurrentTurnMode() == Main.ATTACK_MODE)
            {

                // Try and attack octagon if available
                if (terrain != Main.MOUNTAIN)
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

                    // Also check for neighbors through bridges
                    foreach (int square in squareNeighbors)
                    {

                        Square squareObject = (Square)neighbors[square];

                        if (squareObject.getBuilding() == Main.BRIDGE)
                        {

                            if (neighbors[square].getNeighbors()[square].getOwner() == attackingPlayer)
                            {

                                hasNeighber = true;
                                break;

                            }

                        }

                    }

                    if (hasNeighber)
                    {

                        if (owner == Main.NO_ONE)
                        {

                            int attacksNeeded = findAttacksNeededToClaim();

                            if (main.tryClaimOctagon(attacksNeeded, terrain == Main.WATER ? attacksNeeded - 1 : -1))
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

            } else if (main.getCurrentTurnMode() == Main.MORTAR_MODE)
            {

                // Try and use the mortar on an octagon
                if (main.getCurrentTurnCount() > 2 && main.tryUseMortar())
                {

                    // For each octagon in a 3x3 area centered on this octagon,
                    // nuetralize each octagon without an owned building
                    for (int i = 0; i < 3; i++)
                    {

                        for (int j  = 0; j < 3; j++)
                        {

                            // Use i to choose first diagonal
                            Octagon rowOctagon;

                            if (i == 0)
                            {
                                rowOctagon = (Octagon)neighbors[Main.NE];
                            } else if (i == 1)
                            {
                                rowOctagon = this;
                            } else
                            {
                                rowOctagon = (Octagon)neighbors[Main.SW];
                            }

                            // Use j to choose second diagonal
                            Octagon chosenOctagon;

                            if (j == 0)
                            {
                                chosenOctagon = (Octagon)rowOctagon.getNeighbors()[Main.NW];
                            }
                            else if (j == 1)
                            {
                                chosenOctagon = rowOctagon;
                            }
                            else
                            {
                                chosenOctagon = (Octagon)rowOctagon.getNeighbors()[Main.SE];
                            }

                            // Test octagon to see if it needs to be nuetralized
                            bool needsToBeNeutralized = true;

                            foreach (int squareIndex in chosenOctagon.getSquareNeighbors())
                            {

                                Square squareNeighbor = (Square)chosenOctagon.getNeighbors()[squareIndex];

                                if (squareNeighbor.getBuilding() != Main.NONE && squareNeighbor.getBuilding() < Main.NUM_TYPE_BUILDINGS)
                                {

                                    needsToBeNeutralized = false;
                                    break;

                                }

                            }

                            if (needsToBeNeutralized)
                            {

                                chosenOctagon.setOwner(Main.NO_ONE);

                            }

                        }

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
        foreach (int squareIndex in squareNeighbors) {

            ((Square)neighbors[squareIndex]).checkOwnership();

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

    public List<int> getSquareNeighbors()
    {
        return squareNeighbors;
    }

}
