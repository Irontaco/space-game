using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseThing : MonoBehaviour
{
    public Tile Tile { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool CanBuildUnder { get; set; }
    public GameObject GameObject { get; set; }

    public abstract void CreateThing(Tile tile, int width, int height, bool CanBuildUnder, GameObject gameObject);

    public abstract void DeleteThing();

}
