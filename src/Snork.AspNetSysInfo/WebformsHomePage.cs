using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Snork.AspNet.Dashboard.SysInfo.Properties;

namespace Snork.AspNet.Dashboard.SysInfo
{
    internal class WebformsHomePage : Page
    {
        private readonly HtmlGenericControl _body = new HtmlGenericControl("body");

        private readonly HtmlHead _head = new HtmlHead();
        private readonly HtmlGenericControl _html = new HtmlGenericControl("html");
        private readonly HyperLink _hyperLink = new HyperLink().AddClass("k-button").Css("margin-bottom", "10px");
        private readonly Panel _tabStrip = new Panel { ID = "tabstrip", ClientIDMode = ClientIDMode.Static };
        private readonly HtmlGenericControl _tabStripUtl = new HtmlGenericControl("ul");

        public WebformsHomePage(HomePage homePage)
        {
            HomePage = homePage;
        }

        public HomePage HomePage { get; }

        protected override void CreateChildControls()
        {
            Controls.Add(_html.SetAttribute("xmlns", "http://www.w3.org/1999/xhtml")
                .AddControl(() => _head
                    .AddControl(() => new HtmlTitle { Text = "System Information" })
                    .AddControl(() => new HtmlMeta
                    {
                        HttpEquiv = "Content-Type",
                        Content = "text/html; charset=utf-8"
                    })
                    .AddControl(() =>
                        CreateRemoteScriptControl("//code.jquery.com/jquery-2.2.4.min.js"))
                    .AddControl(() =>
                        CreateRemoteScriptControl("//kendo.cdn.telerik.com/2018.1.117/js/kendo.all.min.js"))
                    .AddControl(() =>
                        CreateRemoteScriptControl(HomePage.Request.PathBase + "/" + nameof(Resource1.sysinfojs)))
                    .AddControl(
                        () => CreateRemoteCssLink(HomePage.Request.PathBase + "/" + nameof(Resource1.default_css)))
                    .AddControl(() =>
                        CreateRemoteCssLink("//kendo.cdn.telerik.com/2018.1.117/styles/kendo.common.min.css"))
                    .AddControl(() =>
                        CreateRemoteCssLink("//kendo.cdn.telerik.com/2018.1.117/styles/kendo.default.min.css"))
                )
                .AddControl(() => _body
                    .AddControl(() => _hyperLink)
                    .AddControl(() => _tabStrip
                        .AddControl(() => _tabStripUtl))));


            base.CreateChildControls();
        }

        private static HtmlGenericControl CreateRemoteCssLink(string href)
        {
            return new HtmlGenericControl("link").SetAttribute("rel", "stylesheet").SetAttribute("href", href);
        }

        private static HtmlGenericControl CreateRemoteScriptControl(string src)
        {
            return new HtmlGenericControl("script").SetAttribute("src", src);
        }

        private string FormatNumber(ulong value)
        {
            if (value < 4 * 1024)
            {
                return string.Format("{0} Bytes", value);
            }
            if (value < (long) 4 * 1024 * 1024)
            {
                return string.Format("{0} KB", (value / (double) 1024).ToString("N"));
            }
            if (value < (long) 4 * 1024 * 1024 * 1024)
            {
                return string.Format("{0} MB", (value / (double) ((long) 1024 * 1024)).ToString("N"));
            }
            if (value < (long) 4 * 1024 * 1024 * 1024 * 1024)
            {
                return string.Format("{0} GB", (value / (double) ((long) 1024 * 1024 * 1024)).ToString("N"));
            }
            return string.Format("{0} TB", (value / (double) ((long) 1024 * 1024 * 1024 * 1024)).ToString("N"));
        }


        private void AppendGrid(GridItemList table)
        {
            var id = "a_" + Guid.NewGuid();
            var grid = new HtmlGenericControl("table").SetID(id)
                    .SetAttribute("class", "makeAGrid")
                    .AddControl(() =>
                    {
                        return new HtmlGenericControl("colgroup").AddControl(() =>
                                new HtmlGenericControl("col").SetAttribute("style", "width:300px"))
                            .AddControl(() => new HtmlGenericControl("col"));
                    })
                    .AddControl(() => new HtmlGenericControl("thead")
                        .AddControl(() => new HtmlGenericControl("tr")
                            .AddControl(() =>
                                new HtmlGenericControl("th").SetAttribute("data-field", nameof(GridItem.Name))
                                    .SetInnerHtml("Name"))
                            .AddControl(() =>
                                new HtmlGenericControl("th").SetAttribute("data-field", nameof(GridItem.Value))
                                    .SetInnerHtml("Value"))
                        ))
                    .AddControl(() => new HtmlGenericControl("tbody")
                        .AddMany(table, i =>
                        {
                            return new HtmlGenericControl("tr")
                                .AddControl(
                                    () => new HtmlGenericControl("td").SetInnerHtml(HttpUtility.HtmlEncode(i.Name)))
                                .AddControl(
                                    () => new HtmlGenericControl("td").SetInnerHtml(HttpUtility.HtmlEncode(i.Value)));
                        }))
                ;


            var div = new Panel();
            div.Controls.Add(grid);
            var htmlGenericControl = new HtmlGenericControl("li").SetInnerHtml(table.GridName);
            if (_tabStripUtl.Controls.Count == 0)
            {
                htmlGenericControl.Attributes["class"] = "k-state-active";
            }
            _tabStripUtl.Controls.Add(htmlGenericControl);
            _tabStrip.Controls.Add(div);
        }

       

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
         
