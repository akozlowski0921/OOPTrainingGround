import React, { Suspense } from 'react';

// ✅ GOOD: Preloading i error boundaries

const HeavyChart = React.lazy(() => import('./HeavyChart'));

// Preload function
const preloadHeavyChart = () => {
  import('./HeavyChart');
};

class ErrorBoundary extends React.Component<
  { children: React.ReactNode },
  { hasError: boolean }
> {
  state = { hasError: false };

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  render() {
    if (this.state.hasError) {
      return <div>Something went wrong loading the component.</div>;
    }
    return this.props.children;
  }
}

export function GoodPreloadApp() {
  const [showChart, setShowChart] = React.useState(false);

  return (
    <div>
      <button 
        onMouseEnter={preloadHeavyChart} // ✅ Preload on hover
        onClick={() => setShowChart(true)}
      >
        Show Chart
      </button>
      
      {showChart && (
        <ErrorBoundary>
          <Suspense fallback={<div>Loading chart...</div>}>
            <HeavyChart />
          </Suspense>
        </ErrorBoundary>
      )}
    </div>
  );
}
