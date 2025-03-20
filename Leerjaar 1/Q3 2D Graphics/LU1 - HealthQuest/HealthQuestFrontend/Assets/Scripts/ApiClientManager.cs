using UnityEngine;

public class ApiClientManager : MonoBehaviour
{
    public static ApiClientManager Instance;

    // References to other components on the same GameObject
    public WebClient WebClient;
    //public UserApiClient UserApiClient;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // <-- Key line to keep this GameObject across scenes
        }
        else
        {
            Destroy(gameObject); // If another instance already exists, destroy this one
            return;
        }

        if (!WebClient) WebClient = GetComponent<WebClient>();
        //if (!UserApiClient) UserApiClient = GetComponent<UserApiClient>();
    }
}
