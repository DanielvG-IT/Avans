using CropBotics.Data;
using CropBotics.Models;
using CropBotics.Functions;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics;

public class FarmRobot : IInitializable, IUpdatable, IWaitable
{
  // Local vars for defining hardware 
  const int AlertLedPin = 5;
  const int emergencyStopButtonPin = 6;

  // Local vars for including functions
  private DriveSystem driveSystem = new();
  private CommsSystem commsSystem = new();
  private AlertSystem alertSystem = new();
  private ObstacleDetectionSystem obstacleDetectionSystem = new();

  // IInitializing hardware
  private Button emergencyStopButton = new(emergencyStopButtonPin);
  private Led AlertLed = new(AlertLedPin);


  // TODO: Implement state machine
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
    await alertSystem.Init();
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