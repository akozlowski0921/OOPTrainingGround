import React, { useState, useEffect } from 'react';

// ✅ GOOD: Complete dependencies in useEffect

export function UserDashboard({ userId }: { userId: number }) {
  const [userData, setUserData] = useState(null);
  const [settings, setSettings] = useState({ theme: 'light' });

  useEffect(() => {
    fetch(`/api/users/${userId}?theme=${settings.theme}`)
      .then(r => r.json())
      .then(setUserData);
  }, [userId, settings.theme]); // Wszystkie używane wartości w dependencies

  return <div>{userData ? JSON.stringify(userData) : 'Loading'}</div>;
}
