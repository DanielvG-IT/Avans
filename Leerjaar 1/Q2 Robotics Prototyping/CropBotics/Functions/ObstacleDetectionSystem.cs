using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class ObstacleDetectionSystem : IUpdatable
{
  const int UltrasonicPinNumber = 16;
  const int ScanIntervalMilliseconds = 500;
  private Ultrasonic distanceSensor;
  private PeriodTimer scanIntervalTimer;
  public int ObstacleDistance { get; private set; }

  public ObstacleDetectionSystem()
  {
    Console.WriteLine("DEBUG: ObstacleDetectionSystem constructor called");
    distanceSensor = new Ultrasonic(UltrasonicPinNumber);
    scanIntervalTimer = new PeriodTimer(ScanIntervalMilliseconds);
  }

  public void Update()
  {
    if (scanIntervalTimer.Check())
    {
      Robot.LEDs(0, 0, 255);
      ObstacleDistance = distanceSensor.GetUltrasoneDistance();
      Robot.LEDs(0, 0, 0);
    }
  }

}