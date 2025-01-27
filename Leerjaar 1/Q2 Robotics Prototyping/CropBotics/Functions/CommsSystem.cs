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
    catch (HiveMQttClientException hmqex)
    {
      _farmRobot.HandleExeption(hmqex.Message, hmqex.Source);
      // Handle MQTT-specific errors
    }
    catch (IOException ioex)
    {
      _farmRobot.HandleExeption(ioex.Message, ioex.Source);
      // Handle network/IO related errors
    }
    catch (TimeoutException tex)
    {
      _farmRobot.HandleExeption(tex.Message, tex.Source);
      // Handle timeout errors
    }
    catch (TaskCanceledException tcex)
    {
      _farmRobot.HandleExeption(tcex.Message, tcex.Source);
      // Handle canceled operations
    }
    catch (InvalidOperationException ioex)
    {
      _farmRobot.HandleExeption(ioex.Message, ioex.Source);
      // Handle invalid operations
    }
    catch (NotSupportedException nsex)
    {
      _farmRobot.HandleExeption(nsex.Message, nsex.Source);
    }
    catch (Exception ex)
    {
      _farmRobot.HandleExeption(ex.Message, ex.Source);
      // Handle any other unexpected exceptions
    }
  }

}