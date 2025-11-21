import React, { ReactNode } from 'react';

// ✅ GOOD: Proper generic typing

// ✅ Generic z constraints
interface HasId {
  id: string | number;
}

interface GoodListProps<T extends HasId> {
  items: T[];
  renderItem: (item: T) => ReactNode;
}

function GoodGenericList<T extends HasId>({ items, renderItem }: GoodListProps<T>) {
  return (
    <ul>
      {items.map(item => (
        <li key={item.id}>{renderItem(item)}</li>
      ))}
    </ul>
  );
}

// ✅ Proper typing bez assertions
interface User {
  id: number;
  name: string;
}

interface UserListProps {
  users: User[];
}

function GoodUserList({ users }: UserListProps) {
  return (
    <ul>
      {users.map(user => (
        <li key={user.id}>{user.name}</li>
      ))}
    </ul>
  );
}

// ✅ Optional props z default values
interface GoodButtonProps {
  label?: string;
  onClick?: () => void;
}

function GoodButton({ label = 'Click me', onClick }: GoodButtonProps) {
  return <button onClick={onClick}>{label.toUpperCase()}</button>;
}

// ✅ Strong types
interface GoodModalProps {
  isOpen: boolean;
  content: ReactNode;
  onClose: () => void;
}

function GoodModal({ isOpen, content, onClose }: GoodModalProps) {
  if (!isOpen) return null;
  return (
    <div onClick={onClose}>
      {content}
    </div>
  );
}

// ✅ Utility types
type ButtonVariant = 'primary' | 'secondary' | 'danger';

interface BaseButtonProps {
  children: ReactNode;
  disabled?: boolean;
}

type PrimaryButtonProps = BaseButtonProps & {
  variant: 'primary';
  color: string; // Required dla primary
};

type SecondaryButtonProps = BaseButtonProps & {
  variant: 'secondary';
  color?: never; // Nie dozwolone dla secondary
};

type StyledButtonProps = PrimaryButtonProps | SecondaryButtonProps;

function StyledButton(props: StyledButtonProps) {
  if (props.variant === 'primary') {
    return <button style={{ color: props.color }}>{props.children}</button>;
  }
  return <button>{props.children}</button>;
}
