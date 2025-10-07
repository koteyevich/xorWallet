using MongoDB.Bson;
using xorWallet.Models;

namespace xorWallet.Services.Interfaces;

public interface IInvoiceService
{
    IEnumerable<InvoiceModel> GetAll();

    InvoiceModel? Get(ObjectId id);
    InvoiceModel? Get(string id);
    List<InvoiceModel> GetAllOfUser(UserModel user);

    Task Add(InvoiceModel invoice);
    Task Edit(InvoiceModel invoice);
    Task Delete(InvoiceModel invoice);
}