use TeeTimeDB
GO

select * from TeeTimeUser
select * from Roles
select * from TeeTimeStart
select * from TeeTimeConfirmation

select * From MembershipApplication

execute GetTeeTimesForUser @Email = 'admin@baist.ca'
