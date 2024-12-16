using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class ColourDetectionSystem : IUpdatable
{
  const string colourSensorAddress = "0u0n0hn2";

  // TODO Implement ColourSensor

  public ColourDetectionSystem()
  {
    // Initializing hardware
    // colourSensor = new(colourSensorAddress);
  }

  // private ColourSensor colourSensor;

  public void Update()
  {
    // Update state
    // var colour = colourSensor.GetColour();
  }
}
