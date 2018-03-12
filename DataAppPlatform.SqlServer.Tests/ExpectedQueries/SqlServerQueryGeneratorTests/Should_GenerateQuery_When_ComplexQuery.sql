SELECT
	[T1].[FirstName] AS 'FirstName',
	[T1].[LastName] AS 'LastName',
	[T2].[FirstName] AS 'Manager.FirstName',
	[T3].[Name] AS 'Manager.Department.Name'
FROM [Contacts] AS [T1]
LEFT JOIN [Contacts] AS [T2] ON [T1].[ManagerId] = [T2].[Id]
LEFT JOIN [Departments] AS [T3] ON [T2].[DepartmentId] = [T3].[Id]
WHERE (([T1].[FirstName] = 'Value1') AND ([T1].[LastName] IS NOT NULL) AND (([T3].[Name] <> 'Value2') AND ([T3].[Title] = 'Company')))
ORDER BY [T1].[FirstName] 
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY