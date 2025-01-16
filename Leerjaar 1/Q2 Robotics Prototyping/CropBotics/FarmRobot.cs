using CropBotics.Data;
using CropBotics.Models;
using CropBotics.Functions;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics;

public class FarmRobot : IInitializable, IUpdatable, IWaitable, IMessageHandler
{
  public FarmRobot()
  {
    // Initializing hardware
    AlertLed = new(alertLedPin, AlertBlinkMilSec);
    emergencyStopButton = new(emergencyStopButtonPin);

    // Initializing systems
    alertSystem = new(this, AlertLed, emergencyStopButton);
    commsSystem = new(this);
    driveSystem = new(this);
    pixelDetectionSystem = new(this);
    obstacleDetectionSystem = new(this);

  }

  // Local vars for defining hardware 
  const int alertLedPin = 5;
  const int AlertBlinkMilSec = 5000;
  const int emergencyStopButtonPin = 6;
  private bool stopped = false;


  // Local vars for including functions
  private readonly AlertSystem alertSystem;
  private readonly CommsSystem commsSystem;
  private readonly DriveSystem driveSystem;
  private readonly PixelDetectionSystem pixelDetectionSystem;
  private readonly ObstacleDetectionSystem obstacleDetectionSystem;
  BlinkLed AlertLed;
  Button emergencyStopButton;

  public State CurrentState { get; private set; } = State.INIT;

  public void SetState(State inputState)
  {
    CurrentState = inputState;
  }


  public async Task Init()
  {
    SetState(State.INIT);
    Console.WriteLine($"CropBotics started at {DateTime.Now}");
    Robot.PlayNotes("g>g");
    await commsSystem.Init();
    SetState(State.READY);
  }

  public void Update()
  {
    alertSystem.Update();
    driveSystem.Update();
    pixelDetectionSystem.Update();
    obstacleDetectionSystem.Update();
    HandleObsacle();
  }

  public void Wait()
  {
    Thread.Sleep(500);
    Robot.Wait(500);
  }


  // Secundary functions for passing between systems
  public static string CheckBatteryLevel()
  {
    var batterymV = Robot.ReadBatteryMillivolts();
    var batteryPercentage = batterymV / 9000 * 100;

    return $"{batteryPercentage}";
  }

  public async void SendMessage(string topic, string message)
  {
    await commsSystem.SendMessage(topic, message);
  }

  public double GetSpeed()
  {
    return driveSystem.CurrentSpeed;
  }

  public void SetSpeed(double speed)
  {
    driveSystem.TargetSpeed = speed;
  }

  public void HandleMessage(SimpleMqttMessage message)
  {
    Console.WriteLine($"""DEBUG: Handling message "{message.Message}" on topic "{message.Topic}"!""");

    switch (message.Topic)
    {
      case "CropBotics/commands":
        {
          switch (message.Message)
          {
            case "emergency_stop":
              {
                driveSystem.EmergencyStop();
                if (!alertSystem.EmergencyStop)
                {
                  alertSystem.HandleAlert("Emergency stop was triggered");
                  alertSystem.EmergencyStop = true;
                }
                else if (alertSystem.EmergencyStop)
                {
                  alertSystem.HandleAlert("Emergency stop was released");
                  alertSystem.EmergencyStop = false;
                }
                break;
              }
            case "forward":
              {
                if (alertSystem.EmergencyStop)
                {
                  Console.WriteLine("ERROR: Emergency stop is active, ignoring message");
                  return;
                }
                driveSystem.TargetSpeed = 0.5;
                break;
              }
            case "backward":
              {
                if (alertSystem.EmergencyStop)
                {
                  Console.WriteLine("ERROR: Emergency stop is active, ignoring message");
                  return;
                }
                driveSystem.TargetSpeed = -0.5;
                break;
              }
            case "stop":
              {
                driveSystem.TargetSpeed = 0.0;
                break;
              }
            default:
              {
                Console.WriteLine("ERROR: Unknown command received");
                break;
              }
          }
        }
        break;
      case "CropBotics/request":
        {
          switch (message.Message)
          {
            case "all":
              {
                SendMessage("CropBotics/status/status", "Active");
                SendMessage("CropBotics/status/battery", $"{CheckBatteryLevel()}");
                SendMessage("CropBotics/request/motorsEnabled", driveSystem.MotorsEnabled ? "true" : "false");
                SendMessage("CropBotics/request/colourGain", pixelDetectionSystem.CurrentGain.ToString());
                break;
              }
            default:
              {
                Console.WriteLine("ERROR: Unknown request received");
                break;
              }
          }
          break;
        }
      case "CropBotics/settings/colourGain":
        {
          switch (message.Message)
          {
            case "1x":
              {
                pixelDetectionSystem.SetGain(RGBSensor.Gain.GAIN_1X);
                break;
              }
            case "4x":
              {
                pixelDetectionSystem.SetGain(RGBSensor.Gain.GAIN_4X);
                break;
              }
            case "16x":
              {
                pixelDetectionSystem.SetGain(RGBSensor.Gain.GAIN_16X);
                break;
              }
            case "60x":
              {
                pixelDetectionSystem.SetGain(RGBSensor.Gain.GAIN_60X);
                break;
              }
            default:
              {
                Console.WriteLine("ERROR: Unknown colourGain setting received");
                break;
              }
          }
          break;
        }
      case "CropBotics/settings/motorsEnabled":
        {
          driveSystem.MotorsEnabled = message.Message == "true";
          break;
        }
      default:
        {
          Console.WriteLine("ERROR: Unknown topic received");
          break;
        }
    }
  }

  public void HandleObsacle()
  {
    int distance = obstacleDetectionSystem.ObstacleDistance;
    Console.WriteLine($"DEBUG: Distance {distance} cm");

    if ((distance < 3 && !stopped) || alertSystem.EmergencyStop)
    {
      stopped = true;
      driveSystem.EmergencyStop();
      alertSystem.HandleAlert($"Emergency stop\nDistance {distance} cm");
      alertSystem.EmergencyStop = true;
    }
    else if (distance >= 5 && stopped)
    {
      stopped = false;
      alertSystem.EmergencyStop = false;
    }

    if (distance >= 5 && distance < 10)
    {
      driveSystem.TargetSpeed = 0.2;
    }
    else if (distance >= 10 && distance < 15)
    {
      driveSystem.TargetSpeed = 0.4;
    }

    Console.WriteLine($"DEBUG: Target speed {driveSystem.TargetSpeed}");
  }

}