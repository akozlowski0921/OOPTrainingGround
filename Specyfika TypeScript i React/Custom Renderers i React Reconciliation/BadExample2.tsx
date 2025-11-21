import React from 'react';

// ❌ BAD: Direct DOM manipulation ignoring React Fiber and reconciliation

interface ListItem {
  id: number;
  text: string;
  completed: boolean;
}

// Problem: Bezpośrednia manipulacja DOM, omijanie React Fiber
export class TodoListDirect extends React.Component<{}, { items: ListItem[] }> {
  private listRef = React.createRef<HTMLUListElement>();

  state = {
    items: [
      { id: 1, text: 'Learn React', completed: false },
      { id: 2, text: 'Learn TypeScript', completed: false }
    ]
  };

  // Problem: Bezpośrednia manipulacja DOM zamiast pozwolić React zarządzać
  addItem = (text: string) => {
    const newItem = { 
      id: Date.now(), 
      text, 
      completed: false 
    };

    // Problem 1: Manipulujemy DOM bezpośrednio
    if (this.listRef.current) {
      const li = document.createElement('li');
      li.textContent = text;
      li.dataset.id = String(newItem.id);
      this.listRef.current.appendChild(li);
    }

    // Problem 2: Stan nie jest zsynchronizowany z DOM
    // Ten setState może wywołać re-render, który nadpisze nasze zmiany DOM
    this.setState(prev => ({
      items: [...prev.items, newItem]
    }));
  };

  // Problem: Ręczne zarządzanie DOM zamiast używać React state
  toggleItem = (id: number) => {
    // Szukamy elementu w DOM
    const element = this.listRef.current?.querySelector(`[data-id="${id}"]`);
    if (element) {
      // Modyfikujemy DOM bezpośrednio
      element.classList.toggle('completed');
      element.setAttribute('aria-checked', 
        element.classList.contains('completed') ? 'true' : 'false'
      );
    }

    // Stan aktualizujemy później - niespójna synchronizacja
    setTimeout(() => {
      this.setState(prev => ({
        items: prev.items.map(item =>
          item.id === id ? { ...item, completed: !item.completed } : item
        )
      }));
    }, 100);
  };

  // Problem: Ręczne usuwanie z DOM
  removeItem = (id: number) => {
    const element = this.listRef.current?.querySelector(`[data-id="${id}"]`);
    if (element) {
      // Animacja usunięcia - manipulacja DOM
      element.style.transition = 'opacity 0.3s';
      element.style.opacity = '0';
      
      setTimeout(() => {
        element.remove();
        // Aktualizacja stanu po usunięciu z DOM
        this.setState(prev => ({
          items: prev.items.filter(item => item.id !== id)
        }));
      }, 300);
    }
  };

  // Problem: Mix React rendering i DOM manipulation
  render() {
    return (
      <div>
        <ul ref={this.listRef}>
          {/* React renderuje początkową listę */}
          {this.state.items.map(item => (
            <li 
              key={item.id} 
              data-id={item.id}
              className={item.completed ? 'completed' : ''}
            >
              {item.text}
              <button onClick={() => this.toggleItem(item.id)}>Toggle</button>
              <button onClick={() => this.removeItem(item.id)}>Remove</button>
            </li>
          ))}
        </ul>
        <button onClick={() => this.addItem('New item')}>Add Item</button>
      </div>
    );
  }
}

// Problem: Próba "hakowania" React internals bez zrozumienia Fiber
export function ManualFiberManipulation() {
  const elementRef = React.useRef<HTMLDivElement>(null);

  React.useEffect(() => {
    if (!elementRef.current) return;

    // Problem: Próba dostępu do React internals (Fiber node)
    // @ts-ignore - ignorowanie błędów TypeScript to zły znak!
    const fiberNode = elementRef.current._reactInternalInstance;
    
    if (fiberNode) {
      // Problem: Bezpośrednia modyfikacja Fiber tree - bardzo niebezpieczne!
      // @ts-ignore
      fiberNode.memoizedState = { modified: true };
      
      // Problem: Wymuszanie update'u przez manipulację internals
      // @ts-ignore
      fiberNode.alternate = null;
    }
  }, []);

  return (
    <div ref={elementRef}>
      This is dangerous!
    </div>
  );
}

// Problem: Ręczne zarządzanie reconciliation zamiast pozwolić React
export class ManualReconciliation extends React.Component<
  { items: string[] },
  { renderedItems: string[] }
> {
  private mountedItems = new Set<string>();

  state = {
    renderedItems: this.props.items
  };

  componentDidUpdate(prevProps: { items: string[] }) {
    // Problem: Ręczna implementacja reconciliation logic
    const newItems = this.props.items;
    const oldItems = prevProps.items;

    // Znajdź dodane
    const added = newItems.filter(item => !oldItems.includes(item));
    
    // Znajdź usunięte
    const removed = oldItems.filter(item => !newItems.includes(item));

    // Problem: Próba "optymalizacji" przez ręczne zarządzanie
    // React robi to lepiej i szybciej!
    if (added.length > 0 || removed.length > 0) {
      // Dodaj nowe
      added.forEach(item => {
        this.mountedItems.add(item);
      });

      // Usuń stare
      removed.forEach(item => {
        this.mountedItems.delete(item);
      });

      // Wymuszamy update
      this.forceUpdate();
    }
  }

  render() {
    // Problem: Renderujemy bazując na ręcznie zarządzanym state
    // zamiast na props, co może prowadzić do niespójności
    return (
      <ul>
        {Array.from(this.mountedItems).map(item => (
          <li key={item}>{item}</li>
        ))}
      </ul>
    );
  }
}
