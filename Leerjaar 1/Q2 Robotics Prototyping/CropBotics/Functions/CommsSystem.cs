using CropBotics.Data;
using CropBotics.Interfaces;
using HiveMQtt.Client.Exceptions;

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
    catch (TaskCanceledException tcex)
    {
      Console.WriteLine($"Task Canceled Exception: {tcex.Message}");
      // Consider retrying or handling the canceled operation
    }
    catch (HiveMQttClientException hmqex)
    {
      Console.WriteLine($"HiveMQtt Client Exception: {hmqex.Message}");
      // Handle MQTT-specific errors
    }
    catch (InvalidOperationException ioex)
    {
      Console.WriteLine($"Invalid Operation Exception: {ioex.Message}");
      // Handle invalid operations
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Unexpected Exception: {ex.Message}");
      // Handle any other unexpected exceptions
    }
  }

}