using CropBotics.Data;
using CropBotics.Models;
using CropBotics.Functions;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics;

public class FarmRobot : IInitializable, IUpdatable, IWaitable, IMessageHandler
{
  // Local vars for defining hardware 
  const int alertLedPin = 5;
  const int AlertBlinkMilSec = 1000;
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
  public bool EmergencyStop { get; private set; } = false;

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

  public async Task Init()
  {
    Console.WriteLine($"CropBotics started at {DateTime.Now}");
    Robot.PlayNotes("g>g");
    await commsSystem.Init();
  }

  public void Update()
  {
    alertSystem.Update();
    driveSystem.Update();
    pixelDetectionSystem.Update();
    obstacleDetectionSystem.Update();

    HandleObsacle();
    EmergencyStop = alertSystem.EmergencyStop;
  }

  public void Wait()
  {
    Thread.Sleep(250);
    Robot.Wait(250);
  }

  // Secundary functions for passing between systems
  public static int CheckBatteryLevel()
  {
    var batterymV = Robot.ReadBatteryMillivolts();
    var batteryPercentage = batterymV / 9000 * 100;

    return batteryPercentage;
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
                  alertSystem.EmergencyStop = true;
                }
                else if (alertSystem.EmergencyStop)
                {
                  alertSystem.EmergencyStop = false;
                }
                break;
              }
            case "forward":
              {
                if (!alertSystem.EmergencyStop)
                {
                  stopped = false;
                  driveSystem.TargetSpeed = 0.2;
                }
                Console.WriteLine("ERROR: Emergency stop is active, ignoring message");
                break;
              }
            case "backward":
              {
                if (alertSystem.EmergencyStop)
                {
                  stopped = false;
                  driveSystem.TargetSpeed = -0.5;
                }
                Console.WriteLine("ERROR: Emergency stop is active, ignoring message");
                break;
              }
            case "stop":
              {
                driveSystem.TargetSpeed = 0.0;
                stopped = true;
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
          if (message.Message == "all")
          {
            SendMessage("CropBotics/status/status", "Active");
            SendMessage("CropBotics/status/battery", $"{CheckBatteryLevel()}");
            SendMessage("CropBotics/status/emergency_stop", alertSystem.EmergencyStop ? "True" : "False");
            SendMessage("CropBotics/request/MotorsEnabled", driveSystem.MotorsEnabled ? "True" : "False");
            SendMessage("CropBotics/request/colourGain", pixelDetectionSystem.CurrentGain.ToString());
          }
        }
        break;
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
      case "CropBotics/settings/MotorsEnabled":
        {
          driveSystem.MotorsEnabled = message.Message == "True";
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

    if (distance <= 5 || EmergencyStop)
    {
      if (!stopped)
      {
        stopped = true;
        driveSystem.EmergencyStop();
        alertSystem.EmergencyStop = true;
        SendMessage("CropBotics/status/emergency_stop", "True");
        return;
      }
    }

    // Dynamic speed control based on distance
    if (!stopped && !EmergencyStop && !pixelDetectionSystem.nextPixel)
    {
      double targetSpeed = 0.0;
      if (distance > 30)
      {
        targetSpeed = 0.3; // Medium speed
      }
      else if (distance > 20)
      {
        targetSpeed = 0.2; // Slow speed
      }
      else if (distance > 10)
      {
        targetSpeed = 0.1; // Very slow speed
      }

      // Only update speed if it needs to change
      if (Math.Abs(driveSystem.TargetSpeed - targetSpeed) > 0.01)
      {
        driveSystem.TargetSpeed = targetSpeed;
        Console.WriteLine($"DEBUG: Adjusting speed to {targetSpeed} based on distance {distance}cm");
      }
    }
  }

}