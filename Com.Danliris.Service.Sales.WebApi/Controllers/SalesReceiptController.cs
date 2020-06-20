using AutoMapper;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Sales.Lib.PDFTemplates;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.ViewModels.SalesReceipt;
using Com.Danliris.Service.Sales.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Sales.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/sales/sales-receipts")]
    [Authorize]

    public class SalesReceiptController : BaseController<SalesReceiptModel, SalesReceiptViewModel, ISalesReceiptContract>
    {
        private readonly ISalesReceiptContract _facade;
        private readonly static string apiVersion = "1.0";
        public SalesReceiptController(IIdentityService identityService, IValidateService validateService, ISalesReceiptContract salesReceiptFacade, IMapper mapper, IServiceProvider serviceProvider) : base(identityService, validateService, salesReceiptFacade, mapper, apiVersion)
        {
            _facade = salesReceiptFacade;
        }

        [HttpGet("salesReceiptPdf/{Id}")]
        public async Task<IActionResult> GetSalesReceiptPDF([FromRoute] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                int timeoffsset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                SalesReceiptModel model = await Facade.ReadByIdAsync(Id);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    SalesReceiptViewModel viewModel = Mapper.Map<SalesReceiptViewModel>(model);
                    var detailViewModel = viewModel.SalesReceiptDetails.FirstOrDefault();

                    SalesReceiptPdfTemplate PdfTemplate = new SalesReceiptPdfTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, detailViewModel, timeoffsset);
                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "Kuitansi - " + viewModel.SalesReceiptNo + ".pdf"
                    };
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

