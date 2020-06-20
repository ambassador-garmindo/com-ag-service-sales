using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.Models.SalesInvoice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Sales.Test.BussinesLogic.DataUtils.SalesInvoice
{
    public class SalesInvoiceDataUtil : BaseDataUtil<SalesInvoiceFacade, SalesInvoiceModel>
    {
        public SalesInvoiceDataUtil(SalesInvoiceFacade facade) : base(facade)
        {
        }

        public override async Task<SalesInvoiceModel> GetNewData()
        {
            return new SalesInvoiceModel()
            {
                Code = "code",
                AutoIncreament = 1,
                SalesInvoiceNo = "SalesInvoiceNo",
                SalesInvoiceType = "BPF",
                SalesInvoiceDate = DateTimeOffset.UtcNow,
                DueDate = DateTimeOffset.UtcNow.AddDays(-2),
                DeliveryOrderNo = "DeliveryOrderNo",
                DebtorIndexNo = "DebtorIndexNo",
                ShipmentDocumentId = 1,
                ShipmentDocumentCode = "ShipmentDocumentCode",
                BuyerId = 1,
                BuyerName = "BuyerName",
                BuyerAddress = "BuyerAddress",
                BuyerNPWP = "BuyerNPWP",
                IDNo = "IDNo",
                CurrencyId = 1,
                CurrencyCode = "IDR",
                CurrencySymbol = "Rp",
                CurrencyRate = 14000,
                VatType = "PPN Kawasan Berikat",
                Remark = "Remark",
                TotalPayment = 100,
                TotalPaid = 0,

                SalesInvoiceDetails = new List<SalesInvoiceDetailModel>()
                {
                    new SalesInvoiceDetailModel()
                    {
                        ProductCode = "ProductCode",
                        ProductName = "ProductName",
                        Quantity = "Quantity",
                        UomId = 1,
                        UomUnit = "PCS",
                        Total = 1,
                        Price = 1,
                        Amount = 1
                    }
                }
            };
        }
    }
}
