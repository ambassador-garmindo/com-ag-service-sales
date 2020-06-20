using Com.Danliris.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Sales.Lib.ViewModels.Garment;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.Garment
{
    public class BudgetJobOrderDisplayLogic : BaseMonitoringLogic<BudgetJobOrderDisplayViewModel>
    {
        private IIdentityService identityService;
        private SalesDbContext dbContext;
        private DbSet<CostCalculationGarment> dbSet;

        public BudgetJobOrderDisplayLogic(IIdentityService identityService, SalesDbContext dbContext)
        {
            this.identityService = identityService;
            this.dbContext = dbContext;
            dbSet = dbContext.Set<CostCalculationGarment>();
        }

        public override IQueryable<BudgetJobOrderDisplayViewModel> GetQuery(string filter)
        {
            Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);

            IQueryable<CostCalculationGarment> Query = dbSet;

            try
            {
                var roNo = FilterDictionary["RONo"];
                Query = dbSet.Where(d => d.RO_Number == roNo);
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception(string.Concat("[RONo]", e.Message));
            }

            Query = Query.OrderBy(o => o.BuyerBrandName).ThenBy(o => o.RO_Number);

            var newQ = (from a in Query
                        join b in dbContext.CostCalculationGarment_Materials on a.Id equals b.CostCalculationGarmentId

                        select new BudgetJobOrderDisplayViewModel
                        {
                            RO_Number = a.RO_Number,
                            DeliveryDate = a.DeliveryDate,
                            Article = a.Article,
                            BuyerCode = a.BuyerCode,
                            BuyerName = a.BuyerName,
                            BrandCode = a.BuyerBrandCode,
                            BrandName = a.BuyerBrandName,
                            Quantity = a.Quantity,
                            UOMUnit = a.UOMUnit,
                            ComodityCode = a.ComodityCode,
                            ProductCode = b.ProductCode,
                            Description = b.Description,
                            BudgetQuantity = b.BudgetQuantity,
                            UomPriceName = b.UOMPriceName,
                            Price = b.Price,
                            POSerialNumber = b.PO_SerialNumber,
                        });
            return newQ;
        }
    }
}
