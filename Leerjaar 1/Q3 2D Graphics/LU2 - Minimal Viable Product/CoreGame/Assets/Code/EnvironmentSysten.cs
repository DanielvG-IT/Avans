using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnvironmentSystem : MonoBehaviour
{
    [Header("Prefab Linking")]
    public GameObject circlePrefab;
    public GameObject squarePrefab;
    public GameObject trianglePrefab;

    [Header("UI References")]
    public TMP_InputField envName;
    public TMP_InputField envMaxHeight;
    public TMP_InputField envMaxLength;
    public UnityEngine.UIElements.ScrollView loadEnvironment;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private GameObject environmentButtonPrefab;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient enviroment2DApiClient;
    public Object2DApiClient object2DApiClient;
    
    [Header("TEMP")]
    private List<Environment2D> environmentList;
    //private Environment2D loadedEnvironment;

    private void Start()
    {
        ReadEnvironments();
    }


    #region Environment

    public void ShowExistingEnvironments()
    {
        // Clear existing items.
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in environmentList)
        {
            // Instantiate a new button for each environment.
            GameObject buttonObj = Instantiate(environmentButtonPrefab, contentPanel);
            Button button = buttonObj.GetComponent<Button>();

            // Set the button's text to the environment name.
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = item.name;
            }

            // Use a local copy of the environment ID for the listener (to avoid closure issues).
            string environmentId = item.id;
            button.onClick.AddListener(() => {
                //LoadEnvironment(environmentId); 
                Debug.Log($"Environment with name {item.name}");
            });
        }
    }

    //public async void CreateEnvironment2D()
    //{
    //    IWebRequestReponse webRequestResponse = await enviroment2DApiClient.CreateEnvironment(environment2D);

    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<Environment2D> dataResponse:
    //            environment2D.id = dataResponse.Data.id;
    //            // TODO: Handle succes scenario.
    //            break;
    //        case WebRequestError errorResponse:
    //            string errorMessage = errorResponse.ErrorMessage;
    //            Debug.Log("Create environment2D error: " + errorMessage);
    //            // TODO: Handle error scenario. Show the errormessage to the user.
    //            break;
    //        default:
    //            throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //    }
    //}

    public async void ReadEnvironments()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                environmentList = dataResponse.Data;
                Debug.Log("List of environment2Ds: ");
                // TODO: Handle succes scenario.
                ShowExistingEnvironments();
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    //public void LoadEnvironment(string environmentId)
    //{
    //    // TODO Some magic
    //    loadedEnvironment = null; // TODO make get list entry
    //    //ReadObjects();
    //}

    //public async void DeleteEnvironments()
    //{
    //    IWebRequestReponse webRequestResponse = await enviroment2DApiClient.DeleteEnvironment(loadedEnvironment.id);

    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<string> dataResponse:
    //            string responseData = dataResponse.Data;
    //            // TODO: Handle succes scenario.
    //            break;
    //        case WebRequestError errorResponse:
    //            string errorMessage = errorResponse.ErrorMessage;
    //            Debug.Log("Delete environment error: " + errorMessage);
    //            // TODO: Handle error scenario. Show the errormessage to the user.
    //            break;
    //        default:
    //            throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //    }
    //}

    #endregion Environment

    #region Object2D

    //public async void ReadObjects()
    //{
    //    IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(object2D.environmentId);

    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<List<Object2D>> dataResponse:
    //            List<Object2D> object2Ds = dataResponse.Data;
    //            Debug.Log("List of object2Ds: " + object2Ds);
    //            object2Ds.ForEach(object2D => Debug.Log(object2D.id));
    //            GameObject prefabToInstantiate;
    //            // TODO: Succes scenario. Show the enviroments in the UI

    //            foreach (var item in object2Ds)
    //            {
    //                prefabToInstantiate = null;

    //                switch (item.prefabId)
    //                {
    //                    case "Circle":
    //                        prefabToInstantiate = circlePrefab;
    //                        break;
    //                    case "Square":
    //                        prefabToInstantiate = squarePrefab;
    //                        break;
    //                    case "Triangle":
    //                        prefabToInstantiate = trianglePrefab;
    //                        break;
    //                    default:
    //                        Debug.LogWarning($"Unknown prefab ID: {item.prefabId}");
    //                        continue;
    //                }

    //                if (prefabToInstantiate != null)
    //                {
    //                    // Convert item properties (Position, Scale and Rotation) to Unity types
    //                    Vector2 position = new(item.positionX, item.positionY);
    //                    Quaternion rotation = Quaternion.Euler(0f, 0f, item.rotationZ);
    //                    Vector2 scale = new(item.scaleX, item.scaleY);

    //                    // Spawn Object and assert scale
    //                    GameObject spawnedObject = Instantiate(prefabToInstantiate, position, rotation);
    //                    spawnedObject.transform.localScale = scale;

    //                    // Get SpriteRenderer and assert SortingLayer
    //                    spawnedObject.GetComponent<SpriteRenderer>().sortingOrder = item.sortingLayer;
    //                }

    //                // TODO Remove existing objects

    //            }
    //            break;

    //        case WebRequestError errorResponse:
    //            string errorMessage = errorResponse.ErrorMessage;
    //            Debug.Log("Read object2Ds error: " + errorMessage);
    //            // TODO: Error scenario. Show the errormessage to the user.
    //            break;

    //        default:
    //            throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //    }
    //}

    //public async void CreateObject2D()
    //{
    //    IWebRequestReponse webRequestResponse = await object2DApiClient.CreateObject2D(object2D);

    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<Object2D> dataResponse:
    //            object2D.id = dataResponse.Data.id;
    //            // TODO: Handle succes scenario.
    //            break;
    //        case WebRequestError errorResponse:
    //            string errorMessage = errorResponse.ErrorMessage;
    //            Debug.Log("Create Object2D error: " + errorMessage);
    //            // TODO: Handle error scenario. Show the errormessage to the user.
    //            break;
    //        default:
    //            throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //    }
    //}

    //public async void UpdateObject2D()
    //{
    //    IWebRequestReponse webRequestResponse = await object2DApiClient.UpdateObject2D(object2D);

    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<string> dataResponse:
    //            string responseData = dataResponse.Data;
    //            // TODO: Handle succes scenario.
    //            break;
    //        case WebRequestError errorResponse:
    //            string errorMessage = errorResponse.ErrorMessage;
    //            Debug.Log("Update object2D error: " + errorMessage);
    //            // TODO: Handle error scenario. Show the errormessage to the user.
    //            break;
    //        default:
    //            throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //    }
    //}

    #endregion

}
