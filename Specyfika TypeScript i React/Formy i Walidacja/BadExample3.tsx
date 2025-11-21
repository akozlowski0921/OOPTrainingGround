import React, { useState } from 'react';

// ❌ BAD: Submit handling issues

export function BadSubmitHandling() {
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    
    // ❌ Brak error handling
    await fetch('/api/submit');
    
    setLoading(false); // ❌ Nie ustawia się przy error
  };

  return (
    <form onSubmit={handleSubmit}>
      <input name="email" />
      <button disabled={loading}>Submit</button>
      {/* ❌ Brak error display */}
    </form>
  );
}

// BŁĄD: Brak resetowania formy
export function BadFormReset() {
  const [data, setData] = useState({ name: '', email: '' });

  const handleSubmit = () => {
    console.log(data);
    // ❌ Form nie resetuje się po submit
  };

  return <form onSubmit={handleSubmit}>...</form>;
}
