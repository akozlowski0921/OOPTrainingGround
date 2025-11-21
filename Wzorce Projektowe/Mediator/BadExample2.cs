using System;

namespace DesignPatterns.Mediator.Bad2
{
    // ❌ BAD: Direct coupling

    public class Button
    {
        private TextBox _textBox;
        private Label _label;

        public void SetDependencies(TextBox tb, Label l)
        {
            _textBox = tb;
            _label = l;
        }

        public void Click()
        {
            // ❌ Zna o innych komponentach
            _textBox.Clear();
            _label.SetText("Clicked");
        }
    }

    public class TextBox
    {
        public void Clear() { }
    }

    public class Label
    {
        public void SetText(string text) { }
    }
}
