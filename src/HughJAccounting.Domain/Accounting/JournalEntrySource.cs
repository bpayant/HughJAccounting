namespace HughJAccounting.Domain.Accounting;

public enum JournalEntrySource
{
    Manual = 1,
    OpeningBalance = 2,
    Bill = 3,
    BillPayment = 4,
    Invoice = 5,
    CustomerPayment = 6,
    BankAdjustment = 7,
    Accrual = 8,
    Reversal = 9
}