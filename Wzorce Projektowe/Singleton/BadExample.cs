// ❌ BAD EXAMPLE: Singleton bez thread-safety

public class DatabaseConnection
{
    private static DatabaseConnection _instance;
    private string _connectionString;

    private DatabaseConnection()
    {
        _connectionString = "Server=localhost;Database=MyDb;";
        Console.WriteLine("Database connection initialized");
        // Symulacja kosztownej operacji
        Thread.Sleep(100);
    }

    public static DatabaseConnection Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DatabaseConnection();
            }
            return _instance;
        }
    }

    public void ExecuteQuery(string query)
    {
        Console.WriteLine($"Executing: {query} on {_connectionString}");
    }
}

// Użycie - wygląda OK w single-threaded:
var db1 = DatabaseConnection.Instance;
var db2 = DatabaseConnection.Instance;
Console.WriteLine($"Same instance? {ReferenceEquals(db1, db2)}"); // True

// ALE w multi-threaded środowisku:
var tasks = new Task[10];
for (int i = 0; i < 10; i++)
{
    int taskId = i;
    tasks[i] = Task.Run(() =>
    {
        var instance = DatabaseConnection.Instance;
        Console.WriteLine($"Task {taskId}: Got instance {instance.GetHashCode()}");
    });
}

Task.WaitAll(tasks);

// Problemy:
// 1. RACE CONDITION - wiele wątków może jednocześnie wejść do if (_instance == null)
// 2. Możliwe utworzenie WIELU INSTANCJI zamiast jednej
// 3. Brak gwarancji thread-safety
// 4. Niezdefiniowane zachowanie w środowisku wielowątkowym
// 5. Trudne do wykrycia błędy (działają sporadycznie)

// Scenariusz problemu:
// Thread 1: sprawdza _instance == null (TRUE)
// Thread 2: sprawdza _instance == null (TRUE) - jeszcze nie ustawione!
// Thread 1: tworzy nową instancję
// Thread 2: tworzy KOLEJNĄ nową instancję
// = DWA SINGLETONY! 😱
