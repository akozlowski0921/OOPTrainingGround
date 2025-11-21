import React, { useState } from 'react';

// ❌ BAD: Rendering large lists without virtualization

interface Product {
  id: number;
  name: string;
  price: number;
  category: string;
  description: string;
}

// Problem: Renderowanie 10,000 elementów bez virtualization
export function LargeListWithoutVirtualization() {
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

  // Problem: Filtrowanie wszystkich 10,000 elementów przy każdym renderze
  const filteredProducts = products.filter(p =>
    p.name.toLowerCase().includes(filter.toLowerCase()) ||
    p.category.toLowerCase().includes(filter.toLowerCase())
  );

  console.log('Rendering', filteredProducts.length, 'products');

  return (
    <div>
      <input
        type="text"
        placeholder="Filter products..."
        value={filter}
        onChange={(e) => setFilter(e.target.value)}
      />
      
      {/* Problem: Wszystkie elementy renderowane na raz */}
      <div style={{ height: '600px', overflow: 'auto' }}>
        {filteredProducts.map(product => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
}

// Problem: Komponent re-renderuje się niepotrzebnie
function ProductCard({ product }: { product: Product }) {
  console.log('Rendering ProductCard', product.id);

  // Problem: Ciężkie obliczenia przy każdym renderze
  const formattedPrice = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(product.price);

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
}

// Problem: Brak memoization dla kosztownych obliczeń
export function ExpensiveCalculations() {
  const [count, setCount] = useState(0);
  const [input, setInput] = useState('');

  // Problem: Fibonacci wykonywany przy każdym renderze (nawet gdy count się nie zmienia)
  const fibonacci = (n: number): number => {
    if (n <= 1) return n;
    return fibonacci(n - 1) + fibonacci(n - 2);
  };

  const result = fibonacci(count);

  return (
    <div>
      <input
        value={input}
        onChange={(e) => setInput(e.target.value)}
        placeholder="Type something..."
      />
      <button onClick={() => setCount(count + 1)}>
        Calculate Fibonacci({count})
      </button>
      <p>Result: {result}</p>
    </div>
  );
}

// Problem: Renderowanie wszystkich danych tabelarycznych bez optymalizacji
export function LargeTable() {
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

  return (
    <div style={{ height: '500px', overflow: 'auto' }}>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Email</th>
            <th>Age</th>
            <th>Salary</th>
            <th>Department</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {/* Problem: Wszystkie 1000 wierszy renderowane naraz */}
          {data.map(row => (
            <tr key={row.id}>
              <td>{row.id}</td>
              <td>{row.name}</td>
              <td>{row.email}</td>
              <td>{row.age}</td>
              <td>${row.salary.toLocaleString()}</td>
              <td>{row.department}</td>
              <td>{row.status}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

// Problem: Infinite scroll bez virtualization
export function InfiniteScrollNaive() {
  const [items, setItems] = useState<number[]>(
    Array.from({ length: 50 }, (_, i) => i)
  );
  const [loading, setLoading] = useState(false);

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    const target = e.currentTarget;
    const bottom = target.scrollHeight - target.scrollTop === target.clientHeight;

    if (bottom && !loading) {
      setLoading(true);
      // Symulacja ładowania więcej danych
      setTimeout(() => {
        setItems(prev => [
          ...prev,
          ...Array.from({ length: 50 }, (_, i) => prev.length + i)
        ]);
        setLoading(false);
      }, 1000);
    }
  };

  // Problem: Im więcej załadowanych elementów, tym wolniej działa scroll
  return (
    <div
      style={{ height: '400px', overflow: 'auto' }}
      onScroll={handleScroll}
    >
      {items.map(item => (
        <div key={item} style={{ padding: '20px', border: '1px solid #ddd' }}>
          Item {item}
          <ExpensiveItemContent />
        </div>
      ))}
      {loading && <div>Loading more...</div>}
    </div>
  );
}

function ExpensiveItemContent() {
  // Symulacja ciężkiego komponentu
  const dummy = Array.from({ length: 100 }, (_, i) => i).reduce((a, b) => a + b);
  return <span>{dummy}</span>;
}
