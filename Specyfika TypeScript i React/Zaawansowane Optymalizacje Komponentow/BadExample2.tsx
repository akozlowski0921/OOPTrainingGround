import React, { useState } from 'react';

// ❌ BAD: Not using React.memo and causing unnecessary re-renders

interface User {
  id: number;
  name: string;
  avatar: string;
}

interface Message {
  id: number;
  userId: number;
  text: string;
  timestamp: Date;
}

// Problem: Parent re-render powoduje re-render wszystkich dzieci
export function ChatApp() {
  const [messages, setMessages] = useState<Message[]>([
    { id: 1, userId: 1, text: 'Hello!', timestamp: new Date() },
    { id: 2, userId: 2, text: 'Hi there!', timestamp: new Date() }
  ]);
  
  const [users] = useState<User[]>([
    { id: 1, name: 'Alice', avatar: '👩' },
    { id: 2, name: 'Bob', avatar: '👨' }
  ]);

  const [inputValue, setInputValue] = useState('');

  const sendMessage = () => {
    setMessages(prev => [...prev, {
      id: prev.length + 1,
      userId: 1,
      text: inputValue,
      timestamp: new Date()
    }]);
    setInputValue('');
  };

  // Problem: Przy każdej zmianie inputValue wszystkie MessageItem re-renderują się
  return (
    <div>
      <div style={{ height: '400px', overflow: 'auto' }}>
        {messages.map(message => (
          <MessageItem
            key={message.id}
            message={message}
            user={users.find(u => u.id === message.userId)!}
          />
        ))}
      </div>
      
      <input
        value={inputValue}
        onChange={(e) => setInputValue(e.target.value)}
        placeholder="Type a message..."
      />
      <button onClick={sendMessage}>Send</button>
    </div>
  );
}

// Problem: Bez React.memo - re-renderuje się zawsze gdy parent się renderuje
function MessageItem({ message, user }: { message: Message; user: User }) {
  console.log('Rendering MessageItem', message.id);
  
  return (
    <div style={{ padding: '10px', borderBottom: '1px solid #eee' }}>
      <div style={{ fontWeight: 'bold' }}>
        {user.avatar} {user.name}
      </div>
      <div>{message.text}</div>
      <div style={{ fontSize: '12px', color: '#888' }}>
        {message.timestamp.toLocaleTimeString()}
      </div>
    </div>
  );
}

// Problem: Tworzenie nowych obiektów w render - łamie reference equality
export function ProductList() {
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');
  const [selectedCategory, setSelectedCategory] = useState('all');

  const products = [
    { id: 1, name: 'Laptop', category: 'electronics', price: 1000 },
    { id: 2, name: 'Phone', category: 'electronics', price: 500 },
    { id: 3, name: 'Shirt', category: 'clothing', price: 50 }
  ];

  return (
    <div>
      <button onClick={() => setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc')}>
        Toggle Sort
      </button>
      
      <select 
        value={selectedCategory} 
        onChange={(e) => setSelectedCategory(e.target.value)}
      >
        <option value="all">All</option>
        <option value="electronics">Electronics</option>
        <option value="clothing">Clothing</option>
      </select>

      {products.map(product => (
        <ProductItem
          key={product.id}
          product={product}
          // Problem: Nowy obiekt przy każdym renderze
          config={{ showPrice: true, currency: 'USD' }}
          // Problem: Nowa funkcja przy każdym renderze
          onSelect={() => console.log('Selected', product.id)}
        />
      ))}
    </div>
  );
}

function ProductItem({ 
  product, 
  config, 
  onSelect 
}: { 
  product: { id: number; name: string; category: string; price: number };
  config: { showPrice: boolean; currency: string };
  onSelect: () => void;
}) {
  console.log('Rendering ProductItem', product.id);
  
  return (
    <div onClick={onSelect}>
      <h3>{product.name}</h3>
      {config.showPrice && <p>{config.currency} {product.price}</p>}
    </div>
  );
}

// Problem: Głębokie porównanie obiektów bez custom comparator
export function ComplexDataList() {
  const [refresh, setRefresh] = useState(0);

  const items = [
    { 
      id: 1, 
      data: { name: 'Item 1', details: { color: 'red', size: 'L' } },
      metadata: { created: new Date('2024-01-01'), updated: new Date('2024-01-02') }
    },
    { 
      id: 2, 
      data: { name: 'Item 2', details: { color: 'blue', size: 'M' } },
      metadata: { created: new Date('2024-01-03'), updated: new Date('2024-01-04') }
    }
  ];

  return (
    <div>
      <button onClick={() => setRefresh(r => r + 1)}>
        Refresh ({refresh})
      </button>
      
      {items.map(item => (
        <ComplexItem key={item.id} item={item} />
      ))}
    </div>
  );
}

// Problem: React.memo z default shallow comparison nie pomoże dla głębokich obiektów
const ComplexItem = React.memo(({ item }: { 
  item: {
    id: number;
    data: { name: string; details: { color: string; size: string } };
    metadata: { created: Date; updated: Date };
  };
}) => {
  console.log('Rendering ComplexItem', item.id);
  
  return (
    <div>
      <h3>{item.data.name}</h3>
      <p>Color: {item.data.details.color}, Size: {item.data.details.size}</p>
      <p>Created: {item.metadata.created.toLocaleDateString()}</p>
    </div>
  );
});

// Problem: Brak memoization dla callback'ów
export function FormWithCallbacks() {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: ''
  });

  return (
    <div>
      {/* Problem: Nowa funkcja przy każdym renderze */}
      <FormField
        label="Name"
        value={formData.name}
        onChange={(value) => setFormData({ ...formData, name: value })}
      />
      <FormField
        label="Email"
        value={formData.email}
        onChange={(value) => setFormData({ ...formData, email: value })}
      />
      <FormField
        label="Phone"
        value={formData.phone}
        onChange={(value) => setFormData({ ...formData, phone: value })}
      />
    </div>
  );
}

const FormField = React.memo(({ 
  label, 
  value, 
  onChange 
}: { 
  label: string; 
  value: string; 
  onChange: (value: string) => void;
}) => {
  // Problem: onChange zawsze nowa referencja, więc memo nie działa
  console.log('Rendering FormField', label);
  
  return (
    <div>
      <label>{label}</label>
      <input value={value} onChange={(e) => onChange(e.target.value)} />
    </div>
  );
});
