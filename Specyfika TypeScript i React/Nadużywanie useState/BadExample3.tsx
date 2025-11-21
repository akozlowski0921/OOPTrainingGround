import React, { useState } from 'react';

// ❌ BAD: useState dla danych które można obliczyć (derived state)

export function ShoppingCart() {
  const [items, setItems] = useState<CartItem[]>([]);
  
  // Problem: Derived state - można obliczyć z items
  const [itemCount, setItemCount] = useState(0);
  const [totalPrice, setTotalPrice] = useState(0);
  const [hasDiscount, setHasDiscount] = useState(false);

  const addItem = (item: CartItem) => {
    const newItems = [...items, item];
    setItems(newItems);
    
    // Problem: Musimy pamiętać o aktualizacji wszystkich powiązanych stanów
    setItemCount(newItems.length);
    setTotalPrice(newItems.reduce((sum, i) => sum + i.price * i.quantity, 0));
    setHasDiscount(newItems.length >= 5); // Łatwo zapomnieć!
  };

  const removeItem = (id: number) => {
    const newItems = items.filter(i => i.id !== id);
    setItems(newItems);
    
    // Problem: Duplikacja logiki obliczeniowej
    setItemCount(newItems.length);
    setTotalPrice(newItems.reduce((sum, i) => sum + i.price * i.quantity, 0));
    setHasDiscount(newItems.length >= 5);
  };

  return (
    <div>
      <div>Items: {itemCount}</div>
      <div>Total: ${totalPrice}</div>
      {hasDiscount && <div>Discount available!</div>}
    </div>
  );
}

interface CartItem {
  id: number;
  name: string;
  price: number;
  quantity: number;
}
