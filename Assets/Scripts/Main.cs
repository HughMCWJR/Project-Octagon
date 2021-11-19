using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    // Instantiate player objects
    private Player leftPlayer;
    private Player rightPlayer;

    // Constants for player ownership
    public const int NO_ONE = -1;
    public const int RIGHT_PLAYER = 0;
    public const int LEFT_PLAYER = 1;
    public const int BOTH_PLAYERS = 2;

    // 1 + Number of players who have unique sprites for buildings
    // The additional 1 is because no one owning the building is counted as a player
    public const int PLAYER_COUNT = 3;

    // Whos turn is it
    private bool leftPlayersTurn = false;

    // What turn the game is on
    private int currentTurnCount = 0;

    // What mode the turn is currently in
    private int turnMode;

    // Constants for turn mode
    public const int ATTACK_MODE = 0;
    public const int ARMORY_MODE = 1;
    public const int BUILD_MODE  = 2;
    public const int MORTAR_MODE = 3;

    // Prefabs
    [SerializeField] private Transform octagonPrefab;
    [SerializeField] private Transform squarePrefab;

    // Text
    [SerializeField] private GameObject PrimaryMovesLeft;
    [SerializeField] private GameObject SecondaryMovesLeft;

    // Buttons
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject buildButton;

    [SerializeField] private GameObject nextTurnButton;

    [SerializeField] private GameObject chooseAttackButton;
    [SerializeField] private GameObject chooseArmoryButton;
    [SerializeField] private GameObject chooseMortarButton;

    [SerializeField] private GameObject Select_Barrack;
    [SerializeField] private GameObject Select_Factory;
    [SerializeField] private GameObject Select_Bunker;
    [SerializeField] private GameObject Select_Armory;
    [SerializeField] private GameObject Select_Mortar;
    [SerializeField] private GameObject Select_Project;


    // Chosen building for when trying to build
    private int chosenBuilding;

    // TEMP
    // DISPLAY TEXT
    [SerializeField] private Text primaryMovesLeftText;
    [SerializeField] private Text secondaryMovesLeftText;
    [SerializeField] private Text whosTurnText;

    // Octagons
    // Key is position with x being from left side of layer, y being layers top to bottom
    private Dictionary<Vector2, Octagon> octagons;

    // Constants for terrains of tiles
    // Acts as index for tile sprite arrays
    public const int MOUNTAIN = 0;
    public const int DESERT   = 1;
    public const int WATER    = 2;
    public const int CITY     = 3;

    // Distance between centers of tiles
    const float TILE_HEIGHT = 0.9f;
    const float TILE_WIDTH = 0.9f;

    // Side lengths of grid, makes an irreregular hexagon shaped board
    const int GRID_WIDTH = 12;
    const int GRID_HEIGHT = 19;

    // Float for justifying the board's position
    const float BOARD_Y = 0.15f;

    // Side length of starting square for players
    const int STARTING_SIZE = 3;

    // Constants for directions
    public const int N  = 0;
    public const int NE = 1;
    public const int E  = 2;
    public const int SE = 3;
    public const int S  = 4;
    public const int SW = 5;
    public const int W  = 6;
    public const int NW = 7;

    // Constants for buildings
    // Number of type of ownable buildings
    public const int NUM_TYPE_BUILDINGS = 5;
    // Index for each building type in sprite array
    public const int NONE = -1;
    public const int FACTORY = 0;
    public const int BARRACKS = 1;
    public const int BUNKER = 2;
    public const int ARMORY = 3;
    public const int MORTAR = 4;
    public const int BRIDGE = 5;
    public const int RUIN = 6;

    // Names of maps to be loaded
    // Text files are layed out in two lines, first for octagons, second for squares
    // The strings are written in order of how the tiles are instantiated
    private readonly string[] mapNames = new string[] { "desert", "test", "Map1_Gulch", "Map2_Ridge"};

    // Dictionary for maps
    private Dictionary<string, string[]> maps;

    // TEMP
    // SELECTS MAP
    [SerializeField] private  string SELECTED_MAP;

    // Constants for map indexes
    const int OCTAGON = 0;
    const int SQUARE  = 1;

    // Start is called before the first frame update
    void Start()
    {

        // Instantiate player objects
        leftPlayer  = new Player(true);
        rightPlayer = new Player(false);

        // Set starting value for chosen building
        chosenBuilding = BARRACKS;

        // Dictionary for octagons, keys are their positions on a diagonal grid with 0,0 being the top left (used j and i from spawnGrid() to create these coordinates)
        octagons = new Dictionary<Vector2, Octagon>();

        // Instantiate octagons and squares
        // Board centered at global position 0,0,0
        for (int i = 0; i < GRID_HEIGHT; i++)
        {

            // Find width of this layer
            int width = GRID_WIDTH - (Mathf.Abs((GRID_HEIGHT / 2) - i));

            for (int j = 0; j < width; j++)
            {

                // Octagons are placed left to right, top to bottom
                octagons.Add(new Vector2(j, i), instantiateTile<Octagon>(octagonPrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT) + BOARD_Y, ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT))));

            }

        }

        int middleI = (int)((GRID_HEIGHT / 2.0f) - 0.5f);

        // Spawn squares
        // For each octagon, set their neighbor variables
        // For each square only set northern neighbor, will set others afterwards
        foreach (KeyValuePair<Vector2, Octagon> octagon in octagons)
        {

            int j = (int)octagon.Key.x;
            int i = (int)octagon.Key.y;

            // Find width of this layer
            int width = GRID_WIDTH - (Mathf.Abs((GRID_HEIGHT / 2) - i));

            // Boolean if octagon has both southern neighbors
            bool hasBothNeighbors = true;

            // Set SW Neighbor if applicable
            if ((i < middleI || j != 0) && i != GRID_HEIGHT - 1)
            {

                Octagon swOctagon;

                if (i >= middleI)
                {

                    swOctagon = octagons[new Vector2(j - 1, i + 1)];

                }
                else
                {

                    swOctagon = octagons[new Vector2(j, i + 1)];

                }

                setNeighbor(octagon.Value, swOctagon, SW);

            }
            else
            {

                hasBothNeighbors = false;

            }

            // Set SE Neighbor if applicable
            if ((i < middleI || j < width - 1) && i != GRID_HEIGHT - 1)
            {

                Octagon seOctagon;

                if (i >= middleI)
                {

                    seOctagon = octagons[new Vector2(j, i + 1)];

                }
                else
                {

                    seOctagon = octagons[new Vector2(j + 1, i + 1)];

                }

                setNeighbor(octagon.Value, seOctagon, SE);

            }
            else
            {

                hasBothNeighbors = false;

            }

            // Spawn squares below octagons if applicable
            if (i < GRID_HEIGHT - 2 && hasBothNeighbors)
            {

                // Extra 0.16f fixes issue with misalignment
                Square square = instantiateTile<Square>(squarePrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i + 1) * TILE_HEIGHT) + 0.16f + BOARD_Y, ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i + 1) * TILE_HEIGHT) + 0.16f));

                // Set neighbor for octagon and square
                setNeighbor(octagon.Value, square, S);

                // Set square to have no building on it
                square.setBuilding(NONE);

            }

        }

        // Set non-northern neighbors for squares
        foreach (KeyValuePair<Vector2, Octagon> octagon in octagons)
        {

            if (octagon.Value.getNeighbors().ContainsKey(S))
            {

                Square square = (Square) octagon.Value.getNeighbors()[S];

                // Set neighbors, we know they exist because every square is surrounded by 4 octagons
                setNeighbor(octagon.Value.getNeighbors()[SW], square, E);
                setNeighbor(octagon.Value.getNeighbors()[SE], square, W);
                setNeighbor(octagon.Value.getNeighbors()[SW].getNeighbors()[SE], square, N);

            }

        }

        // Set starting tiles for players
        int tilesThisLayer = STARTING_SIZE;

        while (tilesThisLayer != 0)
        {

            for (int i = 0; i < tilesThisLayer; i++)
            {

                // Difference between current layer and starting layer
                int difference = STARTING_SIZE - tilesThisLayer;

                // Set lower layer
                octagons[new Vector2(i, middleI + difference)].setOwner(LEFT_PLAYER);
                octagons[new Vector2(GRID_WIDTH - 1 - i - difference, middleI + difference)].setOwner(RIGHT_PLAYER);

                // Set upper layer
                octagons[new Vector2(i, middleI - difference)].setOwner(LEFT_PLAYER);
                octagons[new Vector2(GRID_WIDTH - 1 - i - difference, middleI - difference)].setOwner(RIGHT_PLAYER);

            }

            tilesThisLayer--;

        }

        // Load map files
        maps = loadMaps(mapNames);

        // Assign map terrains
        setMap(SELECTED_MAP);

        // Start first turn
        nextTurn();

    }

    // Update is called once per frame
    void Update()
    {

        // Checks for inputs
        if (Input.GetKey("escape"))
        {

            Application.Quit();

        } else if (Input.GetKeyDown("space"))
        {

            nextTurn();

        } else if (Input.GetKeyDown("a"))
        {

            startAttackTurn();

        } else if (Input.GetKeyDown("b"))
        {

            startBuildTurn();

        } else if (Input.GetKeyDown("1"))
        {

            setChosenBuilding(BARRACKS);

        } else if (Input.GetKeyDown("2"))
        {

            setChosenBuilding(FACTORY);

        } else if (Input.GetKeyDown("3"))
        {

            setChosenBuilding(BUNKER);

        } else if (Input.GetKeyDown("4"))
        {

            setChosenBuilding(ARMORY);

        } else if (Input.GetKeyDown("5"))
        {

            setChosenBuilding(MORTAR);

        }

    }

    // Go to next turn
    public void nextTurn()
    {

        //nextTurnButton.SetActive(false);

        // Increase number of turns if it was just now red's turn
        if (!leftPlayersTurn) {
            currentTurnCount++;
        }

        leftPlayersTurn = !leftPlayersTurn;

        whosTurnText.text = leftPlayersTurn ? "Blue's Turn" : "Red's Turn";

        // Toggle needed and unneeded buttons
        attackButton.SetActive(true);
        buildButton.SetActive(true);

        chooseAttackButton.SetActive(false);
        chooseArmoryButton.SetActive(false);
        chooseMortarButton.SetActive(false);
        Select_Barrack.SetActive(false);
        Select_Factory.SetActive(false);
        Select_Bunker.SetActive(false);
        Select_Armory.SetActive(false);
        Select_Mortar.SetActive(false);
        Select_Project.SetActive(false);
        PrimaryMovesLeft.SetActive(false);
        SecondaryMovesLeft.SetActive(false);

    }

    // Start attack turn for player
    public void startAttackTurn()
    {

        // Only start turn if buttons are on
        if (attackButton.activeSelf)
        {

            // Turn off buttons
            attackButton.SetActive(false);
            buildButton.SetActive(false);

            // Tell correct player to start
            getCurrentPlayer().startAttackTurn();

            // Set default turn mode
            turnMode = ATTACK_MODE;

            // Enable turn mode buttons
            chooseAttackButton.SetActive(true);
            chooseArmoryButton.SetActive(true);

            // Enable counters and place them in the right position
            PrimaryMovesLeft.SetActive(true);
            SecondaryMovesLeft.SetActive(true);

            //TEMP
            primaryMovesLeftText.text = getCurrentPlayer().getAttacks().ToString();
            secondaryMovesLeftText.text = getCurrentPlayer().getArmoryAttacks().ToString();

        }

    }

    // Start build turn for player
    public void startBuildTurn()
    {

        // Only start turn if buttons are on
        if (attackButton.activeSelf)
        {

            // Turn off buttons
            attackButton.SetActive(false);
            buildButton.SetActive(false);

            // Tell correct player to start
            getCurrentPlayer().startBuildTurn();

            // Set default turn mode
            turnMode = BUILD_MODE;

            // Set chosen building to barracks by default
            chosenBuilding = BARRACKS;

            // Enable turn mode buttons
            chooseMortarButton.SetActive(true);
            Select_Barrack.SetActive(true);
            Select_Factory.SetActive(true);
            Select_Bunker.SetActive(true);
            Select_Armory.SetActive(true);
            Select_Mortar.SetActive(true);
            Select_Project.SetActive(true);

            // Enable counters and place them in the right position
            PrimaryMovesLeft.SetActive(true);
            SecondaryMovesLeft.SetActive(true);

            //TEMP
            primaryMovesLeftText.text = getCurrentPlayer().getBuilds().ToString();
            secondaryMovesLeftText.text = getCurrentPlayer().getMortarAttacks().ToString();

        }

    }

    // Set turn mode
    public void setTurnMode(int newTurnMode)
    {
        turnMode = newTurnMode;
    }

    // See if player can claim neutral octagon, if so tell player that octagon has been claimed
    // @param:  attacksNeeded = Integer number of needed attacks
    //          attacksUsed   = Integer number of attacks used on claim, default is -1 which means use attacksNeeded instead
    // @return: true if octagon can be claimed, false otherwise
    public bool tryClaimOctagon(int attacksNeeded, int attacksUsed = -1)
    {

        if  (getCurrentPlayer().getAttacks() < attacksNeeded)
        {

            return false;

        }

        // Subtract attacks used which is attacks needed unless otherwise set
        int attacksLeft = getCurrentPlayer().attackedOctagon(attacksUsed == -1 ? attacksNeeded : attacksUsed);

        //TEMP
        primaryMovesLeftText.text = attacksLeft.ToString();

        return true;

    }

    // See if player can attack the claimed octagon, if so tell player that octagon has been attacked
    // @param:  owner         = current owner of octagon
    //          attacksNeeded = Integer number of needed attacks
    // @return: true if octagon can be attacked, false otherwise
    public bool tryAttackOctagon(int owner, int attacksNeeded)
    {

        if (getCurrentPlayer().getAttacks() < attacksNeeded)
        {

            return false;

        }

        // Remove octagon from the owner
        /*
        switch (owner)
        {
            case LEFT_PLAYER:
                leftPlayer.loseOctagon();
                break;
            case RIGHT_PLAYER:
                rightPlayer.loseOctagon();
                break;
        }
        */

        int attacksLeft = getCurrentPlayer().attackedOctagon(attacksNeeded);

        //TEMP
        primaryMovesLeftText.text = attacksLeft.ToString();

        return true;

    }

    // See if player can use armory, if so tell player that armory has been used
    // @return: true if armory was used, false otherwise
    public bool tryUseArmory()
    {

        if (getCurrentPlayer().getArmoryAttacks() == 0)
        {

            return false;

        }

        int armoryAttacksLeft = getCurrentPlayer().usedArmory();

        secondaryMovesLeftText.text = armoryAttacksLeft.ToString();

        return true;

    }

    // See if player can build, if so tell player that building has been built
    // @param:  building         = building trying to be built
    // @return: true if building was built, false otherwise
    public bool tryBuild(int building)
    {

        if (getCurrentPlayer().getBuilds() == 0)
        {

            return false;

        }

        int buildsLeft = getCurrentPlayer().built(building);

        //TEMP
        primaryMovesLeftText.text = buildsLeft.ToString();

        return true;

    }

    // See if player can use mortar, if so tell player that mortar has been used
    // @return: true if mortar was used, false otherwise
    public bool tryUseMortar()
    {

        if (getCurrentPlayer().getMortarAttacks() == 0)
        {

            return false;

        }

        int mortarAttacksLeft = getCurrentPlayer().usedMortar();

        secondaryMovesLeftText.text = mortarAttacksLeft.ToString();

        return true;

    }

    // Update building count for player
    // @param: player that needs to be updated, what building, true if building increasing by 1 and false if decreasing by one
    public void updateBuildingCount(int player, int building, bool increase)
    {

        // Only update if building is ownable
        if (building < NUM_TYPE_BUILDINGS)
        {

            switch (player)
            {
                case LEFT_PLAYER:
                    leftPlayer.changeBuildingCount(building, increase);
                    break;
                case RIGHT_PLAYER:
                    rightPlayer.changeBuildingCount(building, increase);
                    break;
            }

        }

    }

    public void setChosenBuilding(int building)
    {

        chosenBuilding = building;

    }

    public  int getChosenBuilding()
    {

        return chosenBuilding;

    }

    // Instantiate a tile
    // @param: object type, prefab to instantiate, vector3 position to instantiate at
    // @return: object of class of tile
    private T instantiateTile<T>(Transform prefab, Vector3 position) where T : Tile 
    {

        T tile = Instantiate(prefab, position, Quaternion.identity).gameObject.GetComponent<T>();

        // Gives tile this instance of Main
        tile.setMain(this);

        // Each tile starts with no owner
        tile.setOwner(NO_ONE, true);

        return tile;

    }

    // Set neighbor pairing in both tiles
    // @param: tile, other tile, direction from first tile to other tile
    private void setNeighbor(Tile tile, Tile otherTile, int direction)
    {

        tile.addNeighbor(direction, otherTile);
        otherTile.addNeighbor(getOppositeDirection(direction), tile);

    }

    // Get opposite direction
    // @param: direction to find opposite of
    public int getOppositeDirection(int direction)
    {

        int oppDir = direction + 4;

        if (oppDir < 8)
        {
            return oppDir;
        }

        return oppDir - 8;

    }

    // Returns current player taking turn
    private Player getCurrentPlayer()
    {
        return leftPlayersTurn ? leftPlayer : rightPlayer;
    }

    // Returns true if current player if leftPlayer, false otherwise
    public bool getCurrentTurn()
    {
        return leftPlayersTurn;
    }

    public int getCurrentTurnCount()
    {
        return currentTurnCount;
    }

    public int getCurrentTurnMode()
    {
        return turnMode;
    }

    private class Player
    {

        // If this player is left player
        private bool leftPlayer;

        // Array of buildings owned
        // Meaning of indexes are declared as constants
        private int[] numBuildings;

        // Number of actions currently available
        private int attacks;
        private int armoryAttacks;
        private int builds;
        private int mortarAttacks;

        public Player(bool leftPlayer)
        {
            
            this.leftPlayer = leftPlayer;
            this.numBuildings = new int[NUM_TYPE_BUILDINGS];
            this.attacks = 0;
            this.builds = 0;

        }

        public void startAttackTurn()
        {

            attacks = numBuildings[BARRACKS] + 1;
            armoryAttacks = numBuildings[ARMORY];

        }

        public void startBuildTurn()
        {

            builds = numBuildings[FACTORY] + 1;
            mortarAttacks = numBuildings[MORTAR];

        }

        // Update player after claiming octagon
        // @return: number of attacks left
        public int claimedOctagon()
        {

            return --attacks;

        }

        // Update player after attacking octagon
        // @param:  attackNeeded = Integer number of attacks needed to take the last action
        // @return: number of attacks left
        public int attackedOctagon(int attacksNeeded)
        {

            attacks -= attacksNeeded;
            return attacks;

        }

        public int usedArmory()
        {
            return --armoryAttacks;
        }

        // Update player after building a building
        // @return: number of builds left
        public int built(int building)
        {

            numBuildings[building]++;

            return --builds;

        }

        public int usedMortar()
        {
            return --mortarAttacks;
        }

        public int getAttacks()
        {
            return attacks;
        }

        public int getArmoryAttacks()
        {
            return armoryAttacks;
        }

        public int getBuilds()
        {
            return builds;
        }

        public int getMortarAttacks()
        {
            return mortarAttacks;
        }

        // Increase or decrease building count by 1
        // @param: which building, increase of decrease
        public void changeBuildingCount(int building, bool increase)
        {
            numBuildings[building] += increase ? 1 : -1;
        }

    }

    // Load map text files
    // @param:  mapNames = Array of String names of maps to be loaded
    // @return: Dictionary with String keys for Arrays that contain
    // the String for Octagon terrains and the String for Squares
    private Dictionary<string, string[]> loadMaps(string[] mapNames)
    {

        Dictionary<string, string[]> maps = new Dictionary<string, string[]>();

        foreach (string mapName in mapNames)
        {

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader("Assets/Map/" + mapName + ".txt"))
                {

                    maps.Add(mapName, new string[2]);

                    // Read Octagon tile terrains
                    maps[mapName][OCTAGON] = sr.ReadLine();

                    // Read Square tile terrains
                    maps[mapName][SQUARE] = sr.ReadLine();

                }

            }
            catch
            {
                Debug.Log("The map files could not be read");
            }

        }

        return maps;

    }

    // Set map terrain types to each tile
    // @param:  mapName = name of map to set it to
    private void setMap(string mapName)
    {

        // Index of map string for Octagon and Square terrains
        int octagonIndex = 0;
        int squareIndex = 0;

        // Go through every Octagon
        for (int i = 0; i < GRID_HEIGHT; i++)
        {

            // Find width of this layer
            int width = GRID_WIDTH - (Mathf.Abs((GRID_HEIGHT / 2) - i));

            for (int j = 0; j < width; j++)
            {

                // Set Octagon terrain
                // Increase octagon index
                octagons[new Vector2(j, i)].setTerrain(int.Parse(maps[mapName][OCTAGON].Substring(octagonIndex++, 1)));

                // Set Square terrain if Octagon has square below it
                // Increase square index
                if (octagons[new Vector2(j, i)].getNeighbors().ContainsKey(S))
                {

                    // If Square being given a starting building then assign desert as terrain
                    // then assign building
                    int index = int.Parse(maps[mapName][SQUARE].Substring(squareIndex++, 1));

                    if (index >= NUM_TYPE_BUILDINGS)
                    {

                        Square square = (Square)octagons[new Vector2(j, i)].getNeighbors()[S];

                        square.getNeighbors()[S].setTerrain(DESERT);
                        square.setBuilding(index);

                    } else
                    {

                        octagons[new Vector2(j, i)].getNeighbors()[S].setTerrain(index);

                    }

                }

            }

        }

    }

}
