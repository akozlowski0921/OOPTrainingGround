import React, { ReactNode } from 'react';

// ❌ BAD: Ostatnie typing anti-patterns

// BŁĄD 1: Index signature abuse
interface BadConfig {
  [key: string]: any; // ❌ Wszystko dozwolone
}

// BŁĄD 2: Brak readonly dla props
interface BadProps {
  items: string[]; // ❌ Można modyfikować
  config: { value: number }; // ❌ Można modyfikować
}

// BŁĄD 3: Union types bez discriminator
type BadResponse = {
  data: string;
  error: string; // ❌ Oba mogą istnieć jednocześnie
};

// BŁĄD 4: Props spreading bez type safety
function BadWrapper(props: any) {
  return <div {...props} />; // ❌ Brak kontroli nad props
}
