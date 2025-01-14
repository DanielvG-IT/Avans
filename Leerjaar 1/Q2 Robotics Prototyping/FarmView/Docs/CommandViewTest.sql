-- Clear existing data from CommandHistory
DELETE FROM CommandHistory;

-- Insert test data for CommandHistory
INSERT INTO CommandHistory (ID, CommandName, DataTimestamp, CommandData)
VALUES
    (1, 'StartProcess', GETDATE(), 'Success'),
    (2, 'StopProcess', GETDATE(), 'Failed'),
    (3, 'RestartService', GETDATE(), 'Success'),
    (4, 'UpdateConfig', DATEADD(DAY, -1, GETDATE()), 'Success'),
    (5, 'BackupData', DATEADD(DAY, -1, GETDATE()), 'InProgress'),
    (6, 'RestoreData', DATEADD(DAY, -1, GETDATE()), 'Failed'),
    (7, 'Shutdown', DATEADD(DAY, -2, GETDATE()), 'Success'),
    (8, 'InitializeSystem', DATEADD(DAY, -2, GETDATE()), 'Pending'),
    (9, 'Reboot', DATEADD(DAY, -2, GETDATE()), 'Success'),
    (10, 'ValidateConfig', GETDATE(), 'Completed');

SELECT * FROM CommandHistory;
SELECT * FROM CommandHistory WHERE CAST(DataTimestamp AS DATE) = CAST(GETDATE() AS DATE);
