using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess;

namespace CXmlInvoiceGenerator
{
   public class InvoiceReader
    {
        private Logger logger = new(true, AppSettings.logFilePath);
        public List<Invoice> InvoiceList { get; } = new List<Invoice>();
        public void ReadInvoices()
        {
            try
            {

                InvoiceList.Clear();
                Invoices invoiceDB = new();
                DataTable newInvoices = invoiceDB.GetNewInvoices();
                foreach (DataRow row in newInvoices.Rows)
                {
                    logger.LogInfo("Reading Invoice: " + row["Id"].ToString());
                    Invoice invoiceItem = new Invoice();
                    invoiceItem.Id = Int32.Parse(row["Id"].ToString());
                    invoiceItem.CustomerId = Int32.Parse(row["CustomerId"].ToString());
                    invoiceItem.SalesOrderId = Int32.Parse(row["SalesOrderId"].ToString());
                    invoiceItem.CurrencyCode = row["CurrencyCode"].ToString();
                    invoiceItem.NetAmount = Decimal.Parse(row["NetAmount"].ToString());
                    invoiceItem.VATAmount = Decimal.Parse(row["VATAmount"].ToString());
                    invoiceItem.GrossAmount = Decimal.Parse(row["GrossAmount"].ToString());
                    invoiceItem.VATCode = row["VATCode"].ToString();
                    invoiceItem.VATPercentage = Decimal.Parse(row["VATPercentage"].ToString());
                    invoiceItem.PaymentTermsDays = Int32.Parse(row["PaymentTermsDays"].ToString());

                    DataRow deliveryRow = invoiceDB.GetDeliveryAddressForSalesOrder(invoiceItem.SalesOrderId);
                    if (deliveryRow != null)
                    {
                        invoiceItem.DeliveryContactName = deliveryRow["ContactName"].ToString();
                        invoiceItem.DeliveryAddrLine1 = deliveryRow["AddrLine1"].ToString();
                        invoiceItem.DeliveryAddrLine2 = deliveryRow["AddrLine2"].ToString();
                        invoiceItem.DeliveryAddrLine3 = deliveryRow["AddrLine3"].ToString();
                        invoiceItem.DeliveryAddrLine4 = deliveryRow["AddrLine4"].ToString();
                        invoiceItem.DeliveryAddrLine5 = deliveryRow["AddrLine5"].ToString();
                        invoiceItem.DeliveryAddrPostCode = deliveryRow["AddrPostCode"].ToString();
                        invoiceItem.DeliveryCountryCode = deliveryRow["CountryCode"].ToString();
                        invoiceItem.DeliveryCountry = deliveryRow["Country"].ToString();

                    }
                    DataRow billingRow = invoiceDB.GetBillingAddressForInvoice(invoiceItem.Id);
                    if (billingRow != null)
                    {
                        invoiceItem.BillingContactName = billingRow["ContactName"].ToString();
                        invoiceItem.BillingAddrLine1 = billingRow["AddrLine1"].ToString();
                        invoiceItem.BillingAddrLine2 = billingRow["AddrLine2"].ToString();
                        invoiceItem.BillingAddrLine3 = billingRow["AddrLine3"].ToString();
                        invoiceItem.BillingAddrLine4 = billingRow["AddrLine4"].ToString();
                        invoiceItem.BillingAddrLine5 = billingRow["AddrLine5"].ToString();
                        invoiceItem.BillingAddrPostCode = billingRow["AddrPostCode"].ToString();
                        invoiceItem.BillingCountryCode = billingRow["CountryCode"].ToString();
                        invoiceItem.BillingCountry = billingRow["Country"].ToString();
                    }

                    DataTable newInvoiceRows = invoiceDB.GetItemsOnInvoice(invoiceItem.Id);

                    foreach (DataRow itemRow in newInvoiceRows.Rows)
                    {
                        InvoiceRow invoiceRow = new InvoiceRow();
                        invoiceRow.Id = Int32.Parse(itemRow["Id"].ToString());
                        invoiceRow.InvoiceId = Int32.Parse(itemRow["InvoiceId"].ToString());
                        invoiceRow.StockItemId = Int32.Parse(itemRow["StockItemId"].ToString());
                        invoiceRow.Manufacturer = itemRow["Manufacturer"].ToString();
                        invoiceRow.PartNo = itemRow["PartNo"].ToString();
                        invoiceRow.Description = itemRow["Description"].ToString();
                        invoiceRow.Qty = Int32.Parse(itemRow["Qty"].ToString());
                        invoiceRow.UnitPrice = Decimal.Parse(itemRow["UnitPrice"].ToString());
                        invoiceRow.LineTotal = Decimal.Parse(itemRow["LineTotal"].ToString());
                        invoiceItem.Rows.Add(invoiceRow);
                    }
                    InvoiceList.Add(invoiceItem);
                }
            }
            catch(Exception ex) 
            {
                logger.LogError("InvoiceReader.ReadInvoices Failed because " + ex.Message);
            }
            
            }
         
    }
}
