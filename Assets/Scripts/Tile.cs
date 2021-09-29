using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // Dictionary for neighbors, keys are the direction in which the neighbor is
    public Dictionary<int, Tile> neighbors = new Dictionary<int, Tile>();

    // Terrain of tile
    public int terrain;

    // Main script
    public Main main;

    // Owner of tile
    public int owner;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getOwner()
    {
        return owner;
    }

    public int getTerrain()
    {
        return terrain;
    }

    public Dictionary<int, Tile> getNeighbors()
    {
        return neighbors;
    }

    public virtual void addNeighbor(int direction, Tile tile)
    {
        neighbors.Add(direction, tile);
    }

    public virtual void setTerrain(int terrain)
    {
        this.terrain = terrain;
    }

    public virtual void setOwner(int newOwner)
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

    }

    public void setMain(Main main)
    {
        this.main = main;
    }

}
