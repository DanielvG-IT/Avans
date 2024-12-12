using CropBotics.Models;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class AlertSystem : IUpdatable
{
  private FarmRobot _farmRobot;
  private Led _alertLed;
  private Button _emergencyButton;
  private bool _emergencyButtonWasPressed;
  public bool EmergencyStop { get; set; }


  public AlertSystem(FarmRobot farmRobot, Led led, Button button)
  {
    _farmRobot = farmRobot;
    _emergencyButton = button;
    _emergencyButtonWasPressed = _emergencyButton.GetState().Equals("Pressed");
    _alertLed = led;
  }


  public void PlayAlert()
  {
    _alertLed.SetOn();
    Robot.PlayNotes("fd");
  }

  public void StopAlert()
  {
    _alertLed.SetOff();
    Robot.PlayNotes("f>c");
  }

  public void EmergencyAlert()
  {
    _alertLed.SetOn();
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
      Console.WriteLine("DEBUG: Emergency stop button pressed");
      _emergencyButtonWasPressed = true;
      EmergencyStop = true;
      EmergencyAlert();
      _farmRobot.SetState(State.EMERGENCY_STOP);
    }
    else if (!currentEmergencyButtonState && _emergencyButtonWasPressed)
    {
      _emergencyButtonWasPressed = false;
      EmergencyStop = false;
      StopAlert();
      _farmRobot.SetState(State.READY);
    }
  }
}
