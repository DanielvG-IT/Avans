public interface IMqttProcessingService
{
  string robotStatus { get; }
  int robotBattery { get; }
  bool robotEmergencyStop { get; }
}