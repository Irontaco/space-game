using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BuildController : MonoBehaviour
{

    //Determines how the BuilderController will behave.
    private enum BuildMode
    {
        TILE,
        THING,
        DELETETILE,
        DELETETHING
    }

    private BuildMode buildMode;

    public WorldMapController WorldController;
    private TileAtlasResolver AtlasResolver;

    public TileType CurrentTileType = TileType.Generic;

    private readonly HashSet<Vector3> VectorSelectionSet = new HashSet<Vector3>();

    private Mesh PreviewMesh;
    private Vector3[] MeshVerts;
    private Vector2[] PreviewMeshUVs;

    private GameObject PreviewMeshGO;

    private Dictionary<int, List<Vector2>> PreviewTileSprite;

    public Vector3 SelectionStartPosition;
    public Vector3 SelectionCurrentPosition;

    private float timeSinceLastCall;

    void Start()
    {

        AtlasResolver = new TileAtlasResolver();

        PreviewTileSprite = new Dictionary<int, List<Vector2>>
        {
            [0] = AtlasResolver.GenerateUvAtlas(8, AssetLoader.TileLibrary["PreviewAtlas"])[0]
        };

        //instantiate new mesh
        PreviewMesh = new Mesh();

        PreviewMeshGO = new GameObject();

        MeshRenderer meshRenderer = PreviewMeshGO.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = AssetLoader.MaterialLibrary["PreviewAtlas"];
        MeshFilter meshFilter = PreviewMeshGO.AddComponent<MeshFilter>();

        meshFilter.mesh = PreviewMesh;

    }

    void Update()
    {
        timeSinceLastCall += Time.deltaTime;
    }

    public bool ReadyForCall()
    {
        if (timeSinceLastCall > 0.04f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the vectors to be modified, updated per frame.
    /// </summary>
    /// <param name="currPos"></param>
    /// NOTE: Z is treated as Y within code.
    public void SetSelectionVectors(Vector3 currPos)
    {
        if(VectorSelectionSet.Count > (128 * 128))
        {
            if(timeSinceLastCall < 0.10f) { return; }
        }

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

        VectorSelectionSet.Clear();
        PreviewMesh.Clear();

        Vector3 vectorTest = new Vector3(0, 0, 0);

        for (int x = startDrag_x; x <= endDrag_x; x++)
        {
            for (int y = startDrag_y; y <= endDrag_y; y++)
            {
                vectorTest.Set(x, 0, y);

                if (VectorSelectionSet.Contains(vectorTest) || WorldMapController.Instance.GetTileAtWorldVector(vectorTest) == null)
                {
                    continue;
                }
                else
                {
                    VectorSelectionSet.Add(vectorTest);
                    continue;
                }
            }
        }

        PreviewMesh = BuildPreviewMesh();

        timeSinceLastCall = 0f;
    }

    /// <summary>
    /// Figures out what to do based on the buildMode.
    /// </summary>
    public void ResolveContext()
    {
        PreviewMesh.Clear();

        List<Tile> TileSelection = new List<Tile>();

        foreach (Vector3 v in VectorSelectionSet)
        {
            TileSelection.Add(WorldMapController.Instance.GetTileAtWorldVector(v));
        }

        VectorSelectionSet.Clear();

        if (TileSelection == null)
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
            case BuildMode.THING:
                DoBuildThing(TileSelection);
                break;
            case BuildMode.DELETETHING:
                DoDeleteThing(TileSelection);
                break;
        }
    }

    private void DoDeleteThing(List<Tile> tileSelection)
    {
        foreach(Tile t in tileSelection)
        {
            if(t.Thing != null)
            {
                t.Thing.DeleteThing();

            }
        }
    }

    private void DoBuildThing(List<Tile> tileSelection)
    {
        foreach (Tile t in tileSelection)
        {
            if(t.Type != TileType.None && t.Thing == null)
            {
                GameObject g = new GameObject();
                Wall w = g.AddComponent<Wall>();

                w.CreateThing(t, 1, 1, true, g);
            }
        }
    }

    private void DoBuildTile(List<Tile> tileSelection)
    {

        foreach (Tile t in tileSelection)
        {
            WorldController.TileManager.ChangeTileType(t, CurrentTileType);
        }

        WorldController.World.UpdateWorldMesh();
    }

    private void DoDeleteTile(List<Tile> tileSelection)
    {
        foreach (Tile t in tileSelection)
        {
            WorldController.TileManager.ChangeTileType(t, TileType.None);
        }

        WorldController.World.UpdateWorldMesh();

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
            case "Thing":
                buildMode = BuildMode.THING;
                break;
            case "DeleteThing":
                buildMode = BuildMode.DELETETHING;
                break;
        }
    }
    private Mesh BuildPreviewMesh()
    {
        if (VectorSelectionSet.Count > (128 * 128))
        {
            PreviewMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        //One quad per tile
        int tiles = VectorSelectionSet.Count;

        //4 vertexes per quad, non-shared
        MeshVerts = new Vector3[tiles * 4];
        PreviewMeshUVs = new Vector2[MeshVerts.Length];

        int[] tris = new int[tiles * 6];
        int iIndexCount = 0;
        int iVertCount = 0;

        foreach (Vector3 vec in VectorSelectionSet)
        {
            //now entering baby hell
            //represents a quad, counter-clockwise.
            MeshVerts[iVertCount + 0] = new Vector3(vec.x, 0, vec.z);
            MeshVerts[iVertCount + 1] = new Vector3(vec.x + 1, 0, vec.z);
            MeshVerts[iVertCount + 2] = new Vector3(vec.x, 0, vec.z + 1);
            MeshVerts[iVertCount + 3] = new Vector3(vec.x + 1, 0, vec.z + 1);

            PreviewMeshUVs[iVertCount + 0] = PreviewTileSprite[0][0];
            PreviewMeshUVs[iVertCount + 1] = PreviewTileSprite[0][1];
            PreviewMeshUVs[iVertCount + 2] = PreviewTileSprite[0][2];
            PreviewMeshUVs[iVertCount + 3] = PreviewTileSprite[0][3];

            tris[iIndexCount + 0] += (iVertCount + 0);
            tris[iIndexCount + 1] += (iVertCount + 2);
            tris[iIndexCount + 2] += (iVertCount + 1);
            tris[iIndexCount + 3] += (iVertCount + 2);
            tris[iIndexCount + 4] += (iVertCount + 3);
            tris[iIndexCount + 5] += (iVertCount + 1);

            iVertCount += 4; iIndexCount += 6;
        }

        PreviewMesh.vertices = MeshVerts;
        PreviewMesh.triangles = tris;
        PreviewMesh.uv = PreviewMeshUVs;

        return PreviewMesh;
    }

}
