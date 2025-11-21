import React, { useState } from 'react';

// ✅ GOOD: Jeden useState dla obiektu formularza

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
  // Jeden useState dla całego formularza
  const [formData, setFormData] = useState<FormData>({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    zipCode: '',
    country: ''
  });

  // Jeden handler dla wszystkich pól
  const handleChange = (field: keyof FormData) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({ ...prev, [field]: e.target.value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      <input value={formData.firstName} onChange={handleChange('firstName')} placeholder="First Name" />
      <input value={formData.lastName} onChange={handleChange('lastName')} placeholder="Last Name" />
      <input value={formData.email} onChange={handleChange('email')} placeholder="Email" />
      <input value={formData.phone} onChange={handleChange('phone')} placeholder="Phone" />
      <input value={formData.address} onChange={handleChange('address')} placeholder="Address" />
      <input value={formData.city} onChange={handleChange('city')} placeholder="City" />
      <input value={formData.zipCode} onChange={handleChange('zipCode')} placeholder="Zip" />
      <input value={formData.country} onChange={handleChange('country')} placeholder="Country" />
      <button type="submit">Submit</button>
    </form>
  );
}
