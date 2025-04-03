using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AppointmentListUI : MonoBehaviour
{
    public GameObject appointmentPrefab;
    public Transform contentParent;


    private readonly AppointmentApiClient appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;

    async void Start()
    {
        string treatmentId = ApiClientManager.Instance.CurrentTreatment.id;
        var response = await appointmentApiClient.ReadAppointmentsByTreatmentIdAsync(treatmentId);

        switch (response)
        {
            case WebRequestData<List<Appointment>> dataResponse:
                {
                    foreach (var appointment in dataResponse.Data)
                    {
                        GameObject appointmentGO = Instantiate(appointmentPrefab, contentParent);
                        TextMeshProUGUI tmpText = appointmentGO.GetComponentInChildren<TextMeshProUGUI>();

                        if (tmpText != null)
                        {
                            tmpText.text = appointment.name;
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
