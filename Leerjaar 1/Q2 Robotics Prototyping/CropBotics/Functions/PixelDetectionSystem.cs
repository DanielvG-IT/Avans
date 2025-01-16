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
  public bool nextPixel = true;
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
    currentPixel = 0;
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

    if (distance <= 5 && nextPixel)
    {
      nextPixel = false; // Not detect the same pixel twice
      _farmrobot.SetSpeed(0.1D);

      currentPixel += 1;
      var currentColour = CalculateColour();

      Console.WriteLine($"DEBUG: Pixel {currentPixel}: Distance: {distance}, Colour: {currentColour}");
      _farmrobot.SendMessage("CropBotics/sensor/pixelDistance", $"{distance}");
      _farmrobot.SendMessage($"CropBotics/pixel/{currentPixel}", $"{currentColour}");
    }
    else if (distance > 10 && !nextPixel)
    {
      nextPixel = true;
    }
  }

  private Colour CalculateColour()
  {
    _colourSensor.GetRawData(out ushort red, out ushort blue, out ushort green, out ushort clear);

    const float LowBrightnessThreshold = 0.05f;
    const float LowSaturationThreshold = 0.1f;

    float r = red / 65535f;
    float g = green / 65535f;
    float b = blue / 65535f;

    float maxC = Math.Max(r, Math.Max(g, b));
    float minC = Math.Min(r, Math.Min(g, b));
    float delta = maxC - minC;

    if (maxC == 0f)
      return Colour.Unknown;

    float hue = 0f;
    if (delta > 0f)
    {
      if (maxC == r) hue = 60f * (((g - b) / delta) % 6f);
      else if (maxC == g) hue = 60f * (((b - r) / delta) + 2f);
      else hue = 60f * (((r - g) / delta) + 4f);

      if (hue < 0f) hue += 360f;
    }

    float saturation = (delta / maxC);

    if (maxC < LowBrightnessThreshold && saturation < LowSaturationThreshold)
      return Colour.Unknown;

    if (hue >= 0f && hue < 30f) return Colour.Red;
    if (hue >= 30f && hue < 90f) return Colour.Yellow;
    if (hue >= 90f && hue < 150f) return Colour.Green;
    if (hue >= 150f && hue < 210f) return Colour.Cyan;
    if (hue >= 210f && hue < 270f) return Colour.Blue;
    if (hue >= 270f && hue < 330f) return Colour.Magenta;

    return Colour.Unknown; // Fallback to Unknown if hue doesn't match.
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