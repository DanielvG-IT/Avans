use DB2226789
go

Create table SensorHistory(
	Id int Primary key Identity(1,1) not null,
	SensorName varchar(255) not null,
	DataTimestamp  DateTime not null,
	SensorData varchar(255) not null,
);

Create table CommandHistory(
	Id int Primary key Identity(1,1) not null,
	CommandName varchar(255) not null,
	DataTimestamp  DateTime not null,
	CommandData varchar(255) not null,
);

