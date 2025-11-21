// ✅ GOOD: Prawidłowe użycie różnych frameworków testowych (xUnit, NUnit, MSTest)

using NUnit.Framework;
using Xunit;
using FluentAssertions;
using System.Diagnostics;

// ✅ xUnit - nowoczesny framework, preferowany dla nowych projektów
namespace XUnitExamples
{
    // ✅ xUnit używa konstruktora dla Setup i IDisposable dla Cleanup
    public class DatabaseTests : IDisposable
    {
        private readonly Database _database;
        
        // ✅ Konstruktor działa jak [SetUp]
        public DatabaseTests()
        {
            _database = new Database();
            _database.Connect();
        }
        
        [Fact]
        public void Query_ValidSql_ReturnsResults()
        {
            // ✅ _database jest zainicjalizowany w konstruktorze
            var result = _database.Query("SELECT *");
            result.Should().NotBeNull();
        }
        
        // ✅ Dispose działa jak [TearDown]
        public void Dispose()
        {
            _database?.Dispose();
        }
    }
    
    // ✅ Theory w xUnit - parametryzowane testy
    public class CalculatorTests
    {
        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(5, 10, 15)]
        [InlineData(-1, 1, 0)]
        [InlineData(0, 0, 0)]
        public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
        {
            // ✅ Arrange
            var calculator = new Calculator();
            
            // ✅ Act
            var result = calculator.Add(a, b);
            
            // ✅ Assert
            result.Should().Be(expected);
        }
    }
    
    // ✅ xUnit Traits - kategoryzacja testów
    public class CategorizedTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Speed", "Fast")]
        public void FastUnitTest()
        {
            // ✅ Szybki test jednostkowy
            Assert.True(true);
        }
        
        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Speed", "Slow")]
        public void SlowIntegrationTest()
        {
            // ✅ Wolny test integracyjny - wyraźnie oznaczony
            // Możemy uruchomić: dotnet test --filter "Category=Unit"
            Thread.Sleep(100);
            Assert.True(true);
        }
    }
    
    // ✅ Testowanie wyjątków w xUnit
    public class ExceptionTests
    {
        [Fact]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            // ✅ Arrange
            var calculator = new Calculator();
            
            // ✅ Act & Assert
            var exception = Assert.Throws<DivideByZeroException>(
                () => calculator.Divide(10, 0));
            
            // ✅ Możemy sprawdzić message
            exception.Message.Should().Contain("divide by zero");
        }
    }
    
    // ✅ MemberData dla złożonych test cases
    public class ComplexDataTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] { new List<int> { 1, 2, 3 }, 6 };
            yield return new object[] { new List<int> { 10, 20 }, 30 };
            yield return new object[] { new List<int>(), 0 };
        }
        
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Sum_VariousLists_ReturnsCorrectSum(List<int> numbers, int expected)
        {
            // ✅ Arrange
            var calculator = new Calculator();
            
            // ✅ Act
            var result = calculator.Sum(numbers);
            
            // ✅ Assert
            result.Should().Be(expected);
        }
    }
}

// ✅ NUnit - framework z bogatym API, dobry dla legacy projektów
namespace NUnitExamples
{
    [TestFixture]
    public class DatabaseTests
    {
        private Database _database;
        
        // ✅ SetUp wykonywany przed każdym testem
        [SetUp]
        public void Setup()
        {
            _database = new Database();
            _database.Connect();
        }
        
        [Test]
        public void Query_ValidSql_ReturnsResults()
        {
            // ✅ Arrange
            var sql = "SELECT * FROM Users";
            
            // ✅ Act
            var result = _database.Query(sql);
            
            // ✅ Assert - NUnit fluent syntax
            NUnit.Framework.Assert.That(result, Is.Not.Null);
        }
        
        // ✅ TearDown wykonywany po każdym teście
        [TearDown]
        public void Cleanup()
        {
            _database?.Dispose();
        }
        
        // ✅ OneTimeSetUp - wykonywany raz przed wszystkimi testami
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Np. inicjalizacja połączenia do test database
        }
        
