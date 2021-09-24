using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octagon : Tile
{

    // Dictionary for neighbors, keys are the direction in which the neighbor is
    private Dictionary<int, Octagon> neighborOctagons = new Dictionary<int, Octagon>();
    private Dictionary<int, Square>  neighborSquares  = new Dictionary<int, Square>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<int, Octagon> getNeighborOctagons()
    {
        return neighborOctagons;
    }

    public Dictionary<int, Square> getNeighborSquares()
    {
        return neighborSquares;
    }

    public void addNeighborOctagon(int direction, Octagon otherOctagon)
    {
        neighborOctagons.Add(direction, otherOctagon);
    }

    public void addNeighborSquare(int direction, Square square)
    {
        neighborSquares.Add(direction, square);
    }
}
