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
  public bool nextPixel = true;
  private int currentPixel;
  private string currentColour;
  private int distance;
  public RGBSensor.Gain CurrentGain { get; private set; } = RGBSensor.Gain.GAIN_1X;

  public PixelDetectionSystem(FarmRobot farmrobot)
  {
    Console.WriteLine("DEBUG: PixelDetectionSystem constructor called");
    _colourSensor = new(colourSensorAddress);
    _ultrasonic = new(pixelDetectorUltrasoonPin);
    _farmrobot = farmrobot;
    currentPixel = 0;
    currentColour = "Unknown";
    Thread.Sleep(1000);
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
    distance = _ultrasonic.GetUltrasoneDistance();

    if (distance <= 5 && nextPixel)
    {
      nextPixel = false; // Not detect the same pixel twice
      _farmrobot.SetSpeed(0.1D);

      currentPixel += 1;
      _colourSensor.GetRawData(out ushort r, out ushort b, out ushort g, out ushort c);
      currentColour = CalculateColour(r, g, b, c);

      if (currentColour == "Unknown" && currentPixel == 1)
      {
        return;
      }

      Console.WriteLine($"DEBUG: Pixel {currentPixel}: Distance: {distance}, Colour: {currentColour}");
      _farmrobot.SendMessage("CropBotics/sensor/pixelDistance", $"{distance}");
      _farmrobot.SendMessage($"CropBotics/pixel/{currentPixel}", $"{currentColour}");
    }
    else if (distance > 10 && !nextPixel)
    {
      nextPixel = true;
    }
  }

  private static string CalculateColour(ushort r, ushort g, ushort b, ushort c)
  {
    Console.WriteLine($"DEBUG: Raw colour values - R:{r} G:{g} B:{b} C:{c}");

    // Input validation to avoid division by zero
    double sumRGB = r + g + b;
    if (sumRGB <= 0)
    {
      return "Unknown";
    }

    // Normalize RGB values to percentages
    double rPercentage = r / sumRGB * 100;
    double gPercentage = g / sumRGB * 100;
    double bPercentage = b / sumRGB * 100;

    if (bPercentage >= 35)
    {
      return "blue";
    }
    else if (rPercentage >= 35 && rPercentage <= 44 && gPercentage >= 28 && gPercentage <= 37 && bPercentage <= 20)
    {
      return "yellow";
    }
    else if (gPercentage >= 38 && bPercentage <= 24 && rPercentage >= 30)
    {
      return "green";
    }
    else if (rPercentage >= 45)
    {
      return "red";
    }
    return "Unknown";
  }

  public void SetGain(RGBSensor.Gain gain)
  {
    _colourSensor.SetGain(gain);
    CurrentGain = gain;
  }

}