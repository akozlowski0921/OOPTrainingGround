using System;

namespace SpecyfikaDotNet.Attributes
{
    // ❌ BAD: Ignorowanie custom attributes - strata metadanych
    
    // Custom attribute, który jest zdefiniowany ale nigdy nie używany
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
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }
        public string DataType { get; }

        public ColumnAttribute(string name, string dataType)
        {
            Name = name;
            DataType = dataType;
        }
    }

    [Description("User entity")]
    public class BadUser
    {
        [Description("User's unique identifier")]
        [Column("user_id", "int")]
        public int Id { get; set; }

        [Description("User's email address")]
        [Column("email", "varchar(255)")]
        public string Email { get; set; }

        [Description("User's display name")]
        [Column("username", "varchar(100)")]
        public string Username { get; set; }
    }

    public class BadAttributeUsage
    {
        public static void Main()
        {
            var user = new BadUser
            {
                Id = 1,
                Email = "user@example.com",
                Username = "john_doe"
            };

            // Atrybuty są zdefiniowane, ale nie są używane!
            // Nie ma kodu, który by je odczytywał
            // Informacje są tracone

            Console.WriteLine("User created:");
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Username: {user.Username}");

            // Brak możliwości:
            // - Wygenerowania SQL DDL na podstawie atrybutów
            // - Automatycznego mapowania na kolumny DB
            // - Generowania dokumentacji z Description
            // - Runtime inspection metadanych

            // Atrybuty są bezużyteczne bez refleksji!
        }
    }
}
