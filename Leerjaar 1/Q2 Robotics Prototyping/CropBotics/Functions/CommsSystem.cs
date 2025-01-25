using CropBotics.Data;
using CropBotics.Interfaces;

namespace CropBotics.Functions;

class CommsSystem : IInitializable
{
  private readonly SimpleMqttClient _mqttClient;
  private readonly FarmRobot _farmRobot;

  public CommsSystem(FarmRobot farmRobot)
  {
    Console.WriteLine("DEBUG: CommsSystem constructor called");
    _farmRobot = farmRobot;
    _mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ("FarmRobot-v1.6");
    _mqttClient.OnMessageReceived += (sender, message) =>
    {
      Console.WriteLine($"DEBUG: Bericht ontvangen; topic={message.Topic}; message={message.Message};");
      _farmRobot.HandleMessage(message);
    };
  }

  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic("CropBotics/command");
    await _mqttClient.SubscribeToTopic("CropBotics/request");
  }

  public async Task SendMessage(string topic, string message)
  {
    try
    {
      await _mqttClient.PublishMessage(message, topic);
    }
    catch (InvalidOperationException ioex)
    {
      Console.WriteLine($"Invalid Operation Exception thrown: {ioex.Message}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Exception thrown: {ex.Message}");
    }

  }
}
