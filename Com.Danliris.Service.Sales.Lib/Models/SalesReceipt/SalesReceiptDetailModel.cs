using Com.Danliris.Service.Sales.Lib.Utilities.BaseClass;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Sales.Lib.Models.SalesReceipt
{
    public class SalesReceiptDetailModel : BaseModel
    {
        /*Sales Invoice*/
        public int SalesInvoiceId { get; set; }
        [MaxLength(255)]
        public string SalesInvoiceNo { get; set; }
        public DateTimeOffset DueDate { get; set; }
        [MaxLength(255)]
        public string VatType { get; set; }
        public double Tempo { get; set; }

        /*Currency*/
        public int CurrencyId { get; set; }
        [MaxLength(255)]
        public string CurrencyCode { get; set; }
        [MaxLength(255)]
        public string CurrencySymbol { get; set; }
        public double CurrencyRate { get; set; }

        public double TotalPayment { get; set; }
        public double TotalPaid { get; set; }
        public double Paid { get; set; }
        public double Nominal { get; set; }
        public double Unpaid { get; set; }
        public double OverPaid { get; set; }
        public bool IsPaidOff { get; set; }

        public int SalesReceiptId { get; set; }


        public virtual SalesReceiptModel SalesReceiptModel { get; set; }
    }
}
