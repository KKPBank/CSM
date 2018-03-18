using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Ajax;

namespace MvcPaging
{
    public class Pager
    {
        private ViewContext viewContext;
        private readonly Options options;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;

        public Pager(ViewContext viewContext, Options options, RouteValueDictionary valuesDictionary)
        {
            this.viewContext = viewContext;
            this.options = options;
            this.linkWithoutPageValuesDictionary = valuesDictionary;
        }

        public string RenderHtml()
        {
            int pageCount = (int)Math.Ceiling(this.options.TotalItemCount / (double)this.options.PageSize);
            int nrOfPagesToDisplay = this.options.NrOfPagesToDisplay;
            var sb = new StringBuilder();
            sb.AppendFormat("<div class=\"pagination pagination-{0} pagination-{1} {2}\"><ul>", options.Size.ToString(), options.Alignment.ToString(), options.CssClass);

            #region First Button
            // First
            if (options.IsShowFirstLast == true)
            {
                if (this.options.CurrentPage > 1)
                {
                    sb.AppendFormat("<li class=\"first\">{0}</li>", GeneratePageLink(options.ItemTexts.First, 1, options.TooltipTitles.First, options.ItemIcon.First));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(options.ItemIcon.First))
                        sb.AppendFormat("<li class=\"active first\"><span>{0}</span></li>", options.ItemTexts.First);
                    else
                        sb.AppendFormat("<li class=\"active first\"><span><i class=\"{1}\"></i> {0}</span></li>", options.ItemTexts.First, options.ItemIcon.First);
                }
            }
            #endregion

            #region Previous Button
            // Previous «
            if (options.IsShowControls == true)
            {
                if (this.options.CurrentPage > 1)
                {
                    sb.AppendFormat("<li class=\"previous\">{0}</li>", GeneratePageLink(options.ItemTexts.Previous, this.options.CurrentPage - 1, options.TooltipTitles.Previous, options.ItemIcon.Previous));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(options.ItemIcon.Previous))
                        sb.AppendFormat("<li class=\"active previous\"><span>{0}</span></li>", options.ItemTexts.Previous);
                    else
                        sb.AppendFormat("<li class=\"active previous\"><span><i class=\"{1}\"></i> {0}</span></li>", options.ItemTexts.Previous, options.ItemIcon.Previous);
                }
            }
            #endregion

            #region Pages

