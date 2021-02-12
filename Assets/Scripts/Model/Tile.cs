using System;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

//Tiles can be of types defined in the enum.
public enum TileType { Generic, Blue, Yellow, Green, None };

//Tiles are the main component of the game world's interactability. Everything is managed by Tile object locations, and everything will be placed on top of Tiles.
public class Tile
{
    //By default, tiles are of "Empty" type.
    private TileType _type = TileType.Generic;

    //World instance this tile belongs to.
    public WorldMap World;

    //The X and Y positions of this tile in the World array.
    public int X { get; protected set; }
    public int Y { get; protected set; }

    //Callback called whenever this Tile changes, this triggers the tile to call for changes automatically.
    public static event Action<Tile> OnChanged;

    public TileType Type
    {

        get { return _type; }
        set
        {
            //Keep track of the old type before we replace it.
            TileType oldtype = _type;
            _type = value;

            //if the tile exists, and it's not the previous tile type, then we update the tile.
            if (OnChanged != null && oldtype != _type)
            {
                OnChanged(this);
            }
        }
    }

    //Instantiator for Tiles.
    public Tile(WorldMap world, int x, int y)
    {
        this.World = world;
        this.X = x;
        this.Y = y;

    }


}

