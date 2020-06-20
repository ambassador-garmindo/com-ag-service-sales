using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.LocalMerchandiserInterfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.LocalMerchandiserFacades
{
    public class HOrderFacade : IHOrderFacade
    {
        private readonly ILocalMerchandiserDbContext dbContext;

        public HOrderFacade(ILocalMerchandiserDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<string> GetKodeByNo(string no = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("no", no));

            var reader = dbContext.ExecuteReader("SELECT Kode FROM HOrder WHERE No=@no", parameters);

            List<string> data = new List<string>();

            while (reader.Read())
            {
                data.Add(reader.GetString(0));
            }

            return data;
        }
    }
}
