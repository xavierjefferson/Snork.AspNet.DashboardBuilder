using System.Diagnostics;
using System.Net;
using System.Text;

namespace Snork.AspNet.DashboardBuilder
{
    public abstract class HtmlPage
    {
        private readonly StringBuilder _content = new StringBuilder();
        private string _body;


        protected HtmlPage()
        {
            GenerationTime = Stopwatch.StartNew();
            Html = new HtmlHelper(this);
        }

        public HtmlPage Layout { get; protected set; }
        public HtmlHelper Html { get; }
        public UrlHelper Url { get; private set; }

        public string AppPath { get; internal set; }
        public DashboardOptions DashboardOptions { get; private set; }
        public Stopwatch GenerationTime { get; private set; }

        public DashboardRequest Request {   get; private set; }
        public DashboardResponse Response { get; private set; }

        public string RequestPath => Request.Path;

        /// <exclude />
        public abstract void Execute();

        public string Query(string key)
        {
            return Request.GetQuery(key);
        }

        public override string ToString()
        {
            return TransformText(null);
        }

        /// <exclude />
        public void Assign(HtmlPage parentPage)
        {
            Request = parentPage.Request;
            Response = parentPage.Response;
            AppPath = parentPage.AppPath;
            DashboardOptions = parentPage.DashboardOptions;
            Url = parentPage.Url;

            GenerationTime = parentPage.GenerationTime;
        }

        internal void Assign(DashboardContext context)
        {
            Request = context.Request;
            Response = context.Response;

            AppPath = context.Options.AppPath;
            DashboardOptions = context.Options;
            Url = new UrlHelper(context);
        }

        /// <exclude />
        protected void WriteLiteral(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            _content.Append(textToAppend);
        }

        /// <exclude />
        protected virtual void Write(object value)
        {
            if (value == null)
                return;
            var html = value as NonEscapedString;
            WriteLiteral(html?.ToString() ?? Encode(value.ToString()));
        }

        protected virtual object RenderBody()
        {
            return new NonEscapedString(_body);
        }

        private string TransformText(string body)
        {
            _body = body;

            Execute();

            if (Layout != null)
            {
                Layout.Assign(this);
                return Layout.TransformText(_content.ToString());
            }

            return _content.ToString();
        }

        private static string Encode(string text)
        {
            return string.IsNullOrEmpty(text)
                ? string.Empty
                : WebUtility.HtmlEncode(text);
        }
    }
}