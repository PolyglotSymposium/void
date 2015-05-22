using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FSharp.Core;
using Void.Core;
using Void.ViewModel;

namespace Void.UI
{
    // This class exists to protect the View and the View Model from each other,
    // so that each can be built the way it wants to be built
    public static class TypeTranslationExtensions
    {
        public static FSharpFunc<DrawingObject, Unit> DrawAsFSharpFunc(this WinFormsArtist artist)
        {
            Action<DrawingObject> draw = artist.Draw;
            return FuncConvert.ToFSharpFunc(draw);
        }

        public static Color AsWinFormsColor(this RGBColor color)
        {
            return Color.FromArgb(color.Red, color.Green, color.Blue);
        }

        public static SolidBrush AsWinFormsSolidBrush(this RGBColor color)
        {
            return new SolidBrush(color.AsWinFormsColor());
        }

        public static Size AsWinFormsSize(this PointGrid.Dimensions size, CellMetrics cellMetrics)
        {
            return new Size(size.Width * cellMetrics.Width, size.Height * cellMetrics.Height);
        }

        public static Point AsWinFormsPoint(this PointGrid.Point point, CellMetrics cellMetrics)
        {
            return new Point(point.X * cellMetrics.Width, point.Y * cellMetrics.Height);
        }

        public static PointF AsWinFormsPointF(this PointGrid.Point point, CellMetrics cellMetrics)
        {
            return new PointF(Convert.ToSingle(point.X * cellMetrics.Width), Convert.ToSingle(point.Y * cellMetrics.Height));
        }

        public static SizeF AsWinFormsSizeF(this PointGrid.Dimensions size, CellMetrics cellMetrics)
        {
            return new SizeF(Convert.ToSingle(size.Width * cellMetrics.Width), Convert.ToSingle(size.Height * cellMetrics.Height));
        }

        public static Rectangle AsWinFormsRectangle(this PointGrid.Block block, CellMetrics cellMetrics)
        {
            return new Rectangle(block.UpperLeftCorner.AsWinFormsPoint(cellMetrics), block.Dimensions.AsWinFormsSize(cellMetrics));
        }

