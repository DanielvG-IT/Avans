USE GraphicsSecureCommunication;

-- Create Environment2D table
CREATE TABLE Environment2D (
    Id UniqueIdentifier PRIMARY KEY,
    Name VARCHAR(255),
    MaxHeight INT,
    MaxLength INT
);

-- Create Object2D table
CREATE TABLE Object2D (
    Id UniqueIdentifier PRIMARY KEY,
    PrefabId INT, 
    PositionX FLOAT,
    PositionY FLOAT,
    ScaleX FLOAT,
    ScaleY FLOAT,
    RotationZ FLOAT,
    SortingLayer INT,
    EnvironmentId UniqueIdentifier,  -- Foreign key to link to Environment2D
    FOREIGN KEY (EnvironmentId) REFERENCES Environment2D(Id)
);
