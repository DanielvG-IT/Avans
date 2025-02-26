using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectSystem : MonoBehaviour
{
    [Header("Kkas")]
    public TextMeshPro m_Tex;
    public Object2D object2D;
    public GameObject circlePrefab;
    public GameObject squarePrefab;
    public GameObject trianglePrefab;


    // Internal
    private Object2DApiClient object2DApiClient;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        object2DApiClient = ApiClientManager.Instance.Object2DApiClient;


    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Object2D/Read all")]
    public async void ReadObject2Ds()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(object2D.environmentId);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                List<Object2D> object2Ds = dataResponse.Data;
                Debug.Log("List of object2Ds: " + object2Ds);
                object2Ds.ForEach(object2D => Debug.Log(object2D.id));
                GameObject prefabToInstantiate;
                // TODO: Succes scenario. Show the enviroments in the UI

                foreach (var item in object2Ds)
                {
                    prefabToInstantiate = null;

                    switch (item.prefabId)
                    {
                        case "Circle":
                            prefabToInstantiate = circlePrefab;
                            break;
                        case "Square":
                            prefabToInstantiate = squarePrefab;
                            break;
                        case "Triangle":
                            prefabToInstantiate = trianglePrefab;
                            break;
                        default:
                            Debug.LogWarning($"Unknown prefab ID: {item.prefabId}");
                            continue;
                    }

                    if (prefabToInstantiate != null)
                    {
                        // Convert item properties (Position, Scale and Rotation) to Unity types
                        Vector2 position = new(item.positionX, item.positionY);
                        Quaternion rotation = Quaternion.Euler(0f, 0f, item.rotationZ);
                        Vector2 scale = new(item.scaleX, item.scaleY);

                        // Spawn Object and assert scale
                        GameObject spawnedObject = Instantiate(prefabToInstantiate, position, rotation);
                        spawnedObject.transform.localScale = scale;

                        // Get SpriteRenderer and assert SortingLayer
                        spawnedObject.GetComponent<SpriteRenderer>().sortingOrder = item.sortingLayer;
                    }

                    // TODO Remove existing objects

                }
                break;

            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read object2Ds error: " + errorMessage);
                // TODO: Error scenario. Show the errormessage to the user.
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    [ContextMenu("Object2D/Create")]
    public async void CreateObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.CreateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2D> dataResponse:
                object2D.id = dataResponse.Data.id;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Create Object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    [ContextMenu("Object2D/Update")]
    public async void UpdateObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.UpdateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Update object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    [ContextMenu("Object2D/Delete")]
    public async void DeleteObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.DeleteObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Update object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

}
