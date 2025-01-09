using CropBotics.Models;
using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;
public class PixelDetectionSystem : IUpdatable
{
  private RGBSensor _colourSensor;
  private FarmRobot _farmrobot;
  private const byte colourSensorAddress = 0x29;


  public PixelDetectionSystem(FarmRobot farmrobot)
  {
    Console.WriteLine("DEBUG: PixelDetectionSystem constructor called");
    _colourSensor = new(colourSensorAddress);
    _farmrobot = farmrobot;
    _colourSensor.Enable();
    _colourSensor.Begin();
  }


  public void Update()
  {
    // _colourSensor.GetRawData(out ushort red, out ushort blue, out ushort green, out ushort clear);
  }

}