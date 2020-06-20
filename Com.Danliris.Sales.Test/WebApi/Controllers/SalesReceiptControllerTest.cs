using AutoMapper;
using Com.Danliris.Sales.Test.WebApi.Utils;
using Com.Danliris.Service.Sales.Lib.AutoMapperProfiles.SalesReceiptProfiles;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.ViewModels.SalesReceipt;
using Com.Danliris.Service.Sales.WebApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Com.Danliris.Sales.Test.WebApi.Controllers
{
    public class SalesReceiptControllerTest : BaseControllerTest<SalesReceiptController, SalesReceiptModel, SalesReceiptViewModel, ISalesReceiptContract>
    {
        [Fact]
        public void Get_Sales_Receipt_PDF_Success()
        {
            var vm = new SalesReceiptViewModel()
            {

                SalesReceiptDetails = new List<SalesReceiptDetailViewModel>()
                {
                    new SalesReceiptDetailViewModel()
                    {
                        CurrencyCode = "IDR",
                        CurrencySymbol = "Rp",
                    }
                }

            };
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(s => s.Map<SalesReceiptViewModel>(It.IsAny<SalesReceiptModel>()))
                .Returns(vm);
            var controller = GetController(mocks);
            var response = controller.GetSalesReceiptPDF(1).Result;

            Assert.NotNull(response);

        }

        [Fact]
        public void Get_Sales_Receipt_PDF_NotFound()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(default(SalesReceiptModel));
            var controller = GetController(mocks);
            var response = controller.GetSalesReceiptPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void Get_Sales_Receipt_PDF_Exception()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));
            var controller = GetController(mocks);
            var response = controller.GetSalesReceiptPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);

        }

        [Fact]
        public void Mapping_With_AutoMapper_Profiles()
        {
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<SalesReceiptMapper>();
                cfg.AddProfile<SalesReceiptDetailMapper>();
            });
            var mapper = configuration.CreateMapper();

            SalesReceiptViewModel salesReceiptViewModel = new SalesReceiptViewModel { Id = 1 };
            SalesReceiptModel salesReceiptModel = mapper.Map<SalesReceiptModel>(salesReceiptViewModel);

            Assert.Equal(salesReceiptViewModel.Id, salesReceiptModel.Id);

            SalesReceiptDetailViewModel salesReceiptDetailViewModel = new SalesReceiptDetailViewModel { Id = 1 };
            SalesReceiptDetailModel salesReceiptDetailModel = mapper.Map<SalesReceiptDetailModel>(salesReceiptDetailViewModel);

            Assert.Equal(salesReceiptDetailViewModel.Id, salesReceiptDetailModel.Id);
        }

        [Fact]
        public void Validate_Validation_ViewModel()
        {
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {
                new SalesReceiptViewModel{
                    SalesReceiptType = "",
                    SalesReceiptDate = DateTimeOffset.UtcNow.AddDays(-1),
                    BankId = 0,
                    AccountCOA = "",
                    AccountName = "",
                    AccountNumber = "",
                    BankName = "",
                    BankCode = "",
                    BuyerId = 0,
                    BuyerName = "",
                    BuyerAddress = "",
                    TotalPaid = -1,
                    SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            SalesReceiptId = 0,
                            SalesInvoiceId = 0,
                            SalesInvoiceNo = "",
                            DueDate = DateTimeOffset.UtcNow.AddDays(-1),
                            VatType = "",
                            CurrencyId = 0,
                            CurrencyCode = "",
                            CurrencySymbol = "",
                            CurrencyRate = 0,
                            TotalPayment = -1,
                            TotalPaid = -1,
                            Paid = -1,
                            Nominal = -1,
                            Unpaid = -1,
                            IsPaidOff = false
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
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {};
            foreach (var viewModel in viewModels)
            {
                var defaultValidationResult = viewModel.Validate(null);
                Assert.True(defaultValidationResult.Count() > 0);
            }
        }

        [Fact]
        public void Validate_Duplicate_DetailViewModel()
        {
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {
                new SalesReceiptViewModel{
                    SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            SalesReceiptId = 10,
                            SalesInvoiceId = 10,
                            SalesInvoiceNo = "SalesInvoiceNo",
                            DueDate = DateTimeOffset.UtcNow,
                            VatType = "PPN Kawasan Berikat",
                            Tempo = 10,
                            CurrencyId = 10,
                            CurrencyCode = "USD",
                            CurrencySymbol = "$",
                            CurrencyRate = 10,
                            TotalPayment = 10,
                            TotalPaid = 10,
                            Paid = 10,
                            Nominal = 10,
                            Unpaid = 10,
                            OverPaid = 10,
                            IsPaidOff = true
                        },
                        new SalesReceiptDetailViewModel{
                            SalesReceiptId = 10,
                            SalesInvoiceId = 10,
                            SalesInvoiceNo = "SalesInvoiceNo",
                            DueDate = DateTimeOffset.UtcNow,
                            VatType = "PPN Kawasan Berikat",
                            Tempo = 10,
                            CurrencyId = 10,
                            CurrencyCode = "USD",
                            CurrencySymbol = "$",
                            CurrencyRate = 10,
                            TotalPayment = 10,
                            TotalPaid = 10,
                            Paid = 10,
                            Nominal = 10,
                            Unpaid = 10,
                            OverPaid = 10,
                            IsPaidOff = true
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
        public void Validate_VatType_DetailViewModel()
        {
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {
                new SalesReceiptViewModel{
                    SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            VatType = "PPN Umum",
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
    }
}
