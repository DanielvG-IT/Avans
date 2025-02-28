using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;

public class EnvironmentSystem : MonoBehaviour
{
    [Header("UI Links")]
    public TMP_Text nameText;
    public TMP_Text UserMessage;
    //public GameObject worldMap;

    [Header("Prefab References")]
    public GameObject chipWhiteBluePrefab;
    public GameObject chipGreenPrefab;
    public GameObject chipBlackWhitePrefab;
    public GameObject chipWhitePrefab;
    public GameObject chipBluePrefab;
    public GameObject chipBlueWhitePrefab;
    public GameObject chipRedWhitePrefab;

    // Internal
    private Dictionary<string, GameObject> prefabDictionary;
    private Environment2D environment2D;
    private Object2DApiClient object2DApiClient;
    private Environment2DApiClient enviroment2DApiClient;
    public GameObject SelectedObject { get; private set; }

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
        nameText.text = environment2D.name;
        //worldMap.transform.height = environment2D.maxHeight;
        //worldMap.transform.lenght = environment2D.maxLength;

        ReadObject2Ds();
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
                // Initialize the dictionary with prefabId as the key and the prefab as the value
                prefabDictionary = new Dictionary<string, GameObject>
        {
            { "chipWhiteBlue", chipWhiteBluePrefab },
            { "chipGreen", chipGreenPrefab },
            { "chipBlackWhite", chipBlackWhitePrefab },
            { "chipWhite", chipWhitePrefab },
            { "chipBlue", chipBluePrefab },
            { "chipBlueWhite", chipBlueWhitePrefab },
            { "chipRedWhite", chipRedWhitePrefab }
        };

                // Load data into List
                List<Object2D> object2Ds = dataResponse.Data;

                // Iterate through each item and spawn prefabs based on the prefabId
                foreach (var item in object2Ds)
                {
                    Debug.Log("Loading item with id: " + item.id);
                    if (prefabDictionary.TryGetValue(item.prefabId, out GameObject prefabToInstantiate))
                    {
                        Debug.Log("Found prefab to instanciate: " + prefabToInstantiate.name);
                        // Convert item properties (Position, Scale, and Rotation) to Unity types
                        Vector2 position = new(item.positionX, item.positionY);
                        Quaternion rotation = Quaternion.Euler(0f, 0f, item.rotationZ);
                        Vector2 scale = new(item.scaleX, item.scaleY);

                        // Spawn the object and assert scale
                        GameObject spawnedObject = Instantiate(prefabToInstantiate, position, rotation);
                        spawnedObject.transform.localScale = scale;

                        // Get SpriteRenderer and assert SortingLayer
                        if (spawnedObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                        {
                            spriteRenderer.sortingOrder = item.sortingLayer;
                        }

                        // Assign draggable functionality
                        DraggableObject draggable = spawnedObject.AddComponent<DraggableObject>();
                        draggable.Initialize(item);
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown prefab ID: {item.prefabId}");
                    }
                }

                // TODO Remove existing objects if necessary
                break;

            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read object2Ds error: " + errorMessage);

                UserMessage.color = Color.red;
                UserMessage.text = "Getting existing object went wrong!";
                
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
                // MOVE HERE FROM DraggableObject, otherwise no update or delete possible (cuz of id not there!)
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