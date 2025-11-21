import React, { Suspense } from 'react';

// ✅ GOOD: Route-based code splitting

const Home = React.lazy(() => import('./pages/Home'));
const About = React.lazy(() => import('./pages/About'));
const Contact = React.lazy(() => import('./pages/Contact'));

export function GoodRoutes() {
  const [route, setRoute] = React.useState('home');

  return (
    <div>
      <nav>
        <button onClick={() => setRoute('home')}>Home</button>
        <button onClick={() => setRoute('about')}>About</button>
        <button onClick={() => setRoute('contact')}>Contact</button>
      </nav>
      
      <Suspense fallback={<div>Loading page...</div>}>
        {route === 'home' && <Home />}
        {route === 'about' && <About />}
        {route === 'contact' && <Contact />}
      </Suspense>
    </div>
  );
}
