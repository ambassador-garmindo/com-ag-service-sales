using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Sales.Lib.Models.SalesReceipt;
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
using System.Threading.Tasks;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.SalesReceipt
{
    public class SalesReceiptLogic : BaseLogic<SalesReceiptModel>
    {
        private SalesReceiptDetailLogic salesReceiptDetailLogic;

        public SalesReceiptLogic(IServiceProvider serviceProvider, IIdentityService identityService, SalesDbContext dbContext) : base(identityService, serviceProvider, dbContext)
        {
            this.salesReceiptDetailLogic = serviceProvider.GetService<SalesReceiptDetailLogic>();
        }

        public override ReadResponse<SalesReceiptModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<SalesReceiptModel> Query = DbSet.Include(x => x.SalesReceiptDetails);

            List<string> SearchAttributes = new List<string>()
            {
                "SalesReceiptNo","BankCode","BuyerName"
            };

            Query = QueryHelper<SalesReceiptModel>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<SalesReceiptModel>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                "Id","Code","SalesReceiptNo","SalesReceiptType","SalesReceiptDate","BankId","AccountCOA","AccountName","AccountNumber","BankName","BankCode","BuyerId","BuyerName","BuyerAddress","TotalPaid","SalesReceiptDetails"
            };

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<SalesReceiptModel>.Order(Query, OrderDictionary);

            Pageable<SalesReceiptModel> pageable = new Pageable<SalesReceiptModel>(Query, page - 1, size);
            List<SalesReceiptModel> data = pageable.Data.ToList<SalesReceiptModel>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<SalesReceiptModel>(data, totalData, OrderDictionary, SelectedFields);
        }

        public override async void UpdateAsync(long id, SalesReceiptModel model)
        {
            try
            {
                if (model.SalesReceiptDetails != null)
                {
                    HashSet<long> detailIds = salesReceiptDetailLogic.GetIds(id);
                    foreach (var itemId in detailIds)
                    {
                        SalesReceiptDetailModel data = model.SalesReceiptDetails.FirstOrDefault(prop => prop.Id.Equals(itemId));
                        if (data == null)
                            await salesReceiptDetailLogic.DeleteAsync(itemId);
                        else
                        {
                            salesReceiptDetailLogic.UpdateAsync(itemId, data);
                        }
                    }

                    foreach (SalesReceiptDetailModel item in model.SalesReceiptDetails)
                    {
                        if (item.Id == 0)
                            salesReceiptDetailLogic.Create(item);
                    }
                }

                EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
                DbSet.Update(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Create(SalesReceiptModel model)
        {
            if (model.SalesReceiptDetails.Count > 0)
            {
                foreach (var detail in model.SalesReceiptDetails)
                {
                    salesReceiptDetailLogic.Create(detail);
                }
            }

            EntityExtension.FlagForCreate(model, IdentityService.Username, "sales-service");
            DbSet.Add(model);
        }

        public override async Task DeleteAsync(long id)
        {

            SalesReceiptModel model = await ReadByIdAsync(id);

            foreach (var detail in model.SalesReceiptDetails)
            {
                await salesReceiptDetailLogic.DeleteAsync(detail.Id);
            }

            EntityExtension.FlagForDelete(model, IdentityService.Username, "sales-service", true);
            DbSet.Update(model);
        }

        public override async Task<SalesReceiptModel> ReadByIdAsync(long id)
        {
            var SalesReceipt = await DbSet.Where(p => p.SalesReceiptDetails.Select(d => d.SalesReceiptModel.Id)
            .Contains(p.Id)).Include(p => p.SalesReceiptDetails).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
            SalesReceipt.SalesReceiptDetails = SalesReceipt.SalesReceiptDetails.OrderBy(s => s.Id).ToArray();
            return SalesReceipt;
        }
    }
}

