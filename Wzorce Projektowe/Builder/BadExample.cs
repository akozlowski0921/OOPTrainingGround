// ❌ BAD EXAMPLE: Konstruktor z wieloma parametrami

public class UserProfile
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Bio { get; set; }

    public UserProfile(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string address,
        string city,
        string zipCode,
        string country,
        DateTime? dateOfBirth,
        string bio)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        ZipCode = zipCode;
        Country = country;
        DateOfBirth = dateOfBirth;
        Bio = bio;
    }
}

// Użycie:
var user1 = new UserProfile(
    "Jan",
    "Kowalski",
    "jan@example.com",
    "+48123456789",
    "ul. Przykładowa 1",
    "Warszawa",
    "00-001",
    "Polska",
    new DateTime(1990, 5, 15),
    "Software developer"
);

// Problemy:
// 1. Nieczytelne wywołanie - trudno określić który parametr jest który
// 2. Łatwo pomylić kolejność parametrów (szczególnie tego samego typu)
// 3. Brak elastyczności - trzeba podać wszystkie parametry nawet gdy nie są wymagane
// 4. Niemożność walidacji w trakcie budowania
// 5. Co jeśli potrzebujemy tylko części danych?

// Próba rozwiązania przez przeciążenia konstruktora:
var user2 = new UserProfile(
    "Anna",
    "Nowak",
    "anna@example.com",
    null,  // phoneNumber
    null,  // address
    null,  // city
    null,  // zipCode
    null,  // country
    null,  // dateOfBirth
    null   // bio
);
// To nie rozwiązuje problemu - wciąż nieczytelne i podatne na błędy!
