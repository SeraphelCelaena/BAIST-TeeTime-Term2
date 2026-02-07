-- Use Database
use TeeTimeDB
GO

-- Drop Tables if they exist
If Exists (Select Name From sys.tables Where Name = 'TeeTimeConfirmation')
	Drop Table TeeTimeConfirmation
GO

If Exists (Select Name From sys.tables Where Name = 'UserWarnings')
	Drop Table UserWarnings
GO

If Exists (Select Name From sys.tables Where Name = 'TeeTimeUser')
	Drop Table TeeTimeUser
GO

If Exists (Select Name From sys.tables Where Name = 'Roles')
	Drop Table Roles
GO

If Exists (Select Name From sys.tables Where Name = 'TeeTimeStart')
	Drop Table TeeTimeStart
GO

-- Drop Stored Procedures if they exist
If Exists (Select Name From sys.procedures Where Name = 'RegisterUser')
	Drop Procedure RegisterUser
GO

If Exists (Select Name From sys.procedures Where Name = 'LoginUser')
	Drop Procedure LoginUser
GO

If Exists (Select Name From sys.procedures Where Name = 'CheckTeeTimeOnDate')
	Drop Procedure CheckTeeTimeOnDate
GO

If Exists (Select Name From sys.procedures Where Name = 'BookTeeTime')
	Drop Procedure BookTeeTime
GO

If Exists (Select Name From sys.procedures Where Name = 'AddConfirmTeeTime')
	Drop Procedure AddConfirmTeeTime
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

