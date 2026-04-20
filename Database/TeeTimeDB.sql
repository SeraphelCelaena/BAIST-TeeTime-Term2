-- Use Database
-- use TeeTimeDB
-- GO

-- Drop Tables if they exist
If Exists (Select Name From sys.tables Where Name = 'MembershipApplication')
	Drop Table MembershipApplication
GO

If Exists (Select Name From sys.tables Where Name = 'StandingTeeTimeConfirmation')
	Drop Table StandingTeeTimeConfirmation
GO

If Exists (Select Name From sys.tables Where Name = 'StandingTeeTime')
	Drop Table StandingTeeTime
GO

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

If Exists (Select Name From sys.procedures Where Name = 'GetAllRoles')
	Drop Procedure GetAllRoles
GO

If Exists (Select Name From sys.procedures Where Name = 'GetAllTeeTimes')
	Drop Procedure GetAllTeeTimes
GO

If Exists (Select Name From sys.procedures Where Name = 'AddStandingTeeTime')
	Drop Procedure AddStandingTeeTime
GO

If Exists (Select Name From sys.procedures Where Name = 'AddStandingTeeTimeConfirmation')
	Drop Procedure AddStandingTeeTimeConfirmation
GO

If Exists (Select Name From sys.procedures Where Name = 'GetTeeTimesForUser')
	Drop Procedure GetTeeTimesForUser
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateTeeTimeForUser')
	Drop Procedure UpdateTeeTimeForUser
GO

If Exists (Select Name From sys.procedures Where Name = 'DeleteTeeTime')
	Drop Procedure DeleteTeeTime
GO

If Exists (Select Name From sys.procedures Where Name = 'AddWarningToUser')
	Drop Procedure AddWarningToUser
GO

If Exists (Select Name From sys.procedures Where Name = 'GetWarningsForUser')
	Drop Procedure GetWarningsForUser
GO

If Exists (Select Name From sys.procedures Where Name = 'DeleteWarning')
	Drop Procedure DeleteWarning
GO

If Exists (Select Name From sys.procedures Where Name = 'GetAllUsers')
	Drop Procedure GetAllUsers
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateUserAdmin')
	Drop Procedure UpdateUserAdmin
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateUser')
	Drop Procedure UpdateUser
GO

If Exists (Select Name From sys.procedures Where Name = 'AddMembershipApplication')
	Drop Procedure AddMembershipApplication
GO

If Exists (Select Name From sys.procedures Where Name = 'GetAllMembershipApplications')
	Drop Procedure GetAllMembershipApplications
GO

If Exists (Select Name From sys.procedures Where Name = 'GetMembershipApplication')
	Drop Procedure GetMembershipApplication
GO

If Exists (Select Name From sys.procedures Where Name = 'GetUserInformation')
	Drop Procedure GetUserInformation
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateEmail')
	Drop Procedure UpdateEmail
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateName')
	Drop Procedure UpdateName
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdatePhoneNumber')
	Drop Procedure UpdatePhoneNumber
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateAddress')
	Drop Procedure UpdateAddress
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdatePassword')
	Drop Procedure UpdatePassword
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateMembershipApplicationStatus')
	Drop Procedure UpdateMembershipApplicationStatus
GO

If Exists (Select Name From sys.procedures Where Name = 'UpdateUserRole')
	Drop Procedure UpdateUserRole

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
	Address VarChar(100) Null,
	City VarChar(50) Null,
	Province VarChar(50) Null,
	PostalCode VarChar(6) Null,
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
	Constraint FK_UserWarnings_TeeTimeUser Foreign Key (Email) References TeeTimeUser(Email) On Update Cascade
)
GO

Create Table TeeTimeStart
(
	TeeTimeID Int Identity(1000000, 1),
	Date Date Not Null,
	StartTime Time Not Null,
	Count Int Not Null,
	Constraint PK_TeeTimeStart Primary Key (TeeTimeID)
)
GO

Create Table TeeTimeConfirmation
(
	TeeTimeID Int Not Null,
	Email VarChar(100) Not Null,
	Confirmed Bit Not Null,
	Constraint PK_TeeTimeConfirmation Primary Key (TeeTimeID, Email),
	Constraint FK_TeeTimeConfirmation_TeeTimeUser Foreign Key (Email) References TeeTimeUser(Email) On Update Cascade,
	Constraint FK_TeeTimeConfirmation_TeeTimeStart Foreign Key (TeeTimeID) References TeeTimeStart(TeeTimeID)
)
GO

