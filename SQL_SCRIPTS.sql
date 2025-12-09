
Create  database ClinicalDb
go

------
use ClinicalDb
go
------

Create table Clinician
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

create table Notes
(
Id int Primary Key identity(1,1),
Patient_Id varchar(10),
notes varchar(500),
create_date datetime
)

--select * from Notes
--select * from Clinician
--insert into Clinician values('admin','admin','CLINICIAN',getdate(),'Admin','Admin',getdate())


