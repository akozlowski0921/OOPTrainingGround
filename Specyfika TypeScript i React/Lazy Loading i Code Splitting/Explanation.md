# Lazy Loading i Code Splitting

## React.lazy
Dynamiczne importowanie komponentów.

```tsx
const HeavyComponent = React.lazy(() => import('./HeavyComponent'));

<Suspense fallback={<Loading />}>
  <HeavyComponent />
</Suspense>
```

## Dynamic Import
```tsx
const module = await import('./utils');
module.doSomething();
```

## Code Splitting Strategies

### 1. Route-based
```tsx
const Home = lazy(() => import('./pages/Home'));
const About = lazy(() => import('./pages/About'));

<Routes>
  <Route path="/" element={<Suspense fallback={<Loading />}><Home /></Suspense>} />
</Routes>
```

### 2. Component-based
```tsx
const HeavyChart = lazy(() => import('./HeavyChart'));

{showChart && (
  <Suspense fallback={<Spinner />}>
    <HeavyChart />
  </Suspense>
)}
```

### 3. Library splitting
```tsx
// Import heavy library only when needed
const loadPdfLib = () => import('heavy-pdf-library');
```

## Suspense
Obsługuje loading state dla lazy components.

```tsx
<Suspense fallback={<Skeleton />}>
  <LazyComponent />
</Suspense>
```

## Critical Path Optimization
1. **Inline critical CSS** - above the fold
2. **Defer non-critical JS** - below fold, modals
3. **Preload key resources** - `<link rel="preload">`
4. **Code split per route** - każda strona osobny bundle

## Benefits
✅ Mniejszy initial bundle size  
✅ Szybszy Time to Interactive  
✅ Better user experience  
✅ Load code on demand

## Best Practices
✅ Split per route  
✅ Lazy load modals, heavy components  
✅ Use Suspense with fallback  
✅ Preload dla anticipated navigation  
✅ Monitor bundle sizes

## Webpack/Vite
Automatyczny code splitting z dynamic imports.

```tsx
// Creates separate chunk
import('./LargeComponent').then(module => {
  module.default();
});
```
