import React, { useState, useMemo } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
  quantity: number;
}

// Rozwiązanie: Derived state - obliczamy wartości on the fly
export function ShoppingCart() {
  const [products, setProducts] = useState<Product[]>([
    { id: 1, name: 'Laptop', price: 3000, quantity: 1 },
    { id: 2, name: 'Mouse', price: 50, quantity: 2 }
  ]);

  // Derived state - obliczamy gdy potrzebujemy, brak redundancji
  const totalPrice = products.reduce((sum, p) => sum + (p.price * p.quantity), 0);
  const totalItems = products.reduce((sum, p) => sum + p.quantity, 0);
  const hasExpensiveItems = products.some(p => p.price > 1000);

  const updateQuantity = (id: number, newQuantity: number) => {
    setProducts(products.map(p => 
      p.id === id ? { ...p, quantity: newQuantity } : p
    ));
    // Nie musimy nic więcej robić - wartości są automatycznie aktualne
  };

  return (
    <div>
      <h2>Shopping Cart</h2>
      {products.map(product => (
        <div key={product.id}>
          <span>{product.name} - ${product.price}</span>
          <input 
            type="number" 
            value={product.quantity}
            onChange={(e) => updateQuantity(product.id, parseInt(e.target.value))}
          />
        </div>
      ))}
      <div>
        <p>Total Items: {totalItems}</p>
        <p>Total Price: ${totalPrice}</p>
        {hasExpensiveItems && <p>⚠️ Cart contains expensive items</p>}
      </div>
    </div>
  );
}

// Rozwiązanie: Obliczamy filtrowane dane on the fly, z useMemo dla optymalizacji
export function UserList() {
  const [users] = useState([
    { id: 1, name: 'Jan Kowalski', age: 25, active: true },
    { id: 2, name: 'Anna Nowak', age: 30, active: false },
    { id: 3, name: 'Piotr Wiśniewski', age: 35, active: true }
  ]);
  
  const [searchTerm, setSearchTerm] = useState('');
  const [showActiveOnly, setShowActiveOnly] = useState(false);
  
  // useMemo zapewnia że filtrowanie jest wykonane tylko gdy to konieczne
  const filteredUsers = useMemo(() => {
    let result = users;
    
    if (searchTerm) {
      result = result.filter(u => 
        u.name.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }
    
    if (showActiveOnly) {
      result = result.filter(u => u.active);
    }
    
    return result;
  }, [users, searchTerm, showActiveOnly]);

  return (
    <div>
      <input 
        type="text"
        placeholder="Search users..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      <label>
        <input 
          type="checkbox"
          checked={showActiveOnly}
          onChange={(e) => setShowActiveOnly(e.target.checked)}
        />
        Show active only
      </label>
      <ul>
        {filteredUsers.map(user => (
          <li key={user.id}>
            {user.name} - {user.age} years {user.active ? '✓' : '✗'}
          </li>
        ))}
      </ul>
    </div>
  );
}

// Alternatywnie: Jeśli obliczenia są bardzo proste i szybkie, można pominąć useMemo
export function SimpleUserList() {
  const [users] = useState([
    { id: 1, name: 'Jan Kowalski', age: 25, active: true },
    { id: 2, name: 'Anna Nowak', age: 30, active: false }
  ]);
  
  const [searchTerm, setSearchTerm] = useState('');

  // Dla prostych przypadków useMemo może być przesadą
  const filteredUsers = users.filter(u => 
    u.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div>
      <input 
        type="text"
        placeholder="Search users..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      <ul>
        {filteredUsers.map(user => (
          <li key={user.id}>{user.name}</li>
        ))}
      </ul>
    </div>
  );
}
