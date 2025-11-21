using System;

namespace DesignPatterns.Command
{
    // ❌ BAD: Bezpośrednie wywołania metod bez Command pattern

    public class TextEditor
    {
        private string _content = "";

        public void InsertText(string text)
        {
            _content += text;
            // ❌ Brak możliwości undo
        }

        public void DeleteText(int length)
        {
            if (length <= _content.Length)
                _content = _content.Substring(0, _content.Length - length);
            // ❌ Brak możliwości undo
        }

        public string GetContent() => _content;
    }

    // BŁĄD: Bezpośrednie wywołania - brak historii, undo/redo
    public class BadTextEditorApp
    {
        private readonly TextEditor _editor = new();

        public void Run()
        {
            _editor.InsertText("Hello");
            _editor.InsertText(" World");
            _editor.DeleteText(6);
            
            // ❌ Nie możemy cofnąć operacji
            // ❌ Nie możemy powtórzyć operacji
            // ❌ Brak historii operacji
        }
    }
}
