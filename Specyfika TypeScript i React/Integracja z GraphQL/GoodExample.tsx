import React from 'react';
import { useQuery, gql, ApolloClient, InMemoryCache } from '@apollo/client';

// ✅ GOOD: Using Apollo Client with proper cache management

const GET_USER = gql`
  query GetUser($userId: ID!) {
    user(id: $userId) {
      id
      name
      email
    }
  }
`;

const GET_POSTS = gql`
  query GetPosts($userId: ID!) {
    posts(userId: $userId) {
      id
      title
      content
      author {
        id
        name
      }
    }
  }
`;

export function ApolloGraphQLComponent({ userId }: { userId: string }) {
  const { data: userData, loading: userLoading, refetch: refetchUser } = useQuery(GET_USER, {
    variables: { userId }
  });

  const { data: postsData, loading: postsLoading, refetch: refetchPosts } = useQuery(GET_POSTS, {
    variables: { userId }
  });

  const handleRefresh = () => {
    refetchUser();
    refetchPosts();
  };

  if (userLoading || postsLoading) return <div>Loading...</div>;

  return (
    <div>
      <button onClick={handleRefresh}>Refresh</button>
      <div>{userData?.user.name}</div>
      {postsData?.posts.map((post: any) => (
        <div key={post.id}>{post.title}</div>
      ))}
    </div>
  );
}

// Cache configuration
export const client = new ApolloClient({
  uri: '/graphql',
  cache: new InMemoryCache({
    typePolicies: {
      Query: {
        fields: {
          posts: {
            merge(existing = [], incoming) {
              return incoming;
            }
          }
        }
      }
    }
  })
});
