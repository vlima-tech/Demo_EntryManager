using System.Text.RegularExpressions;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Provides a base abstract class for application contracts.
/// </summary>
public abstract class Contract : IContract
{
    #region Properties

    /// <summary>
    /// Gets the unique identifier of the contract.
    /// </summary>
    public Guid ContractId { get; protected set; }

    /// <summary>
    /// Gets the idempotency key used to ensure the operation is not processed more than once.
    /// </summary>
    public string IdempotencyKey { get; protected set; }

    /// <summary>
    /// Gets the correlation identifier used to link the contract to a workflow process.
    /// </summary>
    public string CorrelationId { get; protected set; }

    /// <summary>
    /// Gets the trace identifier used for observability and distributed tracing.
    /// </summary>
    public string TraceId { get; protected set; }
    
    /// <summary>
    /// Gets the name of the contract.
    /// </summary>
    public string ContractName { get; protected set; }

    /// <summary>
    /// Gets the contract type, which can be <see cref="ContractType.Command"/> or <see cref="ContractType.Event"/>.
    /// </summary>
    public ContractType ContractType { get; protected set; }
    
    /// <summary>
    /// Gets the UTC date and time when the contract was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }
    
    private static readonly string _contractNameRemovePrefixPattern = string.Join("|", Enum.GetNames(typeof(ContractType)));

    private static readonly Regex _trimContractName = new(@"^_+|_+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private static readonly Regex _cleanContractName = new ($"{string.Join("|", Enum.GetNames(typeof(ContractType)))}", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    #endregion
    
    #region Constructors

    /// <summary>
    /// Creates a new contract instance. It represents a <see cref="Command"/> or an <see cref="Event"/>.
    /// </summary>
    protected Contract()
    {
        this.ContractId = Guid.NewGuid();
        this.TraceId = Guid.NewGuid().ToString();
        this.CorrelationId = Guid.NewGuid().ToString();
        this.IdempotencyKey = Guid.NewGuid().ToString();
        this.CreatedAt = DateTime.UtcNow;
        this.ContractName = this.GenerateContractName(this.GetType());
        this.ContractType = this.IdentifyContractType();
    }
    
    /// <summary>
    /// Creates a new contract instance correlated with a workflow process and tracing context.
    /// </summary>
    /// <param name="traceId">The identifier used for observability and tracing.</param>
    protected Contract(string traceId) : this()
    {
        this.TraceId = traceId;
        this.CorrelationId = Guid.NewGuid().ToString();
        this.IdempotencyKey = Guid.NewGuid().ToString();
    }
    
    /// <summary>
    /// Creates a new contract instance correlated with a workflow process and tracing context.
    /// </summary>
    /// <param name="traceId">The identifier used for observability and tracing.</param>
    /// <param name="correlationId">The identifier used to correlate the contract with a workflow process.</param>
    protected Contract(string traceId, string correlationId) : this()
    {
        this.TraceId = traceId;
        this.CorrelationId = correlationId;
    }
    
    /// <summary>
    /// Creates a new contract instance correlated with a workflow process and tracing context.
    /// </summary>
    /// <param name="traceId">The identifier used for observability and tracing.</param>
    /// <param name="correlationId">The identifier used to correlate the contract with a workflow process.</param>
    /// <param name="idempotencyKey">The unique key that ensures the contract is not processed more than once.</param> 
    protected Contract(string traceId, string correlationId, string idempotencyKey) : this()
    {
        this.TraceId = traceId;
        this.CorrelationId = correlationId;
        this.IdempotencyKey = idempotencyKey;
    }
    
    #endregion

    protected virtual string GenerateContractName(Type contractType)
    {
        var contractName = contractType.Name.Aggregate("", (acc, c) 
                => $"{acc}{(char.IsUpper(c) && acc.Length > 0 ? "_" : "")}{c}").ToUpper();

        contractName = _cleanContractName.Replace(contractName, string.Empty);
        contractName = _trimContractName.Replace(contractName, string.Empty);

        return contractName;
    }

    private ContractType IdentifyContractType()
        => this.GetType().IsAssignableTo(typeof(IEvent)) ? ContractType.Event : ContractType.Command;
    
    /// <summary>
    /// Correlates the contract to a specified workflow process identifier.
    /// </summary>
    /// <param name="correlationId">The identifier used to correlate the contract.</param>
    public virtual void CorrelateToId(string correlationId)
        => this.CorrelationId = correlationId != default ? correlationId : this.CorrelationId;
    
    /// <summary>
    /// Correlates the contract to a specified trace identifier used for observability and troubleshooting.
    /// </summary>
    /// <param name="traceId">The identifier used for observability and tracing.</param>
    public virtual void CorrelateToTrace(string traceId) => this.TraceId ??= traceId;

    public bool IsCorrelated()
        => string.IsNullOrEmpty(this.TraceId) || string.IsNullOrEmpty(this.CorrelationId) || string.IsNullOrEmpty(this.IdempotencyKey);
    
    public bool IsNotCorrelated() => !this.IsCorrelated();
    
    public void CorrelateTo(IContract contract)
    {
        this.TraceId = contract.TraceId;
        this.CorrelationId = contract.CorrelationId;
        this.IdempotencyKey = contract.IdempotencyKey;
    }

    public void CorrelateTo(string traceId)
        => this.CorrelateTo(traceId, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

    public void CorrelateTo(string traceId, string correlationId)
        => this.CorrelateTo(traceId, correlationId, correlationId);

    public void CorrelateTo(string traceId, string correlationId, string idempotencyKey)
    {
        if(string.IsNullOrEmpty(traceId.Trim()) || string.IsNullOrEmpty(correlationId.Trim()) || string.IsNullOrEmpty(idempotencyKey.Trim()))
            return;
            
        this.TraceId = traceId;
        this.CorrelationId = correlationId;
        this.IdempotencyKey = idempotencyKey;
    }
}