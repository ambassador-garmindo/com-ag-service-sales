using AutoMapper;
using Com.Danliris.Sales.Test.BussinesLogic.DataUtils.GarmentOmzetTargetDataUtils;
using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib;
using Com.Danliris.Service.Sales.Lib.AutoMapperProfiles.GarmentOmzetTargetProfiles;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades.GarmentOmzetTargetFacades;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Logic.GarmentOmzetTargetLogics;
using Com.Danliris.Service.Sales.Lib.Models.GarmentOmzetTargetModel;
using Com.Danliris.Service.Sales.Lib.Services;
using Com.Danliris.Service.Sales.Lib.ViewModels.GarmentOmzetTargetViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Sales.Test.BussinesLogic.Facades.GarmentOmzetTargetTest
{
    public class GarmentOmzetTargetTest : BaseFacadeTest<SalesDbContext, GarmentOmzetTargetFacade, GarmentOmzetTargetLogic, GarmentOmzetTarget, GarmentOmzetTargetDataUtil>
    {
        private const string ENTITY = "GarmentOmzetTargets";

        public GarmentOmzetTargetTest() : base(ENTITY)
        {
        }

        [Fact]
        public void Mapping_With_AutoMapper_Profiles()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GarmentOmzetTargetMapper>();
            });
            var mapper = configuration.CreateMapper();

            GarmentOmzetTargetViewModel vm = new GarmentOmzetTargetViewModel { Id = 1 };
            GarmentOmzetTarget model = mapper.Map<GarmentOmzetTarget>(vm);

            Assert.Equal(vm.Id, model.Id);

        }
    }
}