SELECT * FROM PixelHistory

-- Clear existing data
DELETE FROM PixelHistory;

-- Insert test data for today
INSERT INTO PixelHistory (PixelNumber, DataTimestamp, PixelData) VALUES 
(1, GETDATE(), 'Green'),
(2, GETDATE(), 'Yellow'),
(3, GETDATE(), 'Red'),
(4, GETDATE(), 'Green'),
(5, GETDATE(), 'Yellow'),
(6, GETDATE(), 'Red'),
(7, GETDATE(), 'Green'),
(8, GETDATE(), 'Yellow'),
(9, GETDATE(), 'Red');


-- Insert test data for yesterday
INSERT INTO PixelHistory (PixelNumber, DataTimestamp, PixelData) VALUES 
(1, DATEADD(day, -1, GETDATE()), 'Yellow'),
(2, DATEADD(day, -1, GETDATE()), 'Red'),
(3, DATEADD(day, -1, GETDATE()), 'Green'),
(4, DATEADD(day, -1, GETDATE()), 'Yellow'),
(5, DATEADD(day, -1, GETDATE()), 'Red'),
(6, DATEADD(day, -1, GETDATE()), 'Green'),
(7, DATEADD(day, -1, GETDATE()), 'Yellow'),
(8, DATEADD(day, -1, GETDATE()), 'Red'),
(9, DATEADD(day, -1, GETDATE()), 'Green');


SELECT * FROM PixelHistory

SELECT PixelNumber, DataTimestamp, PixelData FROM PixelHistory WHERE DataTimestamp LIKE GETDATE()