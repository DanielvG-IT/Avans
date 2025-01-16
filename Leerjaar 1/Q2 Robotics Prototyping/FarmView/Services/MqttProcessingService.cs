using System.Text.RegularExpressions;
using SimpleMqtt;


public class MqttProcessingService : IHostedService, IMqttProcessingService
{
  private readonly IDatabaseAccess _databaseAccess;
  private readonly SimpleMqttClient _mqttClient;

  // Public properties for dashboard
  public string robotStatus { get; set; }
  public string robotColourSensorGain { get; private set; }
  public bool robotEmergencyStop { get; private set; }
  public bool robotMotorsEnabled { get; private set; }
  public int obstacleDistance { get; private set; }
  public int pixelDistance { get; private set; }
  public int robotBattery { get; set; }

  public MqttProcessingService(IDatabaseAccess databaseAccess, SimpleMqttClient mqttClient)
  {
    _databaseAccess = databaseAccess;
    _mqttClient = mqttClient;

    // Initialize public properties
    robotStatus = "Unknown";
    robotBattery = 0;
    robotEmergencyStop = false;
    robotMotorsEnabled = true;
    robotColourSensorGain = "Unknown";

    _mqttClient.OnMessageReceived += (sender, args) =>
    {
      Console.WriteLine($"DEBUG: Incoming MQTT message on {args.Topic}:{args.Message}");

      string topic = args.Topic ?? string.Empty;
      string message = args.Message ?? string.Empty;

      string pixelPattern = @"CropBotics/pixel/(\d+)";
      string commandPattern = @"CropBotics/command/(\w+)";
      string sensorPattern = @"CropBotics/sensor/(\w+)";
      string statusPattern = @"CropBotics/status/(\w+)";
      string requestPattern = @"CropBotics/request/(\w+)";

      var pixelMatch = Regex.Match(topic, pixelPattern);
      var commandMatch = Regex.Match(topic, commandPattern);
      var sensorMatch = Regex.Match(topic, sensorPattern);
      var statusMatch = Regex.Match(topic, statusPattern);
      var requestMatch = Regex.Match(topic, requestPattern);

      var mqttMessage = new SimpleMqttMessage { Topic = args.Topic, Message = args.Message };

      if (pixelMatch.Success)
      {
        int pixelNumber = Convert.ToInt32(pixelMatch.Groups[1].Value);
        _databaseAccess.WritePixelData(mqttMessage, pixelNumber);
      }
      else if (sensorMatch.Success)
      {
        _databaseAccess.WriteMqttData(mqttMessage, "sensor");

        switch (sensorMatch.Groups[1].Value)
        {
          case "pixelDistance":
            pixelDistance = Convert.ToInt32(message);
            break;
          case "obstacleDistance":
            obstacleDistance = Convert.ToInt32(message);
            break;
        }
      }
      else if (commandMatch.Success)
      {
        _databaseAccess.WriteMqttData(mqttMessage, "command");
      }
      else if (statusMatch.Success)
      {
        switch (statusMatch.Groups[1].Value)
        {
          case "status":
            robotStatus = message;
            break;
          case "battery":
            robotBattery = Convert.ToInt32(message);
            break;
          case "emergency_stop":
            robotEmergencyStop = message == "true";
            break;
          default:
            Console.WriteLine("Unknown status message received");
            break;
        }
      }
      else if (requestMatch.Success)
      {
        switch (requestMatch.Groups[1].Value)
        {
          case "motorsEnabled":
            robotMotorsEnabled = message == "true";
            break;
          case "colourGain":
            robotColourSensorGain = message;
            break;
          default:
            Console.WriteLine("Unknown request response received");
            break;
        }
      }
    };
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _mqttClient.SubscribeToTopic("CropBotics/#");
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _mqttClient.Dispose();
    return Task.CompletedTask;
  }
}