        // ✅ OneTimeTearDown - wykonywany raz po wszystkich testach
        [OneTimeTearDown]
        public void OneTimeCleanup()
        {
            // Np. czyszczenie test database
        }
    }
    
    // ✅ TestCase w NUnit - parametryzowane testy
    [TestFixture]
    public class CalculatorTests
    {
        [TestCase(1, 2, 3)]
        [TestCase(5, 10, 15)]
        [TestCase(-1, 1, 0)]
        [TestCase(0, 0, 0)]
        public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
        {
            // ✅ Arrange
            var calculator = new Calculator();
            
            // ✅ Act
            var result = calculator.Add(a, b);
            
            // ✅ Assert
            NUnit.Framework.Assert.That(result, Is.EqualTo(expected));
        }
        
        // ✅ TestCase z Description
        [TestCase(10, 0, TestName = "Divide by zero", 
                  ExpectedResult = typeof(DivideByZeroException))]
        public Type Divide_ByZero_ThrowsException(int a, int b)
        {
            var calculator = new Calculator();
            
            try
            {
                calculator.Divide(a, b);
                return null;
            }
            catch (Exception ex)
            {
                return ex.GetType();
            }
        }
    }
    
    // ✅ Category w NUnit
    [TestFixture]
    [Category("Unit")]
    public class FastTests
    {
        [Test]
        public void FastTest()
        {
            NUnit.Framework.Assert.Pass();
        }
    }
    
    [TestFixture]
    [Category("Integration")]
    [Explicit("Slow test - run explicitly")]
    public class SlowTests
    {
        [Test]
        public void SlowTest()
        {
            Thread.Sleep(1000);
            NUnit.Framework.Assert.Pass();
        }
    }
    
    // ✅ NUnit CollectionAssert
    [TestFixture]
    public class CollectionTests
    {
        [Test]
        public void GetUsers_ReturnsExpectedUsers()
        {
            // ✅ Arrange
            var service = new UserService();
            var expected = new[] { "Alice", "Bob", "Charlie" };
            
            // ✅ Act
            var result = service.GetUserNames();
            
            // ✅ Assert - NUnit CollectionAssert
            CollectionAssert.AreEqual(expected, result);
            CollectionAssert.AllItemsAreNotNull(result);
            CollectionAssert.AllItemsAreUnique(result);
        }
    }
}

// ✅ MSTest - framework od Microsoft, dobry dla projektów Visual Studio
namespace MSTestExamples
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator _calculator;
        
        // ✅ TestInitialize - odpowiednik SetUp
        [TestInitialize]
        public void Setup()
        {
            _calculator = new Calculator();
        }
        
        [TestMethod]
        public void Add_TwoPositiveNumbers_ReturnsSum()
        {
            // ✅ Arrange - już w Setup
            
            // ✅ Act
            var result = _calculator.Add(5, 3);
            
            // ✅ Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(8, result);
        }
        
        // ✅ TestCleanup - odpowiednik TearDown
        [TestCleanup]
        public void Cleanup()
        {
            _calculator = null;
        }
        
        // ✅ ClassInitialize - wykonywany raz przed wszystkimi testami
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // One-time setup
        }
        
        // ✅ ClassCleanup - wykonywany raz po wszystkich testach
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // One-time cleanup
        }
    }
    
    // ✅ DataTestMethod w MSTest - parametryzowane testy
    [TestClass]
    public class ParameterizedTests
    {
        [DataTestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(5, 10, 15)]
        [DataRow(-1, 1, 0)]
        public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
        {
            // ✅ Arrange
            var calculator = new Calculator();
            
            // ✅ Act
            var result = calculator.Add(a, b);
            
            // ✅ Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, result);
        }
    }
    
    // ✅ ExpectedException - nowoczesny sposób
    [TestClass]
    public class ExceptionTests
    {
        [TestMethod]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            // ✅ Arrange
            var calculator = new Calculator();
            
            // ✅ Act & Assert - nowoczesny sposób
            var exception = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsException<DivideByZeroException>(
                () => calculator.Divide(10, 0));
            
            // ✅ Możemy sprawdzić message
            StringAssert.Contains(exception.Message, "divide");
        }
    }
    
    // ✅ TestCategory
    [TestClass]
    [TestCategory("Unit")]
    public class UnitTests
    {
        [TestMethod]
        [TestCategory("Fast")]
        public void FastTest()
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(true);
        }
    }
}

