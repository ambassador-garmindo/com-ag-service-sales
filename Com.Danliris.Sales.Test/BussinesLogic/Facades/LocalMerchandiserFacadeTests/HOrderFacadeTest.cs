using Com.Danliris.Service.Sales.Lib;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.LocalMerchandiserFacades;
using Com.Danliris.Service.Sales.WebApi.Controllers.LocalMerchandiserControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using Xunit;

namespace Com.Danliris.Sales.Test.WebApi.Controllers.LocalMerchandiserControllerTests
{
    public class HOrderFacadeTest
    {
        [Fact]
        public void Get_Kode_By_No_Success()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Kode", typeof(string));
            dataTable.Rows.Add("KODE");

            Mock<ILocalMerchandiserDbContext> mockDbContext = new Mock<ILocalMerchandiserDbContext>();
            mockDbContext.Setup(s => s.ExecuteReader(It.IsAny<string>(), It.IsAny<List<SqlParameter>>()))
                .Returns(dataTable.CreateDataReader());

            HOrderFacade facade = new HOrderFacade(mockDbContext.Object);

            var result = facade.GetKodeByNo();

            Assert.NotNull(result);
        }

        [Fact]
        public void Get_Kode_By_No_Error()
        {
            ILocalMerchandiserDbContext dbContext = new LocalMerchandiserDbContext("Server=server;Database=test;");

            HOrderFacade facade = new HOrderFacade(dbContext);

            var result = Assert.ThrowsAny<Exception>(() => facade.GetKodeByNo());
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_Connection_Error()
        {
            var result = Assert.ThrowsAny<Exception>(() => new LocalMerchandiserDbContext(""));
            Assert.NotNull(result);
        }
    }
}
