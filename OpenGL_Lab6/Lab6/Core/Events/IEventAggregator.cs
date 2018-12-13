using System;
using System.Windows.Forms;

namespace Core.Events
{
    public interface IEventAggregator
    {
        void KeyDown(object sender, KeyEventArgs keyEventArgs);
        void MouseMove(object sender, MouseEventArgs mouseEventArgs);
        void MouseDown(object sender, MouseEventArgs mouseEventArgs);
        void MouseUp(object sender, MouseEventArgs mouseEventArgs);
        void MouseScroll(object sender, MouseEventArgs scrollEventArgs);
        void DragDrop(object sender, DragEventArgs e);

        event EventHandler<KeyEventArgs> OnKeyDown;
        event EventHandler<MouseEventArgs> OnMouseMove;
        event EventHandler<MouseEventArgs> OnMouseDown;
        event EventHandler<MouseEventArgs> OnMouseUp;
        event EventHandler<MouseEventArgs> OnMouseScroll;
        event EventHandler<DragEventArgs> OnDragDrop;
    }
}