using AutoMapper;
using Com.Danliris.Sales.Test.WebApi.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Models.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.Models.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.ViewModels.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.ViewModels.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.ViewModels.IntegrationViewModel;
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
    public class FinishingPrintingCostCalculationControllerTest : BaseControllerTest<FinishingPrintingCostCalculationController, FinishingPrintingCostCalculationModel, FinishingPrintingCostCalculationViewModel, IFinishingPrintingCostCalculationService>
    {
        protected override FinishingPrintingCostCalculationController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IFinishingPrintingCostCalculationService> Facade, Mock<IMapper> Mapper, Mock<IServiceProvider> ServiceProvider) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            Mock<IFinishingPrintingPreSalesContractFacade> preSCFacade = new Mock<IFinishingPrintingPreSalesContractFacade>();
            preSCFacade.Setup(s => s.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new FinishingPrintingPreSalesContractModel());
            user.Setup(u => u.Claims).Returns(claims);
            mocks.ServiceProvider.Setup(s => s.GetService(typeof(IHttpClientService))).Returns(new HttpClientTestService());
            mocks.ServiceProvider.Setup(s => s.GetService(typeof(IFinishingPrintingPreSalesContractFacade))).Returns(preSCFacade.Object);
            FinishingPrintingCostCalculationController controller = (FinishingPrintingCostCalculationController)Activator.CreateInstance(typeof(FinishingPrintingCostCalculationController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Facade.Object, mocks.Mapper.Object, mocks.ServiceProvider.Object);
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

        protected override FinishingPrintingCostCalculationModel Model => new FinishingPrintingCostCalculationModel()
        {
            Machines = new List<FinishingPrintingCostCalculationMachineModel>()
            {
                new FinishingPrintingCostCalculationMachineModel()
                {
                    Chemicals = new List<FinishingPrintingCostCalculationChemicalModel>()
                    {
                        new FinishingPrintingCostCalculationChemicalModel()
                    }
                }
            }
        };

        protected override FinishingPrintingCostCalculationViewModel ViewModel => new FinishingPrintingCostCalculationViewModel()
        {
            PreSalesContract = new FinishingPrintingPreSalesContractViewModel()
            {
                Id = 1
            },
            Instruction = new InstructionViewModel()
            {

            },
            Machines = new List<FinishingPrintingCostCalculationMachineViewModel>()
            {
                new FinishingPrintingCostCalculationMachineViewModel()
                {
                    Chemicals = new List<FinishingPrintingCostCalculationChemicalViewModel>()
                    {
                        new FinishingPrintingCostCalculationChemicalViewModel()
                    }
                }
            }
        };

        protected override List<FinishingPrintingCostCalculationViewModel> ViewModels => new List<FinishingPrintingCostCalculationViewModel>()
        {
            ViewModel
        };

        [Fact]
        public async Task Should_Success_CCPost()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<FinishingPrintingPreSalesContractViewModel>())).Verifiable();
            var id = 1;
            var viewModel = new FinishingPrintingPreSalesContractViewModel()
            {
                Id = id
            };
            mocks.Mapper.Setup(m => m.Map<FinishingPrintingPreSalesContractViewModel>(It.IsAny<FinishingPrintingPreSalesContractModel>())).Returns(viewModel);
            mocks.Facade.Setup(f => f.CCPost(It.IsAny<List<long>>())).ReturnsAsync(1);
            List<long> listId = new List<long> { viewModel.Id };

            var controller = GetController(mocks);
            var response = await controller.CCPost(listId);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_PreSalesPost()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<FinishingPrintingPreSalesContractViewModel>())).Verifiable();
            var id = 1;
            var viewModel = new FinishingPrintingPreSalesContractViewModel()
            {
                Id = id
            };
            mocks.Mapper.Setup(m => m.Map<FinishingPrintingPreSalesContractViewModel>(It.IsAny<FinishingPrintingPreSalesContractModel>())).Returns(viewModel);
            mocks.Facade.Setup(f => f.CCPost(It.IsAny<List<long>>())).ThrowsAsync(new Exception());
            List<long> listId = new List<long> { viewModel.Id };

            var controller = GetController(mocks);
            var response = await controller.CCPost(listId);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_CCApprovePPIC()
        {
            var mocks = GetMocks();
            
            var id = 1;
            mocks.Facade.Setup(f => f.CCApprovePPIC(It.IsAny<long>())).ReturnsAsync(1);
            
            var controller = GetController(mocks);
            var response = await controller.CCApproveByPPIC(id);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_CCApprovePPIC()
        {
            var mocks = GetMocks();

            var id = 1;
            mocks.Facade.Setup(f => f.CCApprovePPIC(It.IsAny<long>())).ThrowsAsync(new Exception("err"));

            var controller = GetController(mocks);
            var response = await controller.CCApproveByPPIC(id);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_CCApproveMD()
        {
            var mocks = GetMocks();

            var id = 1;
            mocks.Facade.Setup(f => f.CCApproveMD(It.IsAny<long>())).ReturnsAsync(1);

            var controller = GetController(mocks);
            var response = await controller.CCApproveByMD(id);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_CCApproveMD()
        {
            var mocks = GetMocks();

            var id = 1;
            mocks.Facade.Setup(f => f.CCApproveMD(It.IsAny<long>())).ThrowsAsync(new Exception("err"));

            var controller = GetController(mocks);
            var response = await controller.CCApproveByMD(id);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
