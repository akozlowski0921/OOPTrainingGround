# Command Pattern

## Intent
Encapsulate a request as an object, allowing parameterization of clients with different requests, queue or log requests, and support undoable operations.

## Key Components
- **Command**: Interface z Execute() i Undo()
- **ConcreteCommand**: Implementuje command, przechowuje receiver i parametry
- **Invoker**: Wywołuje command
- **Receiver**: Wykonuje akcję

## Use Cases
✅ Undo/Redo functionality  
✅ Command queue/history  
✅ Macro commands (composite)  
✅ Transaction management  
✅ Background job processing

## Benefits
- Decouples invoker from receiver
- Commands jako first-class objects
- Easy to add new commands (OCP)
- Support for undo/redo
