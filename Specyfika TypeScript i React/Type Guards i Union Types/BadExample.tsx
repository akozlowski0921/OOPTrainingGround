import React from 'react';

// Problem: Rzutowanie typu "na siłę" - brak bezpieczeństwa typów
interface Dog {
  type: 'dog';
  bark: () => void;
  breed: string;
}

interface Cat {
  type: 'cat';
  meow: () => void;
  color: string;
}

type Pet = Dog | Cat;

// Problem: Używanie `as` do wymuszenia typu bez sprawdzenia
export function PetProfile({ pet }: { pet: Pet }) {
  // Problem: Zakładamy że to Dog bez sprawdzenia
  const dog = pet as Dog;
  
  return (
    <div>
      <h2>Pet Profile</h2>
      {/* To zadziała tylko dla Dog, dla Cat będzie undefined! */}
      <p>Breed: {dog.breed}</p>
      <button onClick={() => dog.bark()}>Make Sound</button>
    </div>
  );
}

// Problem: Rzutowanie any zamiast użycia union types
export function UserProfile({ user }: { user: any }) {
  // Problem: user może być czymkolwiek, brak type safety
  return (
    <div>
      <h2>{user.name}</h2>
      {/* TypeScript nie ostrzeże nas jeśli user.email nie istnieje */}
      <p>Email: {user.email}</p>
      <p>Age: {user.age}</p>
      {/* Co jeśli user ma role jako array? Lub wcale nie ma tej właściwości? */}
      <p>Role: {user.role}</p>
    </div>
  );
}

// Problem: Używanie string literala zamiast discriminated union
interface ApiResponse {
  status: string; // może być 'success' | 'error' | cokolwiek
  data?: any;
  error?: any;
}

export function ApiResultDisplay({ response }: { response: ApiResponse }) {
  // Problem: Brak type safety - TypeScript nie wie co jest w data/error
  if (response.status === 'success') {
    // Musimy rzutować na siłę
    const data = response.data as { message: string };
    return <div>{data.message}</div>;
  } else {
    // Znowu rzutowanie
    const error = response.error as { code: number; message: string };
    return <div>Error {error.code}: {error.message}</div>;
  }
}

// Problem: Brak Type Guards dla złożonych warunków
interface User {
  id: number;
  name: string;
  email?: string;
  address?: {
    street: string;
    city: string;
  };
}

export function UserDetails({ user }: { user: User }) {
  // Problem: Sprawdzamy istnienie, ale TypeScript tego nie widzi
  const hasCompleteAddress = user.address && user.address.street && user.address.city;
  
  return (
    <div>
      <h2>{user.name}</h2>
      {hasCompleteAddress && (
        <div>
          {/* TypeScript nadal myśli że address może być undefined */}
          <p>{user.address!.street}</p>
          <p>{user.address!.city}</p>
        </div>
      )}
    </div>
  );
}

// Problem: Nie używamy discriminated unions dla form state
export function PaymentForm() {
  const [paymentMethod, setPaymentMethod] = React.useState<string>('card');
  const [cardNumber, setCardNumber] = React.useState('');
  const [bankAccount, setBankAccount] = React.useState('');
  const [paypalEmail, setPaypalEmail] = React.useState('');

  // Problem: Wszystkie pola istnieją jednocześnie, mimo że używamy tylko jednego
  return (
    <div>
      <select value={paymentMethod} onChange={e => setPaymentMethod(e.target.value)}>
        <option value="card">Credit Card</option>
        <option value="bank">Bank Transfer</option>
        <option value="paypal">PayPal</option>
      </select>

      {/* Problem: TypeScript nie wie które pole jest aktywne */}
      {paymentMethod === 'card' && (
        <input value={cardNumber} onChange={e => setCardNumber(e.target.value)} />
      )}
      {paymentMethod === 'bank' && (
        <input value={bankAccount} onChange={e => setBankAccount(e.target.value)} />
      )}
      {paymentMethod === 'paypal' && (
        <input value={paypalEmail} onChange={e => setPaypalEmail(e.target.value)} />
      )}
    </div>
  );
}
