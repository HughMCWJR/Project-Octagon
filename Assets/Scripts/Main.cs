using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    
    // Instantiate player objects
    private Player leftPlayer;
    private Player rightPlayer;

    // Whos turn is it
    private bool leftPlayersTurn = true;

    // Prefabs
    [SerializeField] private Transform octagonPrefab;
    [SerializeField] private Transform squarePrefab;

    // Sprites
    [SerializeField] private Sprite[] octagonSprites;
    [SerializeField] private Sprite[] squareSprites;

    // Constants for types of tiles
    // Acts as index for tile sprite arrays
    const int mountain = 0;
    const int desert = 0;

    // Distance between centers of tiles
    [SerializeField] const float TILE_HEIGHT = 0.72f;
    [SerializeField] const float TILE_WIDTH = 0.72f;

    // Side lengths of grid, makes an irreregular hexagon shaped board
    [SerializeField] const int GRID_WIDTH = 11;
    [SerializeField] const int GRID_HEIGHT = 13;

    // Constants for directions
    const int N  = 0;
    const int NE = 1;
    const int E  = 2;
    const int SE = 3;
    const int S  = 4;
    const int SW = 5;
    const int W  = 6;
    const int NW = 7;

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
                octagons.Add(new Vector2(j, i), Instantiate(octagonPrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT)), Quaternion.identity).gameObject.GetComponent<Octagon>());

                // TEMPORARY
                // OCTAGON TYPE ASSIGNMENT
                int randType = Random.Range(0, 2);
                octagons[new Vector2(j, i)].setType(randType, octagonSprites[randType]);

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

                Square square = Instantiate(squarePrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i + 1) * TILE_HEIGHT), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i + 1) * TILE_HEIGHT)), Quaternion.identity).gameObject.GetComponent<Square>();

                setNeighbor(octagon.Value, square, S);

                // TEMPORARY
                // SQUARE TYPE ASSIGNMENT
                int randType = Random.Range(0, 2);
                square.setType(randType, squareSprites[randType]);

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set neighbor pairing in both tiles
    // @param: tile, other tile, direction from first tile to other tile
    void setNeighbor(Tile tile, Tile otherTile, int direction)
    {

        tile.addNeighbor(direction, otherTile);
        otherTile.addNeighbor(getOppositeDirection(direction), tile);

    }

    // Set neighbor pairing in both octagons
    // @param: octagon, other octagon, direction from first octagon to other octagon
    void setNeighborOctagon(Octagon octagon, Octagon otherOctagon, int direction)
    {

        octagon.addNeighborOctagon(direction, otherOctagon);
        otherOctagon.addNeighborOctagon(getOppositeDirection(direction), octagon);

    }

    // Set neighbor pairing for octagon and square
    // @param: octagon, square, direction from octagon to square
    void setNeighborSquare(Octagon octagon, Square square, int direction)
    {

        octagon.addNeighborSquare(direction, square);
        square.addNeighbor(getOppositeDirection(direction), octagon);

    }

    // Get opposite direction
    // @param: direction to find opposite of
    int getOppositeDirection(int direction)
    {

        int oppDir = direction + 4;

        if (oppDir < 8)
        {
            return oppDir;
        }

        return oppDir - 8;

    }
}