Create Table StandingTeeTime
(
	StandingTeeTimeID Int Identity(100000, 1),
	StakeholderEmail VarChar(100) Not Null,
	DayOfWeek Int Not Null,
	StartDate Date Not Null,
	EndDate Date Not Null,
	RequestedTime Time Not Null,
	NumberOfCarts Int Not Null,
	Constraint PK_StandingTeeTime Primary Key (StandingTeeTimeID),
	Constraint FK_StandingTeeTime_TeeTimeUser Foreign Key (StakeholderEmail) References TeeTimeUser(Email) On Update Cascade
)
GO

Create Table StandingTeeTimeConfirmation
(
	StandingTeeTimeConfirmationID Int Identity(10000000, 1),
	StandingTeeTimeID Int Not Null,
	Email VarChar(100) Not Null,
	Confirmed Bit Not Null,
	Constraint PK_StandingTeeTimeConfirmation Primary Key (StandingTeeTimeConfirmationID),
	Constraint FK_StandingTeeTimeConfirmation_TeeTimeUser Foreign Key (Email) References TeeTimeUser(Email) On Update Cascade,
	Constraint FK_StandingTeeTimeConfirmation_StandingTeeTime Foreign Key (StandingTeeTimeID) References StandingTeeTime(StandingTeeTimeID)
)
GO

