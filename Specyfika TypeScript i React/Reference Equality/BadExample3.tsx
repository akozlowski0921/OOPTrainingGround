import React, { useState, useEffect } from 'react';

// ❌ BAD: Array/object dependencies without stable references

export function SearchComponent() {
  const [results, setResults] = useState([]);
  const [query, setQuery] = useState('');

  // Problem: Nowa tablica przy każdym renderze
  const filters = ['active', 'verified'];

  useEffect(() => {
    // Problem: useEffect uruchamia się przy każdym renderze
    // bo filters to nowa referencja
    console.log('Fetching with filters:', filters);
    fetch(`/api/search?q=${query}&filters=${filters.join(',')}`)
      .then(r => r.json())
      .then(setResults);
  }, [query, filters]); // filters zmienia się przy każdym renderze!

  return (
    <div>
      <input value={query} onChange={e => setQuery(e.target.value)} />
      <div>Results: {results.length}</div>
    </div>
  );
}
