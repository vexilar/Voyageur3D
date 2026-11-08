using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class has helpers for the enum data type
/// </summary>
public static class EnumUtil {

    /// <summary>
    /// This creates a list from an enum
    /// </summary>
    /// <typeparam name="T">the enum</typeparam>
    /// <param name="ignoreEmpty">default true, if true, it removes the enum member of empty for the list</param>
    /// <returns>the enum as a list</returns>
    public static List<T> GetValues<T>(bool ignoreEmpty = true) {
        var list = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        if (ignoreEmpty) {
            return list.FindAll(type => type.ToString() != "Empty");
        }

        return list;
    }
}