            int start = 1;
            int end = pageCount;
            if (options.IsShowPages == true)
            {
                if (pageCount > nrOfPagesToDisplay)
                {
                    int middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                    int below = (this.options.CurrentPage - middle);
                    int above = (this.options.CurrentPage + middle);

                    if (below < 4)
                    {
                        above = nrOfPagesToDisplay;
                        below = 1;
                    }
                    else if (above > (pageCount - 4))
                    {
                        above = pageCount;
                        below = (pageCount - nrOfPagesToDisplay);
                    }

                    start = below;
                    end = above;
                }

                #region ...
                if (options.IsShowFirstLast == false)
                {
                    if (start > 3)
                    {
                        sb.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, "1"), 1, options.TooltipTitles.Page, options.ItemIcon.Page));
                        sb.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, "2"), 2, options.TooltipTitles.Page, options.ItemIcon.Page));
                        sb.Append("<li class=\"disabled\"><span>...</span></li>");
                    }
                }

                #endregion

                if (options.IsShowInputPage == false)
                {
                    for (int i = start; i <= end; i++)
                    {
                        if (i == this.options.CurrentPage)
                        {
                            sb.AppendFormat("<li class=\"active\"><span>{0}</span></li>", string.Format("{0}{1}", options.ItemTexts.Page, i));
                        }
                        else
                        {
                            sb.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, i.ToString()), i, options.TooltipTitles.Page, options.ItemIcon.Page));
                        }
                    }
                }
                else
                {
                    var extendCssClass = this.options.CurrentPage == 1 ? "": " pagination-label ";

                    sb.AppendFormat("<li class=\"active padding-left-10 padding-right-10\"><strong class='padding-right-20 {1}'>หน้าที่</strong><input type='text' class='form-control input-page' value='{0}' /></li>",
                            string.Format("{0}{1}", options.ItemTexts.Page, this.options.CurrentPage), extendCssClass);
                }

                #region ...
                if (options.IsShowFirstLast == false)
                {
                    if (end < (pageCount - 3))
                    {
                        sb.Append("<li class=\"disabled\"><span>...</span></li>");
                        sb.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, (pageCount - 1).ToString()), pageCount - 1, options.TooltipTitles.Page, options.ItemIcon.Page));
                        sb.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, pageCount.ToString()), pageCount, options.TooltipTitles.Page, options.ItemIcon.Page));
                    }
                }
                #endregion
            }

            #endregion

            #region Next Button
            // Next »
            if (options.IsShowControls == true)
            {
                if (this.options.CurrentPage < pageCount)
                {
                    sb.AppendFormat("<li class=\"next\">{0}</li>", GeneratePageLink(options.ItemTexts.Next, this.options.CurrentPage + 1, options.TooltipTitles.Next, options.ItemIcon.Next, "last"));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(options.ItemIcon.Next))
                        sb.AppendFormat("<li class=\"active next\"><span>{0}</span></li>", options.ItemTexts.Next);
                    else
                        sb.AppendFormat("<li class=\"active next\"><span>{0} <i class=\"{1}\"></i></span></li>", options.ItemTexts.Next, options.ItemIcon.Next);
                }
            }
            #endregion

            #region Last Button
            // Last
            if (options.IsShowFirstLast == true)
            {
                if (this.options.CurrentPage < pageCount)
                {
                    sb.AppendFormat("<li class=\"last\">{0}</li>", GeneratePageLink(options.ItemTexts.Last, pageCount, options.TooltipTitles.Last, options.ItemIcon.Last, "last"));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(options.ItemIcon.Last))
                        sb.AppendFormat("<li class=\"active last\"><span>{0}</span></li>", options.ItemTexts.Last);
                    else
                        sb.AppendFormat("<li class=\"active last\"><span>{0} <i class=\"{1}\"></i></span></li>", options.ItemTexts.Last, options.ItemIcon.Last);
                }
            }
            #endregion

            sb.Append("</ul></div>");
            return sb.ToString();
        }

        private string GeneratePageLink(string linkText, int pageNumber, string title, string icon, string buttonType = "")
        {
            var routeDataValues = viewContext.RequestContext.RouteData.Values;

            var pageLinkValueDictionary = new RouteValueDictionary(this.linkWithoutPageValuesDictionary);
            pageLinkValueDictionary.Add("page", pageNumber);
            var virtualPathData = RouteTable.Routes.GetVirtualPath(this.viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathData != null)
            {
                string newTitle = string.Format(title, pageNumber);
                string linkFormat = "<a href=\"{0}\" title=\"" + newTitle + "\">{1}</a>";

                if (!string.IsNullOrWhiteSpace(icon))
                {
                    //string link = string.Empty;
                    var builder = new TagBuilder("i");
                    builder.MergeAttribute("class", icon);

                    if (string.IsNullOrWhiteSpace(buttonType))
                        linkFormat = "<a href=\"{0}\" title=\"" + newTitle + "\"><i class=\"" + icon + "\"></i> {1}</a>";
                    else
                        linkFormat = "<a href=\"{0}\" title=\"" + newTitle + "\">{1} <i class=\"" + icon + "\"></i></a>";
                }

                // Get the right route, ensure the controller and action are specified
                if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
                {
                    pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
                }
                if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
                {
                    pageLinkValueDictionary.Add("action", routeDataValues["action"]);
                }

                // Render virtual path
                var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);
                var url = virtualPathForArea == null ? null : virtualPathForArea.VirtualPath;

                return String.Format(linkFormat, url, linkText, title, pageNumber);
            }
            else
            {
                return null;
            }
        }
    }
}