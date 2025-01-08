using CropBotics.Data;
using CropBotics.Interfaces;

namespace CropBotics.Functions;

class CommsSystem : IInitializable
{

  const string topicCommands = "CropBotics/commands/#";
  const string topicAlert = "CropBotics/alerts/#";

  private readonly SimpleMqttClient _mqttClient;
  private readonly IMessageHandler _messageHandler;
  private string clientId = "FarmRobot-v1";


  public CommsSystem(IMessageHandler messageHandler)
  {
    _mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(clientId);
    _messageHandler = messageHandler;
  }



  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic(topicCommands);
    _mqttClient.OnMessageReceived += MessageCallback;
  }


  private void MessageCallback(object? sender, SimpleMqttMessage msg)
  {
    Console.WriteLine($"MQTT message received: topic={msg.Topic}, msg={msg.Message}");
    _messageHandler.HandleMessage(msg);
  }

  public async Task SendState(string state)
  {
    Console.WriteLine($"Publishing alert state: topic={topicAlert}, msg={state}");
    await _mqttClient.PublishMessage(state, topicAlert);
  }
}
