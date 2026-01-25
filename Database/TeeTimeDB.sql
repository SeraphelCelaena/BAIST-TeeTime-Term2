-- Use Database
use TeeTimeDB;
GO

-- Create
Create Table Roles
(
	RoleID Int Identity(1, 1),
	RoleName VarChar(30) Not Null,
	Constraint PK_Roles Primary Key (RoleID)
)
GO

Create Table TeeTimeUser
(
	Email VarChar(100) Not Null,
	Password VarChar(50) Not Null,
	FirstName VarChar(50) Not Null,
	LastName VarChar(50) Not Null,
	PhoneNumber VarChar(10) Not Null,
	Address VarChar(100) Not Null,
	City VarChar(50) Not Null,
	Province VarChar(50) Not Null,
	PostalCode VarChar(6) Not Null,
	RoleID Int Not Null,
	Constraint PK_TeeTimeUser Primary Key (Email),
	Constraint FK_TeeTimeUser_Roles Foreign Key (RoleID) References Roles(RoleID),
	Constraint Unique_Email Unique (Email),
	Constraint Valid_Email Check (Email Like '%_@__%.__%'), -- Stole the Regex from the internet
	Constraint Valid_Phone Check (PhoneNumber Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' And Len(PhoneNumber) = 10),
	Constraint Valid_PostalCode Check (PostalCode Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]')
)
GO

Create Table TeeTimeStart
(
	TeeTimeID Int Identity(1000000, 1),
	Date Date Not Null,
	StartTime Time Not Null,
	Constraint PK_TeeTimeStart Primary Key (TeeTimeID)
)
GO

-- Insert Data
Insert into Roles (RoleName)
values
	('Admin'),
	('Stakeholder'),
	('Gold'),
	('Silver'),
	('Bronze'),
	('Copper')
GO

Insert into TeeTimeUser (Email, Password, FirstName, LastName, PhoneNumber, Address, City, Province, PostalCode, RoleID)
values
	('admin@baist.com', 'AdminPass123', 'Admin', 'User', '1234567890', '123 Admin St.', 'AdminCity', 'AdminProvince', 'A1A1A1', 1),
	('stakeholder@baist.com', 'StakeholderPass123', 'Stake', 'Holder', '2345678901', '123 Stakeholder St.', 'StakeholderCity', 'StakeholderProvince', 'B2B2B2', 2),
	('gold@baist.com', 'GoldPass123', 'Gold', 'Member', '3456789012', '123 Gold St.', 'GoldCity', 'GoldProvince', 'C3C3C3', 3),
	('silver@baist.com', 'SilverPass123', 'Silver', 'Member', '4567890123', '123 Silver St.', 'SilverCity', 'SilverProvince', 'D4D4D4', 4),
	('bronze@baist.com', 'BronzePass123', 'Bronze', 'Member', '5678901234', '123 Bronze St.', 'BronzeCity', 'BronzeProvince', 'E5E5E5', 5)
GO

-- Stored Procedures
Create Procedure LoginUser(
	@Email VarChar(100),
	@Password VarChar(50)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @Password Is Null
		Raiserror('LoginUser - Email and Password must be provided.', 16, 1)
	Else
		Begin
			-- Get Basic user information
			Select
			Email, FirstName, LastName, PhoneNumber, Roles.RoleName
			From TeeTimeUser
			Inner Join Roles on TeeTimeUser.RoleID = Roles.RoleID
			Where Email = @Email And Password = @Password

			If @@Error = 0
				Set @TeeTimeReturnCode = 0 -- Success
			Else
				Raiserror('LoginUser - Invalid Email or Password.', 16, 1)
		End

	Return @TeeTimeReturnCode
GO
