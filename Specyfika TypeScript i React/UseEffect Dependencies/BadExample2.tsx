import React, { useState, useEffect } from 'react';

// ❌ BAD: Missing dependencies in useEffect

export function UserDashboard({ userId }: { userId: number }) {
  const [userData, setUserData] = useState(null);
  const [settings, setSettings] = useState({ theme: 'light' });

  useEffect(() => {
    // Problem: Używa userId i settings.theme, ale nie ma ich w dependencies
    fetch(`/api/users/${userId}?theme=${settings.theme}`)
      .then(r => r.json())
      .then(setUserData);
  }, []); // Pusta tablica = wykonuje się tylko raz

  return <div>{userData ? JSON.stringify(userData) : 'Loading'}</div>;
}
