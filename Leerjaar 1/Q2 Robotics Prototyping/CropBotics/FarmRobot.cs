using CropBotics.Data;
using CropBotics.Models;
using CropBotics.Functions;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics;

public class FarmRobot : IInitializable, IUpdatable, IWaitable
{
  public FarmRobot()
  {
    // Initializing hardware
    AlertLed = new(AlertLedPin);
    emergencyStopButton = new(emergencyStopButtonPin);

    // Initializing systems
    alertSystem = new(this, AlertLed, emergencyStopButton);
    commsSystem = new();
    driveSystem = new();
    obstacleDetectionSystem = new();
  }

  // Local vars for defining hardware 
  const int AlertLedPin = 5;
  const int emergencyStopButtonPin = 6;

  // Local vars for including functions
  private DriveSystem driveSystem;
  private CommsSystem commsSystem;
  private AlertSystem alertSystem;
  private ObstacleDetectionSystem obstacleDetectionSystem;
  Led AlertLed;
  Button emergencyStopButton;

  // TODO Implement state machine
  public State CurrentState { get; private set; } = State.INIT;

  public void SetState(State State)
  {
    CurrentState = State;
  }


  public async Task Init()
  {
    // Initialize state
    this.SetState(State.INIT);

    //Logging
    Console.WriteLine($"CropBotics started at {DateTime.Now}");
    Robot.PlayNotes("g>g");

    // Initialize systems
    await commsSystem.Init();
    await driveSystem.Init();
    await obstacleDetectionSystem.Init();

    // Update state
    this.SetState(State.READY);
  }

  public async void Update()
  {
    alertSystem.Update();
    commsSystem.Update();
    driveSystem.Update();
    obstacleDetectionSystem.Update();
  }

  public void Wait()
  {
    Thread.Sleep(200);
  }
}