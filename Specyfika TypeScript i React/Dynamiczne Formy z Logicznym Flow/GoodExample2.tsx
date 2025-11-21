import React from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

// ✅ GOOD: Schema-based validation with Zod

// Comprehensive validation schema
const userSchema = z.object({
  username: z.string()
    .min(3, 'Username must be at least 3 characters')
    .regex(/^[a-zA-Z0-9_]+$/, 'Username can only contain letters, numbers and underscores'),
  email: z.string().email('Invalid email format'),
  password: z.string()
    .min(8, 'Password must be at least 8 characters')
    .regex(/[A-Z]/, 'Password must contain an uppercase letter')
    .regex(/[a-z]/, 'Password must contain a lowercase letter')
    .regex(/[0-9]/, 'Password must contain a number')
    .regex(/[!@#$%^&*]/, 'Password must contain a special character'),
  age: z.number()
    .min(13, 'You must be at least 13 years old')
    .max(120, 'Invalid age'),
  website: z.string().url('Invalid URL format').optional().or(z.literal('')),
  terms: z.boolean().refine((val) => val === true, {
    message: 'You must accept the terms and conditions'
  })
});

type UserFormData = z.infer<typeof userSchema>;

export function SchemaValidatedForm() {
  const {
    register,
    handleSubmit,
    formState: { errors, touchedFields }
  } = useForm<UserFormData>({
    resolver: zodResolver(userSchema),
    mode: 'onBlur'
  });

  const onSubmit = (data: UserFormData) => {
    console.log('Form submitted:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div>
        <label>Username</label>
        <input {...register('username')} />
        {errors.username && <span>{errors.username.message}</span>}
      </div>

      <div>
        <label>Email</label>
        <input {...register('email')} />
        {errors.email && <span>{errors.email.message}</span>}
      </div>

      <div>
        <label>Password</label>
        <input type="password" {...register('password')} />
        {errors.password && <span>{errors.password.message}</span>}
      </div>

      <div>
        <label>Age</label>
        <input 
          type="number" 
          {...register('age', { valueAsNumber: true })} 
        />
        {errors.age && <span>{errors.age.message}</span>}
      </div>

      <div>
        <label>Website (optional)</label>
        <input {...register('website')} />
        {errors.website && <span>{errors.website.message}</span>}
      </div>

      <div>
        <label>
          <input type="checkbox" {...register('terms')} />
          I accept the terms and conditions
        </label>
        {errors.terms && <span>{errors.terms.message}</span>}
      </div>

      <button type="submit">Register</button>
    </form>
  );
}

// Conditional validation with discriminated unions
const accountSchema = z.discriminatedUnion('accountType', [
  z.object({
    accountType: z.literal('personal'),
    name: z.string().min(1, 'Name is required')
  }),
  z.object({
    accountType: z.literal('business'),
    name: z.string().min(1, 'Name is required'),
    companyName: z.string().min(1, 'Company name is required'),
    taxId: z.string().regex(/^\d{10}$/, 'Tax ID must be 10 digits')
  })
]);

type AccountFormData = z.infer<typeof accountSchema>;

export function ConditionalValidationWithZod() {
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors }
  } = useForm<AccountFormData>({
    resolver: zodResolver(accountSchema),
    defaultValues: {
      accountType: 'personal',
      name: ''
    }
  });

  const accountType = watch('accountType');

  const onSubmit = (data: AccountFormData) => {
    console.log('Account created:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div>
        <label>Account Type</label>
        <select {...register('accountType')}>
          <option value="personal">Personal</option>
          <option value="business">Business</option>
        </select>
      </div>

      <div>
        <label>Name</label>
        <input {...register('name')} />
        {errors.name && <span>{errors.name.message}</span>}
      </div>

      {accountType === 'business' && (
        <>
          <div>
            <label>Company Name</label>
            <input {...register('companyName')} />
            {errors.companyName && <span>{errors.companyName.message}</span>}
          </div>

          <div>
            <label>Tax ID</label>
            <input {...register('taxId')} />
            {errors.taxId && <span>{errors.taxId.message}</span>}
          </div>
        </>
      )}

      <button type="submit">Submit</button>
    </form>
  );
}

// Advanced: Custom validation and async validation
const asyncUserSchema = z.object({
  username: z.string()
    .min(3)
    .refine(
      async (username) => {
        // Simulate API call to check username availability
        await new Promise(resolve => setTimeout(resolve, 500));
        return username !== 'admin'; // admin is taken
      },
      { message: 'Username is already taken' }
    ),
  email: z.string().email()
});

// Cross-field validation
const passwordSchema = z.object({
  password: z.string().min(8),
  confirmPassword: z.string()
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword']
});

type PasswordFormData = z.infer<typeof passwordSchema>;

export function CrossFieldValidation() {
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema)
  });

  const onSubmit = (data: PasswordFormData) => {
    console.log('Password set:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div>
        <label>Password</label>
        <input type="password" {...register('password')} />
        {errors.password && <span>{errors.password.message}</span>}
      </div>

      <div>
        <label>Confirm Password</label>
        <input type="password" {...register('confirmPassword')} />
        {errors.confirmPassword && <span>{errors.confirmPassword.message}</span>}
      </div>

      <button type="submit">Set Password</button>
    </form>
  );
}
