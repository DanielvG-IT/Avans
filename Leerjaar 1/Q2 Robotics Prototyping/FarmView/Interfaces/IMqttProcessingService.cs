public interface IMqttProcessingService
{
  string robotStatus { get; set; }
  string robotBattery { get; set; }
  bool robotEmergencyStop { get; }
  bool robotMotorsEnabled { get; }
  string robotColourSensorGain { get; }
  int pixelDistance { get; }
  int obstacleDistance { get; }
}