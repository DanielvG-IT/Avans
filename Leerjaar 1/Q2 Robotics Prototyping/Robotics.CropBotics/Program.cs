using Avans.StatisticalRobot;

namespace MasterRobot
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var coolLed = new Led(5);
      var coolButton = new Button(6);

      string storedButtonState = "Unknown";
      bool motorsOn = false;

      while (true)
      {
        var currentButtonStatus = coolButton.GetState();

        if (currentButtonStatus != storedButtonState)
        {
          if (currentButtonStatus == "Released")
          {
            if (motorsOn == false)
            {
              Robot.Motors(150, 150);
              motorsOn = true;
            }
            else if (motorsOn == true)
            {
              Robot.Motors(0, 0);
              motorsOn = false;
            }
          }
          storedButtonState = currentButtonStatus;
          Robot.Wait(1000);
        }
      }
    }
  }
}