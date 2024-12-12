using CropBotics.Data;
using CropBotics.Functions;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics
{
  public class FarmRobot : IUpdatable, IInitializable
  {
    // Local vars for defining hardware 
    const int AlertLedPin = 5;
    const int emergencyStopButtonPin = 6;

    // Local vars for including functions
    private DriveSystem driveSystem = new();
    private CommsSystem commsSystem = new();
    private AlertSystem alertSystem = new();
    private ObstacleDetectionSystem obstacleDetectionSystem = new();

    // IInitializing hardware
    private Button emergencyStopButton = new(emergencyStopButtonPin);
    private Led AlertLed = new(AlertLedPin);











    public Task Init()
    {
      throw new NotImplementedException();
    }

    public void Update()
    {
      throw new NotImplementedException();
    }

    public enum State { INIT, STARTUP, READY, DRIVING, PAUSED, ERROR, EMERGENCY_STOP }














  }
}
