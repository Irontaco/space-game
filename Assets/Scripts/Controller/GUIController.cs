using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject CreateTileBtn;
    public GameObject DeleteTileBtn;

    public BuildController BuildController;

    void Start()
    {
        


    }


    public void CreateTile()
    {
        BuildController.SetMode("Tile");

    }

    public void DeleteTile()
    {
        BuildController.SetMode("DeleteTile");
    }

}
