using Com.Danliris.Service.Sales.Lib.Models.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Utilities.BaseInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrintingCostCalculation
{
    public interface IFinishingPrintingCostCalculationService : IBaseFacade<FinishingPrintingCostCalculationModel>
    {
        Task<int> CCPost(List<long> listId);
        Task<FinishingPrintingCostCalculationModel> ReadParent(long id);
        Task<int> CCApproveMD(long id);
        Task<int> CCApprovePPIC(long id);
        Task<bool> ValidatePreSalesContractId(long id);
    }
}
