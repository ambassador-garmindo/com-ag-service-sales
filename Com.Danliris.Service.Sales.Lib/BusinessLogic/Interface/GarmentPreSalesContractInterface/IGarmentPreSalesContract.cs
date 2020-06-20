using Com.Danliris.Service.Sales.Lib.Models.GarmentPreSalesContractModel;
using Com.Danliris.Service.Sales.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Sales.Lib.ViewModels.GarmentPreSalesContractViewModels;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.GarmentPreSalesContractInterface
{
    public interface IGarmentPreSalesContract : IBaseFacade<GarmentPreSalesContract>
    {
        Task<int> Patch(long id, JsonPatchDocument<GarmentPreSalesContract> jsonPatch);
        Task<int> PreSalesPost(List<long> listId, string user);
        Task<int> PreSalesUnpost(long id, string user);
    }
}