import React, { useState } from 'react';

// ❌ BAD: Manual form generation without schema-driven approach

interface FormField {
  name: string;
  type: string;
  label: string;
  required: boolean;
  options?: string[];
}

// Problem: Ręczne tworzenie każdego pola - nie skalowalne
export function ManualFormCreation() {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    age: '',
    country: '',
    newsletter: false,
    interests: [] as string[]
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Problem: Hardcoded validation dla każdego pola
  const validateForm = () => {
    const newErrors: Record<string, string> = {};
    
    if (!formData.firstName) {
      newErrors.firstName = 'First name is required';
    }
    if (!formData.lastName) {
      newErrors.lastName = 'Last name is required';
    }
    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }
    if (!formData.age) {
      newErrors.age = 'Age is required';
    } else if (parseInt(formData.age) < 18) {
      newErrors.age = 'Must be at least 18';
    }
    if (!formData.country) {
      newErrors.country = 'Country is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validateForm()) {
      console.log('Form submitted:', formData);
    }
  };

  // Problem: Każde pole ręcznie zaimplementowane
  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>First Name</label>
        <input
          value={formData.firstName}
          onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
        />
        {errors.firstName && <span>{errors.firstName}</span>}
      </div>

      <div>
        <label>Last Name</label>
        <input
          value={formData.lastName}
          onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
        />
        {errors.lastName && <span>{errors.lastName}</span>}
      </div>

      <div>
        <label>Email</label>
        <input
          type="email"
          value={formData.email}
          onChange={(e) => setFormData({ ...formData, email: e.target.value })}
        />
        {errors.email && <span>{errors.email}</span>}
      </div>

      <div>
        <label>Age</label>
        <input
          type="number"
          value={formData.age}
          onChange={(e) => setFormData({ ...formData, age: e.target.value })}
        />
        {errors.age && <span>{errors.age}</span>}
      </div>

      <div>
        <label>Country</label>
        <select
          value={formData.country}
          onChange={(e) => setFormData({ ...formData, country: e.target.value })}
        >
          <option value="">Select...</option>
          <option value="US">United States</option>
          <option value="UK">United Kingdom</option>
          <option value="PL">Poland</option>
        </select>
        {errors.country && <span>{errors.country}</span>}
      </div>

      <div>
        <label>
          <input
            type="checkbox"
            checked={formData.newsletter}
            onChange={(e) => setFormData({ ...formData, newsletter: e.target.checked })}
          />
          Subscribe to newsletter
        </label>
      </div>

      <button type="submit">Submit</button>
    </form>
  );
}

// Problem: Dodanie nowego pola wymaga zmian w wielu miejscach
export function AddingNewField() {
  // 1. Dodaj do state
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    // Nowe pole - musimy pamiętać o dodaniu tutaj
    phone: ''
  });

  // 2. Dodaj do validation
  const validateForm = () => {
    const errors: Record<string, string> = {};
    if (!formData.name) errors.name = 'Required';
    if (!formData.email) errors.email = 'Required';
    // Musimy pamiętać o dodaniu walidacji
    if (!formData.phone) errors.phone = 'Required';
    return errors;
  };

  // 3. Dodaj do JSX
  return (
    <form>
      <input
        value={formData.name}
        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
      />
      <input
        value={formData.email}
        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
      />
      {/* Musimy pamiętać o dodaniu inputa */}
      <input
        value={formData.phone}
        onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
      />
      <button type="submit">Submit</button>
    </form>
  );
}

// Problem: Brak centralizacji konfiguracji formularza
export function DecentralizedFormConfig() {
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    confirmPassword: ''
  });

  // Problem: Reguły walidacji rozrzucone po kodzie
  const MIN_USERNAME_LENGTH = 3;
  const MIN_PASSWORD_LENGTH = 8;

  const handleUsernameChange = (value: string) => {
    // Walidacja tutaj
    if (value.length < MIN_USERNAME_LENGTH) {
      console.log('Username too short');
    }
    setFormData({ ...formData, username: value });
  };

  const handlePasswordChange = (value: string) => {
    // Inna walidacja tutaj
    if (value.length < MIN_PASSWORD_LENGTH) {
      console.log('Password too short');
    }
    setFormData({ ...formData, password: value });
  };

  const handleConfirmPasswordChange = (value: string) => {
    // Jeszcze inna walidacja tutaj
    if (value !== formData.password) {
      console.log('Passwords do not match');
    }
    setFormData({ ...formData, confirmPassword: value });
  };

  // Problem: Trudno zobaczyć całą strukturę formularza
  return (
    <form>
      <input value={formData.username} onChange={(e) => handleUsernameChange(e.target.value)} />
      <input type="password" value={formData.password} onChange={(e) => handlePasswordChange(e.target.value)} />
      <input type="password" value={formData.confirmPassword} onChange={(e) => handleConfirmPasswordChange(e.target.value)} />
      <button type="submit">Register</button>
    </form>
  );
}
