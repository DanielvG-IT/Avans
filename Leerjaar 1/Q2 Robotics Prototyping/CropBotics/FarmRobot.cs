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
    AlertLed = new(AlertLedPin);
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
  const int emergencyStopButtonPin = 6;

  // Local vars for including functions
  private readonly AlertSystem alertSystem;
  private readonly CommsSystem commsSystem;
  private readonly DriveSystem driveSystem;
  private readonly LineFollowingSystem lineFollowingSystem;
  private readonly ColourDetectionSystem colourDetectionSystem;
  private readonly ObstacleDetectionSystem obstacleDetectionSystem;
  Led AlertLed;
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
    await driveSystem.Init();
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
  }
}