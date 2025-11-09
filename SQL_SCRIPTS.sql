Create database ClinicalDb
go

------
use ClinicalDb
go
------
Create table db_Clinician
(
Id int Primary Key identity(1,1),
username varchar(50),
password varchar(50),
role varchar(50),
create_date datetime,
Name varchar(100),
lastName varchar(100),
DOJ datetime
)


