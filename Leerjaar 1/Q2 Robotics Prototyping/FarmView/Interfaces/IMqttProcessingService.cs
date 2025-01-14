public interface IMqttProcessingService
{
  string robotStatus { get; }
  int robotBattery { get; }
  bool robotEmergencyStop { get; }
  bool robotMotorsEnabled { get; }
  string robotColourSensorGain { get; }
}