using System.Drawing;
using System.Runtime.InteropServices;

namespace WinFlipped.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT(int Left, int Top, int Right, int Bottom)
    {
        private int _Left = Left;
        private int _Top = Top;
        private int _Right = Right;
        private int _Bottom = Bottom;

        public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom) {}

        public int X
        {
            readonly get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            readonly get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            readonly get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            readonly get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            readonly get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            readonly get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            readonly get { return _Bottom - _Top; }
            set { _Bottom = value + _Top; }
        }
        public int Width
        {
            readonly get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public Point Location
        {
            readonly get { return new Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            readonly get { return new Size(Width, Height); }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override readonly string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override readonly int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public readonly bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public override readonly bool Equals(object? Object)
        {
            if (Object is RECT rect)
            {
                return Equals(rect);
            }
            else if (Object is Rectangle rectangle)
            {
                return Equals(new RECT(rectangle));
            }

            return false;
        }
    }
}