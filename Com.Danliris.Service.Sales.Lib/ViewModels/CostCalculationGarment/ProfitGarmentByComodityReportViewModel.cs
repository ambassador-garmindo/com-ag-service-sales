using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Sales.Lib.ViewModels.CostCalculationGarment
{
    public class ProfitGarmentByComodityReportViewModel 
    {
        public int count { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityName { get; set; }
        public int Quantity { get; set; }
        public string UOMUnit { get; set; }        
        public double Amount { get; set; }               
    }
}
