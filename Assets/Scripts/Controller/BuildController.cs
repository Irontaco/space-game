using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildController : MonoBehaviour
{

    //Determines how the BuilderController will behave.
    private enum BuildMode
    {
        TILE,
        STRUCTURE,
        DELETETILE,
        DELETESTRUCTURE,
        INTERACT
    }

    public string StructureType;
    public string StructureSprite;

    private BuildMode buildMode;

    public List<Vector3> VectorSelection = new List<Vector3>();
    public List<Vector3> LastVectorSelection = new List<Vector3>();

    public Sprite PreviewSprite;
    public List<GameObject> PreviewTileSelection;
    public Pooler PreviewPooler;

    public Vector3 SelectionStartPosition;
    public Vector3 SelectionCurrentPosition;
    private Vector3 SelectionSingle;

    private int LastDragx;
    private int LastDragy;

    /// <summary>
    /// Sets the vectors to be modified, updated per frame.
    /// </summary>
    /// <param name="currPos"></param>
    /// NOTE: Z is treated as Y within code.
    public void SetSelectionVectors(Vector3 currPos)
    {
        
        if(WorldMapController.Instance.GetTileAtWorldVector(currPos) == null)
        {
            return;
        }

        SelectionCurrentPosition = currPos;

        //Save the starting drag position, and the end drag position.
        int startDrag_x = Mathf.FloorToInt(SelectionStartPosition.x);
        int endDrag_x = Mathf.FloorToInt(SelectionCurrentPosition.x);
        int startDrag_y = Mathf.FloorToInt(SelectionStartPosition.z);
        int endDrag_y = Mathf.FloorToInt(SelectionCurrentPosition.z);

        if (endDrag_x < startDrag_x)
        {
            int tmp = endDrag_x;
            endDrag_x = startDrag_x;
            startDrag_x = tmp;
        }

        if (endDrag_y < startDrag_y)
        {
            int tmp = endDrag_y;
            endDrag_y = startDrag_y;
            startDrag_y = tmp;
        }

        DestroyPreview();

        VectorSelection.Clear();
        PreviewTileSelection.Clear();

        for (int x = startDrag_x; x <= endDrag_x; x++)
        {
            for (int y = startDrag_y; y <= endDrag_y; y++)
            {
                Vector3 vec = new Vector3(x, 0, y);

                if(WorldMapController.Instance.GetTileAtWorldVector(vec) == null)
                {
                    Debug.Log("Tile at (" + x + "," + y + ") was out of range. It won't be added to the selection.");
                    return;
                }
                if (!VectorSelection.Contains(vec))
                {
                    VectorSelection.Add(vec);
                }
            }
        }

        LastDragx = endDrag_x;
        LastDragy = endDrag_y;
        LastVectorSelection = VectorSelection;

        CreatePreview();
    }

    void Start()
    {

        GameObject PreviewPoolerObject = new GameObject();
        PreviewPooler = PreviewPoolerObject.AddComponent<Pooler>();

        GameObject preview = new GameObject();

        preview.name = "PreviewTile";
        preview.transform.Rotate(new Vector3(90, 0, 0));
        preview.AddComponent<SpriteRenderer>();
        preview.GetComponent<SpriteRenderer>().sprite = PreviewSprite;
        preview.GetComponent<SpriteRenderer>().sortingLayerName = "Structure";

        PreviewPooler.PreLoad(preview, 200);
        Destroy(preview);


    }

    /// <summary>
    /// Creates tile preview GameObjects from a vector list.
    /// </summary>
    private void CreatePreview()
    {

        foreach (Vector3 vec in VectorSelection)
        {
            GameObject preview = PreviewPooler.Spawn();
            preview.name = "PreviewTile[" + vec.x + "," + vec.z + "]";
            preview.transform.position = vec;

            PreviewTileSelection.Add(preview);
        }

    }

    /// <summary>
    /// Destroys the tile preview GameObjects.
    /// </summary>
    public void DestroyPreview()
    {

        if (PreviewTileSelection == null)
        {
            return;
        }

        if (PreviewTileSelection.Count == 0)
        {
            return;
        }

        foreach (GameObject go in PreviewTileSelection)
        {
            PreviewPooler.Despawn(go);
        }

        PreviewTileSelection.Clear();
    }


    /// <summary>
    /// Figures out what to do based on the buildMode.
    /// </summary>
    public void ResolveContext()
    {
        DestroyPreview();

        Tile tileSingle = WorldMapController.Instance.GetTileAtWorldVector(SelectionSingle);
        List<Tile> TileSelection = new List<Tile>();

        foreach (Vector3 v in VectorSelection)
        {
            TileSelection.Add(WorldMapController.Instance.GetTileAtWorldVector(v));
        }

        if (TileSelection == null || tileSingle == null)
        {
            Debug.LogError("The tile selection given was null, likely to be caused by the vectors given previously.");
            return;
        }

        switch (buildMode)
        {
            case BuildMode.DELETETILE:
                DoDeleteTile(TileSelection);
                break;
            case BuildMode.TILE:
                DoBuildTile(TileSelection);
                break;
        }

    }

    private void DoBuildTile(List<Tile> tileSelection)
    {
        foreach (Tile t in tileSelection)
        {
            WorldMapController.Instance.TileManager.ChangeTileType(t, TileType.Generic);
        }

    }
    private void DoDeleteTile(List<Tile> tileSelection)
    {
        foreach (Tile t in tileSelection)
        {
            WorldMapController.Instance.TileManager.ChangeTileType(t, TileType.None);
        }
    }

    public void SetMode(string mode)
    {

        switch (mode)
        {
            case "Tile":
                buildMode = BuildMode.TILE;
                break;
            case "DeleteTile":
                buildMode = BuildMode.DELETETILE;
                break;
        }
    }

}
