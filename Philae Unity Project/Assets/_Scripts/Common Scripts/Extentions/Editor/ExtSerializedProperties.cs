using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace hedCommon.extension.editor
{
    /// <summary>
    /// Extension class for SerializedProperties
    /// See also: http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
    /// 
    /// use:
    ///  // Get a serialized object
    /// var serializedObject = new UnityEditor.SerializedObject(target);
    ///
    /// Set the property debug to true
    /// serializedObject.FindProperty("debug").SetValue<bool>(true);
    ///
    /// Get the property value of debug
    /// bool debugValue = serializedObject.FindProperty("debug").GetValue<bool>();
    /// </summary>
    public static class ExtSerializedProperties
    {
        public static void UpdateEditor(this Editor editor)
        {
            editor.serializedObject.Update();
        }

        public static void ApplyModification(this Editor editor, bool withUndo = true)
        {
            if (withUndo)
            {
                editor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                editor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public static SerializedProperty GetPropertie(this Editor editor, string propertie)
        {
            return (editor.serializedObject.FindProperty(propertie));
        }

        public static SerializedProperty GetPropertie(this SerializedProperty editor, string propertie)
        {
            return (editor.FindPropertyRelative(propertie));
        }

        public static T GetValue<T>(this Editor editor, string propertieName)
        {
            SerializedProperty property = editor.GetPropertie(propertieName);
            return (property.GetValue<T>());
        }

        public static bool SetValue<T>(this Editor editor, string propertieName, T value)
        {
            SerializedProperty property = editor.GetPropertie(propertieName);
            return (property.SetValue(value));
        }

        /// <summary>
        /// Get the object the serialized property of a Component holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValue<T>(this SerializedProperty property)
        {
            if (property == null)
            {
                return (default(T));
            }
            UnityEngine.Object tar = property.serializedObject.targetObject;
            object obj = tar as Component;
            if (obj != null)
            {
                return (GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property)));
            }
            obj = tar as ScriptableObject;
            if (obj != null)
            {
                return (GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootScriptableObject(property)));
            }
            return (default(T));
        }

        /// <summary>
        /// Get the object the serialized property of a Component holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static bool SetValue<T>(this SerializedProperty property, T value)
        {
            if (property == null)
            {
                return (false);
            }
            UnityEngine.Object tar = property.serializedObject.targetObject;
            object obj = tar as Component;
            if (obj != null)
            {
                bool setValue = SetValueOnComponent<T>(property, value);
                return (setValue);
            }
            obj = tar as ScriptableObject;
            if (obj != null)
            {
                return (SetValueOnScriptableObject<T>(property, value));
            }
            return (false);
        }

        /// <summary>
        /// Get the object the serialized property of a Component holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValueFromComponents<T>(this SerializedProperty property)
        {
            return (GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property)));
        }

        /// <summary>
        /// Get the object the serialized property of a ScriptableObject holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        private static T GetValueFromScriptableObject<T>(this SerializedProperty property)
        {
            return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootScriptableObject(property));
        }

        /// <summary>
        /// Set the value of a field of the property with the type T on a Component
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        private static bool SetValueOnComponent<T>(this SerializedProperty property, T value)
        {
            object obj = GetSerializedPropertyRootComponent(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }
            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);

        }

        /// <summary>
        /// Set the value of a field of the property with the type T on a ScriptableObject
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        private static bool SetValueOnScriptableObject<T>(this SerializedProperty property, T value)
        {

            object obj = GetSerializedPropertyRootScriptableObject(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }
            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);

        }

        /// <summary>
        /// Get the component of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the component</param>
        /// <returns>The root component of the property</returns>
        public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
        {
            return (Component)property.serializedObject.targetObject;
        }

        /// <summary>
        /// Get the scriptable object of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the scriptable object</param>
        /// <returns>The root scriptable object of the property</returns>
        public static ScriptableObject GetSerializedPropertyRootScriptableObject(SerializedProperty property)
        {
            return (ScriptableObject)property.serializedObject.targetObject;
        }

        /// <summary>
        /// Iterates through objects to handle objects that are nested in the root object
        /// </summary>
        /// <typeparam name="T">The type of the nested object</typeparam>
        /// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
        /// <param name="obj">The root object from which this path leads to the property</param>
        /// <param name="includeAllBases">Include base classes and interfaces as well</param>
        /// <returns>Returns the nested object casted to the type T</returns>
        public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
        {
            foreach (string part in path.Split('.'))
            {
                obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
            }
            return (T)obj;
        }

        public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null) return (T)field.GetValue(obj);

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null) return (T)property.GetValue(obj, null);

            if (includeAllBases)
            {

                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null) return (T)field.GetValue(obj);

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null) return (T)property.GetValue(obj, null);
                }
            }

            return default(T);
        }

        public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }

            if (includeAllBases)
            {
                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                        return true;
                    }

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null)
                    {
                        property.SetValue(obj, value, null);
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            List<Type> allTypes = new List<Type>();

            if (includeSelf) allTypes.Add(type);

            if (type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }
            else
            {
                allTypes.AddRange(
                        Enumerable
                        .Repeat(type.BaseType, 1)
                        .Concat(type.GetInterfaces())
                        .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                        .Distinct());
            }

            return allTypes;
        }

        public static List<int> GetListInt(SerializedProperty listProp)
        {
            List<int> newList = new List<int>(listProp.arraySize);
            for (int i = 0; i < listProp.arraySize; i++)
            {
                newList.Add(listProp.GetArrayElementAtIndex(i).intValue);
            }
            return (newList);
        }

        public static void SetListInt(SerializedProperty listprop, List<int> datas)
        {
            listprop.arraySize = datas.Count;
            for (int i = 0; i < listprop.arraySize; i++)
            {
                listprop.GetArrayElementAtIndex(i).intValue = datas[i];
            }
        }
    }
}
