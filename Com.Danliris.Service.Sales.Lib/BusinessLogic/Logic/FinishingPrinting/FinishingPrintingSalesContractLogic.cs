﻿using Com.Danliris.Service.Sales.Lib.Models.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.Utilities;
using Com.Danliris.Service.Sales.Lib.Utilities.BaseClass;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.FinishingPrinting
{
    public class FinishingPrintingSalesContractLogic : BaseLogic<FinishingPrintingSalesContractModel>
    {
        private FinishingPrintingSalesContractDetailLogic FinishingPrintingSalesContractDetailLogic;
        public FinishingPrintingSalesContractLogic(FinishingPrintingSalesContractDetailLogic finishingPrintingSalesContractDetailLogic, IServiceProvider serviceProvider, IIdentityService identityService, SalesDbContext dbContext) : base(identityService, serviceProvider, dbContext)
        {
            this.FinishingPrintingSalesContractDetailLogic = finishingPrintingSalesContractDetailLogic;
        }

        public override ReadResponse<FinishingPrintingSalesContractModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<FinishingPrintingSalesContractModel> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "SalesContractNo", "Buyer.Type", "Buyer.Name"
            };

            Query = QueryHelper<FinishingPrintingSalesContractModel>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<FinishingPrintingSalesContractModel>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Buyer", "DeliverySchedule", "SalesContractNo", "LastModifiedUtc"
            };

            Query = Query
                .Select(field => new FinishingPrintingSalesContractModel
                {
                    Id = field.Id,
                    Code = field.Code,
                    SalesContractNo = field.SalesContractNo,
                    BuyerType = field.BuyerType,
                    BuyerName = field.BuyerName,
                    DeliverySchedule = field.DeliverySchedule,
                    LastModifiedUtc = field.LastModifiedUtc
                });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<FinishingPrintingSalesContractModel>.Order(Query, OrderDictionary);


            Pageable<FinishingPrintingSalesContractModel> pageable = new Pageable<FinishingPrintingSalesContractModel>(Query, page - 1, size);
            List<FinishingPrintingSalesContractModel> data = pageable.Data.ToList<FinishingPrintingSalesContractModel>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<FinishingPrintingSalesContractModel>(data, totalData, OrderDictionary, SelectedFields);
        }

        public override void Create(FinishingPrintingSalesContractModel model)
        {
            SalesContractNumberGenerator(model);
            foreach (var detail in model.Details)
            {
                FinishingPrintingSalesContractDetailLogic.Create(detail);
                //EntityExtension.FlagForCreate(detail, IdentityService.Username, "sales-service");
            }

            EntityExtension.FlagForCreate(model, IdentityService.Username, "sales-service");
            DbSet.Add(model);
        }

        public override async Task<FinishingPrintingSalesContractModel> ReadByIdAsync(int id)
        {
            var finishingPrintingSalesContract = await DbSet.Include(p => p.Details).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
            finishingPrintingSalesContract.Details = finishingPrintingSalesContract.Details.OrderBy(s => s.Id).ToArray();
            return finishingPrintingSalesContract;
        }

        public override async void Update(int id, FinishingPrintingSalesContractModel model)
        {
            if (model.Details != null)
            {
                HashSet<long> detailIds = FinishingPrintingSalesContractDetailLogic.GetFPSalesContractIds(id);
                foreach (var itemId in detailIds)
                {
                    FinishingPrintingSalesContractDetailModel data = model.Details.FirstOrDefault(prop => prop.Id.Equals(itemId));
                    if (data == null)
                        await FinishingPrintingSalesContractDetailLogic.DeleteAsync(Convert.ToInt32(itemId));
                    else
                    {
                        FinishingPrintingSalesContractDetailLogic.Update(Convert.ToInt32(itemId), data);
                    }

                    foreach (FinishingPrintingSalesContractDetailModel item in model.Details)
                    {
                        if (item.Id == 0)
                            FinishingPrintingSalesContractDetailLogic.Create(item);
                    }
                }
            }

            EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
            DbSet.Update(model);
        }

        public override async Task DeleteAsync(int id)
        {
            var model = await ReadByIdAsync(id);

            foreach (var Detail in model.Details)
            {
                EntityExtension.FlagForDelete(Detail, IdentityService.Username, "sales-service");
            }

            EntityExtension.FlagForDelete(model, IdentityService.Username, "sales-service", true);
            DbSet.Update(model);
        }

        private void SalesContractNumberGenerator(FinishingPrintingSalesContractModel model)
        {
            FinishingPrintingSalesContractModel lastData = DbSet.IgnoreQueryFilters().Where(w => w.OrderTypeName.Equals(model.OrderTypeName)).OrderByDescending(o => o.AutoIncrementNumber).FirstOrDefault();

            string DocumentType = model.BuyerType.ToLower().Equals("ekspor") || model.BuyerType.ToLower().Equals("export") ? "FPE" : "FPL";

            int YearNow = DateTime.Now.Year;
            int MonthNow = DateTime.Now.Month;

            if (lastData == null)
            {
                model.AutoIncrementNumber = 1;
                model.SalesContractNo = $"0001/{DocumentType}/{MonthNow}/{YearNow}";
            }
            else
            {
                if (YearNow > lastData.CreatedUtc.Year)
                {
                    model.AutoIncrementNumber = 1;
                    model.SalesContractNo = $"0001/{DocumentType}/{MonthNow}/{YearNow}";
                }
                else
                {
                    model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    model.SalesContractNo = $"{lastData.AutoIncrementNumber.ToString().PadLeft(4, '0')}/{DocumentType}/{MonthNow}/{YearNow}";
                }
            }
        }
    }
}
