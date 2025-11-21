# Formy i Walidacja

## Controlled vs Uncontrolled

**Controlled:** React zarządza stanem
```tsx
const [value, setValue] = useState('');
<input value={value} onChange={e => setValue(e.target.value)} />
```

**Uncontrolled:** DOM zarządza stanem
```tsx
const inputRef = useRef<HTMLInputElement>(null);
<input ref={inputRef} defaultValue="initial" />
```

## React Hook Form

Library minimalizujące re-renders, używa uncontrolled inputs z ref.

```tsx
const { register, handleSubmit, formState: { errors } } = useForm<FormData>();
<input {...register('email', { required: true })} />
```

## Walidacja

**Zod:** Type-safe schema validation
```tsx
const schema = z.object({
  email: z.string().email(),
  age: z.number().min(18)
});
```

**Yup:** Popular validation library
```tsx
const schema = yup.object({
  email: yup.string().email().required()
});
```

## Best Practices
✅ React Hook Form dla performance  
✅ Zod/Yup dla complex validation  
✅ Custom validators gdy needed  
✅ Error messages accessible
