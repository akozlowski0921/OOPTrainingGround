// ❌ BAD: Brak anulowania requestów, race conditions, memory leaks
import { useEffect, useState } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
}

function ProductSearch({ searchTerm }: { searchTerm: string }) {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    
    // Brak abort controller - może prowadzić do race conditions
    fetch(`https://api.example.com/products?search=${searchTerm}`)
      .then(res => res.json())
      .then(data => {
        setProducts(data);
        setLoading(false);
      })
      .catch(err => {
        console.error(err); // Słaba obsługa błędów
        setLoading(false);
      });
    
    // Brak cleanup - request nie jest anulowany gdy komponent się odmontowuje
  }, [searchTerm]);

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      {products.map(product => (
        <div key={product.id}>
          {product.name} - ${product.price}
        </div>
      ))}
    </div>
  );
}

export default ProductSearch;
