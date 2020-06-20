using AutoMapper;
using Com.Danliris.Sales.Test.WebApi.Utils;
using Com.Danliris.Service.Sales.Lib.AutoMapperProfiles.SalesInvoiceProfiles;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.Models.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.ViewModels.SalesInvoice;
using Com.Danliris.Service.Sales.WebApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Com.Danliris.Sales.Test.WebApi.Controllers
{
    public class SalesInvoiceControllerTest : BaseControllerTest<SalesInvoiceController, SalesInvoiceModel, SalesInvoiceViewModel, ISalesInvoiceContract>
    {
        [Fact]
        public void Get_Delivery_Order_PDF_Success()
        {
            var vm = new SalesInvoiceViewModel()
            {
                DeliveryOrderNo = "DeliveryOrderNo",
                SalesInvoiceNo = "SalesInvoiceNo",
                SalesInvoiceDate = DateTimeOffset.Now,
                BuyerName = "BuyerName",
                BuyerAddress = "BuyerAddress",
                Remark = "Remark",
                SalesInvoiceDetails = new List<SalesInvoiceDetailViewModel>()
                {
                    new SalesInvoiceDetailViewModel()
                    {
                        ProductName = "ProductName",
                        UomUnit = "PACKS",
                        Quantity = "Quantity",
                        Total = 1,
                    }
                }

            };
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(s => s.Map<SalesInvoiceViewModel>(It.IsAny<SalesInvoiceModel>()))
                .Returns(vm);
            var controller = GetController(mocks);
            var response = controller.GetDeliveryOrderPDF(1).Result;

            Assert.NotNull(response);

        }

        [Fact]
        public void Get_Delivery_Order_PDF_NotFound()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(default(SalesInvoiceModel));
            var controller = GetController(mocks);
            var response = controller.GetDeliveryOrderPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void Get_Delivery_Order_PDF_Exception()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));
            var controller = GetController(mocks);
            var response = controller.GetDeliveryOrderPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);

        }

        [Fact]
        public void Get_Sales_Invoice_PDF_VatType_Is_PPN_Umum_And_CurrencySymbol_Is_IDR()
        {
            var vm = new SalesInvoiceViewModel()
            {
                BuyerName = "BuyerName",
                BuyerAddress = "BuyerAddress",
                BuyerNPWP = "BuyerNPWP",
                SalesInvoiceNo = "SalesInvoiceNo",
                SalesInvoiceDate = DateTimeOffset.Now,
                DueDate = DateTimeOffset.Now,
                DebtorIndexNo = "DebtorIndexNo",
                IDNo = "IDNo",
                CurrencySymbol = "Rp",
                Remark = "Remark",
                VatType = "PPN Umum",
                SalesInvoiceDetails = new List<SalesInvoiceDetailViewModel>()
                {
                    new SalesInvoiceDetailViewModel()
                    {
                        ProductCode = "ProductCode",
                        Quantity = "Quantity",
                        UomUnit = "PACKS",
                        ProductName = "ProductName",
                        Total = 1,
                        Price = 1,
                        Amount = 1,
                    }
                }

            };
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(s => s.Map<SalesInvoiceViewModel>(It.IsAny<SalesInvoiceModel>()))
                .Returns(vm);
            var controller = GetController(mocks);
            var response = controller.GetSalesInvoicePDF(1).Result;

            Assert.NotNull(response);

        }
        [Fact]
        public void Get_Sales_Invoice_PDF_VatType_Is_PPN_BUMN_And_CurrencySymbol_Is_USD()
        {
            var vm = new SalesInvoiceViewModel()
            {
                BuyerName = "BuyerName",
                BuyerAddress = "BuyerAddress",
                BuyerNPWP = "BuyerNPWP",
                SalesInvoiceNo = "SalesInvoiceNo",
                SalesInvoiceDate = DateTimeOffset.Now,
                DueDate = DateTimeOffset.Now,
                DebtorIndexNo = "DebtorIndexNo",
                IDNo = "IDNo",
                CurrencySymbol = "$",
                Remark = "Remark",
                VatType = "PPN BUMN",
                SalesInvoiceDetails = new List<SalesInvoiceDetailViewModel>()
                {
                    new SalesInvoiceDetailViewModel()
                    {
                        ProductCode = "ProductCode",
                        Quantity = "Quantity",
                        UomUnit = "PACKS",
                        ProductName = "ProductName",
                        Total = 1,
                        Price = 1,
                        Amount = 1,
                    }
                }

            };
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(s => s.Map<SalesInvoiceViewModel>(It.IsAny<SalesInvoiceModel>()))
                .Returns(vm);
            var controller = GetController(mocks);
            var response = controller.GetSalesInvoicePDF(1).Result;

            Assert.NotNull(response);

        }

        [Fact]
        public void Get_Sales_Invoice_PDF__Other_CurrencySymbol()
        {
            var vm = new SalesInvoiceViewModel()
            {
                CurrencySymbol = "Rp",
            };
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(s => s.Map<SalesInvoiceViewModel>(It.IsAny<SalesInvoiceModel>()))
                .Returns(vm);
            var controller = GetController(mocks);
            var response = controller.GetSalesInvoicePDF(1).Result;

            Assert.NotNull(response);

        }


        [Fact]
        public void Get_Sales_Invoice_PDF_NotFound()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(default(SalesInvoiceModel));
            var controller = GetController(mocks);
            var response = controller.GetSalesInvoicePDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void Get_Sales_Invoice_PDF_Exception()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));
            var controller = GetController(mocks);
            var response = controller.GetSalesInvoicePDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);

        }

        [Fact]
        public void Mapping_With_AutoMapper_Profiles()
        {
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<SalesInvoiceMapper>();
                cfg.AddProfile<SalesInvoiceDetailMapper>();
            });
            var mapper = configuration.CreateMapper();

            SalesInvoiceViewModel salesInvoiceViewModel = new SalesInvoiceViewModel { Id = 1 };
            SalesInvoiceModel salesInvoiceModel = mapper.Map<SalesInvoiceModel>(salesInvoiceViewModel);

            Assert.Equal(salesInvoiceViewModel.Id, salesInvoiceModel.Id);

            SalesInvoiceDetailViewModel salesInvoiceDetailViewModel = new SalesInvoiceDetailViewModel { Id = 1 };
            SalesInvoiceDetailModel salesInvoiceDetailModel = mapper.Map<SalesInvoiceDetailModel>(salesInvoiceDetailViewModel);

            Assert.Equal(salesInvoiceDetailViewModel.Id, salesInvoiceDetailModel.Id);
        }

        [Fact]
        public void Validate_Validation_ViewModel()
        {
            List<SalesInvoiceViewModel> viewModels = new List<SalesInvoiceViewModel>
            {
                new SalesInvoiceViewModel{
                    SalesInvoiceType = "",
                    SalesInvoiceDate = DateTimeOffset.UtcNow.AddDays(1),
                    DeliveryOrderNo = "",
                    ShipmentDocumentId = 0,
                    ShipmentDocumentCode = "",
                    CurrencyId = 0,
                    CurrencyCode = "",
                    CurrencyRate = 0,
                    BuyerId = 0,
                    BuyerName = "",
                    DebtorIndexNo = "",
                    DueDate = DateTimeOffset.UtcNow.AddDays(-1),
                    TotalPayment = 0,
                    TotalPaid = -1,
                    SalesInvoiceDetails = new List<SalesInvoiceDetailViewModel>{
                        new SalesInvoiceDetailViewModel{
                            ProductCode = "",
                            Quantity = "",
                            UomId = 0,
                            UomUnit = "",
                            ProductName = "",
                            Amount = 0,
                        }
                    }
                }
            };
            foreach (var viewModel in viewModels)
            {
                var defaultValidationResult = viewModel.Validate(null);
                Assert.True(defaultValidationResult.Count() > 0);
            }
        }

        [Fact]
        public void Validate_Duplicate_DetailViewModel()
        {
            List<SalesInvoiceViewModel> viewModels = new List<SalesInvoiceViewModel>
            {
                new SalesInvoiceViewModel{
                    DueDate = DateTimeOffset.Now,
                    SalesInvoiceDetails = new List<SalesInvoiceDetailViewModel>{
                        new SalesInvoiceDetailViewModel{
                            SalesInvoiceId = 2,
                            ProductCode = "ProductCode",
                            Quantity = "Quantity",
                            Total = 10,
                            UomId = 10,
                            UomUnit = "PCS",
                            ProductName = "ProductName",
                            Price = 100,
                            Amount = 100,
                        },
                        new SalesInvoiceDetailViewModel{
                            SalesInvoiceId = 2,
                            ProductCode = "ProductCode",
                            Quantity = "Quantity",
                            Total = 10,
                            UomId = 10,
                            UomUnit = "PCS",
                            ProductName = "ProductName",
                            Price = 100,
                            Amount = 100,
                        }
                    }
                }
            };
            foreach (var viewModel in viewModels)
            {
                var defaultValidationResult = viewModel.Validate(null);
                Assert.True(defaultValidationResult.Count() > 0);
            }
        }

        [Fact]
        public void Validate_Null_Model_and_DetailViewModel()
        {
            List<SalesInvoiceViewModel> viewModels = new List<SalesInvoiceViewModel>
            {};
            foreach (var viewModel in viewModels)
            {
                var defaultValidationResult = viewModel.Validate(null);
                Assert.True(defaultValidationResult.Count() > 0);
            }
        }
    }
}
