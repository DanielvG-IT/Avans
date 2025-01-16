using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class ObstacleDetectionSystem : IUpdatable
{
  private readonly FarmRobot _farmrobot;
  const int UltrasonicPinNumber = 16;
  const int ScanIntervalMilliseconds = 500;
  private Ultrasonic distanceSensor;
  private PeriodTimer scanIntervalTimer;
  public int ObstacleDistance { get; private set; }

  public ObstacleDetectionSystem(FarmRobot farmrobot)
  {
    Console.WriteLine("DEBUG: ObstacleDetectionSystem constructor called");
    distanceSensor = new Ultrasonic(UltrasonicPinNumber);
    scanIntervalTimer = new PeriodTimer(ScanIntervalMilliseconds);
    _farmrobot = farmrobot;
  }

  public void Update()
  {
    if (scanIntervalTimer.Check())
    {
      Robot.LEDs(0, 0, 255);
      ObstacleDistance = distanceSensor.GetUltrasoneDistance();
      _farmrobot.SendMessage("CropBotics/sensor/obstacleDistance", $"{ObstacleDistance}");
      Robot.LEDs(0, 0, 0);
    }
  }

}