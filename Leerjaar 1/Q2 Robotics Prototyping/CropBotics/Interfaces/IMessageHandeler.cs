using CropBotics.Data;

namespace CropBotics.Interfaces
{
  public interface IMessageHandler
  {
    void HandleMessage(SimpleMqttMessage message);
  }
}