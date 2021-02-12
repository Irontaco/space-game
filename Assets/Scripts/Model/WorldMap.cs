using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

//World is the 2D array of Tile Objects that makes up the game world, and everything placed inside it.

public class WorldMap
{
    //The World is made of a 2D array of Tile objects.
    public Tile[,] Tiles;

    public Dictionary<Tile, int> TileVertIndex;

    //Array dimensions
    public int Width;
    public int Height;

    public Mesh WorldMesh;
    public Vector3[] MeshVerts;
    Vector2[] MeshUVs;

    //Defines the current world as an object to be accessed statically, for convenience.
    public static WorldMap Current;

    public WorldMap() { }

    public WorldMap(int width = 4, int height = 4)
    {

        //World dimensions, X and Y.
        Width = width;
        Height = height;

        Tiles = new Tile[Width,Height];
        TileVertIndex = new Dictionary<Tile, int>();

        BuildWorldMesh();

        //Sets the static world object to this one.
        Current = this;
    }

    /// <summary>
    /// Returns a mesh based on dimensions given.
    /// </summary>
    public Mesh BuildWorldMesh()
    {
        //instantiate new mesh
        WorldMesh = new Mesh();
        WorldMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //One quad per tile
        int tiles = Width * Height;

        //4 vertexes per quad, non-shared
        MeshVerts = new Vector3[tiles * 4];
        MeshUVs = new Vector2[MeshVerts.Length];

        //Iterates through each possible position in the vertex array and creates a Vector3 point in it.

        int iVertCount = 0;

        //NOTE: Code-wise we treat XZ as XY, for simplicity's sake. IN-CODE = X: West-East, Y: North-South, Z: Up-Down. IN UNITY = X: West-East, Y: Up-Down, Z: North-South.
        //This is important to keep in mind whenever we see vectors being manipulated...
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {

                //now entering baby hell
                //represents a quad, counter-clockwise.
                MeshVerts[iVertCount + 0] = new Vector3(x       , 0     , y );
                MeshVerts[iVertCount + 1] = new Vector3(x + 1   , 0     , y );
                MeshVerts[iVertCount + 2] = new Vector3(x       , 0 , y + 1 );
                MeshVerts[iVertCount + 3] = new Vector3(x + 1   , 0 , y + 1 );

                MeshUVs[iVertCount + 0] = new Vector2(0f   , 0f);
                MeshUVs[iVertCount + 1] = new Vector2(0f  , 0f);
                MeshUVs[iVertCount + 2] = new Vector2(0f, 0f);
                MeshUVs[iVertCount + 3] = new Vector2(0f, 0f);

                Tile tile = new Tile(this, Mathf.FloorToInt(MeshVerts[iVertCount + 0].x), Mathf.FloorToInt(MeshVerts[iVertCount + 0].z));

                Tiles[x, y] = tile;

                TileVertIndex.Add(tile, iVertCount);

                iVertCount += 4;
            }
        };

        int[] tris = new int[tiles * 6];
        int iIndexCount = iVertCount = 0;

        for (int i = 0; i < tiles; i++)
        {
            tris[iIndexCount + 0] += (iVertCount + 0);
            tris[iIndexCount + 1] += (iVertCount + 2);
            tris[iIndexCount + 2] += (iVertCount + 1);
            tris[iIndexCount + 3] += (iVertCount + 2);
            tris[iIndexCount + 4] += (iVertCount + 3);
            tris[iIndexCount + 5] += (iVertCount + 1);

            iVertCount += 4; iIndexCount += 6;
        }

        WorldMesh.vertices = MeshVerts;
        WorldMesh.triangles = tris;
        WorldMesh.uv = MeshUVs;
        WorldMesh.RecalculateNormals();

        return WorldMesh;

    }

    public void UpdateTileUVs(Tile t, List<Vector2> uvs)
    {
        int index = TileVertIndex[t];

        MeshUVs[index + 0] = uvs[0];
        MeshUVs[index + 1] = uvs[1];
        MeshUVs[index + 2] = uvs[2];
        MeshUVs[index + 3] = uvs[3];
    }

    public void UpdateWorldMesh()
    {
        WorldMesh.uv = MeshUVs;
    }

    public int GetVertIndexFromTile(Tile t)
    {
        int index = TileVertIndex[t];

        return index;
    }

    /// <summary>
    /// Iterates through the World array and sets the tiles' types.
    /// </summary>
    public void SetTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y].Type = TileType.None;
            }

        }
    }

    /// <summary>
    /// Returns a Tile depending on the X,Y coordinates provided.
    /// </summary>
    public Tile GetTileAt(int x, int y)
    {
        //Checks for the Tile being out of the World array dimensions, returns null if so.
        if (x >= Width || x < 0 || y >= Width || y < 0)
        {
            Debug.LogError("Tile at (" + x + "," + y + ") is out of range!");
            return null;
        }

        try
        {
            return Tiles[x, y];
        }

        catch (IndexOutOfRangeException)
        {
            Debug.LogError("Tile was somehow passed through, while being out of range. INFORMATION [" + "X = " + x + " Y = " + y + "]");
            return null;
        }

    }

}