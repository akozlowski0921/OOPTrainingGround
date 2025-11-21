import React from 'react';

// ✅ GOOD: Type guards dla bezpiecznego type narrowing

type ApiResponse = 
  | { success: true; data: string }
  | { success: false; error: string };

function isSuccessResponse(response: ApiResponse): response is { success: true; data: string } {
  return response.success === true;
}

export function ResponseDisplay({ response }: { response: ApiResponse }) {
  if (isSuccessResponse(response)) {
    // TypeScript wie, że to success response
    return <div>Data: {response.data}</div>;
  } else {
    // TypeScript wie, że to error response
    return <div>Error: {response.error}</div>;
  }
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

function isAdmin(account: Account): account is Admin {
  return account.type === 'admin';
}

export function AccountDisplay({ account }: { account: Account }) {
  return (
    <div>
      <div>Name: {account.name}</div>
      {isAdmin(account) && (
        <div>Permissions: {account.permissions.join(', ')}</div>
      )}
      {!isAdmin(account) && (
        <div>Email: {account.email}</div>
      )}
    </div>
  );
}
