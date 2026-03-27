using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace InventoryManagement.Services
{
    public static class InvoicePdfGenerator
    {
        private const string CompanyName = "Inventory Management System";
        private const string CompanyAddress = "Business Address, City, State";
        private const string CompanyEmail = "support@inventory.local";
        private const string CompanyPhone = "+91 90000 00000";

        public static byte[] GenerateSaleInvoice(Sale sale)
        {
            var unitPrice = sale.QuantitySold > 0 ? sale.TotalAmount / sale.QuantitySold : 0m;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(32);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Element(c => ComposeHeader(c, "SALES INVOICE", $"SI-{sale.Id:D5}", sale.CreatedAt));

                    page.Content().PaddingVertical(16).Column(col =>
                    {
                        col.Spacing(14);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c => ComposeInfoCard(c, "Billed To", new[]
                            {
                                "Walk-in Customer",
                                "Customer details not captured"
                            }));

                            row.ConstantItem(12);

                            row.RelativeItem().Element(c => ComposeInfoCard(c, "Order Details", new[]
                            {
                                $"Product: {sale.Product?.Name ?? "N/A"}",
                                $"Transaction Date: {sale.CreatedAt:dd MMM yyyy HH:mm}",
                                $"Generated On: {DateTime.Now:dd MMM yyyy HH:mm}"
                            }));
                        });

                        col.Item().Element(c => ComposeItemsTable(
                            c,
                            "Sales Line Items",
                            new[]
                            {
                                ("1", sale.Product?.Name ?? "N/A", sale.QuantitySold.ToString(), $"₹{unitPrice:N2}", $"₹{sale.TotalAmount:N2}")
                            }));

                        col.Item().AlignRight().Width(220).Element(c => ComposeSummary(c,
                            ("Subtotal", sale.TotalAmount),
                            ("Tax", 0m),
                            ("Grand Total", sale.TotalAmount)));

                        col.Item().PaddingTop(8).Text("Thank you for your business.").SemiBold().FontColor(Colors.Grey.Darken2);
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public static byte[] GeneratePurchaseInvoice(Purchase purchase)
        {
            var unitPrice = purchase.Quantity > 0 ? purchase.TotalCost / purchase.Quantity : 0m;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(32);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Element(c => ComposeHeader(c, "PURCHASE INVOICE", $"PI-{purchase.Id:D5}", purchase.CreatedAt));

                    page.Content().PaddingVertical(16).Column(col =>
                    {
                        col.Spacing(14);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c => ComposeInfoCard(c, "Supplier", new[]
                            {
                                purchase.Supplier?.Name ?? "N/A",
                                purchase.Supplier?.Email ?? "",
                                purchase.Supplier?.Phone ?? ""
                            }));

                            row.ConstantItem(12);

                            row.RelativeItem().Element(c => ComposeInfoCard(c, "Purchase Details", new[]
                            {
                                $"Product: {purchase.Product?.Name ?? "N/A"}",
                                $"Purchase Date: {purchase.CreatedAt:dd MMM yyyy HH:mm}",
                                $"Generated On: {DateTime.Now:dd MMM yyyy HH:mm}"
                            }));
                        });

                        col.Item().Element(c => ComposeItemsTable(
                            c,
                            "Purchase Line Items",
                            new[]
                            {
                                ("1", purchase.Product?.Name ?? "N/A", purchase.Quantity.ToString(), $"₹{unitPrice:N2}", $"₹{purchase.TotalCost:N2}")
                            }));

                        col.Item().AlignRight().Width(220).Element(c => ComposeSummary(c,
                            ("Subtotal", purchase.TotalCost),
                            ("Tax", 0m),
                            ("Grand Total", purchase.TotalCost)));

                        col.Item().PaddingTop(8).Text("This is a system-generated purchase invoice.").SemiBold().FontColor(Colors.Grey.Darken2);
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, string title, string invoiceNo, DateTime invoiceDate)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.ConstantItem(64).Height(64).Element(ComposeLogo);

                    row.ConstantItem(12);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(CompanyName).Bold().FontSize(18).FontColor(Colors.Red.Darken2);
                        c.Item().Text(CompanyAddress);
                        c.Item().Text($"Email: {CompanyEmail} | Phone: {CompanyPhone}");
                    });

                    row.ConstantItem(190).AlignRight().Column(c =>
                    {
                        c.Item().Background(Colors.Grey.Lighten4).Padding(8).Border(1).BorderColor(Colors.Grey.Lighten2).Column(meta =>
                        {
                            meta.Item().Text(title).Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
                            meta.Item().Text($"Invoice No: {invoiceNo}");
                            meta.Item().Text($"Invoice Date: {invoiceDate:dd MMM yyyy}");
                            meta.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy}");
                        });
                    });
                });

                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        }

        private static void ComposeLogo(IContainer container)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");

            if (File.Exists(logoPath))
            {
                var imageData = File.ReadAllBytes(logoPath);
                container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().AlignMiddle().Image(imageData);
                return;
            }

            container
                .Background(Colors.Red.Darken2)
                .Border(1)
                .BorderColor(Colors.Red.Darken3)
                .AlignCenter()
                .AlignMiddle()
                .Text("IMS")
                .Bold()
                .FontSize(20)
                .FontColor(Colors.White);
        }

        private static void ComposeInfoCard(IContainer container, string heading, IEnumerable<string> lines)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
            {
                col.Spacing(3);
                col.Item().Text(heading).Bold().FontColor(Colors.Blue.Darken2);

                foreach (var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
                    col.Item().Text(line);
            });
        }

        private static void ComposeItemsTable(IContainer container, string heading, IEnumerable<(string Sl, string Item, string Qty, string UnitPrice, string LineTotal)> items)
        {
            container.Column(col =>
            {
                col.Item().Text(heading).Bold().FontSize(11).FontColor(Colors.Grey.Darken3);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(35);
                        columns.RelativeColumn(3);
                        columns.ConstantColumn(70);
                        columns.ConstantColumn(95);
                        columns.ConstantColumn(100);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellHeaderStyle).Text("#");
                        header.Cell().Element(CellHeaderStyle).Text("Description");
                        header.Cell().Element(CellHeaderStyle).AlignRight().Text("Qty");
                        header.Cell().Element(CellHeaderStyle).AlignRight().Text("Unit Price");
                        header.Cell().Element(CellHeaderStyle).AlignRight().Text("Amount");
                    });

                    foreach (var item in items)
                    {
                        table.Cell().Element(CellBodyStyle).Text(item.Sl);
                        table.Cell().Element(CellBodyStyle).Text(item.Item);
                        table.Cell().Element(CellBodyStyle).AlignRight().Text(item.Qty);
                        table.Cell().Element(CellBodyStyle).AlignRight().Text(item.UnitPrice);
                        table.Cell().Element(CellBodyStyle).AlignRight().Text(item.LineTotal);
                    }
                });
            });
        }

        private static void ComposeSummary(IContainer container, params (string Label, decimal Value)[] rows)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(col =>
            {
                col.Spacing(4);

                for (var i = 0; i < rows.Length; i++)
                {
                    var row = rows[i];
                    var isGrand = i == rows.Length - 1;

                    col.Item().Row(r =>
                    {
                        if (isGrand)
                            r.RelativeItem().Text(row.Label).SemiBold();
                        else
                            r.RelativeItem().Text(row.Label);

                        if (isGrand)
                            r.ConstantItem(90).AlignRight().Text($"₹{row.Value:N2}").SemiBold().FontColor(Colors.Red.Darken2);
                        else
                            r.ConstantItem(90).AlignRight().Text($"₹{row.Value:N2}").FontColor(Colors.Grey.Darken3);
                    });

                    if (i == rows.Length - 2)
                        col.Item().PaddingTop(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                }
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Text("This is a computer-generated invoice.").FontSize(9).FontColor(Colors.Grey.Darken1);
                    row.ConstantItem(180).AlignRight().Text($"Printed on: {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
                });
            });
        }

        private static IContainer CellHeaderStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1)
                .PaddingVertical(6)
                .PaddingHorizontal(6)
                .DefaultTextStyle(x => x.SemiBold());
        }

        private static IContainer CellBodyStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3)
                .PaddingVertical(6)
                .PaddingHorizontal(6);
        }
    }
}
