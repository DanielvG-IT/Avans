using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AppointmentListUI : MonoBehaviour
{
    public GameObject appointmentPrefab;
    public Transform contentParent;

    private AppointmentApiClient appointmentApiClient;
    private Treatment treatment;
    private int counter = 1;


    public async void Start()
    {
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;
        treatment = ApiClientManager.Instance.CurrentTreatment;
        var response = await appointmentApiClient.ReadAppointmentsByTreatmentIdAsync(treatment.id);
        if (response == null)
        {
            Debug.LogError("Failed to load appointments from API.");
            return;
        }

        switch (response)
        {
            case WebRequestData<List<AppointmentWithNr>> dataResponse:
                {
                    if (dataResponse.Data == null || dataResponse.Data.Count == 0)
                    {
                        Debug.Log("No appointments found for this treatment.");
                        return;
                    }

                    foreach (var appointment in dataResponse.Data)
                    {
                        GameObject appointmentGO = Instantiate(appointmentPrefab, contentParent);
                        TextMeshProUGUI tmpText = appointmentGO.GetComponentInChildren<TextMeshProUGUI>();

                        if (tmpText != null)
                        {
                            tmpText.text = $"{counter}: {appointment.name}";
                            counter++;
                        }
                    }
                    break;
                }

            case WebRequestError errorResponse:
                {
                    Debug.Log("Error: " + errorResponse.ErrorMessage);
                    break;
                }
        }
    }
}
