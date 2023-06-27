using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CXmlInvoiceGenerator
{
    public class ExporttoCXml
    {
        public XmlDocument xmlDoc = new XmlDocument();
        private XmlElement rootElement;
        private Logger logger = new(true, AppSettings.logFilePath);
        private void createCxmlDoc()
            {
            try
            {
                // Create the root element and add it to the document
                rootElement = xmlDoc.CreateElement("cXML");

                rootElement.SetAttribute("version", "1.0");
                rootElement.SetAttribute("payloadID", AppSettings.payLoadID);
                rootElement.SetAttribute("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-00:00"));
                xmlDoc.AppendChild(rootElement);

                // Create the Header element
                XmlElement headerElement = xmlDoc.CreateElement("Header");
                rootElement.AppendChild(headerElement);

                // Create the From element
                XmlElement fromElement = xmlDoc.CreateElement("From");
                headerElement.AppendChild(fromElement);

                // Create the Credential element for From
                XmlElement fromCredentialElement = xmlDoc.CreateElement("Credential");
                fromCredentialElement.SetAttribute("domain", "DUNS");
                fromElement.AppendChild(fromCredentialElement);

                // Create the Identity element for From Credential
                XmlElement fromIdentityElement = xmlDoc.CreateElement("Identity");
                fromIdentityElement.InnerText = AppSettings.fromDomain;
                fromCredentialElement.AppendChild(fromIdentityElement);

                // Create the To element
                XmlElement toElement = xmlDoc.CreateElement("To");
                headerElement.AppendChild(toElement);

                // Create the Credential element for To
                XmlElement toCredentialElement = xmlDoc.CreateElement("Credential");
                toCredentialElement.SetAttribute("domain", "NetworkID");
                toElement.AppendChild(toCredentialElement);

                // Create the Identity element for To Credential
                XmlElement toIdentityElement = xmlDoc.CreateElement("Identity");
                toIdentityElement.InnerText = AppSettings.toDomain;
                toCredentialElement.AppendChild(toIdentityElement);

                // Create the Sender element
                XmlElement senderElement = xmlDoc.CreateElement("Sender");
                headerElement.AppendChild(senderElement);

                // Create the Credential element for Sender
                XmlElement senderCredentialElement = xmlDoc.CreateElement("Credential");
                senderCredentialElement.SetAttribute("domain", AppSettings.senderDomain);
                senderElement.AppendChild(senderCredentialElement);

                // Create the Identity element for Sender Credential
                XmlElement senderIdentityElement = xmlDoc.CreateElement("Identity");
                senderIdentityElement.InnerText = AppSettings.senderIdentity;
                senderCredentialElement.AppendChild(senderIdentityElement);

                // Create the SharedSecret element for Sender Credential
                XmlElement sharedSecretElement = xmlDoc.CreateElement("SharedSecret");
                sharedSecretElement.InnerText = AppSettings.senderIdentitySharedSecret;
                senderCredentialElement.AppendChild(sharedSecretElement);

                // Create the UserAgent element
                XmlElement userAgentElement = xmlDoc.CreateElement("UserAgent");
                userAgentElement.InnerText = "Coupa Procurement 1.0";
                senderElement.AppendChild(userAgentElement);
            }
            catch(Exception ex) 
            {
                logger.LogError("ExporttoCXml.createCxmlDoc Failed because " + ex.Message);
               
            }
        }

        private void addInvoices(List<Invoice> InvoiceList)
        {
            int addressID = 0;
            try
            {

                foreach (Invoice invoice in InvoiceList)
                {
                    if (invoice != null)
                    {
                        logger.LogInfo("Converting Invoice" + invoice.Id.ToString());
                        // Create the Request element
                        XmlElement requestElement = xmlDoc.CreateElement("Request");
                        requestElement.SetAttribute("deploymentMode", "production");
                        rootElement.AppendChild(requestElement);

                        // Create the InvoiceDetailRequest element
                        XmlElement invoiceDetailRequestElement = xmlDoc.CreateElement("InvoiceDetailRequest");
                        requestElement.AppendChild(invoiceDetailRequestElement);

                        // Create the InvoiceDetailRequestHeader element
                        XmlElement invoiceDetailRequestHeaderElement = xmlDoc.CreateElement("InvoiceDetailRequestHeader");
                        invoiceDetailRequestHeaderElement.SetAttribute("invoiceID", invoice.Id.ToString());
                        invoiceDetailRequestHeaderElement.SetAttribute("purpose", "standard");
                        invoiceDetailRequestHeaderElement.SetAttribute("operation", "new");
                        invoiceDetailRequestHeaderElement.SetAttribute("invoiceDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-00:00"));
                        invoiceDetailRequestElement.AppendChild(invoiceDetailRequestHeaderElement);

                        // Create the InvoiceDetailHeaderIndicator element
                        XmlElement invoiceDetailHeaderIndicatorElement = xmlDoc.CreateElement("InvoiceDetailHeaderIndicator");
                        invoiceDetailRequestHeaderElement.AppendChild(invoiceDetailHeaderIndicatorElement);


                        // Create the InvoiceDetailLineIndicator element
                        XmlElement invoiceDetailLineIndicatorElement = xmlDoc.CreateElement("InvoiceDetailLineIndicator");
                        invoiceDetailLineIndicatorElement.SetAttribute("isAccountingInLine", "yes");
                        invoiceDetailRequestHeaderElement.AppendChild(invoiceDetailLineIndicatorElement);

                        // Create the InvoicePartner element for soldTo
                        XmlElement soldToElement = xmlDoc.CreateElement("InvoicePartner");
                        invoiceDetailRequestHeaderElement.AppendChild(soldToElement);

                        // Create the Contact element for soldTo
                        XmlElement soldToContactElement = xmlDoc.CreateElement("Contact");
                        soldToContactElement.SetAttribute("role", "soldTo");
                        soldToElement.AppendChild(soldToContactElement);

                        // Create the Name element for soldTo Contact
                        XmlElement soldToNameElement = xmlDoc.CreateElement("Name");
                        soldToNameElement.SetAttribute("xml:lang", "en-US");
                        soldToNameElement.InnerText = "COUPA";
                        soldToContactElement.AppendChild(soldToNameElement);

                        // Create the PostalAddress element for soldTo Contact
                        XmlElement soldToPostalAddressElement = xmlDoc.CreateElement("PostalAddress");
                        soldToContactElement.AppendChild(soldToPostalAddressElement);

                        // Set the soldTo Delivery details
                        soldToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "Street", ""));
                        soldToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "Street", invoice.DeliveryAddrLine1));
                        soldToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "City", invoice.DeliveryAddrLine2));
                        soldToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "State", invoice.DeliveryAddrLine3));
                        soldToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "PostalCode", invoice.DeliveryAddrLine4));
                        soldToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "Country", invoice.DeliveryAddrLine5, "isoCountryCode", invoice.DeliveryCountryCode));

                        // Create the InvoicePartner element for billTo
                        XmlElement billToElement = xmlDoc.CreateElement("InvoicePartner");
                        invoiceDetailRequestHeaderElement.AppendChild(billToElement);

                        addressID += 1; //I'm assuming addess id corresponds to an a possible internal database of addresses, which we don't have, some I'm creating one
                        // Create the Contact element for billTo
                        XmlElement billToContactElement = xmlDoc.CreateElement("Contact");
                        billToContactElement.SetAttribute("role", "billTo");
                        billToContactElement.SetAttribute("addressID", addressID.ToString());
                        billToElement.AppendChild(billToContactElement);

                        // Create the Name element for billTo Contact
                        XmlElement billToNameElement = xmlDoc.CreateElement("Name");
                        billToNameElement.SetAttribute("xml:lang", "en-US");
                        billToNameElement.InnerText = "COUPA";
                        billToContactElement.AppendChild(billToNameElement);

                        // Create the PostalAddress element for billTo Contact
                        XmlElement billToPostalAddressElement = xmlDoc.CreateElement("PostalAddress");
                        billToContactElement.AppendChild(billToPostalAddressElement);

                        // Set the billTo Billing details
                        billToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "Street", invoice.BillingContactName));
                        billToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "Street", invoice.BillingAddrLine1));
                        billToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "City", invoice.BillingAddrLine2));
                        billToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "State", invoice.BillingAddrLine3));
                        billToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "PostalCode", invoice.BillingAddrLine4));
                        billToPostalAddressElement.AppendChild(CreateElementWithValue(xmlDoc, "Country", invoice.BillingAddrLine5, "isoCountryCode", invoice.BillingCountryCode));


                        // Create the PaymentTerm element
                        XmlElement paymentTermElement = xmlDoc.CreateElement("PaymentTerm");
                        paymentTermElement.SetAttribute("payInNumberofDays", invoice.PaymentTermsDays.ToString());
                        invoiceDetailRequestHeaderElement.AppendChild(paymentTermElement);

                        // Create the Discount element within PaymentTerm
                        XmlElement discountElement = xmlDoc.CreateElement("Discount");
                        paymentTermElement.AppendChild(discountElement);

                        // no discount in our structure so I have omitted the details
                        // Create the DiscountPercent element within Discount
                        //XmlElement discountPercentElement = xmlDoc.CreateElement("DiscountPercent");
                        //discountPercentElement.SetAttribute("percent", "2");
                        //discountElement.AppendChild(discountPercentElement);

                        // Create the DiscountDueDays element within Discount
                        //XmlElement discountDueDaysElement = xmlDoc.CreateElement("DiscountDueDays");
                        //discountDueDaysElement.InnerText = "";
                        //discountElement.AppendChild(discountDueDaysElement);

                        // Create the NetDueDays element within PaymentTerm
                        XmlElement netDueDaysElement = xmlDoc.CreateElement("NetDueDays");
                        netDueDaysElement.InnerText = invoice.PaymentTermsDays.ToString();
                        paymentTermElement.AppendChild(netDueDaysElement);

                        // Create the InvoiceDetailOrder element
                        XmlElement invoiceDetailOrderElement = xmlDoc.CreateElement("InvoiceDetailOrder");
                        invoiceDetailRequestElement.AppendChild(invoiceDetailOrderElement);
                        // Create the InvoiceDetailOrderInfo element within InvoiceDetailOrder
                        XmlElement invoiceDetailOrderInfoElement = xmlDoc.CreateElement("InvoiceDetailOrderInfo");
                        invoiceDetailOrderElement.AppendChild(invoiceDetailOrderInfoElement);

                        int invLineNumber = 0;
                        foreach (InvoiceRow invoiceRow in invoice.Rows)
                        {
                            invLineNumber++;



                            // Create the OrderReference element within InvoiceDetailOrderInfo
                            XmlElement orderReferenceElement = xmlDoc.CreateElement("OrderReference");
                            invoiceDetailOrderInfoElement.AppendChild(orderReferenceElement);

                            // Create the DocumentReference element within OrderReference
                            XmlElement documentReferenceElement = xmlDoc.CreateElement("DocumentReference");
                            documentReferenceElement.SetAttribute("payloadID", invoice.SalesOrderId.ToString());
                            orderReferenceElement.AppendChild(documentReferenceElement);

                            // Create the InvoiceDetailItem element within InvoiceDetailOrder
                            XmlElement invoiceDetailItemElement = xmlDoc.CreateElement("InvoiceDetailItem");
                            invoiceDetailItemElement.SetAttribute("invoiceLineNumber", invLineNumber.ToString());
                            invoiceDetailItemElement.SetAttribute("quantity", invoiceRow.Qty.ToString());
                            invoiceDetailOrderElement.AppendChild(invoiceDetailItemElement);

                            // Create the UnitOfMeasure element within InvoiceDetailItem
                            XmlElement unitOfMeasureElement = xmlDoc.CreateElement("UnitOfMeasure");
                            unitOfMeasureElement.InnerText = "EA";
                            invoiceDetailItemElement.AppendChild(unitOfMeasureElement);

                            // Create the UnitPrice element within InvoiceDetailItem
                            XmlElement unitPriceElement = xmlDoc.CreateElement("UnitPrice");
                            invoiceDetailItemElement.AppendChild(unitPriceElement);

                            // Create the Money element within UnitPrice
                            XmlElement moneyElement = xmlDoc.CreateElement("Money");
                            moneyElement.SetAttribute("currency", invoice.CurrencyCode);
                            moneyElement.InnerText = invoiceRow.UnitPrice.ToString();
                            unitPriceElement.AppendChild(moneyElement);

                            //Create the SubTotal Element within InvoiceDetailItem
                            XmlElement subTotalElement = xmlDoc.CreateElement("SubTotal");
                            XmlElement moneyElementSubTotal = xmlDoc.CreateElement("Money");
                            moneyElementSubTotal.SetAttribute("currency", invoice.CurrencyCode);
                            moneyElementSubTotal.InnerText = invoiceRow.LineTotal.ToString();
                            subTotalElement.AppendChild(moneyElementSubTotal);
                            invoiceDetailItemElement.AppendChild(subTotalElement);

                            //Create the GrossAmount Element within InvoiceDetailItem
                            XmlElement grossAmountElement = xmlDoc.CreateElement("GrossAmount");
                            XmlElement moneyElementGrossAmount = xmlDoc.CreateElement("Money");
                            moneyElementGrossAmount.SetAttribute("currency", invoice.CurrencyCode);
                            moneyElementGrossAmount.InnerText = invoiceRow.LineTotal.ToString();
                            grossAmountElement.AppendChild(moneyElementGrossAmount);
                            invoiceDetailItemElement.AppendChild(grossAmountElement);


                            //Create the NetAmount Element within InvoiceDetailItem
                            XmlElement netAmountElement = xmlDoc.CreateElement("NetAmount");
                            XmlElement moneyElementNetAmount = xmlDoc.CreateElement("Money");
                            moneyElementNetAmount.SetAttribute("currency", invoice.CurrencyCode);
                            moneyElementNetAmount.InnerText = invoiceRow.LineTotal.ToString();
                            netAmountElement.AppendChild(moneyElementNetAmount);
                            invoiceDetailItemElement.AppendChild(netAmountElement);

                        }
                        // Create the InvoiceDetailSummary element
                        XmlElement invoiceDetailSummary = xmlDoc.CreateElement("InvoiceDetailSummary");
                        invoiceDetailRequestElement.AppendChild(invoiceDetailSummary);

                        // Create the SubTotal element within the InvoiceDetailSummary
                        XmlElement summarySubTotalElement = xmlDoc.CreateElement("SubTotal");
                        XmlElement summaryMoneyElementSubTotal = xmlDoc.CreateElement("Money");
                        summaryMoneyElementSubTotal.SetAttribute("currency", invoice.CurrencyCode);
                        summaryMoneyElementSubTotal.InnerText = invoice.GrossAmount.ToString();
                        summarySubTotalElement.AppendChild(summaryMoneyElementSubTotal);
                        invoiceDetailSummary.AppendChild(summarySubTotalElement);

                        // Create the Tax element within the InvoiceDetailSummary
                        XmlElement summaryTaxElement = xmlDoc.CreateElement("Tax");
                        XmlElement moneyElementSummarTax = xmlDoc.CreateElement("Money");
                        moneyElementSummarTax.SetAttribute("currency", invoice.CurrencyCode);
                        summaryTaxElement.AppendChild(moneyElementSummarTax);

                        XmlElement summaryTaxDescriptionElement = xmlDoc.CreateElement("Description");
                        summaryTaxDescriptionElement.SetAttribute("xml:lang", "en-US");
                        summaryTaxElement.AppendChild(summaryTaxDescriptionElement);

                        // Create the TaxDetail element within the InvoiceDetailSummary Tax
                        XmlElement taxDetailElement = xmlDoc.CreateElement("TaxDetail");
                        taxDetailElement.SetAttribute("purpose", "tax");
                        taxDetailElement.SetAttribute("category", invoice.VATCode.ToString());
                        taxDetailElement.SetAttribute("percentageRate", invoice.VATPercentage.ToString());

                        // Create the TaxableAmount element within theTaxDetail
                        XmlElement taxableAmountElement = xmlDoc.CreateElement("TaxableAmount");
                        XmlElement moneyElementTaxableAmount = xmlDoc.CreateElement("Money");
                        moneyElementTaxableAmount.SetAttribute("currency", invoice.CurrencyCode);
                        moneyElementTaxableAmount.InnerText = invoice.NetAmount.ToString();
                        taxableAmountElement.AppendChild(moneyElementTaxableAmount);
                        taxDetailElement.AppendChild(taxableAmountElement);

                        // Create the TaxAmount element within theTaxDetail
                        XmlElement taxAmountElement = xmlDoc.CreateElement("TaxableAmount");
                        XmlElement moneyElementTaxAmount = xmlDoc.CreateElement("Money");
                        moneyElementTaxAmount.SetAttribute("currency", invoice.CurrencyCode);
                        moneyElementTaxAmount.InnerText = invoice.VATAmount.ToString();
                        taxAmountElement.AppendChild(moneyElementTaxAmount);
                        taxDetailElement.AppendChild(taxAmountElement);

                        invoiceDetailSummary.AppendChild(taxDetailElement);

                        // Create the ShippingAmount element within the InvoiceDetailSummary
                        XmlElement summaryShippingAmountElement = xmlDoc.CreateElement("SubTotal");
                        XmlElement summaryMoneyElementShippingAmount = xmlDoc.CreateElement("Money");
                        summaryMoneyElementShippingAmount.SetAttribute("currency", invoice.CurrencyCode);
                        summaryMoneyElementShippingAmount.InnerText = "0.00";
                        summaryShippingAmountElement.AppendChild(summaryMoneyElementShippingAmount);
                        invoiceDetailSummary.AppendChild(summaryShippingAmountElement);

                        // Create the GrossAmount element within the InvoiceDetailSummary
                        XmlElement summaryGrossAmountElement = xmlDoc.CreateElement("SubTotal");
                        XmlElement summaryMoneyElementGrossAmount = xmlDoc.CreateElement("Money");
                        summaryMoneyElementGrossAmount.SetAttribute("currency", invoice.CurrencyCode);
                        summaryMoneyElementGrossAmount.InnerText = invoice.GrossAmount.ToString().ToString();
                        summaryGrossAmountElement.AppendChild(summaryMoneyElementGrossAmount);
                        invoiceDetailSummary.AppendChild(summaryGrossAmountElement);

                        // Create the NetAmount element within the InvoiceDetailSummary
                        XmlElement summaryNetAmountElement = xmlDoc.CreateElement("SubTotal");
                        XmlElement summaryMoneyElementNetAmount = xmlDoc.CreateElement("Money");
                        summaryMoneyElementNetAmount.SetAttribute("currency", invoice.CurrencyCode);
                        summaryMoneyElementNetAmount.InnerText = invoice.NetAmount.ToString().ToString();
                        summaryNetAmountElement.AppendChild(summaryMoneyElementNetAmount);
                        invoiceDetailSummary.AppendChild(summaryNetAmountElement);

                        // Create the DueAmount element within the InvoiceDetailSummary
                        XmlElement summaryDueAmountElement = xmlDoc.CreateElement("SubTotal");
                        XmlElement summaryMoneyElementDueAmount = xmlDoc.CreateElement("Money");
                        summaryMoneyElementDueAmount.SetAttribute("currency", invoice.CurrencyCode);
                        summaryMoneyElementDueAmount.InnerText = invoice.GrossAmount.ToString().ToString();
                        summaryDueAmountElement.AppendChild(summaryMoneyElementDueAmount);
                        invoiceDetailSummary.AppendChild(summaryDueAmountElement);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("ExporttoCXml.addInvoices Failed because " + ex.Message);

            }

        }
        public void doExport(List<Invoice> InvoiceList, string fileName)
        {

            
                createCxmlDoc();
                addInvoices(InvoiceList);
                
               
                string xml = xmlDoc.InnerXml;
            try
            {
                System.IO.Directory.CreateDirectory(AppSettings.outputFilePath);

                string filePath = AppSettings.outputFilePath + "\\" + fileName;

                logger.LogInfo("Writing cXml file: " + filePath);
                if (File.Exists(filePath))
                    
                    File.Delete(filePath);
                
                    File.WriteAllText(filePath, xml);
                
            }
            catch (Exception ex)
            {
                logger.LogError("Create File Failed because " + ex.Message);
            }
        }

        private static XmlElement CreateElementWithValue(XmlDocument xmlDoc, string elementName, string value, string attributeName = null, string attributeValue = null)
        {
            XmlElement element = xmlDoc.CreateElement(elementName);
            element.InnerText = value;
            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
            {
                element.SetAttribute(attributeName, attributeValue);
            }
            return element;
        }
    }
}
