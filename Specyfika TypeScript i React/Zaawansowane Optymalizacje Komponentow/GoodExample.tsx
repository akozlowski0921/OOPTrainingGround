import React, { useState, useMemo, memo } from 'react';
import { FixedSizeList } from 'react-window';

// ✅ GOOD: Using react-window for virtualization

interface Product {
  id: number;
  name: string;
  price: number;
  category: string;
  description: string;
}

// Proper virtualization with react-window
export function LargeListWithVirtualization() {
  const [products] = useState<Product[]>(() =>
    Array.from({ length: 10000 }, (_, i) => ({
      id: i,
      name: `Product ${i}`,
      price: Math.random() * 1000,
      category: ['Electronics', 'Clothing', 'Food'][i % 3],
      description: `Description for product ${i}`.repeat(5)
    }))
  );

  const [filter, setFilter] = useState('');

  // Memoize filtered products
  const filteredProducts = useMemo(() => {
    console.log('Filtering products...');
    return products.filter(p =>
      p.name.toLowerCase().includes(filter.toLowerCase()) ||
      p.category.toLowerCase().includes(filter.toLowerCase())
    );
  }, [products, filter]);

  // Row renderer for react-window
  const Row = ({ index, style }: { index: number; style: React.CSSProperties }) => (
    <div style={style}>
      <ProductCard product={filteredProducts[index]} />
    </div>
  );

  return (
    <div>
      <input
        type="text"
        placeholder="Filter products..."
        value={filter}
        onChange={(e) => setFilter(e.target.value)}
      />
      
      {/* react-window renderuje tylko widoczne elementy */}
      <FixedSizeList
        height={600}
        itemCount={filteredProducts.length}
        itemSize={150}
        width="100%"
      >
        {Row}
      </FixedSizeList>
    </div>
  );
}

// Memoized component z useMemo dla kosztownych obliczeń
const ProductCard = memo(({ product }: { product: Product }) => {
  console.log('Rendering ProductCard', product.id);

  // Memoize formatter - tworzy się tylko raz
  const priceFormatter = useMemo(
    () => new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }),
    []
  );

  const formattedPrice = useMemo(
    () => priceFormatter.format(product.price),
    [priceFormatter, product.price]
  );

  return (
    <div style={{ 
      padding: '20px', 
      border: '1px solid #ccc', 
      marginBottom: '10px' 
    }}>
      <h3>{product.name}</h3>
      <p>{product.category}</p>
      <p>{formattedPrice}</p>
      <p>{product.description}</p>
    </div>
  );
});

// useMemo dla kosztownych obliczeń
export function ExpensiveCalculationsOptimized() {
  const [count, setCount] = useState(0);
  const [input, setInput] = useState('');

  // Fibonacci z memoizacją - obliczany tylko gdy count się zmienia
  const result = useMemo(() => {
    console.log('Calculating fibonacci...');
    const fibonacci = (n: number): number => {
      if (n <= 1) return n;
      return fibonacci(n - 1) + fibonacci(n - 2);
    };
    return fibonacci(count);
  }, [count]);

  return (
    <div>
      <input
        value={input}
        onChange={(e) => setInput(e.target.value)}
        placeholder="Type something (no recalculation)..."
      />
      <button onClick={() => setCount(count + 1)}>
        Calculate Fibonacci({count})
      </button>
      <p>Result: {result}</p>
    </div>
  );
}

// react-window dla dużych tabel
import { FixedSizeGrid } from 'react-window';

export function LargeTableVirtualized() {
  const [data] = useState(() =>
    Array.from({ length: 1000 }, (_, i) => ({
      id: i,
      name: `User ${i}`,
      email: `user${i}@example.com`,
      age: Math.floor(Math.random() * 80) + 18,
      salary: Math.floor(Math.random() * 100000) + 30000,
      department: ['IT', 'HR', 'Sales', 'Marketing'][i % 4],
      status: ['Active', 'Inactive'][i % 2]
    }))
  );

  const columns = ['ID', 'Name', 'Email', 'Age', 'Salary', 'Department', 'Status'];

  const Cell = ({ 
    columnIndex, 
    rowIndex, 
    style 
  }: { 
    columnIndex: number; 
    rowIndex: number; 
    style: React.CSSProperties;
  }) => {
    if (rowIndex === 0) {
      // Header row
      return (
        <div style={{ ...style, fontWeight: 'bold', background: '#f0f0f0' }}>
          {columns[columnIndex]}
        </div>
      );
    }

    const row = data[rowIndex - 1];
    const values = [
      row.id,
      row.name,
      row.email,
      row.age,
      `$${row.salary.toLocaleString()}`,
      row.department,
      row.status
    ];

    return (
      <div style={{ ...style, borderBottom: '1px solid #ddd' }}>
        {values[columnIndex]}
      </div>
    );
  };

  return (
    <FixedSizeGrid
      columnCount={columns.length}
      columnWidth={150}
      height={500}
      rowCount={data.length + 1} // +1 for header
      rowHeight={35}
      width={1050}
    >
      {Cell}
    </FixedSizeGrid>
  );
}

// react-window dla infinite scroll
import { VariableSizeList } from 'react-window';
import InfiniteLoader from 'react-window-infinite-loader';

export function InfiniteScrollVirtualized() {
  const [items, setItems] = useState<number[]>(
    Array.from({ length: 50 }, (_, i) => i)
  );
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);

  const loadMoreItems = () => {
    if (loading || !hasMore) return Promise.resolve();
    
    setLoading(true);
    return new Promise<void>((resolve) => {
      setTimeout(() => {
        setItems(prev => {
          const newItems = [
            ...prev,
            ...Array.from({ length: 50 }, (_, i) => prev.length + i)
          ];
          if (newItems.length >= 500) {
            setHasMore(false);
          }
          return newItems;
        });
        setLoading(false);
        resolve();
      }, 1000);
    });
  };

  const isItemLoaded = (index: number) => !hasMore || index < items.length;

  const Item = ({ index, style }: { index: number; style: React.CSSProperties }) => {
    if (!isItemLoaded(index)) {
      return <div style={style}>Loading...</div>;
    }

    return (
      <div style={{ ...style, padding: '20px', border: '1px solid #ddd' }}>
        Item {items[index]}
        <MemoizedExpensiveContent />
      </div>
    );
  };

  return (
    <InfiniteLoader
      isItemLoaded={isItemLoaded}
      itemCount={hasMore ? items.length + 1 : items.length}
      loadMoreItems={loadMoreItems}
    >
      {({ onItemsRendered, ref }) => (
        <FixedSizeList
          height={400}
          itemCount={hasMore ? items.length + 1 : items.length}
          itemSize={80}
          onItemsRendered={onItemsRendered}
          ref={ref}
          width="100%"
        >
          {Item}
        </FixedSizeList>
      )}
    </InfiniteLoader>
  );
}

const MemoizedExpensiveContent = memo(() => {
  const dummy = useMemo(
    () => Array.from({ length: 100 }, (_, i) => i).reduce((a, b) => a + b),
    []
  );
  return <span>{dummy}</span>;
});
