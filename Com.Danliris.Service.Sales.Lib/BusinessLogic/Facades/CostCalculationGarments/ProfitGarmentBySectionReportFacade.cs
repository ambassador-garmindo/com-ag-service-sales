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
    public class ProfitGarmentBySectionReportFacade : IProfitGarmentBySectionReport
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<CostCalculationGarment> DbSet;
        private IdentityService IdentityService;
        private ProfitGarmentBySectionReportLogic ProfitGarmentBySectionReportLogic;

        public ProfitGarmentBySectionReportFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<CostCalculationGarment>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
            this.ProfitGarmentBySectionReportLogic = serviceProvider.GetService<ProfitGarmentBySectionReportLogic>();
        }
        
        public Tuple<MemoryStream, string> GenerateExcel(string filter = "{}")
        {

            Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);

            var Query = ProfitGarmentBySectionReportLogic.GetQuery(filter);
            var data = Query.ToList();
            DataTable result = new DataTable();
            var offset = 7;

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Seksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Agent", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Agent", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Brand", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Brand", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Article", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Deskripsi Garment", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Shipment", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Profit %", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Order", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Confirm Price", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "CM Price", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "FOB Price", DataType = typeof(String) });
            
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();
            if (Query.ToArray().Count() == 0)
                     result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                    int index = 0;
                    foreach (ProfitGarmentBySectionReportViewModel item in data)
                    {
                        index++;

                        string ShipDate = item.DeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.DeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        string QtyOrder = string.Format("{0:N2}", item.Quantity);
                        string CfrmPrc = string.Format("{0:N4}", item.ConfirmPrice);
                        string PrftGmt = string.Format("{0:N2}", item.Profit);
                        string CMPrc1 = string.Format("{0:N4}", item.CMPrice);
                        string FOBPrc = string.Format("{0:N4}", item.ConfirmPrice + item.CMPrice);

                        result.Rows.Add(index, item.RO_Number, item.Section, item.UnitName, item.BuyerCode, item.BuyerName, item.BrandCode, item.BrandName, item.Article,
                                        item.Comodity, item.ComodityDescription, ShipDate, PrftGmt, QtyOrder, item.UOMUnit, CfrmPrc, CMPrc1, FOBPrc);
                }
            }
            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Profit Garment By Seksi");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            MemoryStream streamExcel = new MemoryStream();
            package.SaveAs(streamExcel);

            string fileName = string.Concat("Profit Garment Per Seksi", ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }

        public Tuple<List<ProfitGarmentBySectionReportViewModel>, int> Read(int page = 1, int size = 25, string filter = "{}")
        {
            var Query = ProfitGarmentBySectionReportLogic.GetQuery(filter);
            var data = Query.ToList();

            int TotalData = data.Count();
            return Tuple.Create(data, TotalData);
        }     
    }
}
