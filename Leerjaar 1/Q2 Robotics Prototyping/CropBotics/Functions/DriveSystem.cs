using CropBotics.Interfaces;
using Avans.StatisticalRobot;

namespace CropBotics.Functions;

public class DriveSystem(FarmRobot farmrobot) : IUpdatable
{
  private readonly FarmRobot _farmrobot = farmrobot;
  public short CalibrationLeft = 4;

  public short CalibrationRight = 0;

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
        (short)(CalculateRobotSpeed(_currentSpeed) + CalibrationLeft),
        (short)(CalculateRobotSpeed(_currentSpeed) + CalibrationRight)
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
