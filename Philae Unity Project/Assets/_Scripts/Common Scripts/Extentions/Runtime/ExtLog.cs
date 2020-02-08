using hedCommon.time;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtLog
    {
        /// <summary>
        /// is this list contain a string (nice for enum test)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="listEnum"></param>
        /// <returns></returns>
        public static void LogList<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Debug.Log(list[i]);
            }
        }

        public static void Log(object toLog)
        {
            Debug.Log(toLog);
        }

        public static void Log(object toLog, UnityEngine.Object context)
        {
            Debug.Log(toLog, context);
        }

        public static void Log(object toLog, Color color, UnityEngine.Object context = null)
        {
            string colorText = TransformColorToText(color, out bool exist);

            if (exist)
            {
                Debug.Log("<color='" + colorText + "'>" + toLog + "</color>", context);
            }
            else
            {
                Debug.Log(toLog, context);
            }
        }

        private static string TransformColorToText(Color color, out bool exist)
        {
            exist = true;
            if (color == Color.red)
            {
                return ("red");
            }
            else if (color == Color.cyan)
            {
                return ("cyan");
            }
            else if (color == Color.grey || color == Color.gray)
            {
                return ("gray");
            }
            else if (color == Color.magenta)
            {
                return ("magenta");
            }
            else if (color == Color.yellow)
            {
                return ("yellow");
            }
            else if (color == Color.black)
            {
                return ("black");
            }
            else if (color == Color.white)
            {
                return ("white");
            }
            else if (color == Color.blue)
            {
                return ("blue");
            }
            else if (color == Color.green)
            {
                return ("green");
            }
            exist = false;
            return ("");
        }

        public static void LogArray<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Debug.Log(array[i]);
            }
        }

        public static void LogIfTimeScaleNotZero(string message)
        {
            if (TimeEditor.timeScale != 0)
            {
                Debug.Log(message);
            }
        }

        public static void LogIfTimeScaleNotZero(UnityEngine.Object message)
        {
            if (TimeEditor.timeScale != 0)
            {
                Debug.Log(message);
            }
        }
    }
}