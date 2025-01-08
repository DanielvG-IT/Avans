using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions
{
  public class ObstacleDetectionSystem : IUpdatable, IInitializable
  {

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