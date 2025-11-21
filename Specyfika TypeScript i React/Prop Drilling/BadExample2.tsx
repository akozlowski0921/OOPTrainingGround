import React, { useState } from 'react';

// ❌ BAD: Multiple pieces of state drilled through many levels

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

  // Problem: Wszystkie komponenty pośrednie muszą przekazywać state
  return (
    <MainLayout 
      appState={appState} 
      onStateChange={setAppState} 
    />
  );
}

function MainLayout({ appState, onStateChange }: {
  appState: AppState;
  onStateChange: (state: AppState) => void;
}) {
  // MainLayout nie używa appState, tylko przekazuje
  return (
    <div>
      <Sidebar appState={appState} onStateChange={onStateChange} />
      <ContentArea appState={appState} onStateChange={onStateChange} />
    </div>
  );
}

function Sidebar({ appState, onStateChange }: {
  appState: AppState;
  onStateChange: (state: AppState) => void;
}) {
  // Sidebar przekazuje dalej
  return (
    <div>
      <UserInfo appState={appState} />
      <SettingsPanel appState={appState} onStateChange={onStateChange} />
    </div>
  );
}

function UserInfo({ appState }: { appState: AppState }) {
  // Wreszcie używamy!
  return <div>User: {appState.userId}, Theme: {appState.darkMode ? 'Dark' : 'Light'}</div>;
}

function SettingsPanel({ appState, onStateChange }: {
  appState: AppState;
  onStateChange: (state: AppState) => void;
}) {
  // Przekazuje jeszcze głębiej
  return (
    <div>
      <LanguageSelector 
        language={appState.preferences.language}
        onChange={(lang) => onStateChange({
          ...appState,
          preferences: {...appState.preferences, language: lang}
        })}
      />
      <NotificationSettings 
        settings={appState.preferences.notifications}
        onChange={(notif) => onStateChange({
          ...appState,
          preferences: {...appState.preferences, notifications: notif}
        })}
      />
    </div>
  );
}

function LanguageSelector({ language, onChange }: {
  language: string;
  onChange: (lang: string) => void;
}) {
  return (
    <select value={language} onChange={(e) => onChange(e.target.value)}>
      <option value="en">English</option>
      <option value="pl">Polish</option>
    </select>
  );
}

function NotificationSettings({ settings, onChange }: {
  settings: NotificationSettings;
  onChange: (settings: NotificationSettings) => void;
}) {
  return (
    <div>
      <label>
        <input 
          type="checkbox" 
          checked={settings.email}
          onChange={(e) => onChange({...settings, email: e.target.checked})}
        />
        Email
      </label>
      <label>
        <input 
          type="checkbox" 
          checked={settings.push}
          onChange={(e) => onChange({...settings, push: e.target.checked})}
        />
        Push
      </label>
    </div>
  );
}

function ContentArea({ appState, onStateChange }: {
  appState: AppState;
  onStateChange: (state: AppState) => void;
}) {
  // ContentArea też musi przekazywać
  return (
    <div>
      <ArticleList appState={appState} onStateChange={onStateChange} />
    </div>
  );
}

function ArticleList({ appState, onStateChange }: {
  appState: AppState;
  onStateChange: (state: AppState) => void;
}) {
  // Używa tylko darkMode, ale musi przyjąć cały appState
  const theme = appState.darkMode ? 'dark' : 'light';
  return <div className={theme}>Articles list...</div>;
}
