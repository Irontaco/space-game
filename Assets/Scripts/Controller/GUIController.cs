using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{

    public GameObject CreateTileBtn;
    public GameObject DeleteTileBtn;

    public BuildController BuildController;

    //god wtf
    public Dictionary<string, GameObject> TileTypeButtons;
    public Dictionary<GameObject, TileType> ButtonTileTypes;


    void Start()
    {
        TileTypeButtons = new Dictionary<string, GameObject>();
        ButtonTileTypes = new Dictionary<GameObject, TileType>();

    }

    private GameObject InstantiateTypeButton(string ButtonName, TileType correspondingType)
    {
        GameObject TypeButtonGO = Instantiate(AssetLoader.PrefabLibrary["TileTypeSelectorButton"]);

        TypeButtonGO.transform.SetParent(GameObject.Find("TileTypeSelector").transform);

        //Assign the Button's text to the inputted WorldName.
        TypeButtonGO.GetComponentInChildren<Text>().text = ButtonName;

        GUIController GuiControllerInstance = FindObjectOfType<GUIController>();
        Button TypeButton = TypeButtonGO.GetComponent<Button>();

        //This whole section generates a persistent OnClick listener which will be applied to the WorldEntry prefab.
        TypeButton.onClick.AddListener(() => SelectTileType(TypeButtonGO));

        //var targetFunc = UnityEvent.GetValidMethodInfo(GuiControllerInstance, "SelectWorldEntry", new Type[] { typeof(GameObject) });
        //UnityAction<GameObject> action = Delegate.CreateDelegate(typeof(UnityAction<GameObject>), GuiControllerInstance, targetFunc, false) as UnityAction<GameObject>;
        //UnityEventTools.AddObjectPersistentListener<GameObject>(WorldEntryButton.onClick, action, WorldEntry);

        TileTypeButtons.Add(ButtonName, TypeButtonGO);
        ButtonTileTypes.Add(TypeButtonGO, correspondingType);

        return TypeButtonGO;
    }

    private void CreateTileTypeButtonList()
    {
        GameObject TileTypeSelector = GameObject.Find("TileTypeSelector");
        Vector3 TileTypeButtonSize = TileTypeButtons["Purple"].GetComponent<Image>().sprite.bounds.size;

        //Spaces occupied by the LoadButton. We need this to place WorldEntries under it.
        Vector3 TileTypeSelectorPosition = TileTypeSelector.transform.position;

        //Space that will be added in-between each WorldEntry.
        Vector3 TileTypeButtonInitialPosition = new Vector3(TileTypeSelectorPosition.x, TileTypeSelectorPosition.y + TileTypeButtonSize.y - 15, 0);

        int i = 0;

        //Loop through our existing WorldEntries
        foreach (KeyValuePair<string, GameObject> KVP in TileTypeButtons)
        {
            //Each new WorldEntry has to be offset by the size of the previous WorldEntry sprite, plus 5 for spacing.
            GameObject TileTypeSelectorButton = KVP.Value;
            Vector3 TileTypeSelectorButtonImageSize = TileTypeSelectorButton.GetComponent<Image>().sprite.bounds.size;

            TileTypeSelectorButton.transform.position = TileTypeButtonInitialPosition;

            Vector3 TileTypeSelectorOffsetBy = new Vector3(0, 16 * i, 0);

            TileTypeSelectorButton.transform.position -= new Vector3(0, TileTypeSelectorOffsetBy.y - (-3 * i), 0);

            i++;
        }
    }

    public void SelectTileType(GameObject typeButtonGO)
    {

        BuildController.CurrentTileType = ButtonTileTypes[typeButtonGO];

    }


    //barebones and nonadaptive
    public void CreateTileTypeButtons(Dictionary<string, TileType> sprites)
    {

        foreach(KeyValuePair<string,TileType> kvp in sprites)
        {
            InstantiateTypeButton(kvp.Key, kvp.Value);
        }

        CreateTileTypeButtonList();
    }

    public void CreateTile()
    {

        Debug.Log("we've been called!");
        BuildController.SetMode("Tile");
    }

    public void DeleteTile()
    {
        BuildController.SetMode("DeleteTile");
    }

}
