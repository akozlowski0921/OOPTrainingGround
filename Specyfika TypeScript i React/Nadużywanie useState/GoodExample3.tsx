import React, { useState, useMemo } from 'react';

// ✅ GOOD: Obliczanie derived state zamiast przechowywania

export function ShoppingCart() {
  const [items, setItems] = useState<CartItem[]>([]);

  // Derived values - obliczane z items
  const itemCount = items.length;
  const totalPrice = useMemo(
    () => items.reduce((sum, item) => sum + item.price * item.quantity, 0),
    [items]
  );
  const hasDiscount = itemCount >= 5;

  const addItem = (item: CartItem) => {
    // Tylko jedna aktualizacja state
    setItems([...items, item]);
    // itemCount, totalPrice, hasDiscount zaktualizują się automatycznie
  };

  const removeItem = (id: number) => {
    setItems(items.filter(i => i.id !== id));
    // Jedna aktualizacja, wszystko reszta się przeliczy
  };

  return (
    <div>
      <div>Items: {itemCount}</div>
      <div>Total: ${totalPrice.toFixed(2)}</div>
      {hasDiscount && <div>Discount available!</div>}
      <button onClick={() => addItem({id: Date.now(), name: 'Item', price: 10, quantity: 1})}>
        Add Item
      </button>
    </div>
  );
}

interface CartItem {
  id: number;
  name: string;
  price: number;
  quantity: number;
}
