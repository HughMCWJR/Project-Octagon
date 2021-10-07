using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Tile
{

    private int building;

    [SerializeField] private Sprite[] tileSprites;
    // Structured so that each sprite is at "(building * playerCount) + owner + 1"
    // This allows each player and building combination to be represented
    // "owner + 1" allows there to be a sprite that represents no owner
    [SerializeField] private Sprite[] buildingSprites;

    public override void setTerrain(int terrain)
    {
        this.terrain = terrain;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tileSprites[terrain];
    }

    // Called every frame mouse if over, used for checking if clicked
    void OnMouseOver()
    {

        if (terrain == Main.DESERT && building == Main.NONE)
        {

            // Current player building
            int buildingPlayer = main.getCurrentTurn() ? Main.LEFT_PLAYER : Main.RIGHT_PLAYER;

            // Check that building player has square surrounded
            bool surrounded = checkSurrounded() == buildingPlayer;

            if (surrounded)
            {

                // If clicked, try and build on square if available
                if (Input.GetMouseButtonDown(1))
                {

                    if (main.tryBuild(Main.FACTORY))
                    {

                        setBuilding(Main.FACTORY);

                    }

                }
                else if (Input.GetMouseButtonDown(0))
                {

                    if (main.tryBuild(Main.ARMORY))
                    {
                        
                        setBuilding(Main.ARMORY);

                    }

                }

            }

        }

    }

    // Checks neighbors to see which player now owns this tile and updates it as necessary
    // Also updates players
    public void checkOwnership()
    {

        // If square has no building, then only set ownership if completely surrounded
        // Otherwise show ownership to player who owns most surrounding Octagons
        if (building == Main.NONE)
        {

            setOwner(checkSurrounded());

        } else
        {

            // Go through each neighbor and count how many neighbors each player owns
            Dictionary<int, int> ownedPerPlayer = new Dictionary<int, int>();

            foreach (KeyValuePair<int, Tile> tile in neighbors)
            {

                int player = tile.Value.getOwner();

                if (ownedPerPlayer.ContainsKey(player))
                {

                    ownedPerPlayer[player]++;

                }
                else
                {

                    ownedPerPlayer.Add(player, 1);

                }

            }

            // Now it goes through each player and sees which player owns the most
            int maxOwned = -1;
            int indexOfMax = Main.NO_ONE;

            foreach (KeyValuePair<int, int> player in ownedPerPlayer)
            {

                if (player.Value > maxOwned)
                {

                    maxOwned = player.Value;
                    indexOfMax = player.Key;

                }
                else if (player.Value == maxOwned)
                {

                    maxOwned = -1;
                    indexOfMax = Main.NO_ONE;

                }

            }

            // Applies which player owns it
            setOwner(indexOfMax);

        }

    }

    // Find player that surrounds square, returns Main.NO_ONE if no one does
    private int checkSurrounded()
    {

        int possiblePlayer = Main.NO_ONE;

        bool surrounded = true;

        foreach (KeyValuePair<int, Tile> tile in neighbors)
        {

            if (tile.Value.getOwner() == Main.NO_ONE)
            {

                surrounded = false;
                break;

            }

            if (possiblePlayer == Main.NO_ONE)
            {

                possiblePlayer = tile.Value.getOwner();

            } else
            {

                if (tile.Value.getOwner() != possiblePlayer)
                {

                    surrounded = false;
                    break;

                }

            }

        }

        return surrounded ? possiblePlayer : Main.NO_ONE;

    }

    public override void setOwner(int newOwner, bool firstTurn = false)
    {

        // If it has a building and new owner is different from old owner than update players
        if (building != Main.NONE && newOwner != owner && !firstTurn)
        {

            if (newOwner == Main.NO_ONE)
            {

                main.updateBuildingCount(owner, building, false);

            } else
            {

                main.updateBuildingCount(newOwner, building, true);

            }

        }

        owner = newOwner;

        // Update sprite
        setBuilding(building);

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

    }

    public void setBuilding(int building)
    {

        this.building = building;

        if (building != Main.NONE)
        {

            // Formula for sprite location explained where sprite array is declared
            this.gameObject.GetComponent<SpriteRenderer>().sprite = buildingSprites[(building * Main.PLAYER_COUNT) + owner + 1];

        } else
        {

            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;

        }

    }

}
