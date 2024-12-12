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
      // Update the robot, then Wait to prevent the loop from running too fast
      robot.Update();
      robot.Wait();
    }
  }
}
