SELECT
	[T1].[FirstName] AS 'FirstName'
FROM [Contacts] AS [T1]
LEFT JOIN [Contacts] AS [T2] ON [T1].[ManagerId] = [T2].[Id]
LEFT JOIN [Departments] AS [T3] ON [T2].[DepartmentId] = [T3].[Id]
LEFT JOIN [Contacts] AS [T4] ON [T3].[HeadId] = [T4].[Id]
WHERE (([T4].[Age] = 20))
ORDER BY [T1].[Id] ASC
OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY