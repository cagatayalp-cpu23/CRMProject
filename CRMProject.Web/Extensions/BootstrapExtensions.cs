using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace CRMProject.Web.Extensions
{

    public static class BootstrapExtensions
    {
        public static MvcHtmlString ModelStateFor<TModel, TValue>(this HtmlHelper<TModel> html,
               Expression<Func<TModel, TValue>> expression)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            if (html.ViewData.ModelState.IsValidField(modelMetadata.PropertyName))
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString("has-error");
        }

        private static string buildAlertDiv(string message, string classAlertType, string iconClass, bool closeable)
        {
            var div = new TagBuilder("div");
            div.AddCssClass("alert");
            div.AddCssClass(classAlertType);

            if (closeable)
            {
                div.AddCssClass("alert-dismissable");
                var button = new TagBuilder("button");
                button.AddCssClass("close");
                button.MergeAttribute("type", "button");
                button.MergeAttribute("data-dismiss", "alert");
                button.MergeAttribute("aria-hidden", "true");
                button.InnerHtml = "&times;";
                div.InnerHtml += button.ToString();
            }
            //var icon = new TagBuilder("i");
            //icon.AddCssClass(iconClass);
            //div.InnerHtml += icon.ToString();
            //div.InnerHtml += "&nbsp;" + message;
            div.InnerHtml += message;

            return div.ToString();
        }


        //public static MvcHtmlString RenderAlertMessages(this HtmlHelper html, IDictionary<string, object> viewData, bool closeable = true)
        //{
        //    if (viewData[StringConstants.SuccessMessage] == null &&
        //        viewData[StringConstants.ErrorMessage] == null &&
        //        viewData[StringConstants.WarningMessage] == null &&
        //        viewData[StringConstants.InfoMessage] == null)
        //    {
        //        return new MvcHtmlString(string.Empty);
        //    }
        //    string alerts = "";

        //    if (viewData[StringConstants.SuccessMessage] != null)
        //    {
        //        alerts += buildAlertDiv((string)viewData[StringConstants.SuccessMessage], "alert-success", "fa fa-check", closeable);
        //    }
        //    if (viewData[StringConstants.ErrorMessage] != null)
        //    {
        //        alerts += buildAlertDiv((string)viewData[StringConstants.ErrorMessage], "alert-danger", "fa fa-ban", closeable);
        //    }
        //    if (viewData[StringConstants.WarningMessage] != null)
        //    {
        //        alerts += buildAlertDiv((string)viewData[StringConstants.WarningMessage], "alert-warning", "fa fa-warning", closeable);
        //    }
        //    if (viewData[StringConstants.InfoMessage] != null)
        //    {
        //        alerts += buildAlertDiv((string)viewData[StringConstants.InfoMessage], "alert-info", "fa fa-info", closeable);
        //    }


        //    return new MvcHtmlString(alerts);
        //}


        //public static MvcHtmlString RenderNotifications(this HtmlHelper html, IDictionary<string, object> viewData, bool sticky = true)
        //{
        //    if (viewData[StringConstants.Notification] == null)
        //    {
        //        return new MvcHtmlString(string.Empty);
        //    }


        //    var notifyMessage = (NotificationModel)viewData[StringConstants.Notification];
        //    var notifications = new StringBuilder();
        //    notifications.Append("<script>");
        //    notifications.Append("$(function(){");

        //    if (notifyMessage.NotificationType == NotificationType.Success)
        //    {
        //        notifications.Append("toastr.success('" + HttpUtility.JavaScriptStringEncode(notifyMessage.Message) + "','" + HttpUtility.JavaScriptStringEncode(notifyMessage.Title) + "', { timeOut:0, extendedTimeOut:0, positionClass: 'toast-top-full-width'});");
        //    }
        //    if (notifyMessage.NotificationType == NotificationType.Error)
        //    {
        //        notifications.Append("toastr.error('" + HttpUtility.JavaScriptStringEncode(notifyMessage.Message) + "','" + HttpUtility.JavaScriptStringEncode(notifyMessage.Title) + "', { timeOut:0, extendedTimeOut:0, positionClass: 'toast-top-full-width'});");
        //    }
        //    if (notifyMessage.NotificationType == NotificationType.Warning)
        //    {
        //        notifications.Append("toastr.warning('" + HttpUtility.JavaScriptStringEncode(notifyMessage.Message) + "','" + HttpUtility.JavaScriptStringEncode(notifyMessage.Title) + "', { timeOut:0, extendedTimeOut:0, positionClass: 'toast-top-full-width'});");
        //    }
        //    if (notifyMessage.NotificationType == NotificationType.Info)
        //    {
        //        notifications.Append("toastr.info('" + HttpUtility.JavaScriptStringEncode(notifyMessage.Message) + "','" + HttpUtility.JavaScriptStringEncode(notifyMessage.Title) + "', { timeOut:0, extendedTimeOut:0, positionClass: 'toast-top-full-width'});");
        //    }
        //    notifications.Append("});");
        //    notifications.Append("</script>");
        //    return new MvcHtmlString(notifications.ToString());
        //}

        // Returns non-null list of model states, which caller will render in order provided.
        private static IEnumerable<ModelState> GetModelStateList(HtmlHelper htmlHelper, bool excludePropertyErrors)
        {
            if (excludePropertyErrors)
            {
                ModelState ms;
                //htmlHelper.ViewData.ModelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out ms);
                htmlHelper.ViewData.ModelState.TryGetValue("", out ms);
                if (ms != null)
                {
                    return new ModelState[] { ms };
                }

                return new ModelState[0];
            }
            else
            {
                // Sort modelStates to respect the ordering in the metadata.                 
                // ModelState doesn't refer to ModelMetadata, but we can correlate via the property name.
                var ordering = new Dictionary<string, int>();

                var metadata = htmlHelper.ViewData.ModelMetadata;
                if (metadata != null)
                {
                    foreach (ModelMetadata m in metadata.Properties)
                    {
                        ordering[m.PropertyName] = m.Order;
                    }
                }

                return from kv in htmlHelper.ViewData.ModelState
                       let name = kv.Key
                       orderby ordering.GetOrDefault(name, ModelMetadata.DefaultOrder)
                       select kv.Value;
            }
        }


        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }
            return @default;
        }


        public static MvcHtmlString RenderValidationSummary(this HtmlHelper html, bool closeable)
        {
            return RenderValidationSummary(html, closeable, false);
        }
        public static MvcHtmlString RenderValidationSummary(this HtmlHelper html, bool closeable, bool excludePropertyErrors)
        {
            var errorList = new List<string>();
            var hasErrors = html.ViewContext.ViewData.ModelState.SelectMany(state => state.Value.Errors.Select(error => error.ErrorMessage)).Any();

            IEnumerable<ModelState> modelStates = GetModelStateList(html, excludePropertyErrors);
            foreach (var modelState in modelStates)
            {
                errorList.AddRange(modelState.Errors.Select(modelError => modelError.ErrorMessage).Where(errorText => !String.IsNullOrEmpty(errorText)));
            }
            //var errors = html.ViewContext.ViewData.ModelState.SelectMany(state => state.Value.Errors.Select(error => error.ErrorMessage));
            //var errorList = errors as IList<string> ?? errors.ToList();
            var errorCount = errorList.Count();
            //var errorList = errors as IList<string> ?? errors.ToList();
            if (errorCount == 0 && !hasErrors)
            {
                return new MvcHtmlString(string.Empty);
            }

            var div = new TagBuilder("div");
            div.AddCssClass("alert");
            div.AddCssClass("alert-danger");


            if (closeable)
            {
                div.AddCssClass("alert-dismissible");
                var button = new TagBuilder("button");
                button.AddCssClass("close");
                button.MergeAttribute("type", "button");
                button.MergeAttribute("data-dismiss", "alert");
                button.InnerHtml = "&times;";
                div.InnerHtml += button.ToString();
            }

            if (errorCount > 0)
            {
                var ul = new TagBuilder("ul");
                ul.AddCssClass("fa-ul");
                foreach (var error in errorList)
                {
                    var li = new TagBuilder("li");
                    li.AddCssClass("has-error");
                    var span = new TagBuilder("span");
                    span.AddCssClass("fa-li");
                    var i = new TagBuilder("i");
                    i.AddCssClass("fas");
                    i.AddCssClass("fa-exclamation");

                    span.InnerHtml += i.ToString();
                    li.InnerHtml += span + error;

                    ul.InnerHtml += li.ToString();
                }

                div.InnerHtml += ul.ToString();
            }

            return new MvcHtmlString(div.ToString());
        }

    }
}