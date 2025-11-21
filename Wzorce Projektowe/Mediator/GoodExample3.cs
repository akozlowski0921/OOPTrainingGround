using System;

namespace DesignPatterns.Mediator.Good3
{
    // ✅ GOOD: Clean mediator

    public interface IMediator
    {
        void Notify(Component sender, string ev);
    }

    public abstract class Component
    {
        protected IMediator Mediator;
        
        public Component(IMediator mediator)
        {
            Mediator = mediator;
        }
    }

    public class Button1 : Component
    {
        public Button1(IMediator mediator) : base(mediator) { }
        
        public void Click()
        {
            Mediator.Notify(this, "click");
        }
    }

    public class Button2 : Component
    {
        public Button2(IMediator mediator) : base(mediator) { }
        
        public void Click()
        {
            Mediator.Notify(this, "click");
        }
    }

    public class FormMediator : IMediator
    {
        private Button1 _btn1;
        private Button2 _btn2;

        public void RegisterComponents(Button1 b1, Button2 b2)
        {
            _btn1 = b1;
            _btn2 = b2;
        }

        public void Notify(Component sender, string ev)
        {
            if (sender == _btn1)
                Console.WriteLine("Button1 clicked");
            else if (sender == _btn2)
                Console.WriteLine("Button2 clicked");
        }
    }
}
