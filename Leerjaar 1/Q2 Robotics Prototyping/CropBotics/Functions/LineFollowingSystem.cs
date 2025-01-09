using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class LineFollowingSystem : IUpdatable
{
  const int lineSensorPin = 5;
  public LineFollowingSystem()
  {
    // Initializing hardware
    _lineSensor = new InfraredReflective(lineSensorPin);
  }

  private readonly InfraredReflective _lineSensor;

  public void Update()
  {
    int state = _lineSensor.Watch();
    if (state == 0)
    {
      // TODO Impelemt actions when Line is detected
    }
    else if (state == 1)
    {
      // TODO Impelemt actions when Line is Lost
    }
  }
}