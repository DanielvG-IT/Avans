using Microsoft.Data.SqlClient;
using SimpleMqtt;

public interface IDatabaseAccess
{
  public void WriteMqttData(SimpleMqttMessage mqtt, string typeData);
  public void ReadMqttData(string typeData);
  public void WritePixelData(SimpleMqttMessage mqtt, string typeData, int pixelNumber);
  public List<Pixel> ReadPixelData(DateTime date);
}