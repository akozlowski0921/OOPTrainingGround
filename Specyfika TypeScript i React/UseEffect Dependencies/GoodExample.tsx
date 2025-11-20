import React, { useState, useEffect, useCallback } from 'react';

interface User {
  id: number;
  name: string;
}

// Rozwiązanie: Wszystkie dependencies są kompletne
export function UserProfile({ userId }: { userId: number }) {
  const [user, setUser] = useState<User | null>(null);
  const [retryCount, setRetryCount] = useState(0);

  useEffect(() => {
    console.log(`Fetching user ${userId}, retry count: ${retryCount}`);
    
    fetch(`/api/users/${userId}`)
      .then(res => res.json())
      .then(data => setUser(data))
      .catch(err => {
        console.error('Failed to fetch user:', err);
        // Używamy functional update - zawsze mamy aktualną wartość
        setRetryCount(prevCount => {
          if (prevCount < 3) {
            return prevCount + 1;
          }
          return prevCount;
        });
      });
  }, [userId, retryCount]); // pełna lista dependencies

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

// Rozwiązanie: useCallback zapewnia stabilną referencję funkcji
export function SearchComponent() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<string[]>([]);
  const [filters, setFilters] = useState({ category: 'all', sortBy: 'name' });

  // useCallback zapewnia, że funkcja jest odtworzona tylko gdy zmienią się filters
  const performSearch = useCallback((searchQuery: string) => {
    console.log(`Searching for: ${searchQuery} with filters:`, filters);
    setResults([`Result for ${searchQuery} (${filters.category})`]);
  }, [filters]); // performSearch jest odtworzona tylko gdy zmieni się filters

  useEffect(() => {
    if (query) {
      performSearch(query);
    }
  }, [query, performSearch]); // pełna lista dependencies - teraz bezpieczna

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
