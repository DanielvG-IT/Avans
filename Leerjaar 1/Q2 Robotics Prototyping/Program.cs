using Avans.StatisticalRobot;

namespace MasterRobot
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var statusLed = new Led(5);
      var startButton = new Button(6);
      // var afstandSensorRight = new Ultrasonic(6);
      // var afstandSensorLeft = new Ultrasonic(6);
      // var lineSensorRight = new InfraredReflective(6);
      // var lineSensorLeft = new InfraredReflective(6);
      // var colorSensor = new colorSensor();

      string storedButtonState = "Unknown";
      bool motorsOn = false;

      while (true)
      {
        var currentButtonStatus = startButton.GetState();

        if (currentButtonStatus != storedButtonState)
        {
          if (currentButtonStatus == "Released")
          {
            if (motorsOn == false)
            {
              statusLed.SetOn();
              Robot.Motors(150, 150);
              motorsOn = true;
            }
            else if (motorsOn == true)
            {
              statusLed.SetOff();
              Robot.Motors(0, 0);
              motorsOn = false;
            }
          }
          storedButtonState = currentButtonStatus;
        }
      }
    }
  }
}