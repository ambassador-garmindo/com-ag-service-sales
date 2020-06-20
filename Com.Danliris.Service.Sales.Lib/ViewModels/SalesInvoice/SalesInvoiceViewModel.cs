using Com.Danliris.Service.Sales.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Danliris.Service.Sales.Lib.ViewModels.SalesInvoice
{
    public class SalesInvoiceViewModel : BaseViewModel, IValidatableObject
    {
        [MaxLength(255)]
        public string Code { get; set; }
        public long AutoIncreament { get; set; }
        [MaxLength(255)]
        public string SalesInvoiceNo { get; set; }
        [MaxLength(255)]
        public string SalesInvoiceType { get; set; }
        public DateTimeOffset? SalesInvoiceDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }

        [MaxLength(255)]
        public string DeliveryOrderNo { get; set; }
        [MaxLength(255)]
        public string DebtorIndexNo { get; set; }

        /*Shipment Document*/
        public int? ShipmentDocumentId { get; set; }
        [MaxLength(255)]
        public string ShipmentDocumentCode { get; set; }

        /*Buyer*/
        public int? BuyerId { get; set; }
        [MaxLength(255)]
        public string BuyerName { get; set; }
        [MaxLength(1000)]
        public string BuyerAddress { get; set; }
        [MaxLength(255)]
        public string BuyerNPWP { get; set; }
        [MaxLength(255)]
        public string IDNo { get; set; }

        /*Currency*/
        public int? CurrencyId { get; set; }
        [MaxLength(255)]
        public string CurrencyCode { get; set; }
        [MaxLength(255)]
        public string CurrencySymbol { get; set; }
        public double CurrencyRate { get; set; }

        [MaxLength(255)]
        public string VatType { get; set; }
        public double TotalPayment { get; set; }
        public double TotalPaid { get; set; }
        [MaxLength(1000)]
        public string Remark { get; set; }


        public ICollection<SalesInvoiceDetailViewModel> SalesInvoiceDetails { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(SalesInvoiceType))
                yield return new ValidationResult("Kode Faktur Penjualan harus diisi", new List<string> { "SalesInvoiceType" });

            if (!SalesInvoiceDate.HasValue || SalesInvoiceDate.Value > DateTimeOffset.Now)
                yield return new ValidationResult("Tgl Faktur Penjualan harus diisi & lebih kecil  atau sama dengan hari ini", new List<string> { "SalesInvoiceDate" });

            if (string.IsNullOrWhiteSpace(ShipmentDocumentCode))
                yield return new ValidationResult("No. Bon Pengiriman Barang harus diisi", new List<string> { "ShipmentDocumentCode" });

            if (string.IsNullOrWhiteSpace(DeliveryOrderNo))
                yield return new ValidationResult("No. Surat Jalan harus diisi", new List<string> { "DeliveryOrderNo" });

            if (string.IsNullOrWhiteSpace(DebtorIndexNo))
                yield return new ValidationResult("No. Index Debitur harus diisi", new List<string> { "DebtorIndexNo" });

            if (string.IsNullOrWhiteSpace(CurrencyCode))
                yield return new ValidationResult("Kurs harus diisi", new List<string> { "CurrencyCode" });

            if (!DueDate.HasValue || Id == 0 && DueDate.Value < DateTimeOffset.Now.AddDays(-1))
                yield return new ValidationResult("Tanggal jatuh tempo harus diisi & lebih besar dari hari ini", new List<string> { "DueDate" });
            
            if (string.IsNullOrWhiteSpace(VatType))
                yield return new ValidationResult("Jenis PPN harus diisi", new List<string> { "VatType" });

            if (TotalPayment <= 0)
                yield return new ValidationResult("Total termasuk PPN kosong", new List<string> { "TotalPayment" });

            if (TotalPaid < 0)
                yield return new ValidationResult("Total Paid harus lebih besar atau sama dengan 0", new List<string> { "TotalPayment" });

            int Count = 0;
            string DetailErrors = "[";

            if (SalesInvoiceDetails != null && SalesInvoiceDetails.Count > 0)
            {
                foreach (SalesInvoiceDetailViewModel detail in SalesInvoiceDetails)
                {
                    DetailErrors += "{";

                    var rowErrorCount = 0;

                    if (string.IsNullOrWhiteSpace(detail.ProductCode))
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "ProductCode : 'Kode harus diisi',";
                    }
                    if (string.IsNullOrWhiteSpace(detail.ProductName))
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "ProductName : 'Kode harus diisi',";
                    }
                    if (string.IsNullOrWhiteSpace(detail.Quantity))
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "Quantity : 'Kuantitas harus diisi',";
                    }
                    if (!detail.Total.HasValue || detail.Total.Value <= 0)
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "Total : 'Jumlah harus lebih besar dari 0',";
                    }
                    if (string.IsNullOrWhiteSpace(detail.UomUnit))
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "UomUnit : 'Satuan harus diisi',";
                    }
                    if (!detail.Price.HasValue || detail.Price.Value <= 0)
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "Price : 'Harga barang harus lebih besar dari 0',";
                    }
                    if (detail.Amount <= 0)
                    {
                        Count++;
                        rowErrorCount++;
                        DetailErrors += "Amount : 'Jumlah & Harga Satuan harus diisi',";
                    }

                    if (rowErrorCount == 0)
                    {
                        var duplicateDetails = SalesInvoiceDetails.Where(f =>
                                f.ProductCode.Equals(detail.ProductCode) &&
                                f.ProductName.Equals(detail.ProductName) &&
                                f.Price.GetValueOrDefault().Equals(detail.Price.GetValueOrDefault()) &&
                                f.UomId.GetValueOrDefault().Equals(detail.UomId.GetValueOrDefault())
                            ).ToList();

                        if (duplicateDetails.Count > 1)
                        {
                            Count++;
                            DetailErrors += "ProductCode : 'Kode, Nama Barang, dan Harga Satuan tidak boleh duplikat',";
                            DetailErrors += "ProductName : 'Kode, Nama Barang, dan Harga Satuan tidak boleh duplikat',";
                            DetailErrors += "Price : 'Kode, Nama Barang, dan Harga Satuan tidak boleh duplikat',";
                        }
                    }
                    DetailErrors += "}, ";
                }
            }
            else
            {
                yield return new ValidationResult("Detail harus diisi", new List<string> { "SalesInvoiceDetail" });
            }

            DetailErrors += "]";

            if (Count > 0)
                yield return new ValidationResult(DetailErrors, new List<string> { "SalesInvoiceDetails" });

        }
    }
}
