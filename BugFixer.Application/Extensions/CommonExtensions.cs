using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BugFixer.Application.Extensions;

public static class CommonExtensions
{
    public static string GetEnumDisplayName(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()!
            .Name!;
    }
}