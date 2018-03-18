using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcPaging;

namespace MvcPaging
{
    /// <summary>
    /// Set page options and values
    /// </summary>
    public static class PagingExtensions
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
        /// <para>    IsShowControls = true</para>
        /// <para>}, new { filterParameter = ViewData["foo"] })</para>
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="options"></param>
        /// <param name="values">
        /// Set your fileter parameter
        /// <code>
        /// new { parameterName = ViewData["foo"] }
        /// </code>
        /// </param>
        /// <returns></returns>
        public static string Pager(this HtmlHelper htmlHelper, Options options, object values)
        {
            return Pager(htmlHelper, options, new RouteValueDictionary(values));
        }

        public static string Pager(this HtmlHelper htmlHelper, Options options, RouteValueDictionary valuesDictionary)
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
            var pager = new Pager(htmlHelper.ViewContext, options, valuesDictionary);
            return pager.RenderHtml();
        }

        #endregion
    }
}