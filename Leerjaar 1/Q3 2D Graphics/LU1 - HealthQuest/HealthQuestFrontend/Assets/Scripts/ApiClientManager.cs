using UnityEngine;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ApiClientManager : MonoBehaviour
{
    public static ApiClientManager Instance;

    [Header("ApiClients")]
    public WebClient WebClient;
    public UserApiClient UserApiClient;
    public DoctorApiClient DoctorApiClient;
    public JournalApiClient JournalApiClient;
    public PatientApiClient PatientApiClient;
    public StickerApiClient StickerApiClient;
    public GuardianApiClient GuardianApiClient;
    public TreatmentApiClient TreatmentApiClient;
    public AppointmentApiClient AppointmentApiClient;

    private Timer tokenTimer;

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

        // Get the ApiClients from the GameObjects
        if (WebClient == null) { WebClient = GetComponent<WebClient>(); }
        if (UserApiClient == null) { UserApiClient = GetComponent<UserApiClient>(); }
        if (DoctorApiClient == null) { DoctorApiClient = GetComponent<DoctorApiClient>(); }
        if (JournalApiClient == null) { JournalApiClient = GetComponent<JournalApiClient>(); }
        if (PatientApiClient == null) { PatientApiClient = GetComponent<PatientApiClient>(); }
        if (StickerApiClient == null) { StickerApiClient = GetComponent<StickerApiClient>(); }
        if (GuardianApiClient == null) { GuardianApiClient = GetComponent<GuardianApiClient>(); }
        if (TreatmentApiClient == null) { TreatmentApiClient = GetComponent<TreatmentApiClient>(); }
        if (AppointmentApiClient == null) { AppointmentApiClient = GetComponent<AppointmentApiClient>(); }
    }

    private void Start()
    {
        tokenTimer = new Timer(1800000); // 30 minutes
        tokenTimer.Elapsed += OnTokenRefresh;
        tokenTimer.AutoReset = true;
        tokenTimer.Enabled = true;
    }

    private void OnApplicationQuit()
    {
        tokenTimer?.Stop();
        tokenTimer?.Dispose();
    }

    private async void OnTokenRefresh(object sender, ElapsedEventArgs e)
    {
        var refreshToken = new RefreshToken { refreshToken = WebClient.token.refreshToken };
        var refreshResult = await UserApiClient.RefreshAccessToken(refreshToken);
        if (refreshResult is WebRequestError refreshError)
        {
            Debug.Log($"Failed to refresh the token: {refreshError.ErrorMessage}");
        }
        else if (refreshResult is WebRequestData<string> refreshData)
        {
            Debug.Log($"Refresh data: {refreshData}");
        }
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

    public Treatment CurrentTreatment { get; private set; }
    public void SetCurrentTreatment(Treatment treatment)
    {
        CurrentTreatment = treatment;
    }

    public void ClearData()
    {
        CurrentUser = null;
        CurrentGuardian = null;
        CurrentPatient = null;
        CurrentTreatment = null;
    }
}
