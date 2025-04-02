using UnityEngine;

public class DagboekScherm : MonoBehaviour
{
    

    private PatientApiClient patientApiClient;
    private JournalApiClient journalApiClient;

    public void Start()
    {
        journalApiClient = ApiClientManager.Instance.JournalApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
    }
}
