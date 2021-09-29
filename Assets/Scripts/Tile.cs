using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // Dictionary for neighbors, keys are the direction in which the neighbor is
    public Dictionary<int, Tile> neighbors = new Dictionary<int, Tile>();

    // Type of tile
    public int type;

    // Main script
    public Main main;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getType()
    {
        return type;
    }

    public Dictionary<int, Tile> getNeighbors()
    {
        return neighbors;
    }

    public void setType(int type, Sprite sprite)
    {
        this.type = type;
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void addNeighbor(int direction, Tile tile)
    {
        neighbors.Add(direction, tile);
    }

    public void setMain(Main main)
    {
        this.main = main;
    }

}
