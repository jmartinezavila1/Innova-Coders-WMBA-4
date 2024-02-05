using Microsoft.AspNetCore.Mvc.Rendering;
using WMBA_4.Utilities;

public static class PageSizeHelper
{
    /// <summary>
    /// Gets the page size coming from either the Select control
    /// or the cookie.  Otherwise it sets the default of 5
    /// </summary>
    /// <param name="httpContext">The HttpContext from the controller</param>
    /// <param name="pageSizeID">the pageSizeID value from the Request</param>
    /// <returns></returns>
    public static int SetPageSize(HttpContext httpContext, int? pageSizeID)
    {
        //
        int pageSize;
        if (pageSizeID.HasValue)
        {
            //Value selected from DDL so use and save it to Cookie
            pageSize = pageSizeID.GetValueOrDefault();
            CookieHelper.CookieSet(httpContext, "pageSizeValue", pageSize.ToString(), 30);
        }
        else
        {
            //Not selected so see if it is in Cookie
            pageSize = Convert.ToInt32(httpContext.Request.Cookies["pageSizeValue"]);
        }
        return (pageSize == 0) ? 5 : pageSize;//Neither Selected or in Cookie so go with default
    }
    /// <summary>
    /// Creates a SelectList for choices for page size
    /// </summary>
    /// <param name="pageSize">Current value for the selected option</param>
    /// <returns></returns>
    public static SelectList PageSizeList(int? pageSize)
    {
        return new SelectList(new[] { "5", "10", "15", "20", "25", "30"}, pageSize.ToString());
    }
}