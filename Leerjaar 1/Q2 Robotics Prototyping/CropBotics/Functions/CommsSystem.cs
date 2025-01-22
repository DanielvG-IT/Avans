using CropBotics.Data;
using CropBotics.Interfaces;

namespace CropBotics.Functions;

class CommsSystem : IInitializable
{
  private readonly SimpleMqttClient _mqttClient;
  private readonly FarmRobot _farmRobot;
  private readonly string clientId = "FarmRobot-v1.6";


  public CommsSystem(FarmRobot farmRobot)
  {
    Console.WriteLine("DEBUG: CommsSystem constructor called");
    _farmRobot = farmRobot;
    _mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(clientId);
    _mqttClient.OnMessageReceived += (sender, message) =>
    {
      Console.WriteLine($"DEBUG: Bericht ontvangen; topic={message.Topic}; message={message.Message};");
      _farmRobot.HandleMessage(message);
    };
  }

  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic("CropBotics/commands");
    await _mqttClient.SubscribeToTopic("CropBotics/request");
  }

  public async Task SendMessage(string topic, string message)
  {
    await _mqttClient.PublishMessage(message, topic);
  }
}
