// ❌ BAD: Problemy specyficzne dla różnych frameworków testowych

using NUnit.Framework;
using Xunit;

// ❌ Problem 1: Mieszanie frameworków w jednym projekcie
[TestFixture] // NUnit
public class MixedFrameworkTests
{
    [Fact] // ❌ xUnit w klasie NUnit
    public void Test1()
    {
        Assert.True(true); // xUnit Assert
    }
    
    [Test] // NUnit
    public void Test2()
    {
        NUnit.Framework.Assert.That(true, Is.True); // ❌ NUnit Assert
    }
}

// ❌ Problem 2: Brak Setup/Teardown w xUnit
public class DatabaseTests
{
    private Database _database;
    
    // ❌ xUnit nie ma [SetUp]
    [SetUp] // ❌ To jest NUnit!
    public void Setup()
    {
        _database = new Database();
        _database.Connect();
    }
    
    [Fact]
    public void Query_ValidSql_ReturnsResults()
    {
        // ❌ _database może być null jeśli używamy xUnit
        var result = _database.Query("SELECT *");
        Assert.NotNull(result);
    }
    
    [TearDown] // ❌ To jest NUnit!
    public void Cleanup()
    {
        _database?.Dispose();
    }
}

// ❌ Problem 3: Niewłaściwe użycie MSTest attributes
[TestClass] // MSTest
public class MsTestIssues
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))] // ❌ Przestarzały sposób
    public void Method_InvalidArg_ThrowsException()
    {
        throw new ArgumentException();
        // ❌ Nie możemy sprawdzić message wyjątku
        // ❌ Test przejdzie nawet jeśli wyjątek rzucony w złym miejscu
    }
    
    [TestMethod]
    public void Method_WithMultipleAsserts()
    {
        // ❌ MSTest nie ma natywnego CollectionAssert dla wielu warunków
        var list = new List<int> { 1, 2, 3 };
        
        Assert.IsTrue(list.Count == 3);
        Assert.IsTrue(list.Contains(1));
        Assert.IsTrue(list.Contains(2));
        Assert.IsTrue(list.Contains(3));
        // ❌ Wiele assertów - przy pierwszym failurze reszta się nie wykona
    }
}

// ❌ Problem 4: Nieefektywne parametryzowane testy w NUnit
[TestFixture]
public class ParameterizedTestsIssues
{
    [Test]
    public void Add_FirstCase_ReturnsSum()
    {
        var calculator = new Calculator();
        Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
    }
    
    [Test]
    public void Add_SecondCase_ReturnsSum()
    {
        var calculator = new Calculator();
        Assert.That(calculator.Add(5, 10), Is.EqualTo(15));
    }
    
    [Test]
    public void Add_ThirdCase_ReturnsSum()
    {
        var calculator = new Calculator();
        Assert.That(calculator.Add(-1, 1), Is.EqualTo(0));
    }
    
    // ❌ 3 prawie identyczne testy zamiast jednego parametryzowanego
}

// ❌ Problem 5: Brak kategoryzacji testów
public class UncategorizedTests
{
    [Fact]
    public void FastTest()
    {
        // Szybki test jednostkowy
        Assert.True(true);
    }
    
    [Fact]
    public void SlowDatabaseTest()
    {
        // ❌ Wolny test integracyjny bez oznaczenia
        Thread.Sleep(5000); // Symulacja wolnego testu
        Assert.True(true);
    }
    
    // ❌ Nie możemy uruchomić tylko szybkich testów
}

// ❌ Problem 6: Niewłaściwe użycie TestCase w NUnit
[TestFixture]
public class TestCaseIssues
{
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Process_VariousIds_Succeeds(int id)
    {
        var service = new DataService();
        
        // ❌ Test pobiera dane z prawdziwej bazy dla każdego ID
        var result = service.GetFromDatabase(id);
        
        Assert.That(result, Is.Not.Null);
        // ❌ Test zależy od danych w bazie
    }
}

// ❌ Problem 7: Brak opisowych nazw testów
public class PoorlyNamedTests
{
    [Fact]
    public void Test1() // ❌ Co testuje?
    {
        var result = Calculate(5, 3);
        Assert.Equal(8, result);
    }
    
    [Fact]
    public void Test2() // ❌ Co testuje?
    {
        var result = Calculate(-1, 1);
        Assert.Equal(0, result);
    }
    
    private int Calculate(int a, int b) => a + b;
}

// ❌ Problem 8: Testy zależne od siebie
[TestFixture]
public class DependentTests
{
    private static User _user; // ❌ Shared state
    
    [Test, Order(1)]
    public void CreateUser_ValidData_CreatesUser()
    {
        _user = new User { Id = 1, Name = "John" };
        Assert.That(_user, Is.Not.Null);
    }
    
    [Test, Order(2)]
    public void UpdateUser_ExistingUser_UpdatesSuccessfully()
    {
        // ❌ Zależy od Test1
        _user.Name = "Jane";
        Assert.That(_user.Name, Is.EqualTo("Jane"));
    }
    
    [Test, Order(3)]
    public void DeleteUser_ExistingUser_DeletesSuccessfully()
    {
        // ❌ Zależy od Test1 i Test2
        _user = null;
        Assert.That(_user, Is.Null);
    }
    // ❌ Jeśli Test1 failuje, Test2 i Test3 też failują
}

// ❌ Problem 9: Brak assert w testach
public class MissingAssertTests
{
    [Fact]
    public void Process_ValidInput_NoException()
    {
        var service = new ProcessingService();
        
        // ❌ Act - wywołanie metody
        service.Process("data");
        
        // ❌ Brak assert - test przechodzi jeśli nie ma wyjątku
        // Ale nie sprawdzamy czy coś faktycznie zostało zrobione
    }
}

// ❌ Problem 10: Testy które czasem przechodzą, czasem nie (flaky tests)
public class FlakyTests
{
    [Fact]
    public void GetCurrentTime_ReturnsRecentTime()
    {
        var service = new TimeService();
        
        var time1 = service.GetCurrentTime();
        Thread.Sleep(10); // ❌ Timing dependency
        var time2 = service.GetCurrentTime();
        
        // ❌ Test może failować w zależności od timing
        Assert.True(time2 > time1);
        Assert.True((time2 - time1).TotalMilliseconds < 50); // ❌ Może failować na wolnej maszynie
    }
}

// Supporting classes
public class Database : IDisposable
{
    public void Connect() { }
    public object Query(string sql) => new object();
    public void Dispose() { }
}

public class Calculator
{
    public int Add(int a, int b) => a + b;
}

public class DataService
{
    public object GetFromDatabase(int id) => new object();
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ProcessingService
{
    public void Process(string data) { }
}

public class TimeService
{
    public DateTime GetCurrentTime() => DateTime.Now;
}
