-- Clear existing data from SensorHistory
DELETE FROM SensorHistory;

-- Insert test data for SensorHistory
INSERT INTO SensorHistory (ID, SensorName, DataTimestamp, SensorData)
VALUES
    (1, 'TemperatureSensor', GETDATE(), '22.5°C'),
    (2, 'HumiditySensor', GETDATE(), '45%'),
    (3, 'PressureSensor', GETDATE(), '1013 hPa'),
    (4, 'LightSensor', DATEADD(DAY, -1, GETDATE()), '500 lux'),
    (5, 'MotionSensor', DATEADD(DAY, -1, GETDATE()), 'Active'),
    (6, 'ProximitySensor', DATEADD(DAY, -1, GETDATE()), 'Detected'),
    (7, 'SoundSensor', DATEADD(DAY, -2, GETDATE()), '75 dB'),
    (8, 'GasSensor', DATEADD(DAY, -2, GETDATE()), 'Normal'),
    (9, 'VibrationSensor', DATEADD(DAY, -2, GETDATE()), 'Low'),
    (10, 'TemperatureSensor', GETDATE(), '23.0°C');

SELECT * FROM SensorHistory;
SELECT * FROM SensorHistory WHERE CAST(DataTimestamp AS DATE) = CAST(GETDATE() AS DATE);
