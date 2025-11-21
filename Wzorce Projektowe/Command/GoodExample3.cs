using System;
using System.Collections.Generic;

namespace DesignPatterns.Command.Good3
{
    // ✅ GOOD: Macro command

    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class MacroCommand : ICommand
    {
        private readonly List<ICommand> _commands = new();

        public void Add(ICommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }

    public class CutCommand : ICommand
    {
        public void Execute() => Console.WriteLine("Cut");
        public void Undo() => Console.WriteLine("Undo cut");
    }

    public class PasteCommand : ICommand
    {
        public void Execute() => Console.WriteLine("Paste");
        public void Undo() => Console.WriteLine("Undo paste");
    }
}
