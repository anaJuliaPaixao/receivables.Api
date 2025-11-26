namespace receivables.Infrastructure.Entities;

public class Invoice
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CompanyId { get; private set; }
    public string Number { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }
    public bool InCart { get; private set; }

    private Invoice() { }

    public Invoice(Guid companyId, string number, decimal amount, DateTime dueDate)
    {
        CompanyId = companyId;
        Number = number;
        Amount = amount;
        DueDate = dueDate;
        InCart = false;
    }

    public void Update(string number, decimal amount, DateTime dueDate)
    {
        Number = number;
        Amount = amount;
        DueDate = dueDate;
    }

    public void AddToCart() => InCart = true;
    public void RemoveFromCart() => InCart = false;
}
