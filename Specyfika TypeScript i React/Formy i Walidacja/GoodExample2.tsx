import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

// ✅ GOOD: Advanced validation with yup

const schema = yup.object({
  email: yup.string().email().required(),
  age: yup.number().min(18).required(),
  website: yup.string().url().nullable()
}).required();

type FormData = yup.InferType<typeof schema>;

export function GoodAdvancedForm() {
  const { register, handleSubmit, formState: { errors }, control } = 
    useForm<FormData>({ resolver: yupResolver(schema) });

  const onSubmit = (data: FormData) => console.log(data);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register('email')} />
      {errors.email && <span>{errors.email.message}</span>}
      
      <input {...register('age')} type="number" />
      {errors.age && <span>{errors.age.message}</span>}
      
      <input {...register('website')} />
      {errors.website && <span>{errors.website.message}</span>}
      
      <button>Submit</button>
    </form>
  );
}
