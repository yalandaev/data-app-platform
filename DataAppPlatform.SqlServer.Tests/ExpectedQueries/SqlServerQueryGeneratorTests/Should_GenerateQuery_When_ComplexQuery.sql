SELECT
	[T1].[FirstName] AS 'FirstName',
	[T2].[FirstName] AS 'Manager.FirstName'
FROM [Contacts] AS [T1]
INNER JOIN [Contacts] AS [T2] ON [T1].[ManagerId] = [T2].[Id]
ORDER BY [T1].[Id] DESC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY