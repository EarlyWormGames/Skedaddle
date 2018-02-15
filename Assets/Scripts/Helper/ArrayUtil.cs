using System;
using System.Reflection;
using System.Collections.Generic;

//Extension methods must be defined in a static class
public static class ArrayExtension
{
    public static T[] Add<T>(this T[] a_array, T a_object)
    {
        T[] arr = new T[a_array.Length + 1];

        for (int i = 0; i < a_array.Length; ++i)
        {
            arr[i] = a_array[i];
        }
        arr[a_array.Length] = a_object;

        return arr;
    }

    public static T[] Remove<T>(this T[] a_array, T a_object) where T : IComparable<T>
    {
        T[] arr = new T[a_array.Length - 1];

        bool changed = false;
        for (int i = 0; i < a_array.Length; ++i)
        {
            if (!changed)
            {
                if (a_array[i].CompareTo(a_object) != 0)
                {
                    arr[i] = a_array[i];
                }
                else
                {
                    changed = true;
                }
            }
            else
            {
                arr[i - 1] = a_array[i];
            }
        }

        return arr;
    }

    public static T[] RemoveAt<T>(this T[] a_array, int a_index)
    {
        T[] arr = new T[a_array.Length - 1];

        bool changed = false;
        for (int i = 0; i < a_array.Length; ++i)
        {
            if (!changed)
            {
                if (i != a_index)
                {
                    arr[i] = a_array[i];
                }
                else
                {
                    changed = true;
                }
            }
            else
            {
                arr[i - 1] = a_array[i];
            }
        }

        return arr;
    }

    public static int IndexOf<T>(this T[] a_array, T a_object) where T : IComparable<T>
    {
        for (int i = 0; i < a_array.Length; ++i)
        {
            if (a_array[i].CompareTo(a_object) == 0)
            {
                return i;
            }
        }
        return -1;
    }

    public static Type[] GetTypes(this Array a_array)
    {
        Type[] types = new Type[a_array.Length];
        for (int i = 0; i < a_array.Length; ++i)
        {
            types[i] = a_array.GetValue(i).GetType();
        }
        return types;
    }

    public static bool Contains(this List<string> a_array, string a_string, StringComparison a_comp)
    {
        if (a_array == null)
            return false;
        if (a_array.Count <= 0)
            return false;

        foreach (string word in a_array)
        {
            if (word.IndexOf(a_string, a_comp) >= 0)
                return true;
        }
        return false;
    }

    public static string[] ToStringArray(this Array a_array)
    {
        string[] vals = new string[a_array.Length];
        for (int i = 0; i < a_array.Length; ++i)
        {
            vals[i] = a_array.GetValue(i).ToString();
        }        
        return vals;
    }

    public static string[] MemberToStringArray(this MemberInfo[] a_array)
    {
        List<string> vals = new List<string>();
        for (int i = 0; i < a_array.Length; ++i)
        {
            if (a_array[i].UnderLyingType() != null)
                vals.Add(a_array[i].Name);
        }
        return vals.ToArray();
    }

    public static MemberInfo[] RemoveMethods(this MemberInfo[] a_array)
    {
        List<MemberInfo> vals = new List<MemberInfo>();
        for (int i = 0; i < a_array.Length; ++i)
        {
            if (a_array[i].UnderLyingType() != null)
                vals.Add(a_array[i]);
        }
        return vals.ToArray();
    }
}

public static class ReflectionUtils
{

    /// <summary>
    /// Gets the member's underlying type.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>The underlying type of the member.</returns>
    public static Type UnderLyingType(this MemberInfo member)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)member).FieldType;
            case MemberTypes.Property:
                return ((PropertyInfo)member).PropertyType;
            case MemberTypes.Event:
                return ((EventInfo)member).EventHandlerType;
            default:
                return null;
        }
    }

    public static Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains("."))
        {

            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;

        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {

            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;

    }
}
