using System.Collections.Generic;
using UnityEngine;


public class GanzenbordManager : MonoBehaviour
{
    [Header("Data Instellingen")]
    public TextAsset afsprakenJSON;

    private List<Afspraak> afspraken = new List<Afspraak>();
    private int levelsCompleted = 0;

    void Awake()
    {
        LaadAfsprakenUitJSON();
    }

    private void LaadAfsprakenUitJSON()
    {
        if (afsprakenJSON == null)
        {
            Debug.LogError("❌ JSON-bestand niet toegewezen.");
            return;
        }

        var data = JsonUtility.FromJson<AfspraakList>("{\"afspraken\":" + afsprakenJSON.text + "}");
        afspraken = new List<Afspraak>(data.afspraken);
    }

    public Afspraak GetAfspraak(int index)
    {
//<<<<<<< Updated upstream
        if (index >= 0 && index < afspraken.Count)
//=======
//        Debug.Log("Syncing completed levels with backend...");
//        var treatmentId = apiClientManager.CurrentTreatment.id;

//        IWebRequestReponse response = await apiClientManager.AppointmentApiClient.ReadAppointmentsByTreatmentIdAsync(treatmentId);

//        Debug.Log(JsonUtility.ToJson(response, true));

//        switch (response)
//>>>>>>> Stashed changes
        {
            return afspraken[index];
        }

        Debug.LogWarning("⚠️ Ongeldig index in GetAfspraak");
        return null;
    }

    public void VoltooiLevel(int index)
    {
        if (index >= 0 && index < afspraken.Count && index >= levelsCompleted)
        {
            levelsCompleted = index + 1;
        }
    }

    public bool IsLevelUnlocked(int index)
    {
        return index <= levelsCompleted;
    }

    public bool IsLevelVoltooid(int index)
    {
        return index < levelsCompleted;
    }

    public int AantalLevels => afspraken.Count;

    public int GetLevelsCompleted() => levelsCompleted;

    public void SetLevelsCompleted(int count) => levelsCompleted = Mathf.Clamp(count, 0, AantalLevels);
}
