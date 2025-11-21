using System;
using System.Collections.Generic;

namespace DesignPatterns.Command
{
    // ✅ GOOD: Command Pattern z undo/redo

    // ✅ Command interface
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // ✅ Receiver
    public class TextEditor
    {
        private string _content = "";

        public void InsertText(string text) => _content += text;
        public void DeleteText(int length)
        {
            if (length <= _content.Length)
                _content = _content.Substring(0, _content.Length - length);
        }
        public string GetContent() => _content;
    }

    // ✅ Concrete Commands
    public class InsertTextCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly string _text;

        public InsertTextCommand(TextEditor editor, string text)
        {
            _editor = editor;
            _text = text;
        }

        public void Execute() => _editor.InsertText(_text);
        public void Undo() => _editor.DeleteText(_text.Length);
    }

    public class DeleteTextCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly int _length;
        private string _deletedText = "";

        public DeleteTextCommand(TextEditor editor, int length)
        {
            _editor = editor;
            _length = length;
        }

        public void Execute()
        {
            var content = _editor.GetContent();
            _deletedText = content.Substring(content.Length - _length);
            _editor.DeleteText(_length);
        }

        public void Undo() => _editor.InsertText(_deletedText);
    }

    // ✅ Invoker - zarządza historią i undo/redo
    public class CommandManager
    {
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Nowe polecenie czyści redo stack
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
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
                _undoStack.Push(command);
            }
        }
    }

    // ✅ Usage
    public class GoodTextEditorApp
    {
        public static void Run()
        {
            var editor = new TextEditor();
            var manager = new CommandManager();

            // Execute commands
            manager.ExecuteCommand(new InsertTextCommand(editor, "Hello"));
            manager.ExecuteCommand(new InsertTextCommand(editor, " World"));
            Console.WriteLine(editor.GetContent()); // "Hello World"

            // Undo
            manager.Undo();
            Console.WriteLine(editor.GetContent()); // "Hello"

            // Redo
            manager.Redo();
            Console.WriteLine(editor.GetContent()); // "Hello World"

            manager.ExecuteCommand(new DeleteTextCommand(editor, 6));
            Console.WriteLine(editor.GetContent()); // "Hello"

            manager.Undo();
            Console.WriteLine(editor.GetContent()); // "Hello World"
        }
    }

    // ✅ Macro Command - composite
    public class MacroCommand : ICommand
    {
        private readonly List<ICommand> _commands = new();

        public void Add(ICommand command) => _commands.Add(command);

        public void Execute()
        {
            foreach (var command in _commands)
                command.Execute();
        }

        public void Undo()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
                _commands[i].Undo();
        }
    }

    // ✅ Command Queue dla background processing
    public class CommandQueue
    {
        private readonly Queue<ICommand> _queue = new();

        public void Enqueue(ICommand command) => _queue.Enqueue(command);

        public void ProcessAll()
        {
            while (_queue.Count > 0)
            {
                var command = _queue.Dequeue();
                command.Execute();
            }
        }
    }
}
