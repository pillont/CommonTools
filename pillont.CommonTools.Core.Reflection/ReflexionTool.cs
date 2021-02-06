using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using pillont.CommonTools.Core.Reflection.ReflectionCaches;

namespace pillont.CommonTools.Core.Reflection
{
    /// <summary>
    /// Reflexion tools
    /// </summary>
    public static class ReflectionTools
    {
        private readonly static Lazy<PropertiesCache> m_LazyAllPropertiesCache = new Lazy<PropertiesCache>(() => new PropertiesCache(true));
        private readonly static Lazy<AssembliesCache> m_LazyAssembliesCache = new Lazy<AssembliesCache>();
        private readonly static Lazy<CustomAttributeCache> m_LazyCustomAttributeCache = new Lazy<CustomAttributeCache>();
        private readonly static Lazy<FieldsCache> m_LazyFieldsCache = new Lazy<FieldsCache>();
        private readonly static Lazy<GenericArgumentsCache> m_LazyGenericArgumentCache = new Lazy<GenericArgumentsCache>();
        private readonly static Lazy<GenericTypeDefinitionCache> m_LazyGenericTypeDefinitionCache = new Lazy<GenericTypeDefinitionCache>();
        private readonly static Lazy<InterfacesCache> m_LazyInterfacesCache = new Lazy<InterfacesCache>();
        private readonly static Lazy<MethodsCache> m_LazyMethodsCache = new Lazy<MethodsCache>();
        private readonly static Lazy<MethodCacheWithParameters> m_LazyMethodsCacheWithParameters = new Lazy<MethodCacheWithParameters>();
        private readonly static Lazy<PropertiesCache> m_LazyPropertiesCache = new Lazy<PropertiesCache>(() => new PropertiesCache(false));
        private readonly static Lazy<TypesInAssemblyCache> m_LazyTypesInAssemblyCache = new Lazy<TypesInAssemblyCache>();

