using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.Models.SalesInvoice;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.SalesInvoice
{
    public class SalesInvoiceFacade : ISalesInvoiceContract
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<SalesInvoiceModel> DbSet;
        private IdentityService identityService;
        private readonly IServiceProvider _serviceProvider;
        private SalesInvoiceLogic salesInvoiceLogic;
        public SalesInvoiceFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            _serviceProvider = serviceProvider;
            this.DbSet = DbContext.Set<SalesInvoiceModel>();
            this.identityService = serviceProvider.GetService<IdentityService>();
            this.salesInvoiceLogic = serviceProvider.GetService<SalesInvoiceLogic>();
        }

        public async Task<int> CreateAsync(SalesInvoiceModel model)
        {
            int result = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    int index = 0;
                    do
                    {
                        model.Code = CodeGenerator.Generate();
                    }
                    while (DbSet.Any(d => d.Code.Equals(model.Code)));

                    SalesInvoiceNumberGenerator(model, index);
                    salesInvoiceLogic.Create(model);
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
                    SalesInvoiceModel model = await salesInvoiceLogic.ReadByIdAsync(id);
                    if (model != null)
                    {
                        SalesInvoiceModel salesInvoiceModel = new SalesInvoiceModel();

                        salesInvoiceModel = model;
                        await salesInvoiceLogic.DeleteAsync(id);
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

        public ReadResponse<SalesInvoiceModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return salesInvoiceLogic.Read(page, size, order, select, keyword, filter);
        }

        public async Task<SalesInvoiceModel> ReadByIdAsync(int id)
        {
            return await salesInvoiceLogic.ReadByIdAsync(id);
        }

        public async Task<int> UpdateAsync(int id, SalesInvoiceModel model)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    salesInvoiceLogic.UpdateAsync(id, model);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return await DbContext.SaveChangesAsync();
        }

        private void SalesInvoiceNumberGenerator(SalesInvoiceModel model, int index)
        {
            SalesInvoiceModel lastData = DbSet.IgnoreQueryFilters().Where(w => w.SalesInvoiceType.Equals(model.SalesInvoiceType)).OrderByDescending(o => o.AutoIncreament).FirstOrDefault();

            int YearNow = DateTime.Now.Year;
            var YearNowString = DateTime.Now.ToString("yy");

            if (lastData == null)
            {
                if (model.SalesInvoiceType == "BNG")
                {
                    index = 28;
                }
                else if (model.SalesInvoiceType == "BAB")
                {
                    index = 8;
                }
                else if (model.SalesInvoiceType == "BNS")
                {
                    index = 98;
                }
                else if (model.SalesInvoiceType == "RNG")
                {
                    index = 14;
                }
                else if (model.SalesInvoiceType == "BRG")
                {
                    index = 28;
                }
                else if (model.SalesInvoiceType == "BAG")
                {
                    index = 8;
                }
                else if (model.SalesInvoiceType == "BGS")
                {
                    index = 98;
                }
                else if (model.SalesInvoiceType == "RRG")
                {
                    index = 14;
                }
                else if (model.SalesInvoiceType == "BLL")
                {
                    index = 8;
                }
                else if (model.SalesInvoiceType == "BPF")
                {
                    index = 98;
                }
                else if (model.SalesInvoiceType == "BSF")
                {
                    index = 14;
                }
                else if (model.SalesInvoiceType == "RPF")
                {
                    index = 28;
                }
                else if (model.SalesInvoiceType == "BPR")
                {
                    index = 8;
                }
                else if (model.SalesInvoiceType == "BSR")
                {
                    index = 98;
                }
                else if (model.SalesInvoiceType == "RPR")
                {
                    index = 14;
                }
                else if (model.SalesInvoiceType == "BAV")
                {
                    index = 8;
                }
                else if (model.SalesInvoiceType == "BON")
                {
                    index = 98;
                }
                else if (model.SalesInvoiceType == "BGM")
                {
                    index = 14;
                }
                else if (model.SalesInvoiceType == "GPF")
                {
                    index = 28;
                }
                else if (model.SalesInvoiceType == "RGF")
                {
                    index = 8;
                }
                else if (model.SalesInvoiceType == "GPR")
                {
                    index = 98;
                }
                else if (model.SalesInvoiceType == "RGR")
                {
                    index = 14;
                }
                else if (model.SalesInvoiceType == "RON")
                {
                    index = 14;
                }
                else
                {
                    index = 0;
                }
                model.AutoIncreament = 1 + index;
                model.SalesInvoiceNo = $"{YearNowString}{model.SalesInvoiceType}{model.AutoIncreament.ToString().PadLeft(6, '0')}";
            }
            else
            {
                if (YearNow > lastData.CreatedUtc.Year)
                {
                    model.AutoIncreament = 1 + index;
                    model.SalesInvoiceNo = $"{YearNowString}{model.SalesInvoiceType}{model.AutoIncreament.ToString().PadLeft(6, '0')}";
                }
                else
                {
                    model.AutoIncreament = lastData.AutoIncreament + (1 + index);
                    model.SalesInvoiceNo = $"{YearNowString}{model.SalesInvoiceType}{model.AutoIncreament.ToString().PadLeft(6, '0')}";
                }
            }
        }
    }
}
