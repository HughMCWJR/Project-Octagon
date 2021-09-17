using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // Prefabs
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform squarePrefab;

    // Sprites
    [SerializeField] private Sprite[] tileSprites;
    [SerializeField] private Sprite[] squareSprites;

    // Height and width of individual tiles
    const float TILE_HEIGHT = 0.72f;
    const float TILE_WIDTH = 0.72f;

    // Side lengths of grid, makes a hexagon shaped board
    const int GRID_WIDTH = 11;
    const int GRID_HEIGHT = 13;

    // Start is called before the first frame update
    void Start()
    {

        // Instantiate tiles, octagons and squares
        // Board centered at 0,0
        for (int i = 0; i < GRID_HEIGHT; i++)
        {

            // Find width of this layer
            int width = GRID_WIDTH - (Mathf.Abs((GRID_HEIGHT / 2) - i));

            for (int j = 0; j < width; j++)
            {

                Transform tile = Instantiate(tilePrefab, new Vector3((2 * j * TILE_WIDTH) - (TILE_WIDTH * (width - 1)), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT), ((GRID_HEIGHT - 1) * TILE_HEIGHT / 2) - (i * TILE_HEIGHT)), Quaternion.identity);

                // Assign random sprite to tiles
                tile.GetComponent<SpriteRenderer>().sprite = tileSprites[Random.Range(0, tileSprites.Length)];

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
