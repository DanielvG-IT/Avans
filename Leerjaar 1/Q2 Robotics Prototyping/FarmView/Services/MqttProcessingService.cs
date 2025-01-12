using SimpleMqtt;
using System.Text.RegularExpressions;


public class MqttProcessingService : IHostedService
{
  private readonly IDatabaseAccess _databaseAccess;
  private readonly SimpleMqttClient _mqttClient;

  public MqttProcessingService(IDatabaseAccess databaseAccess, SimpleMqttClient mqttClient)
  {
    _databaseAccess = databaseAccess;
    _mqttClient = mqttClient;

    _mqttClient.OnMessageReceived += (sender, args) =>
    {
      Console.WriteLine($"Incoming MQTT message on {args.Topic}:{args.Message}");

      string topic = args.Topic ?? string.Empty;
      string sensorPattern = @"Cropbotics/sensor/(.*)";
      string pixelPattern = @"Cropbotics/pixel/(\d+)";
      var sensorMatch = Regex.Match(topic, sensorPattern);
      var pixelMatch = Regex.Match(topic, pixelPattern);

      var mqttMessage = new SimpleMqttMessage { Topic = args.Topic, Message = args.Message };

      if (sensorMatch.Success)
      {
        _databaseAccess.WriteMqttData(mqttMessage, "sensor");
      }
      else if (pixelMatch.Success)
      {
        int pixelNumber = Convert.ToInt32(pixelMatch.Groups[1].Value);
        _databaseAccess.WritePixelData(mqttMessage, "pixel", pixelNumber);

        // TODO: Display pixel on display or trigger reload that gets it from the database
      }
      else
      {
        _databaseAccess.WriteMqttData(mqttMessage, "command");
      }
    };
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _mqttClient.SubscribeToTopic("Cropbotics/#");
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _mqttClient.Dispose();
    return Task.CompletedTask;
  }
}
