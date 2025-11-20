import React, { useState, useEffect } from 'react';

interface User {
  id: number;
  name: string;
}

// Problem: Zmienna użyta poza tablicą zależności - stale closure
export function UserProfile({ userId }: { userId: number }) {
  const [user, setUser] = useState<User | null>(null);
  const [retryCount, setRetryCount] = useState(0);

  // Problem: retryCount jest używany wewnątrz effectu, ale nie jest w dependencies
  useEffect(() => {
    console.log(`Fetching user ${userId}, retry count: ${retryCount}`);
    
    fetch(`/api/users/${userId}`)
      .then(res => res.json())
      .then(data => setUser(data))
      .catch(err => {
        console.error('Failed to fetch user:', err);
        // Ta linia zawsze widzi początkową wartość retryCount (0) - stale closure!
        if (retryCount < 3) {
          setRetryCount(retryCount + 1);
        }
      });
  }, [userId]); // brak retryCount w dependencies - stale closure

  return (
    <div>
      {user ? (
        <div>
          <h2>{user.name}</h2>
          <p>User ID: {user.id}</p>
        </div>
      ) : (
        <p>Loading...</p>
      )}
      <p>Retry count: {retryCount}</p>
    </div>
  );
}

// Problem: Funkcja używana w useEffect nie jest zamknięta w useCallback
export function SearchComponent() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<string[]>([]);
  const [filters, setFilters] = useState({ category: 'all', sortBy: 'name' });

  // Ta funkcja jest tworzona na nowo przy każdym renderze
  const performSearch = (searchQuery: string) => {
    console.log(`Searching for: ${searchQuery} with filters:`, filters);
    // Symulacja wyszukiwania
    setResults([`Result for ${searchQuery}`]);
  };

  // Problem: performSearch nie jest w dependencies, ale jest używana
  useEffect(() => {
    if (query) {
      performSearch(query);
    }
  }, [query]); // brak performSearch - będzie zawsze używać starej wersji funkcji

  return (
    <div>
      <input 
        type="text" 
        value={query} 
        onChange={(e) => setQuery(e.target.value)} 
        placeholder="Search..."
      />
      <button onClick={() => setFilters({ ...filters, category: 'books' })}>
        Filter Books
      </button>
      <ul>
        {results.map((result, index) => (
          <li key={index}>{result}</li>
        ))}
      </ul>
    </div>
  );
}
