using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseThing
{

    //Defines the Tile this belongs to.
    Tile Tile { get; set; }

    GameObject GameObject { get; set; }

    //By default, Structures occupy 1 Tile. Bigger structures will occupy more Tiles.
    int Width { get; set; }
    int Height { get; set; }

    //Defines if this Structure can be built over/under.
    bool CanBuildUnder { get; set; }

    //Defines if this Structure will be given collision detection.
    bool CanWalkOver { get; set; }

    event Action<IBaseThing> OnChanged;


}
