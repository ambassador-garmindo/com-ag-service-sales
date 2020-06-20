using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Sales.Lib.ViewModels.IntegrationViewModel
{
    public class DOSalesViewModel
    {
        public long Id { get; set; }
        [MaxLength(250)]
        public string Code { get; set; }

        public MaterialViewModel Material { get; set; }

        public MaterialConstructionViewModel MaterialConstruction { get; set; }
        public BuyerViewModel Buyer { get; set; }

        //public UomViewModel PackingUom { get; set; }
    }
}
