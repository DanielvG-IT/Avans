using CropBotics.Models;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class AlertSystem : IUpdatable
{
  private FarmRobot _farmRobot;
  private BlinkLed _alertLed;
  private Button _emergencyButton;
  private bool _emergencyButtonWasPressed;
  public bool EmergencyStop { get; set; }


  public AlertSystem(FarmRobot farmRobot, BlinkLed led, Button button)
  {
    _farmRobot = farmRobot;
    _emergencyButton = button;
    _emergencyButtonWasPressed = _emergencyButton.GetState().Equals("Pressed");
    _alertLed = led;
  }


  public void HandleAlert()
  {
    Robot.PlayNotes("fd");
  }

  public void EmergencyAlert()
  {
    Robot.PlayNotes("f");
    Robot.Wait(100);
    Robot.PlayNotes("f");
    Robot.Wait(100);
    Robot.PlayNotes("f");
  }

  public void Update()
  {
    var currentEmergencyButtonState = _emergencyButton.GetState() == "Pressed" ? true : false;

    if (currentEmergencyButtonState && !_emergencyButtonWasPressed)
    {
      EmergencyStop = true;
      EmergencyAlert();
    }
    else if (!currentEmergencyButtonState && _emergencyButtonWasPressed)
    {
      EmergencyStop = false;
    }

    _emergencyButtonWasPressed = currentEmergencyButtonState;

    if (EmergencyStop)
    {
      // USE METHODES WITH Avans.StatisticalRobot.Timer to blink every 5 seconds
    }
  }
}
