using UnityEngine;

public class ApiClientManager : MonoBehaviour
{
    public static ApiClientManager Instance;

    // References to other components on the same GameObject
    // (These will be automatically populated if you drag them in the Inspector, or you can use GetComponent<> in Awake() to populate them.)
    public WebClient WebClient;
    public UserApiClient UserApiClient;
    public Object2DApiClient Object2DApiClient;
    public Environment2DApiClient Environment2DApiClient;

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

        // If you prefer automatic assignment (instead of Inspector references):
        WebClient = GetComponent<WebClient>();
        UserApiClient = GetComponent<UserApiClient>();
        Object2DApiClient = GetComponent<Object2DApiClient>();
        Environment2DApiClient = GetComponent<Environment2DApiClient>();
    }
}
