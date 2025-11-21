# Mediator Pattern

## Intent
Define an object that encapsulates how a set of objects interact. Promotes loose coupling by preventing objects from referring to each other explicitly.

## Problem
- Components znają się nawzajem (tight coupling)
- Trudne dodawanie nowych komponentów
- Skomplikowana sieć zależności

## Solution
- Mediator enkapsuluje interakcje
- Komponenty komunikują się tylko przez Mediator
- Loose coupling między komponentami

## Real-world Examples
- Chat room (users communicate through chat room)
- Air traffic control (planes communicate through controller)
- **MediatR library** in ASP.NET Core (CQRS pattern)

## Use Cases
✅ Complex communication between many objects  
✅ Reusable components that shouldn't know about each other  
✅ CQRS pattern (Commands/Queries)  
✅ Event bus/Message bus
