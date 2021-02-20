using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


//WorldController manages the World and it's Tile objects, both it's Model data and it's View GameObject data.
public class WorldMapController : MonoBehaviour
{
    //WorldController instance to be statically accessed by other classes.
    public static WorldMapController Instance;
    public GUIController GUIController;

    //World instance to manage through this class.
    public WorldMap World;

    public TileManagement TileManager;

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

        TileManager = new TileManagement(World);

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = AssetLoader.MaterialLibrary["test"];
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = World.WorldMesh;

        Dictionary<string, TileType> TempSpriteNames = new Dictionary<string, TileType>();

        TempSpriteNames.Add("Purple", TileType.Generic);
        TempSpriteNames.Add("Blue", TileType.Blue);
        TempSpriteNames.Add("Yellow", TileType.Yellow);
        TempSpriteNames.Add("Green", TileType.Green);
        TempSpriteNames.Add("Space", TileType.None);

        World.SetTiles();

        foreach(Tile t in World.Tiles)
        {
            TileManager.InitialTileState(t);
        }

        World.UpdateWorldMesh();

        GUIController.GetComponent<GUIController>().CreateTileTypeButtons(TempSpriteNames);

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