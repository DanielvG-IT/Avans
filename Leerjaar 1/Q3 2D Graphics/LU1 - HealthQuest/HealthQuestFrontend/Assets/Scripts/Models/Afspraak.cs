[System.Serializable]
public class Afspraak
{
    public string title;
    public string description;
}

[System.Serializable]
public class AfspraakList
{
    public Afspraak[] afspraken;
}
