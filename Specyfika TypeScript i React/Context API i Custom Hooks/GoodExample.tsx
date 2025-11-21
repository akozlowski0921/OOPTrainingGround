import React, { createContext, useContext, useState, useMemo, ReactNode, useCallback } from 'react';

// ✅ GOOD: Segregowane konteksty + custom hooks

// ✅ 1. User Context - tylko user-related data
interface User {
  id: number;
  name: string;
  email: string;
}

interface UserContextType {
  user: User | null;
  setUser: (user: User | null) => void;
  isLoggedIn: boolean;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export function UserProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  
  // ✅ useMemo - stabilna referencja, re-render tylko gdy user się zmienia
  const value = useMemo(() => ({
    user,
    setUser,
    isLoggedIn: user !== null
  }), [user]);

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
}

// ✅ Custom hook - reusable i type-safe
export function useUser() {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUser must be used within UserProvider');
  }
  return context;
}

// ✅ 2. Theme Context - oddzielny od User
interface ThemeContextType {
  theme: 'light' | 'dark';
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setTheme] = useState<'light' | 'dark'>('light');
  
  // ✅ useCallback - stabilna referencja funkcji
  const toggleTheme = useCallback(() => {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  }, []);
  
  const value = useMemo(() => ({
    theme,
    toggleTheme
  }), [theme, toggleTheme]);

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

export function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within ThemeProvider');
  }
  return context;
}

// ✅ 3. Notifications Context - izolowany state
interface NotificationsContextType {
  notifications: string[];
  addNotification: (message: string) => void;
  clearNotifications: () => void;
}

const NotificationsContext = createContext<NotificationsContextType | undefined>(undefined);

export function NotificationsProvider({ children }: { children: ReactNode }) {
  const [notifications, setNotifications] = useState<string[]>([]);
  
  const addNotification = useCallback((message: string) => {
    setNotifications(prev => [...prev, message]);
  }, []);
  
  const clearNotifications = useCallback(() => {
    setNotifications([]);
  }, []);
  
  const value = useMemo(() => ({
    notifications,
    addNotification,
    clearNotifications
  }), [notifications, addNotification, clearNotifications]);

  return <NotificationsContext.Provider value={value}>{children}</NotificationsContext.Provider>;
}

export function useNotifications() {
  const context = useContext(NotificationsContext);
  if (!context) {
    throw new Error('useNotifications must be used within NotificationsProvider');
  }
  return context;
}

// ✅ Komponenty używają tylko tego co potrzebują
export function GoodUserProfile() {
  const { user } = useUser(); // ✅ Tylko User context
  
  // ✅ Re-renderuje się TYLKO gdy user się zmienia
  return <div>User: {user?.name || 'Guest'}</div>;
}

export function GoodThemeToggle() {
  const { theme, toggleTheme } = useTheme(); // ✅ Tylko Theme context
  
  // ✅ Re-renderuje się TYLKO gdy theme się zmienia
  return (
    <button onClick={toggleTheme}>
      Toggle Theme: {theme}
    </button>
  );
}

export function GoodNotificationBell() {
  const { notifications } = useNotifications(); // ✅ Tylko Notifications context
  
  // ✅ Re-renderuje się TYLKO gdy notifications się zmieniają
  return <div>Notifications: {notifications.length}</div>;
}

// ✅ 4. Custom hooks dla złożonej logiki
export function useAuth() {
  const { user, setUser, isLoggedIn } = useUser();
  
  const login = useCallback(async (email: string, password: string) => {
    // Logika logowania
    const user = { id: 1, name: 'John', email };
    setUser(user);
  }, [setUser]);
  
  const logout = useCallback(() => {
    setUser(null);
  }, [setUser]);
  
  return { user, isLoggedIn, login, logout };
}

export function useThemePreference() {
  const { theme, toggleTheme } = useTheme();
  
  const isDarkMode = theme === 'dark';
  const themeIcon = isDarkMode ? '🌙' : '☀️';
  
  return { isDarkMode, themeIcon, toggleTheme };
}

// ✅ Komponenty z custom hooks - czysty i reusable kod
export function GoodComplexComponent() {
  const { user, isLoggedIn } = useAuth();
  const { themeIcon } = useThemePreference();
  
  return (
    <div>
      {isLoggedIn ? `Welcome ${user?.name}` : 'Please login'}
      <span>Theme: {themeIcon}</span>
    </div>
  );
}

// ✅ 5. Composed providers - ładne zagnieżdżenie
export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <UserProvider>
      <ThemeProvider>
        <NotificationsProvider>
          {children}
        </NotificationsProvider>
      </ThemeProvider>
    </UserProvider>
  );
}

// ✅ 6. Context z default value - bezpieczniejsze
interface SafeContextType {
  value: string;
  setValue: (v: string) => void;
}

const defaultValue: SafeContextType = {
  value: '',
  setValue: () => console.warn('setValue called outside provider')
};

const SafeContext = createContext<SafeContextType>(defaultValue);

// ✅ 7. Selector pattern dla partial updates
interface StoreState {
  user: User | null;
  theme: 'light' | 'dark';
  count: number;
}

function useStore<T>(selector: (state: StoreState) => T): T {
  // Implementacja z state management library
  // Komponent re-renderuje się TYLKO gdy selected value się zmienia
  return selector({ user: null, theme: 'light', count: 0 });
}

export function OptimizedComponent() {
  // ✅ Re-renderuje TYLKO gdy user.name się zmienia
  const userName = useStore(state => state.user?.name);
  
  return <div>{userName}</div>;
}

// ✅ 8. Context dla API calls
interface ApiContextType {
  fetchUser: (id: number) => Promise<User>;
  fetchPosts: () => Promise<any[]>;
}

const ApiContext = createContext<ApiContextType | undefined>(undefined);

export function ApiProvider({ children, baseUrl }: { children: ReactNode; baseUrl: string }) {
  const fetchUser = useCallback(async (id: number) => {
    const response = await fetch(`${baseUrl}/users/${id}`);
    return response.json();
  }, [baseUrl]);
  
  const fetchPosts = useCallback(async () => {
    const response = await fetch(`${baseUrl}/posts`);
    return response.json();
  }, [baseUrl]);
  
  const value = useMemo(() => ({
    fetchUser,
    fetchPosts
  }), [fetchUser, fetchPosts]);

  return <ApiContext.Provider value={value}>{children}</ApiContext.Provider>;
}

export function useApi() {
  const context = useContext(ApiContext);
  if (!context) {
    throw new Error('useApi must be used within ApiProvider');
  }
  return context;
}
