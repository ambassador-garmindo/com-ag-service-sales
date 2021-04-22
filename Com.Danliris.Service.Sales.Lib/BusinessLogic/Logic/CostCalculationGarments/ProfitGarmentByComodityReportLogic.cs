using Com.Danliris.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.CostCalculationGarments
{
    public class ProfitGarmentByComodityReportLogic : BaseMonitoringLogic<ProfitGarmentByComodityReportViewModel>
    {
        private IIdentityService identityService;
        private SalesDbContext dbContext;
        private DbSet<CostCalculationGarment> dbSet;

        public ProfitGarmentByComodityReportLogic(IIdentityService identityService, SalesDbContext dbContext)
        {
            this.identityService = identityService;
            this.dbContext = dbContext;
            dbSet = dbContext.Set<CostCalculationGarment>();
        }

        public override IQueryable<ProfitGarmentByComodityReportViewModel> GetQuery(string filter)
        {
            Filter _filter = JsonConvert.DeserializeObject<Filter>(filter);

            IQueryable<CostCalculationGarment> Query = dbSet;

            if (_filter.dateFrom != null)
            {
                var filterDate = _filter.dateFrom.GetValueOrDefault().ToOffset(TimeSpan.FromHours(identityService.TimezoneOffset)).Date;
                Query = Query.Where(cc => cc.DeliveryDate.AddHours(identityService.TimezoneOffset).Date >= filterDate);
            }
            if (_filter.dateTo != null)
            {
                var filterDate = _filter.dateTo.GetValueOrDefault().ToOffset(TimeSpan.FromHours(identityService.TimezoneOffset)).AddDays(1).Date;
                Query = Query.Where(cc => cc.DeliveryDate.AddHours(identityService.TimezoneOffset).Date < filterDate);
            }
 
            Query = Query.OrderBy(o => o.ComodityCode);
            var newQ = (from a in Query
                        where a.IsApprovedKadivMD == true
                        group new { Qty = a.Quantity, Amt = a.Quantity * a.ConfirmPrice } by new { a.ComodityCode, a.Commodity, a.UOMUnit } into G

                        select new ProfitGarmentByComodityReportViewModel
                        {                
                            ComodityCode = G.Key.ComodityCode,
                            ComodityName = G.Key.Commodity,
                            Quantity = G.Sum(m => m.Qty),
                            Amount = Math.Round(G.Sum(m => m.Amt), 2),
                            UOMUnit = G.Key.UOMUnit,
                        });
            return newQ;
        }

        private class Filter
        {
            public DateTimeOffset? dateFrom { get; set; }
            public DateTimeOffset? dateTo { get; set; }
        }
    }
}
