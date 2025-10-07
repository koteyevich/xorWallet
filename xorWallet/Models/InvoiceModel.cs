using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using xorWallet.Helpers;

namespace xorWallet.Models;

[Collection("Invoices")]
public class InvoiceModel
{
    public ObjectId Id { get; set; }

    public string InvoiceId { get; set; } = $"invoice_{IdGenerator.Generate()}"; /*
                                                                                    embedding "invoice_" is not really necessary,
                                                                                    but makes it easier to parse in /start
                                                                                */

    public long OwnerUserId { get; set; }
    public long XORs { get; set; }
}