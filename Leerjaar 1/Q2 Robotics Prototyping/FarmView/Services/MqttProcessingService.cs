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

      string pattern = @"cropbotics/sensor/(.*)";
      string input = args.Topic ?? string.Empty;
      Match match = Regex.Match(input, pattern);


      if (match.Success)
      {
        var mqttMessage = new SimpleMqttMessage { Topic = args.Topic, Message = args.Message };
        _databaseAccess.WriteMqttData(mqttMessage, "sensor");
      }

      else
      {
        var mqttMessage = new SimpleMqttMessage { Topic = args.Topic, Message = args.Message };
        _databaseAccess.WriteMqttData(mqttMessage, "command");
      }
    };
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
