using Com.Danliris.Sales.Test.BussinesLogic.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Facades;
using Com.Danliris.Service.Sales.Lib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Sales.Test.BussinesLogic.DataUtils
{
    public class ArticleColorDataUtil : BaseDataUtil<ArticleColorFacade, ArticleColor>
    {
        public ArticleColorDataUtil(ArticleColorFacade facade) : base(facade)
        {
        }

        public override Task<ArticleColor> GetNewData()
        {
            return Task.FromResult(new ArticleColor()
            {
                Name = "das",
                Description = "des"
            });
        }
    }
}
