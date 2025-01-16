public interface IMqttProcessingService
{
  string robotStatus { get; set; }
  int robotBattery { get; set; }
  bool robotEmergencyStop { get; }
  bool robotMotorsEnabled { get; }
  string robotColourSensorGain { get; }
}