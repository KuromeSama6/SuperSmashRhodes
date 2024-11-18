using System;

namespace SuperSmashRhodes.Extensions {

public static class TypeExtensions {
    public static bool IsSubclassOfGeneric(this Type type, Type genericBaseType) {
        if (type == null || genericBaseType == null)
            throw new ArgumentNullException();

        if (!genericBaseType.IsGenericTypeDefinition)
            throw new ArgumentException("The genericBaseType must be a generic type definition.", nameof(genericBaseType));

        while (type != null && type != typeof(object)) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBaseType)
                return true;

            type = type.BaseType; // Move up the inheritance hierarchy
        }

        return false;
    }
}

}
