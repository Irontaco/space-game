using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

/// <summary>
/// Manages UV coords for Tile sprites...
/// </summary>
public class TileAtlasResolver
{
    //Temporary implementation
    //We will assume we will have 16 possible tile sprites for the time being...

    //Sprites present in the atlas
    private int SpriteNum = 16;
    private int SpritePerSide = 4;
    private float uvStep;

    private Sprite TileAtlas;

    public Dictionary<int, List<Vector2>> SpriteUvCoords;

    public TileAtlasResolver()
    {
        SpriteUvCoords = new Dictionary<int, List<Vector2>>();
    }

    public Dictionary<int, List<Vector2>> GenerateUvAtlas(int SprPerSide, Sprite SprAtlas )
    {

        SpritePerSide = SprPerSide;
        TileAtlas = SprAtlas;
        uvStep = 1f / SpritePerSide;

        int currentSprite = 0;

        //"go through the grid" of all needed UV coords...
        for (float y = 0f; y < SpritePerSide; y++)
        {
            for (float x = 0f; x < SpritePerSide; x++)
            {

                List<Vector2> currentUVList = new List<Vector2>();

                float currentStepX = x / SpritePerSide;
                float currentStepY = y / SpritePerSide;

                Vector2 uv_bottomleft = new Vector2(currentStepX, currentStepY);
                Vector2 uv_bottomright = new Vector2(currentStepX + uvStep, currentStepY);
                Vector2 uv_topleft = new Vector2(currentStepX, currentStepY + uvStep);
                Vector2 uv_topright = new Vector2(currentStepX + uvStep, currentStepY + uvStep);

                currentUVList.Add(uv_bottomleft);
                currentUVList.Add(uv_bottomright);
                currentUVList.Add(uv_topleft);
                currentUVList.Add(uv_topright);

                SpriteUvCoords.Add(currentSprite, currentUVList);

                currentSprite++;
            }
        }

        return SpriteUvCoords;
    }



}
