using CropBotics.Models;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class AlertSystem : IUpdatable
{
  private readonly FarmRobot _farmRobot;
  private readonly BlinkLed _alertLed;
  private readonly Button _emergencyButton;
  private bool _emergencyButtonWasPressed;
  public bool EmergencyStop { get; set; }

  public AlertSystem(FarmRobot farmRobot, BlinkLed led, Button button)
  {
    Console.WriteLine("DEBUG: AlertSystem constructor called");
    _alertLed = led;
    _farmRobot = farmRobot;
    _emergencyButton = button;
    _emergencyButtonWasPressed = _emergencyButton.GetState() == "Pressed";
  }

  public void HandleAlert(string message)
  {
    Robot.PlayNotes("fd");
    _farmRobot.SendMessage("CropBotics/alerts/#", message);
  }

  public void Update()
  {
    bool currentState = _emergencyButton.GetState() == "Pressed";

    // Keep flashing led if emergency stop is active
    if (EmergencyStop)
    {
      _alertLed.Update();
    }

    // Check if emergency button was pressed
    if (currentState && !_emergencyButtonWasPressed)
    {
      EmergencyStop = !EmergencyStop;
      if (EmergencyStop)
      {
        HandleAlert("Emergency Stop was triggered");
        _farmRobot.SetState(State.EMERGENCY_STOP);
        EmergencyStop = true;
      }
      else if (!EmergencyStop)
      {
        HandleAlert("Emergency Stop was released");
        _farmRobot.SetState(State.PAUSED);
        EmergencyStop = false;
      }
    }
    _emergencyButtonWasPressed = currentState;
  }
}
