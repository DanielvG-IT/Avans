using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class DriveSystem : IUpdatable
{
  private readonly FarmRobot _farmrobot;

  public DriveSystem(FarmRobot farmrobot)
  {
    _farmrobot = farmrobot;
  }

  private short _calibrationLeft = 1;
  public short CalibrationLeft
  {
    get { return _calibrationLeft; }
    set { if (value < 25 && value > -25) _calibrationLeft = value; }
  }
  private short _calibrationRight = 0;
  public short CalibrationRight
  {
    get { return _calibrationRight; }
    set { if (value < 25 && value > -25) _calibrationRight = value; }
  }

  private double _speedIncrease = 0.1;
  public double SpeedIncrease
  {
    get { return _speedIncrease; }
    set { if (value > 0.0 && value <= 1.0) _speedIncrease = value; }
  }

  private double _targetSpeed = 0.0;
  public double TargetSpeed
  {
    get { return _targetSpeed; }
    set { if (value > 0.0 && value <= 1.0) _targetSpeed = value; }
  }

  private double _currentSpeed = 0.0;
  public double CurrentSpeed { get { return _currentSpeed; } }
  public bool MotorsEnabled { get; set; } = true;

  private short CalculateRobotSpeed(double speed)
  {
    return (short)Math.Round(speed * 300);
  }

  private void SetMotorSpeed()
  {
    if (MotorsEnabled)
    {
      Robot.Motors(
        (short)(CalculateRobotSpeed(_currentSpeed) + _calibrationLeft),
        (short)(CalculateRobotSpeed(_currentSpeed) + _calibrationRight)
      );
    }
    else
      Console.WriteLine("ERROR: Failed to set motor speed, motors are disabled");
  }


  public void EmergencyStop()
  {
    _currentSpeed = 0.0;
    _targetSpeed = 0.0;
    SetMotorSpeed();
  }

  public void Update()
  {
    if (_farmrobot.EmergencyStop)
    {
      return;
    }

    double previousSpeed = _currentSpeed;

    if (_currentSpeed < _targetSpeed)
    {
      // Increase speed but don't exceed maximum of 1.0
      _currentSpeed += _speedIncrease;
      if (_currentSpeed > 1.0)
      {
        _currentSpeed = 1.0;
      }
      else if (_currentSpeed > _targetSpeed)
      {
        _currentSpeed = _targetSpeed;
      }
    }
    else if (_currentSpeed > _targetSpeed)
    {
      // Decrease speed but don't exceed minimum of -1.0
      _currentSpeed -= _speedIncrease;
      if (_currentSpeed < -1.0)
      {
        _currentSpeed = -1.0;
      }
      else if (_currentSpeed < -_targetSpeed)
      {
        _currentSpeed = -_targetSpeed;
      }
    }

    if (previousSpeed != _currentSpeed)
    {
      SetMotorSpeed();
      _farmrobot.SendMessage("CropBotics/sensor/Motors", $"{CurrentSpeed}");
    }
  }
}
