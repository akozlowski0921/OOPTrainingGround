import React, { useState } from 'react';

// ❌ BAD: Multiple useState dla powiązanych danych

interface FormData {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  zipCode: string;
  country: string;
}

export function RegistrationForm() {
  // Problem: 8 osobnych useState dla danych formularza
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [city, setCity] = useState('');
  const [zipCode, setZipCode] = useState('');
  const [country, setCountry] = useState('');

  // 8 osobnych handlerow
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const data: FormData = {
      firstName, lastName, email, phone,
      address, city, zipCode, country
    };
    console.log(data);
  };

  return (
    <form onSubmit={handleSubmit}>
      <input value={firstName} onChange={e => setFirstName(e.target.value)} placeholder="First Name" />
      <input value={lastName} onChange={e => setLastName(e.target.value)} placeholder="Last Name" />
      <input value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" />
      <input value={phone} onChange={e => setPhone(e.target.value)} placeholder="Phone" />
      <input value={address} onChange={e => setAddress(e.target.value)} placeholder="Address" />
      <input value={city} onChange={e => setCity(e.target.value)} placeholder="City" />
      <input value={zipCode} onChange={e => setZipCode(e.target.value)} placeholder="Zip" />
      <input value={country} onChange={e => setCountry(e.target.value)} placeholder="Country" />
      <button type="submit">Submit</button>
    </form>
  );
}
