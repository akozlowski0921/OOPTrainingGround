import React from 'react';

// ❌ BAD: Type assertions zamiast type guards

type ApiResponse = 
  | { success: true; data: string }
  | { success: false; error: string };

export function ResponseDisplay({ response }: { response: ApiResponse }) {
  // Problem: Type assertion - brak sprawdzenia w runtime
  const successResponse = response as { success: true; data: string };
  
  return (
    <div>
      {/* Może crashować jeśli response.success === false */}
      <div>Data: {successResponse.data}</div>
    </div>
  );
}

type User = {
  type: 'user';
  name: string;
  email: string;
};

type Admin = {
  type: 'admin';
  name: string;
  permissions: string[];
};

type Account = User | Admin;

export function AccountDisplay({ account }: { account: Account }) {
  // Problem: Sprawdzanie właściwości bez type guard
  const adminPermissions = (account as Admin).permissions;
  
  return (
    <div>
      <div>Name: {account.name}</div>
      {/* Może być undefined dla User */}
      <div>Permissions: {adminPermissions?.join(', ')}</div>
    </div>
  );
}
