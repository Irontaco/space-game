using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagement
{

    //UV modification
    TileAtlasResolver TileResolver;
    public Dictionary<int, List<Vector2>> TileAtlas;

    //Tiles we're working with
    public Tile[,] Tiles;

    public WorldMap World;

    public TileManagement(WorldMap world)
    {
        Tiles = world.Tiles;
        World = world;

        TileResolver = new TileAtlasResolver();

        TileAtlas = TileResolver.GenerateUvAtlas(4, AssetLoader.TileLibrary["test"]);

    }

    public void InitialTileState(Tile tile)
    {
        World.UpdateTileUVs(tile, TileAtlas[4]);
    }

    private void OnTileUpdate(Tile tile)
    {
        TileType type = tile.Type;

        switch (type)
        {
            case TileType.Generic:
                World.UpdateTileUVs(tile, TileAtlas[0]);
                break;
            case TileType.Blue:
                World.UpdateTileUVs(tile, TileAtlas[1]);
                break;
            case TileType.Yellow:
                World.UpdateTileUVs(tile, TileAtlas[2]);
                break;
            case TileType.Green:
                World.UpdateTileUVs(tile, TileAtlas[3]);
                break;
            case TileType.None:
                World.UpdateTileUVs(tile, TileAtlas[4]);
                break;
            default:
                Debug.LogError("We were provided an incorrect TileType!");
                break;
        }
    }

    public void ChangeTileType(Tile t, TileType type)
    {
        if(t == null)
        {
            Debug.LogError("Tile given was null!");
            return;
        }

        t.Type = type;

        OnTileUpdate(t);

    }
}
