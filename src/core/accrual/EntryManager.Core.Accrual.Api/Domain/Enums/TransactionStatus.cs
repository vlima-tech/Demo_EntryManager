namespace EntryManager.Core.Accrual.Api.Domain.Enums;

public enum TransactionStatus
{
    Canceled = 0,
    Created = 1,
    Committed = 2,
    Confirmed = 3,
    RolledBack = 4,
}