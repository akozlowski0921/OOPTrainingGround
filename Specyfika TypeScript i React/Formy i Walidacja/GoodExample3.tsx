import React from 'react';
import { useForm } from 'react-hook-form';

// ✅ GOOD: Proper error handling and reset

interface FormData {
  name: string;
  email: string;
}

export function GoodFormWithReset() {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting }
  } = useForm<FormData>();

  const [submitError, setSubmitError] = React.useState<string | null>(null);

  const onSubmit = async (data: FormData) => {
    try {
      setSubmitError(null);
      await fetch('/api/submit', {
        method: 'POST',
        body: JSON.stringify(data)
      });
      reset(); // ✅ Reset form po success
    } catch (error) {
      setSubmitError('Submission failed');
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register('name', { required: 'Name is required' })} />
      {errors.name && <span>{errors.name.message}</span>}
      
      <input {...register('email', { required: true })} type="email" />
      {errors.email && <span>Email is required</span>}
      
      {submitError && <div className="error">{submitError}</div>}
      
      <button disabled={isSubmitting}>
        {isSubmitting ? 'Submitting...' : 'Submit'}
      </button>
    </form>
  );
}