Create Table MembershipApplication
(
	ApplicationID Int Identity(1, 1),
	Email VarChar(100) Not Null,
	FirstName VarChar(50) Not Null,
	LastName VarChar(50) Not Null,
	Address VarChar(100) Not Null,
	City VarChar(50) Not Null,
	Province VarChar(50) Not Null,
	PostalCode VarChar(6) Not Null,
	PhoneNumber VarChar(10) Not Null,
	Alt_PhoneNumber VarChar(10) Null,
	DateOfBirth Date Not Null,
	Occupation VarChar(50) Not Null,
	CompanyName VarChar(50) Null,
	CompanyAddress VarChar(100) Null,
	CompanyPostalCode VarChar(6) Null,
	CompanyPhoneNumber VarChar(10) Null,
	DateApplied Date Not Null,
	Status VarChar(20) Not Null,
	Constraint PK_MembershipApplication Primary Key (ApplicationID),
	Constraint Valid_Membership_Email Check (Email Like '%_@__%.__%'), -- Stole the Regex from the internet
	Constraint Valid_Membership_Phone Check (PhoneNumber Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' And Len(PhoneNumber) = 10),
	Constraint Valid_Membership_Alt_Phone Check (Alt_PhoneNumber Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' And Len(Alt_PhoneNumber) = 10),
	Constraint Valid_Membership_CompanyPhone Check (CompanyPhoneNumber Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' And Len(CompanyPhoneNumber) = 10),
	Constraint Valid_Membership_PostalCode Check (PostalCode Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]'),
	Constraint Valid_Membership_CompanyPostalCode Check (CompanyPostalCode Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]'),
	Constraint FK_MembershipApplication_TeeTimeUser Foreign Key (Email) References TeeTimeUser(Email) On Update Cascade
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
	('Copper'),
	('User')
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
		@PhoneNumber Is Null Or @RoleID Is Null -- Checks if all fields are provided
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
	@Count Int,
	@TeeTimeIDReturn Int Output
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Date Is Null Or @StartTime Is Null Or @Count Is Null -- Checks if Date and StartTime are provided
		Raiserror('BookTeeTime - Date and StartTime must be provided.', 16, 1)
	Else
		If @Date < Cast(GetDate() As Date) -- Check if date is in the past
			Raiserror('BookTeeTime - Date cannot be in the past.', 16, 1)
		Else
			If (
				Select Sum(Count)
				From TeeTimeStart
				Where Date = @Date And StartTime = @StartTime
				) + @Count > 4 -- Check if the tee time is already overbooked
				Raiserror('BookTeeTime - Tee time will be over capacity for this date and time.', 16, 1)
			Else
				Begin -- Insert the new Tee Time
					Insert into TeeTimeStart (Date, StartTime, Count)
					Values (@Date, @StartTime, @Count)

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

Create Procedure GetAllRoles
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	Begin
		Select RoleID, RoleName
		From Roles

		If @@Error = 0
			Set @TeeTimeReturnCode = 0 -- Success
		Else
			Raiserror('GetAllRoles - Error retrieving roles.', 16, 1)
	End
GO

Create Procedure GetAllTeeTimes
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	Begin
		Select
		TeeTimeStart.TeeTimeID, Date, StartTime, Count, (Select Count (*) From TeeTimeConfirmation Where TeeTimeConfirmation.TeeTimeID = TeeTimeStart.TeeTimeID And Confirmed = 1) As ConfirmedCount, TeeTimeConfirmation.Confirmed
		From TeeTimeStart
		Join TeeTimeConfirmation on TeeTimeStart.TeeTimeID = TeeTimeConfirmation.TeeTimeID
		Where Date >= Cast(GetDate() As Date)

		If @@Error = 0
			Set @TeeTimeReturnCode = 0 -- Success
		Else
			Raiserror('GetAllTeeTimes - Error retrieving tee times.', 16, 1)
	End
GO

Create Procedure AddStandingTeeTime(
	@StakeholderEmail VarChar(100),
	@DayOfWeek Int,
	@StartDate Date,
	@EndDate Date,
	@RequestedTime Time,
	@NumberOfCarts Int,
	@TeeTimeIDReturn Int Output
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @StakeholderEmail Is Null Or @DayOfWeek Is Null Or @StartDate Is Null Or @EndDate Is Null Or @RequestedTime Is Null -- Checks if all fields are provided
		Raiserror('AddStandingTeeTime - All fields must be provided.', 16, 1)
	Else
		If (@StakeholderEmail Not In (Select Email From TeeTimeUser Where RoleID = (Select RoleID From Roles Where RoleName = 'Stakeholder')) ) -- Check if user is a Stakeholder
			Raiserror('AddStandingTeeTime - User is not a Stakeholder.', 16, 1)
		Else
			If @StartDate < Cast(GetDate() As Date) -- Check if StartDate is in the past
				Raiserror('AddStandingTeeTime - StartDate cannot be in the past.', 16, 1)
			Else
				If @EndDate < @StartDate -- Check if EndDate is before StartDate
					Raiserror('AddStandingTeeTime - EndDate cannot be before StartDate.', 16, 1)
				Else
					If @DayOfWeek < 1 Or @DayOfWeek > 7 -- Check if DayOfWeek is valid
						Raiserror('AddStandingTeeTime - DayOfWeek must be between 1 and 7.', 16, 1)
					Else
						If @NumberOfCarts < 1 -- Check if NumberOfCarts is valid
							Raiserror('AddStandingTeeTime - NumberOfCarts must be at least 1.', 16, 1)
						Else
						Begin -- Insert the new Standing Tee Time
							Insert into StandingTeeTime (StakeholderEmail, DayOfWeek, StartDate, EndDate, RequestedTime, NumberOfCarts)
							Values (@StakeholderEmail, @DayOfWeek, @StartDate, @EndDate, @RequestedTime, @NumberOfCarts)

							If @@Error = 0
								Begin
									Set @TeeTimeIDReturn = SCOPE_IDENTITY() -- Get the newly created StandingTeeTimeID
									Set @TeeTimeReturnCode = 0 -- Success
								End
							Else
								Raiserror('AddStandingTeeTime - Error adding standing tee time.', 16, 1)
						End

	Return @TeeTimeReturnCode
GO

Create Procedure AddStandingTeeTimeConfirmation(
	@StandingTeeTimeID Int,
	@Email VarChar(100),
	@Date Date,
	@Confirmed Bit
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @StandingTeeTimeID Is Null Or @Email Is Null Or @Date Is Null Or @Confirmed Is Null -- Checks if all fields are provided
		Raiserror('AddStandingTeeTimeConfirmation - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From StandingTeeTime Where StandingTeeTimeID = @StandingTeeTimeID) -- Check for valid StandingTeeTimeID
			Raiserror('AddStandingTeeTimeConfirmation - Invalid StandingTeeTimeID.', 16, 1)
		Else
			If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
				Raiserror('AddStandingTeeTimeConfirmation - Invalid Email.', 16, 1)
			Else
				Begin -- Insert the confirmation
					Insert into StandingTeeTimeConfirmation (StandingTeeTimeID, Email, Confirmed)
					Values (@StandingTeeTimeID, @Email, @Confirmed)

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('AddStandingTeeTimeConfirmation - Error adding confirmation.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure GetTeeTimesForUser(
	@Email VarChar(100)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null -- Checks if Email is provided
		Raiserror('GetTeeTimesForUser - Email must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('GetTeeTimesForUser - Invalid Email.', 16, 1)
		Else
			Begin -- Get all tee times the user is confirmed for
				Select
				TeeTimeStart.TeeTimeID, TeeTimeConfirmation.Email, Date, StartTime, Count, TeeTimeConfirmation.Confirmed
				From TeeTimeStart
				Inner Join TeeTimeConfirmation on TeeTimeStart.TeeTimeID = TeeTimeConfirmation.TeeTimeID
				Where TeeTimeConfirmation.Email = @Email and Date >= Cast(GetDate() As Date)

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('GetTeeTimesForUser - Error retrieving tee times for user.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateTeeTimeForUser(
	@TeeTimeID Int,
	@Email VarChar(100),
	@Date Date,
	@StartTime Time,
	@Count Int,
	@Confirmed Bit
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @TeeTimeID Is Null Or @Date Is Null Or @StartTime Is Null Or @Count Is Null Or @Confirmed Is Null -- Checks if all fields are provided
		Raiserror('UpdateTeeTimeForUser - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeStart Where TeeTimeID = @TeeTimeID) -- Check for valid TeeTimeID
			Raiserror('UpdateTeeTimeForUser - Invalid TeeTimeID.', 16, 1)
		Else
			If Not Exists (Select 1 From TeeTimeConfirmation Where TeeTimeID = @TeeTimeID) -- Check if the tee time is already booked
				Raiserror('UpdateTeeTimeForUser - Tee time not found.', 16, 1)
			Else
				If @Date < Cast(GetDate() As Date) -- Check if date is in the past
					Raiserror('UpdateTeeTimeForUser - Date cannot be in the past.', 16, 1)
				Else
					If (
						Select Sum(Count)
						From TeeTimeStart
						Where Date = @Date And StartTime = @StartTime And TeeTimeID <> @TeeTimeID
						) + @Count > 4 -- Check if the tee time is already overbooked
						Raiserror('UpdateTeeTimeForUser - Tee time already booked for this date and time.', 16, 1)
					Else
						Begin
							Update TeeTimeStart
							Set Date = @Date, StartTime = @StartTime, Count = @Count
							Where TeeTimeID = @TeeTimeID

							If @@Error = 0
								Begin
									Update TeeTimeConfirmation
									Set Confirmed = @Confirmed
									Where TeeTimeID = @TeeTimeID and Email = @Email

									If @@Error = 0
										Set @TeeTimeReturnCode = 0 -- Success
									Else
										Raiserror('UpdateTeeTimeForUser - Error updating tee time confirmation.', 16, 1)
								End
							Else
								Raiserror('UpdateTeeTimeForUser - Error updating tee time.', 16, 1)
						End

	Return @TeeTimeReturnCode
GO

Create Procedure DeleteTeeTime(
	@TeeTimeID Int
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @TeeTimeID Is Null -- Checks if TeeTimeID is provided
		Raiserror('DeleteTeeTime - TeeTimeID must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeStart Where TeeTimeID = @TeeTimeID) -- Check for valid TeeTimeID
			Raiserror('DeleteTeeTime - Invalid TeeTimeID.', 16, 1)
		Else
			Begin
				Delete From TeeTimeConfirmation Where TeeTimeID = @TeeTimeID -- Delete confirmations first due to foreign key constraint
				Delete From TeeTimeStart Where TeeTimeID = @TeeTimeID

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('DeleteTeeTime - Error deleting tee time.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure AddWarningToUser(
	@Email VarChar(100),
	@WarningMessage VarChar(255),
	@WarningStartDate Date,
	@WarningEndDate Date
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @WarningMessage Is Null Or @WarningStartDate Is Null Or @WarningEndDate Is Null -- Checks if all fields are provided
		Raiserror('AddWarningToUser - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('AddWarningToUser - Invalid Email.', 16, 1)
		Else
			If @WarningEndDate < @WarningStartDate -- Check if EndDate is before StartDate
				Raiserror('AddWarningToUser - WarningEndDate cannot be before WarningStartDate.', 16, 1)
			Else
				Begin -- Insert the warning
					Insert into UserWarnings (Email, WarningMessage, WarningStartDate, WarningEndDate)
					Values (@Email, @WarningMessage, @WarningStartDate, @WarningEndDate)

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('AddWarningToUser - Error adding warning to user.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure GetWarningsForUser(
	@Email VarChar(100)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null -- Checks if Email is provided
		Raiserror('GetWarningsForUser - Email must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('GetWarningsForUser - Invalid Email.', 16, 1)
		Else
			Begin -- Get all warnings for the user
				Select WarningID, WarningMessage, WarningStartDate, WarningEndDate
				From UserWarnings
				Where Email = @Email and WarningEndDate >= Cast(GetDate() As Date)

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('GetWarningsForUser - Error retrieving warnings for user.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure DeleteWarning(
	@WarningID Int
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @WarningID Is Null -- Checks if WarningID is provided
		Raiserror('DeleteWarning - WarningID must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From UserWarnings Where WarningID = @WarningID) -- Check for valid WarningID
			Raiserror('DeleteWarning - Invalid WarningID.', 16, 1)
		Else
			Begin
				Delete From UserWarnings Where WarningID = @WarningID

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('DeleteWarning - Error deleting warning.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure GetAllUsers
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	Begin
		Select Email, FirstName, LastName, PhoneNumber, Address, City, Province, PostalCode, RoleID
		From TeeTimeUser
		Where Email != 'superuser@baist.ca'

		If @@Error = 0
			Set @TeeTimeReturnCode = 0 -- Success
		Else
			Raiserror('GetAllUsers - Error retrieving users.', 16, 1)
	End
GO

Create Procedure UpdateUserAdmin(
	@Email VarChar(100),
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

	If @Email Is Null Or @FirstName Is Null Or @LastName Is Null Or @PhoneNumber Is Null Or @Address Is Null Or @City Is Null Or @Province Is Null Or @PostalCode Is Null Or @RoleID Is Null -- Checks if all fields are provided
		Raiserror('UpdateUser - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdateUser - Invalid Email.', 16, 1)
		Else
			If Not Exists (Select 1 From Roles Where RoleID = @RoleID) -- Check for valid RoleID
				Raiserror('UpdateUser - Invalid RoleID.', 16, 1)
			Else
				Begin -- Update the user
					Update TeeTimeUser
					Set FirstName = @FirstName,
						LastName = @LastName,
						PhoneNumber = @PhoneNumber,
						Address = @Address,
						City = @City,
						Province = @Province,
						PostalCode = @PostalCode,
						RoleID = @RoleID
					Where Email = @Email

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('UpdateUser - Error updating user.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateUser(
	@Email VarChar(100),
	@Password VarChar(50),
	@FirstName VarChar(50),
	@LastName VarChar(50),
	@PhoneNumber VarChar(10),
	@Address VarChar(100),
	@City VarChar(50),
	@Province VarChar(50),
	@PostalCode VarChar(6)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @Password Is Null Or @FirstName Is Null Or @LastName Is Null Or @PhoneNumber Is Null Or @Address Is Null Or @City Is Null Or @Province Is Null Or @PostalCode Is Null -- Checks if all fields are provided
		Raiserror('UpdateUser - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdateUser - Invalid Email.', 16, 1)
		Else
			Begin -- Update the user
				Update TeeTimeUser
				Set Password = @Password,
					FirstName = @FirstName,
					LastName = @LastName,
					PhoneNumber = @PhoneNumber,
					Address = @Address,
					City = @City,
					Province = @Province,
					PostalCode = @PostalCode
				Where Email = @Email

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('UpdateUser - Error updating user.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure AddMembershipApplication(
	@Email VarChar(100),
	@FirstName VarChar(50),
	@LastName VarChar(50),
	@Address VarChar(100),
	@City VarChar(50),
	@Province VarChar(50),
	@PostalCode VarChar(6),
	@PhoneNumber VarChar(10),
	@Alt_PhoneNumber VarChar(10) = Null,
	@DateOfBirth Date,
	@Occupation VarChar(50),
	@CompanyName VarChar(50) = Null,
	@CompanyAddress VarChar(100) = Null,
	@CompanyPostalCode VarChar(6) = Null,
	@CompanyPhoneNumber VarChar(10) = Null
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure
	Set @Alt_PhoneNumber = NullIf(Ltrim(Rtrim(@Alt_PhoneNumber)), '')
	Set @CompanyName = NullIf(Ltrim(Rtrim(@CompanyName)), '')
	Set @CompanyAddress = NullIf(Ltrim(Rtrim(@CompanyAddress)), '')
	Set @CompanyPostalCode = NullIf(Ltrim(Rtrim(@CompanyPostalCode)), '')
	Set @CompanyPhoneNumber = NullIf(Ltrim(Rtrim(@CompanyPhoneNumber)), '')

	If @Email Is Null Or @FirstName Is Null Or @LastName Is Null Or @Address Is Null Or @City Is Null Or @Province Is Null Or @PostalCode Is Null Or @PhoneNumber Is Null Or @DateOfBirth Is Null Or @Occupation Is Null -- Checks if all required fields are provided
		Raiserror('AddMembershipApplication - All required fields must be provided.', 16, 1)
	Else
		If @Email Not Like '%_@__%.__%' -- Check Email format
			Raiserror('AddMembershipApplication - Invalid Email format.', 16, 1)
		Else
			If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check if the email is already associated with a user account
				Raiserror('AddMembershipApplication - An account with this email already exists.', 16, 1)
			Else
				If Len(@PhoneNumber) <> 10 Or @PhoneNumber Not Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' -- Check PhoneNumber format
					Raiserror('AddMembershipApplication - Invalid PhoneNumber format.', 16, 1)
				Else
					If @Alt_PhoneNumber Is Not Null And (Len(@Alt_PhoneNumber) <> 10 Or @Alt_PhoneNumber Not Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]') -- Check Alt_PhoneNumber format only when supplied
						Raiserror('AddMembershipApplication - Invalid Alt_PhoneNumber format.', 16, 1)
					Else
						If Len(@CompanyPhoneNumber) <> 10 Or @CompanyPhoneNumber Not Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' -- Check CompanyPhoneNumber format
							Raiserror('AddMembershipApplication - Invalid CompanyPhoneNumber format.', 16, 1)
						Else
							If Len(@PostalCode) <> 6 Or @PostalCode Not Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]' -- Check PostalCode format
								Raiserror('AddMembershipApplication - Invalid PostalCode format.', 16, 1)
							Else
								If Len(@CompanyPostalCode) <> 6 Or @CompanyPostalCode Not Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]' -- Check CompanyPostalCode format
									Raiserror('AddMembershipApplication - Invalid CompanyPostalCode format.', 16, 1)
								Else
									If Not Exists (Select 1 From MembershipApplication Where Email = @Email) -- Check for existing application with the same email
										Begin -- Insert the new membership application
											Insert into MembershipApplication (Email, FirstName, LastName, Address, City, Province, PostalCode, PhoneNumber, Alt_PhoneNumber, DateOfBirth, Occupation, CompanyName, CompanyAddress, CompanyPostalCode, CompanyPhoneNumber, DateApplied, Status)
											Values (@Email, @FirstName, @LastName, @Address, @City, @Province, @PostalCode, @PhoneNumber, @Alt_PhoneNumber, @DateOfBirth, @Occupation, @CompanyName, @CompanyAddress, @CompanyPostalCode, @CompanyPhoneNumber, Cast(GetDate() As Date), 'Pending')

											If @@Error = 0
												Set @TeeTimeReturnCode = 0 -- Success
											Else
												Raiserror('AddMembershipApplication - Error adding membership application.', 16, 1)
										End
									Else
										Raiserror('AddMembershipApplication - An application with this email already exists.', 16, 1)

	Return @TeeTimeReturnCode
GO

Create Procedure GetMembershipApplication(
	@Email VarChar(100)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null -- Checks if Email is provided
		Raiserror('GetMembershipApplication - Email must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From MembershipApplication Where Email = @Email) -- Check for valid Email
			Raiserror('GetMembershipApplication - Invalid Email.', 16, 1)
		Else
			Begin -- Get membership application information
				Select Email, FirstName, LastName, Address, City, Province, PostalCode, PhoneNumber, Alt_PhoneNumber, DateOfBirth, Occupation, CompanyName, CompanyAddress, CompanyPostalCode, CompanyPhoneNumber, DateApplied, Status
				From MembershipApplication
				Where Email = @Email

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('GetMembershipApplication - Error retrieving membership application information.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure GetAllMembershipApplications
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	Begin
		Select ApplicationID, Email, FirstName, LastName, Address, City, Province, PostalCode, PhoneNumber, Alt_PhoneNumber, DateOfBirth, Occupation, CompanyName, CompanyAddress, CompanyPostalCode, CompanyPhoneNumber, DateApplied, Status
		From MembershipApplication

		If @@Error = 0
			Set @TeeTimeReturnCode = 0 -- Success
		Else
			Raiserror('GetAllMembershipApplications - Error retrieving membership applications.', 16, 1)
	End
GO

Create Procedure GetUserInformation(
	@Email VarChar(100)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null -- Checks if Email is provided
		Raiserror('GetUserInformation - Email must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('GetUserInformation - Invalid Email.', 16, 1)
		Else
			Begin -- Get user information
				Select Email, FirstName, LastName, PhoneNumber, Address, City, Province, PostalCode, RoleID
				From TeeTimeUser
				Where Email = @Email

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('GetUserInformation - Error retrieving user information.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateEmail(
	@CurrentEmail VarChar(100),
	@NewEmail VarChar(100)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @CurrentEmail Is Null or @NewEmail Is Null -- Checks if Emails are provided
		Raiserror('UpdateEmail - Both Current and New Emails must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @CurrentEmail) -- Check for valid Current Email
			Raiserror('UpdateEmail - Invalid Current Email.', 16, 1)
		Else
			If Exists (Select 1 From TeeTimeUser Where Email = @NewEmail) -- Check if New Email is already in use
				Raiserror('UpdateEmail - New Email is already in use.', 16, 1)
			Else
				Begin -- Update user's email
					Update TeeTimeUser
					Set Email = @NewEmail
					Where Email = @CurrentEmail

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('UpdateEmail - Error updating user email.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateName(
	@Email VarChar(100),
	@NewFirstName VarChar(50),
	@NewLastName VarChar(50)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @NewFirstName Is Null Or @NewLastName Is Null -- Checks if all fields are provided
		Raiserror('UpdateName - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdateName - Invalid Email.', 16, 1)
		Else
			Begin -- Update user's name
				Update TeeTimeUser
				Set FirstName = @NewFirstName,
					LastName = @NewLastName
				Where Email = @Email

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('UpdateName - Error updating user name.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdatePassword(
	@Email VarChar(100),
	@CurrentPassword VarChar(50),
	@NewPassword VarChar(50)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @CurrentPassword Is Null Or @NewPassword Is Null -- Checks if all fields are provided
		Raiserror('UpdatePassword - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdatePassword - Invalid Email.', 16, 1)
		Else
			If Not Exists (Select 1 From TeeTimeUser Where Email = @Email And Password = @CurrentPassword) -- Check if current password is correct
				Raiserror('UpdatePassword - Current password is incorrect.', 16, 1)
			Else
				Begin -- Update user's password
					Update TeeTimeUser
					Set Password = @NewPassword
					Where Email = @Email

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('UpdatePassword - Error updating user password.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdatePhoneNumber(
	@Email VarChar(100),
	@NewPhoneNumber VarChar(10)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @NewPhoneNumber Is Null -- Checks if all fields are provided
		Raiserror('UpdatePhoneNumber - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdatePhoneNumber - Invalid Email.', 16, 1)
		Else
			If Len(@NewPhoneNumber) <> 10 Or @NewPhoneNumber Not Like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'
				Raiserror('UpdatePhoneNumber - Invalid PhoneNumber format.', 16, 1)
			Else
				Begin -- Update user's phone number
					Update TeeTimeUser
					Set PhoneNumber = @NewPhoneNumber
					Where Email = @Email

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('UpdatePhoneNumber - Error updating user phone number.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateAddress(
	@Email VarChar(100),
	@NewAddress VarChar(100),
	@NewCity VarChar(50),
	@NewProvince VarChar(50),
	@NewPostalCode VarChar(6)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @NewAddress Is Null Or @NewCity Is Null Or @NewProvince Is Null Or @NewPostalCode Is Null -- Checks if all fields are provided
		Raiserror('UpdateAddress - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdateAddress - Invalid Email.', 16, 1)
		Else
			If Len(@NewPostalCode) <> 6 Or @NewPostalCode Not Like '[A-Z][0-9][A-Z][0-9][A-Z][0-9]' -- Check PostalCode format
				Raiserror('UpdateAddress - Invalid PostalCode format.', 16, 1)
			Else
				Begin -- Update user's address
					Update TeeTimeUser
					Set Address = @NewAddress,
						City = @NewCity,
						Province = @NewProvince,
						PostalCode = @NewPostalCode
					Where Email = @Email

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('UpdateAddress - Error updating user address.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateMembershipApplicationStatus(
	@ApplicationID Int,
	@Status VarChar(20)
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @ApplicationID Is Null Or @Status Is Null -- Checks if all fields are provided
		Raiserror('UpdateMembershipApplicationStatus - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From MembershipApplication Where ApplicationID = @ApplicationID) -- Check for valid ApplicationID
			Raiserror('UpdateMembershipApplicationStatus - Invalid ApplicationID.', 16, 1)
		Else
			If @Status Not In ('Pending', 'Approved', 'Rejected') -- Check for valid status value
				Raiserror('UpdateMembershipApplicationStatus - Invalid status value.', 16, 1)
			Else
				Begin -- Update the application status
					Update MembershipApplication
					Set Status = @Status
					Where ApplicationID = @ApplicationID

					If @@Error = 0
						Set @TeeTimeReturnCode = 0 -- Success
					Else
						Raiserror('UpdateMembershipApplicationStatus - Error updating membership application status.', 16, 1)
				End

	Return @TeeTimeReturnCode
GO

Create Procedure UpdateUserRole(
	@Email VarChar(100),
	@NewRoleID Int
)
AS
	Declare @TeeTimeReturnCode Int
	Set @TeeTimeReturnCode = 1 -- Default to failure

	If @Email Is Null Or @NewRoleID Is Null -- Checks if all fields are provided
		Raiserror('UpdateUserRole - All fields must be provided.', 16, 1)
	Else
		If Not Exists (Select 1 From TeeTimeUser Where Email = @Email) -- Check for valid Email
			Raiserror('UpdateUserRole - Invalid Email.', 16, 1)
		Else
			Begin -- Update user's role
				Update TeeTimeUser
				Set RoleID = @NewRoleID
				Where Email = @Email

				If @@Error = 0
					Set @TeeTimeReturnCode = 0 -- Success
				Else
					Raiserror('UpdateUserRole - Error updating user role.', 16, 1)
			End

	Return @TeeTimeReturnCode
GO

-- Insert Data using stored procedures
Exec RegisterUser
	@Email = 'superuser@baist.ca',
	@Password = 'SuperPass123',
	@FirstName = 'Super',
	@LastName = 'User',
	@PhoneNumber = '0123456789',
	@Address = '123 Super St.',
	@City = 'SuperCity',
	@Province = 'SuperProvince',
	@PostalCode = 'A1A1A1',
	@RoleID = 1
GO

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
	@Password = 'StakePass123',
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

Exec RegisterUser
	@Email = 'copper@baist.ca',
	@Password = 'CopperPass123',
	@FirstName = 'Copper',
	@LastName = 'Member',
	@PhoneNumber = '6789012345',
	@Address = '123 Copper St.',
	@City = 'CopperCity',
	@Province = 'CopperProvince',
	@PostalCode = 'F6F6F6',
	@RoleID = 6
GO

Exec RegisterUser
	@Email = 'user@baist.ca',
	@Password = 'UserPass123',
	@FirstName = 'Regular',
	@LastName = 'User',
	@PhoneNumber = '7890123456',
	@Address = null,
	@City = null,
	@Province = null,
	@PostalCode = null,
	@RoleID = 7
GO
