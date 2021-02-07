using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles object pre-pooling and instantiation.
/// </summary>

public class Pooler : MonoBehaviour
{

    public List<GameObject> PooledObjects;
    public GameObject currentObject;

    void Awake()
    {
        gameObject.transform.name = "ObjectPooler";
        PooledObjects = new List<GameObject>();
    }

    public void PreLoad(GameObject request, int ammount)
    {
        currentObject = Instantiate(request);

        for (int i = 0; i < ammount; i++)
        {
            GameObject requestedObject = (GameObject)GameObject.Instantiate(currentObject);
            requestedObject.transform.SetParent(gameObject.transform);
            requestedObject.transform.name = "PooledObj[" + request.name + "]_" + i;
            requestedObject.SetActive(false);
            PooledObjects.Add(requestedObject); 
        }

        currentObject.SetActive(false);
    }

    public GameObject Spawn()
    {
        GameObject obj;

        if(PooledObjects.Count == 0)
        {
            obj = (GameObject)GameObject.Instantiate(currentObject);
            obj.transform.SetParent(gameObject.transform);
            obj.name = "Pooled " + obj.name;

            PooledObjects.Add(obj);
            return obj;
        }
        else
        {
            for(int i=0; i < PooledObjects.Count; i++)
            {
                if (!PooledObjects[i].activeInHierarchy)
                {
                    PooledObjects[i].SetActive(true);
                    return PooledObjects[i];
                }
            }
        }

        obj = (GameObject)GameObject.Instantiate(currentObject);
        obj.transform.SetParent(gameObject.transform);
        PooledObjects.Add(obj);

        return obj;

    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
