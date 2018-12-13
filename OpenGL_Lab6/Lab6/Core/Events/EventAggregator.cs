using System;
using System.Windows.Forms;

namespace Core.Events
{
    public class EventAggregator : IEventAggregator
    {
        public void KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            OnKeyDown?.Invoke(sender, keyEventArgs);
        }

        public void MouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            OnMouseMove?.Invoke(sender, mouseEventArgs);
        }

        public void MouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            OnMouseDown?.Invoke(sender, mouseEventArgs);
        }

        public void MouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            OnMouseUp?.Invoke(sender, mouseEventArgs);
        }

        public void MouseScroll(object sender, MouseEventArgs scrollEventArgs)
        {
            OnMouseScroll?.Invoke(sender, scrollEventArgs);
        }

        public void DragDrop(object sender, DragEventArgs e)
        {
            OnDragDrop?.Invoke(sender, e);
        }

        public event EventHandler<KeyEventArgs> OnKeyDown;
        public event EventHandler<MouseEventArgs> OnMouseMove;
        public event EventHandler<MouseEventArgs> OnMouseDown;
        public event EventHandler<MouseEventArgs> OnMouseUp;
        public event EventHandler<MouseEventArgs> OnMouseScroll;
        public event EventHandler<DragEventArgs> OnDragDrop;
    }
}