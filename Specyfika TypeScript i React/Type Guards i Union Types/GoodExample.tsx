import React from 'react';

// Rozwiązanie: Discriminated Union Types z Type Guards
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

// Type Guard - TypeScript wie jaki to typ po sprawdzeniu
function isDog(pet: Pet): pet is Dog {
  return pet.type === 'dog';
}

function isCat(pet: Pet): pet is Cat {
  return pet.type === 'cat';
}

export function PetProfile({ pet }: { pet: Pet }) {
  // Type Guard zapewnia bezpieczeństwo typów
  if (isDog(pet)) {
    // TypeScript wie że to Dog - mamy dostęp do breed i bark
    return (
      <div>
        <h2>Dog Profile</h2>
        <p>Breed: {pet.breed}</p>
        <button onClick={() => pet.bark()}>Bark</button>
      </div>
    );
  } else {
    // TypeScript wie że to Cat - mamy dostęp do color i meow
    return (
      <div>
        <h2>Cat Profile</h2>
        <p>Color: {pet.color}</p>
        <button onClick={() => pet.meow()}>Meow</button>
      </div>
    );
  }
}

// Rozwiązanie: Używamy union types zamiast any
interface BasicUser {
  type: 'basic';
  name: string;
  email: string;
}

interface PremiumUser {
  type: 'premium';
  name: string;
  email: string;
  age: number;
  subscription: string;
}

type User = BasicUser | PremiumUser;

export function UserProfile({ user }: { user: User }) {
  return (
    <div>
      <h2>{user.name}</h2>
      <p>Email: {user.email}</p>
      {/* Type narrowing sprawdza typ przed dostępem do właściwości */}
      {user.type === 'premium' && (
        <>
          <p>Age: {user.age}</p>
          <p>Subscription: {user.subscription}</p>
        </>
      )}
    </div>
  );
}

// Rozwiązanie: Discriminated unions dla API responses
type ApiResponse<T> = 
  | { status: 'success'; data: T }
  | { status: 'error'; error: { code: number; message: string } }
  | { status: 'loading' };

export function ApiResultDisplay<T>({ 
  response 
}: { 
  response: ApiResponse<{ message: string }> 
}) {
  // TypeScript automatycznie zawęża typ na podstawie status
  switch (response.status) {
    case 'success':
      // TypeScript wie że mamy data
      return <div>{response.data.message}</div>;
    
    case 'error':
      // TypeScript wie że mamy error
      return <div>Error {response.error.code}: {response.error.message}</div>;
    
    case 'loading':
      return <div>Loading...</div>;
  }
}

// Rozwiązanie: Custom Type Guard dla złożonych warunków
interface UserAddress {
  street: string;
  city: string;
  zipCode?: string;
}

interface UserWithAddress {
  id: number;
  name: string;
  email?: string;
  address: UserAddress;
}

interface UserWithoutAddress {
  id: number;
  name: string;
  email?: string;
  address?: undefined;
}

type UserData = UserWithAddress | UserWithoutAddress;

// Type Guard dla użytkownika z pełnym adresem
function hasCompleteAddress(user: UserData): user is UserWithAddress {
  return user.address !== undefined && 
         user.address.street !== undefined && 
         user.address.city !== undefined;
}

export function UserDetails({ user }: { user: UserData }) {
  return (
    <div>
      <h2>{user.name}</h2>
      {/* TypeScript wie że user.address istnieje w tym bloku */}
      {hasCompleteAddress(user) && (
        <div>
          <p>{user.address.street}</p>
          <p>{user.address.city}</p>
          {user.address.zipCode && <p>{user.address.zipCode}</p>}
        </div>
      )}
    </div>
  );
}

// Rozwiązanie: Discriminated unions dla form state
type PaymentMethod =
  | { type: 'card'; cardNumber: string }
  | { type: 'bank'; bankAccount: string }
  | { type: 'paypal'; paypalEmail: string };

export function PaymentForm() {
  const [payment, setPayment] = React.useState<PaymentMethod>({
    type: 'card',
    cardNumber: ''
  });

  return (
    <div>
      <select 
        value={payment.type} 
        onChange={e => {
          const type = e.target.value as PaymentMethod['type'];
          // TypeScript zapewnia że tworzymy poprawny obiekt dla każdego typu
          switch (type) {
            case 'card':
              setPayment({ type: 'card', cardNumber: '' });
              break;
            case 'bank':
              setPayment({ type: 'bank', bankAccount: '' });
              break;
            case 'paypal':
              setPayment({ type: 'paypal', paypalEmail: '' });
              break;
          }
        }}
      >
        <option value="card">Credit Card</option>
        <option value="bank">Bank Transfer</option>
        <option value="paypal">PayPal</option>
      </select>

      {/* TypeScript wie dokładnie jaki typ payment mamy w każdym bloku */}
      {payment.type === 'card' && (
        <input 
          value={payment.cardNumber} 
          onChange={e => setPayment({ ...payment, cardNumber: e.target.value })} 
        />
      )}
      {payment.type === 'bank' && (
        <input 
          value={payment.bankAccount} 
          onChange={e => setPayment({ ...payment, bankAccount: e.target.value })} 
        />
      )}
      {payment.type === 'paypal' && (
        <input 
          value={payment.paypalEmail} 
          onChange={e => setPayment({ ...payment, paypalEmail: e.target.value })} 
        />
      )}
    </div>
  );
}
