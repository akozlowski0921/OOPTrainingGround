// ❌ BAD: Nadużycie Context API, wszystko w jednym kontekście, brak optymalizacji
import { createContext, useContext, useState, ReactNode } from 'react';

interface User {
  id: number;
  name: string;
  email: string;
}

interface Notification {
  id: string;
  message: string;
}

interface AppState {
  user: User | null;
  theme: 'light' | 'dark';
  notifications: Notification[];
  isLoading: boolean;
  error: string | null;
}

// Jeden wielki Context - każda zmiana powoduje re-render WSZYSTKICH konsumentów
const AppContext = createContext<{
  state: AppState;
  setUser: (user: User | null) => void;
  setTheme: (theme: 'light' | 'dark') => void;
  addNotification: (notification: Notification) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
} | null>(null);

export function AppProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AppState>({
    user: null,
    theme: 'light',
    notifications: [],
    isLoading: false,
    error: null,
  });

  // Brak memoizacji - nowy obiekt przy każdym renderze
  const value = {
    state,
    setUser: (user: User | null) => setState(s => ({ ...s, user })),
    setTheme: (theme: 'light' | 'dark') => setState(s => ({ ...s, theme })),
    addNotification: (notification: Notification) => 
      setState(s => ({ ...s, notifications: [...s.notifications, notification] })),
    setLoading: (loading: boolean) => setState(s => ({ ...s, isLoading: loading })),
    setError: (error: string | null) => setState(s => ({ ...s, error })),
  };

  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
}

// Komponent użytkownika - rerenderuje się gdy zmienia się theme, notifications, loading, etc.
export function UserProfile() {
  const context = useContext(AppContext);
  if (!context) throw new Error('useContext must be used within AppProvider');

  return (
    <div>
      {context.state.user ? (
        <p>{context.state.user.name}</p>
      ) : (
        <p>Not logged in</p>
      )}
    </div>
  );
}

// Komponent theme - rerenderuje się gdy zmienia się user, notifications, loading, etc.
export function ThemeToggle() {
  const context = useContext(AppContext);
  if (!context) throw new Error('useContext must be used within AppProvider');

  return (
    <button onClick={() => 
      context.setTheme(context.state.theme === 'light' ? 'dark' : 'light')
    }>
      Toggle Theme
    </button>
  );
}
