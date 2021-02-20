using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Wall : MonoBehaviour, IBaseThing, ISegmentable
{
    public Tile Tile { get; set; }
    public GameObject GameObject { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool CanBuildUnder { get; set; }
    public bool CanWalkOver { get; set; }

    public SpriteRenderer CurrentSprite;

    public BoxCollider2D Collider;

    public event Action<IBaseThing> OnChanged;

    public bool UpdateNeighbors(ISegmentable segment)
    {
        throw new System.NotImplementedException();
    }

    void Awake()
    {
        CurrentSprite = gameObject.AddComponent<SpriteRenderer>();
        Collider = gameObject.AddComponent<BoxCollider2D>();
        
    }

    public bool GetNeighbors()
    {
        throw new NotImplementedException();
    }

    public bool SetSprites()
    {
        throw new NotImplementedException();
    }
}
