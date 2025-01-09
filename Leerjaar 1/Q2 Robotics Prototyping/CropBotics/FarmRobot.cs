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
    driveSystem = new();
    pixelDetectionSystem = new(this);
    obstacleDetectionSystem = new();

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
    lineFollowingSystem.Update();
    colourDetectionSystem.Update();
    obstacleDetectionSystem.Update();
    HandleObsacle();
  }

  public void Wait()
  {
    Thread.Sleep(200);
  }


  // Secundary functions for passing between systems
  public async void SendMessage(string topic, string message)
  {
    await commsSystem.SendMessage(topic, message);
  }

  public void HandleMessage(SimpleMqttMessage message)
  {
    Console.WriteLine($"Handling message: {message.Message}");
    if (alertSystem.EmergencyStop)
    {
      Console.WriteLine("ERROR: Emergency stop is active, ignoring message");
      return;
    }

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
          driveSystem.TargetSpeed = 0.5;
          break;
        }
      case "backward":
        {
          driveSystem.TargetSpeed = -0.5;
          break;
        }
      case "left":
        {
          // TODO Implement turning left via MQTT
          break;
        }
      case "right":
        {
          // TODO Implement turning right via MQTT
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

    // Determine the target speed based on the distance to the nearest obstacle
    if (distance >= 5 && distance < 15)
    {
      driveSystem.TargetSpeed = 0.1;
    }
    else if (distance >= 15 && distance < 40)
    {
      driveSystem.TargetSpeed = 0.2;
    }
    else if (distance >= 40)
    {
      driveSystem.TargetSpeed = 0.4;
    }

    Console.WriteLine($"DEBUG: Target speed {driveSystem.TargetSpeed}");
  }

}