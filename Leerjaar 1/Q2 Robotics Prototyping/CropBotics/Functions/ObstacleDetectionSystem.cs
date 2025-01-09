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
    // Don't measure at every call because it blocks all processing
    // during the measurement; so use a timer that times out periodically
    if (scanIntervalTimer.Check())
    {
      Robot.LEDs(0, 0, 255); // Flash the green LED on the Romi board
      ObstacleDistance = distanceSensor.GetUltrasoneDistance();
      Robot.LEDs(0, 0, 0);
    }
  }

}