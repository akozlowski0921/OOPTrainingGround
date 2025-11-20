import React, { useState } from 'react';

interface Theme {
  primaryColor: string;
  backgroundColor: string;
}

interface User {
  name: string;
  email: string;
}

// Problem: Props przekazywane przez wiele poziomów komponentów (prop drilling)
export function App() {
  const [theme, setTheme] = useState<Theme>({
    primaryColor: '#007bff',
    backgroundColor: '#ffffff'
  });
  
  const [user, setUser] = useState<User>({
    name: 'Jan Kowalski',
    email: 'jan@example.com'
  });

  // Layout musi przyjąć theme i user, aby przekazać je dalej
  return <Layout theme={theme} user={user} onThemeChange={setTheme} />;
}

function Layout({ theme, user, onThemeChange }: { 
  theme: Theme; 
  user: User; 
  onThemeChange: (theme: Theme) => void;
}) {
  // Layout nie używa tych props, tylko przekazuje je dalej
  return (
    <div style={{ backgroundColor: theme.backgroundColor }}>
      <Header theme={theme} user={user} onThemeChange={onThemeChange} />
      <MainContent theme={theme} user={user} />
      <Footer theme={theme} />
    </div>
  );
}

function Header({ theme, user, onThemeChange }: { 
  theme: Theme; 
  user: User; 
  onThemeChange: (theme: Theme) => void;
}) {
  // Header też tylko przekazuje dalej
  return (
    <header>
      <Navigation theme={theme} user={user} />
      <ThemeToggle theme={theme} onThemeChange={onThemeChange} />
    </header>
  );
}

function Navigation({ theme, user }: { theme: Theme; user: User }) {
  // Navigation też przekazuje dalej
  return (
    <nav style={{ color: theme.primaryColor }}>
      <UserMenu user={user} theme={theme} />
    </nav>
  );
}

function UserMenu({ user, theme }: { user: User; theme: Theme }) {
  // Dopiero tutaj faktycznie używamy user i theme
  return (
    <div style={{ color: theme.primaryColor }}>
      <span>{user.name}</span>
      <span>{user.email}</span>
    </div>
  );
}

function ThemeToggle({ theme, onThemeChange }: { 
  theme: Theme; 
  onThemeChange: (theme: Theme) => void;
}) {
  // Tutaj używamy theme i onThemeChange
  return (
    <button onClick={() => onThemeChange({
      ...theme,
      primaryColor: theme.primaryColor === '#007bff' ? '#28a745' : '#007bff'
    })}>
      Toggle Theme
    </button>
  );
}

function MainContent({ theme, user }: { theme: Theme; user: User }) {
  // Przekazujemy dalej
  return (
    <main>
      <Article theme={theme} user={user} />
    </main>
  );
}

function Article({ theme, user }: { theme: Theme; user: User }) {
  // Wreszcie używamy
  return (
    <article style={{ color: theme.primaryColor }}>
      <h1>Article by {user.name}</h1>
    </article>
  );
}

function Footer({ theme }: { theme: Theme }) {
  return (
    <footer style={{ color: theme.primaryColor }}>
      Footer content
    </footer>
  );
}