        public static RectangleF AsWinFormsRectangleF(this PointGrid.Block block, CellMetrics cellMetrics)
        {
            return new RectangleF(block.UpperLeftCorner.AsWinFormsPointF(cellMetrics), block.Dimensions.AsWinFormsSizeF(cellMetrics));
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

        public static Func<KeyPress,Unit> AsKeyPressesHandler(this InputMode<Unit> inputMode)
        {
            return ((InputMode<Unit>.KeyPresses) inputMode).Item.Invoke;
        }

        public static Func<TextOrHotKey,Unit> AsTextAndHotKeysHandler(this InputMode<Unit> inputMode)
        {
            return ((InputMode<Unit>.TextAndHotKeys) inputMode).Item.Invoke;
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
                    case Keys.OemSemicolon:
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
                    case Keys.OemSemicolon:
                        keyPress = KeyPress.ControlSemicolon;
                        break;
                }
            } 
            else if (keyEvent.KeyCode == Keys.Escape)
            {
                keyPress = KeyPress.Escape;
            }
            else if (keyEvent.KeyCode == Keys.Enter)
            {
                keyPress = KeyPress.Enter;
            }
            else if (keyEvent.KeyCode == Keys.Back)
            {
                keyPress = KeyPress.Backspace;
            }
            else if (keyEvent.KeyCode == Keys.F1)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F2)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F3)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F4)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F5)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F6)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F7)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F8)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F9)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F10)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F11)
            {
                keyPress = KeyPress.F12;
            }
            else if (keyEvent.KeyCode == Keys.F12)
            {
                keyPress = KeyPress.F12;
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
                    case Keys.OemSemicolon:
                        keyPress = KeyPress.Semicolon;
                        break;
                }
            }
            return keyPress;
        }

        public static TextOrHotKey AsVoidTextOrHotKeyProvisionalHack(this KeyEventArgs keyEvent)
        {
            TextOrHotKey textOrHotKey = null;
            if (keyEvent.Shift)
            {
                var character = string.Empty;
                switch (keyEvent.KeyCode)
                {
                    case Keys.A:
                        character = "A";
                        break;
                    case Keys.B:
                        character = "B";
                        break;
                    case Keys.C:
                        character = "C";
                        break;
                    case Keys.D:
                        character = "D";
                        break;
                    case Keys.E:
                        character = "E";
                        break;
                    case Keys.F:
                        character = "F";
                        break;
                    case Keys.G:
                        character = "G";
                        break;
                    case Keys.H:
                        character = "H";
                        break;
                    case Keys.I:
                        character = "I";
                        break;
                    case Keys.J:
                        character = "J";
                        break;
                    case Keys.K:
                        character = "K";
                        break;
                    case Keys.L:
                        character = "L";
                        break;
                    case Keys.M:
                        character = "M";
                        break;
                    case Keys.N:
                        character = "N";
                        break;
                    case Keys.O:
                        character = "O";
                        break;
                    case Keys.P:
                        character = "P";
                        break;
                    case Keys.Q:
                        character = "Q";
                        break;
                    case Keys.R:
                        character = "R";
                        break;
                    case Keys.S:
                        character = "S";
                        break;
                    case Keys.T:
                        character = "T";
                        break;
                    case Keys.U:
                        character = "U";
                        break;
                    case Keys.V:
                        character = "V";
                        break;
                    case Keys.W:
                        character = "W";
                        break;
                    case Keys.X:
                        character = "X";
                        break;
                    case Keys.Y:
                        character = "Y";
                        break;
                    case Keys.Z:
                        character = "Z";
                        break;
                    case Keys.OemSemicolon:
                        character = ":";
                        break;
                }
                textOrHotKey = TextOrHotKey.NewText(character);
            } 
            else if (keyEvent.Control)
            {
                HotKey hotKey = HotKey.Escape; // Random default
                switch (keyEvent.KeyCode)
                {
                    case Keys.A:
                        hotKey = HotKey.ControlA;
                        break;
                    case Keys.B:
                        hotKey = HotKey.ControlB;
                        break;
                    case Keys.C:
                        hotKey = HotKey.ControlC;
                        break;
                    case Keys.D:
                        hotKey = HotKey.ControlD;
                        break;
                    case Keys.E:
                        hotKey = HotKey.ControlE;
                        break;
                    case Keys.F:
                        hotKey = HotKey.ControlF;
                        break;
                    case Keys.G:
                        hotKey = HotKey.ControlG;
                        break;
                    case Keys.H:
                        hotKey = HotKey.ControlH;
                        break;
                    case Keys.I:
                        hotKey = HotKey.ControlI;
                        break;
                    case Keys.J:
                        hotKey = HotKey.ControlJ;
                        break;
                    case Keys.K:
                        hotKey = HotKey.ControlK;
                        break;
                    case Keys.L:
                        hotKey = HotKey.ControlL;
                        break;
                    case Keys.M:
                        hotKey = HotKey.ControlM;
                        break;
                    case Keys.N:
                        hotKey = HotKey.ControlN;
                        break;
                    case Keys.O:
                        hotKey = HotKey.ControlO;
                        break;
                    case Keys.P:
                        hotKey = HotKey.ControlP;
                        break;
                    case Keys.Q:
                        hotKey = HotKey.ControlQ;
                        break;
                    case Keys.R:
                        hotKey = HotKey.ControlR;
                        break;
                    case Keys.S:
                        hotKey = HotKey.ControlS;
                        break;
                    case Keys.T:
                        hotKey = HotKey.ControlT;
                        break;
                    case Keys.U:
                        hotKey = HotKey.ControlU;
                        break;
                    case Keys.V:
                        hotKey = HotKey.ControlV;
                        break;
                    case Keys.W:
                        hotKey = HotKey.ControlW;
                        break;
                    case Keys.X:
                        hotKey = HotKey.ControlX;
                        break;
                    case Keys.Y:
                        hotKey = HotKey.ControlY;
                        break;
                    case Keys.Z:
                        hotKey = HotKey.ControlZ;
                        break;
                    case Keys.OemSemicolon:
                        hotKey = HotKey.ControlSemicolon;
                        break;
                }
                textOrHotKey = TextOrHotKey.NewHotKey(hotKey);
            } 
            else if (keyEvent.KeyCode == Keys.Escape)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.Escape);
            }
            else if (keyEvent.KeyCode == Keys.Enter)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.Enter);
            }
            else if (keyEvent.KeyCode == Keys.Back)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.Backspace);
            }
            else if (keyEvent.KeyCode == Keys.F1)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F2)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F3)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F4)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F5)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F6)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F7)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F8)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F9)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F10)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F11)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else if (keyEvent.KeyCode == Keys.F12)
            {
                textOrHotKey = TextOrHotKey.NewHotKey(HotKey.F12);
            }
            else
            {
                string character = string.Empty;
                switch (keyEvent.KeyCode)
                {
                    case Keys.A:
                        character = "a";
                        break;
                    case Keys.B:
                        character = "b";
                        break;
                    case Keys.C:
                        character = "c";
                        break;
                    case Keys.D:
                        character = "d";
                        break;
                    case Keys.E:
                        character = "e";
                        break;
                    case Keys.F:
                        character = "f";
                        break;
                    case Keys.G:
                        character = "g";
                        break;
                    case Keys.H:
                        character = "h";
                        break;
                    case Keys.I:
                        character = "i";
                        break;
                    case Keys.J:
                        character = "j";
                        break;
                    case Keys.K:
                        character = "k";
                        break;
                    case Keys.L:
                        character = "l";
                        break;
                    case Keys.M:
                        character = "m";
                        break;
                    case Keys.N:
                        character = "n";
                        break;
                    case Keys.O:
                        character = "o";
                        break;
                    case Keys.P:
                        character = "p";
                        break;
                    case Keys.Q:
                        character = "q";
                        break;
                    case Keys.R:
                        character = "r";
                        break;
                    case Keys.S:
                        character = "s";
                        break;
                    case Keys.T:
                        character = "t";
                        break;
                    case Keys.U:
                        character = "u";
                        break;
                    case Keys.V:
                        character = "v";
                        break;
                    case Keys.W:
                        character = "w";
                        break;
                    case Keys.X:
                        character = "x";
                        break;
                    case Keys.Y:
                        character = "y";
                        break;
                    case Keys.Z:
                        character = "z";
                        break;
                    case Keys.OemSemicolon:
                        character = ";";
                        break;
                }
                textOrHotKey = TextOrHotKey.NewText(character);
            }
            return textOrHotKey;
        }
    }
}
