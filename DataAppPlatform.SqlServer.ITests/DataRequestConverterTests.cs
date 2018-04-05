using System;
using System.Collections.Generic;
using System.Linq;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.Filter;
using DataAppPlatform.Core.DataService.Models.TableData;
using Moq;
using Xunit;

namespace DataAppPlatform.DataServices.Tests
{
    public class DataRequestConverterTests
    {
        [Fact]
        public void Should_GenerateQueryModel()
        {
            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn()
                    {
                        DisplayName = "First name",
                        Name = "FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Last name",
                        Name = "LastName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10
            };

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            QueryModel queryModel = dataRequestConverter.GetQueryModel(request);

            Assert.NotNull(queryModel);
            Assert.Equal($"[{request.EntitySchema}]", queryModel.RootSchema.TableName);
            Assert.Equal("[T1]", queryModel.RootSchema.Alias);
            Assert.Equal(string.Empty, queryModel.RootSchema.ReferenceName);
            Assert.Equal("[T1].[Id]", queryModel.OrderBy);
            Assert.Equal(Sort.DESC.ToString(), queryModel.Sort);
            Assert.Equal(0, queryModel.Offset);
            Assert.Equal(10, queryModel.Fetch);
            Assert.Equal(2, queryModel.RootSchema.Columns.Count);
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Name == "[FirstName]"));
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Name == "[LastName]"));

        }

        [Fact]
        public void Should_GenerateQueryModel_When_PagingSortOrderSpecified()
        {
            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn()
                    {
                        DisplayName = "First name",
                        Name = "FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 3,
                PageSize = 15,
                Sort = Sort.ASC,
                OrderBy = "FirstName"
            };

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            QueryModel queryModel = dataRequestConverter.GetQueryModel(request);

            Assert.NotNull(queryModel);

            Assert.Equal($"[T1].[{request.OrderBy}]", queryModel.OrderBy);
            Assert.Equal(Sort.ASC.ToString(), queryModel.Sort);
            Assert.Equal(30, queryModel.Offset);
            Assert.Equal(15, queryModel.Fetch);
        }

        [Fact]
        public void Should_GenerateQueryModel_When_ComplexRequest()
        {
            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn()
                    {
                        DisplayName = "First name",
                        Name = "FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Manager",
                        Name = "Manager.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Manager",
                        Name = "Manager.LastName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Department",
                        Name = "Department.Name",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Department Head FirstName",
                        Name = "Department.Head.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Department Head LastName",
                        Name = "Department.Head.LastName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Department Manager Head FirstName",
                        Name = "Department.Head.Manager.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10,
                OrderBy = "Department.Head.Manager.FirstName"
            };

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Manager"))).Returns("Contacts");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Department"))).Returns("Departments");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Departments]"), It.Is<string>(s => s == "Head"))).Returns("Contacts");
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            QueryModel queryModel = dataRequestConverter.GetQueryModel(request);

            Assert.NotNull(queryModel);

            Assert.Equal("[T1]", queryModel.RootSchema.Alias);
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Name == "[FirstName]"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.TableName == "[Contacts]" && x.ReferenceName == "Manager"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.TableName == "[Departments]" && x.ReferenceName == "Department"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T2]"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T3]"));
            Assert.True(queryModel.RootSchema.Join.All(x => x.Parent == queryModel.RootSchema));

            Assert.Equal(2, queryModel.RootSchema.Join
                .FirstOrDefault(x => x.ReferenceName == "Manager")
                .Columns.Count);
            Assert.True(queryModel.RootSchema.Join
                .FirstOrDefault(x => x.ReferenceName == "Manager")
                .Columns.Any(x => x.Name == "[FirstName]"));
            Assert.True(queryModel.RootSchema.Join
                .FirstOrDefault(x => x.ReferenceName == "Manager")
                .Columns.Any(x => x.Name == "[LastName]"));

            Assert.True(queryModel.RootSchema.Join
                .FirstOrDefault(x => x.ReferenceName == "Department")
                .Join
                .Any(x => x.TableName == "[Contacts]" && x.ReferenceName == "Head"));
        }

        [Fact]
        public void Should_GenerateQueryModel_When_ComplexRequestWithoutTableChain()
        {
            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn()
                    {
                        DisplayName = "Department Head First Name",
                        Name = "Department.Head.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Department Head Last Name",
                        Name = "Department.Head.LastName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Department Manager Head First Name",
                        Name = "Department.Head.Manager.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10,
                OrderBy = "Department.Head.Manager.FirstName"
            };

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Manager"))).Returns("Contacts");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Department"))).Returns("Departments");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Departments]"), It.Is<string>(s => s == "Head"))).Returns("Contacts");
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            QueryModel queryModel = dataRequestConverter.GetQueryModel(request);

            Assert.NotNull(queryModel);

            Assert.Equal("[T1]", queryModel.RootSchema.Alias);
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T2]"));
            Assert.True(queryModel.RootSchema.Join[0].Join.Any(x => x.Alias == "[T3]"));
            Assert.True(queryModel.RootSchema.Join[0].Join[0].Join.Any(x => x.Alias == "[T4]"));
        }

        [Fact]
        public void Should_GenerateQueryModel_When_JoinOnlyInFilter()
        {
            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn()
                    {
                        DisplayName = "FirstName",
                        Name = "FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                Filter = new FilterGroup()
                {
                    LogicalOperation = LogicalOperation.AND,
                    Conditions = new List<Condition>()
                    {
                        new Condition()
                        {
                            Column = "Manager.Department.Head.FirstName",
                            ComparisonType = ComparisonType.Equals,
                            Value = "SomeValue"
                        }
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10
            };

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Manager"))).Returns("Contacts");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Department"))).Returns("Departments");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Departments]"), It.Is<string>(s => s == "Head"))).Returns("Contacts");
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            QueryModel queryModel = dataRequestConverter.GetQueryModel(request);

            Assert.NotNull(queryModel);

            Assert.Equal("[T1]", queryModel.RootSchema.Alias);
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T2]" && x.JoinPath == "Manager"));
            Assert.True(queryModel.RootSchema.Join[0].Join.Any(x => x.Alias == "[T3]" && x.JoinPath == "Manager.Department"));
            Assert.True(queryModel.RootSchema.Join[0].Join[0].Join.Any(x => x.Alias == "[T4]" && x.JoinPath == "Manager.Department.Head"));
        }

        [Fact]
        public void Should_GenerateQueryModel_When_EntityDataRequestPassed()
        {
            EntityDataRequest request = new EntityDataRequest()
            {
                EntitySchema = "Contacts",
                EntityId = 1,
                Columns = new List<string>()
                {
                    "FirstName",
                    "LastName",
                    "Manager",
                    "Department"
                }
            };

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]" || s == "Contacts"), It.Is<string>(s => s == "Manager"))).Returns("Contacts");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]" || s == "Contacts"), It.Is<string>(s => s == "Department"))).Returns("Departments");
            mockSchemaInfoProvider.Setup(x => x.GetTableDisplayColumn(It.Is<string>(s => s == "Contacts"))).Returns("FullName");
            mockSchemaInfoProvider.Setup(x => x.GetTableDisplayColumn(It.Is<string>(s => s == "Departments"))).Returns("Title");
            mockSchemaInfoProvider.Setup(x => x.GetColumnType(It.Is<string>(s => s == "Contacts"), It.Is<string>(s => s == "Manager" || s == "[Manager]"))).Returns(ColumnType.Lookup);
            mockSchemaInfoProvider.Setup(x => x.GetColumnType(It.Is<string>(s => s == "Contacts"), It.Is<string>(s => s == "Department" || s == "[Department]"))).Returns(ColumnType.Lookup);
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            QueryModel queryModel = dataRequestConverter.GetQueryModel(request);

            Assert.NotNull(queryModel);
            Assert.Equal("[T1]", queryModel.RootSchema.Alias);
            Assert.NotNull((queryModel.Filter.Conditions.FirstOrDefault()));
            Assert.True(Convert.ToInt32(queryModel.Filter.Conditions[0].Value) == 1);
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T2]"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T3]"));
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Alias == "FirstName.value" && x.Name == "[FirstName]"));
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Alias == "LastName.value" && x.Name == "[LastName]"));
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Alias == "Manager.value" && x.Name == "[ManagerId]"));
            Assert.True(queryModel.RootSchema.Join.First(x => x.ReferenceName == "Manager").Columns.Any(x => x.Alias == "Manager.displayValue" && x.Name == "[FullName]"));
            Assert.True(queryModel.RootSchema.Columns.Any(x => x.Alias == "Department.value" && x.Name == "[DepartmentId]"));
            Assert.True(queryModel.RootSchema.Join.First(x => x.ReferenceName == "Department").Columns.Any(x => x.Alias == "Department.displayValue" && x.Name == "[Title]"));
            
        }

        [Fact]
        public void Should_ReplaceLookupFields()
        {
            EntityDataUpdateRequest request = new EntityDataUpdateRequest()
            {
                EntitySchema = "Contacts",
                EntityId = 50,
                Fields = new Dictionary<string, EntityDataFieldUpdate>()
            };
            request.Fields.Add("Manager", new EntityDataFieldUpdate()
            {
                Value = 12467
            });
            request.Fields.Add("Department", new EntityDataFieldUpdate()
            {
                Value = 4654
            });

            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]" || s == "Contacts"), It.Is<string>(s => s == "Manager"))).Returns("Contacts");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]" || s == "Contacts"), It.Is<string>(s => s == "Department"))).Returns("Departments");
            mockSchemaInfoProvider.Setup(x => x.GetTableDisplayColumn(It.Is<string>(s => s == "Contacts"))).Returns("FullName");
            mockSchemaInfoProvider.Setup(x => x.GetTableDisplayColumn(It.Is<string>(s => s == "Departments"))).Returns("Title");
            mockSchemaInfoProvider.Setup(x => x.GetColumnType(It.Is<string>(s => s == "Contacts"), It.Is<string>(s => s == "Manager" || s == "[Manager]"))).Returns(ColumnType.Lookup);
            mockSchemaInfoProvider.Setup(x => x.GetColumnType(It.Is<string>(s => s == "Contacts"), It.Is<string>(s => s == "Department" || s == "[Department]"))).Returns(ColumnType.Lookup);
            IDataRequestConverter dataRequestConverter = new DataRequestConverter(mockSchemaInfoProvider.Object);

            request = dataRequestConverter.ReplaceLookupFields(request);

            Assert.NotNull(request);
            Assert.True(request.Fields.ContainsKey("ManagerId"));
            Assert.False(request.Fields.ContainsKey("Manager"));
            Assert.True(request.Fields.ContainsKey("DepartmentId"));
            Assert.False(request.Fields.ContainsKey("Department"));

        }
    }
}