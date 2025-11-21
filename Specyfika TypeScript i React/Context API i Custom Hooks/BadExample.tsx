import React, { createContext, useContext, useState, ReactNode } from 'react';

// ❌ BAD: Wszystko w jednym Context - performance i coupling issues

// BŁĄD 1: Jeden wielki Context ze wszystkim
interface BadAppContextType {
  user: { id: number; name: string; email: string } | null;
  theme: 'light' | 'dark';
  language: string;
  notifications: string[];
  settings: Record<string, any>;
  setUser: (user: any) => void;
  setTheme: (theme: 'light' | 'dark') => void;
  setLanguage: (lang: string) => void;
  addNotification: (msg: string) => void;
  updateSettings: (key: string, value: any) => void;
}

const BadAppContext = createContext<BadAppContextType | undefined>(undefined);

// ❌ Provider z całym stanem aplikacji
export function BadAppProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState(null);
  const [theme, setTheme] = useState<'light' | 'dark'>('light');
  const [language, setLanguage] = useState('en');
  const [notifications, setNotifications] = useState<string[]>([]);
  const [settings, setSettings] = useState({});

  const addNotification = (msg: string) => {
    setNotifications(prev => [...prev, msg]);
  };

  const updateSettings = (key: string, value: any) => {
    setSettings(prev => ({ ...prev, [key]: value }));
  };

  // ❌ Wszystkie komponenty re-renderują się przy każdej zmianie
  const value = {
    user, setUser,
    theme, setTheme,
    language, setLanguage,
    notifications, addNotification,
    settings, updateSettings
  };

  return <BadAppContext.Provider value={value}>{children}</BadAppContext.Provider>;
}

// BŁĄD 2: Brak custom hook - duplikacja kodu
export function BadUserProfile() {
  const context = useContext(BadAppContext);
  if (!context) throw new Error('Must be used within BadAppProvider');
  
  // ❌ Re-renderuje się przy każdej zmianie w context (theme, language, etc.)
  return <div>User: {context.user?.name}</div>;
}

export function BadThemeToggle() {
  const context = useContext(BadAppContext);
  if (!context) throw new Error('Must be used within BadAppProvider');
  
  // ❌ Re-renderuje się przy zmianie user, language, notifications...
  return (
    <button onClick={() => context.setTheme(context.theme === 'light' ? 'dark' : 'light')}>
      Toggle Theme: {context.theme}
    </button>
  );
}

// BŁĄD 3: Context bez default value
const BadUnsafeContext = createContext<{ value: string; setValue: (v: string) => void }>(
  undefined as any // ❌ Type assertion zamiast proper default
);

// BŁĄD 4: Nowa referencja obiektu przy każdym renderze
export function BadPerformanceProvider({ children }: { children: ReactNode }) {
  const [count, setCount] = useState(0);
  
  // ❌ Nowy obiekt przy każdym renderze - wszystkie konsumenty re-renderują
  return (
    <BadAppContext.Provider value={{ 
      user: null, setUser: () => {},
      theme: 'light', setTheme: () => {},
      language: 'en', setLanguage: () => {},
      notifications: [], addNotification: () => {},
      settings: {}, updateSettings: () => {}
    }}>
      {children}
    </BadAppContext.Provider>
  );
}

// BŁĄD 5: Brak segregacji kontekstów - tight coupling
export function BadNotificationBell() {
  const context = useContext(BadAppContext);
  if (!context) throw new Error('Must be used within BadAppProvider');
  
  // ❌ Potrzebuje tylko notifications, ale dostaje cały context
  // ❌ Re-renderuje przy zmianie user, theme, language, settings...
  return <div>Notifications: {context.notifications.length}</div>;
}

// BŁĄD 6: Kompleksowa logika bez custom hook
export function BadComplexComponent() {
  const context = useContext(BadAppContext);
  if (!context) throw new Error('Must be used within BadAppProvider');
  
  // ❌ Duplikacja logiki w każdym komponencie
  const isLoggedIn = context.user !== null;
  const userName = context.user?.name || 'Guest';
  const isDarkMode = context.theme === 'dark';
  
  return (
    <div>
      {isLoggedIn ? `Welcome ${userName}` : 'Please login'}
      <span>Theme: {isDarkMode ? '🌙' : '☀️'}</span>
    </div>
  );
}
