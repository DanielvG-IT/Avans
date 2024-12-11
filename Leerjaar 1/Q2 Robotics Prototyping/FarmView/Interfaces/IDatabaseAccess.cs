using SimpleMqtt;

public interface IDatabaseAccess
{
  public void WriteMqttData(SimpleMqttMessage mqtt, string typeData);
  public void ReadMqttData(string typeData);

  // TODO Add new methods from DatabaseAccess.cs

}