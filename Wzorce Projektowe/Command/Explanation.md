# Command Pattern

## 📌 Definicja
Command (Polecenie) to behawioralny wzorzec projektowy, który **enkapsuluje żądanie jako obiekt**, umożliwiając parametryzację klientów różnymi żądaniami, kolejkowanie, logowanie żądań oraz **obsługę operacji Undo/Redo**.

### Znany również jako:
- **Action**
- **Transaction**

## 🔴 Problem w BadExample

Bezpośrednie wywoływanie operacji na obiektach prowadzi do:

```csharp
public class TextEditor
{
    private string _text = "";
    
    public void HandleKeyPress(char key)
    {
        if (key == 'z' && IsCtrlPressed())
        {
            // ❌ Jak zrobić Undo? Brak historii operacji!
        }
        else
        {
            _text += key;
        }
    }
}
```

### Problemy:
❌ **Brak historii** – nie można cofnąć operacji  
❌ **Tight coupling** – UI bezpośrednio wywołuje operacje na modelu  
❌ **Brak kolejkowania** – operacje wykonywane natychmiast  
❌ **Niemożliwość logowania** – brak śladu co zostało wykonane  
❌ **Brak transakcji** – nie można grupować operacji  
❌ **Trudne testowanie** – nie da się przetestować operacji w izolacji  

## ✅ Rozwiązanie: Command Pattern

### Kluczowe komponenty:

```
┌─────────────┐      ┌──────────────┐
│   Client    │─────►│   Invoker    │
└─────────────┘      │  (Button)    │
                     └──────┬───────┘
                            │ Execute()
                            ↓
                     ┌──────────────┐
                     │  ICommand    │
                     │  + Execute() │
                     │  + Undo()    │
                     └──────┬───────┘
                            │
                ┌───────────┴────────────┐
                ↓                        ↓
         ┌─────────────┐         ┌─────────────┐
         │  AddText    │         │ DeleteText  │
         │  Command    │         │  Command    │
         └──────┬──────┘         └──────┬──────┘
                │                       │
                └───────┬───────────────┘
                        ↓
                 ┌──────────────┐
                 │   Receiver   │
                 │ (TextEditor) │
                 └──────────────┘
```

### Implementacja:

```csharp
// Command interface
public interface ICommand
{
    void Execute();
    void Undo();
}

// Receiver - wykonuje rzeczywiste operacje
public class TextEditor
{
    private StringBuilder _text = new();
    
    public void InsertText(string text, int position)
    {
        _text.Insert(position, text);
    }
    
    public void DeleteText(int position, int length)
    {
        _text.Remove(position, length);
    }
    
    public string GetText() => _text.ToString();
}

// Concrete Commands
public class InsertTextCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly string _text;
    private readonly int _position;
    
    public InsertTextCommand(TextEditor editor, string text, int position)
    {
        _editor = editor;
        _text = text;
        _position = position;
    }
    
    public void Execute()
    {
        _editor.InsertText(_text, _position);
    }
    
    public void Undo()
    {
        _editor.DeleteText(_position, _text.Length);
    }
}

public class DeleteTextCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly int _position;
    private readonly int _length;
    private string _deletedText;  // Zapamiętane dla Undo!
    
    public DeleteTextCommand(TextEditor editor, int position, int length)
    {
        _editor = editor;
        _position = position;
        _length = length;
    }
    
    public void Execute()
    {
        _deletedText = _editor.GetText().Substring(_position, _length);
        _editor.DeleteText(_position, _length);
    }
    
    public void Undo()
    {
        _editor.InsertText(_deletedText, _position);
    }
}

// Invoker - zarządza wykonywaniem komend
public class CommandManager
{
    private Stack<ICommand> _history = new();
    private Stack<ICommand> _redoStack = new();
    
    public void Execute(ICommand command)
    {
        command.Execute();
        _history.Push(command);
        _redoStack.Clear();  // Nowa komenda czyści redo
    }
    
    public void Undo()
    {
        if (_history.Count > 0)
        {
            var command = _history.Pop();
            command.Undo();
            _redoStack.Push(command);
        }
    }
    
    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _history.Push(command);
        }
    }
}
```

### Użycie:

```csharp
var editor = new TextEditor();
var manager = new CommandManager();

// Wykonaj komendy
manager.Execute(new InsertTextCommand(editor, "Hello", 0));
manager.Execute(new InsertTextCommand(editor, " World", 5));

Console.WriteLine(editor.GetText());  // "Hello World"

// Undo
manager.Undo();
Console.WriteLine(editor.GetText());  // "Hello"

// Redo
manager.Redo();
Console.WriteLine(editor.GetText());  // "Hello World"
```

## 🎯 Po co stosować Command?

### 1. **Undo/Redo functionality**
Komendy przechowują stan przed wykonaniem, umożliwiając cofnięcie.

### 2. **Decouple invoker from receiver**
Button nie musi wiedzieć co robi operacja, tylko że "wykonaj komendę".

### 3. **Command queuing**
Kolejkuj operacje do wykonania później (background jobs, batch processing).

