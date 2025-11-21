using System;

namespace SpecyfikaDotNet.RecordsVsClasses.Bad3
{
    // ❌ BAD: Ręczna implementacja Equals, GetHashCode, ToString dla value object
    public class BadPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        // Próba ręcznej implementacji równości - podatna na błędy
        public override bool Equals(object obj)
        {
            if (obj is BadPoint other)
            {
                // BŁĄD: Zapomnieliśmy sprawdzić Y!
                return X == other.X;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // BŁĄD: Niekonsekwentne z Equals (Y nie jest uwzględnione)
            return X.GetHashCode();
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        // BŁĄD: Zapomnieliśmy override operator ==
        // Więc == porównuje referencje, ale Equals porównuje wartości!
    }

    public class BadConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }

        // Wiele boilerplate code dla prostego value object
        public override bool Equals(object obj)
        {
            if (obj is BadConfiguration other)
            {
                return Server == other.Server &&
                       Port == other.Port &&
                       Database == other.Database &&
                       Username == other.Username;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Server, Port, Database, Username);
        }

        public override string ToString()
        {
            return $"{Server}:{Port}/{Database} (User: {Username})";
        }

        public BadConfiguration Clone()
        {
            return new BadConfiguration
            {
                Server = Server,
                Port = Port,
                Database = Database,
                Username = Username
            };
        }
    }
}
