﻿using AutoMapper;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.Garment;
using Com.Danliris.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.Utilities;
using Com.Danliris.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Danliris.Service.Sales.Lib.ViewModels.Garment;
using Com.Danliris.Service.Sales.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Sales.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/merchandiser/ro-garment-validations")]
    [Authorize]
    public class RO_Garment_ValidationController : Controller
    {
        private readonly string ApiVersion = "1.0.0";
        private IRO_Garment_Validation facade;
        protected IIdentityService IdentityService;
        protected readonly IValidateService ValidateService;
        protected readonly IMapper Mapper;

        public RO_Garment_ValidationController(IRO_Garment_Validation facade, IIdentityService identityService, IValidateService validateService, IMapper mapper)
        {
            this.facade = facade;
            this.IdentityService = identityService;
            this.ValidateService = validateService;
            this.Mapper = mapper;
        }

        private void ValidateUser()
        {
            IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
        }

        private void ValidateViewModel(CostCalculationGarment_RO_Garment_ValidationViewModel viewModel)
        {
            ValidateService.Validate(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CostCalculationGarment_RO_Garment_ValidationViewModel viewModel)
        {
            try
            {
                ValidateUser();
                ValidateViewModel(viewModel);

                var model = Mapper.Map<CostCalculationGarment>(viewModel);

                await facade.ValidateROGarment(model);
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok();
                return Ok(Result);
            }
            catch (ServiceValidationException e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.BAD_REQUEST_STATUS_CODE, Common.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
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