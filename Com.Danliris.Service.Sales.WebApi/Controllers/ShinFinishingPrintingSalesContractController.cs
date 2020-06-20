using AutoMapper;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.Models.FinishingPrinting;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.ViewModels.FinishingPrinting;
using Com.Danliris.Service.Sales.WebApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Com.Danliris.Service.Sales.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Sales.Lib.PDFTemplates;
using System.IO;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.ViewModels.FinishingPrintingCostCalculation;
using Com.Danliris.Service.Sales.Lib.Utilities;

namespace Com.Danliris.Service.Sales.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/sales/finishing-printing-sales-contracts/shin")]
    [Authorize]
    public class ShinFinishingPrintingSalesContractController : BaseController<FinishingPrintingSalesContractModel, ShinFinishingPrintingSalesContractViewModel, IShinFinishingPrintingSalesContractFacade>
    {
        private readonly static string apiVersion = "1.0";
        private readonly IHttpClientService HttpClientService;
        private readonly IFinishingPrintingCostCalculationService finishingPrintingCostCalculationService;
        private readonly IFinishingPrintingPreSalesContractFacade fpPreSalesContractFacade;
        public ShinFinishingPrintingSalesContractController(IIdentityService identityService, IValidateService validateService, IShinFinishingPrintingSalesContractFacade facade, IMapper mapper, IServiceProvider serviceProvider) : base(identityService, validateService, facade, mapper, apiVersion)
        {
            HttpClientService = serviceProvider.GetService<IHttpClientService>();
            finishingPrintingCostCalculationService = serviceProvider.GetService<IFinishingPrintingCostCalculationService>();
            fpPreSalesContractFacade = serviceProvider.GetService<IFinishingPrintingPreSalesContractFacade>();
        }


        public override IActionResult Get(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                ValidateUser();

                ReadResponse<FinishingPrintingSalesContractModel> read = Facade.Read(page, size, order, select, keyword, filter);

                //Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> Data = Facade.Read(page, size, order, select, keyword, filter);
                List<ShinFinishingPrintingSalesContractViewModel> DataVM = Mapper.Map<List<ShinFinishingPrintingSalesContractViewModel>>(read.Data);

                foreach(var data in DataVM)
                {
                    var fpCCModel = finishingPrintingCostCalculationService.ReadParent(data.CostCalculation.Id).Result;
                    if(fpCCModel != null)
                    {

                        var fpCCVM = Mapper.Map<FinishingPrintingCostCalculationViewModel>(fpCCModel);
                        var preSalesContractModel = fpPreSalesContractFacade.ReadByIdAsync((int)fpCCVM.PreSalesContract.Id).Result;
                        fpCCVM.PreSalesContract = Mapper.Map<FinishingPrintingPreSalesContractViewModel>(preSalesContractModel);
                        data.CostCalculation = fpCCVM;
                    }
                }

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok<ShinFinishingPrintingSalesContractViewModel>(Mapper, DataVM, page, size, read.Count, DataVM.Count, read.Order, read.Selected);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        public override async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var model = await Facade.ReadByIdAsync(id);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    ShinFinishingPrintingSalesContractViewModel viewModel = Mapper.Map<ShinFinishingPrintingSalesContractViewModel>(model);
                    var fpCCModel = await finishingPrintingCostCalculationService.ReadParent(viewModel.CostCalculation.Id);

                    if(fpCCModel != null)
                    {

                        var fpCCVM = Mapper.Map<FinishingPrintingCostCalculationViewModel>(fpCCModel);
                        var preSalesContractModel = await fpPreSalesContractFacade.ReadByIdAsync((int)fpCCVM.PreSalesContract.Id);
                        fpCCVM.PreSalesContract = Mapper.Map<FinishingPrintingPreSalesContractViewModel>(preSalesContractModel);
                        viewModel.CostCalculation = fpCCVM;
                    }

                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                        .Ok(viewModel);
                    return Ok(Result);
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPDF([FromRoute] int Id)
        {
           
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                int timeoffsset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                FinishingPrintingSalesContractModel model = await Facade.ReadByIdAsync(Id);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    string BuyerUri = "master/buyers";
                    string BankUri = "master/account-banks";
                    //string CurrenciesUri = "master/currencies";
                    string Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");


                    FinishingPrintingSalesContractViewModel viewModel = Mapper.Map<FinishingPrintingSalesContractViewModel>(model);

                    /* Get Buyer */
                    var response = HttpClientService.GetAsync($@"{APIEndpoint.Core}{BuyerUri}/" + viewModel.Buyer.Id).Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                    object json;
                    if (result.TryGetValue("data", out json))
                    {
                        Dictionary<string, object> buyer = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
                        viewModel.Buyer.City = buyer.TryGetValue("City", out json) ? (json != null ? json.ToString() : "") : "";
                        viewModel.Buyer.Address = buyer.TryGetValue("Address", out json) ? (json != null ? json.ToString() : "") : "";
                        viewModel.Buyer.Contact = buyer.TryGetValue("Contact", out json) ? (json != null ? json.ToString() : "") : "";
                        viewModel.Buyer.Country = buyer.TryGetValue("Country", out json) ? (json != null ? json.ToString() : "") : "";
                    }

                    /* Get Agent */
                    var responseAgent = HttpClientService.GetAsync($@"{APIEndpoint.Core}{BuyerUri}/" + viewModel.Agent.Id).Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> resultAgent = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseAgent.Result);
                    object jsonAgent;
                    if (resultAgent.TryGetValue("data", out jsonAgent))
                    {
                        Dictionary<string, object> agent = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonAgent.ToString());
                        viewModel.Agent.City = agent.TryGetValue("City", out jsonAgent) ? (jsonAgent != null ? jsonAgent.ToString() : "") : "";
                        viewModel.Agent.Address = agent.TryGetValue("Address", out jsonAgent) ? (jsonAgent != null ? jsonAgent.ToString() : "") : "";
                        viewModel.Agent.Contact = agent.TryGetValue("Contact", out jsonAgent) ? (jsonAgent != null ? jsonAgent.ToString() : "") : "";
                        viewModel.Agent.Country = agent.TryGetValue("Country", out jsonAgent) ? (jsonAgent != null ? jsonAgent.ToString() : "") : "";
                    }

                    /* Get AccountBank */
                    var responseBank = HttpClientService.GetAsync($@"{APIEndpoint.Core}{BankUri}/" + viewModel.AccountBank.Id).Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> resultBank = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBank.Result);
                    object jsonBank;
                    if (resultBank.TryGetValue("data", out jsonBank))
                    {
                        Dictionary<string, object> bank = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBank.ToString());
                        var currencyBankObj = new CurrencyViewModel();
                        object objResult = new object();
                        if (bank.TryGetValue("Currency", out objResult))
                        {
                            currencyBankObj = JsonConvert.DeserializeObject<CurrencyViewModel>(objResult.ToString());
                        }
                        viewModel.AccountBank.BankAddress = bank.TryGetValue("BankAddress", out objResult) ? (objResult != null ? objResult.ToString() : "") : "";
                        viewModel.AccountBank.SwiftCode = bank.TryGetValue("SwiftCode", out objResult) ? (objResult != null ? objResult.ToString() : "") : "";

                        viewModel.AccountBank.Currency = new CurrencyViewModel();
                        viewModel.AccountBank.Currency.Description = currencyBankObj.Description;
                        viewModel.AccountBank.Currency.Symbol = currencyBankObj.Symbol;
                        viewModel.AccountBank.Currency.Rate = currencyBankObj.Rate;
                        viewModel.AccountBank.Currency.Code = currencyBankObj.Code;

                    }

                    if (viewModel.Buyer.Type != "Ekspor")
                    {
                        FinishingPrintingSalesContractPDFTemplate PdfTemplate = new FinishingPrintingSalesContractPDFTemplate();
                        MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, timeoffsset);
                        return new FileStreamResult(stream, "application/pdf")
                        {
                            FileDownloadName = "finishing printing sales contract (id)" + viewModel.SalesContractNo + ".pdf"
                        };
                    }
                    else
                    {
                        FinishingPrintingSalesContractExportPDFTemplate PdfTemplate = new FinishingPrintingSalesContractExportPDFTemplate();
                        MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, timeoffsset);
                        return new FileStreamResult(stream, "application/pdf")
                        {
                            FileDownloadName = "finishing printing sales contract (en) " + viewModel.SalesContractNo + ".pdf"
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
