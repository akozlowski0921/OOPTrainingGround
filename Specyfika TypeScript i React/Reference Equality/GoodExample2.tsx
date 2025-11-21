import React, { useState, useCallback, useMemo } from 'react';

// ✅ GOOD: Using useMemo and useCallback for stable references

interface Props {
  userId: number;
}

export function UserProfile({ userId }: Props) {
  const [userData, setUserData] = useState(null);

  // Stabilna referencja do config
  const config = useMemo(() => ({
    apiUrl: 'https://api.example.com',
    timeout: 5000
  }), []); // Pusta zależność = tworzone raz

  // Stabilna referencja do funkcji
  const fetchUser = useCallback(() => {
    fetch(`${config.apiUrl}/users/${userId}`)
      .then(r => r.json())
      .then(setUserData);
  }, [userId, config.apiUrl]); // Re-create tylko gdy zależności się zmienią

  return (
    <div>
      <MemoizedComponent config={config} onFetch={fetchUser} />
      <div>{userData ? JSON.stringify(userData) : 'No data'}</div>
    </div>
  );
}

const MemoizedComponent = React.memo(({ config, onFetch }: {
  config: { apiUrl: string; timeout: number };
  onFetch: () => void;
}) => {
  console.log('MemoizedComponent rendered!'); // Renderuje tylko gdy props faktycznie się zmieniają
  return <button onClick={onFetch}>Fetch</button>;
});
