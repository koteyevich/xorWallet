using MongoDB.Bson;
using xorWallet.Models;

namespace xorWallet.Services.Interfaces;

public interface IInvoiceService
{
    IEnumerable<InvoiceModel> GetAllInvoices();

    InvoiceModel? GetInvoiceById(ObjectId id);
    InvoiceModel? GetInvoiceById(string id);
    List<InvoiceModel> GetInvoicesOfUser(UserModel user);

    Task AddInvoice(InvoiceModel invoice);
    Task EditInvoice(InvoiceModel invoice);
    Task DeleteInvoice(InvoiceModel invoice);
}