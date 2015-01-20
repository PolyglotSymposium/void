using System;
using Eto.Forms;
using Eto.Drawing;
using Void.Core;
using Void.ViewModel;

namespace Void
{
    // This class exists to protect the View and the View Model from each other,
    // so that each can be built the way it wants to be built
    public static class TypeTranslationExtensions
    {
        public static Color AsEtoColor(this RGBColor color)
        {
            return Color.FromArgb(color.Red, color.Green, color.Blue);
        }

        public static Size AsEtoSize(this SizeInPixels size)
        {
            return new Size(size.Width, size.Height);
        }

        public static PointF AsEtoPointF(this View.Point point)
        {
            return new PointF(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }

        public static RectangleF AsEtoRectangleF(this View.Block block)
        {
            return new RectangleF(block.UpperLeftCorner.AsEtoPointF(), new SizeF(Convert.ToSingle(block.Width), Convert.ToSingle(block.Height)));
        }

        public static KeyPress AsVoidKeyPress(this KeyEventArgs keyEvent)
        {
            KeyPress keyPress = null;
            if (keyEvent.Shift)
            {
                if (keyEvent.Key == Keys.Q)
                {
                    keyPress = KeyPress.ShiftQ;
                }
                else if (keyEvent.Key == Keys.L)
                {
                    keyPress = KeyPress.ShiftL;
                }
                else if (keyEvent.Key == Keys.Z)
                {
                    keyPress = KeyPress.ShiftZ;
                }
            } 
            if (keyEvent.Control)
            {
                if (keyEvent.Key == Keys.Q)
                {
                    keyPress = KeyPress.ShiftQ;
                }  
                else if (keyEvent.Key == Keys.L)
                {
                    keyPress = KeyPress.ShiftL;
                }
                else if (keyEvent.Key == Keys.Z)
                {
                    keyPress = KeyPress.ShiftZ;
                }
            } 
            if (keyEvent.Key == Keys.Escape)
            {
                keyPress = KeyPress.Escape;
            }
            return keyPress;
        }
    }
}
