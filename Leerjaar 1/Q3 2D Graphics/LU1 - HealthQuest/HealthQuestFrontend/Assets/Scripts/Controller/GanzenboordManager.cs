using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GanzenboordManager : MonoBehaviour
{
    public TextAsset appointmentsJson;

    private List<Afspraak> appointments = new();
    private int completedLevels = 0;
    private ApiClientManager apiClientManager => ApiClientManager.Instance;

    public int TotalLevels => appointments.Count;
    public int CompletedLevelCount => completedLevels;

    void Awake()
    {
        LoadAppointmentsFromJson();
    }

    private void LoadAppointmentsFromJson()
    {
        if (appointmentsJson == null)
        {
            Debug.LogError("No JSON file assigned to GooseBoardManager.");
            return;
        }

        try
        {
            string wrappedJson = $"{{\"afspraken\":{appointmentsJson.text}}}";
            var data = JsonUtility.FromJson<AfspraakList>(wrappedJson);
            appointments = new List<Afspraak>(data.afspraken);
        }
        catch
        {
            Debug.LogError("Failed to parse appointment JSON.");
        }
    }

    public void MarkLevelCompleted(int index)
    {
        if (!IsValidIndex(index)) return;
        if (index >= completedLevels) completedLevels = index + 1;
    }

    public bool IsLevelUnlocked(int index) => IsValidIndex(index) && index <= completedLevels;
    public bool IsLevelCompleted(int index) => IsValidIndex(index) && index < completedLevels;
    public Afspraak GetAppointment(int index) => IsValidIndex(index) ? appointments[index] : null;
    public void SetCompletedLevelCount(int count) => completedLevels = Mathf.Clamp(count, 0, TotalLevels);

    private bool IsValidIndex(int index) => index >= 0 && index < TotalLevels;
}
