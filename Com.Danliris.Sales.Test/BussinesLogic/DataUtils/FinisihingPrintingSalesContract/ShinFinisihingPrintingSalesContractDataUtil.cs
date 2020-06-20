using Com.Danliris.Sales.Test.BussinesLogic.DataUtils.FinishingPrintingCostCalculation;
using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Models.FinishingPrinting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Sales.Test.BussinesLogic.DataUtils.FinisihingPrintingSalesContract
{
    public class ShinFinisihingPrintingSalesContractDataUtil : BaseDataUtil<ShinFinishingPrintingSalesContractFacade, FinishingPrintingSalesContractModel>
    {
        private readonly FinishingPrintingCostCalculationFacade finishingPrintingCostCalculationFacade;
        public ShinFinisihingPrintingSalesContractDataUtil(ShinFinishingPrintingSalesContractFacade facade, FinishingPrintingCostCalculationFacade costCalculationFacade) : base(facade)
        {
            finishingPrintingCostCalculationFacade = costCalculationFacade;
        }

        public override async Task<FinishingPrintingSalesContractModel> GetNewData()
        {
            FinishingPrintingCostCalculationDataUtils ccDU = new FinishingPrintingCostCalculationDataUtils(finishingPrintingCostCalculationFacade);
            var ccData = await ccDU.GetTestData();

            return new FinishingPrintingSalesContractModel()
            {
                AgentCode = "c",
                AgentID = 1,
                AgentName = "name",
                Amount = 1,
                CostCalculationId = ccData.Id,
                DesignMotiveID = 1,
                SalesContractNo = "np",
                UnitName = "np",
                BuyerName = "a00",
                ProductionOrderNo = "no",
                AccountBankAccountName = "a",
                AccountBankCode = "a",
                AccountBankCurrencyCode = "a",
                AccountBankCurrencyID = 1,
                AccountBankCurrencyRate = 1,
                AccountBankCurrencySymbol = "a",
                AccountBankID = 1,
                AccountBankName = "name",
                AccountBankNumber = "nm",
                Details = new List<FinishingPrintingSalesContractDetailModel>()
                {
                    new FinishingPrintingSalesContractDetailModel()
                    {
                        Color = "c",
                        UseIncomeTax = true,
                        Price = 1,
                        ScreenCost = 1
                    }
                },
                BuyerType = "type"
            };

        }
    }
}
