SELECT
	[T1].[FirstName] AS 'FirstName.value',
	[T1].[LastName] AS 'LastName.value',
	[T1].[ManagerId] AS 'Manager.value',
	[T1].[DepartmentId] AS 'Department.value',
	[T2].[FullName] AS 'Manager.displayValue',
	[T3].[Title] AS 'Department.displayValue'
FROM [Contacts] AS [T1]
LEFT JOIN [Contacts] AS [T2] ON [T1].[ManagerId] = [T2].[Id]
LEFT JOIN [Departments] AS [T3] ON [T1].[DepartmentId] = [T3].[Id]
WHERE (([T1].[Id] = 1))
ORDER BY [T1].[Id] DESC
OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY