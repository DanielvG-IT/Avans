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
    _mqttClient.OnMessageReceived += MessageCallback;
  }

  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic("CropBotics/commands");
    await _mqttClient.SubscribeToTopic("CropBotics/request");
  }

  private void MessageCallback(object? sender, SimpleMqttMessage msg)
  {
    _messageHandler.HandleMessage(msg);
  }

  public async Task SendMessage(string topic, string message)
  {
    Console.WriteLine($"Publishing message: topic={topic}, msg={message}");
    await _mqttClient.PublishMessage(topic, message);
  }
}
