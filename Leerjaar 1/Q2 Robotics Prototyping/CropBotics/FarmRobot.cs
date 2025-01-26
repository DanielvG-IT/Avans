using CropBotics.Data;
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
  public readonly PeriodTimer timer;
  BlinkLed AlertLed;
  Button emergencyStopButton;
  public bool EmergencyStop { get; private set; } = false;

  public FarmRobot()
  {
    // Initializing hardware
    AlertLed = new(alertLedPin, AlertBlinkMilSec);
    emergencyStopButton = new(emergencyStopButtonPin);
    timer = new(5000);

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

    if (timer.Check())
    {
      SendMessage("CropBotics/status/status", "Online");
    }
  }

  public void Wait()
  {
    Thread.Sleep(250);
    Robot.Wait(250);
  }

  // Secundary functions for passing between systems

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

  public void HandleMessage(SimpleMqttMessage Mqtt)
  {
    Console.WriteLine($"""DEBUG: Handling message "{Mqtt.Message}" on topic "{Mqtt.Topic}"!""");

    switch (Mqtt.Topic)
    {
      case "CropBotics/command":
        {
          switch (Mqtt.Message)
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
                if (!alertSystem.EmergencyStop)
                {
                  stopped = false;
                  driveSystem.TargetSpeed = -0.5;
                }
                Console.WriteLine("ERROR: Emergency stop is active, ignoring message");
                break;
              }
            case "Stop":
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
          if (Mqtt.Message == "all")
          {
            SendMessage("CropBotics/request/MotorCalibrationRight", Convert.ToString(driveSystem.CalibrationRight));
            SendMessage("CropBotics/request/MotorCalibrationLeft", Convert.ToString(driveSystem.CalibrationLeft));
            SendMessage("CropBotics/status/emergency_stop", alertSystem.EmergencyStop ? "True" : "False");
            SendMessage("CropBotics/request/MotorsEnabled", driveSystem.MotorsEnabled ? "True" : "False");
            SendMessage("CropBotics/request/colourGain", pixelDetectionSystem.CurrentGain.ToString());
            SendMessage("CropBotics/status/battery", $"{Robot.ReadBatteryMillivolts() / 90}");
          }
        }
        break;
      case "CropBotics/settings/colourGain":
        {
          switch (Mqtt.Message)
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
          Console.WriteLine($"DEBUG: Setting motors enabled to {Mqtt.Message}");
          driveSystem.MotorsEnabled = Mqtt.Message == "True";
          break;
        }
      case "CropBotics/settings/MotorCalibrationLeft":
        {
          Console.WriteLine($"DEBUG: Setting calibration left to {Mqtt.Message}");
          driveSystem.CalibrationLeft = Convert.ToInt16(Mqtt.Message);
          break;
        }
      case "CropBotics/settings/MotorCalibrationRight":
        {
          Console.WriteLine($"DEBUG: Setting calibration right to {Mqtt.Message}");
          driveSystem.CalibrationRight = Convert.ToInt16(Mqtt.Message);
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
    try
    {
      int distance = obstacleDetectionSystem.ObstacleDistance;

      if (distance <= 10 || EmergencyStop)
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
        if (distance > 40)
        {
          targetSpeed = 0.2; // Slow speed
        }
        else if (distance > 20)
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
    catch (OperationCanceledException ex)
    {
      Console.WriteLine($"Operation was canceled: {ex.Message}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Exception thrown: {ex.Message}");
    }
  }

  public void HandleExeption(string message, string? exeptiontype)
  {
    Console.WriteLine($"ERROR: Exeption {exeptiontype} thrown: {message}");
    driveSystem.EmergencyStop();
    alertSystem.EmergencyStop = true;
    SendMessage("CropBotics/status/emergency_stop", "True");
  }

}