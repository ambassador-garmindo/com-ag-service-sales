using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using Com.Danliris.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Danliris.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.CostCalculationGarments;
using Com.Danliris.Service.Sales.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Com.Danliris.Service.Sales.Lib.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Com.Moonlay.NetCore.Lib;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.CostCalculationGarments
{
    public class ProfitGarmentByComodityReportFacade : IProfitGarmentByComodityReport
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<CostCalculationGarment> DbSet;
        private IdentityService IdentityService;
        private ProfitGarmentByComodityReportLogic ProfitGarmentByComodityReportLogic;

        public ProfitGarmentByComodityReportFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<CostCalculationGarment>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
            this.ProfitGarmentByComodityReportLogic = serviceProvider.GetService<ProfitGarmentByComodityReportLogic>();
        }
        
        public Tuple<MemoryStream, string> GenerateExcel(string filter = "{}")
        {

            Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);

            var Query = ProfitGarmentByComodityReportLogic.GetQuery(filter);
            var data = Query.ToList();
            DataTable result = new DataTable();
       
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Order", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Amount", DataType = typeof(String) });

            Dictionary<string, string> Rowcount = new Dictionary<string, string>();
            if (Query.ToArray().Count() == 0)
                     result.Rows.Add("", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                Dictionary<string, List<ProfitGarmentByComodityReportViewModel>> dataByUOM = new Dictionary<string, List<ProfitGarmentByComodityReportViewModel>>();

                Dictionary<string, double> subTotalAmount = new Dictionary<string, double>();

                foreach (ProfitGarmentByComodityReportViewModel item in Query.ToList())
                {
                    string BgtUOM = item.UOMUnit;

                    if (!dataByUOM.ContainsKey(BgtUOM)) dataByUOM.Add(BgtUOM, new List<ProfitGarmentByComodityReportViewModel> { });
                    dataByUOM[BgtUOM].Add(new ProfitGarmentByComodityReportViewModel
                    {
                        ComodityCode = item.ComodityCode,
                        ComodityName = item.ComodityName,
                        Quantity = item.Quantity,
                        UOMUnit = item.UOMUnit,
                        Amount = item.Amount,                        
                    });

                    if (!subTotalAmount.ContainsKey(BgtUOM))
                    {
                        subTotalAmount.Add(BgtUOM, 0);
                    };

                    subTotalAmount[BgtUOM] += item.Amount;
                }

                double totalAmount = 0;

                int rowPosition = 1;

                foreach (KeyValuePair<string, List<ProfitGarmentByComodityReportViewModel>> UoM in dataByUOM)
                {
                    string U_o_M = "";

                    int index = 0;
                    foreach (ProfitGarmentByComodityReportViewModel item in UoM.Value)
                    {
                        index++;

                        string TotQty = string.Format("{0:N2}", item.Quantity);
                        string TotAmt = string.Format("{0:N2}", item.Amount);

                        result.Rows.Add(index, item.ComodityCode, item.ComodityName, TotQty, item.UOMUnit, TotAmt);

                        rowPosition += 1;
                        U_o_M = item.UOMUnit;
                    }
                    result.Rows.Add("", "", "", "SUB TOTAL", U_o_M, Math.Round(subTotalAmount[UoM.Key], 2));

                    rowPosition += 1;
                    totalAmount += subTotalAmount[UoM.Key];
                }
                result.Rows.Add("", "", "", "", "T O T A L", Math.Round(totalAmount, 2));
                rowPosition += 1;
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Profit Garment By Komoditi");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            MemoryStream streamExcel = new MemoryStream();
            package.SaveAs(streamExcel);

            string fileName = string.Concat("Profit Garment Per Komoditi", ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }

        public Tuple<List<ProfitGarmentByComodityReportViewModel>, int> Read(int page = 1, int size = 25, string filter = "{}")
        {
            var Query = ProfitGarmentByComodityReportLogic.GetQuery(filter);
            var data = Query.ToList();

            int TotalData = data.Count();
            return Tuple.Create(data, TotalData);
        }     
    }
}
