import React from 'react';
import Reconciler from 'react-reconciler';

// ✅ GOOD: Proper custom renderer using react-reconciler

interface TerminalInstance {
  type: string;
  props: Record<string, any>;
  children: TerminalInstance[];
  output: string;
}

type Container = {
  lines: string[];
  children: TerminalInstance[];
};

// Implementacja właściwego renderer'a dla terminala
const TerminalRenderer = Reconciler<
  string, // Type
  Record<string, any>, // Props
  Container, // Container
  TerminalInstance, // Instance
  any, // TextInstance
  any, // SuspenseInstance
  any, // HydratableInstance
  TerminalInstance, // PublicInstance
  any, // HostContext
  any, // UpdatePayload
  any, // ChildSet
  any, // TimeoutHandle
  any  // NoTimeout
>({
  supportsMutation: true,
  supportsPersistence: false,

  createInstance(type, props) {
    return {
      type,
      props,
      children: [],
      output: ''
    };
  },

  createTextInstance(text) {
    return text;
  },

  appendInitialChild(parentInstance, child) {
    if (typeof child === 'string') {
      parentInstance.output += child;
    } else {
      parentInstance.children.push(child);
    }
  },

  appendChild(parentInstance, child) {
    if (typeof child === 'string') {
      parentInstance.output += child;
    } else {
      parentInstance.children.push(child);
    }
  },

  removeChild(parentInstance, child) {
    if (typeof child !== 'string') {
      const index = parentInstance.children.indexOf(child);
      if (index !== -1) {
        parentInstance.children.splice(index, 1);
      }
    }
  },

  insertBefore(parentInstance, child, beforeChild) {
    if (typeof child !== 'string' && typeof beforeChild !== 'string') {
      const index = parentInstance.children.indexOf(beforeChild);
      if (index !== -1) {
        parentInstance.children.splice(index, 0, child);
      }
    }
  },

  finalizeInitialChildren() {
    return false;
  },

  prepareUpdate() {
    return {};
  },

  commitUpdate(instance, updatePayload, type, oldProps, newProps) {
    instance.props = newProps;
  },

  commitTextUpdate(textInstance, oldText, newText) {
    // Text updates handled in parent
  },

  getRootHostContext() {
    return {};
  },

  getChildHostContext() {
    return {};
  },

  shouldSetTextContent() {
    return false;
  },

  prepareForCommit() {
    return null;
  },

  resetAfterCommit(container) {
    // Renderuj do konsoli po każdym commit
    container.lines = renderToTerminal(container.children);
  },

  clearContainer(container) {
    container.children = [];
  },

  getPublicInstance(instance) {
    return instance;
  },

  now: () => Date.now(),
  scheduleTimeout: setTimeout,
  cancelTimeout: clearTimeout,
  noTimeout: -1,
  isPrimaryRenderer: false,
  supportsHydration: false,
});

// Helper do renderowania do tekstu
function renderToTerminal(instances: TerminalInstance[]): string[] {
  const lines: string[] = [];

  instances.forEach(instance => {
    const childOutput = instance.children.length > 0 
      ? renderToTerminal(instance.children).join('')
      : instance.output;

    switch (instance.type) {
      case 'box':
        lines.push(`╔${'═'.repeat(50)}╗`);
        lines.push(`║ ${childOutput}${' '.repeat(49 - childOutput.length)}║`);
        lines.push(`╚${'═'.repeat(50)}╝`);
        break;
      case 'text':
        lines.push(childOutput);
        break;
      case 'line':
        lines.push(childOutput);
        break;
      default:
        lines.push(childOutput);
    }
  });

  return lines;
}

// Komponenty CLI z pełnym wsparciem React
function Box({ children }: { children: React.ReactNode }) {
  return React.createElement('box', {}, children);
}

function Text({ children }: { children: React.ReactNode }) {
  return React.createElement('text', {}, children);
}

function Line({ children }: { children: React.ReactNode }) {
  return React.createElement('line', {}, children);
}

// Teraz hooki działają prawidłowo!
function Counter() {
  const [count, setCount] = React.useState(0);

  React.useEffect(() => {
    const timer = setTimeout(() => setCount(prev => prev + 1), 1000);
    return () => clearTimeout(timer);
  }, [count]);

  return React.createElement(Text, {}, `Count: ${count}`);
}

// Kontekst też działa
const ThemeContext = React.createContext({ color: 'blue' });

function ThemedText({ children }: { children: React.ReactNode }) {
  const theme = React.useContext(ThemeContext);
  return React.createElement(Text, {}, `[${theme.color}] ${children}`);
}

// Użycie z właściwym renderer'em
export function renderTerminalApp() {
  const container: Container = { lines: [], children: [] };
  const root = TerminalRenderer.createContainer(
    container,
    0, // tag
    null, // hydration callbacks
    false, // isStrictMode
    null, // concurrentUpdatesByDefaultOverride
    '', // identifierPrefix
    () => {}, // onRecoverableError
    null // transitionCallbacks
  );

  const App = () => (
    React.createElement(ThemeContext.Provider, { value: { color: 'green' } },
      React.createElement(Box, {},
        React.createElement(Line, {}, 'My CLI App'),
        React.createElement(ThemedText, {}, 'Themed text'),
        React.createElement(Counter, {})
      )
    )
  );

  TerminalRenderer.updateContainer(
    React.createElement(App, {}),
    root,
    null,
    () => console.log('Rendered')
  );

  return container.lines.join('\n');
}

// Eksport renderer'a dla użycia zewnętrznego
export const render = (element: React.ReactElement, callback?: () => void) => {
  const container: Container = { lines: [], children: [] };
  const root = TerminalRenderer.createContainer(
    container,
    0,
    null,
    false,
    null,
    '',
    () => {},
    null
  );

  TerminalRenderer.updateContainer(element, root, null, callback);
  return container.lines.join('\n');
};
