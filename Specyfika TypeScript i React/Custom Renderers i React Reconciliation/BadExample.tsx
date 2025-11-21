import React from 'react';

// ❌ BAD: Trying to use React for non-DOM rendering without proper renderer

interface TerminalElement {
  type: string;
  props: Record<string, any>;
  children: any[];
}

// Problem: Próba renderowania React komponentów do CLI bez właściwego renderer'a
export class TerminalApp {
  private buffer: string[] = [];

  // Naiwna próba "renderowania" React elementów do terminala
  render(element: React.ReactElement): string {
    // To nie zadziała prawidłowo - ignoruje React lifecycle, hooks, context
    return this.processElement(element);
  }

  private processElement(element: any): string {
    if (typeof element === 'string' || typeof element === 'number') {
      return String(element);
    }

    if (!element || !element.type) {
      return '';
    }

    // Problem: Wywołujemy komponent bezpośrednio jako funkcję
    // To pomija cały mechanizm React (hooks, context, lifecycle)
    if (typeof element.type === 'function') {
      const result = element.type(element.props);
      return this.processElement(result);
    }

    // Prosta konwersja do tekstu
    const children = React.Children.toArray(element.props?.children || []);
    const childrenText = children.map(child => this.processElement(child)).join('');

    switch (element.type) {
      case 'box':
        return `╔${'═'.repeat(50)}╗\n║ ${childrenText}${' '.repeat(49 - childrenText.length)}║\n╚${'═'.repeat(50)}╝\n`;
      case 'text':
        return childrenText;
      case 'line':
        return childrenText + '\n';
      default:
        return childrenText;
    }
  }
}

// Przykładowe komponenty CLI
function Title({ children }: { children: React.ReactNode }) {
  return React.createElement('box', {}, children);
}

function Paragraph({ children }: { children: React.ReactNode }) {
  return React.createElement('line', {}, children);
}

// Próba użycia hooków w CLI - to NIE ZADZIAŁA
function Counter() {
  // Problem: useState nie zadziała, bo brak właściwego renderer'a
  const [count, setCount] = React.useState(0);
  
  return React.createElement('text', {}, `Count: ${count}`);
}

// Użycie
export function renderTerminalApp() {
  const app = new TerminalApp();
  
  const element = React.createElement(
    'box',
    {},
    React.createElement(Title, {}, 'My CLI App'),
    React.createElement(Paragraph, {}, 'This is a paragraph'),
    React.createElement(Counter, {})
  );

  // Problem: Counter hook nie zadziała, nie ma lifecycle management
  return app.render(element);
}

// Problem z aktualizacjami stanu
export function InteractiveTerminalApp() {
  const app = new TerminalApp();
  let output = '';

  // Problem: Nie ma mechanizmu do aktualizacji UI gdy stan się zmienia
  const App = () => {
    const [count, setCount] = React.useState(0);
    
    // Ta funkcja nigdy nie zostanie wywołana prawidłowo
    setTimeout(() => setCount(prev => prev + 1), 1000);

    return React.createElement(
      'box',
      {},
      React.createElement('text', {}, `Count: ${count}`)
    );
  };

  // Renderuje tylko raz, nie reaguje na zmiany stanu
  output = app.render(React.createElement(App, {}));
  return output;
}
