using System.Text.RegularExpressions;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Shared.Bus.Kafka.Helppers;

internal static partial class TopicNameConventionHelpper
{
    public static string FormatTopicName(string applicationName, Type messageType)
    {
        var typeName = messageType.Name;

        typeName = TrimSuffix(typeName, typeof(IEvent).IsAssignableFrom(messageType) 
            ? ContractType.Event 
            : ContractType.Command);
        
        var kebabName = ToKebabCase(typeName);

        return $"{applicationName}-{kebabName}".ToLower();
    }
    
    private static string TrimSuffix(string name, ContractType suffix)
        => name.EndsWith(suffix.ToString(), StringComparison.OrdinalIgnoreCase) ? name[..^suffix.ToString().Length] : name;
    
    private static string ToKebabCase(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        return KebabCaseRegex()
            .Replace(text, "-$1")
            .ToLower();
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex KebabCaseRegex();
}