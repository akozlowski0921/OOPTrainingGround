// ✅ GOOD: Segregowane konteksty, proper memoization, targeted re-renders
import { createContext, useContext, useState, useMemo, useCallback, ReactNode } from 'react';

interface User {
  id: number;
  name: string;
  email: string;
}

// Oddzielne konteksty dla różnych domen
const UserContext = createContext<{
  user: User | null;
  setUser: (user: User | null) => void;
} | null>(null);

const ThemeContext = createContext<{
  theme: 'light' | 'dark';
  setTheme: (theme: 'light' | 'dark') => void;
} | null>(null);

interface Notification {
  id: string;
  message: string;
}

const NotificationsContext = createContext<{
  notifications: Notification[];
  addNotification: (notification: Notification) => void;
  removeNotification: (id: string) => void;
} | null>(null);

// Provider dla użytkownika
export function UserProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);

  // Memoizacja value - zapobiega re-renderom
  const value = useMemo(() => ({ user, setUser }), [user]);

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
}

// Provider dla theme
export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setTheme] = useState<'light' | 'dark'>('light');

  const value = useMemo(() => ({ theme, setTheme }), [theme]);

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

// Provider dla notyfikacji
export function NotificationsProvider({ children }: { children: ReactNode }) {
  const [notifications, setNotifications] = useState<Notification[]>([]);

  const addNotification = useCallback((notification: Notification) => {
    setNotifications(prev => [...prev, notification]);
  }, []);

  const removeNotification = useCallback((id: string) => {
    setNotifications(prev => prev.filter(n => n.id !== id));
  }, []);

  const value = useMemo(
    () => ({ notifications, addNotification, removeNotification }),
    [notifications, addNotification, removeNotification]
  );

  return (
    <NotificationsContext.Provider value={value}>
      {children}
    </NotificationsContext.Provider>
  );
}

// Custom hooks dla wygodnego dostępu
export function useUser() {
  const context = useContext(UserContext);
  if (!context) throw new Error('useUser must be used within UserProvider');
  return context;
}

export function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) throw new Error('useTheme must be used within ThemeProvider');
  return context;
}

export function useNotifications() {
  const context = useContext(NotificationsContext);
  if (!context) throw new Error('useNotifications must be used within NotificationsProvider');
  return context;
}

// Komponenty - rerenderują się tylko gdy zmienia się ich własny context
export function UserProfile() {
  const { user } = useUser(); // Tylko user context
  
  return (
    <div>
      {user ? <p>{user.name}</p> : <p>Not logged in</p>}
    </div>
  );
}

export function ThemeToggle() {
  const { theme, setTheme } = useTheme(); // Tylko theme context
  
  return (
    <button onClick={() => setTheme(theme === 'light' ? 'dark' : 'light')}>
      Toggle Theme (Current: {theme})
    </button>
  );
}
