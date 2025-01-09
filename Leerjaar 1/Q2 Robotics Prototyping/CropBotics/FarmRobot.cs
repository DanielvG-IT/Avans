using CropBotics.Data;
using CropBotics.Models;
using CropBotics.Functions;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics;

public class FarmRobot : IInitializable, IUpdatable, IWaitable, IMessageHandler, IColourHandler
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
    lineFollowingSystem = new();
    colourDetectionSystem = new(this);
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
  private readonly LineFollowingSystem lineFollowingSystem;
  private readonly ColourDetectionSystem colourDetectionSystem;
  private readonly ObstacleDetectionSystem obstacleDetectionSystem;
  BlinkLed AlertLed;
  Button emergencyStopButton;

  // TODO Implement state machine
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

  public void HandleColour(Colour colour)
  {
    Console.WriteLine(colour);
    switch (colour)
    {
      // TODO Implement actions for each colour
      case Colour.Red:
        break;
      case Colour.Green:
        // Healthy
        break;
      case Colour.Blue:
        // Do something
        break;
      case Colour.Yellow:
        // Do something
        break;
      case Colour.White:
        // Do something
        break;
      case Colour.Black:
        // Do something
        break;
      case Colour.Unknown:
        // Do nothing
        break;
    }
  }

  public void HandleMessage(SimpleMqttMessage message)
  {
    Console.WriteLine($"Handling message: {message.Message}");
    switch (message.Message)
    {
      case "emergency_stop":
        {
          driveSystem.EmergencyStop();
          break;
        }
      case "forward":
        {
          driveSystem.TargetSpeed = 0.75;
          break;
        }
      case "backward":
        {
          driveSystem.TargetSpeed = -0.75;
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

  public async void SendMessage(string topic, string message)
  {
    await commsSystem.SendMessage(topic, message);
  }

  public async void HandleObsacle()
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