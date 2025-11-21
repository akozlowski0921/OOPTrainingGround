// ✅ GOOD: AbortController dla cancellation, proper cleanup, no race conditions
import { useEffect, useState } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
}

type AsyncState<T> = 
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'success'; data: T }
  | { status: 'error'; error: string };

function ProductSearch({ searchTerm }: { searchTerm: string }) {
  const [state, setState] = useState<AsyncState<Product[]>>({ status: 'idle' });

  useEffect(() => {
    // AbortController dla anulowania requestu
    const abortController = new AbortController();
    
    const fetchProducts = async () => {
      setState({ status: 'loading' });
      
      try {
        const response = await fetch(
          `https://api.example.com/products?search=${searchTerm}`,
          { signal: abortController.signal }
        );
        
        if (!response.ok) {
          throw new Error(`Error: ${response.status}`);
        }
        
        const data = await response.json();
        setState({ status: 'success', data });
      } catch (error) {
        // Ignoruj błędy abort
        if (error instanceof Error && error.name === 'AbortError') {
          return;
        }
        setState({ 
          status: 'error', 
          error: error instanceof Error ? error.message : 'Unknown error' 
        });
      }
    };

    if (searchTerm) {
      fetchProducts();
    }

    // Cleanup - anuluj request gdy komponent się odmontowuje lub searchTerm się zmienia
    return () => {
      abortController.abort();
    };
  }, [searchTerm]);

  if (state.status === 'loading') return <div>Searching...</div>;
  if (state.status === 'error') return <div>Error: {state.error}</div>;
  if (state.status === 'idle') return <div>Enter search term</div>;
  if (state.status === 'success' && state.data.length === 0) {
    return <div>No products found</div>;
  }

  return (
    <div>
      {state.status === 'success' && state.data.map(product => (
        <div key={product.id}>
          {product.name} - ${product.price}
        </div>
      ))}
    </div>
  );
}

export default ProductSearch;
