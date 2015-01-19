﻿using System;
using Eto.Forms;
using Eto.Drawing;
using Void.Core;
using Void.ViewModel;

namespace Void
{
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

        public static KeyPress AsVoidKeyPress(this KeyEventArgs keyEvent)
        {
            KeyPress keyPress = null;
            if (keyEvent.Shift)
            {
                if (keyEvent.Key == Keys.Z)
                {
                    keyPress = KeyPress.ShiftZ;
                }
                else if (keyEvent.Key == Keys.Q)
                {
                    keyPress = KeyPress.ShiftQ;
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