using AutoMapper;
using Com.Danliris.Sales.Test.WebApi.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Models.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.Models.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.ViewModels.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.ViewModels.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Sales.Test.WebApi.Controllers
{
    public class ShinFinishingPrintingSalesContractControllerTest : BaseControllerTest<ShinFinishingPrintingSalesContractController, FinishingPrintingSalesContractModel, ShinFinishingPrintingSalesContractViewModel, IShinFinishingPrintingSalesContractFacade>
    {
        protected override FinishingPrintingSalesContractModel Model => new FinishingPrintingSalesContractModel()
        {
            Details = new List<FinishingPrintingSalesContractDetailModel>()
            {
                new FinishingPrintingSalesContractDetailModel()
            }
        };

        protected override ShinFinishingPrintingSalesContractViewModel ViewModel => new ShinFinishingPrintingSalesContractViewModel()
        {
            CostCalculation = new FinishingPrintingCostCalculationViewModel()
            {
                Id = 1
            },
            Details = new List<FinishingPrintingSalesContractDetailViewModel>()
            {
                new FinishingPrintingSalesContractDetailViewModel()
            }
        };

        protected override List<ShinFinishingPrintingSalesContractViewModel> ViewModels => new List<ShinFinishingPrintingSalesContractViewModel>() { ViewModel };

        protected override ShinFinishingPrintingSalesContractController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IShinFinishingPrintingSalesContractFacade> Facade, Mock<IMapper> Mapper, Mock<IServiceProvider> ServiceProvider) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            Mock<IFinishingPrintingPreSalesContractFacade> preSCFacade = new Mock<IFinishingPrintingPreSalesContractFacade>();
            Mock<IFinishingPrintingCostCalculationService> ccFacade = new Mock<IFinishingPrintingCostCalculationService>();
            preSCFacade.Setup(s => s.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new FinishingPrintingPreSalesContractModel());
            ccFacade.Setup(s => s.ReadParent(It.IsAny<long>())).ReturnsAsync(new FinishingPrintingCostCalculationModel());
            user.Setup(u => u.Claims).Returns(claims);
            mocks.ServiceProvider.Setup(s => s.GetService(typeof(IHttpClientService))).Returns(new HttpClientTestService());
            mocks.Mapper.Setup(x => x.Map<FinishingPrintingCostCalculationViewModel>(It.IsAny<FinishingPrintingCostCalculationModel>())).Returns(new FinishingPrintingCostCalculationViewModel() { PreSalesContract = new FinishingPrintingPreSalesContractViewModel() { Id = 1 } });
            mocks.Mapper.Setup(x => x.Map<FinishingPrintingPreSalesContractViewModel>(It.IsAny<FinishingPrintingPreSalesContractModel>())).Returns(new FinishingPrintingPreSalesContractViewModel());
            mocks.ServiceProvider.Setup(s => s.GetService(typeof(IFinishingPrintingCostCalculationService))).Returns(ccFacade.Object);
            mocks.ServiceProvider.Setup(s => s.GetService(typeof(IFinishingPrintingPreSalesContractFacade))).Returns(preSCFacade.Object);
            ShinFinishingPrintingSalesContractController controller = (ShinFinishingPrintingSalesContractController)Activator.CreateInstance(typeof(ShinFinishingPrintingSalesContractController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Facade.Object, mocks.Mapper.Object, mocks.ServiceProvider.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        [Fact]
        public void Get_PDF_NotFound()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(default(FinishingPrintingSalesContractModel));
            var controller = GetController(mocks);
            var response = controller.GetPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void Get_PDF_Local_OK()
        {
            var mocks = GetMocks();

            var vm = new FinishingPrintingSalesContractViewModel
            {
                Agent = new Service.Sales.Lib.ViewModels.IntegrationViewModel.AgentViewModel()
                {
                    Id = 1,
                    Name = "name"
                },
                Buyer = new Service.Sales.Lib.ViewModels.IntegrationViewModel.BuyerViewModel
                {
                    Id = 1,
                    Type = "Lokal"
                },
                AccountBank = new Service.Sales.Lib.ViewModels.IntegrationViewModel.AccountBankViewModel
                {
                    Id = 1
                },
                OrderQuantity = 1,
                UOM = new Service.Sales.Lib.ViewModels.IntegrationViewModel.UomViewModel()
                {
                    Unit = "unit"
                },
                Commodity = new Service.Sales.Lib.ViewModels.IntegrationViewModel.CommodityViewModel()
                {
                    Name = "comm"
                },
                Quality = new Service.Sales.Lib.ViewModels.IntegrationViewModel.QualityViewModel()
                {
                    Name = "name"
                },
                DesignMotive = new Service.Sales.Lib.ViewModels.IntegrationViewModel.OrderTypeViewModel()
                {
                    Name = "name"
                },
                TermOfPayment = new Service.Sales.Lib.ViewModels.IntegrationViewModel.TermOfPaymentViewModel()
                {
                    Name = "tp"
                },
                DeliverySchedule = DateTimeOffset.UtcNow,
                UseIncomeTax = false,
                Details = new List<FinishingPrintingSalesContractDetailViewModel>()
                {
                    new FinishingPrintingSalesContractDetailViewModel()
                    {
                        UseIncomeTax = false,
                        Currency = new Service.Sales.Lib.ViewModels.IntegrationViewModel.CurrencyViewModel()
                        {
                            Code = "code",
                            Symbol = "c"
                        }
                    }
                },
                Amount = 1,
                Material = new Service.Sales.Lib.ViewModels.IntegrationViewModel.ProductViewModel(),
                MaterialConstruction = new Service.Sales.Lib.ViewModels.IntegrationViewModel.MaterialConstructionViewModel(),
                YarnMaterial = new Service.Sales.Lib.ViewModels.IntegrationViewModel.YarnMaterialViewModel()
            };

            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(f => f.Map<FinishingPrintingSalesContractViewModel>(It.IsAny<FinishingPrintingSalesContractModel>())).Returns(vm);

            var controller = GetController(mocks);
            var response = controller.GetPDF(1).Result;

            Assert.NotNull(response);

        }

        [Fact]
        public void Get_PDF_Ekspor_OK()
        {
            var mocks = GetMocks();

            var vm = new FinishingPrintingSalesContractViewModel
            {
                Agent = new Service.Sales.Lib.ViewModels.IntegrationViewModel.AgentViewModel()
                {
                    Id = 1,
                    Name = "name"
                },
                Buyer = new Service.Sales.Lib.ViewModels.IntegrationViewModel.BuyerViewModel
                {
                    Id = 1,
                    Type = "Ekspor"
                },
                AccountBank = new Service.Sales.Lib.ViewModels.IntegrationViewModel.AccountBankViewModel
                {
                    Id = 1
                },
                OrderQuantity = 1,
                UOM = new Service.Sales.Lib.ViewModels.IntegrationViewModel.UomViewModel()
                {
                    Unit = "unit"
                },
                Commodity = new Service.Sales.Lib.ViewModels.IntegrationViewModel.CommodityViewModel()
                {
                    Name = "comm"
                },
                Quality = new Service.Sales.Lib.ViewModels.IntegrationViewModel.QualityViewModel()
                {
                    Name = "name"
                },
                DesignMotive = new Service.Sales.Lib.ViewModels.IntegrationViewModel.OrderTypeViewModel()
                {
                    Name = "name"
                },
                TermOfPayment = new Service.Sales.Lib.ViewModels.IntegrationViewModel.TermOfPaymentViewModel()
                {
                    Name = "tp"
                },
                DeliverySchedule = DateTimeOffset.UtcNow,
                UseIncomeTax = false,
                Details = new List<FinishingPrintingSalesContractDetailViewModel>()
                {
                    new FinishingPrintingSalesContractDetailViewModel()
                    {
                        UseIncomeTax = false,
                        Currency = new Service.Sales.Lib.ViewModels.IntegrationViewModel.CurrencyViewModel()
                        {
                            Code = "code",
                            Symbol = "c"
                        }
                    }
                },
                Amount = 1,
                Material = new Service.Sales.Lib.ViewModels.IntegrationViewModel.ProductViewModel(),
                MaterialConstruction = new Service.Sales.Lib.ViewModels.IntegrationViewModel.MaterialConstructionViewModel(),
                YarnMaterial = new Service.Sales.Lib.ViewModels.IntegrationViewModel.YarnMaterialViewModel()
            };

            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(f => f.Map<FinishingPrintingSalesContractViewModel>(It.IsAny<FinishingPrintingSalesContractModel>())).Returns(vm);

            var controller = GetController(mocks);
            var response = controller.GetPDF(1).Result;

            Assert.NotNull(response);

        }

        [Fact]
        public void Get_PDF_Exception()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));
            var controller = GetController(mocks);
            var response = controller.GetPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);

        }

        public override async Task GetById_NotNullModel_ReturnOK()
        {
            var mocks = this.GetMocks();
            mocks.Facade.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(this.Model);
            mocks.Mapper.Setup(f => f.Map<ShinFinishingPrintingSalesContractViewModel>(It.IsAny<FinishingPrintingSalesContractModel>())).Returns(this.ViewModel);
            mocks.Mapper.Setup(f => f.Map<FinishingPrintingCostCalculationViewModel>(It.IsAny<FinishingPrintingCostCalculationModel>())).Returns(new FinishingPrintingCostCalculationViewModel()
            {
                PreSalesContract = new FinishingPrintingPreSalesContractViewModel()
                {
                    Id = 1
                }
            });
            ShinFinishingPrintingSalesContractController controller = this.GetController(mocks);
            IActionResult response = await controller.GetById(1);

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }
    }
}
