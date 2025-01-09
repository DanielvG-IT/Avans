using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions
{
  public class ObstacleDetectionSystem : IUpdatable, IInitializable
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

    public async Task Init()
    {
      Console.WriteLine("ObstacleDetectionSystem Init called.");
    }

    public void Update()
    {
      Console.WriteLine("ObstacleDetectionSystem Update called.");
    }

  }
}