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

        public static Size AsEtoSize(this PixelGrid.Dimensions size)
        {
            return new Size(size.Width, size.Height);
        }

        public static PointF AsEtoPointF(this PixelGrid.Point point)
        {
            return new PointF(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }

        public static SizeF AsEtoSizeF(this PixelGrid.Dimensions size)
        {
            return new SizeF(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static RectangleF AsEtoRectangleF(this PixelGrid.Block block)
        {
            return new RectangleF(block.UpperLeftCorner.AsEtoPointF(), block.Dimensions.AsEtoSizeF());
        }

        public static ScreenLineObject AsLine(this DrawingObject drawing)
        {
            return ((DrawingObject.Line) drawing).Item;
        }

        public static ScreenTextObject AsText(this DrawingObject drawing)
        {
            return ((DrawingObject.Text) drawing).Item;
        }

        public static ScreenBlockObject AsBlock(this DrawingObject drawing)
        {
            return ((DrawingObject.Block) drawing).Item;
        }

        public static KeyPress AsVoidKeyPress(this KeyEventArgs keyEvent)
        {
            KeyPress keyPress = null;
            if (keyEvent.Shift)
            {
                if (keyEvent.Key == Keys.G)
                {
                    keyPress = KeyPress.ShiftG;
                }
                else if (keyEvent.Key == Keys.L)
                {
                    keyPress = KeyPress.ShiftL;
                }
                if (keyEvent.Key == Keys.Q)
                {
                    keyPress = KeyPress.ShiftQ;
                }
                else if (keyEvent.Key == Keys.Z)
                {
                    keyPress = KeyPress.ShiftZ;
                }
            } 
            else if (keyEvent.Control)
            {
                if (keyEvent.Key == Keys.G)
                {
                    keyPress = KeyPress.ControlG;
                }
                else if (keyEvent.Key == Keys.L)
                {
                    keyPress = KeyPress.ControlL;
                }
                if (keyEvent.Key == Keys.Q)
                {
                    keyPress = KeyPress.ControlQ;
                }  
                else if (keyEvent.Key == Keys.Z)
                {
                    keyPress = KeyPress.ControlZ;
                }
            } 
            else if (keyEvent.Key == Keys.Escape)
            {
                keyPress = KeyPress.Escape;
            }
            else
            {
                if (keyEvent.Key == Keys.G)
                {
                    keyPress = KeyPress.G;
                }
                else if (keyEvent.Key == Keys.L)
                {
                    keyPress = KeyPress.L;
                }
                if (keyEvent.Key == Keys.Q)
                {
                    keyPress = KeyPress.Q;
                }  
                else if (keyEvent.Key == Keys.Z)
                {
                    keyPress = KeyPress.Z;
                }
            }
            return keyPress;
        }
    }
}
