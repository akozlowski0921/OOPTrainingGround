import React, { useState } from 'react';

// ❌ BAD: Manual validation without schema validation library

interface UserFormData {
  username: string;
  email: string;
  password: string;
  age: number;
  website: string;
  terms: boolean;
}

// Problem: Ręczna walidacja każdego pola - nie skalowalne
export function ManualValidation() {
  const [formData, setFormData] = useState<UserFormData>({
    username: '',
    email: '',
    password: '',
    age: 0,
    website: '',
    terms: false
  });

  const [errors, setErrors] = useState<Partial<Record<keyof UserFormData, string>>>({});
  const [touched, setTouched] = useState<Partial<Record<keyof UserFormData, boolean>>>({});

  // Problem: Duplikacja logiki walidacji
  const validateUsername = (value: string): string | undefined => {
    if (!value) return 'Username is required';
    if (value.length < 3) return 'Username must be at least 3 characters';
    if (!/^[a-zA-Z0-9_]+$/.test(value)) return 'Username can only contain letters, numbers and underscores';
    return undefined;
  };

  const validateEmail = (value: string): string | undefined => {
    if (!value) return 'Email is required';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) return 'Invalid email format';
    return undefined;
  };

  const validatePassword = (value: string): string | undefined => {
    if (!value) return 'Password is required';
    if (value.length < 8) return 'Password must be at least 8 characters';
    if (!/[A-Z]/.test(value)) return 'Password must contain an uppercase letter';
    if (!/[a-z]/.test(value)) return 'Password must contain a lowercase letter';
    if (!/[0-9]/.test(value)) return 'Password must contain a number';
    if (!/[!@#$%^&*]/.test(value)) return 'Password must contain a special character';
    return undefined;
  };

  const validateAge = (value: number): string | undefined => {
    if (!value) return 'Age is required';
    if (value < 13) return 'You must be at least 13 years old';
    if (value > 120) return 'Invalid age';
    return undefined;
  };

  const validateWebsite = (value: string): string | undefined => {
    if (!value) return undefined; // Optional
    try {
      new URL(value);
      return undefined;
    } catch {
      return 'Invalid URL format';
    }
  };

  const validateTerms = (value: boolean): string | undefined => {
    if (!value) return 'You must accept the terms and conditions';
    return undefined;
  };

  // Problem: onChange handler dla każdego pola
  const handleUsernameChange = (value: string) => {
    setFormData({ ...formData, username: value });
    if (touched.username) {
      const error = validateUsername(value);
      setErrors({ ...errors, username: error });
    }
  };

  const handleEmailChange = (value: string) => {
    setFormData({ ...formData, email: value });
    if (touched.email) {
      const error = validateEmail(value);
      setErrors({ ...errors, email: error });
    }
  };

  // ... więcej handlers dla każdego pola

  const handleBlur = (field: keyof UserFormData) => {
    setTouched({ ...touched, [field]: true });
    
    // Problem: Switch statement dla każdego pola
    let error: string | undefined;
    switch (field) {
      case 'username':
        error = validateUsername(formData.username);
        break;
      case 'email':
        error = validateEmail(formData.email);
        break;
      case 'password':
        error = validatePassword(formData.password);
        break;
      case 'age':
        error = validateAge(formData.age);
        break;
      case 'website':
        error = validateWebsite(formData.website);
        break;
      case 'terms':
        error = validateTerms(formData.terms);
        break;
    }
    
    setErrors({ ...errors, [field]: error });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // Problem: Walidacja wszystkich pól przed submitem
    const newErrors: Partial<Record<keyof UserFormData, string>> = {};
    newErrors.username = validateUsername(formData.username);
    newErrors.email = validateEmail(formData.email);
    newErrors.password = validatePassword(formData.password);
    newErrors.age = validateAge(formData.age);
    newErrors.website = validateWebsite(formData.website);
    newErrors.terms = validateTerms(formData.terms);

    // Filter out undefined errors
    Object.keys(newErrors).forEach(key => {
      if (!newErrors[key as keyof UserFormData]) {
        delete newErrors[key as keyof UserFormData];
      }
    });

    setErrors(newErrors);

    if (Object.keys(newErrors).length === 0) {
      console.log('Form submitted:', formData);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>Username</label>
        <input
          value={formData.username}
          onChange={(e) => handleUsernameChange(e.target.value)}
          onBlur={() => handleBlur('username')}
        />
        {touched.username && errors.username && <span>{errors.username}</span>}
      </div>

      <div>
        <label>Email</label>
        <input
          value={formData.email}
          onChange={(e) => handleEmailChange(e.target.value)}
          onBlur={() => handleBlur('email')}
        />
        {touched.email && errors.email && <span>{errors.email}</span>}
      </div>

      {/* ... więcej pól z podobną duplikacją */}

      <button type="submit">Register</button>
    </form>
  );
}

// Problem: Conditional validation bez schema
export function ConditionalValidationManual() {
  const [formData, setFormData] = useState({
    accountType: 'personal' as 'personal' | 'business',
    name: '',
    companyName: '',
    taxId: ''
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    // Problem: Conditional logic rozrzucona w kodzie
    if (!formData.name) {
      newErrors.name = 'Name is required';
    }

    if (formData.accountType === 'business') {
      if (!formData.companyName) {
        newErrors.companyName = 'Company name is required for business accounts';
      }
      if (!formData.taxId) {
        newErrors.taxId = 'Tax ID is required for business accounts';
      } else if (!/^\d{10}$/.test(formData.taxId)) {
        newErrors.taxId = 'Tax ID must be 10 digits';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  return (
    <form onSubmit={(e) => { e.preventDefault(); validateForm(); }}>
      <select 
        value={formData.accountType} 
        onChange={(e) => setFormData({ ...formData, accountType: e.target.value as any })}
      >
        <option value="personal">Personal</option>
        <option value="business">Business</option>
      </select>

      <input
        placeholder="Name"
        value={formData.name}
        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
      />
      {errors.name && <span>{errors.name}</span>}

      {formData.accountType === 'business' && (
        <>
          <input
            placeholder="Company Name"
            value={formData.companyName}
            onChange={(e) => setFormData({ ...formData, companyName: e.target.value })}
          />
          {errors.companyName && <span>{errors.companyName}</span>}

          <input
            placeholder="Tax ID"
            value={formData.taxId}
            onChange={(e) => setFormData({ ...formData, taxId: e.target.value })}
          />
          {errors.taxId && <span>{errors.taxId}</span>}
        </>
      )}

      <button type="submit">Submit</button>
    </form>
  );
}
