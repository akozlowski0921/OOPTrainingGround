import React from 'react';

// ✅ GOOD: Discriminated unions with type guards

type LoadingState = { status: 'loading'; };
type ErrorState = { status: 'error'; message: string; };
type SuccessState = { status: 'success'; data: any; };

type State = LoadingState | ErrorState | SuccessState;

export function DataDisplay({ state }: { state: State }) {
  // Discriminated union - TypeScript zna typ bazując na status
  switch (state.status) {
    case 'loading':
      return <div>Loading...</div>;
      
    case 'error':
      // TypeScript wie, że state ma message
      return <div>Error: {state.message}</div>;
      
    case 'success':
      // TypeScript wie, że state ma data
      return <div>Data: {JSON.stringify(state.data)}</div>;
  }
}
