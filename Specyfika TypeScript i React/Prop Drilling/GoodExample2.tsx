import React, { createContext, useContext, useState } from 'react';

// ✅ GOOD: Using Context API to avoid prop drilling

interface NotificationSettings {
  email: boolean;
  push: boolean;
  sms: boolean;
}

interface UserPreferences {
  language: string;
  timezone: string;
  notifications: NotificationSettings;
}

interface AppState {
  userId: string;
  preferences: UserPreferences;
  darkMode: boolean;
}

interface AppContextType {
  appState: AppState;
  updateLanguage: (language: string) => void;
  updateNotifications: (settings: NotificationSettings) => void;
  toggleDarkMode: () => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

export function Dashboard() {
  const [appState, setAppState] = useState<AppState>({
    userId: '123',
    preferences: {
      language: 'en',
      timezone: 'UTC',
      notifications: {
        email: true,
        push: false,
        sms: true
      }
    },
    darkMode: false
  });

  const contextValue: AppContextType = {
    appState,
    updateLanguage: (language) => {
      setAppState(prev => ({
        ...prev,
        preferences: {...prev.preferences, language}
      }));
    },
    updateNotifications: (notifications) => {
      setAppState(prev => ({
        ...prev,
        preferences: {...prev.preferences, notifications}
      }));
    },
    toggleDarkMode: () => {
      setAppState(prev => ({...prev, darkMode: !prev.darkMode}));
    }
  };

  return (
    <AppContext.Provider value={contextValue}>
      <MainLayout />
    </AppContext.Provider>
  );
}

// Hook do łatwego dostępu do kontekstu
function useAppContext() {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error('useAppContext must be used within AppContext.Provider');
  }
  return context;
}

function MainLayout() {
  // Nie musi przyjmować props
  return (
    <div>
      <Sidebar />
      <ContentArea />
    </div>
  );
}

function Sidebar() {
  // Nie musi przyjmować props
  return (
    <div>
      <UserInfo />
      <SettingsPanel />
    </div>
  );
}

function UserInfo() {
  // Używa kontekstu bezpośrednio
  const { appState } = useAppContext();
  return <div>User: {appState.userId}, Theme: {appState.darkMode ? 'Dark' : 'Light'}</div>;
}

function SettingsPanel() {
  // Nie musi przyjmować props
  return (
    <div>
      <LanguageSelector />
      <NotificationSettings />
    </div>
  );
}

function LanguageSelector() {
  // Używa kontekstu bezpośrednio
  const { appState, updateLanguage } = useAppContext();
  
  return (
    <select value={appState.preferences.language} onChange={(e) => updateLanguage(e.target.value)}>
      <option value="en">English</option>
      <option value="pl">Polish</option>
    </select>
  );
}

function NotificationSettings() {
  // Używa kontekstu bezpośrednio
  const { appState, updateNotifications } = useAppContext();
  const settings = appState.preferences.notifications;
  
  return (
    <div>
      <label>
        <input 
          type="checkbox" 
          checked={settings.email}
          onChange={(e) => updateNotifications({...settings, email: e.target.checked})}
        />
        Email
      </label>
      <label>
        <input 
          type="checkbox" 
          checked={settings.push}
          onChange={(e) => updateNotifications({...settings, push: e.target.checked})}
        />
        Push
      </label>
    </div>
  );
}

function ContentArea() {
  // Nie musi przyjmować props
  return (
    <div>
      <ArticleList />
    </div>
  );
}

function ArticleList() {
  // Używa tylko tego co potrzebuje z kontekstu
  const { appState } = useAppContext();
  const theme = appState.darkMode ? 'dark' : 'light';
  return <div className={theme}>Articles list...</div>;
}
