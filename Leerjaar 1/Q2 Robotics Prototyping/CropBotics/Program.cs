using System.Net.Http.Headers;
using Avans.StatisticalRobot;

namespace CropBotics;
public class Program
{
  public static async Task Main(string[] args)
  {
    // Make and Initialize the Robot!
    FarmRobot robot = new();
    await robot.Init();



    while (true)
    {
      /* 
      robot.Update();
      robot.Wait();
      */
      await Task.Delay(200);
    }
  }
}
