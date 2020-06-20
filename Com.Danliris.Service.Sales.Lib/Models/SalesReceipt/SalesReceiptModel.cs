using Com.Danliris.Service.Sales.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Sales.Lib.Models.SalesReceipt
{
    public class SalesReceiptModel : BaseModel
    {
        [MaxLength(255)]
        public string Code { get; set; }
        public long AutoIncreament { get; set; }
        [MaxLength(255)]
        public string SalesReceiptNo { get; set; }
        [MaxLength(255)]
        public string SalesReceiptType { get; set; }
        public DateTimeOffset SalesReceiptDate { get; set; }

        /*Bank*/
        public int BankId { get; set; }
        [MaxLength(255)]
        public string AccountCOA { get; set; }
        [MaxLength(255)]
        public string AccountName { get; set; }
        [MaxLength(255)]
        public string AccountNumber { get; set; }
        [MaxLength(255)]
        public string BankName { get; set; }
        [MaxLength(255)]
        public string BankCode { get; set; }

        /*Buyer*/
        public int BuyerId { get; set; }
        [MaxLength(255)]
        public string BuyerName { get; set; }
        [MaxLength(1000)]
        public string BuyerAddress { get; set; }

        public double TotalPaid { get; set; }

        public virtual ICollection<SalesReceiptDetailModel> SalesReceiptDetails { get; set; }

    }
}
