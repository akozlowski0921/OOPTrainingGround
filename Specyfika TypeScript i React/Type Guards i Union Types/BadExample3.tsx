import React from 'react';

// ❌ BAD: Multiple type checks without discriminated unions

type LoadingState = { status: string; };
type ErrorState = { status: string; message: string; };
type SuccessState = { status: string; data: any; };

type State = LoadingState | ErrorState | SuccessState;

export function DataDisplay({ state }: { state: State }) {
  // Problem: Sprawdzanie właściwości zamiast discriminated union
  if ('message' in state) {
    return <div>Error: {(state as any).message}</div>;
  }
  
  if ('data' in state) {
    return <div>Data: {JSON.stringify((state as any).data)}</div>;
  }
  
  return <div>Loading...</div>;
}
