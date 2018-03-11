using System.Collections.Generic;
using System.Linq;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.Filter;
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
            Assert.Equal("[Id]", queryModel.OrderBy);
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

            Assert.Equal($"[{request.OrderBy}]", queryModel.OrderBy);
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
            // При отсутствии последовательных связей к "дальней" колонки - не создаются джоины
            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
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
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T2]"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T3]"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T4]"));
            Assert.True(queryModel.RootSchema.Join.Any(x => x.Alias == "[T5]"));
        }
    }
}