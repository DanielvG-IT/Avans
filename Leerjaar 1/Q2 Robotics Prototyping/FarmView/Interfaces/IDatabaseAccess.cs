using SimpleMqtt;

public interface IDatabaseAccess
{
  public List<Sensor> ReadSensorData(DateTime date);
  public List<Command> ReadCommandData(DateTime date);
  public List<Pixel> ReadPixelData(DateTime date);
  public void WriteMqttData(SimpleMqttMessage mqtt, string typeData);
  public void WritePixelData(SimpleMqttMessage mqtt, int pixelNumber);
}