import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

// ✅ GOOD: Multi-step wizard with proper state management

const wizardSchema = z.object({
  // Step 1
  name: z.string().min(1, 'Name is required'),
  email: z.string().email('Invalid email'),
  // Step 2
  address: z.string().min(1, 'Address is required'),
  city: z.string().min(1, 'City is required'),
  // Step 3
  cardNumber: z.string().regex(/^\d{16}$/, 'Card number must be 16 digits'),
  cvv: z.string().regex(/^\d{3}$/, 'CVV must be 3 digits')
});

type WizardFormData = z.infer<typeof wizardSchema>;

const steps = [
  { id: 1, title: 'Personal Info', fields: ['name', 'email'] as const },
  { id: 2, title: 'Address', fields: ['address', 'city'] as const },
  { id: 3, title: 'Payment', fields: ['cardNumber', 'cvv'] as const }
];

export function WizardForm() {
  const [currentStep, setCurrentStep] = useState(0);
  
  const {
    register,
    handleSubmit,
    trigger,
    getValues,
    formState: { errors }
  } = useForm<WizardFormData>({
    resolver: zodResolver(wizardSchema),
    mode: 'onBlur'
  });

  const handleNext = async () => {
    const fields = steps[currentStep].fields;
    const isValid = await trigger(fields as any);
    
    if (isValid) {
      setCurrentStep((prev) => Math.min(prev + 1, steps.length - 1));
    }
  };

  const handleBack = () => {
    setCurrentStep((prev) => Math.max(prev - 1, 0));
  };

  const onSubmit = (data: WizardFormData) => {
    console.log('Wizard completed:', data);
  };

  const currentStepData = steps[currentStep];

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div>
        <h2>Step {currentStep + 1} of {steps.length}: {currentStepData.title}</h2>
        <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
          {steps.map((step, index) => (
            <div
              key={step.id}
              style={{
                flex: 1,
                height: '4px',
                background: index <= currentStep ? '#007bff' : '#e0e0e0'
              }}
            />
          ))}
        </div>
      </div>

      {currentStep === 0 && (
        <div>
          <div>
            <label>Name</label>
            <input {...register('name')} />
            {errors.name && <span>{errors.name.message}</span>}
          </div>
          <div>
            <label>Email</label>
            <input {...register('email')} />
            {errors.email && <span>{errors.email.message}</span>}
          </div>
        </div>
      )}

      {currentStep === 1 && (
        <div>
          <div>
            <label>Address</label>
            <input {...register('address')} />
            {errors.address && <span>{errors.address.message}</span>}
          </div>
          <div>
            <label>City</label>
            <input {...register('city')} />
            {errors.city && <span>{errors.city.message}</span>}
          </div>
        </div>
      )}

      {currentStep === 2 && (
        <div>
          <div>
            <label>Card Number</label>
            <input {...register('cardNumber')} />
            {errors.cardNumber && <span>{errors.cardNumber.message}</span>}
          </div>
          <div>
            <label>CVV</label>
            <input {...register('cvv')} />
            {errors.cvv && <span>{errors.cvv.message}</span>}
          </div>
        </div>
      )}

      <div style={{ marginTop: '20px', display: 'flex', gap: '10px' }}>
        {currentStep > 0 && (
          <button type="button" onClick={handleBack}>Back</button>
        )}
        {currentStep < steps.length - 1 && (
          <button type="button" onClick={handleNext}>Next</button>
        )}
        {currentStep === steps.length - 1 && (
          <button type="submit">Submit</button>
        )}
      </div>
    </form>
  );
}

// Advanced: Conditional wizard flow
export function ConditionalWizardFlow() {
  const [userType, setUserType] = useState<'individual' | 'company'>('individual');
  
  const individualSteps = ['personal', 'contact', 'verification'];
  const companySteps = ['company', 'representative', 'contact', 'documents', 'verification'];
  
  const steps = userType === 'individual' ? individualSteps : companySteps;
  
  return (
    <div>
      <select value={userType} onChange={(e) => setUserType(e.target.value as any)}>
        <option value="individual">Individual</option>
        <option value="company">Company</option>
      </select>
      <p>Steps: {steps.join(' → ')}</p>
    </div>
  );
}
