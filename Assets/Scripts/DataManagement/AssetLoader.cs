using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetLoader loads all the prefabs, sprites, sounds and objects from Resources for use elsewhere.
/// </summary>
public class AssetLoader : MonoBehaviour
{

    //Stores all prefabs, sprites.
    public static Dictionary<string, GameObject> PrefabLibrary;
    public static Dictionary<string, Sprite> TileLibrary;
    public static Dictionary<string, Sprite> ThingLibrary;
    public static Dictionary<string, Material> MaterialLibrary;

    void Awake()
    {

        //Array for loading all the available Prefabs in our Resources.
        GameObject[] Prefabs = Resources.LoadAll<GameObject>("Prefabs");

        //Instantiate our Prefab Dictionary, then populate it with our Prefabs Array.
        PrefabLibrary = new Dictionary<string, GameObject>();

        foreach (GameObject g in Prefabs)
        {
            PrefabLibrary.Add(g.name, g);
        }

        //Creates a temporary sprite array where it loads every Wall resource from the directory.
        Sprite[] TileSprites = Resources.LoadAll<Sprite>("Images/Tile");

        //Initialize our Sprite dictionary.
        TileLibrary = new Dictionary<string, Sprite>();

        //We iterate through the Sprite array, adding them to the Dictionary while modifying it's key name for convenience.
        TileLibrary = new Dictionary<string, Sprite>();
        foreach (Sprite s in TileSprites)
        {
            TileLibrary.Add(s.name, s);
        }

        Material[] Materials = Resources.LoadAll<Material>("Materials");

        MaterialLibrary = new Dictionary<string, Material>();

        foreach(Material m in Materials)
        {
            MaterialLibrary.Add(m.name, m);
        }

        Sprite[] Things = Resources.LoadAll<Sprite>("Images/Things");

        ThingLibrary = new Dictionary<string, Sprite>();

        foreach(Sprite s in Things)
        {
            ThingLibrary.Add(s.name, s);
        }

    }


}
