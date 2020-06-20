using Com.Danliris.Sales.Test.BussinesLogic.DataUtils.SalesInvoice;
using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.Models.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.Services;
using Moq;
using System;

namespace Com.Danliris.Sales.Test.BussinesLogic.Facades.SalesInvoice
{
    public class SalesInvoiceFacadeTest : BaseFacadeTest<SalesDbContext, SalesInvoiceFacade, SalesInvoiceLogic, SalesInvoiceModel, SalesInvoiceDataUtil>
    {
        private const string ENTITY = "SalesInvoice";
        public SalesInvoiceFacadeTest() : base(ENTITY)
        {
        }

        protected override Mock<IServiceProvider> GetServiceProviderMock(SalesDbContext dbContext)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            IIdentityService identityService = new IdentityService { Username = "Username" };

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(identityService);

            var salesInvoiceDetailLogic = new SalesInvoiceDetailLogic(serviceProviderMock.Object, identityService, dbContext);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(SalesInvoiceDetailLogic)))
                .Returns(salesInvoiceDetailLogic);

            var salesInvoiceLogic = new SalesInvoiceLogic(serviceProviderMock.Object, identityService, dbContext);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(SalesInvoiceLogic)))
                .Returns(salesInvoiceLogic);

            return serviceProviderMock;
        }
    }
}
