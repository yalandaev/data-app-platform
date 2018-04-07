using System;
using System.Linq;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Entities;
using DataAppPlatform.Entities.Common.Attributes;
using Microsoft.EntityFrameworkCore;

namespace DataAppPlatform.DataAccess
{
    public class SchemaInfoProvider: ISchemaInfoProvider
    {
        public string GetReferenceColumnSchema(string tableName, string referenceField)
        {
            tableName = tableName.Replace("[", string.Empty).Replace("]", string.Empty);
            referenceField = referenceField.Replace("[", string.Empty).Replace("]", string.Empty);

            // Contact
            var tableEntityType = GetTableEntityType(tableName);
            // Contact.Manager
            var referenceFieldProperty = tableEntityType.GetProperty(referenceField);

            if(referenceFieldProperty == null)
                throw new Exception($"Table {tableName} doesn't have property {referenceField}");

            var dbSets = typeof(DataContext).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
            // DbSet<Contact>
            var dbSet = dbSets.FirstOrDefault(x => x.PropertyType.GenericTypeArguments[0] == referenceFieldProperty.PropertyType);

            if(dbSet == null)
                throw new Exception($"There is no table for entity type {referenceFieldProperty.PropertyType.Name}");

            // Contacts
            return dbSet.Name;
        }

        public ColumnType GetColumnType(string tableName, string columnName)
        {
            tableName = tableName.Replace("[", string.Empty).Replace("]", string.Empty);
            columnName = columnName.Replace("[", string.Empty).Replace("]", string.Empty);

            var entityType = GetTableEntityType(tableName);

            var columnPropertyType = entityType.GetProperty(columnName).PropertyType;
            return GetColumnType(columnPropertyType);
        }

        private ColumnType GetColumnType(Type columnPropertyType)
        {
            if (columnPropertyType.IsSubclassOf(typeof(Entity)))
                return ColumnType.Lookup;

            switch (columnPropertyType)
            {
                case Type type when type == typeof(string):
                    return ColumnType.Text;
                case Type type when type == typeof(DateTime):
                    return ColumnType.DateTime;
                case Type type when type == typeof(DateTime?):
                    return ColumnType.DateTime;
                case Type type when type == typeof(int):
                    return ColumnType.Int;
                case Type type when type == typeof(long):
                    return ColumnType.Int;
                case Type type when type == typeof(bool):
                    return ColumnType.Boolean;
                default: return ColumnType.Text;
            }
        }

        public string GetTableDisplayColumn(string tableName)
        {
            tableName = tableName.Replace("[", string.Empty).Replace("]", string.Empty);

            var entityType = GetTableEntityType(tableName);

            return GetEntityDisplayValueColumn(entityType);
        }

        private Type GetTableEntityType(string tableName)
        {
            var dbSet = typeof(DataContext).GetProperty(tableName);

            if (dbSet == null)
                throw new Exception($"Table {tableName} doesn't exists");

            return dbSet.PropertyType.GenericTypeArguments[0];
        }

        private string GetEntityDisplayValueColumn(Type type)
        {
            if (type.GetCustomAttributes(
                typeof(DisplayValueAttribute), true
            ).FirstOrDefault() is DisplayValueAttribute dvAttribute)
            {
                return dvAttribute.Name;
            }
            return null;
        }
    }
}