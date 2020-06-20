using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Sales.Lib.Utilities.BaseClass
{
    public abstract class BaseMonitoringLogic<TViewModel>
    {
        public abstract IQueryable<TViewModel> GetQuery(string filterString);
    }
}