            if (string.IsNullOrEmpty(HomePage.AppPath))
            {
                _hyperLink.Visible = false;
            }
            else
            {   _hyperLink.Text = "Return to Application";
                _hyperLink.NavigateUrl = HomePage.AppPath;
            }
            AppendGrid(GetSystemInfo());
            AppendGrid(GetSystemProcessorInfo());
            AppendGrid(GetSystemMemoryInfo());
            AppendGrid(GetSystemStorageInfo());
            AppendGrid(GetRequestHeaderInfo());
            AppendGrid(GetServerVariables());
            AppendGrid(GetEnvironmentVariables());
            AppendGrid(GetSessionInfo());
            AppendGrid(GetAssemblies());
            base.OnLoad(e);
        }


        #region Get Information Function

        private GridItemList GetSystemInfo()
        {
            var os = Environment.OSVersion;
            var text = string.Empty;
            switch (os.Platform)
            {
                case PlatformID.Win32Windows:
                    switch (os.Version.Minor)
                    {
                        case 0:
                            text = "Microsoft Windows 95";
                            break;
                        case 10:
                            text = "Microsoft Windows 98";
                            break;
                        case 90:
                            text = "Microsoft Windows Millennium Edition";
                            break;
                        default:
                            text = "Microsoft Windows 95 or later";
                            break;
                    }
                    break;
                case PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3:
                            text = "Microsoft Windows NT 3.51";
                            break;
                        case 4:
                            text = "Microsoft Windows NT 4.0";
                            break;
                        case 5:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    text = "Microsoft Windows 2000";
                                    break;
                                case 1:
                                    text = "Microsoft Windows XP";
                                    break;
                                case 2:
                                    text = "Microsoft Windows 2003";
                                    break;
                                default:
                                    text = "Microsoft NT 5.x";
                                    break;
                            }
                            break;
                        case 6:
                            text = "Microsoft Windows Vista or 2008 Server";
                            break;
                    }
                    break;
                default:
                    if ((int) os.Platform > 3)
                    {
                        var name = "/proc/version";
                        if (File.Exists(name))
                        {
                            using (var reader = new StreamReader(name))
                            {
                                text = reader.ReadToEnd().Trim();
                            }
                        }
                    }
                    break;
            }
            text = string.Format("{0} -- {1}", text, os);
            var table = new GridItemList("System Information")
            {
                { "Server Name", Server.MachineName },
                { "Server IP", Request.ServerVariables["LOCAl_ADDR"] },
                { "Server Domain", Request.ServerVariables["Server_Name"] },
                { "Server Port", Request.ServerVariables["Server_Port"] },
                { "Web Server Version", Request.ServerVariables["Server_SoftWare"] },
                { "Virtual Request Path", Request.FilePath },
                { "Virtual Application Root Path", Request.ApplicationPath },
                { "Physical Application Root Path", Request.PhysicalApplicationPath },
                { "Operating System", text },
                { "Operating System Installation Directory", Environment.SystemDirectory },
                { ".Net Version", Environment.Version.ToString() },
                { ".Net Language", CultureInfo.InstalledUICulture.EnglishName },
                { "Server Current Time", DateTime.Now.ToString() },
                { "System Uptime", TimeSpan.FromMilliseconds(Environment.TickCount).ToString() },
                { "Script Timeout", TimeSpan.FromSeconds(Server.ScriptTimeout).ToString() }
            };


            return table;
        }

        private void GetSystemStorageInfo_DriveInfo(GridItemList table)
        {
            try
            {
                var typeDriveInfo = Type.GetType(nameof(DriveInfo));
                var get_drives = typeDriveInfo.GetMethod(nameof(DriveInfo.GetDrives));
                var result = get_drives.Invoke(null, null);

                foreach (var o in (IEnumerable) result)
                {
                    try
                    {
                        //  Use reflection to call DriveInfo.GetProperties() to make 1.x compiler don't complain.
                        var props = typeDriveInfo.GetProperties();
                        var is_ready = (bool) typeDriveInfo.GetProperty(nameof(DriveInfo.IsReady)).GetValue(o, null);
                        var name = string.Empty;
                        var volume_label = string.Empty;
                        var drive_format = string.Empty;
                        var drive_type = string.Empty;
                        ulong total_free_space = 0;
                        ulong total_space = 0;
                        foreach (var prop in props)
                        {
                            switch (prop.Name)
                            {
                                case "Name":
                                    name = (string) prop.GetValue(o, null);
                                    break;
                                case "VolumeLabel":
                                    if (is_ready)
                                        volume_label = (string) prop.GetValue(o, null);
                                    break;
                                case "DriveFormat":
                                    if (is_ready)
                                        drive_format = (string) prop.GetValue(o, null);
                                    break;
                                case "DriveType":
                                    drive_type = prop.GetValue(o, null).ToString();
                                    break;
                                case "TotalFreeSpace":
                                    if (is_ready)
                                        total_free_space = (ulong) (long) prop.GetValue(o, null);
                                    break;
                                case "TotalSize":
                                    if (is_ready)
                                        total_space = (ulong) (long) prop.GetValue(o, null);
                                    break;
                            }
                        }

                        var label = string.Empty;
                        var size = string.Empty;

                        if (is_ready)
                        {
                            label = string.Format("{0} - <{1}> [{2}] - {3,-10}", name, volume_label, drive_format,
                                drive_type);
                            if (total_space > 0 && total_space != ulong.MaxValue && total_space != int.MaxValue)
                            {
                                size = string.Format("Free {0} / Total {1}", FormatNumber(total_free_space),
                                    FormatNumber(total_space));
                            }
                        }
                        else
                        {
                            label = string.Format("{0} {1,-10}", name, drive_type);
                        }

                        table.Add(label, size);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void GetSystemStorageInfo_WMI(GridItemList table)
        {
            try
            {
                //  Use reflection to call WMI to make Mono compiler don't complain about assembly reference
                var dSystemManangement = Assembly.Load(
                    "System.Management, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A");
                if (dSystemManangement == null) return;

                var tManagementObjectSearcher =
                    dSystemManangement.GetType("System.Management.ManagementObjectSearcher");
                if (dSystemManangement == null) return;

                var mGet = tManagementObjectSearcher.GetMethod("Get", new Type[] { });

                var ctor = tManagementObjectSearcher.GetConstructor(new[] { typeof(string) });

                var searcher = ctor.Invoke(new object[] { "Select * From Win32_LogicalDisk" });
                if (dSystemManangement == null) return;

                var disks = mGet.Invoke(searcher, null);

                //  ManagementObject
                var tManagementObject = dSystemManangement.GetType("System.Management.ManagementObject");

                foreach (var disk in (IEnumerable) disks)
                {
                    try
                    {
                        var prop = tManagementObject.GetProperty("Item", new[] { typeof(string) });
                        var i_drive_type = (uint) prop.GetValue(disk, new object[] { "DriveType" });
                        var drive_type = string.Empty;
                        switch (i_drive_type)
                        {
                            case 1:
                                drive_type = "No Root Directory";
                                break;
                            case 2:
                                drive_type = "Removable Disk";
                                break;
                            case 3:
                                drive_type = "Local Disk";
                                break;
                            case 4:
                                drive_type = "Network Drive";
                                break;
                            case 5:
                                drive_type = "Compact Disc";
                                break;
                            case 6:
                                drive_type = "RAM Disk";
                                break;
                            default:
                                drive_type = "Unknown";
                                break;
                        }
                        var name = prop.GetValue(disk, new object[] { "Name" }) as string;
                        var volume_label = prop.GetValue(disk, new object[] { "VolumeName" }) as string;
                        var filesystem = prop.GetValue(disk, new object[] { "FileSystem" }) as string;

                        var free_space = string.Empty;
                        try
                        {
                            free_space = FormatNumber((ulong) prop.GetValue(disk, new object[] { "FreeSpace" }));
                        }
                        catch (Exception)
                        {
                        }

                        var total_space = string.Empty;
                        try
                        {
                            total_space = FormatNumber((ulong) prop.GetValue(disk, new object[] { "Size" }));
                        }
                        catch (Exception)
                        {
                        }

                        var left = string.Format("{0} - <{1}> [{2}] - {3,-10}",
                            name,
                            volume_label,
                            filesystem,
                            drive_type
                        );

                        var right = free_space == null || free_space == ""
                            ? string.Empty
                            : string.Format("Free {0} / Total {1}", free_space, total_space);

                        table.Add(left, right);
                    }
                    catch (Exception exception)
                    {
                        table.Add("Exception Occurs", exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                table.Add("Exception Occurs", exception.ToString());
            }
        }

        private GridItemList GetSystemStorageInfo()
        {
            var table = new GridItemList("Storage Information");

            try
            {
                table.Add("Logical Driver Information", string.Join(", ", Directory.GetLogicalDrives()));
            }
            catch (Exception)
            {
            }

            if (Environment.Version.Major >= 2)
            {
                GetSystemStorageInfo_DriveInfo(table);
            }
            else
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    GetSystemStorageInfo_WMI(table);
                }
            }

            return table;
        }

        private void GetSystemMemoryInfo_proc(GridItemList table)
        {
            var name = "/proc/meminfo";
            if (File.Exists(name))
            {
                using (var reader = new StreamReader(name, Encoding.ASCII))
                {
                    var ht = new Hashtable();
                    var line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var item = line.Split(":".ToCharArray());
                        if (item.Length == 2)
                        {
                            var k = item[0].Trim();
                            var v = item[1].Trim();
                            ht.Add(k, v);
                        }
                    }
                    table.Add("Physical Memory Size", string.Format("{0}", ht["MemTotal"]));
                    table.Add("Physical Free Memory Size", string.Format("{0}", ht["MemFree"]));
                    table.Add("Swap Total Size", string.Format("{0}", ht["SwapTotal"]));
                    table.Add("Swap Free Size", string.Format("{0}", ht["SwapFree"]));
                }
            }
        }

        private GridItemList GetSystemMemoryInfo()
        {
            var table = new GridItemList("Memory Information");
            ;
            table.Add("Current Working Set", FormatNumber((ulong) Environment.WorkingSet));
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    var memory = SystemInfo.Memory;
                    table.Add("Physical Memory Size", FormatNumber(memory.dwTotalPhys));
                    table.Add("Physical Free Memory Size", FormatNumber(memory.dwAvailPhys));
                    table.Add("PageFile Size", FormatNumber(memory.dwTotalPageFile));
                    table.Add("Available PageFile Size", FormatNumber(memory.dwAvailPageFile));
                    table.Add("Virtual Memory Size", FormatNumber(memory.dwTotalVirtual));
                    table.Add("Available Memory Size", FormatNumber(memory.dwAvailVirtual));
                    table.Add("Memory Load", string.Format("{0} %", memory.dwMemoryLoad.ToString("N")));
                }
                else if ((int) Environment.OSVersion.Platform > 3)
                {
                    GetSystemMemoryInfo_proc(table);
                }
            }
            catch (Exception)
            {
            }
            return table;
        }

        private void GetSystemProcessorInfo_WMI(GridItemList table)
        {
            //  Use reflection to call WMI to make Mono compiler don't complain about assembly reference
            var dSystemManangement = Assembly.Load(
                "System.Management, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A");
            if (dSystemManangement == null) return;

            var tManagementObjectSearcher = dSystemManangement.GetType("System.Management.ManagementObjectSearcher");
            if (dSystemManangement == null) return;

            var mGet = tManagementObjectSearcher.GetMethod("Get", new Type[] { });

            var ctor = tManagementObjectSearcher.GetConstructor(new[] { typeof(string) });

            var searcher = ctor.Invoke(new object[] { "Select * From Win32_Processor" });
            if (dSystemManangement == null) return;

            var processors = mGet.Invoke(searcher, null);

            //  ManagementObject
            var tManagementObject = dSystemManangement.GetType("System.Management.ManagementObject");
            foreach (var processor in (IEnumerable) processors)
            {
                try
                {
                    try
                    {
                        var prop = tManagementObject.GetProperty("Item", new[] { typeof(string) });
                        var sb = new StringBuilder();
                        //  Unique ID
                        var name = (string) prop.GetValue(processor, new object[] { "Name" });
                        sb.Append(name);
                        //  Clock Speed
                        var clock_speed = (uint) prop.GetValue(processor, new object[] { "CurrentClockSpeed" });
                        //  Max Clock Speed
                        var max_clock_speed = (uint) prop.GetValue(processor, new object[] { "MaxClockSpeed" });
                        sb.AppendFormat(" - {0} MHz / {1} MHz", clock_speed, max_clock_speed);
                        //  Current Voltage
                        var i_current_voltage = (ushort) prop.GetValue(processor, new object[] { "CurrentVoltage" });
                        double current_voltage = 0;
                        if (((uint) i_current_voltage & 0x80) == 0)
                        {
                            current_voltage = (i_current_voltage & 0x7F) / 10.0;
                        }
                        else
                        {
                            try
                            {
                                var caps = (uint) prop.GetValue(processor, new object[] { "VoltageCaps" });
                                switch (caps & 0xF)
                                {
                                    case 1:
                                        current_voltage = 5;
                                        break;
                                    case 2:
                                        current_voltage = 3.3;
                                        break;
                                    case 3:
                                        current_voltage = 2.9;
                                        break;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (current_voltage > 0)
                        {
                            sb.AppendFormat(" - {0}v", current_voltage);
                        }
                        //  Load Percentage 
                        var load_percentage = (ushort) prop.GetValue(processor, new object[] { "LoadPercentage" });
                        sb.AppendFormat(" - Load = {0} %", load_percentage);
                        table.Add("Processor", sb.ToString());
                    }
                    catch (Exception exception)
                    {
                        table.Add("Exception Occurs", exception.ToString());
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void GetSystemProcessorInfo_proc(GridItemList table)
        {
            var name = "/proc/cpuinfo";
            if (File.Exists(name))
            {
                using (var reader = new StreamReader(name, Encoding.ASCII))
                {
                    var processors = new ArrayList();
                    var ht = new Hashtable();
                    var line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim().Length == 0)
                        {
                            processors.Add(ht);
                            ht = new Hashtable();
                        }
                        var item = line.Split(":".ToCharArray());
                        if (item.Length == 2)
                        {
                            var k = item[0].Trim();
                            var v = item[1].Trim();
                            ht.Add(k, v);
                        }
                    }

                    foreach (Hashtable processor in processors)
                    {
                        var n = string.Format("Processor {0}", processor["processor"]);
                        var v = string.Format("{0}{1}", processor["model name"],
                            processor["cpu MHz"] != null
                                ? string.Format(" - {0} MHz", processor["cpu MHz"])
                                : string.Empty);
                        table.Add(n, v);
                    }
                }
            }
        }

        private GridItemList GetSystemProcessorInfo()
        {
            var table = new GridItemList("Processor Information");
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    table.Add("Number of Processors", Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
                    table.Add("Processor Id", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"));
                    var cpu = SystemInfo.Cpu;
                    table.Add("Processor Type", cpu.dwProcessorType.ToString());
                    table.Add("Processor Level", cpu.dwProcessorLevel.ToString());
                    table.Add("Processor OEM Id", cpu.dwOemId.ToString());
                    table.Add("Page Size", cpu.dwPageSize.ToString());
                    GetSystemProcessorInfo_WMI(table);
                }
                else if ((int) Environment.OSVersion.Platform > 3)
                {
                    GetSystemProcessorInfo_proc(table);
                }
            }
            catch (Exception)
            {
            }
            return table;
        }

        private GridItemList GetServerVariables()
        {
            var table = new GridItemList("Server Variables");
            foreach (var key in Request.ServerVariables.AllKeys)
            {
                table.Add(key, Request.ServerVariables[key]);
            }
            return table;
        }

        private GridItemList GetEnvironmentVariables()
        {
            var table = new GridItemList("Environment Variables");
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                table.Add(de.Key.ToString(), de.Value.ToString());
            }
            return table;
        }


        private GridItemList GetAssemblies()
        {
            var table = new GridItemList("Assemblies");
            foreach (var x in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyName = x.GetName();
                table.Add(assemblyName.Name,
                    string.Format("Version={0}, Culture={1}, PublicKeyToken={2}", assemblyName.Version,
                            string.IsNullOrWhiteSpace(assemblyName.CultureName) ? "neutral" : assemblyName.CultureName,
                            BitConverter.ToString(assemblyName.GetPublicKeyToken()))
                        .Replace("-", ""));
            }
            return table;
        }

        private GridItemList GetSessionInfo()
        {
            var table = new GridItemList("Session Information")
            {
               
                { "Application Count", Application.Contents.Count.ToString() }
            };
            try
            {
                table.Add("Session Count", Session.Contents.Count.ToString());
            }
            catch (Exception ex)
            {
                table.Add("Session Count", "n/a (disabled)");
            }
            return table;
        }

        private GridItemList GetRequestHeaderInfo()
        {
            var table = new GridItemList("Request Headers");
            foreach (var key in Request.Headers.AllKeys)
            {
                table.Add(key, Request.Headers[key]);
            }
            return table;
        }

        #endregion
    }
}