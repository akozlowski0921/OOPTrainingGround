import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

// ✅ GOOD: Schema-driven form generation with type safety

// Define form schema
const formSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email address'),
  age: z.number().min(18, 'Must be at least 18'),
  country: z.string().min(1, 'Country is required'),
  newsletter: z.boolean(),
  interests: z.array(z.string()).optional()
});

type FormData = z.infer<typeof formSchema>;

// JSON schema for dynamic form generation
interface FieldSchema {
  name: keyof FormData;
  type: 'text' | 'email' | 'number' | 'select' | 'checkbox' | 'multiselect';
  label: string;
  placeholder?: string;
  options?: Array<{ value: string; label: string }>;
}

const formFieldsSchema: FieldSchema[] = [
  { name: 'firstName', type: 'text', label: 'First Name', placeholder: 'Enter first name' },
  { name: 'lastName', type: 'text', label: 'Last Name', placeholder: 'Enter last name' },
  { name: 'email', type: 'email', label: 'Email', placeholder: 'Enter email' },
  { name: 'age', type: 'number', label: 'Age', placeholder: 'Enter age' },
  {
    name: 'country',
    type: 'select',
    label: 'Country',
    options: [
      { value: 'US', label: 'United States' },
      { value: 'UK', label: 'United Kingdom' },
      { value: 'PL', label: 'Poland' }
    ]
  },
  { name: 'newsletter', type: 'checkbox', label: 'Subscribe to newsletter' }
];

// Reusable field renderer
function FormFieldRenderer({ field, control, errors }: {
  field: FieldSchema;
  control: any;
  errors: any;
}) {
  return (
    <div style={{ marginBottom: '1rem' }}>
      <label>{field.label}</label>
      <Controller
        name={field.name}
        control={control}
        render={({ field: controllerField }) => {
          switch (field.type) {
            case 'text':
            case 'email':
              return (
                <input
                  {...controllerField}
                  type={field.type}
                  placeholder={field.placeholder}
                />
              );
            
            case 'number':
              return (
                <input
                  {...controllerField}
                  type="number"
                  placeholder={field.placeholder}
                  onChange={(e) => controllerField.onChange(parseInt(e.target.value) || 0)}
                />
              );
            
            case 'select':
              return (
                <select {...controllerField}>
                  <option value="">Select...</option>
                  {field.options?.map(opt => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              );
            
            case 'checkbox':
              return (
                <input
                  {...controllerField}
                  type="checkbox"
                  checked={controllerField.value}
                />
              );
            
            default:
              return null;
          }
        }}
      />
      {errors[field.name] && (
        <span style={{ color: 'red', fontSize: '0.875rem' }}>
          {errors[field.name]?.message}
        </span>
      )}
    </div>
  );
}

// Schema-driven form component
export function SchemaDrivenForm() {
  const {
    control,
    handleSubmit,
    formState: { errors }
  } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      age: 0,
      country: '',
      newsletter: false,
      interests: []
    }
  });

  const onSubmit = (data: FormData) => {
    console.log('Form submitted:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {formFieldsSchema.map((field) => (
        <FormFieldRenderer
          key={field.name}
          field={field}
          control={control}
          errors={errors}
        />
      ))}
      <button type="submit">Submit</button>
    </form>
  );
}

// Easy to extend - just add to schema
const extendedFormSchema = z.object({
  ...formSchema.shape,
  phone: z.string().regex(/^\+?[1-9]\d{1,14}$/, 'Invalid phone number')
});

type ExtendedFormData = z.infer<typeof extendedFormSchema>;

const extendedFieldsSchema: Array<{
  name: keyof ExtendedFormData;
  type: 'text' | 'email' | 'number' | 'select' | 'checkbox';
  label: string;
  placeholder?: string;
}> = [
  ...formFieldsSchema,
  { name: 'phone', type: 'text', label: 'Phone', placeholder: 'Enter phone number' }
];

export function ExtendedSchemaForm() {
  const {
    control,
    handleSubmit,
    formState: { errors }
  } = useForm<ExtendedFormData>({
    resolver: zodResolver(extendedFormSchema)
  });

  const onSubmit = (data: ExtendedFormData) => {
    console.log('Extended form submitted:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {extendedFieldsSchema.map((field) => (
        <FormFieldRenderer
          key={field.name}
          field={field as any}
          control={control}
          errors={errors}
        />
      ))}
      <button type="submit">Submit</button>
    </form>
  );
}

// Centralized configuration
const registrationSchema = z.object({
  username: z.string().min(3, 'Username must be at least 3 characters'),
  password: z.string().min(8, 'Password must be at least 8 characters')
    .regex(/[A-Z]/, 'Must contain uppercase letter')
    .regex(/[0-9]/, 'Must contain number'),
  confirmPassword: z.string()
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword']
});

type RegistrationData = z.infer<typeof registrationSchema>;

export function CentralizedConfigForm() {
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<RegistrationData>({
    resolver: zodResolver(registrationSchema)
  });

  const onSubmit = (data: RegistrationData) => {
    console.log('Registration data:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div>
        <label>Username</label>
        <input {...register('username')} />
        {errors.username && <span>{errors.username.message}</span>}
      </div>

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

      <button type="submit">Register</button>
    </form>
  );
}

// Loading form schema from API/JSON
export function DynamicFormFromJSON() {
  const [formConfig, setFormConfig] = React.useState<FieldSchema[] | null>(null);

  React.useEffect(() => {
    // Simulate loading form config from API
    setTimeout(() => {
      setFormConfig(formFieldsSchema);
    }, 1000);
  }, []);

  const {
    control,
    handleSubmit,
    formState: { errors }
  } = useForm<FormData>({
    resolver: zodResolver(formSchema)
  });

  if (!formConfig) {
    return <div>Loading form...</div>;
  }

  const onSubmit = (data: FormData) => {
    console.log('Dynamic form submitted:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {formConfig.map((field) => (
        <FormFieldRenderer
          key={field.name}
          field={field}
          control={control}
          errors={errors}
        />
      ))}
      <button type="submit">Submit</button>
    </form>
  );
}
