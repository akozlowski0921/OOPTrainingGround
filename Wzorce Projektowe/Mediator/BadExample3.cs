using System;

namespace DesignPatterns.Mediator.Bad3
{
    // ❌ BAD: Spaghetti dependencies

    public class Form
    {
        private Button1 _btn1;
        private Button2 _btn2;
        private TextBox _textBox;

        public void Initialize()
        {
            _btn1 = new Button1(_btn2, _textBox);
            _btn2 = new Button2(_btn1, _textBox);
            _textBox = new TextBox(_btn1, _btn2);
            // ❌ Circular dependencies
        }
    }

    public class Button1
    {
        private Button2 _btn2;
        private TextBox _tb;
        public Button1(Button2 b, TextBox t) { _btn2 = b; _tb = t; }
    }

    public class Button2
    {
        private Button1 _btn1;
        private TextBox _tb;
        public Button2(Button1 b, TextBox t) { _btn1 = b; _tb = t; }
    }

    public class TextBox
    {
        private Button1 _btn1;
        private Button2 _btn2;
        public TextBox(Button1 b1, Button2 b2) { _btn1 = b1; _btn2 = b2; }
    }
}
