using TMPro;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class EnvironmentSystem : MonoBehaviour
{
    [Header("UI Links")]
    public TMP_Text nameText;
    public TMP_Text UserMessage;

    [Header("Border References")]
    public GameObject BorderPrefab;

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

    #region WorldManager

    public void Start()
    {
        enviroment2DApiClient = ApiClientManager.Instance.Environment2DApiClient;
        object2DApiClient = ApiClientManager.Instance.Object2DApiClient;
        environment2D = GameManager.Instance.SelectedEnvironment;

        nameText.text = environment2D.name;

        CreateDictionary();
        ReadObject2Ds();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void ShowErrorMessage(string message)
    {
        UserMessage.color = Color.red;
        UserMessage.text = message;
        Debug.LogError(message);
    }

    public void CreateDictionary()
    {
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
    }

    public void SpawnObject2D(Object2D objectData)
    {
        if (prefabDictionary.TryGetValue(objectData.prefabId, out GameObject prefabToInstantiate))
        {
            Vector2 position = new(objectData.positionX, objectData.positionY);
            GameObject spawnedObject = Instantiate(prefabToInstantiate, position, Quaternion.identity);

            // Assign draggable functionality with proper ID
            DraggableObject draggable = spawnedObject.AddComponent<DraggableObject>();
            draggable.Initialize(objectData);
        }
    }

    #endregion

    #region Environment2D

    public async Task<bool> UpdateEnvironment2D(Environment2D changedEnvironment)
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.UpdateEnvironment(changedEnvironment);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                var updatedEnvironment = dataResponse.Data;
                GameManager.Instance.SelectedEnvironment = updatedEnvironment;
                return true;
            case WebRequestError:
                ShowErrorMessage("ERROR: Couldn't update environment!");
                return false;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async Task<bool> DeleteEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.DeleteEnvironment(environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<string>:
                return true;
            case WebRequestError:
                ShowErrorMessage("ERROR: Couldn't delete environment!");
                return false;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    #endregion

    #region Object2D

    public async Task<Object2D> CreateObject2D(Object2D object2D)
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.CreateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2D> dataResponse:
                object2D.id = dataResponse.Data.id; // Server-generated Guid
                object2D.positionX = dataResponse.Data.positionX; // Server-validated position
                object2D.positionY = dataResponse.Data.positionY;
                object2D.sortingLayer = dataResponse.Data.sortingLayer;
                object2D.environmentId = dataResponse.Data.environmentId;

                return object2D;
            case WebRequestError:
                ShowErrorMessage("ERROR: Couldn't create environment!");
                return null;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async void ReadObject2Ds()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                // Load data into List
                List<Object2D> object2Ds = dataResponse.Data;

                // Iterate through each item and spawn prefabs based on the prefabId
                foreach (var item in object2Ds)
                {
                    if (prefabDictionary.TryGetValue(item.prefabId, out GameObject prefabToInstantiate))
                    {
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
                break;

            case WebRequestError:
                ShowErrorMessage("ERROR: Couldn't read objects!");
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public async Task<Object2D> UpdateObject2D(Object2D object2D)
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.UpdateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2D> dataResponse:
                return dataResponse.Data; // Return server-validated data

            case WebRequestError errorResponse:
                ShowErrorMessage("ERROR: Couldn't update object!");
                return null;

            default:
                throw new NotImplementedException("Unexpected response type: " + webRequestResponse.GetType());
        }
    }

    public async Task<bool> DeleteObject2D(Object2D object2D)
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.DeleteObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string>:
                return true;
            case WebRequestError:
                ShowErrorMessage("ERROR: Couldn't delete object!");
                return false;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    #endregion

}