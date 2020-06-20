using Com.Danliris.Sales.Test.BussinesLogic.DataUtils;
using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic;
using Com.Danliris.Service.Sales.Lib.Models;
using Com.Danliris.Service.Sales.Lib.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Sales.Test.BussinesLogic.Facades
{
    public class ArticleColorFacadeTest : BaseFacadeTest<SalesDbContext, ArticleColorFacade, ArticleColorLogic, ArticleColor, ArticleColorDataUtil>
    {
        private const string ENTITY = "ArticleColor";
        public ArticleColorFacadeTest() : base(ENTITY)
        {
        }

        protected override Mock<IServiceProvider> GetServiceProviderMock(SalesDbContext dbContext)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            IIdentityService identityService = new IdentityService { Username = "Username" };

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(identityService);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(ArticleColorLogic)))
                .Returns(Activator.CreateInstance(typeof(ArticleColorLogic), serviceProviderMock.Object, identityService, dbContext) as ArticleColorLogic);

            return serviceProviderMock;
        }
    }
}
