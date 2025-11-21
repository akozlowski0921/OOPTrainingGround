import React, { ReactNode, ComponentPropsWithoutRef } from 'react';

// ✅ GOOD: Advanced typing patterns

// ✅ Strongly typed config
interface GoodConfig {
  timeout: number;
  retries: number;
  endpoint: string;
}

// ✅ Readonly props
interface GoodProps {
  readonly items: readonly string[];
  readonly config: Readonly<{ value: number }>;
}

// ✅ Discriminated union
type GoodResponse<T> =
  | { status: 'success'; data: T }
  | { status: 'error'; error: string };

function handleResponse<T>(response: GoodResponse<T>) {
  if (response.status === 'success') {
    return response.data; // ✅ TypeScript wie że data istnieje
  }
  return null;
}

// ✅ Typed props spreading
type DivProps = ComponentPropsWithoutRef<'div'>;

interface GoodWrapperProps extends DivProps {
  variant: 'default' | 'bordered';
}

function GoodWrapper({ variant, ...props }: GoodWrapperProps) {
  return <div className={variant} {...props} />;
}

// ✅ Recursive types
type NestedComment = {
  id: number;
  text: string;
  replies?: NestedComment[];
};

interface CommentProps {
  comment: NestedComment;
}

function Comment({ comment }: CommentProps): JSX.Element {
  return (
    <div>
      <p>{comment.text}</p>
      {comment.replies?.map(reply => (
        <Comment key={reply.id} comment={reply} />
      ))}
    </div>
  );
}

// ✅ Mapped types
type FormValues = {
  email: string;
  password: string;
  age: number;
};

type FormErrors<T> = {
  [K in keyof T]?: string;
};

const errors: FormErrors<FormValues> = {
  email: 'Invalid email',
  // password i age opcjonalne
};

// ✅ Template literal types
type EventName = 'click' | 'focus' | 'blur';
type EventHandler = `on${Capitalize<EventName>}`;

type EventProps = {
  [K in EventHandler]?: () => void;
};

const props: EventProps = {
  onClick: () => {},
  onFocus: () => {}
};
