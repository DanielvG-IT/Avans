using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;
public class PixelDetectionSystem : IUpdatable, IInitializable
{
  private const int pixelDetectorUltrasoonPin = 18;
  private const byte colourSensorAddress = 0x29;
  private readonly RGBSensor _colourSensor;
  private readonly Ultrasonic _ultrasonic;
  private readonly FarmRobot _farmrobot;
  private bool firstMesuarement = true;
  public bool nextPixel = true;
  private int currentPixel;
  public RGBSensor.Gain CurrentGain { get; private set; } = RGBSensor.Gain.GAIN_1X;

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
    _colourSensor.GetRawData(out ushort r, out ushort g, out ushort b, out ushort c);

    var colourFound = CalculateColour(r, g, b, c);
    var distance = _ultrasonic.GetUltrasoneDistance();

    // Pixel detected
    if (distance <= 5 && nextPixel && colourFound != "")
    {
      nextPixel = false;

      try
      {
        Console.WriteLine($"DEBUG: {colourFound} detected on row {currentPixel}!");
        _farmrobot.SendMessage("CropBotics/sensor/pixelDistance", $"{distance}");
        _farmrobot.SendMessage($"CropBotics/pixel/{currentPixel}", $"{colourFound}");

      }
      catch (Exception ex)
      {
        Console.WriteLine($"ERROR: Failed to send messages - {ex.Message}");
      }

      currentPixel++;
    }
    else if (!nextPixel)
    {
      // Reset the nextPixel flag
      if (distance > 15)
      {
        nextPixel = true;
        Console.WriteLine($"DEBUG: Waiting for new color...");
      }
    }
  }

  private static string CalculateColour(ushort r, ushort g, ushort b, ushort c)
  {
    Console.WriteLine($"DEBUG: Raw colour values - R:{r} G:{g} B:{b} C:{c}");

    double sumRGB = r + g + b;

    if (sumRGB <= 0)
    {
      return "";
    }
    else if (r >= sumRGB * 0.4)
    {
      return "red";
    }
    else if (g >= sumRGB * 0.4)
    {
      return "green";
    }
    else if (b >= sumRGB * 0.4)
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