using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcPaging;
using System.Web.Mvc.Ajax;

namespace MvcPaging
{
    public static class AjaxPagingExtensions
    {
        #region HtmlHelper extensions
        /// <summary>
        /// Create Pager with different type of options like custom page title, tooltip, font size, controls option.
        /// <example>
        /// <code>
        /// <para>new Options {</para>
        /// <para>    PageSize = Model.PageSize,</para>
        /// <para>    TotalItemCount = Model.TotalItemCount,</para>
        /// <para>    CurrentPage = Model.PageNumber,</para>
        /// <para>    ItemTexts = new ItemTexts() { Next = "Next", Previous = "Previous", Page = "P" },</para>
        /// <para>    TooltipTitles = new TooltipTitles() { Next = "Next page", Previous = "Previous page", Page = "Page" },</para>
        /// <para>    Size = Size.normal,</para>
        /// <para>    Alignment = Alignment.centered,</para>
        /// <para>    IsShowControls = true },</para>
        /// <para>new AjaxOptions {</para>
        /// <para>    UpdateTargetId = "grid-list",</para>
        /// <para>    OnBegin = "beginPaging",</para>
        /// <para>    OnSuccess = "successPaging",</para>
        /// <para>    OnFailure = "failurePaging" },</para>
        /// <para>new { filterParameter = ViewData["foo"] })</para>
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="options"></param>
        /// /// <param name="ajaxOptions"></param>
        /// <param name="values">
        /// Set your fileter parameter
        /// <code>
        /// new { parameterName = ViewData["foo"] }
        /// </code>
        /// </param>
        /// <returns></returns>
        public static string Pager(this AjaxHelper ajaxHelper, Options options, AjaxOptions ajaxOptions, object values)
        {
            return Pager(ajaxHelper, options, ajaxOptions, new RouteValueDictionary(values));
        }

        public static string Pager(this AjaxHelper ajaxHelper, Options options, AjaxOptions ajaxOptions, RouteValueDictionary valuesDictionary)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (options.ActionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.");
                }
                valuesDictionary.Add("action", options.ActionName);
            }
            var pager = new AjaxPager(ajaxHelper, ajaxHelper.ViewContext, options, ajaxOptions, valuesDictionary);
            return pager.RenderHtml();
        }

        #endregion
    }
}