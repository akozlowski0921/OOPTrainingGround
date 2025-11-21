using System;

namespace DesignPatterns.Command.Bad2
{
    // ❌ BAD: Brak undo capability

    public class Document
    {
        private string _content = "";

        public void AddText(string text)
        {
            _content += text;
            // ❌ Nie możemy cofnąć
        }

        public void RemoveLastWord()
        {
            var lastSpace = _content.LastIndexOf(' ');
            if (lastSpace > 0)
                _content = _content.Substring(0, lastSpace);
            // ❌ Nie możemy cofnąć
        }
    }
}
