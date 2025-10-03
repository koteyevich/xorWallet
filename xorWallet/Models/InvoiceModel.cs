using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using xorWallet.Helpers;

namespace xorWallet.Models;

[Collection("Invoices")]
public class InvoiceModel
{
    public ObjectId Id { get; set; }

    public string InvoiceId { get; set; } = $"{IdGenerator.Generate()}";
    public long OwnerUserId { get; set; }
    public long XORs { get; set; }
}