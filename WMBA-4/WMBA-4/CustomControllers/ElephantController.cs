using Microsoft.AspNetCore.Mvc.Filters;
using WMBA_4.Utilities;

namespace WMBA_4.CustomControllers
{
    public class ElephantController : CognizantController
    {
        //This is the list of Actions that will add the ReturnURL to ViewData
        internal string[] ActionWithURL = new string[] { "Details", "Create", "Edit", "Delete", "Add", "Update", "Remove" };
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ActionWithURL.Contains(ActionName()))
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
            }
            else if (ActionName() == "Index")
            {
                //Clear the sort/filter/paging URL Cookie for Controller
                CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);
            }
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (ActionWithURL.Contains(ActionName()))
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
            }
            else if (ActionName() == "Index")
            {
                //Clear the sort/filter/paging URL Cookie for Controller
                CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
