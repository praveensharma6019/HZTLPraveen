using DCX.Foundation.EServices.ServiceClient.Constant;
using DCX.Foundation.EServices.ServiceClient.Extension;
using DCX.Foundation.EServices.ServiceClient.Models;
using DCX.Foundation.EServices.ServiceClient;
using DCX.Foundation.Extension.Attributes;
using DCX.Foundation.Logging.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using DCX.Feature.EServices.AccountServices.Models.AwayMode;
using DCX.Feature.EServices.AccountServices.Service;
using DCX.Feature.EServices.AccountServices.UnitOfWork;
namespace DCX.Feature.EServices.AccountServices.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class AwayModeController : CustomController
    {
        #region -- Properties --
        private readonly AwayModeUnitOfWork awayModeUnitOfWork = new AwayModeUnitOfWork();
        private readonly ClientCommon clientCommon;
        private ILoggerServices awayModeLoggerService;
        public AwayModeController()
        {
            clientCommon = new ClientCommon();
            awayModeLoggerService = new AwayModeLoggerService();
        }
        #endregion
        //GetAwayModeSet
        #region Action Methods GetAwayModeSet
        /// <summary>
        /// To get Away Mode Set
        /// </summary>
        /// <param name="partnerNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [SessionCheckAttribute]
        public async Task<ActionResult> GetAwayModeSet(string partnerNumber, string contractAccountNumber = "")
        {
            ODataApiResponse<GetAwayModeSetResponse> response = new ODataApiResponse<GetAwayModeSetResponse>(new GetAwayModeSetResponse());
            try
            {
                response = await awayModeUnitOfWork.AwayModeRepository.GetAwayModeSet(partnerNumber, contractAccountNumber, this.Request);
            }
            catch (Exception ex)
            {
                response.Error.ErrorMessage = ex.Message;
            }
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = response, MaxJsonLength = Int32.MaxValue };
        }
        #endregion
        //CreateAwayModeSet
        #region Action Methods CreateAwayModeSet
        /// <summary>
        /// To create Away Mode Request
        /// </summary>
        /// <param name="createAwayModeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [SessionCheckAttribute]
        public async Task<ActionResult> CreateAwayModeSet(CreateAwayModeRequest createAwayModeRequest)
        {
            ODataApiResponse<CreateAwayModeResponse> response = new ODataApiResponse<CreateAwayModeResponse>(new CreateAwayModeResponse());
            try
            {
                response = await awayModeUnitOfWork.AwayModeRepository.CreateAwayModeSet(createAwayModeRequest, this.Request);
            }
            catch (Exception ex)
            {
                response.Error.ErrorMessage = ex.Message;
            }
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = response, MaxJsonLength = Int32.MaxValue };
        }
        #endregion
        //UpdateAwayModeDetailsSet
        #region Action Methods UpdateAwayModeDetailsSet
        /// <summary>
        /// To update Away Mode Request
        /// </summary>
        /// <param name="updateAwayModeDetailsSet"></param>
        /// <returns></returns>
        [HttpPut]
        [SessionCheckAttribute]
        public async Task<ActionResult> UpdateAwayModeDetailsSet(UpdateAwayModeRequest updateAwayModeRequest)
        {
            UpdateAwayModeResponse response = new UpdateAwayModeResponse();
            try
            {
                response = await awayModeUnitOfWork.AwayModeRepository.UpdateAwayModeDetailsSet(updateAwayModeRequest, this.Request);
            }
            catch (Exception ex)
            {
                response.Error.ErrorMessage = ex.Message;
            }
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = response, MaxJsonLength = Int32.MaxValue };
        }
        #endregion
        //UpdateAwayModeDetailsSet
        #region Action Methods DeleteAwayModeDetailsSet
        /// <summary>
        /// To delete Away Mode Request
        /// </summary>
        /// <param name="deleteAwayModeDetailsSet"></param>
        /// <returns></returns>
        [HttpDelete]
        [SessionCheckAttribute]
        public async Task<ActionResult> DeleteAwayModeDetailsSet(string awayModeID, string contractAccount)
        {
            DeleteAwayModeResponse response = new DeleteAwayModeResponse();
            try
            {
                response = await awayModeUnitOfWork.AwayModeRepository.DeleteAwayModeDetailsSet(awayModeID, contractAccount, this.Request);
            }
            catch (Exception ex)
            {
                response.Error.ErrorMessage = ex.Message;
            }
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = response, MaxJsonLength = Int32.MaxValue };
        }
        #endregion
    }
}