import React, { ReactNode } from 'react';

// ❌ BAD: Słabe typowanie komponentów

// BŁĄD 1: React.FC - antywzorzec
export const BadButton: React.FC<{ label: string }> = ({ label }) => {
  // ❌ React.FC implicitly adds children - mylące
  return <button>{label}</button>;
};

// BŁĄD 2: any dla props
export function BadCard({ title, content }: any) { // ❌ any
  return <div>{title}: {content}</div>;
}

// BŁĄD 3: Brak typowania children
export function BadContainer({ children }: { children: any }) { // ❌ any children
  return <div>{children}</div>;
}

// BŁĄD 4: Brak discriminated unions w state
type BadState = {
  isLoading: boolean;
  data?: any; // ❌ any
  error?: string;
  // ❌ Możliwe nieprawidłowe kombinacje: loading=true & data exists
};

// BŁĄD 5: Brak generics dla reusable components
export function BadList({ items }: { items: any[] }) { // ❌ any[]
  return (
    <ul>
      {items.map((item, i) => <li key={i}>{item}</li>)}
    </ul>
  );
}

// BŁĄD 6: Nieprawidłowe typowanie event handlers
export function BadForm() {
  const handleSubmit = (e: any) => { // ❌ any event
    e.preventDefault();
  };

  return <form onSubmit={handleSubmit}></form>;
}
