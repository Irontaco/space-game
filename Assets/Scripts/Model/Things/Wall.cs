using UnityEngine;

public class Wall : BaseThing, ISegmentable
{

    public SpriteRenderer SpriteRenderer;

    public BoxCollider Collider;
    public Rigidbody Body;

    public void CreateSegment()
    {
        SpriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        Body = GameObject.AddComponent<Rigidbody>();
        Body.isKinematic = true;
        Body.useGravity = false;

        Collider = GameObject.AddComponent<BoxCollider>();

        GameObject.transform.name = "WallSegment[" + Tile.X + "," + Tile.Y + "]";
        GameObject.transform.position = new Vector3(Tile.X + .5f, 0.01f, Tile.Y + .5f);
        GameObject.transform.Rotate(new Vector3(90, 0, 0));
        SpriteRenderer.sortingLayerName = "Thing";
        SpriteRenderer.sprite = AssetLoader.ThingLibrary["wall_test_NONE"];


        Collider.size = SpriteRenderer.sprite.bounds.size; 

        UpdateNeighbors();
    }

    public void DeleteSegment()
    {

        Destroy(GameObject);

        Tile.Thing = null;

        UpdateNeighbors();

        Tile = null;
    }


    public void UpdateNeighbors()
    {

        int positionX = Tile.X;
        int positionY = Tile.Y;

        for(int neighborX = positionX - 1; neighborX < positionX + Width + 1; neighborX++)
        {
            for (int neighborY = positionY - 1; neighborY < positionY + Height + 1; neighborY++)
            {
                Tile neighborTile = WorldMap.Current.GetTileAt(neighborX, neighborY);

                if (neighborTile != null && neighborTile.Thing != null)
                {
                    neighborTile.Thing.GetComponent<Wall>().UpdateSelf();
                }
            }
        }

    }

    public void UpdateSelf()
    {
        SpriteRenderer.sprite = GenerateSegmentSprite();

    }

    /// <summary>
    /// Returns a Sprite from the wallSpriteDict depending on the Structure's neighbors. If it has no neighbors, then it's set to the default sprite.
    /// </summary>
    public Sprite GenerateSegmentSprite()
    {
        //SPRITE NAME FORMAT
        //[STRUCTURESPRITE] + [N]?[E]?[S]?[W]

        string directions = "_";
        Tile t = Tile;
        int x = t.X;
        int y = t.Y;

        //An extra letter is added to the directions string for each getNeighborDirection call, first, we check cardinals.
        //TODO: Check corners and return a proper sprite.
        //TODO: Checking through strings is inherently slower, lower priority!
        directions += GetNeighborDirection(x, y + 1, "N");
        directions += GetNeighborDirection(x + 1, y, "E");
        directions += GetNeighborDirection(x, y - 1, "S");
        directions += GetNeighborDirection(x - 1, y, "W");

        if (directions == "_")
        {
            directions = "_NONE";
        }

        return AssetLoader.ThingLibrary["wall_test" + directions];
    }

    /// <summary>
    /// Returns either a proper String or an empty one depending on if it's neighboring tiles are suitable for neighbor-linking.
    /// </summary>
    public string GetNeighborDirection(int x, int y, string direction)
    {

        Tile t = WorldMap.Current.GetTileAt(x, y);
        if (t != null && t.Thing != null)
        {
            return direction;
        }

        return string.Empty;
    }


    public override void CreateThing(Tile tile, int width, int height, bool CanBuildUnder, GameObject gameObject)
    {
        Tile = tile;
        Tile.PlaceThing(this);
        Width = 1;
        Height = 1;
        GameObject = gameObject;

        CreateSegment();

    }

    public override void DeleteThing()
    {
        DeleteSegment();
    }
}
