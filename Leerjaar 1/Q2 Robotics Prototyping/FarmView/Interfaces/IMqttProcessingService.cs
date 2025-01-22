public interface IMqttProcessingService
{
  ColorSensorGain RobotColourSensorGain { get; }
  string robotStatus { get; set; }
  string robotBattery { get; set; }
  bool robotEmergencyStop { get; }
  bool robotMotorsEnabled { get; }
  int pixelDistance { get; }
  int obstacleDistance { get; }
  short CalibrationLeft { get; }
  short CalibrationRight { get; }
}