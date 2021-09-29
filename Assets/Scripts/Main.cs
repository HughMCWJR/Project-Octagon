using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    
    // Instantiate player objects
    private Player leftPlayer;
    private Player rightPlayer;

    // Constants for team ownership
    public const int NO_ONE = -1;
    public const int RIGHT_PLAYER = 0;
    public const int LEFT_PLAYER = 1;

    // Whos turn is it
    private bool leftPlayersTurn = false;

    // Prefabs
    [SerializeField] private Transform octagonPrefab;
    [SerializeField] private Transform squarePrefab;

    // REFACTORED TO OCTAGON AND SQUARE RESPECTIVELY
    // Sprites
    //[SerializeField] private Sprite[] octagonSprites;
    //[SerializeField] private Sprite[] squareSprites;

    // Buttons
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject buildButton;
    [SerializeField] private GameObject nextTurnButton;

    // Constants for terrains of tiles
    // Acts as index for tile sprite arrays
    public const int MOUNTAIN = 0;
    public const int DESERT   = 1;

    // Distance between centers of tiles
    [SerializeField] const float TILE_HEIGHT = 0.72f;
    [SerializeField] const float TILE_WIDTH = 0.72f;

    // Side lengths of grid, makes an irreregular hexagon shaped board
    [SerializeField] const int GRID_WIDTH = 11;
    [SerializeField] const int GRID_HEIGHT = 13;

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
    // Number of type of buildings
    const int NUM_TYPE_BUILDINGS = 2;
    // Index for each building type
    public const int NONE = -1;
    public const int FACTORY = 0;
    public const int ARMORY = 1;

    // Start is called before the first frame update
    void Start()
    {

        // Instantiate player objects
        leftPlayer  = new Player(true);
        rightPlayer = new Player(false);

        // Dictionary for octagons, keys are their positions on a diagonal grid with 0,0 being the top left (used j and i from spawnGrid() to create these coordinates)
        Dictionary<Vector2, Octagon> octagons = new Dictionary<Vector2, Octagon>();

        // Instantiate octagons and squares
        // Board centered at global position 0,0,0
        for (int i = 0; i < GRID_HEIGHT; i++)
        {

            // Find width of this layer
            int width = GRID_WIDTH - (Mathf.Abs((GRID_HEIGHT / 2) - i));

            for (int j = 0; j < width; j++)
            {

                // Octagons are placed left to right, top to bottom
                octagons.Add(new Vector2(j, i), instantiateTile<Octagon>(octagonPrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT))));

                // TEMPORARY
                // OCTAGON TYPE ASSIGNMENT
                int randTerrain = Random.Range(0, 5);
                randTerrain = randTerrain == 0 ? MOUNTAIN : DESERT;
                octagons[new Vector2(j, i)].setTerrain(randTerrain);

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

                Square square = instantiateTile<Square>(squarePrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i + 1) * TILE_HEIGHT), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i + 1) * TILE_HEIGHT)));

                // Set neighbor for octagon and square
                setNeighbor(octagon.Value, square, S);

                // TEMPORARY
                // SQUARE TYPE ASSIGNMENT
                int randTerrain = Random.Range(0, 5);
                randTerrain = randTerrain == 0 ? MOUNTAIN : DESERT;
                square.setTerrain(randTerrain);

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
        octagons[new Vector2(0, middleI)].setOwner(LEFT_PLAYER);
        octagons[new Vector2(GRID_WIDTH - 1, middleI)].setOwner(RIGHT_PLAYER);

        // TEMP
        // FIXES EDGE CASE WHERE STARTING OCTAGON IS MOUNATIN
        octagons[new Vector2(0, middleI)].setTerrain(DESERT);
        octagons[new Vector2(GRID_WIDTH - 1, middleI)].setTerrain(DESERT);

        // Start first turn
        nextTurn();

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Go to next turn
    public void nextTurn()
    {

        nextTurnButton.SetActive(false);

        leftPlayersTurn = !leftPlayersTurn;

        attackButton.SetActive(true);
        buildButton.SetActive(true);

    }

    // Start attack turn for player
    public void startAttackTurn()
    {

        // Turn off buttons
        attackButton.SetActive(false);
        buildButton.SetActive(false);

        // Tell correct player to start
        getCurrentPlayer().startAttackTurn();

    }

    // Start build turn for player
    public void startBuildTurn()
    {

        // Turn off buttons
        attackButton.SetActive(false);
        buildButton.SetActive(false);

        // Tell correct player to start
        getCurrentPlayer().startBuildTurn();

    }

    // See if player can claim nuetral octagon, if so tell player that octagon has been claimed
    // @return: true if octagon can be claimed, false otherwise
    public bool tryClaimOctagon()
    {

        if  (getCurrentPlayer().getAttacks() == 0)
        {

            return false;

        }

        // If there are 0 attacks left then set next turn button to active
        if (getCurrentPlayer().claimedOctagon() == 0)
        {

            nextTurnButton.SetActive(true);

        }

        return true;

    }

    // See if player can attack the claimed octagon, if so tell player that octagon has been attacked
    // @param: current owner of octagon
    // @return: true if octagon can be attacked, false otherwise
    public bool tryAttackOctagon(int owner)
    {

        if (getCurrentPlayer().getAttacks() == 0)
        {

            return false;

        }

        // Remove octagon from the owner
        switch (owner)
        {
            case LEFT_PLAYER:
                leftPlayer.loseOctagon();
                break;
            case RIGHT_PLAYER:
                rightPlayer.loseOctagon();
                break;
        }

        // If there are 0 attacks left then set next turn button to active
        if (getCurrentPlayer().attackedOctagon() == 0)
        {

            nextTurnButton.SetActive(true);

        }

        return true;

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
        tile.setOwner(NO_ONE);

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
    private int getOppositeDirection(int direction)
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

    private class Player
    {

        // If this player is left player
        private bool leftPlayer;

        // Number of octagons owened by player
        private int numOctagons;

        // Array of buildings owned
        // Meaning of indexes are declared as constants
        private int[] numBuildings;

        // Number of actions currently available
        private int attacks;
        private int builds;

        public Player(bool leftPlayer)
        {
            
            this.leftPlayer = leftPlayer;
            this.numOctagons = 0;
            this.numBuildings = new int[NUM_TYPE_BUILDINGS];
            this.attacks = 0;
            this.builds = 0;

        }

        public void startAttackTurn()
        {

            attacks = numBuildings[ARMORY] + 1;

        }

        // Update player after claiming octagon
        // @return: number of attacks left
        public int claimedOctagon()
        {

            numOctagons++;

            return --attacks;

        }

        // Update player after attacking octagon
        // @return: number of attacks left
        public int attackedOctagon()
        {

            return --attacks;

        }

        public void loseOctagon()
        {

            numOctagons--;

        }

        public int getAttacks()
        {
            return attacks;
        }

        public void startBuildTurn()
        {

            builds = numBuildings[FACTORY];

        }

    }

}
