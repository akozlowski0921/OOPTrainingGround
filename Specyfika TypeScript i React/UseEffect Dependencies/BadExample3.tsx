import React, { useState, useEffect } from 'react';

// ❌ BAD: Object/function as dependency causing infinite loops

export function SearchComponent() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);

  const searchConfig = {
    limit: 10,
    sortBy: 'relevance'
  };

  useEffect(() => {
    // Problem: searchConfig to nowy obiekt przy każdym renderze
    // useEffect uruchamia się w nieskończoność
    fetch(`/api/search?q=${query}&limit=${searchConfig.limit}`)
      .then(r => r.json())
      .then(setResults);
  }, [query, searchConfig]); // searchConfig zmienia się przy każdym renderze!

  return <div>Results: {results.length}</div>;
}
