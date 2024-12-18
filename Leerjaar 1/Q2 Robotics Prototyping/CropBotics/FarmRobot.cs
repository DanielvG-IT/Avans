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
    AlertLed = new(AlertLedPin, AlertBlinkMilSec);
    emergencyStopButton = new(emergencyStopButtonPin);

    // Initializing systems
    alertSystem = new(this, AlertLed, emergencyStopButton);
    commsSystem = new(this);
    driveSystem = new();
    lineFollowingSystem = new();
    colourDetectionSystem = new();
    obstacleDetectionSystem = new();
  }

  // Local vars for defining hardware 
  const int AlertLedPin = 5;
  const int AlertBlinkMilSec = 5000;
  const int emergencyStopButtonPin = 6;

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
    // Initialize state
    SetState(State.INIT);

    //Logging
    Console.WriteLine($"CropBotics started at {DateTime.Now}");
    Robot.PlayNotes("g>g");

    // Initialize systems
    await commsSystem.Init();
    await obstacleDetectionSystem.Init();

    // Update state
    SetState(State.READY);
  }

  public void Update()
  {
    alertSystem.Update();
    driveSystem.Update();
    lineFollowingSystem.Update();
    colourDetectionSystem.Update();
    obstacleDetectionSystem.Update();
  }

  public void Wait()
  {
    Thread.Sleep(200);
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
}