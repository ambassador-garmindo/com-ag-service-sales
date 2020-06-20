using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.SalesReceipt
{
    public class SalesReceiptFacade : ISalesReceiptContract
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<SalesReceiptModel> DbSet;
        private IdentityService identityService;
        private readonly IServiceProvider _serviceProvider;
        private SalesReceiptLogic salesReceiptLogic;
        public SalesReceiptFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            _serviceProvider = serviceProvider;
            this.DbSet = DbContext.Set<SalesReceiptModel>();
            this.identityService = serviceProvider.GetService<IdentityService>();
            this.salesReceiptLogic = serviceProvider.GetService<SalesReceiptLogic>();
        }

        public async Task<int> CreateAsync(SalesReceiptModel model)
        {
            int result = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    int index = 0;
                    foreach (var item in model.SalesReceiptDetails)
                    {
                        var updateTotalPaid = DbContext.SalesInvoices.FirstOrDefault(x => x.Id == item.SalesInvoiceId);
                        updateTotalPaid.TotalPaid = item.Paid;
                    }

                    do
                    {
                        model.Code = CodeGenerator.Generate();
                    }
                    while (DbSet.Any(d => d.Code.Equals(model.Code)));

                    SalesReceiptNumberGenerator(model, index);
                    salesReceiptLogic.Create(model);
                    index++;

                    result = await DbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return result;
        }

        public async Task<int> DeleteAsync(int id)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    SalesReceiptModel model = await salesReceiptLogic.ReadByIdAsync(id);
                    if (model != null)
                    {
                        SalesReceiptModel salesReceiptModel = new SalesReceiptModel();

                        foreach (var item in model.SalesReceiptDetails)
                        {
                            var updateTotalPaid = DbContext.SalesInvoices.FirstOrDefault(x => x.Id == item.SalesInvoiceId);
                            updateTotalPaid.TotalPaid = updateTotalPaid.TotalPaid - item.Nominal;
                        }

                        salesReceiptModel = model;
                        await salesReceiptLogic.DeleteAsync(id);
                    }
                }
                catch (Exception e)
                {

                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<SalesReceiptModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return salesReceiptLogic.Read(page, size, order, select, keyword, filter);
        }

        public async Task<SalesReceiptModel> ReadByIdAsync(int id)
        {
            return await salesReceiptLogic.ReadByIdAsync(id);
        }

        public async Task<int> UpdateAsync(int id, SalesReceiptModel model)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in model.SalesReceiptDetails)
                    {
                        var updateTotalPaid = DbContext.SalesInvoices.FirstOrDefault(x => x.Id == item.SalesInvoiceId);
                        updateTotalPaid.TotalPaid = item.Paid;
                    }

                    salesReceiptLogic.UpdateAsync(id, model);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return await DbContext.SaveChangesAsync();
        }

        private void SalesReceiptNumberGenerator(SalesReceiptModel model, int index)
        {
            SalesReceiptModel lastData = DbSet.IgnoreQueryFilters().Where(w => w.SalesReceiptType.Equals(model.SalesReceiptType)).OrderByDescending(o => o.AutoIncreament).FirstOrDefault();

            int YearNow = DateTime.Now.Year;

            if (lastData == null)
            {
                if (model.SalesReceiptType == "A")
                {
                    index = 28;
                }
                else if (model.SalesReceiptType == "B")
                {
                    index = 8;
                }
                else if (model.SalesReceiptType == "C")
                {
                    index = 98;
                }
                else if (model.SalesReceiptType == "D")
                {
                    index = 14;
                }
                else
                {
                    index = 0;
                }
                model.AutoIncreament = 1 + index;
                model.SalesReceiptNo = $"{model.SalesReceiptType}/{YearNow}/{model.AutoIncreament.ToString().PadLeft(6, '0')}";
            }
            else
            {
                if (YearNow > lastData.CreatedUtc.Year)
                {
                    model.AutoIncreament = 1 + index;
                    model.SalesReceiptNo = $"{model.SalesReceiptType}/{YearNow}/{model.AutoIncreament.ToString().PadLeft(6, '0')}";
                }
                else
                {
                    model.AutoIncreament = lastData.AutoIncreament + (1 + index);
                    model.SalesReceiptNo = $"{model.SalesReceiptType}/{YearNow}/{model.AutoIncreament.ToString().PadLeft(6, '0')}";
                }
            }
        }
    }
}

