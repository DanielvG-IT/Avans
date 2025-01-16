using CropBotics.Data;
using CropBotics.Interfaces;

namespace CropBotics.Functions;

class CommsSystem : IInitializable
{
  private readonly SimpleMqttClient _mqttClient;
  private readonly IMessageHandler _messageHandler;
  private readonly string clientId = "FarmRobot-v1.6";


  public CommsSystem(IMessageHandler messageHandler)
  {
    Console.WriteLine("DEBUG: CommsSystem constructor called");
    _messageHandler = messageHandler;
    _mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(clientId);
    _mqttClient.OnMessageReceived += (sender, message) =>
    {
      Console.WriteLine($"Bericht ontvangen; topic={message.Topic}; message={message.Message};");
      _messageHandler.HandleMessage(message);
    };
  }

  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic("CropBotics/commands");
    await _mqttClient.SubscribeToTopic("CropBotics/request");
  }

  public async Task SendMessage(string topic, string message)
  {
    Console.WriteLine($"DEBUG: Publishing message: msg={message}, topic={topic}");
    await _mqttClient.PublishMessage(message, topic);
  }
}
