using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class EnvironmentSystem : MonoBehaviour
{
    //[Header("Environment Links")]
    //public TextMeshPro nameText;
    //public GameObject worldMap;

    //[Header("UI Links")]
    //List<GameObject> gameObjects;
    //public Object2D object2D;
    //public GameObject circlePrefab;
    //public GameObject squarePrefab;
    //public GameObject trianglePrefab;

    // Internal
    private Environment2D environment2D;
    private Object2DApiClient object2DApiClient;
    private Environment2DApiClient enviroment2DApiClient;
    public GameObject SelectedObject { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enviroment2DApiClient = ApiClientManager.Instance.Environment2DApiClient; 
        object2DApiClient = ApiClientManager.Instance.Object2DApiClient;
        environment2D = GameManager.Instance.SelectedEnvironment;

        LoadEnvironment2D();
    }

    public void Quit()
    {
        Application.Quit();
    }


    #region Environment2D

    private void LoadEnvironment2D() 
    { 
        //nameText.text = environment2D.name;
        //worldMap.transform.height = environment2D.maxHeight;
        //worldMap.transform.lenght = environment2D.maxLength;

        //ReadObject2Ds();
    }

    //private void SaveEnvironment2D() 
    //{ 
    //    // TODO Implement saving objects
    //}

    public async void UpdateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.UpdateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete environment error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async void DeleteEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.DeleteEnvironment(environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete environment error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    #endregion

    #region Object2D

    public async void ReadObject2Ds()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                List<Object2D> object2Ds = dataResponse.Data;
                Debug.Log("List of object2Ds: " + object2Ds);
                object2Ds.ForEach(object2D => Debug.Log(object2D.id));
                //GameObject prefabToInstantiate;
                foreach (var item in object2Ds)
                {
                    //prefabToInstantiate = null;

                    switch (item.prefabId)
                    {
                        //case "Circle":
                        //    prefabToInstantiate = circlePrefab;
                        //    break;
                        //case "Square":
                        //    prefabToInstantiate = squarePrefab;
                        //    break;
                        //case "Triangle":
                        //    prefabToInstantiate = trianglePrefab;
                        //    break;
                        default:
                            Debug.LogWarning($"Unknown prefab ID: {item.prefabId}");
                            continue;
                    }

                    //if (prefabToInstantiate != null)
                    //{
                    //    // Convert item properties (Position, Scale and Rotation) to Unity types
                    //    Vector2 position = new(item.positionX, item.positionY);
                    //    Quaternion rotation = Quaternion.Euler(0f, 0f, item.rotationZ);
                    //    Vector2 scale = new(item.scaleX, item.scaleY);

                    //    // Spawn Object and assert scale
                    //    GameObject spawnedObject = Instantiate(prefabToInstantiate, position, rotation);
                    //    spawnedObject.transform.localScale = scale;

                    //    // Get SpriteRenderer and assert SortingLayer
                    //    spawnedObject.GetComponent<SpriteRenderer>().sortingOrder = item.sortingLayer;

                    //    // Assign draggable functionality
                    //    DraggableObject draggable = spawnedObject.AddComponent<DraggableObject>();
                    //    draggable.Initialize(item);
                    //}

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

    public async void CreateObject2D(Object2D object2D)
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

    public async void UpdateObject2D(Object2D object2D)
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

    public async void DeleteObject2D(Object2D object2D)
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

    #endregion

}