        /// <summary>
        /// Can be cast to type T
        /// </summary>
        /// <typeparam name="T">Type to cast</typeparam>
        /// <param name="p_Param">object to cast</param>
        /// <returns>true if cast is possible else false</returns>
        public static bool CanBeCast<T>(this object p_Param)
        {
            try
            {
                T v_Object = (T)p_Param;
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        /// <summary>
        /// Cast to Type T
        /// </summary>
        /// <typeparam name="T">type of cast</typeparam>
        /// <param name="p_ValueToCast">Object to cast</param>
        /// <returns>Object casted</returns>
        public static T Cast<T>(this object p_ValueToCast)
        {
            return (T)p_ValueToCast;
        }

        /// <summary>
        /// Cast to type defined in parameter
        /// </summary>
        /// <param name="v_ValueToCast">Object to cast</param>
        /// <param name="p_Type">Target type for cast</param>
        /// <returns>Object casted</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object Cast(this object v_ValueToCast, Type p_Type)
        {
            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(typeof(ReflectionTools), nameof(Cast), typeof(object))
                ?.MakeGenericMethod(p_Type)
                .Invoke(null, new object[1] { v_ValueToCast });
        }

        /// <summary>
        /// Cast a IEnumerable of List of Type in parameter
        /// </summary>
        /// <param name="p_Enumerable">Enumerable to cast</param>
        /// <param name="p_SubType">Target type for cast</param>
        /// <returns>List of type casted</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object CastList(this IEnumerable p_Enumerable, Type p_SubType)
        {
            object v_Enumerable = m_LazyMethodsCacheWithParameters.Value.CollectWithCache(typeof(Enumerable), nameof(Enumerable.Cast), typeof(IEnumerable))
                  ?.MakeGenericMethod(p_SubType)
                  .Invoke(null, new object[1] { p_Enumerable });

            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(typeof(Enumerable), nameof(Enumerable.ToList))
                ?.MakeGenericMethod(p_SubType)
                .Invoke(null, new object[] { v_Enumerable });
        }

        /// <summary>
        /// Inform if property contains attribute with wanted type
        /// </summary>
        public static bool ContainsAttribute<T>(this MemberInfo p_Prop) where T : Attribute
        {
            return m_LazyCustomAttributeCache.Value.CollectWithCache(p_Prop)?.OfType<T>().Count() > 0;
        }

        /// <summary>
        /// Convert a Expression<Func<T1, T>> en Expression<Func<T2, T>> where T2 : T1
        /// </summary>
        /// <typeparam name="TIn">Source type of the func (T1)</typeparam>
        /// <typeparam name="TOut">Target type of the func (T2)</typeparam>
        /// <typeparam name="TR">Common type of the func</typeparam>
        /// <param name="p_Expression">Source expression to convert</param>
        /// <returns>Converted expression</returns>
        public static Expression<Func<TOut, TR>> ConvertExpression<TIn, TOut, TR>(Expression<Func<TIn, TR>> p_Expression)
            where TOut : TIn
        {
            if (p_Expression != null)
            {
                ParameterExpression v_oldParam = p_Expression.Parameters[0];
                ParameterExpression v_newParam = Expression.Parameter(typeof(TOut), v_oldParam.Name);

                return Expression.Lambda<Func<TOut, TR>>(
                            p_Expression.Body.ReplaceParameter(v_oldParam, v_newParam), v_newParam);
            }
            else return null;
        }

        /// <summary>
        /// Convert a Expression<Func<T1, T>> en Expression<Func<T2, T>> where T2 : T1 (call in reflection)
        /// </summary>
        /// <param name="p_TIn">Source type of the func (T1)</param>
        /// <param name="p_TOut">Target type of the func (T2)</param>
        /// <param name="p_TCommon">Common type of the func</param>
        /// <param name="p_Expression">Source expression to convert</param>
        /// <returns>Converted expression</returns>
        public static object ConvertExpressionGeneric(Type p_TIn, Type p_TOut, Type p_TCommon, object p_Expression)
        {
            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(typeof(ReflectionTools), nameof(ReflectionTools.ConvertExpression))
                ?.MakeGenericMethod(p_TIn, p_TOut, p_TCommon)
                .Invoke(null, new object[1] { p_Expression });
        }

        /// <summary>
        /// Get a class from an interface in a given assembly
        /// </summary>
        /// <param name="p_Assembly">Assembly to get class from</param>
        /// <param name="p_Interface">Interface to get class from</param>
        /// <returns>Class</returns>
        public static IEnumerable<Type> GetClassFromInterface(Assembly p_Assembly, Type p_Interface)
        {
            if (p_Assembly == null)
                throw new ArgumentNullException();

            if (p_Interface == null)
                throw new ArgumentNullException();

            if (!p_Interface.GetTypeInfo().IsInterface)
                throw new ArgumentNullException();

            return m_LazyTypesInAssemblyCache.Value.CollectWithCache(p_Assembly)
                .Where(p => p_Interface.IsAssignableFrom(p));
        }

        /// <summary>
        /// Inform if property contains attribute with wanted type
        /// </summary>
        public static IEnumerable<T> GetCustomAttribute<T>(this MemberInfo p_Prop) where T : Attribute
        {
            return m_LazyCustomAttributeCache.Value.CollectWithCache(p_Prop)?.OfType<T>();
        }

        /// <summary>
        /// Retrieve all derived type of a type. Get all none abstract type which can be instanciate
        /// </summary>
        /// <param name="p_Type"></param>
        /// <returns></returns>
        public static IList<Type> GetDerivedTypeFromAbstractType(this Type p_Type)
        {
            if (!p_Type.IsAbstract)
                throw new ArgumentException("Type must be abstract", nameof(p_Type));

            List<Type> v_ListType = new List<Type>();
            IList<Assembly> v_Assemblies = m_LazyAssembliesCache.Value?.CollectWithCache();
            if (v_Assemblies != null && v_Assemblies.Count > 0)
                foreach (Assembly v_Assembly in v_Assemblies)
                    v_ListType.AddRange(m_LazyTypesInAssemblyCache.Value.CollectWithCache(v_Assembly)?.Where(t => t.IsSubclassOf(p_Type) && !t.IsAbstract));

            return v_ListType;
        }

        /// <summary>
        /// Get PropertiesInfo for a type
        /// </summary>
        /// <param name="p_Type">Type to treat</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFieldsOfType(Type p_Type)
        {
            if (p_Type == null)
                throw new ArgumentNullException(nameof(p_Type), "Type cannot be null");

            return m_LazyFieldsCache.Value.CollectWithCache(p_Type);
        }

        /// <summary>
        /// Get Arguments for a type
        /// </summary>
        /// <param name="p_Type">Type to treat</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetGenericArgumentsOfType(Type p_Type)
        {
            if (p_Type == null)
                throw new ArgumentNullException(nameof(p_Type), "Type cannot be null");

            return m_LazyGenericArgumentCache.Value.CollectWithCache(p_Type);
        }

        /// <summary>
        /// Return the first Generic type from a type
        /// Exmple : GetGenericType(List<object>) will return
        /// the Type object
        /// </summary>
        /// <param name="p_Type">Type to get generic type</param>
        /// <returns>Type corresponding to generic type</returns>
        public static Type GetGenericType(Type p_Type)
        {
            try
            {
                IEnumerable<Type> v_Types = GetGenericTypes(p_Type);
                return v_Types.First();
            }
            catch (InvalidOperationException)
            { return null; }
        }

        /// <summary>
        /// Return a list of Generic Types from a type
        /// Example : GetGenericTypes(Dictionnary<string, object>) will return
        /// an array with [string, object]
        /// </summary>
        /// <param name="p_Type">Type to get generic types</param>
        /// <returns>Array of generic types</returns>
        public static IEnumerable<Type> GetGenericTypes(Type p_Type)
        {
            IEnumerable<Type> v_Types = GetGenericArgumentsOfType(p_Type);
            if (v_Types == null || v_Types.Count() == 0)
            {
                throw new InvalidOperationException(String.Format("{0} n'a pas de type générique", p_Type.FullName));
            }
            return v_Types;
        }

        /// <summary>
        /// Get Interfaces for a type
        /// </summary>
        /// <param name="p_Type">Type to treat</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInterfacesOfType(Type p_Type)
        {
            if (p_Type == null)
                throw new ArgumentNullException(nameof(p_Type), "Type cannot be null");

            return m_LazyInterfacesCache.Value.CollectWithCache(p_Type);
        }

        /// <summary>
        /// Get PropertiesInfo for a type
        /// </summary>
        /// <param name="p_type">Type to treat</param>
        /// <param name="p_All">All properties or not</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesOfType(Type p_Type, bool p_All = false)
        {
            if (p_Type == null)
                throw new ArgumentNullException(nameof(p_Type), "Type cannot be null");

            Lazy<PropertiesCache> v_PropCache = p_All
                ? m_LazyAllPropertiesCache
                : m_LazyPropertiesCache;

            return v_PropCache.Value.CollectWithCache(p_Type);
        }

        /// <summary>
        /// Get Property value by its name from an object
        /// </summary>
        /// <param name="p_Object">Object from which to get property value</param>
        /// <param name="p_PropertyName">Property name to get value</param>
        /// <returns>Object corresponding to value</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object GetProperty(object p_Object, String p_PropertyName)
        {
            if (p_Object == null)
                throw new ArgumentNullException(nameof(p_Object));

            PropertyInfo v_Prop = null;
            try
            {
                v_Prop = GetPropertyInfo(p_Object, p_PropertyName);
                if (v_Prop != null)
                    return v_Prop.GetValue(p_Object);
                else return null;
            }
            catch
            { return null; }
            finally { v_Prop = null; }
        }

        /// <summary>
        /// Get Property value by its name from an object and cast result to T
        /// </summary>
        /// <typeparam name="T">Type to cast result</typeparam>
        /// <param name="p_Object">Object from which to get property value</param>
        /// <param name="p_PropertyName"></param>
        /// <returns>Object corresponding to value casted to T</returns>
        public static T GetProperty<T>(object p_Object, String p_PropertyName)
        {
            return (T)GetProperty(p_Object, p_PropertyName);
        }

        /// <summary>
        /// Get type of a property of an object
        /// </summary>
        /// <param name="p_Object">Object to search</param>
        /// <param name="p_PropertyName">Name of property</param>
        /// <returns>Type of the property</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type GetPropertyType(object p_Object, string p_PropertyName)
        {
            PropertyInfo v_prop = null;
            try
            {
                v_prop = GetPropertiesOfType(p_Object.GetType(), true).SingleOrDefault(c => c.Name.Equals(p_PropertyName, StringComparison.OrdinalIgnoreCase));
                if (v_prop != null)
                    return v_prop.PropertyType;
                else return null;
            }
            catch
            { return null; }
            finally { v_prop = null; }
        }

        /// <summary>
        /// collect first type with same name by assembly
        /// </summary>
        /// <returns> wanted type | null if not found </returns>
        /// <param name="p_ModelTypeName"> simple type name </param>
        /// <param name="p_AssemblyName"> complete name of the assembly </param>
        /// <example>
        /// "System.Collections.Generic","List" => type <System.Collections.Generic.List>
        /// </example>
        public static Type GetTypeFromAssembly(string p_ModelTypeName, string p_AssemblyName)
        {
            if (string.IsNullOrWhiteSpace(p_ModelTypeName))
                throw new ArgumentException("type name is empty", nameof(p_ModelTypeName));
            if (string.IsNullOrWhiteSpace(p_AssemblyName))
                throw new ArgumentException("assembly name is empty", nameof(p_AssemblyName));

            Assembly v_Assembly = m_LazyAssembliesCache.Value?.CollectWithCache(p_AssemblyName);
            if (v_Assembly == null)
                return null;

            return GetTypeFromAssembly(p_ModelTypeName, v_Assembly);
        }

        /// <summary>
        /// collect type by name and assembly
        /// </summary>
        public static Type GetTypeFromAssembly(string p_ModelTypeName, Assembly p_Assembly)
        {
            return GetTypeFromAssembly(p_ModelTypeName, typeof(object), p_Assembly);
        }

        /// <summary>
        /// collect type by name and assembly
        /// </summary>
        public static Type GetTypeFromAssembly(string p_ModelTypeName, Type p_BaseClass, Assembly p_Assembly)
        {
            return GetTypesFromAssembly(p_Assembly)
                            .Where(p_Type => p_BaseClass.IsAssignableFrom(p_Type))
                            .FirstOrDefault(p_Type => p_Type.Name == p_ModelTypeName);
        }

        public static IEnumerable<Type> GetTypesFromAssembly(Assembly p_Assembly)
        {
            return m_LazyTypesInAssemblyCache.Value.CollectWithCache(p_Assembly);
        }

        /// <summary>
        /// Get Types from assembly for a type
        /// </summary>
        /// <param name="p_Type">Type to treat</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesFromAssemblyForType(Type p_Type)
        {
            if (p_Type == null)
                throw new ArgumentNullException(nameof(p_Type), "Type cannot be null");

            return m_LazyTypesInAssemblyCache.Value.CollectWithCache(p_Type);
        }

        /// <summary>
        /// Get types in a namespace
        /// </summary>
        /// <param name="p_Assembly">Assembly where the namespace is</param>
        /// <param name="p_NameSpace"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static List<Type> GetTypesInNamespace(Assembly p_Assembly, string p_NameSpace)
        {
            return m_LazyTypesInAssemblyCache.Value.CollectWithCache(p_Assembly)
                .Where(t => String.Equals(t.Namespace, p_NameSpace, StringComparison.Ordinal)).ToList();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Boolean ImplementsIDictionary(Type p_Type)
        {
            return GetInterfacesOfType(p_Type).Any(x =>
                   x.IsGenericType &&
                   (m_LazyGenericTypeDefinitionCache.Value.CollectWithCache(x) == typeof(IDictionary)
                    || m_LazyGenericTypeDefinitionCache.Value.CollectWithCache(x) == typeof(IDictionary<,>)));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Boolean ImplementsIList(Type p_Type)
        {
            return GetInterfacesOfType(p_Type).Any(x =>
                   x.IsGenericType &&
                   (m_LazyGenericTypeDefinitionCache.Value.CollectWithCache(x) == typeof(IList)
                    || m_LazyGenericTypeDefinitionCache.Value.CollectWithCache(x) == typeof(IList<>)));
        }

        /// <summary>
        /// Call a static method with a generic type parameter by its name on a type result
        /// </summary>
        /// <typeparam name="T">Generic parameter</typeparam>
        /// <param name="p_Object"></param>
        /// <param name="p_MethodName"></param>
        /// <param name="p_Params"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeGenericMethod<T>(object p_Object, String p_MethodName, params object[] p_Params)
        {
            Type v_GenericType = GetGenericType(typeof(T));

            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(p_Object.GetType(), p_MethodName)
                .MakeGenericMethod(v_GenericType)
                .Invoke(p_Object, p_Params);
        }

        /// <summary>
        /// Call a static method with a generic type parameter by its name on a type result and cast result to U
        /// </summary>
        /// <typeparam name="T">Generic Parameter</typeparam>
        /// <typeparam name="U">Type result</typeparam>
        /// <param name="p_Object"></param>
        /// <param name="p_MethodName"></param>
        /// <param name="p_Params"></param>
        /// <returns></returns>
        public static U InvokeGenericMethod<T, U>(object p_Object, String p_MethodName, params object[] p_Params)
        {
            return (U)InvokeGenericMethod<T>(p_Object, p_MethodName, p_Params);
        }

        /// <summary>
        /// Call a method by its name on an object
        /// </summary>
        /// <param name="p_Object">Object to call method</param>
        /// <param name="p_MethodName">Method name to call</param>
        /// <param name="p_Params">List of params to send to method</param>
        /// <returns>Object value returned by method</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeMethod(object p_Object, String p_MethodName, params object[] p_Params)
        {
            Type v_currentType = p_Object.GetType();

            MethodInfo method = v_currentType.GetMethod(p_MethodName);
            if (method == null)
                method = v_currentType.GetInterfaces()
                    .Select(c => v_currentType.GetInterfaceMap(c).TargetMethods)
                        .Select(c => c.FirstOrDefault(z => z.Name.EndsWith($".{p_MethodName}"))).Where(c => c != null).FirstOrDefault();
            return method.Invoke(p_Object, p_Params);
        }

        /// <summary>
        /// Call a method by its name on an object casted to T
        /// </summary>
        /// <param name="p_Object">Object to call method</param>
        /// <param name="p_MethodName">Method name to call</param>
        /// <param name="p_Params">List of params to send to method</param>
        /// <returns>Object value returned by method casted to T</returns>
        public static T InvokeMethod<T>(object p_Object, String p_MethodName, params object[] p_Params)
        {
            return (T)InvokeMethod(p_Object, p_MethodName, p_Params);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_Type"></param>
        /// <param name="p_MethodName"></param>
        /// <param name="p_Params"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeStaticGenericMethod<T>(Type p_Type, String p_MethodName, params object[] p_Params)
        {
            Type v_GenericType = typeof(T);

            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(p_Type, p_MethodName)
                .MakeGenericMethod(v_GenericType)
                .Invoke(null, p_Params);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p_Type"></param>
        /// <param name="p_GenericType"></param>
        /// <param name="p_MethodName"></param>
        /// <param name="p_Params"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeStaticGenericMethod(Type p_Type, Type p_GenericType, String p_MethodName, params object[] p_Params)
        {
            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(p_Type, p_MethodName, p_GenericType)
                .MakeGenericMethod(p_GenericType)
                .Invoke(null, p_Params);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeStaticGenericMethod(Type p_Type, Type[] p_GenericType, String p_MethodName, params object[] p_Params)
        {
            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(p_Type, p_MethodName, p_GenericType)
                .MakeGenericMethod(p_GenericType)
                .Invoke(null, p_Params);
        }

        /// <summary>
        /// Call a static method by its name on a type
        /// </summary>
        /// <param name="p_Type">Type to call static method</param>
        /// <param name="p_MethodName">Method name to call</param>
        /// <param name="p_Params">List of params to send to method</param>
        /// <returns>Object value returned by method</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object InvokeStaticMethod(Type p_Type, String p_MethodName, params object[] p_Params)
        {
            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(p_Type, p_MethodName)
                .Invoke(null, p_Params);
        }

        /// <summary>
        /// Call a static method by its name on a type result casted to T
        /// </summary>
        /// <param name="p_Type">Type to call static method</param>
        /// <param name="p_MethodName">Method name to call</param>
        /// <param name="p_Params">List of params to send to method</param>
        /// <returns>Object value returned by method casted to T</returns>
        public static T InvokeStaticMethod<T>(Type p_Type, String p_MethodName, params object[] p_Params)
        {
            return (T)InvokeStaticMethod(p_Type, p_MethodName, p_Params);
        }

        public static bool IsAssignableTo<T>(string p_ClassName)
        {
            if (!String.IsNullOrEmpty(p_ClassName))
            {
                Type v_InterfaceType = typeof(T);

                //get all classes which inherits abstract class
                return ReflectionTools.GetTypesFromAssemblyForType(v_InterfaceType)
                   .Count(c => v_InterfaceType.IsAssignableFrom(c)
                       && !c.IsAbstract //exclude abstract class
                       && c.Name == p_ClassName) > 0;
            }
            else
                return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Boolean IsDictionary(Type p_Type)
        {
            return typeof(IDictionary).IsAssignableFrom(p_Type);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Boolean IsDictionary(object p_List)
        {
            return p_List is IDictionary || IsDictionary(p_List.GetType()) || ImplementsIDictionary(p_List.GetType());
        }

        /// <summary>
        /// Indicates if a type is a List (Assignable from IList)
        /// </summary>
        /// <param name="p_Type">Type to test</param>
        /// <returns>Boolean, true if it is a type list</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Boolean IsList(Type p_Type)
        {
            return typeof(IList).IsAssignableFrom(p_Type) || ImplementsIList(p_Type);
        }

        /// <summary>
        /// Indicates if an object is a List (IEnumerable or assignable from IList)
        /// </summary>
        /// <param name="p_List">Object to test</param>
        /// <returns>Boolean, true if it is an object list</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Boolean IsList(object p_List, Type p_ListType = null)
        {
            Boolean v_List = p_List is IList || IsList(p_List.GetType());
            if (p_ListType != null)
            {
                if (v_List)
                {
                    Type v_ListType = GetGenericArgumentsOfType(p_List.GetType()).First();
                    if (v_ListType == null || (!v_ListType.Equals(p_ListType) && !v_ListType.IsSubclassOf(p_ListType)))
                        v_List = false;
                }
            }
            return v_List;
        }

        /// <summary>
        /// Verify if the property exist for this object
        /// </summary>
        /// <param name="p_Object">Object from which to get property value</param>
        /// <param name="p_PropertyName">Property name to get value</param>
        /// <returns></returns>
        public static bool IsPropertyExists(object p_Object, String p_PropertyName)
        {
            PropertyInfo v_prop = null;
            try
            {
                v_prop = GetPropertiesOfType(p_Object.GetType(), true).SingleOrDefault(c => c.Name.Equals(p_PropertyName, StringComparison.OrdinalIgnoreCase));
                if (v_prop != null)
                    return true;
                else return false;
            }
            catch
            { return false; }
            finally { v_prop = null; }
        }

        public static bool MethodExistsInMethod(Type p_Type, String p_MethodName)
        {
            return m_LazyMethodsCacheWithParameters.Value.CollectWithCache(p_Type, p_MethodName) != null;
        }

        /// <summary>
        /// Create new instance of p_Type
        /// </summary>
        /// <param name="p_Type">Type of class to instanciate</param>
        /// <returns>New object</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object New(Type p_Type)
        {
            return Activator.CreateInstance(p_Type);
        }

        /// <summary>
        /// Create new instance of p_Type
        /// </summary>
        /// <param name="p_Type">Type of class to instanciate</param>
        /// <param name="p_params">Params for the constructor</param>
        /// <returns>New object</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object New(Type p_Type, params object[] p_params)
        {
            return Activator.CreateInstance(p_Type, p_params);
        }

        /// <summary>
        /// Create new instance of p_Type and cast it to T
        /// </summary>
        /// <typeparam name="T">Generic type to cast</typeparam>
        /// <param name="p_Type">Type of class to instanciate</param>
        /// <returns>New object casted to T</returns>
        public static T New<T>(Type p_Type)
        {
            return (T)New(p_Type);
        }

        /// <summary>
        /// Create new instance of Type T
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <returns>New object casted to T</returns>
        public static T New<T>()
        {
            return (T)New(typeof(T));
        }

        /// <summary>
        /// Create new instance of List of type p_Type
        /// </summary>
        /// <param name="p_Type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IList NewList(Type p_Type)
        {
            return (IList)typeof(List<>)
              .MakeGenericType(p_Type)
              .GetConstructor(Type.EmptyTypes)
              .Invoke(null);
        }

        /// <summary>
        /// Set Member value by its name in an object
        /// </summary>
        /// <param name="p_Object">Object where you want to set Member value</param>
        /// <param name="p_PropertyName">Member name to set</param>
        /// <param name="p_PropertyValue">Member value to set</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SetMember(object p_Object, String p_MemberName, object p_MemberValue)
        {
            FieldInfo v_field = null;
            try
            {
                v_field = GetFieldsOfType(p_Object.GetType()).SingleOrDefault(c => c.Name.Equals(p_MemberName, StringComparison.OrdinalIgnoreCase));
                if (v_field != null)
                    v_field.SetValue(p_Object, p_MemberValue);
            }
            finally { v_field = null; }
        }

        /// <summary>
        /// Set Property value by its name in an object
        /// </summary>
        /// <param name="p_Object">Object where you want to set Property value</param>
        /// <param name="p_PropertyName">Property name to set</param>
        /// <param name="p_PropertyValue">Property value to set</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SetProperty(object p_Object, String p_PropertyName, object p_PropertyValue)
        {
            PropertyInfo v_prop = null;
            try
            {
                v_prop = GetPropertiesOfType(p_Object.GetType(), true).SingleOrDefault(c => c.Name.Equals(p_PropertyName, StringComparison.OrdinalIgnoreCase));
                if (v_prop != null)
                    v_prop.SetValue(p_Object, p_PropertyValue);
            }
            finally { v_prop = null; }
        }

        /// <summary>
        /// try to get Property value by its name from an object
        /// if return true p_Result have the same value (same for null value)
        /// if return false p_Result is null because property not found
        /// </summary>
        /// <param name="p_Object">Object from which to get property value</param>
        /// <param name="p_PropertyName">Property name to get value</param>
        /// <param name="p_Result">Object corresponding to value</param>
        /// <returns>property exist in the object or not</returns>
        public static bool TryGetProperty(object p_Object, String p_PropertyName, out object p_Result)
        {
            try
            {
                var v_Prop = GetPropertyInfo(p_Object, p_PropertyName);
                if (v_Prop == null)
                {
                    p_Result = null;
                    return false;
                }

                p_Result = v_Prop.GetValue(p_Object);
                return true;
            }
            catch
            {
                p_Result = null;
                return false;
            }
        }

        private static PropertyInfo GetPropertyInfo(object p_Object, string p_PropertyName)
        {
            return GetPropertiesOfType(p_Object.GetType(), true).SingleOrDefault(c => c.Name.Equals(p_PropertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Replace a parameter in a lambda expression
        /// </summary>
        /// <param name="p_expression">Expression to treat</param>
        /// <param name="p_sourceParam">Parameter to search and replace</param>
        /// <param name="p_targetParam">New parameter</param>
        /// <returns>Expression with the parameter replaced</returns>
        private static Expression ReplaceParameter(this Expression p_expression,
            ParameterExpression p_sourceParam, ParameterExpression p_targetParam)
        {
            return new ReplaceVisitor(p_sourceParam, p_targetParam).Visit(p_expression);
        }

        /// <summary>
        /// Custom class to replace a expression node in a lambda expression
        /// </summary>
        private class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression m_from = null, m_to = null;

            public ReplaceVisitor(Expression p_from, Expression p_to)
            {
                this.m_from = p_from;
                this.m_to = p_to;
            }

            /// <summary>
            /// Replace all expression node equal to from member with to member. Browse all expression tree
            /// </summary>
            /// <param name="p_currentNode">Current Expression tree</param>
            /// <returns>new Expression Tree with replaced node</returns>
            public override Expression Visit(Expression p_currentNode)
            {
                if (p_currentNode == m_from //if current expression node is equal to from node
                    && this.m_to != null //if to expression node not null, to allow the replace
                    )
                    return m_to; //return new node
                else
                    return base.Visit(p_currentNode); //else browse the tree
            }
        }
    }
}