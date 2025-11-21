import React, { useState, memo, useCallback, useMemo } from 'react';

// ✅ GOOD: Using React.memo with proper memoization

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

// React.memo prevents re-renders when props don't change
const MessageItem = memo(({ message, user }: { message: Message; user: User }) => {
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
});

export function ChatAppOptimized() {
  const [messages, setMessages] = useState<Message[]>([
    { id: 1, userId: 1, text: 'Hello!', timestamp: new Date() },
    { id: 2, userId: 2, text: 'Hi there!', timestamp: new Date() }
  ]);
  
  const [users] = useState<User[]>([
    { id: 1, name: 'Alice', avatar: '👩' },
    { id: 2, name: 'Bob', avatar: '👨' }
  ]);

  const [inputValue, setInputValue] = useState('');

  // useCallback zapewnia stabilną referencję funkcji
  const sendMessage = useCallback(() => {
    setMessages(prev => [...prev, {
      id: prev.length + 1,
      userId: 1,
      text: inputValue,
      timestamp: new Date()
    }]);
    setInputValue('');
  }, [inputValue]);

  // Teraz zmiana inputValue nie powoduje re-render MessageItem (są zmemoizowane)
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

// useMemo dla obiektów i useCallback dla funkcji
export function ProductListOptimized() {
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');
  const [selectedCategory, setSelectedCategory] = useState('all');

  const products = useMemo(() => [
    { id: 1, name: 'Laptop', category: 'electronics', price: 1000 },
    { id: 2, name: 'Phone', category: 'electronics', price: 500 },
    { id: 3, name: 'Shirt', category: 'clothing', price: 50 }
  ], []);

  // Memoized config - ta sama referencja jeśli wartości się nie zmieniły
  const config = useMemo(() => ({
    showPrice: true,
    currency: 'USD'
  }), []);

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
        <ProductItemOptimized
          key={product.id}
          product={product}
          config={config}
          onSelect={useCallback(() => console.log('Selected', product.id), [product.id])}
        />
      ))}
    </div>
  );
}

const ProductItemOptimized = memo(({ 
  product, 
  config, 
  onSelect 
}: { 
  product: { id: number; name: string; category: string; price: number };
  config: { showPrice: boolean; currency: string };
  onSelect: () => void;
}) => {
  console.log('Rendering ProductItem', product.id);
  
  return (
    <div onClick={onSelect}>
      <h3>{product.name}</h3>
      {config.showPrice && <p>{config.currency} {product.price}</p>}
    </div>
  );
});

// Custom comparator dla React.memo z głębokimi obiektami
export function ComplexDataListOptimized() {
  const [refresh, setRefresh] = useState(0);

  const items = useMemo(() => [
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
  ], []);

  return (
    <div>
      <button onClick={() => setRefresh(r => r + 1)}>
        Refresh ({refresh})
      </button>
      
      {items.map(item => (
        <ComplexItemOptimized key={item.id} item={item} />
      ))}
    </div>
  );
}

// Custom comparator - porównaj tylko ID i kluczowe pola
const ComplexItemOptimized = memo(({ item }: { 
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
}, (prevProps, nextProps) => {
  // Custom comparison - porównaj tylko istotne pola
  return prevProps.item.id === nextProps.item.id &&
         prevProps.item.data.name === nextProps.item.data.name &&
         prevProps.item.data.details.color === nextProps.item.data.details.color &&
         prevProps.item.data.details.size === nextProps.item.data.details.size;
});

// useCallback dla memoized callbacks
export function FormWithCallbacksOptimized() {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: ''
  });

  // useCallback zapewnia stabilną referencję
  const handleNameChange = useCallback((value: string) => {
    setFormData(prev => ({ ...prev, name: value }));
  }, []);

  const handleEmailChange = useCallback((value: string) => {
    setFormData(prev => ({ ...prev, email: value }));
  }, []);

  const handlePhoneChange = useCallback((value: string) => {
    setFormData(prev => ({ ...prev, phone: value }));
  }, []);

  return (
    <div>
      <FormFieldOptimized
        label="Name"
        value={formData.name}
        onChange={handleNameChange}
      />
      <FormFieldOptimized
        label="Email"
        value={formData.email}
        onChange={handleEmailChange}
      />
      <FormFieldOptimized
        label="Phone"
        value={formData.phone}
        onChange={handlePhoneChange}
      />
    </div>
  );
}

const FormFieldOptimized = memo(({ 
  label, 
  value, 
  onChange 
}: { 
  label: string; 
  value: string; 
  onChange: (value: string) => void;
}) => {
  console.log('Rendering FormField', label);
  
  return (
    <div>
      <label>{label}</label>
      <input value={value} onChange={(e) => onChange(e.target.value)} />
    </div>
  );
});

// Advanced: Selective re-rendering z context
import { createContext, useContext } from 'react';

const FormContext = createContext<{
  values: Record<string, string>;
  setValue: (key: string, value: string) => void;
} | null>(null);

export function OptimizedFormProvider({ children }: { children: React.ReactNode }) {
  const [values, setValues] = useState<Record<string, string>>({});

  const setValue = useCallback((key: string, value: string) => {
    setValues(prev => ({ ...prev, [key]: value }));
  }, []);

  const contextValue = useMemo(() => ({ values, setValue }), [values, setValue]);

  return (
    <FormContext.Provider value={contextValue}>
      {children}
    </FormContext.Provider>
  );
}

// Komponent re-renderuje się tylko gdy jego pole się zmienia
const OptimizedFormField = memo(({ fieldKey, label }: { fieldKey: string; label: string }) => {
  const context = useContext(FormContext);
  if (!context) throw new Error('Must be used within FormProvider');

  const { values, setValue } = context;
  const value = values[fieldKey] || '';

  console.log('Rendering OptimizedFormField', fieldKey);

  return (
    <div>
      <label>{label}</label>
      <input 
        value={value} 
        onChange={(e) => setValue(fieldKey, e.target.value)} 
      />
    </div>
  );
}, (prev, next) => prev.fieldKey === next.fieldKey && prev.label === next.label);