// ✅ Best Practices - niezależnie od frameworka
namespace BestPractices
{
    // ✅ Dobre nazwy testów - Method_Scenario_ExpectedResult
    public class WellNamedTests
    {
        [Fact]
        public void Add_TwoPositiveNumbers_ReturnsPositiveSum()
        {
            var calculator = new Calculator();
            var result = calculator.Add(5, 3);
            result.Should().Be(8);
        }
        
        [Fact]
        public void Add_PositiveAndNegativeNumber_ReturnsCorrectSum()
        {
            var calculator = new Calculator();
            var result = calculator.Add(5, -3);
            result.Should().Be(2);
        }
    }
    
    // ✅ Testy niezależne - każdy test może być uruchomiony osobno
    public class IndependentTests
    {
        [Fact]
        public void CreateUser_ValidData_CreatesUser()
        {
            // ✅ Arrange - własny setup
            var service = new UserService();
            
            // ✅ Act
            var user = service.CreateUser("John", "Doe");
            
            // ✅ Assert
            user.Should().NotBeNull();
            user.Name.Should().Be("John Doe");
        }
        
        [Fact]
        public void UpdateUser_ExistingUser_UpdatesSuccessfully()
        {
            // ✅ Arrange - tworzenie własnego user
            var service = new UserService();
            var user = service.CreateUser("John", "Doe");
            
            // ✅ Act
            user.Name = "Jane Doe";
            service.UpdateUser(user);
            
            // ✅ Assert
            var updated = service.GetUser(user.Id);
            updated.Name.Should().Be("Jane Doe");
        }
    }
    
    // ✅ Assert na końcu, nie ma "maybe" assertion
    public class ProperAssertions
    {
        [Fact]
        public void Process_ValidInput_ProcessesSuccessfully()
        {
            // ✅ Arrange
            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(r => r.Save(It.IsAny<Data>())).Returns(true);
            
            var service = new ProcessingService(mockRepository.Object);
            
            // ✅ Act
            var result = service.Process(new Data());
            
            // ✅ Assert - zawsze sprawdzamy rezultat
            result.Should().BeTrue();
            mockRepository.Verify(r => r.Save(It.IsAny<Data>()), Times.Once);
        }
    }
    
    // ✅ Stabilne testy - bez timing dependencies
    public class StableTests
    {
        [Fact]
        public void GetCurrentTime_ReturnsRecentTime()
        {
            // ✅ Arrange - mockujemy IDateTimeProvider
            var mockTimeProvider = new Mock<IDateTimeProvider>();
            var fixedTime = new DateTime(2024, 1, 1, 12, 0, 0);
            mockTimeProvider.Setup(t => t.Now).Returns(fixedTime);
            
            var service = new TimeService(mockTimeProvider.Object);
            
            // ✅ Act
            var time = service.GetCurrentTime();
            
            // ✅ Assert - stabilny test, nie zależy od rzeczywistego czasu
            time.Should().Be(fixedTime);
        }
    }
}

// Supporting classes
public class Database : IDisposable
{
    public void Connect() { }
    public object Query(string sql) => new { Data = "test" };
    public void Dispose() { }
}

public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Divide(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException("Cannot divide by zero");
        return a / b;
    }
    public int Sum(List<int> numbers) => numbers.Sum();
}

public class UserService
{
    private readonly Dictionary<int, User> _users = new();
    private int _nextId = 1;
    
    public User CreateUser(string firstName, string lastName)
    {
        var user = new User { Id = _nextId++, Name = $"{firstName} {lastName}" };
        _users[user.Id] = user;
        return user;
    }
    
    public void UpdateUser(User user) => _users[user.Id] = user;
    public User GetUser(int id) => _users.TryGetValue(id, out var user) ? user : null;
    public IEnumerable<string> GetUserNames() => new[] { "Alice", "Bob", "Charlie" };
}

public interface IDateTimeProvider
{
    DateTime Now { get; }
}

public class TimeService
{
    private readonly IDateTimeProvider _timeProvider;
    public TimeService(IDateTimeProvider timeProvider) => _timeProvider = timeProvider;
    public DateTime GetCurrentTime() => _timeProvider.Now;
}

public interface IRepository
{
    bool Save(Data data);
}

public class ProcessingService
{
    private readonly IRepository _repository;
    public ProcessingService(IRepository repository) => _repository = repository;
    public bool Process(Data data) => _repository.Save(data);
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Data { }
