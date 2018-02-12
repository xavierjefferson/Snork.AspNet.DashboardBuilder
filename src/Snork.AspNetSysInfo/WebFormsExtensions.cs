using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Snork.AspNet.Dashboard.SysInfo
{
    internal static class WebFormsExtensions
    {
        public static T AddMany<T, U>(this T control, IEnumerable<U> items, Func<U, Control> createFunc)
            where T : Control
        {
            foreach (var item in items)
            {
                var newControl = createFunc(item);
                if (newControl != null)
                {
                    control.Controls.Add(newControl);
                }
            }
            return control;
        }

        public static T With<T>(this T control, Action<T> contextAction) where T : Control
        {
            contextAction(control);
            return control;
        }

        public static T AddControl<T>(this T control, Func<Control> contextAction) where T : Control
        {
            control.Controls.Add(contextAction());
            return control;
        }

        public static T Css<T>(this T control, string name, string value) where T : WebControl
        {
            control.Style[name] = value;
            return control;
        }

        public static T AddClass<T>(this T control, string value) where T : WebControl
        {
            if (string.IsNullOrEmpty(control.CssClass))
            {
                control.CssClass = value;
            }
            else
            {
                var classes = control.CssClass.Split(' ');
                if (!classes.Contains(value))
                {
                    control.CssClass = string.Join(" ", classes.Union(new[] { value }));
                }
            }
            return control;
        }

        public static T SetAttribute<T>(this T control, string name, string value) where T : HtmlGenericControl
        {
            control.Attributes[name] = value;
            return control;
        }

        public static T SetInnerHtml<T>(this T control, string value) where T : HtmlGenericControl
        {
            control.InnerHtml = value;
            return control;
        }

        public static T SetID<T>(this T control, string value) where T : Control
        {
            control.ID = value;
            return control;
        }
    }
}