/// <summary>
/// MIT License
/// Copyright(c) 2019 usernameHed
/// 
///  Permission is hereby granted, free of charge, to any person obtaining a copy
///  of this software and associated documentation files (the "Software"), to deal
///  in the Software without restriction, including without limitation the rights
///  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
///  copies of the Software, and to permit persons to whom the Software is
///  furnished to do so, subject to the following conditions:
/// 
///  The above copyright notice and this permission notice shall be included in all
///  copies or substantial portions of the Software.
/// 
///  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
///  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
///  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
///  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
///  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
///  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
///  SOFTWARE.
/// </summary>

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;

namespace MoveMouseEditor
{
    public static class Win32Mouse
    {
        [DllImport("User32.Dll")]
        private static extern long SetCursorPos(int x, int y);
 
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        private static Vector2 _lastPosition;
 
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
 
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        /// <summary>
        /// change the mouse position in window
        /// </summary>
        /// <param name="positon">wanted position</param>
        public static void SetCursorPosition(Vector2 positon)
        {
            SetCursorPosition((int)positon.x, (int)positon.y);
        }
        /// <summary>
        /// change the mouse position in window
        /// </summary>
        /// <param name="x">wanted position x</param>
        /// <param name="y">wanted position y</param>
        public static void SetCursorPosition(int x, int y)
        {
            Win32Mouse.SetCursorPos(x, y);
        }

        /// <summary>
        /// change the mouse position with the previously saved position.
        /// If you did'nt saved the mouse position, set the mouse cursor
        /// to 0,0
        /// </summary>
        public static void LoadPreviouslySavedPosition()
        {
            Win32Mouse.SetCursorPosition(Win32Mouse.GetLastSavedPosition());
        }

        /// <summary>
        /// save the mouse position
        /// </summary>
        public static void SavePosition()
        {
            _lastPosition = GetCursorPosition();
        }

        /// <summary>
        /// return the last saved position
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetLastSavedPosition()
        {
            return (_lastPosition);
        }

        /// <summary>
        /// add an offset to the current mouse position
        /// </summary>
        /// <param name="offset">offset to apply</param>
        public static void AddToMousePosition(Vector2 offset)
        {
            Vector2 currentPosition = Win32Mouse.GetCursorPosition();
            Win32Mouse.SetCursorPosition(currentPosition + offset);
        }

        /// <summary>
        /// return the current mouse position X,Y
        /// </summary>
        /// <returns>mouse position</returns>
        public static Vector2 GetCursorPosition()
        {
            POINT newPoint;
            Win32Mouse.GetCursorPos(out newPoint);
            return (new Vector2(newPoint.X, newPoint.Y));
        }
    }
}
#endif