using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecyfikaDotNet.Attributes
{
    // ✅ GOOD: Odczyt i wykorzystanie custom attributes przez Reflection

    // Custom attributes
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; }
        public string Schema { get; set; } = "dbo";

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }
        public string DataType { get; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; } = true;

        public ColumnAttribute(string name, string dataType)
        {
            Name = name;
            DataType = dataType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        public string IndexName { get; }
        public bool IsUnique { get; set; }

        public IndexAttribute(string indexName)
        {
            IndexName = indexName;
        }
    }

    // Model z atrybutami
    [Table("users", Schema = "public")]
    [Description("User entity representing application users")]
    public class GoodUser
    {
        [Column("user_id", "int", IsPrimaryKey = true, IsNullable = false)]
        [Description("User's unique identifier")]
        public int Id { get; set; }

        [Column("email", "varchar(255)", IsNullable = false)]
        [Description("User's email address")]
        [Index("idx_user_email", IsUnique = true)]
        public string Email { get; set; }

        [Column("username", "varchar(100)", IsNullable = false)]
        [Description("User's display name")]
        [Index("idx_user_username")]
        public string Username { get; set; }

        [Column("age", "int", IsNullable = true)]
        [Description("User's age")]
        public int? Age { get; set; }
    }

    // Helper do odczytu atrybutów i generowania SQL
    public static class AttributeHelper
    {
        // Odczyt atrybutu z klasy
        public static T GetClassAttribute<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>();
        }

        // Odczyt atrybutu z property
        public static T GetPropertyAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>();
        }

        // Odczyt wszystkich atrybutów danego typu
        public static T[] GetPropertyAttributes<T>(PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttributes<T>().ToArray();
        }

        // Generowanie SQL CREATE TABLE na podstawie atrybutów
        public static string GenerateCreateTableSql(Type entityType)
        {
            var tableAttr = GetClassAttribute<TableAttribute>(entityType);
            if (tableAttr == null)
            {
                throw new InvalidOperationException($"Type {entityType.Name} does not have TableAttribute");
            }

            var sql = new StringBuilder();
            sql.AppendLine($"CREATE TABLE {tableAttr.Schema}.{tableAttr.Name} (");

            var properties = entityType.GetProperties();
            var columnDefinitions = new List<string>();

            foreach (var property in properties)
            {
                var columnAttr = GetPropertyAttribute<ColumnAttribute>(property);
                if (columnAttr != null)
                {
                    var columnDef = new StringBuilder();
                    columnDef.Append($"    {columnAttr.Name} {columnAttr.DataType}");

                    if (!columnAttr.IsNullable)
                        columnDef.Append(" NOT NULL");

                    if (columnAttr.IsPrimaryKey)
                        columnDef.Append(" PRIMARY KEY");

                    columnDefinitions.Add(columnDef.ToString());
                }
            }

            sql.AppendLine(string.Join(",\n", columnDefinitions));
            sql.AppendLine(");");

            // Generowanie indeksów
            foreach (var property in properties)
            {
                var indexAttr = GetPropertyAttribute<IndexAttribute>(property);
                var columnAttr = GetPropertyAttribute<ColumnAttribute>(property);

                if (indexAttr != null && columnAttr != null)
                {
                    string unique = indexAttr.IsUnique ? "UNIQUE " : "";
                    sql.AppendLine($"CREATE {unique}INDEX {indexAttr.IndexName} ON {tableAttr.Schema}.{tableAttr.Name} ({columnAttr.Name});");
                }
            }

            return sql.ToString();
        }

        // Generowanie dokumentacji na podstawie Description attributes
        public static string GenerateDocumentation(Type entityType)
        {
            var doc = new StringBuilder();
            
            var tableAttr = GetClassAttribute<TableAttribute>(entityType);
            var descAttr = GetClassAttribute<DescriptionAttribute>(entityType);

            doc.AppendLine($"# {entityType.Name}");
            if (descAttr != null)
            {
                doc.AppendLine($"\n{descAttr.Description}");
            }

            if (tableAttr != null)
            {
                doc.AppendLine($"\n**Database:** {tableAttr.Schema}.{tableAttr.Name}");
            }

            doc.AppendLine("\n## Properties");

            foreach (var property in entityType.GetProperties())
            {
                var columnAttr = GetPropertyAttribute<ColumnAttribute>(property);
                var propertyDesc = GetPropertyAttribute<DescriptionAttribute>(property);

                doc.AppendLine($"\n### {property.Name}");
                
                if (propertyDesc != null)
                {
                    doc.AppendLine($"{propertyDesc.Description}");
                }

                if (columnAttr != null)
                {
                    doc.AppendLine($"- **Column:** {columnAttr.Name}");
                    doc.AppendLine($"- **Type:** {columnAttr.DataType}");
                    doc.AppendLine($"- **Primary Key:** {columnAttr.IsPrimaryKey}");
                    doc.AppendLine($"- **Nullable:** {columnAttr.IsNullable}");
                }
            }

            return doc.ToString();
        }

        // Mapowanie obiektu na dictionary (column name -> value)
        public static Dictionary<string, object> MapToColumns<T>(T entity)
        {
            var result = new Dictionary<string, object>();
            var type = typeof(T);

            foreach (var property in type.GetProperties())
            {
                var columnAttr = GetPropertyAttribute<ColumnAttribute>(property);
                if (columnAttr != null)
                {
                    var value = property.GetValue(entity);
                    result[columnAttr.Name] = value;
                }
            }

            return result;
        }
    }

    public class GoodAttributeUsage
    {
        public static void Main()
        {
            Console.WriteLine("=== Custom Attributes with Reflection ===\n");

            var userType = typeof(GoodUser);

            // 1. Generowanie SQL CREATE TABLE
            Console.WriteLine("=== Generated SQL ===");
            string createTableSql = AttributeHelper.GenerateCreateTableSql(userType);
            Console.WriteLine(createTableSql);

            // 2. Generowanie dokumentacji
            Console.WriteLine("=== Generated Documentation ===");
            string documentation = AttributeHelper.GenerateDocumentation(userType);
            Console.WriteLine(documentation);

            // 3. Mapowanie obiektu na kolumny
            Console.WriteLine("\n=== Object to Column Mapping ===");
            var user = new GoodUser
            {
                Id = 1,
                Email = "john@example.com",
                Username = "john_doe",
                Age = 30
            };

            var columnMapping = AttributeHelper.MapToColumns(user);
            foreach (var kvp in columnMapping)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            // 4. Odczyt metadanych w runtime
            Console.WriteLine("\n=== Runtime Metadata ===");
            var tableAttr = AttributeHelper.GetClassAttribute<TableAttribute>(userType);
            Console.WriteLine($"Table name: {tableAttr.Schema}.{tableAttr.Name}");

            var emailProperty = userType.GetProperty("Email");
            var emailColumn = AttributeHelper.GetPropertyAttribute<ColumnAttribute>(emailProperty);
            var emailIndex = AttributeHelper.GetPropertyAttribute<IndexAttribute>(emailProperty);
            
            Console.WriteLine($"\nEmail property:");
            Console.WriteLine($"- Column: {emailColumn.Name}");
            Console.WriteLine($"- Type: {emailColumn.DataType}");
            Console.WriteLine($"- Index: {emailIndex?.IndexName} (Unique: {emailIndex?.IsUnique})");

            // Korzyści:
            // ✅ Automatyczne generowanie SQL DDL
            // ✅ Dokumentacja z kodu
            // ✅ Mapowanie obiektów na DB
            // ✅ Runtime inspection
            // ✅ Reużywalne atrybuty
        }
    }
}
