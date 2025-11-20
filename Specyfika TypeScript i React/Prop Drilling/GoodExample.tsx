import React, { useState, createContext, useContext } from 'react';

interface Theme {
  primaryColor: string;
  backgroundColor: string;
}

interface User {
  name: string;
  email: string;
}

// Rozwiązanie 1: Context API dla globalnego stanu
const ThemeContext = createContext<{
  theme: Theme;
  setTheme: (theme: Theme) => void;
} | undefined>(undefined);

const UserContext = createContext<User | undefined>(undefined);

// Custom hooki dla łatwiejszego dostępu
function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) throw new Error('useTheme must be used within ThemeProvider');
  return context;
}

function useUser() {
  const context = useContext(UserContext);
  if (!context) throw new Error('useUser must be used within UserProvider');
  return context;
}

export function App() {
  const [theme, setTheme] = useState<Theme>({
    primaryColor: '#007bff',
    backgroundColor: '#ffffff'
  });
  
  const [user] = useState<User>({
    name: 'Jan Kowalski',
    email: 'jan@example.com'
  });

  return (
    <ThemeContext.Provider value={{ theme, setTheme }}>
      <UserContext.Provider value={user}>
        <Layout />
      </UserContext.Provider>
    </ThemeContext.Provider>
  );
}

// Komponenty pośrednie nie muszą znać props
function Layout() {
  const { theme } = useTheme();
  
  return (
    <div style={{ backgroundColor: theme.backgroundColor }}>
      <Header />
      <MainContent />
      <Footer />
    </div>
  );
}

function Header() {
  return (
    <header>
      <Navigation />
      <ThemeToggle />
    </header>
  );
}

function Navigation() {
  const { theme } = useTheme();
  
  return (
    <nav style={{ color: theme.primaryColor }}>
      <UserMenu />
    </nav>
  );
}

// Komponenty konsumujące dane używają hooków bezpośrednio
function UserMenu() {
  const user = useUser();
  const { theme } = useTheme();
  
  return (
    <div style={{ color: theme.primaryColor }}>
      <span>{user.name}</span>
      <span>{user.email}</span>
    </div>
  );
}

function ThemeToggle() {
  const { theme, setTheme } = useTheme();
  
  return (
    <button onClick={() => setTheme({
      ...theme,
      primaryColor: theme.primaryColor === '#007bff' ? '#28a745' : '#007bff'
    })}>
      Toggle Theme
    </button>
  );
}

function MainContent() {
  return (
    <main>
      <Article />
    </main>
  );
}

function Article() {
  const user = useUser();
  const { theme } = useTheme();
  
  return (
    <article style={{ color: theme.primaryColor }}>
      <h1>Article by {user.name}</h1>
    </article>
  );
}

function Footer() {
  const { theme } = useTheme();
  
  return (
    <footer style={{ color: theme.primaryColor }}>
      Footer content
    </footer>
  );
}

// Rozwiązanie 2: Component Composition - dla prostszych przypadków
export function AlternativeApp() {
  const [theme, setTheme] = useState<Theme>({
    primaryColor: '#007bff',
    backgroundColor: '#ffffff'
  });

  // Zamiast przekazywać props przez wiele poziomów,
  // komponujemy komponenty bezpośrednio tam gdzie są potrzebne
  return (
    <div style={{ backgroundColor: theme.backgroundColor }}>
      <header>
        <nav>
          <ThemeButton theme={theme} onThemeChange={setTheme} />
        </nav>
      </header>
    </div>
  );
}

function ThemeButton({ theme, onThemeChange }: {
  theme: Theme;
  onThemeChange: (theme: Theme) => void;
}) {
  return (
    <button 
      style={{ backgroundColor: theme.primaryColor }}
      onClick={() => onThemeChange({
        ...theme,
        primaryColor: theme.primaryColor === '#007bff' ? '#28a745' : '#007bff'
      })}
    >
      Toggle
    </button>
  );
}
