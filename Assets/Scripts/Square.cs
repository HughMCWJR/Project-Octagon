using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Tile
{

    private int building;

    [SerializeField] private Sprite[] tileSprites;

    // Sprites for ownable buildings
    // Structured so that each sprite is at "(building * playerCount) + owner + 1"
    // This allows each player and building combination to be represented
    // "owner + 1" allows there to be a sprite that represents no owner
    [SerializeField] private Sprite[] ownableBuildingSprites;

    // Sprites for buildings that do not change based on who owns them
    // Structured so that each sprite is at "building - Number of ownable buildings"
    [SerializeField] private Sprite[] nuetralBuildingSprites;

    public override void setTerrain(int terrain)
    {
        this.terrain = terrain;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tileSprites[terrain];
    }

    // Called every frame mouse if over, used for checking if clicked
    void OnMouseOver()
    {

        // If clicked, see what action is trying to be taken
        if (Input.GetMouseButtonDown(0))
        {

            if (main.getCurrentTurnMode() == Main.BUILD_MODE)
            {

                // Try and build
                if (terrain == Main.DESERT && building == Main.NONE)
                {

                    // Current player building
                    int buildingPlayer = main.getCurrentTurn() ? Main.LEFT_PLAYER : Main.RIGHT_PLAYER;

                    // Check that this player owns this empty square
                    if (owner == buildingPlayer || owner == Main.BOTH_PLAYERS)
                    {

                        if (main.tryBuild(main.getChosenBuilding()))
                        {
                            setBuilding(main.getChosenBuilding());
                            checkOwnership();
                            main.updateBuildingCounters();

                        }

                    }

                }

            } else if (main.getCurrentTurnMode() == Main.ARMORY_MODE)
            {

                // Try and destroy building
                if (building != Main.NONE)
                {

                    // See if current player owns surrounding octagon
                    int armoryUsingPlayer = main.getCurrentTurn() ? Main.LEFT_PLAYER : Main.RIGHT_PLAYER;

                    bool inArmoryRange = false;

                    foreach (KeyValuePair<int, Tile> tile in neighbors)
                    {

                        if (tile.Value.getOwner() == armoryUsingPlayer)
                        {

                            inArmoryRange = true;
                            break;

                        }

                    }

                    if (inArmoryRange)
                    {

                        if (main.tryUseArmory())
                        {
                            main.updateBuildingCount(getOwner(), building, false);
                            setBuilding(Main.NONE);
                            setTerrain(Main.DESERT);
                            checkOwnership();
                            main.updateBuildingCounters();

                        }

                    }

                }

            }
            else if (main.getCurrentTurnMode() == Main.REINFORCE_MODE)
            {

                // See if current player owns surrounding octagon
                int reinforceUsingPlayer = main.getCurrentTurn() ? Main.LEFT_PLAYER : Main.RIGHT_PLAYER;

                bool inReinforceRange = false;

                foreach (KeyValuePair<int, Tile> tile in neighbors)
                {

                    if (tile.Value.getOwner() == reinforceUsingPlayer)
                    {

                        inReinforceRange = true;
                        break;

                    }

                }
                if (inReinforceRange)
                {

                    if (terrain == Main.DESERT && building == Main.NONE)
                    {

                        if (main.tryUseReinforcement())
                        {
                            if (checkSurrounded() > -1)
                            {
                                setBuilding(Main.BUNKER);
                                if (main.getCurrentTurn())
                                {
                                    main.updateBuildingCount(1, 2, true); //For some reason, Main.BUNKER doesn't work here. So it's hardcoded
                                } else
                                {
                                    main.updateBuildingCount(0, 2, true); //For some reason, Main.BUNKER doesn't work here. So it's hardcoded
                                }
                            } else
                            {
                                setBuilding(Main.BUNKER);
                                checkOwnership();
                            }
                            main.updateBuildingCounters();
                        }

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
        // or if next to a city owned by the player
        // Otherwise show ownership to player who owns most surrounding Octagons
        if (building == Main.NONE)
        {

            int surroundingPlayer = checkSurrounded();

            if (surroundingPlayer != Main.NO_ONE)
            {

                setOwner(surroundingPlayer);

            } else
            {

                int foundOwner = Main.NO_ONE;

                foreach (KeyValuePair<int, Tile> tile in neighbors)
                {

                    if (tile.Value.getTerrain() == Main.CITY)
                    {

                        if (foundOwner == Main.NO_ONE || foundOwner == tile.Value.getOwner())
                        {

                            foundOwner = tile.Value.getOwner();

                        } else
                        {

                            foundOwner = Main.BOTH_PLAYERS;

                        }

                    }

                }

                setOwner(foundOwner);

            }

        } else
        {

            // Go through each neighbor and count how many neighbors each player owns
            Dictionary<int, int> ownedPerPlayer = new Dictionary<int, int>();

            foreach (KeyValuePair<int, Tile> tile in neighbors)
            {

                int player = tile.Value.getOwner();

                if (player != Main.NO_ONE)
                {

                    if (ownedPerPlayer.ContainsKey(player))
                    {

                        ownedPerPlayer[player]++;

                    }
                    else
                    {

                        ownedPerPlayer.Add(player, 1);

                    }

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

    // Find player that surrounds square or owns neighboring city, returns Main.NO_ONE if no one does
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

            if (tile.Value.terrain == Main.CITY)
            {
                possiblePlayer = tile.Value.getOwner();
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
                srend.color = rightPlayerColor;
                break;
            case Main.LEFT_PLAYER:
                srend.color = leftPlayerColor;
                break;
            case Main.BOTH_PLAYERS:
                srend.color = Color.magenta;
                break;
        }

    }

    public void setBuilding(int building)
    {

        this.building = building;

        if (building != Main.NONE)
        {

            // Check if building is ownable or not, changes sprite assignment process
            if (building >= 5)
            {

                this.gameObject.GetComponent<SpriteRenderer>().sprite = nuetralBuildingSprites[building - Main.NUM_TYPE_BUILDINGS];

            } else
            {

                // Formula for sprite location explained where sprite array is declared
                this.gameObject.GetComponent<SpriteRenderer>().sprite = ownableBuildingSprites[(building * Main.PLAYER_COUNT) + owner + 1];

            }

        } else
        {

            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;

        }

    }

    public int getBuilding()
    {
        return building;
    }

}
