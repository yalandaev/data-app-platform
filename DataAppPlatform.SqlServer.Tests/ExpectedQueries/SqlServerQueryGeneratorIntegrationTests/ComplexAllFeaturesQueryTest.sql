SELECT
	[T1].[FirstName] AS 'FirstName',
	[T1].[LastName] AS 'LastName',
	[T1].[Age] AS 'Age',
	[T1].[BirthDate] AS 'BirthDate',
	[T2].[FirstName] AS 'Manager.FirstName',
	[T2].[LastName] AS 'Manager.LastName',
	[T4].[FirstName] AS 'Manager.Department.Head.FirstName',
	[T4].[Age] AS 'Manager.Department.Head.Age'
FROM [Contacts] AS [T1]
LEFT JOIN [Contacts] AS [T2] ON [T1].[ManagerId] = [T2].[Id]
LEFT JOIN [Departments] AS [T3] ON [T2].[DepartmentId] = [T3].[Id]
LEFT JOIN [Contacts] AS [T4] ON [T3].[HeadId] = [T4].[Id]
WHERE (([T1].[FirstName] = 'Mark') AND ([T1].[LastName] IS NOT NULL) AND ([T1].[Age] >= [T4].[Age]) AND (([T4].[FirstName] LIKE 'Joe%') OR ([T4].[Age] < 50)))
ORDER BY [T4].[Age] ASC
OFFSET 30 ROWS FETCH NEXT 15 ROWS ONLY