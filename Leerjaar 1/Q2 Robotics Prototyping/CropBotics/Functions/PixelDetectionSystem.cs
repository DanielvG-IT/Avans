using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;
public class PixelDetectionSystem : IUpdatable, IInitializable
{
  private const int pixelDetectorUltrasoonPin = 18;
  private const byte colourSensorAddress = 0x29;
  private RGBSensor _colourSensor;
  private Ultrasonic _ultrasonic;
  private FarmRobot _farmrobot;
  private int currentPixel;
  private int distance;
  public RGBSensor.Gain CurrentGain { get; private set; } = RGBSensor.Gain.GAIN_1X;
  public RGBSensor.IntegrationTime CurrentIntegrationTime { get; private set; } = RGBSensor.IntegrationTime.INTEGRATION_TIME_154MS;

  public PixelDetectionSystem(FarmRobot farmrobot)
  {
    Console.WriteLine("DEBUG: PixelDetectionSystem constructor called");
    _colourSensor = new(colourSensorAddress);
    _ultrasonic = new(pixelDetectorUltrasoonPin);
    _farmrobot = farmrobot;
  }

  public Task Init()
  {
    // Initialize the colour sensor
    _colourSensor.Enable();
    _colourSensor.Begin();
    _colourSensor.SetGain(CurrentGain);
    _colourSensor.SetIntegrationTime(CurrentIntegrationTime);
    return Task.CompletedTask;
  }

  public void Update()
  {
    distance = _ultrasonic.GetUltrasoneDistance();

    if (distance > 5)
    {
      _farmrobot.SendMessage("CropBotics/sensor/pixelDistance", $"{distance}");

      if (_farmrobot.GetSpeed() <= 0.1D)
      {
        _farmrobot.SetSpeed(0.4D);
      }

      return;
    }

    _farmrobot.SetSpeed(0.1D);

    currentPixel += 1;

    _colourSensor.GetRawData(out ushort red, out ushort blue, out ushort green, out ushort clear);
    var currentColour = CalculateColour(red, blue, green, clear);

    _farmrobot.SendMessage("CropBotics/sensor/pixelDistance", $"{distance}");
    _farmrobot.SendMessage($"CropBotics/pixel/{currentPixel}", $"{currentColour}");

  }

  private Colour CalculateColour(ushort Cred, ushort Cblue, ushort Cgreen, ushort Cclear)
  {
    // TODO Implement cleaner colour calculation
    float sumRGB = Cred + Cgreen + Cblue;

    if (sumRGB == 0)
    {
      return Colour.Unknown;
    }

    float redRatio = Cred / sumRGB;
    float greenRatio = Cgreen / sumRGB;
    float blueRatio = Cblue / sumRGB;

    if (redRatio > 0.45 && greenRatio < 0.35 && blueRatio < 0.35)
    {
      Console.WriteLine($"Pixel {currentPixel}: Red detected");
      return Colour.Red;
    }
    else if (greenRatio > 0.45 && redRatio < 0.35 && blueRatio < 0.35)
    {
      Console.WriteLine($"Pixel {currentPixel}: Green detected");
      return Colour.Green;
    }
    else if (blueRatio > 0.45 && redRatio < 0.35 && greenRatio < 0.35)
    {
      Console.WriteLine($"Pixel {currentPixel}: Blue detected");
      return Colour.Blue;
    }
    else if (redRatio > 0.3 && greenRatio > 0.3 && blueRatio < 0.2)
    {
      Console.WriteLine($"Pixel {currentPixel}: Yellow detected");
      return Colour.Yellow;
    }
    else
    {
      Console.WriteLine($"Pixel {currentPixel}: Unknown color");
      return Colour.Unknown;
    }
  }

  public void SetGain(RGBSensor.Gain gain)
  {
    _colourSensor.SetGain(gain);
    CurrentGain = gain;
  }

  public void SetIntegrationTime(RGBSensor.IntegrationTime integrationTime)
  {
    _colourSensor.SetIntegrationTime(integrationTime);
    CurrentIntegrationTime = integrationTime;
  }

}