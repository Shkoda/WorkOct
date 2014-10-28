using UnityEngine;
using System.Collections;
using System;
using Random = System.Random;


public class MapBuilder : MonoBehaviour {

    public GameObject Tile;
    public int TopLeftX, TopLeftY, FieldWidth, FieldHeight, WallWidth;

    private GameObject Map;



	// Use this for initialization
	void Start () {
        Map = new GameObject("Map");
        //vertical
        for (int i = -1-WallWidth; i <= FieldHeight+WallWidth; i++)
        {
            for (int j = 0; j < WallWidth; j++)
            {
                AddTile(-j, i);
                AddTile(FieldWidth+j, i);
            }
           
        }

        //horizontal
        for (int i = -1; i < FieldWidth; i++)
        {
            for (int j = 1; j < WallWidth; j++)
            {
                AddTile(i, -j);
                AddTile(i, FieldHeight+j);
            }
          
        }
	    Random rnd = new Random();
    
       int randTileNumber = rnd.Next(40, 40);
	    for (int i = 0; i < randTileNumber; i++)
	    {
            AddTile(rnd.Next(FieldWidth/4, FieldWidth), rnd.Next(FieldHeight));
	    }
	}

    private void AddTile(float  x, float y)
    {
        var tile = (GameObject)Instantiate(Tile, new Vector3(x, y), Quaternion.identity);
        tile.transform.parent = Map.transform;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
