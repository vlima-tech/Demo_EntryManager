namespace EntryManager.Shared.Bus.Abstractions;

public enum ContractType
{
    /// <summary>
    /// Indicates a <see cref="ICommand"/> or <see cref="Command"/> contract type.
    /// </summary>
    Command = 1,
    
    /// <summary>
    /// Indicates an <see cref="IEvent"/> or <see cref="Event"/> contract type.
    /// </summary>
    Event = 2
}