### 4. **Logging i auditing**
Każda komenda może logować co i kiedy było wykonane.

### 5. **Transakcje**
Grupuj komendy w transakcje (wszystkie albo żadna).

## W czym pomaga?

✅ **GUI operations** – Undo/Redo w edytorach, CAD, graphics apps  
✅ **Macro commands** – łączenie wielu komend w jedną (Composite pattern)  
✅ **Schedulers** – planowanie operacji do wykonania w przyszłości  
✅ **Job queues** – background processing, task queues  
✅ **Transakcje** – rollback przy błędzie  
✅ **Command history** – audyt operacji użytkownika  
✅ **Parametryzacja** – te same przyciski z różnymi komendami  

## ⚖️ Zalety i wady

### Zalety
✅ **Single Responsibility** – command ma jedną odpowiedzialność  
✅ **Open/Closed** – nowe komendy bez modyfikacji invokera  
✅ **Undo/Redo** – łatwa implementacja cofania  
✅ **Deferred execution** – wykonaj później, kolejkuj  
✅ **Composite commands** – makra, transakcje  
✅ **Logging** – każda komenda może logować operację  

### Wady
❌ **Więcej klas** – każda operacja = nowa klasa  
❌ **Złożoność** – dodatkowa warstwa abstrakcji  
❌ **Memory overhead** – przechowywanie historii komend  
❌ **Overkill** – dla prostych przypadków może być przesadą  

## ⚠️ Na co uważać?

### 1. **Memory leaks przy dużej historii**
```csharp
// ❌ BAD: Nieskończona historia
public class CommandManager
{
    private Stack<ICommand> _history = new();  // Rośnie w nieskończoność!
    
    public void Execute(ICommand command)
    {
        command.Execute();
        _history.Push(command);  // Brak limitu!
    }
}

// ✅ GOOD: Limit historii
public class CommandManager
{
    private const int MaxHistorySize = 100;
    private Queue<ICommand> _history = new();
    
    public void Execute(ICommand command)
    {
        command.Execute();
        
        if (_history.Count >= MaxHistorySize)
        {
            _history.Dequeue();  // Usuń najstarszą
        }
        
        _history.Enqueue(command);
    }
}
```

### 2. **Command przechowuje zbyt dużo danych**
```csharp
// ❌ BAD: Kopiowanie całego dokumentu
public class FormatCommand : ICommand
{
    private byte[] _documentBackup;  // Cały dokument!
    
    public void Execute()
    {
        _documentBackup = _document.ToByteArray();  // Huge!
        _document.ApplyFormatting();
    }
}

// ✅ GOOD: Przechowuj tylko delta
public class FormatCommand : ICommand
{
    private Dictionary<int, FormatStyle> _previousStyles;  // Tylko zmiany
    
    public void Execute()
    {
        _previousStyles = _document.GetAffectedStyles(_range);
        _document.ApplyFormatting(_range, _newStyle);
    }
    
    public void Undo()
    {
        _document.RestoreStyles(_previousStyles);
    }
}
```

### 3. **Brak obsługi błędów w Undo**
```csharp
// ❌ BAD: Undo może rzucić exception
public void Undo()
{
    var command = _history.Pop();
    command.Undo();  // Co jeśli rzuci exception?
    // Historia już zmodyfikowana!
}

// ✅ GOOD: Rollback przy błędzie
public void Undo()
{
    if (_history.Count == 0) return;
    
    var command = _history.Peek();  // Peek, nie Pop!
    
    try
    {
        command.Undo();
        _history.Pop();  // Pop tylko po sukcesie
        _redoStack.Push(command);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Undo failed");
        // Historia nie zmieniona!
    }
}
```

### 4. **Command zależy od zewnętrznych zasobów**
```csharp
// ❌ BAD: Command trzyma connection
public class SaveCommand : ICommand
{
    private readonly DbConnection _connection;
    
    public SaveCommand(DbConnection connection)
    {
        _connection = connection;  // Co jeśli connection się zamknie?
    }
}

// ✅ GOOD: Command dostaje factory lub używa DI
public class SaveCommand : ICommand
{
    private readonly IDbConnectionFactory _factory;
    private readonly Data _data;
    
    public void Execute()
    {
        using var connection = _factory.CreateConnection();
        connection.Save(_data);
    }
}
```

### 5. **Thread-safety przy concurrent access**
```csharp
// ❌ BAD: Nie thread-safe
private Stack<ICommand> _history = new();

// ✅ GOOD: Thread-safe operations
private readonly ConcurrentStack<ICommand> _history = new();

// lub
private readonly Stack<ICommand> _history = new();
private readonly object _lock = new object();

public void Execute(ICommand command)
{
    command.Execute();
    
    lock (_lock)
    {
        _history.Push(command);
    }
}
```

## 🔄 Kiedy stosować Command?

### Użyj Command gdy:
✅ **Potrzebujesz Undo/Redo** – edytory, CAD, graphics apps  
✅ **Kolejkowanie operacji** – job queues, schedulers  
✅ **Parametryzacja GUI** – te same przyciski, różne akcje  
✅ **Logging i auditing** – śledzenie operacji użytkownika  
✅ **Transakcje** – all-or-nothing operations  
✅ **Macro commands** – łączenie wielu operacji  

