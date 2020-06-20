using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.LocalMerchandiserInterfaces
{
    public interface IHOrderFacade
    {
        List<string> GetKodeByNo(string no = null);
    }
}
