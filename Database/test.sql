use TeeTimeDB
GO

select * from TeeTimeUser
select * from Roles
select * from TeeTimeStart
select * from TeeTimeConfirmation

execute GetTeeTimesForUser @Email = 'admin@baist.ca'
