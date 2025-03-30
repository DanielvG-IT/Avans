using UnityEngine;

public class ApiClientManager : MonoBehaviour
{
    public static ApiClientManager Instance;

    [Header("ApiClients")]
    public WebClient WebClient;
    public UserApiClient UserApiClient;
    public GuardianApiClient GuardianApiClient;
    public PatientApiClient PatientApiClient;

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
        if (!PatientApiClient) { PatientApiClient = GetComponent<PatientApiClient>(); }
    }

    // Properties and methodes for storing data like logged in user, etc.
    public User CurrentUser { get; private set; }
    public void SetCurrentUser(User user)
    {
        CurrentUser = user;
    }

    public Guardian CurrentGuardian { get; private set; }
    public void SetCurrentGuardian(Guardian guardian)
    {
        CurrentGuardian = guardian;
    }

    public Patient CurrentPatient { get; private set; }
    public void SetCurrentPatient(Patient patient)
    {
        CurrentPatient = patient;
    }
}
