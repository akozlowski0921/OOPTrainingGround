import React, { useState, useEffect, useMemo } from 'react';

// ✅ GOOD: Stable references or primitive dependencies

export function SearchComponent() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);

  // Opcja 1: useMemo dla stabilnej referencji
  const searchConfig = useMemo(() => ({
    limit: 10,
    sortBy: 'relevance'
  }), []); // Tworzone raz

  useEffect(() => {
    fetch(`/api/search?q=${query}&limit=${searchConfig.limit}`)
      .then(r => r.json())
      .then(setResults);
  }, [query, searchConfig]);

  return <div>Results: {results.length}</div>;
}

// Opcja 2: Używaj prymitywów zamiast obiektów
export function SearchComponent2() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);

  const limit = 10;
  const sortBy = 'relevance';

  useEffect(() => {
    fetch(`/api/search?q=${query}&limit=${limit}&sort=${sortBy}`)
      .then(r => r.json())
      .then(setResults);
  }, [query, limit, sortBy]); // Prymitywy są stabilne

  return <div>Results: {results.length}</div>;
}
