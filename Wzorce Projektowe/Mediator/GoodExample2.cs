using System;

namespace DesignPatterns.Mediator.Good2
{
    // ✅ GOOD: UI Mediator

    public interface IMediator
    {
        void Notify(object sender, string eventName);
    }

    public class DialogMediator : IMediator
    {
        private Button _button;
        private TextBox _textBox;
        private Label _label;

        public void RegisterComponents(Button b, TextBox tb, Label l)
        {
            _button = b;
            _textBox = tb;
            _label = l;
        }

        public void Notify(object sender, string eventName)
        {
            if (sender == _button && eventName == "click")
            {
                _textBox.Clear();
                _label.SetText("Button clicked");
            }
        }
    }

    public class Button
    {
        private IMediator _mediator;

        public Button(IMediator mediator) => _mediator = mediator;

        public void Click()
        {
            _mediator.Notify(this, "click");
        }
    }

    public class TextBox
    {
        public void Clear() => Console.WriteLine("Cleared");
    }

    public class Label
    {
        public void SetText(string text) => Console.WriteLine($"Label: {text}");
    }
}
