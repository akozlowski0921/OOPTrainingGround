using System;
using System.Collections.Generic;

namespace DesignPatterns.Command.Good2
{
    // ✅ GOOD: Transaction pattern z commands

    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class Transaction
    {
        private readonly List<ICommand> _commands = new();

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public void Commit()
        {
            foreach (var cmd in _commands)
            {
                cmd.Execute();
            }
            _commands.Clear();
        }

        public void Rollback()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
            _commands.Clear();
        }
    }
}
