using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // Prefabs
    [SerializeField] private Transform octagonPrefab;
    [SerializeField] private Transform squarePrefab;

    // Sprites
    [SerializeField] private Sprite[] octagonSprites;
    [SerializeField] private Sprite[] squareSprites;

    // Distance between centers of tiles
    [SerializeField] const float TILE_HEIGHT = 0.72f;
    [SerializeField] const float TILE_WIDTH = 0.72f;

    // Side lengths of grid, makes an irreregular hexagon shaped board
    [SerializeField] const int GRID_WIDTH = 11;
    [SerializeField] const int GRID_HEIGHT = 13;

    // Dictionaries for octagons and squares, keys are their positions on a diagonal grid with 0,0 being the top left (used i + j from spawnGrid() to create these coordinates)
    public Dictionary<Vector2, Octagon> octagons = new Dictionary<Vector2, Octagon>();
    public Dictionary<Vector2, Square>  squares  = new Dictionary<Vector2, Square>();

    // Start is called before the first frame update
    void Start()
    {

        // Instantiate octagons and squares
        // Board centered at global position 0,0,0
        for (int i = 0; i < GRID_HEIGHT; i++)
        {

            // Find width of this layer
            int width = GRID_WIDTH - (Mathf.Abs((GRID_HEIGHT / 2) - i));

            for (int j = 0; j < width; j++)
            {

                octagons.Add(new Vector2(i, j), Instantiate(octagonPrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT)), Quaternion.identity).gameObject.GetComponent<Octagon>());

                // Assign random sprite to tiles
                octagons[new Vector2(i, j)].gameObject.GetComponent<SpriteRenderer>().sprite = octagonSprites[Random.Range(0, octagonSprites.Length)];

                // Spawn squares in specific spots
                if (i != 0)
                {

                    Transform square = Instantiate(squarePrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i - 1) * TILE_HEIGHT) - 0.01f, ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - ((i - 1) * TILE_HEIGHT) - 0.01f), Quaternion.identity);

                    // Assign random sprite to square
                    square.GetComponent<SpriteRenderer>().sprite = squareSprites[Random.Range(0, squareSprites.Length)];

                }

            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
