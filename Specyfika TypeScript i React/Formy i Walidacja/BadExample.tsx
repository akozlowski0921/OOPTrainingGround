import React, { useState } from 'react';

// ❌ BAD: Uncontrolled forms without validation

export function BadLoginForm() {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);
    const email = formData.get('email');
    const password = formData.get('password');
    
    // ❌ Brak walidacji
    console.log(email, password);
  };

  return (
    <form onSubmit={handleSubmit}>
      <input name="email" /> {/* ❌ Brak type, validation */}
      <input name="password" />
      <button type="submit">Login</button>
    </form>
  );
}

// BŁĄD: Manual state dla każdego pola
export function BadManualForm() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  // ❌ Boilerplate dla każdego pola

  return (
    <form>
      <input value={email} onChange={e => setEmail(e.target.value)} />
      <input value={password} onChange={e => setPassword(e.target.value)} />
      <input value={name} onChange={e => setName(e.target.value)} />
    </form>
  );
}
