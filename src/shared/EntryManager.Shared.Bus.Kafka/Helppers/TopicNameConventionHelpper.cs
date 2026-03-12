using System.Text.RegularExpressions;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Shared.Bus.Kafka.Helppers;

internal static partial class TopicNameConventionHelpper
{
    public static TopicNameResolution Resolve(string applicationName, Type messageType)
    {
        var originService = ObtainsServiceName(messageType) ?? "unknown";
        var contract = ObtainsContractName(messageType).ToKebabCase();
        var service = ObtainsServiceName(messageType) ?? applicationName;
        var topic = $"{service}-{contract}".ToLower();
        
        var resolution = new TopicNameResolution
        {
            OriginService = originService,
            ContractName = contract,
            TopicName = topic,
            ContractType = typeof(IEvent).IsAssignableFrom(messageType)
                ? ContractType.Event
                : ContractType.Command
        };
        
        return resolution;
    }

    private static string? ObtainsServiceName(Type messageType)
    {
        var fullNamespace = messageType.Namespace!;
        const string searchPattern = ".Contracts";

        if (!fullNamespace.Contains(searchPattern)) return null;
        
        var index = fullNamespace.IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase);
        
        var serviceName = fullNamespace[..index]
            .Replace(".", "-", StringComparison.Ordinal)
            .ToLowerInvariant();
        
        return serviceName;
    }
    
    private static string ObtainsContractName(Type messageType)
    {
        var contractName = messageType.Name;
        
        var contractType = typeof(IEvent).IsAssignableFrom(messageType)
            ? ContractType.Event
            : ContractType.Command;
        
        return contractName.EndsWith(contractType.ToString(), StringComparison.InvariantCultureIgnoreCase)
            ? contractName[..^contractType.ToString().Length]
            : contractName;
    }

    private static string ToKebabCase(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        return KebabCaseRegex()
            .Replace(text, "-$1")
            .ToLower();
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex KebabCaseRegex();
}

public struct TopicNameResolution
{
    public string OriginService { get; set; }
    
    public string ContractName { get; set; }
    
    public string TopicName { get; init; }

    public ContractType ContractType { get; set; }
}