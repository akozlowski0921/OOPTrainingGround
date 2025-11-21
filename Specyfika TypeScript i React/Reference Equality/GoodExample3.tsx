import React, { useState, useEffect, useMemo } from 'react';

// ✅ GOOD: Stable references for arrays/objects in dependencies

export function SearchComponent() {
  const [results, setResults] = useState([]);
  const [query, setQuery] = useState('');

  // Stabilna referencja do filtrów
  const filters = useMemo(() => ['active', 'verified'], []);

  useEffect(() => {
    // useEffect uruchamia się tylko gdy query się zmieni
    console.log('Fetching with filters:', filters);
    fetch(`/api/search?q=${query}&filters=${filters.join(',')}`)
      .then(r => r.json())
      .then(setResults);
  }, [query, filters]); // filters ma stabilną referencję

  return (
    <div>
      <input value={query} onChange={e => setQuery(e.target.value)} />
      <div>Results: {results.length}</div>
    </div>
  );
}