Create Table UserWarnings
(
	WarningID Int Identity(1, 1),
	Email VarChar(100) Not Null,
	WarningMessage VarChar(255) Not Null,
	WarningStartDate Date Not Null,
	WarningEndDate Date Not Null,
	Constraint PK_UserWarnings Primary Key (WarningID),
	Constraint FK_UserWarnings_TeeTimeUser Foreign Key (Email) References TeeTimeUser(Email)
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

Create Table TeeTimeConfirmation
(
	TeeTimeID Int Not Null,
	Email VarChar(100) Not Null,
	Confirmed Bit Not Null,
	Constraint PK_TeeTimeConfirmation Primary Key (TeeTimeID, Email),
	Constraint FK_TeeTimeConfirmation_TeeTimeUser Foreign Key (Email) References TeeTimeUser(Email),
	Constraint FK_TeeTimeConfirmation_TeeTimeStart Foreign Key (TeeTimeID) References TeeTimeStart(TeeTimeID)
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

-- Stored Procedures
Create Procedure RegisterUser(
	@Email VarChar(100),
	@Password VarChar(50),
	@FirstName VarChar(50),
	@LastName VarChar(50),
	@PhoneNumber VarChar(10),
	@Address VarChar(100),
	@City VarChar(50),
	@Province VarChar(50),
	@PostalCode VarChar(6),
	@RoleID Int
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @Password Is Null Or @FirstName Is Null Or @LastName Is Null Or
		@PhoneNumber Is Null Or @Address Is Null Or @City Is Null Or @Province Is Null Or
		@PostalCode Is Null Or @RoleID Is Null -- Checks if all fields are provided
		Raiserror('RegisterUser - All fields must be provided.', 16, 1)
	Else
		If Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for existing email
			Raiserror('RegisterUser - Email already exists.', 16, 1)
		Else
			If Not Exists (Select 1 From Roles Where RoleID = @RoleID) -- Check for valid RoleID
				Raiserror('RegisterUser - Invalid RoleID.', 16, 1)
			Else
				If Len(@PhoneNumber) <> 10 Or @PhoneNumber Not Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' -- Check PhoneNumber format
					Raiserror('RegisterUser - Invalid PhoneNumber format.', 16, 1)
				Else
					If Len(@PostalCode) <> 6 Or @PostalCode Not Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]' -- Check PostalCode format
						Raiserror('RegisterUser - Invalid PostalCode format.', 16, 1)
					Else
						If @Email Not Like '%_@__%.__%' -- Check Email format
							Raiserror('RegisterUser - Invalid Email format.', 16, 1)
						Else
							Begin -- Insert the new user
								Insert into TeeTimeUser (Email, Password, FirstName, LastName, PhoneNumber, Address, City, Province, PostalCode, RoleID)
								Values (@Email, @Password, @FirstName, @LastName, @PhoneNumber, @Address, @City, @Province, @PostalCode, @RoleID)

								If @@Error = 0
									Set @TeeTimeReturnCode = 0 -- Success
								Else
									Raiserror('RegisterUser - Error inserting user.', 16, 1)
							End

	Return @TeeTimeReturnCode
GO

Create Procedure LoginUser(
	@Email VarChar(100),
	@Password VarChar(50)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @Password Is Null -- Checks if Email and Password are provided
		Raiserror('LoginUser - Email and Password must be provided.', 16, 1)
	Else
		If @Email Not Like '%_@__%.__%' -- Check Email format
			Raiserror('LoginUser - Invalid Email format.', 16, 1)
		Else
			If Not Exists (Select 1 From TeeTimeUser Where Email = @Email And Password = @Password) -- Check for valid credentials
				Raiserror('LoginUser - Invalid Email or Password.', 16, 1)
			Else
				Begin -- Get Basic user information
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

Create Procedure CheckTeeTimeOnDate(
	@Date Date
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Date Is Null -- Checks if Date is provided
		Raiserror('CheckTeeTimeOnDate - Date must be provided.', 16, 1)
	Else
		If @Date < Cast(GetDate() As Date) -- Check if date is in the past
			Raiserror('CheckTeeTimeOnDate - Date cannot be in the past.', 16, 1)
		Else
			Begin -- Get Tee Times for the specified date
				Select
				TeeTimeID, StartTime, (Select Count (*) From TeeTimeConfirmation Where TeeTimeConfirmation.TeeTimeID = TeeTimeStart.TeeTimeID And Confirmed = 1) As ConfirmedCount
				From TeeTimeStart
				Where Date = @Date

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('CheckTeeTimeOnDate - Error retrieving tee times.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure BookTeeTime(
	@Date Date,
	@StartTime Time,
	@TeeTimeIDReturn Int Output
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Date Is Null Or @StartTime Is Null -- Checks if Date and StartTime are provided
		Raiserror('BookTeeTime - Date and StartTime must be provided.', 16, 1)
	Else
		If @Date < Cast(GetDate() As Date) -- Check if date is in the past
			Raiserror('BookTeeTime - Date cannot be in the past.', 16, 1)
		Else
			Begin -- Insert the new Tee Time
				Insert into TeeTimeStart (Date, StartTime)
				Values (@Date, @StartTime)

				If @@Error = 0
					Begin
						Set @TeeTimeIDReturn = SCOPE_IDENTITY() -- Get the newly created TeeTimeID
						Set @TeeTimeReturnCode = 0 -- Success
					End
				Else
					Raiserror('BookTeeTime - Error booking tee time.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure AddConfirmTeeTime(
	@TeeTimeID Int,
	@Email VarChar(100),
	@Confirmed Bit
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @TeeTimeID Is Null Or @Email Is Null Or @Confirmed Is Null -- Checks if all fields are provided
		Raiserror('AddConfirmTeeTime - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeStart Where TeeTimeID = @TeeTimeID) -- Check for valid TeeTimeID
			Raiserror('AddConfirmTeeTime - Invalid TeeTimeID.', 16, 1)
		Else
			If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
				Raiserror('AddConfirmTeeTime - Invalid Email.', 16, 1)
			Else
				Begin -- Insert the confirmation
					Insert into TeeTimeConfirmation (TeeTimeID, Email, Confirmed)
					Values (@TeeTimeID, @Email, @Confirmed)

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('AddConfirmTeeTime - Error adding confirmation.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

-- Insert Data using stored procedures
Exec RegisterUser
	@Email = 'admin@baist.ca',
	@Password = 'AdminPass123',
	@FirstName = 'Admin',
	@LastName = 'User',
	@PhoneNumber = '1234567890',
	@Address = '123 Admin St.',
	@City = 'AdminCity',
	@Province = 'AdminProvince',
	@PostalCode = 'A1A1A1',
	@RoleID = 1
GO

Exec RegisterUser
	@Email = 'stakeholder@baist.ca',
	@Password = 'StakeholderPass123',
	@FirstName = 'Stake',
	@LastName = 'Holder',
	@PhoneNumber = '2345678901',
	@Address = '123 Stakeholder St.',
	@City = 'StakeholderCity',
	@Province = 'StakeholderProvince',
	@PostalCode = 'B2B2B2',
	@RoleID = 2
GO

Exec RegisterUser
	@Email = 'gold@baist.ca',
	@Password = 'GoldPass123',
	@FirstName = 'Gold',
	@LastName = 'Member',
	@PhoneNumber = '3456789012',
	@Address = '123 Gold St.',
	@City = 'GoldCity',
	@Province = 'GoldProvince',
	@PostalCode = 'C3C3C3',
	@RoleID = 3
GO

Exec RegisterUser
	@Email = 'silver@baist.ca',
	@Password = 'SilverPass123',
	@FirstName = 'Silver',
	@LastName = 'Member',
	@PhoneNumber = '4567890123',
	@Address = '123 Silver St.',
	@City = 'SilverCity',
	@Province = 'SilverProvince',
	@PostalCode = 'D4D4D4',
	@RoleID = 4
GO

Exec RegisterUser
	@Email = 'bronze@baist.ca',
	@Password = 'BronzePass123',
	@FirstName = 'Bronze',
	@LastName = 'Member',
	@PhoneNumber = '5678901234',
	@Address = '123 Bronze St.',
	@City = 'BronzeCity',
	@Province = 'BronzeProvince',
	@PostalCode = 'E5E5E5',
	@RoleID = 5
GO
