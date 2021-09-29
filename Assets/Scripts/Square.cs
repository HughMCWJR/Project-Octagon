using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Tile
{

    [SerializeField] private Sprite[] sprites;

    public override void setTerrain(int terrain)
    {
        this.terrain = terrain;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[terrain];
    }

    // Checks neighbors to see which team now owns this tile and updates it as necessary
    // Also updates teams
    public void checkOwnership()
    {

        // Go through each neighbor and count how many neighbors each team owns
        Dictionary<int, int> ownedPerTeam = new Dictionary<int, int>();

        foreach (KeyValuePair<int, Tile> tile in neighbors)
        {

            int team = tile.Value.getOwner();

            if (ownedPerTeam.ContainsKey(team)) {

                ownedPerTeam[team]++;

            } else
            {

                ownedPerTeam.Add(team, 1);

            }

        }

        // Now it goes through each team and sees which team owns the most
        int maxOwned = -1;
        int indexOfMax = Main.NO_ONE;

        foreach (KeyValuePair<int, int> team in ownedPerTeam)
        {

            if (team.Value > maxOwned)
            {

                maxOwned   = team.Value;
                indexOfMax = team.Key;

            } else if (team.Value == maxOwned)
            {

                maxOwned   = -1;
                indexOfMax = Main.NO_ONE;

            }

        }

        // Applies which team owns it
        setOwner(indexOfMax);

    }

}