### NIE używaj Command gdy:
❌ **Proste callbacki wystarczą** – `Action<T>`, `Func<T>`  
❌ **Brak potrzeby Undo** – bezpośrednie wywołanie prostsze  
❌ **Jednokrotne wykonanie** – command to overkill  

## 🚨 Najczęstsze pomyłki

### 1. **Command modyfikuje stan globalny bez backupu**
```csharp
// ❌ BAD: Brak zapisania poprzedniego stanu
public class SetColorCommand : ICommand
{
    private Color _newColor;
    
    public void Execute()
    {
        _shape.Color = _newColor;  // Jak Undo bez poprzedniego koloru?
    }
}

// ✅ GOOD: Zapamiętaj poprzedni stan
public class SetColorCommand : ICommand
{
    private Color _newColor;
    private Color _previousColor;
    
    public void Execute()
    {
        _previousColor = _shape.Color;  // Backup!
        _shape.Color = _newColor;
    }
    
    public void Undo()
    {
        _shape.Color = _previousColor;  // Restore!
    }
}
```

### 2. **Command nie jest idempotentny**
```csharp
// ❌ BAD: Wielokrotne Execute daje różne wyniki
public class IncrementCommand : ICommand
{
    public void Execute()
    {
        _counter++;  // Co jeśli Execute wywoła się 2 razy?
    }
}

// ✅ GOOD: Command jest idempotentny lub ma flagę
public class IncrementCommand : ICommand
{
    private bool _executed = false;
    
    public void Execute()
    {
        if (_executed) return;  // Guard
        
        _counter++;
        _executed = true;
    }
}
```

### 3. **Używanie Command dla wszystkiego**
```csharp
// ❌ BAD: Overkill
public class GetUserCommand : ICommand
{
    public void Execute()
    {
        return _repo.GetUser(_id);  // Po prostu wywołaj metodę!
    }
}

// ✅ GOOD: Command dla operacji z side-effects
public class UpdateUserCommand : ICommand
{
    public void Execute()
    {
        _user.Name = _newName;  // Ma sens - Undo/Redo możliwe
    }
}
```

### 4. **Macro command bez atomicity**
```csharp
// ❌ BAD: Partial execution przy błędzie
public class MacroCommand : ICommand
{
    private List<ICommand> _commands;
    
    public void Execute()
    {
        foreach (var cmd in _commands)
        {
            cmd.Execute();  // Co jeśli 3. komenda rzuci exception?
        }
    }
}

// ✅ GOOD: All-or-nothing
public class MacroCommand : ICommand
{
    private List<ICommand> _commands;
    private List<ICommand> _executed = new();
    
    public void Execute()
    {
        try
        {
            foreach (var cmd in _commands)
            {
                cmd.Execute();
                _executed.Add(cmd);
            }
        }
        catch
        {
            // Rollback executed commands
            foreach (var cmd in _executed.Reverse<ICommand>())
            {
                cmd.Undo();
            }
            throw;
        }
    }
}
```

## 💼 Kontekst biznesowy

### Przykład: System ERP - edycja zamówień

**Bez Command:**
```csharp
public class OrderEditForm
{
    private void SaveButton_Click()
    {
        _order.CustomerName = txtName.Text;
        _order.Amount = decimal.Parse(txtAmount.Text);
        _repository.Save(_order);
        
        // Jak cofnąć jeśli użytkownik zrobi błąd?
        // Jak zalogować co zostało zmienione?
        // Jak zrobić batch operations?
    }
}
```

**Z Command:**
```csharp
// Każda zmiana to command
manager.Execute(new ChangeCustomerNameCommand(_order, newName));
manager.Execute(new ChangeAmountCommand(_order, newAmount));

// Użytkownik może cofnąć błędne zmiany
if (userMistake)
{
    manager.Undo();  // Cofnij ostatnią operację
}

// Audyt: wszystkie komendy są logowane
_auditLog.Log($"User {user.Name} changed order {order.Id}");

// Batch operations
var macro = new MacroCommand(
    new ChangeCustomerNameCommand(_order, newName),
    new ChangeAmountCommand(_order, newAmount),
    new AddNoteCommand(_order, note)
);
manager.Execute(macro);  // Wszystko albo nic
```

**Korzyści:**
- **Undo/Redo** – użytkownicy mogą cofać błędy
- **Audyt** – każda zmiana jest zalogowana
- **Transakcje** – batch operations z rollback
- **Testowanie** – łatwe mockowanie komend

## 📝 Podsumowanie

- **Command** enkapsuluje żądanie jako obiekt, umożliwiając Undo/Redo, kolejkowanie, logging
- **Stosuj** dla operacji z Undo/Redo, job queues, transakcji, audytu
- **Uważaj** na memory leaks (historia), thread-safety, large data in commands
- **Najczęstsze błędy:** brak backupu stanu, non-idempotent commands, overkill dla prostych operacji
- **W C#** rozważ: CQRS pattern dla complex scenarios, MediatR library dla command handling
