using AutoMapper;
using Com.Danliris.Sales.Test.BussinesLogic.DataUtils.GarmentMasterPlan.WeeklyPlanDataUtils;
using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib;
using Com.Danliris.Service.Sales.Lib.AutoMapperProfiles.GarmentMasterPlanProfiles;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.GarmentMasterPlan.WeeklyPlanFacades;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.GarmentMasterPlan.WeeklyPlanLogics;
using Com.Danliris.Service.Sales.Lib.Models.GarmentMasterPlan.WeeklyPlanModels;
using Com.Danliris.Service.Sales.Lib.ViewModels.Garment.WeeklyPlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Sales.Test.BussinesLogic.Facades.GarmentMasterPlan.WeeklyPlanFacadeTests
{
    public class WeeklyPlanFacadeTest : BaseFacadeTest<SalesDbContext, WeeklyPlanFacade, WeeklyPlanLogic, GarmentWeeklyPlan, WeeklyPlanDataUtil>
    {
        private const string ENTITY = "GarmentWeeklyPlan";

        public WeeklyPlanFacadeTest() : base(ENTITY)
        {
        }

        [Fact]
        public void Mapping_With_AutoMapper_Profiles()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GarmentWeeklyPlanMapper>();
            });
            var mapper = configuration.CreateMapper();

            GarmentWeeklyPlanViewModel vm = new GarmentWeeklyPlanViewModel { Id = 1 };
            GarmentWeeklyPlan model = mapper.Map<GarmentWeeklyPlan>(vm);

            Assert.Equal(vm.Id, model.Id);

        }
    }
}
