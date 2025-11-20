import React, { useState, useEffect } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
  quantity: number;
}

// Problem: Redundantny stan - przechowujemy dane, które można wyliczyć
export function ShoppingCart() {
  const [products, setProducts] = useState<Product[]>([
    { id: 1, name: 'Laptop', price: 3000, quantity: 1 },
    { id: 2, name: 'Mouse', price: 50, quantity: 2 }
  ]);
  
  // Problem: Te wartości są redundantne - można je wyliczyć z products
  const [totalPrice, setTotalPrice] = useState(0);
  const [totalItems, setTotalItems] = useState(0);
  const [hasExpensiveItems, setHasExpensiveItems] = useState(false);

  // Problem: Musimy ręcznie synchronizować stan przy każdej zmianie
  useEffect(() => {
    const newTotal = products.reduce((sum, p) => sum + (p.price * p.quantity), 0);
    setTotalPrice(newTotal);
    
    const newItemCount = products.reduce((sum, p) => sum + p.quantity, 0);
    setTotalItems(newItemCount);
    
    const hasExpensive = products.some(p => p.price > 1000);
    setHasExpensiveItems(hasExpensive);
  }, [products]); // Ryzyko desynchronizacji jeśli zapomnimy wywołać ten effect

  const updateQuantity = (id: number, newQuantity: number) => {
    setProducts(products.map(p => 
      p.id === id ? { ...p, quantity: newQuantity } : p
    ));
    // Łatwo zapomnieć o aktualizacji - co jeśli dodamy nową właściwość?
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

// Problem: Nadmiarowy stan dla wartości filtrowanych
export function UserList() {
  const [users, setUsers] = useState([
    { id: 1, name: 'Jan Kowalski', age: 25, active: true },
    { id: 2, name: 'Anna Nowak', age: 30, active: false },
    { id: 3, name: 'Piotr Wiśniewski', age: 35, active: true }
  ]);
  
  const [searchTerm, setSearchTerm] = useState('');
  const [showActiveOnly, setShowActiveOnly] = useState(false);
  
  // Problem: Przechowujemy przefiltrowane dane jako osobny stan
  const [filteredUsers, setFilteredUsers] = useState(users);

  // Problem: Musimy ręcznie synchronizować filteredUsers
  useEffect(() => {
    let result = users;
    
    if (searchTerm) {
      result = result.filter(u => 
        u.name.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }
    
    if (showActiveOnly) {
      result = result.filter(u => u.active);
    }
    
    setFilteredUsers(result);
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
