using System.Text.RegularExpressions;
using SimpleMqtt;


public class MqttProcessingService : IHostedService, IMqttProcessingService
{
  private readonly IDatabaseAccess _databaseAccess;
  private readonly SimpleMqttClient _mqttClient;

  // Public properties for dashboard
  public string robotStatus { get; set; }
  public string robotBattery { get; set; }
  public ColorSensorGain RobotColourSensorGain { get; private set; }
  public bool robotEmergencyStop { get; private set; }
  public bool robotMotorsEnabled { get; private set; }
  public int obstacleDistance { get; private set; }
  public int pixelDistance { get; private set; }
  public short CalibrationLeft { get; private set; }
  public short CalibrationRight { get; private set; }

  private readonly Timer? _statusTimer;
  private const int STATUS_TIMEOUT_MS = 10000;

  public MqttProcessingService(IDatabaseAccess databaseAccess, SimpleMqttClient mqttClient)
  {
    _databaseAccess = databaseAccess;
    _mqttClient = mqttClient;
    _statusTimer = new Timer(OnStatusTimeout, null, Timeout.Infinite, Timeout.Infinite);

    // Initialize public properties
    robotStatus = "Offline";
    robotBattery = "0";
    robotEmergencyStop = false;
    robotMotorsEnabled = true;
    pixelDistance = 0;
    obstacleDistance = 0;
    RobotColourSensorGain = ColorSensorGain.GAIN_1X;

    _mqttClient.OnMessageReceived += (sender, args) =>
    {
      Console.WriteLine($"DEBUG: Incoming MQTT message on {args.Topic}:{args.Message}");

      string topic = args.Topic ?? string.Empty;
      string message = args.Message ?? string.Empty;

      string commandPattern = @"CropBotics/command/(\w+)";
      string requestPattern = @"CropBotics/request/(\w+)";
      string sensorPattern = @"CropBotics/sensor/(\w+)";
      string statusPattern = @"CropBotics/status/(\w+)";
      string pixelPattern = @"CropBotics/pixel/(\d+)";

      var requestMatch = Regex.Match(topic, requestPattern);
      var commandMatch = Regex.Match(topic, commandPattern);
      var sensorMatch = Regex.Match(topic, sensorPattern);
      var statusMatch = Regex.Match(topic, statusPattern);
      var pixelMatch = Regex.Match(topic, pixelPattern);

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
            {
              pixelDistance = Convert.ToInt32(message);
              break;
            }
          case "obstacleDistance":
            {
              obstacleDistance = Convert.ToInt32(message);
              break;
            }
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
            {
              if (message == "Online")
              {
                robotStatus = "Online";
                _statusTimer.Change(STATUS_TIMEOUT_MS, Timeout.Infinite);
              }
              break;
            }
          case "battery":
            {
              robotBattery = message;
              break;
            }
          case "emergency_stop":
            {
              robotEmergencyStop = message == "True";
              break;
            }
          default:
            {
              Console.WriteLine("Unknown status message received");
              break;
            }
        }
      }
      else if (requestMatch.Success)
      {
        switch (requestMatch.Groups[1].Value)
        {
          case "MotorsEnabled":
            {
              robotMotorsEnabled = message == "True";
              break;
            }
          case "colourGain":
            {
              RobotColourSensorGain = Enum.Parse<ColorSensorGain>($"GAIN_{message}");
              break;
            }
          case "MotorCalibrationLeft":
            {
              CalibrationLeft = Convert.ToInt16(message);
              break;
            }
          case "MotorCalibrationRight":
            {
              CalibrationRight = Convert.ToInt16(message);
              break;
            }
          default:
            {
              Console.WriteLine("Unknown request response received");
              break;
            }
        }
      }
    };
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _mqttClient.SubscribeToTopic("CropBotics/status/#");  // Subscribe to all status topics
    await _mqttClient.SubscribeToTopic("CropBotics/sensor/#");  // Subscribe to all sensor topics 
    await _mqttClient.SubscribeToTopic("CropBotics/pixel/#");   // Subscribe to all pixel topics
    await _mqttClient.SubscribeToTopic("CropBotics/request/#"); // Subscribe to all request topics
    await _mqttClient.SubscribeToTopic("CropBotics/command/#"); // Subscribe to all command topics
  }

  private void OnStatusTimeout(object? state)
  {
    robotStatus = "Offline";
    robotBattery = "0";
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    _mqttClient.Dispose();

    if (_statusTimer != null)
    {
      await _statusTimer.DisposeAsync();
    }
  }
}