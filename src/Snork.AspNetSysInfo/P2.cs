using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Snork.AspNetSysInfo
{
    internal class P2 : Page
    {
        private readonly HtmlGenericControl body = new HtmlGenericControl("body");
        private readonly Panel divCenter = new Panel { CssClass = "center" };
        private readonly HtmlHead head = new HtmlHead();
        private readonly HtmlGenericControl html = new HtmlGenericControl("html");

        protected override void CreateChildControls()
        {
            html.Attributes["xmlns"] = "http://www.w3.org/1999/xhtml";
            Controls.Add(html);
            html.Controls.Add(head);
            head.Controls.Add(new HtmlTitle { Text = "System Information" });
            head.Controls.Add(new HtmlMeta { HttpEquiv = "Content-Type", Content = "text/html; charset=utf-8" });
            var style = new HtmlGenericControl("style")
            {
                InnerText = @" a:link {color: #000099; text-decoration: none; background-color: #ffffff;}
      a:hover {text-decoration: underline;}
      body {font-family: Georgia, ""Times New Roman"", Times, serif; text-align: center}
                table { margin - left: auto; margin - right: auto; text - align: left; border - collapse: collapse; border: 0;
            }
            td, th {border: 1px solid #000000; font-size: 75%; vertical-align: baseline;}
                    .title
                { font-size: 150%; }
                .section {text-align: center;}
                .header {text-align: center; background-color: #9999cc; font-weight: bold; color: #000000;}
                    .name {background-color: #ccccff; font-weight: bold; color: #000000;}
                    .value {background-color: #cccccc; color: #000000;}
                    .value_true {background-color: #cccccc; color: #00ff00;}
                        .value_false {background-color: #cccccc; color: #ff0000;}"
            };
            style.Attributes["type"] = "text/css";
            head.Controls.Add(style);
            html.Controls.Add(body);
            body.Controls.Add(divCenter);

            base.CreateChildControls();
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

        private DataTable GenerateDataTable(string name)
        {
            var table = new DataTable(name);
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Value", typeof(string));
            return table;
        }

        private bool TestObject(string progID)
        {
            try
            {
                Server.CreateObject(progID);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Assign(DataTable table, string name, string value)
        {
            var row = table.NewRow();
            row["Name"] = name;
            row["Value"] = value;
            table.Rows.Add(row);
        }

        private void LoadInformation(DataTable table)
        {
            var grid = new DataGrid();
            BoundColumn col;

            col = new BoundColumn();
            col.DataField = "Name";
            col.HeaderText = "Name";
            col.ItemStyle.CssClass = "name";
            grid.Columns.Add(col);

            col = new BoundColumn();
            col.DataField = "Value";
            col.HeaderText = "Value";
            col.ItemStyle.CssClass = "value";
            grid.Columns.Add(col);

            grid.AutoGenerateColumns = false;
            grid.HeaderStyle.CssClass = "header";
            grid.DataSource = new DataView(table);
            grid.DataBind();


            foreach (DataGridItem item in grid.Items)
            {
                if (item.Cells.Count == 2)
                {
                    var cell = item.Cells[1];
                    //  change true/false style
                    switch (cell.Text.ToLower())
                    {
                        case "true":
                            cell.CssClass = "value_true";
                            break;
                        case "false":
                            cell.CssClass = "value_false";
                            break;
                    }
                    //  wrap <pre> for text contain newline.
                    if (cell.Text.IndexOf(Environment.NewLine) >= 0)
                    {
                        cell.Text = string.Format("<pre>{0}</pre>", cell.Text);
                    }
                }
            }


            var title = new HtmlGenericControl("h1");
            title.InnerText = Server.HtmlEncode(table.TableName);
            title.Attributes.Add("class", "title");

            var div = new HtmlGenericControl("div");
            div.Attributes.Add("class", "section");
            div.Controls.Add(new HtmlGenericControl("p"));
            div.Controls.Add(title);
            div.Controls.Add(grid);
            div.Controls.Add(new HtmlGenericControl("p"));

            divCenter.Controls.Add(div);
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadInformation(GetSystemInfo());
            LoadInformation(GetSystemProcessorInfo());
            LoadInformation(GetSystemMemoryInfo());
            LoadInformation(GetSystemStorageInfo());
            LoadInformation(GetRequestHeaderInfo());
            LoadInformation(GetServerVariables());
            LoadInformation(GetEnvironmentVariables());
            LoadInformation(GetSessionInfo());
            LoadInformation(GetSystemObjectInfo());
            LoadInformation(GetMailObjectInfo());
            LoadInformation(GetUploadObjectInfo());
            LoadInformation(GetGraphicsObjectInfo());
            LoadInformation(GetOtherObjectInfo());
            base.OnLoad(e);
        }


        #region Get Information Function

        private DataTable GetSystemInfo()
        {
            var table = GenerateDataTable("System Information");
            //	Server Name
            Assign(table, "Server Name", Server.MachineName);
            Assign(table, "Server IP", Request.ServerVariables["LOCAl_ADDR"]);
            Assign(table, "Server Domain", Request.ServerVariables["Server_Name"]);
            Assign(table, "Server Port", Request.ServerVariables["Server_Port"]);
            //	Web Server
            Assign(table, "Web Server Version", Request.ServerVariables["Server_SoftWare"]);
            //	Path
            Assign(table, "Virtual Request Path", Request.FilePath);
            Assign(table, "Physical Request Path", Request.PhysicalPath);
            Assign(table, "Virtual Application Root Path", Request.ApplicationPath);
            Assign(table, "Physical Application Root Path", Request.PhysicalApplicationPath);
            //	Platform
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
            Assign(table, "Operating System", text);
            Assign(table, "Operating System Installation Directory", Environment.SystemDirectory);
            Assign(table, ".Net Version", Environment.Version.ToString());
            Assign(table, ".Net Language", CultureInfo.InstalledUICulture.EnglishName);
            Assign(table, "Server Current Time", DateTime.Now.ToString());
            Assign(table, "System Uptime", TimeSpan.FromMilliseconds(Environment.TickCount).ToString());
            Assign(table, "Script Timeout", TimeSpan.FromSeconds(Server.ScriptTimeout).ToString());
            return table;
        }

        private void GetSystemStorageInfo_DriveInfo(DataTable table)
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

                        Assign(table, label, size);
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

        private void GetSystemStorageInfo_WMI(DataTable table)
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

                        Assign(table, left, right);
                    }
                    catch (Exception exception)
                    {
                        Assign(table, "Exception Occurs", exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                Assign(table, "Exception Occurs", exception.ToString());
            }
        }

        private DataTable GetSystemStorageInfo()
        {
            var table = GenerateDataTable("Storage Information");

            try
            {
                Assign(table, "Logical Driver Information", string.Join(", ", Directory.GetLogicalDrives()));
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

        private void GetSystemMemoryInfo_proc(DataTable table)
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
                    Assign(table, "Physical Memory Size", string.Format("{0}", ht["MemTotal"]));
                    Assign(table, "Physical Free Memory Size", string.Format("{0}", ht["MemFree"]));
                    Assign(table, "Swap Total Size", string.Format("{0}", ht["SwapTotal"]));
                    Assign(table, "Swap Free Size", string.Format("{0}", ht["SwapFree"]));
                }
            }
        }

        private DataTable GetSystemMemoryInfo()
        {
            var table = GenerateDataTable("Memory Information");
            ;
            Assign(table, "Current Working Set", FormatNumber((ulong) Environment.WorkingSet));
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    var memory = SystemInfo.Memory;
                    Assign(table, "Physical Memory Size", FormatNumber(memory.dwTotalPhys));
                    Assign(table, "Physical Free Memory Size", FormatNumber(memory.dwAvailPhys));
                    Assign(table, "PageFile Size", FormatNumber(memory.dwTotalPageFile));
                    Assign(table, "Available PageFile Size", FormatNumber(memory.dwAvailPageFile));
                    Assign(table, "Virtual Memory Size", FormatNumber(memory.dwTotalVirtual));
                    Assign(table, "Available Memory Size", FormatNumber(memory.dwAvailVirtual));
                    Assign(table, "Memory Load", string.Format("{0} %", memory.dwMemoryLoad.ToString("N")));
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

        private void GetSystemProcessorInfo_WMI(DataTable table)
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
                        Assign(table, "Processor", sb.ToString());
                    }
                    catch (Exception exception)
                    {
                        Assign(table, "Exception Occurs", exception.ToString());
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void GetSystemProcessorInfo_proc(DataTable table)
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
                        Assign(table, n, v);
                    }
                }
            }
        }

        private DataTable GetSystemProcessorInfo()
        {
            var table = GenerateDataTable("Processor Information");
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Assign(table, "Number of Processors", Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
                    Assign(table, "Processor Id", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"));
                    var cpu = SystemInfo.Cpu;
                    Assign(table, "Processor Type", cpu.dwProcessorType.ToString());
                    Assign(table, "Processor Level", cpu.dwProcessorLevel.ToString());
                    Assign(table, "Processor OEM Id", cpu.dwOemId.ToString());
                    Assign(table, "Page Size", cpu.dwPageSize.ToString());
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

        private DataTable GetServerVariables()
        {
            var table = GenerateDataTable("Server Variables");
            foreach (var key in Request.ServerVariables.AllKeys)
            {
                Assign(table, key, Request.ServerVariables[key]);
            }
            return table;
        }

        private DataTable GetEnvironmentVariables()
        {
            var table = GenerateDataTable("Environment Variables");
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Assign(table, de.Key.ToString(), de.Value.ToString());
            }
            return table;
        }

        private DataTable GetSystemObjectInfo()
        {
            var table = GenerateDataTable("System COM Component Information");
            Assign(table, "Adodb.Connection", TestObject("Adodb.Connection").ToString());
            Assign(table, "Adodb.RecordSet", TestObject("Adodb.RecordSet").ToString());
            Assign(table, "Adodb.Stream", TestObject("Adodb.Stream").ToString());
            Assign(table, "Scripting.FileSystemObject", TestObject("Scripting.FileSystemObject").ToString());
            Assign(table, "Microsoft.XMLHTTP", TestObject("Microsoft.XMLHTTP").ToString());
            Assign(table, "WScript.Shell", TestObject("WScript.Shell").ToString());
            Assign(table, "MSWC.AdRotator", TestObject("MSWC.AdRotator").ToString());
            Assign(table, "MSWC.BrowserType", TestObject("MSWC.BrowserType").ToString());
            Assign(table, "MSWC.NextLink", TestObject("MSWC.NextLink").ToString());
            Assign(table, "MSWC.Tools", TestObject("MSWC.Tools").ToString());
            Assign(table, "MSWC.Status", TestObject("MSWC.Status").ToString());
            Assign(table, "MSWC.Counters", TestObject("MSWC.Counters").ToString());
            Assign(table, "IISSample.ContentRotator", TestObject("IISSample.ContentRotator").ToString());
            Assign(table, "IISSample.PageCounter", TestObject("IISSample.PageCounter").ToString());
            Assign(table, "MSWC.PermissionChecker", TestObject("MSWC.PermissionChecker").ToString());
            return table;
        }

        private DataTable GetMailObjectInfo()
        {
            var table = GenerateDataTable("Mail COM Component Information");
            Assign(table, "JMail.SMTPMail", TestObject("JMail.SMTPMail").ToString());
            Assign(table, "JMail.Message", TestObject("JMail.Message").ToString());
            Assign(table, "CDONTS.NewMail", TestObject("CDONTS.NewMail").ToString());
            Assign(table, "CDO.Message", TestObject("CDO.Message").ToString());
            Assign(table, "Persits.MailSender", TestObject("Persits.MailSender").ToString());
            Assign(table, "SMTPsvg.Mailer", TestObject("SMTPsvg.Mailer").ToString());
            Assign(table, "DkQmail.Qmail", TestObject("DkQmail.Qmail").ToString());
            Assign(table, "SmtpMail.SmtpMail.1", TestObject("SmtpMail.SmtpMail.1").ToString());
            Assign(table, "Geocel.Mailer.1", TestObject("Geocel.Mailer.1").ToString());
            return table;
        }

        private DataTable GetUploadObjectInfo()
        {
            var table = GenerateDataTable("Upload COM Component Information");
            Assign(table, "LyfUpload.UploadFile", TestObject("LyfUpload.UploadFile").ToString());
            Assign(table, "Persits.Upload", TestObject("Persits.Upload").ToString());
            Assign(table, "Ironsoft.UpLoad", TestObject("Ironsoft.UpLoad").ToString());
            Assign(table, "aspcn.Upload", TestObject("aspcn.Upload").ToString());
            Assign(table, "SoftArtisans.FileUp", TestObject("SoftArtisans.FileUp").ToString());
            Assign(table, "SoftArtisans.FileManager", TestObject("SoftArtisans.FileManager").ToString());
            Assign(table, "Dundas.Upload", TestObject("Dundas.Upload").ToString());
            Assign(table, "w3.upload", TestObject("w3.upload").ToString());
            return table;
        }

        private DataTable GetGraphicsObjectInfo()
        {
            var table = GenerateDataTable("Graphics COM Component Information");
            Assign(table, "SoftArtisans.ImageGen", TestObject("SoftArtisans.ImageGen").ToString());
            Assign(table, "W3Image.Image", TestObject("W3Image.Image").ToString());
            Assign(table, "Persits.Jpeg", TestObject("Persits.Jpeg").ToString());
            Assign(table, "XY.Graphics", TestObject("XY.Graphics").ToString());
            Assign(table, "Ironsoft.DrawPic", TestObject("Ironsoft.DrawPic").ToString());
            Assign(table, "Ironsoft.FlashCapture", TestObject("Ironsoft.FlashCapture").ToString());
            return table;
        }

        private DataTable GetOtherObjectInfo()
        {
            var table = GenerateDataTable("Other COM Component Information");
            Assign(table, "dyy.zipsvr", TestObject("dyy.zipsvr").ToString());
            Assign(table, "hin2.com_iis", TestObject("hin2.com_iis").ToString());
            Assign(table, "Socket.TCP", TestObject("Socket.TCP").ToString());
            return table;
        }

        private DataTable GetSessionInfo()
        {
            var table = GenerateDataTable("Session Information");
            Assign(table, "Session Count", Session.Contents.Count.ToString());
            Assign(table, "Application Count", Application.Contents.Count.ToString());
            return table;
        }

        private DataTable GetRequestHeaderInfo()
        {
            var table = GenerateDataTable("Request Headers");
            foreach (var key in Request.Headers.AllKeys)
            {
                Assign(table, key, Request.Headers[key]);
            }
            return table;
        }

        #endregion
    }
}