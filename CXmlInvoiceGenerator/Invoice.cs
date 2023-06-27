using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CXmlInvoiceGenerator
{
    public class Invoice
    {
        // Header
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SalesOrderId { get; set; }
        public string CurrencyCode { get; set; } = "";
        public decimal NetAmount { get; set; } = 0.00m;
        public decimal VATAmount { get; set; } = 0.00m;
        public decimal GrossAmount { get; set; } = 0.00m;
        public string VATCode { get; set; } = "";
        public decimal VATPercentage { get; set; } = 0.00m;
        public int PaymentTermsDays { get; set; } = 30;

        // Delivery
        public string DeliveryContactName { get; set; } = "";
        public string DeliveryAddrLine1 { get; set; } = "";
        public string DeliveryAddrLine2 { get; set; } = "";
        public string DeliveryAddrLine3 { get; set; } = "";
        public string DeliveryAddrLine4 { get; set; } = "";
        public string DeliveryAddrLine5 { get; set; } = "";
        public string DeliveryAddrPostCode { get; set; } = "";
        public string DeliveryCountryCode { get; set; } = "";
        public string DeliveryCountry { get; set; } = "";

        // Billing
        public string BillingContactName { get; set; } = "";
        public string BillingAddrLine1 { get; set; } = "";
        public string BillingAddrLine2 { get; set; } = "";
        public string BillingAddrLine3 { get; set; } = "";
        public string BillingAddrLine4 { get; set; } = "";
        public string BillingAddrLine5 { get; set; } = "";
        public string BillingAddrPostCode { get; set; } = "";
        public string BillingCountryCode { get; set; } = "";
        public string BillingCountry { get; set; } = "";

        // Rows
        public List<InvoiceRow> Rows { get; set; } = new List<InvoiceRow>();
    }

    public class InvoiceRow
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int StockItemId { get; set; }
        public string Manufacturer { get; set; } = "";
        public string PartNo { get; set; } = "";
        public string Description { get; set; } = "";
        public int Qty { get; set; } = 1;
        public decimal UnitPrice { get; set; } = 0.00m;
        public decimal LineTotal { get; set; } = 0.00m;
    }

}
