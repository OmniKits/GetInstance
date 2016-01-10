using System;
using System.Linq;
using System.Reflection;

partial class TypeUtility
{
    static readonly Type StringType = typeof(string);

    public static object GetInstance(Type sourceType, Type targetType, string memberName, string argument = null)
    {
        if (targetType.IsAssignableFrom(sourceType) && string.IsNullOrWhiteSpace(memberName))
        {
            var candidate = sourceType.GetConstructors()
                .Select(c => new { ctor = c, @params = c.GetParameters() })
                .Where(c =>
                {
                    switch (c.@params.Length)
                    {
                        case 0:
                            return argument == null;
                        case 1:
                            return c.@params[0].ParameterType == StringType;
                    }

                    return false;
                })
                .OrderBy(c => c.@params.Length)
                .First();

            var ctor = candidate.ctor;
            if (candidate.@params.Length == 0)
                return ctor.Invoke(null);
            else
                return ctor.Invoke(new[] { argument });
        }

        var members = sourceType.GetMember(memberName, BindingFlags.Public | BindingFlags.Static);
        if (members.Length == 1)
        {
            var member = members[0];
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(null);
                case MemberTypes.Property:
                    // TODO: fix indexers
                    members[0] = ((PropertyInfo)member).GetGetMethod();
                    break;
            }
        }
        // whatever
        {
            var candidate = members
                .Cast<MethodInfo>()
                .Select(m => new { method = m, @params = m.GetParameters() })
                .Where(c =>
                {
                    if (!targetType.IsAssignableFrom(c.method.ReturnType))
                        return false;

                    switch (c.@params.Length)
                    {
                        case 0:
                            return argument == null;
                        case 1:
                            return c.@params[0].ParameterType == StringType;
                    }

                    return false;
                })
                .OrderBy(c => c.@params.Length)
                .First();

            var method = candidate.method;
            if (candidate.@params.Length == 0)
                return method.Invoke(null, null);
            else
                return method.Invoke(null, new[] { argument });
        }
    }
    public static T GetInstance<T>(Type sourceType, string memberName, string argument = null)
        => (T)GetInstance(sourceType, typeof(T), memberName, argument);
}

