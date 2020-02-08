using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace hedCommon.extension.runtime
{
    public static class ExtEnum
    {
        /// <summary>
        /// from a given enum value (with his type), return the right index
        /// use: GetINdexOfEnum<EnumType>(enumVariable);
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="enumValue">enum value</param>
        /// <returns>index of enum</returns>
        public static int GetIndexOfEnum<T>(T enumValue)
        {
            int index = Array.IndexOf(Enum.GetValues(enumValue.GetType()), enumValue);
            return (index);
        }
        /// <summary>
        /// from a given string, return the value of the enum
        /// use: GetEnumValueFromString<EnumType>("value")
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="value"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static T GetEnumValueFromString<T>(string value)
        {
            T parsed_enum = (T)System.Enum.Parse(typeof(T), value);
            return (parsed_enum);
        }

        /// <summary>
        /// Checks if at least one of the provided flags is set in variable
        /// </summary>
        public static bool HasFlag(this Enum variable, params Enum[] flags)
        {
            if (flags.Length == 0)
                throw new ArgumentNullException("flags");

            int varInt = Convert.ToInt32(variable);

            Type T = variable.GetType();
            for (int i = 0; i < flags.Length; i++)
            {
                if (!Enum.IsDefined(T, flags[i]))
                {
                    throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    flags[i].GetType(), T));
                }
                int num = Convert.ToInt32(flags[i]);
                if ((varInt & num) == num)
                    return true;
            }
            return false;
        }

        public static bool HasFlag<T>(this T value, T flag) where T : struct
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        /// <summary>
        /// Sets a flag
        /// </summary>
        public static T Set<T>(this Enum value, T append) { return Set(value, append, true); }
        /// <summary>
        /// Sets a flag
        /// </summary>
        /// <param name="OnOff">whether to set or unset the value</param>
        public static T Set<T>(this Enum value, T append, bool OnOff)
        {
            if (append == null)
                throw new ArgumentNullException("append");

            Type type = value.GetType();
            //return the final value
            if (OnOff)
                return (T)Enum.Parse(type, (Convert.ToUInt64(value) | Convert.ToUInt64(append)).ToString());
            else
                return (T)Enum.Parse(type, (Convert.ToUInt64(value) & ~Convert.ToUInt64(append)).ToString());
        }

        /// <summary>
        /// Toggles a flag
        /// </summary>
        public static T Toggle<T>(this Enum value, T toggleValue)
        {
            if (toggleValue == null)
                throw new ArgumentNullException("toggleValue");

            Type type = value.GetType();
            //return the final value
            ulong intValue = Convert.ToUInt64(value);
            return (T)Enum.Parse(type, (intValue ^ intValue).ToString());
        }

        public static T SetAll<T>(this Enum value)
        {
            Type type = value.GetType();
            object result = value;
            string[] names = Enum.GetNames(type);
            foreach (string name in names)
            {
                ((Enum)result).Set(Enum.Parse(type, name));
            }

            return (T)result;
        }

        /// <summary>
        /// Converts a string to an enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="ignoreCase">true to ignore casing in the string</param>
        public static T ToEnum<T>(this string s, bool ignoreCase) where T : struct
        {
            // exit if null
            if (s == null || s == "")
                return default(T);

            Type genericType = typeof(T);
            if (!genericType.IsEnum)
                return default(T);

            try
            {
                return (T)Enum.Parse(genericType, s, ignoreCase);
            }

            catch (Exception)
            {
                // couldn't parse, so try a different way of getting the enums
                Array ary = Enum.GetValues(genericType);
                foreach (T en in ary.Cast<T>()
                    .Where(en =>
                        (string.Compare(en.ToString(), s, ignoreCase) == 0) ||
                        (string.Compare((en as Enum).ToString(), s, ignoreCase) == 0)))
                {
                    return en;
                }

                return default(T);
            }
        }

        #region ToEnum

        public static T ToEnum<T>(string val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                T result = (T)System.Enum.Parse(typeof(T), val, true);
                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(int val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                return (T)System.Enum.ToObject(typeof(T), val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(long val, T defaultValue) where T : struct, System.IConvertible
        {
            if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");

            try
            {
                return (T)System.Enum.ToObject(typeof(T), val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(object val, T defaultValue) where T : struct, System.IConvertible
        {
            return ToEnum<T>(System.Convert.ToString(val), defaultValue);
        }

        public static T ToEnum<T>(string val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(val, default(T));
        }

        public static T ToEnum<T>(int val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(val, default(T));
        }

        public static T ToEnum<T>(object val) where T : struct, System.IConvertible
        {
            return ToEnum<T>(System.Convert.ToString(val), default(T));
        }
        #endregion
    }
}