using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class ColourDetectionSystem : IUpdatable
{
  private const byte colourSensorAddress = 0x29;

  private RGBSensor _colourSensor;
  private ushort redValue;
  private ushort greenValue;
  private ushort blueValue;
  private IColourHandler _colourHandler;

  public ColourDetectionSystem(IColourHandler colourHandler)
  {
    // Initializing hardware
    _colourSensor = new(colourSensorAddress);
    _colourHandler = colourHandler;
    _colourSensor.Enable();
    _colourSensor.Begin();
  }

  public void Update()
  {
    _colourHandler.HandleColour(GetColour());
  }

  public Colour GetColour()
  {
    GetRGBData();

    // TODO Improve logic to determine colour
    if (redValue > 1000 && greenValue < 500 && blueValue < 500)
    {
      return Colour.Red;
    }
    else if (redValue < 500 && greenValue > 1000 && blueValue < 500)
    {
      return Colour.Green;
    }
    else if (redValue < 500 && greenValue < 500 && blueValue > 1000)
    {
      return Colour.Blue;
    }
    else if (redValue > 1000 && greenValue > 1000 && blueValue < 500)
    {
      return Colour.Yellow;
    }
    else if (redValue > 1000 && greenValue > 1000 && blueValue > 1000)
    {
      return Colour.White;
    }
    else if (redValue < 500 && greenValue < 500 && blueValue < 500)
    {
      return Colour.Black;
    }
    else
    {
      return Colour.Unknown;
    }
  }

  private void GetRGBData()
  {
    _colourSensor.GetRawData(out ushort red, out ushort blue, out ushort green, out ushort clear);
    redValue = red;
    blueValue = blue;
    greenValue = green;
  }
}
