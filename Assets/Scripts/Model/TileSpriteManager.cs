using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maintains data consistency between Tile/GameObject entities.
public class TileSpriteManager
{

    Dictionary<string, Sprite> TileSprites;

    public Tile[,] Tiles;

    public TileSpriteManager(WorldMap World)
    {
        Tiles = World.Tiles;
        TileSprites = AssetLoader.TileLibrary;
    }

    public void ChangeTileType(Tile t, TileType type)
    {

        if (t == null)
        {
            Debug.Log("This tile doesn't exist.");
            return;
        }

        t.Type = type;



    }
}
