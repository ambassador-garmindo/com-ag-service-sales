using Com.Danliris.Sales.Test.WebApi.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface;
using Com.Danliris.Service.Sales.Lib.Models;
using Com.Danliris.Service.Sales.Lib.ViewModels;
using Com.Danliris.Service.Sales.WebApi.Controllers;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Sales.Test.WebApi.Controllers
{
    public class EfficienciesControllerTest : BaseControllerTest<EfficienciesController, Efficiency, EfficiencyViewModel, IEfficiency>
    {
        [Fact]
        public async Task Should_Success_GetByQUantity()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadModelByQuantity(It.IsAny<int>()))
                .ReturnsAsync(Model);
            var controller = GetController(mocks);
            var response = await controller.GetByQuantity(1);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NotFound_GetByQUantity()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadModelByQuantity(It.IsAny<int>()))
                .ReturnsAsync(default(Efficiency));
            var controller = GetController(mocks);
            var response = await controller.GetByQuantity(1);
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_InternalServerError_GetByQUantity()
        {
            var mocks = GetMocks();
            mocks.Facade.Setup(x => x.ReadModelByQuantity(It.IsAny<int>()))
                .ThrowsAsync(new Exception());
            var controller = GetController(mocks);
            var response = await controller.GetByQuantity(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
