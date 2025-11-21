import React, { useState, useEffect } from 'react';

// ❌ BAD: Manual fetch without Apollo Client cache management

export function ManualGraphQLFetch() {
  const [user, setUser] = useState<any>(null);
  const [posts, setPosts] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  // Problem: Brak cache - każde wywołanie to nowy request
  const fetchUser = async (userId: string) => {
    setLoading(true);
    const response = await fetch('/graphql', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        query: `query { user(id: "${userId}") { id name email } }`
      })
    });
    const data = await response.json();
    setUser(data.data.user);
    setLoading(false);
  };

  const fetchPosts = async (userId: string) => {
    setLoading(true);
    const response = await fetch('/graphql', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        query: `query { posts(userId: "${userId}") { id title content } }`
      })
    });
    const data = await response.json();
    setPosts(data.data.posts);
    setLoading(false);
  };

  // Problem: Ręczne zarządzanie refetch
  const refetchAll = () => {
    if (user) {
      fetchUser(user.id);
      fetchPosts(user.id);
    }
  };

  return (
    <div>
      {loading && <div>Loading...</div>}
      <button onClick={refetchAll}>Refresh</button>
      {user && <div>{user.name}</div>}
      {posts.map(post => <div key={post.id}>{post.title}</div>)}
    </div>
  );
}
