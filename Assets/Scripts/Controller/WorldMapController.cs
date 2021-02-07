using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//WorldController manages the World and it's Tile objects, both it's Model data and it's View GameObject data.
public class WorldMapController : MonoBehaviour
{
    //WorldController instance to be statically accessed by other classes.
    public static WorldMapController Instance;

    //World instance to manage through this class.
    public WorldMap World;

    public TileSpriteManager TileManager;

    //Testing parameters for saving/loading
    public int WorldSizeX = 4;
    public int WorldSizeY = 4;

    public string WorldName;

    void Start()
    {

        if (Instance != null)
        {
            Debug.LogError("There shouldn't be two WorldControllers!!");
        }

        Instance = this;    //Set our instance to the current one.
        World = new WorldMap(WorldSizeX, WorldSizeY); //Set a world to use.

        TileManager = new TileSpriteManager(World);

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = AssetLoader.MaterialLibrary["test"];
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = World.WorldMesh;

        TileAtlasResolver AtlasResolver = new TileAtlasResolver();

        Dictionary<int, List<Vector2>> TileSet = AtlasResolver.GenerateUvAtlas(4, AssetLoader.TileLibrary["test"]);

        StartCoroutine(RandomizeMapTest(TileSet));

        

    }

    public IEnumerator RandomizeMapTest(Dictionary<int, List<Vector2>> TileSet)
    {

        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(.001f);
        for (int i = 0; i < 10000; i++)
        {
            for (int x = 0; x < World.Width; x++)
            {
                for (int y = 0; y < World.Height; y++)
                {
                    World.UpdateTileUVs(World.Tiles[x, y], TileSet[Random.Range(1, 16)]);
                }
            }

            World.UpdateWorldMesh();

            yield return wait;
        }
    }

    /*
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (World.verts == null)
        {
            return;
        }
        Gizmos.color = new Color32(0, 255, 0, 100);

        foreach (Vector3 vec in World.verts)
        {
            Gizmos.DrawSphere(vec, 0.05f);

        }
    }
    */

    /// <summary>
    /// Instantiates all tile objects, then rolls for Landmark creation.
    /// </summary>
    void InstantiateTileObjects()
    {

        World.SetTiles();

        int i = 0;

        //Iterates through the requested World's Tile data objects, creating a GameObject that visually represents the Tile.
        // 100x100 2D array of Tile, which adds up to about 10000 Tile GameObjects.
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile tile = World.GetTileAt(x, y);

                TileManager.CreateTile(tile);
                i++;
            }
        }

        Debug.Log("Created " + i + " tiles. World creation successful!");

    }

    /// <summary>
    /// Returns a Tile at the specified Vector coordinates.
    /// </summary>
    public Tile GetTileAtWorldVector(Vector3 vec)
    {
        int x = Mathf.FloorToInt(vec.x);
        int y = Mathf.FloorToInt(vec.z);

        return WorldMapController.Instance.World.GetTileAt(x, y);
    }

}