using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    // Prefabs
    public Transform tilePrefab;

    // Height and width of individual tiles
    const int TILE_HEIGHT = 5;
    const int TILE_WIDTH = 10;

    // Side length of grid, assumes square
    const int GRID_SIDE_LENGTH = 11;

    // Start is called before the first frame update
    void Start()
    {

        // Instantiate tiles
        for (int i = 0; i < GRID_SIDE_LENGTH; i++)
        {

            for (int j = 0; j < GRID_SIDE_LENGTH; j++)
            {

                // Puts center tile at 0, 0
                Instantiate(tilePrefab, new Vector2((-1 * i * TILE_WIDTH) + (j * TILE_WIDTH), (TILE_HEIGHT * (GRID_SIDE_LENGTH - 1) / 2) + (-1 * i * TILE_HEIGHT) + (-1 * j * TILE_HEIGHT)), Quaternion.identity);

            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
