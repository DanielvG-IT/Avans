use DB2226789
go

Create table SensorData(
	Id int Primary key Identity(1,1) not null,
	SensorName varchar(255) not null,
	DataTimestamp  DateTime not null,
	SensorData varchar(255) not null,
);

Create table Commands(
	Id int Primary key Identity(1,1) not null,
	DataTimestamp  DateTime not null,
	Command varchar(255) not null,
);

