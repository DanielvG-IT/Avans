using UnityEngine;

public class ApiClientManager : MonoBehaviour
{
    public static ApiClientManager Instance;

    [Header("ApiClients")]
    public WebClient WebClient;
    public UserApiClient UserApiClient;
    public GuardianApiClient GuardianApiClient;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!WebClient) { WebClient = GetComponent<WebClient>(); }
        if (!UserApiClient) { UserApiClient = GetComponent<UserApiClient>(); }
        if (!GuardianApiClient) { GuardianApiClient = GetComponent<GuardianApiClient>(); }
    }
}
