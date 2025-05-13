using Azure.AI.DocumentIntelligence;
using Azure;
using BlazorDocumentIntelligence.Models;

namespace BlazorDocumentIntelligence.Services
{
    public class AzureDocumentIntelligenceService : IAzureDocumentIntelligenceService
    {
        private readonly DocumentIntelligenceClient _documentIntelligenceClient;
        public AzureDocumentIntelligenceService(IConfiguration configuration)
        {
            string endpoint = configuration["AzureDocumentIntelligence:Endpoint"]
            ?? throw new InvalidOperationException("Document Intelligence endpoint not configured");
            string key = configuration["AzureDocumentIntelligence:Key"]
                ?? throw new InvalidOperationException("Document Intelligence key not configured");

            AzureKeyCredential credential = new(key);
            _documentIntelligenceClient = new DocumentIntelligenceClient(new Uri(endpoint), credential);
        }

        public async Task<InvoiceAnalysis> AnalyzeAsync()
        {
            Uri invoiceUri = new Uri("{YOUR-FILE-URL}");
            Operation<AnalyzeResult> operation = await _documentIntelligenceClient.AnalyzeDocumentAsync(
                WaitUntil.Completed, "prebuilt-invoice", invoiceUri);

            AnalyzeResult result = operation.Value;
            var invoice = new InvoiceAnalysis();

            if (result.Documents.Count > 0)
            {
                AnalyzedDocument document = result.Documents[0];

                if (document.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField)
                    && vendorNameField.FieldType == DocumentFieldType.String)
                {
                    invoice.VendorName = vendorNameField.ValueString;
                }

                if (document.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField)
                    && customerNameField.FieldType == DocumentFieldType.String)
                {
                    invoice.CustomerName = customerNameField.ValueString;
                }

                if (document.Fields.TryGetValue("Items", out DocumentField? itemsField)
                    && itemsField.FieldType == DocumentFieldType.List)
                {
                    foreach (DocumentField itemField in itemsField.ValueList)
                    {
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            var item = new InvoiceItem();
                            var itemFields = itemField.ValueDictionary;

                            if (itemFields.TryGetValue("Description", out DocumentField? descField)
                                && descField.FieldType == DocumentFieldType.String)
                            {
                                item.Description = descField.ValueString;
                                item.Confidence = descField.Confidence;
                            }

                            if (itemFields.TryGetValue("Amount", out DocumentField? amountField)
                                && amountField.FieldType == DocumentFieldType.Currency)
                            {
                                item.Amount = new CurrencyAmount
                                {
                                    Amount = amountField.ValueCurrency.Amount,
                                    CurrencySymbol = amountField.ValueCurrency.CurrencySymbol,
                                    Confidence = amountField.Confidence
                                };
                            }

                            // Add quantity processing
                            if (itemFields.TryGetValue("Quantity", out DocumentField? quantityField)
                                && quantityField.FieldType == DocumentFieldType.Double)
                            {
                                item.Quantity = quantityField.ValueDouble;
                                item.QuantityConfidence = quantityField.Confidence;

                            }

                            invoice.Items.Add(item);
                        }
                    }
                }

                if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField)
                    && subTotalField.FieldType == DocumentFieldType.Currency)
                {
                    invoice.SubTotal = new CurrencyAmount
                    {
                        Amount = subTotalField.ValueCurrency.Amount,
                        CurrencySymbol = subTotalField.ValueCurrency.CurrencySymbol,
                        Confidence = subTotalField.Confidence
                    };
                }

                if (document.Fields.TryGetValue("TotalTax", out DocumentField? totalTaxField)
                    && totalTaxField.FieldType == DocumentFieldType.Currency)
                {
                    invoice.TotalTax = new CurrencyAmount
                    {
                        Amount = totalTaxField.ValueCurrency.Amount,
                        CurrencySymbol = totalTaxField.ValueCurrency.CurrencySymbol,
                        Confidence = totalTaxField.Confidence
                    };
                }

                if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField)
                    && invoiceTotalField.FieldType == DocumentFieldType.Currency)
                {
                    invoice.InvoiceTotal = new CurrencyAmount
                    {
                        Amount = invoiceTotalField.ValueCurrency.Amount,
                        CurrencySymbol = invoiceTotalField.ValueCurrency.CurrencySymbol,
                        Confidence = invoiceTotalField.Confidence
                    };
                }
            }

            return invoice;
        }
    }
}
