using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GeradorDeTestes.WebApp.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        MemberInfo? memberInfo = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
        DisplayAttribute? atributoDisplay = memberInfo?.GetCustomAttribute<DisplayAttribute>();

        return atributoDisplay?.Name ?? enumValue.ToString();
    }
}
