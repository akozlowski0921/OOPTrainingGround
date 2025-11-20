// ✅ GOOD EXAMPLE: Fluent Builder Pattern

public class UserProfile
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string Bio { get; private set; }

    private UserProfile() { }

    public class Builder
    {
        private readonly UserProfile _profile = new UserProfile();

        public Builder WithName(string firstName, string lastName)
        {
            _profile.FirstName = firstName;
            _profile.LastName = lastName;
            return this;
        }

        public Builder WithEmail(string email)
        {
            _profile.Email = email;
            return this;
        }

        public Builder WithPhone(string phoneNumber)
        {
            _profile.PhoneNumber = phoneNumber;
            return this;
        }

        public Builder WithAddress(string address, string city, string zipCode, string country)
        {
            _profile.Address = address;
            _profile.City = city;
            _profile.ZipCode = zipCode;
            _profile.Country = country;
            return this;
        }

        public Builder WithDateOfBirth(DateTime dateOfBirth)
        {
            _profile.DateOfBirth = dateOfBirth;
            return this;
        }

        public Builder WithBio(string bio)
        {
            _profile.Bio = bio;
            return this;
        }

        public UserProfile Build()
        {
            // Walidacja przed utworzeniem obiektu
            if (string.IsNullOrEmpty(_profile.FirstName))
                throw new InvalidOperationException("FirstName is required");
            if (string.IsNullOrEmpty(_profile.LastName))
                throw new InvalidOperationException("LastName is required");
            if (string.IsNullOrEmpty(_profile.Email))
                throw new InvalidOperationException("Email is required");

            return _profile;
        }
    }
}

// Użycie - pełny profil:
var user1 = new UserProfile.Builder()
    .WithName("Jan", "Kowalski")
    .WithEmail("jan@example.com")
    .WithPhone("+48123456789")
    .WithAddress("ul. Przykładowa 1", "Warszawa", "00-001", "Polska")
    .WithDateOfBirth(new DateTime(1990, 5, 15))
    .WithBio("Software developer")
    .Build();

// Użycie - minimalny profil (tylko wymagane pola):
var user2 = new UserProfile.Builder()
    .WithName("Anna", "Nowak")
    .WithEmail("anna@example.com")
    .Build();

// Użycie - częściowy profil:
var user3 = new UserProfile.Builder()
    .WithName("Piotr", "Wiśniewski")
    .WithEmail("piotr@example.com")
    .WithPhone("+48987654321")
    .Build();

// Korzyści:
// 1. Bardzo czytelne wywołanie - każda metoda jasno mówi co ustawia
// 2. Elastyczność - podajemy tylko potrzebne dane
// 3. Niemożliwość pomyłki w kolejności parametrów
// 4. Walidacja w metodzie Build() przed utworzeniem obiektu
// 5. Fluent interface poprawia developer experience
// 6. Immutability - private settery w klasie głównej
