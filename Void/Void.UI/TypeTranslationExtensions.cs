using System;
using System.Drawing;
using System.Windows.Forms;
using Void.Core;
using Void.ViewModel;

namespace Void.UI
{
    // This class exists to protect the View and the View Model from each other,
    // so that each can be built the way it wants to be built
    public static class TypeTranslationExtensions
    {
        public static Color AsWinFormsColor(this RGBColor color)
        {
            return Color.FromArgb(color.Red, color.Green, color.Blue);
        }

        public static Size AsWinFormsSize(this PixelGrid.Dimensions size)
        {
            return new Size(size.Width, size.Height);
        }

        public static PointF AsWinFormsPointF(this PixelGrid.Point point)
        {
            return new PointF(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }

        public static SizeF AsWinFormsSizeF(this PixelGrid.Dimensions size)
        {
            return new SizeF(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static RectangleF AsWinFormsRectangleF(this PixelGrid.Block block)
        {
            return new RectangleF(block.UpperLeftCorner.AsWinFormsPointF(), block.Dimensions.AsWinFormsSizeF());
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
                switch (keyEvent.KeyCode)
                {
                    case Keys.A:
                        keyPress = KeyPress.ShiftA;
                        break;
                    case Keys.B:
                        keyPress = KeyPress.ShiftB;
                        break;
                    case Keys.C:
                        keyPress = KeyPress.ShiftC;
                        break;
                    case Keys.D:
                        keyPress = KeyPress.ShiftD;
                        break;
                    case Keys.E:
                        keyPress = KeyPress.ShiftE;
                        break;
                    case Keys.F:
                        keyPress = KeyPress.ShiftF;
                        break;
                    case Keys.G:
                        keyPress = KeyPress.ShiftG;
                        break;
                    case Keys.H:
                        keyPress = KeyPress.ShiftH;
                        break;
                    case Keys.I:
                        keyPress = KeyPress.ShiftI;
                        break;
                    case Keys.J:
                        keyPress = KeyPress.ShiftJ;
                        break;
                    case Keys.K:
                        keyPress = KeyPress.ShiftK;
                        break;
                    case Keys.L:
                        keyPress = KeyPress.ShiftL;
                        break;
                    case Keys.M:
                        keyPress = KeyPress.ShiftM;
                        break;
                    case Keys.N:
                        keyPress = KeyPress.ShiftN;
                        break;
                    case Keys.O:
                        keyPress = KeyPress.ShiftO;
                        break;
                    case Keys.P:
                        keyPress = KeyPress.ShiftP;
                        break;
                    case Keys.Q:
                        keyPress = KeyPress.ShiftQ;
                        break;
                    case Keys.R:
                        keyPress = KeyPress.ShiftR;
                        break;
                    case Keys.S:
                        keyPress = KeyPress.ShiftS;
                        break;
                    case Keys.T:
                        keyPress = KeyPress.ShiftT;
                        break;
                    case Keys.U:
                        keyPress = KeyPress.ShiftU;
                        break;
                    case Keys.V:
                        keyPress = KeyPress.ShiftV;
                        break;
                    case Keys.W:
                        keyPress = KeyPress.ShiftW;
                        break;
                    case Keys.X:
                        keyPress = KeyPress.ShiftX;
                        break;
                    case Keys.Y:
                        keyPress = KeyPress.ShiftY;
                        break;
                    case Keys.Z:
                        keyPress = KeyPress.ShiftZ;
                        break;
                    case Keys.OemSemicolon /* TODO is this right? */:
                        keyPress = KeyPress.Colon;
                        break;
                }
            } 
            else if (keyEvent.Control)
            {
                switch (keyEvent.KeyCode)
                {
                    case Keys.A:
                        keyPress = KeyPress.ControlA;
                        break;
                    case Keys.B:
                        keyPress = KeyPress.ControlB;
                        break;
                    case Keys.C:
                        keyPress = KeyPress.ControlC;
                        break;
                    case Keys.D:
                        keyPress = KeyPress.ControlD;
                        break;
                    case Keys.E:
                        keyPress = KeyPress.ControlE;
                        break;
                    case Keys.F:
                        keyPress = KeyPress.ControlF;
                        break;
                    case Keys.G:
                        keyPress = KeyPress.ControlG;
                        break;
                    case Keys.H:
                        keyPress = KeyPress.ControlH;
                        break;
                    case Keys.I:
                        keyPress = KeyPress.ControlI;
                        break;
                    case Keys.J:
                        keyPress = KeyPress.ControlJ;
                        break;
                    case Keys.K:
                        keyPress = KeyPress.ControlK;
                        break;
                    case Keys.L:
                        keyPress = KeyPress.ControlL;
                        break;
                    case Keys.M:
                        keyPress = KeyPress.ControlM;
                        break;
                    case Keys.N:
                        keyPress = KeyPress.ControlN;
                        break;
                    case Keys.O:
                        keyPress = KeyPress.ControlO;
                        break;
                    case Keys.P:
                        keyPress = KeyPress.ControlP;
                        break;
                    case Keys.Q:
                        keyPress = KeyPress.ControlQ;
                        break;
                    case Keys.R:
                        keyPress = KeyPress.ControlR;
                        break;
                    case Keys.S:
                        keyPress = KeyPress.ControlS;
                        break;
                    case Keys.T:
                        keyPress = KeyPress.ControlT;
                        break;
                    case Keys.U:
                        keyPress = KeyPress.ControlU;
                        break;
                    case Keys.V:
                        keyPress = KeyPress.ControlV;
                        break;
                    case Keys.W:
                        keyPress = KeyPress.ControlW;
                        break;
                    case Keys.X:
                        keyPress = KeyPress.ControlX;
                        break;
                    case Keys.Y:
                        keyPress = KeyPress.ControlY;
                        break;
                    case Keys.Z:
                        keyPress = KeyPress.ControlZ;
                        break;
                    case Keys.OemSemicolon /* TODO is this right? */:
                        keyPress = KeyPress.ControlSemicolon;
                        break;
                }
            } 
            else if (keyEvent.KeyCode == Keys.Escape)
            {
                keyPress = KeyPress.Escape;
            }
            else
            {
                switch (keyEvent.KeyCode)
                {
                    case Keys.A:
                        keyPress = KeyPress.A;
                        break;
                    case Keys.B:
                        keyPress = KeyPress.B;
                        break;
                    case Keys.C:
                        keyPress = KeyPress.C;
                        break;
                    case Keys.D:
                        keyPress = KeyPress.D;
                        break;
                    case Keys.E:
                        keyPress = KeyPress.E;
                        break;
                    case Keys.F:
                        keyPress = KeyPress.F;
                        break;
                    case Keys.G:
                        keyPress = KeyPress.G;
                        break;
                    case Keys.H:
                        keyPress = KeyPress.H;
                        break;
                    case Keys.I:
                        keyPress = KeyPress.I;
                        break;
                    case Keys.J:
                        keyPress = KeyPress.J;
                        break;
                    case Keys.K:
                        keyPress = KeyPress.K;
                        break;
                    case Keys.L:
                        keyPress = KeyPress.L;
                        break;
                    case Keys.M:
                        keyPress = KeyPress.M;
                        break;
                    case Keys.N:
                        keyPress = KeyPress.N;
                        break;
                    case Keys.O:
                        keyPress = KeyPress.O;
                        break;
                    case Keys.P:
                        keyPress = KeyPress.P;
                        break;
                    case Keys.Q:
                        keyPress = KeyPress.Q;
                        break;
                    case Keys.R:
                        keyPress = KeyPress.R;
                        break;
                    case Keys.S:
                        keyPress = KeyPress.S;
                        break;
                    case Keys.T:
                        keyPress = KeyPress.T;
                        break;
                    case Keys.U:
                        keyPress = KeyPress.U;
                        break;
                    case Keys.V:
                        keyPress = KeyPress.V;
                        break;
                    case Keys.W:
                        keyPress = KeyPress.W;
                        break;
                    case Keys.X:
                        keyPress = KeyPress.X;
                        break;
                    case Keys.Y:
                        keyPress = KeyPress.Y;
                        break;
                    case Keys.Z:
                        keyPress = KeyPress.Z;
                        break;
                    case Keys.OemSemicolon /* TODO is this right? */:
                        keyPress = KeyPress.Semicolon;
                        break;
                    default:
                        #if DEBUG
                        Console.WriteLine("Warning: failed to translate key stroke");
                        #endif
                        break;
                }
            }
            return keyPress;
        }
    }
}
