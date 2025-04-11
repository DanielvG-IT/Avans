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

        if (response is WebRequestError error)
        {
            Debug.LogError($"Failed to load appointments from API: {error.ErrorMessage}");
            return;
        }

        if (response is WebRequestData<List<Appointment>> data && data.Data is not null && data.Data.Count > 0)
        {
            foreach (var appointment in data.Data)
            {
                var appointmentGO = Instantiate(appointmentPrefab, contentParent);
                var tmpText = appointmentGO.GetComponentInChildren<TextMeshProUGUI>();

                if (tmpText != null)
                {
                    tmpText.text = $"{counter}: {appointment.name}";
                    counter++;
                }
            }
        }
        else
        {
            Debug.Log("No appointments found for this treatment.");
        }
    }
}