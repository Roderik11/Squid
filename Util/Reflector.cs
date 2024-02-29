using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// Helper class used for Reflection.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        /// The assemblies
        /// </summary>
        private static readonly Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();
        /// <summary>
        /// The type cache
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> TypeCache = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// The cache
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<Type, object>> Cache = new Dictionary<Type, Dictionary<Type, object>>();
        
        /// <summary>
        /// The properties
        /// </summary>
        private static readonly Dictionary<Type, PropertyInfo[]> Properties = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// The fields
        /// </summary>
        private static readonly Dictionary<Type, FieldInfo[]> Fields = new Dictionary<Type, FieldInfo[]>();

        //public static Mapping<T> GetMapping<T>(Type type) where T : Attribute
        //{
        //    object data = null;
        //    Dictionary<Type, object> lookup = null;

        //    if (!Cache.TryGetValue(typeof(T), out lookup))
        //    {
        //        lookup = new Dictionary<Type, object>();
        //        Cache.Add(typeof(T), lookup);
        //    }

        //    if (!lookup.TryGetValue(type, out data))
        //    {
        //        Mapping<T> map = new Mapping<T>();

        //        MemberInfo[] infos = type.GetMembers();
        //        foreach (MemberInfo info in infos)
        //        {
        //            object[] att = info.GetCustomAttributes(typeof(T), false);
        //            if (att.Length > 0)
        //            {
        //                Field<T> field = new Field<T>();
        //                field.Info = info;
        //                field.Attribute = att[0] as T;

        //                CategoryAttribute cat = field.GetAttribute<CategoryAttribute>();
        //                if (cat != null)
        //                    field.Category = cat.Category;

        //                DescriptionAttribute desc = field.GetAttribute<DescriptionAttribute>();
        //                if (desc != null)
        //                    field.Description = desc.Description;

        //                map.Add(field);
        //            }
        //        }

        //        map.Sort((a, b) => a.Category.CompareTo(b.Category));
        //        lookup.Add(type, map);

        //        data = map;
        //    }

        //    return data as Mapping<T>;
        //}

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>PropertyInfo[][].</returns>
        public static PropertyInfo[] GetProperties<T>()
        {
            return GetProperties(typeof(T));
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>PropertyInfo[][].</returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            if (!Properties.ContainsKey(type))
                Properties.Add(type, type.GetProperties());
            return Properties[type];
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>PropertyInfo[][].</returns>
        public static FieldInfo[] GetFields<T>()
        {
            return GetFields(typeof(T));
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>PropertyInfo[][].</returns>
        public static FieldInfo[] GetFields(Type type)
        {
            if (!Fields.ContainsKey(type))
                Fields.Add(type, type.GetFields());
            return Fields[type];
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Object.</returns>
        public static object GetFieldValue(object obj, string name)
        {
            FieldInfo field = obj.GetType().GetField(name);

            if (field != null)
                return field.GetValue(obj);

            return null;
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null)
                return;

            foreach (Assembly assembly in assemblies)
            {
                if (!Assemblies.ContainsKey(assembly.FullName))
                    Assemblies.Add(assembly.FullName, assembly);
            }
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns>``0.</returns>
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            object[] atts = type.GetCustomAttributes(typeof(T), false);
            if (atts.Length > 0)
                return (T)atts[0];
            return null;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>``0.</returns>
        public static T GetAttribute<T>(Type type, bool inherit) where T : Attribute
        {
            object[] atts = type.GetCustomAttributes(typeof(T), inherit);
            if (atts.Length > 0)
                return (T)atts[0];
            return null;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Type.</returns>
        public static Type GetType(string name)
        {
            Type result = null;
            Assembly main = Assembly.GetAssembly(typeof(Squid.Control));
            result = main.GetType(name);
            if (result != null) return result;

            foreach (Assembly assembly in Assemblies.Values)
            {
                result = assembly.GetType(name);
                if (result != null) return result;
            }

            return null;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="required">The required.</param>
        /// <returns>List{Type}.</returns>
        public static List<Type> GetTypes(Type required)
        {
            if (TypeCache.ContainsKey(required))
                return TypeCache[required];

            List<Type> types = new List<Type>();
            List<Type> result = new List<Type>();

            Assembly main = Assembly.GetAssembly(required);
            types.AddRange(main.GetTypes());

            foreach (Assembly assembly in Assemblies.Values)
            {
                if (main.FullName != assembly.FullName)
                    types.AddRange(assembly.GetTypes());
            }

            foreach (Type type in types)
            {
                if (required.IsInterface && required.IsAssignableFrom(type))
                    result.Add(type);
                else if (type.IsSubclassOf(required))
                    result.Add(type);
            }

            TypeCache.Add(required, result);

            return result;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List{Type}.</returns>
        public static List<Type> GetTypes<T>()
        {
            return GetTypes(typeof(T));
        }
    }
}
