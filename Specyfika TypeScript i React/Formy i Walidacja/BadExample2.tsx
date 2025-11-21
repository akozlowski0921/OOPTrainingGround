import React, { useState } from 'react';

// ❌ BAD: Walidacja problems

export function BadValidation() {
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');

  const validate = () => {
    // ❌ Prosta regex - nie validuje property
    if (!email.includes('@')) {
      setError('Invalid email');
      return false;
    }
    return true;
  };

  return (
    <form onSubmit={(e) => { e.preventDefault(); validate(); }}>
      <input value={email} onChange={e => setEmail(e.target.value)} />
      {error && <span>{error}</span>}
    </form>
  );
}

// BŁĄD: Re-renders na każdym keystroke
export function BadPerformanceForm() {
  const [form, setForm] = useState({ email: '', password: '', bio: '' });

  return (
    <form>
      <input 
        value={form.email} 
        onChange={e => setForm({ ...form, email: e.target.value })} 
      /> {/* ❌ Cały form re-renderuje */}
      <input value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} />
      <textarea value={form.bio} onChange={e => setForm({ ...form, bio: e.target.value })} />
    </form>
  );
}
