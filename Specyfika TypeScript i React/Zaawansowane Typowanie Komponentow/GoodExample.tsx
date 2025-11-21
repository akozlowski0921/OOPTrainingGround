import React, { ReactNode, ReactElement, FormEvent } from 'react';

// ✅ GOOD: Zaawansowane typowanie komponentów

// ✅ 1. Bez React.FC - explicit props typing
interface ButtonProps {
  label: string;
  onClick?: () => void;
  disabled?: boolean;
}

export function GoodButton({ label, onClick, disabled }: ButtonProps) {
  return <button onClick={onClick} disabled={disabled}>{label}</button>;
}

// ✅ 2. Strict props typing
interface CardProps {
  title: string;
  content: string;
  footer?: ReactNode;
}

export function GoodCard({ title, content, footer }: CardProps) {
  return (
    <div>
      <h2>{title}</h2>
      <p>{content}</p>
      {footer && <footer>{footer}</footer>}
    </div>
  );
}

// ✅ 3. Proper children typing
interface ContainerProps {
  children: ReactNode; // ReactNode = null | string | number | ReactElement | ReactNode[]
}

export function GoodContainer({ children }: ContainerProps) {
  return <div className="container">{children}</div>;
}

// Only React elements
interface StrictContainerProps {
  children: ReactElement | ReactElement[];
}

export function StrictContainer({ children }: StrictContainerProps) {
  return <div>{children}</div>;
}

// ✅ 4. Discriminated unions dla state
type LoadingState = { status: 'loading' };
type SuccessState<T> = { status: 'success'; data: T };
type ErrorState = { status: 'error'; error: string };

type AsyncState<T> = LoadingState | SuccessState<T> | ErrorState;

interface User {
  id: number;
  name: string;
}

function useUserData() {
  const [state, setState] = React.useState<AsyncState<User>>({ status: 'loading' });

  // ✅ Type narrowing
  if (state.status === 'loading') {
    return <div>Loading...</div>;
  }

  if (state.status === 'error') {
    return <div>Error: {state.error}</div>; // ✅ error dostępny
  }

  // ✅ TypeScript wie że status === 'success'
  return <div>User: {state.data.name}</div>; // ✅ data dostępny
}

// ✅ 5. Generic components
interface ListProps<T> {
  items: T[];
  renderItem: (item: T) => ReactNode;
  keyExtractor: (item: T) => string | number;
}

export function GoodList<T>({ items, renderItem, keyExtractor }: ListProps<T>) {
  return (
    <ul>
      {items.map(item => (
        <li key={keyExtractor(item)}>
          {renderItem(item)}
        </li>
      ))}
    </ul>
  );
}

// Usage
const users: User[] = [{ id: 1, name: 'John' }];
<GoodList
  items={users}
  renderItem={user => <span>{user.name}</span>}
  keyExtractor={user => user.id}
/>;

// ✅ 6. Event handlers properly typed
export function GoodForm() {
  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // ✅ Full type safety
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    console.log(e.target.value); // ✅ typed
  };

  return (
    <form onSubmit={handleSubmit}>
      <input onChange={handleChange} />
    </form>
  );
}

// ✅ 7. Component with render props
interface TabsProps {
  children: (activeTab: string) => ReactNode;
  defaultTab: string;
}

export function Tabs({ children, defaultTab }: TabsProps) {
  const [active, setActive] = React.useState(defaultTab);
  return <div>{children(active)}</div>;
}

// ✅ 8. Props with union types
interface IconProps {
  name: 'home' | 'user' | 'settings'; // ✅ Only valid icons
  size?: 'sm' | 'md' | 'lg';
}

export function Icon({ name, size = 'md' }: IconProps) {
  return <span>{name} {size}</span>;
}

// ✅ 9. Omit/Pick utility types
interface FullUserProps {
  id: number;
  name: string;
  email: string;
  password: string;
}

// ✅ Omit sensitive fields
type PublicUserProps = Omit<FullUserProps, 'password'>;

// ✅ Pick only needed fields
type UserNameProps = Pick<FullUserProps, 'name'>;

// ✅ 10. Conditional props
type ButtonBaseProps = {
  children: ReactNode;
};

type LinkButtonProps = ButtonBaseProps & {
  href: string;
  onClick?: never; // ✅ href XOR onClick
};

type ActionButtonProps = ButtonBaseProps & {
  onClick: () => void;
  href?: never;
};

type ConditionalButtonProps = LinkButtonProps | ActionButtonProps;

export function ConditionalButton(props: ConditionalButtonProps) {
  if ('href' in props) {
    return <a href={props.href}>{props.children}</a>;
  }
  return <button onClick={props.onClick}>{props.children}</button>;
}

// ✅ 11. Component as prop
interface CardWithHeaderProps {
  header: React.ComponentType<{ title: string }>;
  title: string;
  content: string;
}

export function CardWithHeader({ header: Header, title, content }: CardWithHeaderProps) {
  return (
    <div>
      <Header title={title} />
      <p>{content}</p>
    </div>
  );
}

// ✅ 12. forwardRef properly typed
interface InputProps {
  placeholder?: string;
}

export const ForwardedInput = React.forwardRef<HTMLInputElement, InputProps>(
  ({ placeholder }, ref) => {
    return <input ref={ref} placeholder={placeholder} />;
  }
);

ForwardedInput.displayName = 'ForwardedInput';
