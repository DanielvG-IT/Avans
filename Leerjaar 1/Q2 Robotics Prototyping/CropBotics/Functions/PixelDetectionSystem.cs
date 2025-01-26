using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;
public class PixelDetectionSystem : IUpdatable, IInitializable
{
  private const int pixelDetectorUltrasoonPin = 18;
  private const byte colourSensorAddress = 0x29;
  public RGBSensor.Gain CurrentGain { get; private set; } = RGBSensor.Gain.GAIN_1X;
  private readonly RGBSensor _colourSensor;
  private readonly Ultrasonic _ultrasonic;
  private readonly FarmRobot _farmrobot;
  public bool firstReading = true;
  public bool nextPixel = true;
  private int currentPixel;

  public PixelDetectionSystem(FarmRobot farmrobot)
  {
    Console.WriteLine("DEBUG: PixelDetectionSystem constructor called");
    _colourSensor = new(colourSensorAddress);
    _ultrasonic = new(pixelDetectorUltrasoonPin);
    _farmrobot = farmrobot;
    currentPixel = 0;
  }

  public Task Init()
  {
    // Initialize the colour sensor
    _colourSensor.Enable();
    _colourSensor.SetGain(CurrentGain);
    return Task.CompletedTask;
  }

  public void Update()
  {

    var distance = _ultrasonic.GetUltrasoneDistance();

    // Dismiss the first reading (false measurement)
    if (distance <= 5 && nextPixel && firstReading)
    {
      firstReading = false;
    }

    // Pixel detected and colour found
    else if (distance <= 5 && nextPixel && !firstReading)
    {
      currentPixel++;
      nextPixel = false;
      // var colourFound = "unknown";

      _colourSensor.GetRawData(out ushort r, out ushort g, out ushort b, out ushort c);
      var colourFound = CalculateColour(r, g, b, c);

      Console.WriteLine($"DEBUG: RGB values: R={r}, G={g}, B={b}, C={c} for pixel {currentPixel}");
      Console.WriteLine($"DEBUG: Colour {colourFound} detected on row {currentPixel}!");

      _farmrobot.SendMessage("CropBotics/sensor/pixelDistance", $"{distance}");
      _farmrobot.SendMessage($"CropBotics/pixel/{currentPixel}", $"{colourFound}");
    }

    // Reset the nextPixel flag
    else if (!nextPixel && distance > 15)
    {
      nextPixel = true;
      Console.WriteLine($"DEBUG: Waiting for new color...");
    }
  }

  private static string CalculateColour(ushort r, ushort g, ushort b, ushort c)
  {
    double normalizedR = (double)r / c;
    double normalizedG = (double)g / c;
    double normalizedB = (double)b / c;
    double maxValue = Math.Max(normalizedR, Math.Max(normalizedG, normalizedB));

    if (maxValue == normalizedR)
    {
      return "red";
    }
    else if (maxValue == normalizedG)
    {
      return "green";
    }
    else if (maxValue == normalizedB)
    {
      return "blue";
    }
    else
    {
      return "";
    }

  }

  public void SetGain(RGBSensor.Gain gain)
  {
    _colourSensor.SetGain(gain);
    CurrentGain = gain;
  }

}