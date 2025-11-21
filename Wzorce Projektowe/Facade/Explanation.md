# Facade Pattern

## Intent
Provide a unified interface to a set of interfaces in a subsystem. Facade defines a higher-level interface that makes the subsystem easier to use.

## Problem
- Complex subsystems with many classes
- Client musi znać szczegóły implementacji
- Tight coupling między client i subsystems
- Duplikacja orkiestracji logiki

## Solution
- Facade enkapsuluje subsystems
- Prosty, high-level interface dla client
- Reduces dependencies
- Simplifies complex operations

## Structure
```
Client → Facade → Subsystem1
              → Subsystem2
              → Subsystem3
```

## Use Cases
✅ Simplifying complex libraries/APIs  
✅ Database + Cache + Logger coordination  
✅ Payment processing (gateway + email + SMS)  
✅ External service integration  
✅ Legacy system wrapper

## Benefits
- Simplified interface
- Loose coupling
- Easier to use subsystem
- Can provide caching, error handling
