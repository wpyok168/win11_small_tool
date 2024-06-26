using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using win11_设置.Properties;

namespace win11_设置
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool flag1 = true;
        private bool flag2 = true;
        private bool flag3 = true;
        private bool flag4 = true;
        private void Form1_Load(object sender, EventArgs e)
        {
            //SetTPM();
            GetRJ1();
            GetControl(this.button3, "控制面板");
            GetControl(this.button5, "程序和功能");
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn() { ColumnName = "name" };
            DataColumn dc2 = new DataColumn() { ColumnName = "value" };
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            DataRow dr1 = dt.NewRow();
            dr1["name"] = "常规";
            dr1["value"] = "1";
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2["name"] = "小尺寸";
            dr2["value"] = "0";
            dt.Rows.Add(dr2);
            DataRow dr3 = dt.NewRow();
            dr3["name"] = "大尺寸";
            dr3["value"] = "2";
            dt.Rows.Add(dr3);
            this.comboBox1.DataSource = dt;
            this.comboBox1.DisplayMember = "name";
            this.comboBox1.ValueMember = "value";
            //this.comboBox1.SelectedIndex = -1;
            DevCombobox();
            RWLCombobox();
            GetRWL();
            GetDev();
            GetRWLTaskbarAl();
            
        }
        private void RWLCombobox()
        {
            List<CBListItem> list = new List<CBListItem>();
            CBListItem cb0 = new CBListItem() { Name = "左边", Value = "0" };
            CBListItem cb1 = new CBListItem() { Name = "居中", Value = "1" };
            list.Add(cb0); list.Add(cb1);
            this.comboBox3.DataSource = list;
            this.comboBox3.DisplayMember = "Name";
            this.comboBox3.ValueMember = "Value";
            this.comboBox3.SelectedIndex = -1;
        }
        private void GetRWLTaskbarAl()
        {
            //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
            try
            {
                string[] names = reg.GetValueNames();
                string name = reg.GetValueNames().FirstOrDefault(t => t == "TaskbarAl");
                if (string.IsNullOrEmpty(name))
                {
                    this.comboBox3.SelectedIndex = -1;
                }
                else
                {
                    if (reg.GetValue("TaskbarAl").ToString().ToLower().Contains("0"))
                    {
                        this.comboBox3.SelectedIndex = 0;
                    }
                    else if (reg.GetValue("TaskbarAl").ToString().ToLower().Contains("1"))
                    {
                        this.comboBox3.SelectedIndex = 1;
                    }
                    else
                    {
                        this.comboBox3.SelectedIndex = -1;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            flag3 = false;
            flag4 = false;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flag4)
            {
                return;
            }
            CBListItem drv = (CBListItem)this.comboBox3.SelectedItem;
            if (drv == null)
            {
                return;
            }
            SetTaskbarAl(drv.Value);
        }
        private void SetTaskbarAl(string value)
        {
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true); 
            try
            {
                string name = reg.GetValueNames().FirstOrDefault(t => t == "TaskbarAl");
                if (string.IsNullOrEmpty(name))
                {
                    reg.CreateSubKey("TaskbarAl");
                    reg.SetValue("TaskbarAl", value, Microsoft.Win32.RegistryValueKind.DWord);
                }
                else
                {
                    reg.SetValue("TaskbarAl", value, Microsoft.Win32.RegistryValueKind.DWord);
                }
                this.comboBox3.SelectedIndex = int.Parse(reg.GetValue("TaskbarAl").ToString());
            }
            catch (Exception ex)
            {

            }
        }

        private void SetRWL(int sizenum)
        {
            if (flag2)
            {
                return;
            }
            //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
            try
            {
                string num = reg.GetValue("TaskbarSi").ToString();
            }
            catch (Exception)
            {
                reg.CreateSubKey("TaskbarSi");
            }
            switch (sizenum)
            {
                case 0:
                    reg.SetValue("TaskbarSi", 0);
                    break;
                case 1:
                    reg.SetValue("TaskbarSi", 1);
                    break;
                case 2:
                    reg.SetValue("TaskbarSi", 2);
                    break;
                default:
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object drv = this.comboBox1.SelectedValue;
            if (drv == null)
            {
                return;
            }
            if (drv.GetType().Name.ToLower().Equals("datarowview"))
            {
                return;
            }
            SetRWL(int.Parse(drv.ToString()));
            Reboot();
        }
        private void GetRWL()
        {
            //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
            try
            {
                string name = reg.GetValueNames().FirstOrDefault(t => t == "TaskbarSi");
                if (string.IsNullOrEmpty(name))
                {
                    this.comboBox1.SelectedIndex = -1;
                }
                else
                {
                    int num = int.Parse(reg.GetValue("TaskbarSi").ToString());
                    switch (num)
                    {
                        case 0:
                            this.comboBox1.SelectedIndex = 1;
                            break;
                        case 1:
                            this.comboBox1.SelectedIndex = 0;
                            break;
                        case 2:
                            this.comboBox1.SelectedIndex = 2;
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            flag2 = false;
        }
        private void GetRJ1()
        {
            //HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32  右键
            //reg.exe add "HKCU\Software\Classes\CLSID\{d93ed569-3b3e-4bff-8355-3c44f6a52bb5}\InprocServer32" /f /ve  菜单栏
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser,Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"Software\Classes\CLSID", true);
            try
            {
                string name = reg.GetSubKeyNames().FirstOrDefault(t => t == "{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
                if (string.IsNullOrEmpty(name))
                {
                    this.checkBox1.Text = "win11 右键样式";
                    this.checkBox1.Checked = false;
                }
                else
                {
                    this.checkBox1.Text = "win10 右键样式";
                    this.checkBox1.Checked = true;
                }
                string name1 = reg.GetSubKeyNames().FirstOrDefault(t => t == "{d93ed569-3b3e-4bff-8355-3c44f6a52bb5}");
                if (string.IsNullOrEmpty(name1))
                {
                    this.checkBox3.Text = "win11 菜单栏样式";
                    this.checkBox3.Checked = false;
                }
                else
                {
                    this.checkBox3.Text = "win10 菜单栏样式";
                    this.checkBox3.Checked = true;
                }

            }
            catch (Exception ex)
            {

            }
            flag1 = false;
        }
        private void SetRJ1(CheckBox checkBox, string subkeyname)
        {
            if (flag1)
            {
                return;
            }
            //HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"Software\Classes\CLSID", true);
            try
            {
                string name = reg.GetSubKeyNames().FirstOrDefault(t => t == subkeyname);
                if (checkBox.Checked == false)
                {
                    if (subkeyname.Equals("{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}"))
                    {
                        checkBox.Text = "win11 右键样式";
                    }
                    else if (subkeyname.Equals("{d93ed569-3b3e-4bff-8355-3c44f6a52bb5}"))
                    {
                        checkBox.Text = "win11 菜单栏样式";
                    }
                    
                    if (!string.IsNullOrEmpty(name))
                    {
                        Microsoft.Win32.RegistryKey reg1 = reg.OpenSubKey(name, true);
                        string name1 = reg1.GetSubKeyNames().FirstOrDefault(t => t == "InprocServer32");
                        if (!string.IsNullOrEmpty(name1))
                        {
                            reg1.DeleteSubKey("InprocServer32");
                        }
                        reg.DeleteSubKey(subkeyname);
                        //Runcmd("reg delete \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\" /f");
                    }
                }
                else
                {
                    if (subkeyname.Equals("{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}"))
                    {
                        checkBox.Text = "win10 右键样式";
                    }
                    else if (subkeyname.Equals("{d93ed569-3b3e-4bff-8355-3c44f6a52bb5}"))
                    {
                        checkBox.Text = "win10 菜单栏样式";
                    }
                    
                    if (string.IsNullOrEmpty(name))
                    {
                        reg.CreateSubKey(subkeyname);
                        reg.OpenSubKey(subkeyname, true).CreateSubKey("InprocServer32");
                        reg.OpenSubKey(subkeyname, true).OpenSubKey("InprocServer32", true).SetValue("", "");
                        //Runcmd("reg add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f /ve");
                    }
                }
                Reboot();
            }
            catch (Exception ex)
            {

            }
        }
        #region  失效右键方案
        /// <summary>
        /// 失效右键方案
        /// </summary>
        private void GetRJ()
        {
            //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FeatureManagement\Overrides\4
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\FeatureManagement\Overrides\4", true);
            try
            {
                string name = reg.GetSubKeyNames().FirstOrDefault(t => t == "586118283");
                if (string.IsNullOrEmpty(name))
                {
                    this.checkBox1.Text = "win11 右键样式";
                    this.checkBox1.Checked = false;
                }
                else
                {
                    this.checkBox1.Text = "win10 右键样式";
                    this.checkBox1.Checked = true;
                }

            }
            catch (Exception ex)
            {

            }
            flag1 = false;
        }
        /// <summary>
        /// 右键切换失效方案
        /// </summary>
        private void SetRJ()
        {
            if (flag1)
            {
                return;
            }
            //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FeatureManagement\Overrides\4
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\FeatureManagement\Overrides\4", true);
            try
            {
                string name = reg.GetSubKeyNames().FirstOrDefault(t => t == "586118283");
                if (this.checkBox1.Checked == false)
                {
                    this.checkBox1.Text = "win11 右键样式";
                    if (!string.IsNullOrEmpty(name))
                    {
                        Microsoft.Win32.RegistryKey reg1 = reg.OpenSubKey(name, true);
                        string[] names = reg1.GetValueNames();
                        foreach (var item in names)
                        {
                            reg1.DeleteSubKey(item);
                        }
                        reg.DeleteSubKey("586118283");
                    }
                }
                else
                {
                    this.checkBox1.Text = "win10 右键样式";
                    if (string.IsNullOrEmpty(name))
                    {
                        reg.CreateSubKey("586118283");
                        name = "586118283";
                        reg.OpenSubKey(name, true).CreateSubKey("EnabledState");
                        reg.OpenSubKey(name, true).SetValue("EnabledState", 1);
                        reg.OpenSubKey(name, true).CreateSubKey("EnabledStateOptions");
                        reg.OpenSubKey(name, true).SetValue("EnabledStateOptions", 1);
                        reg.OpenSubKey(name, true).CreateSubKey("Variant");
                        reg.OpenSubKey(name, true).SetValue("Variant", 0);
                        reg.OpenSubKey(name, true).CreateSubKey("VariantPayload");
                        reg.OpenSubKey(name, true).SetValue("VariantPayload", 0);
                        reg.OpenSubKey(name, true).CreateSubKey("VariantPayloadKind");
                        reg.OpenSubKey(name, true).SetValue("VariantPayloadKind", 0);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetRJ1(this.checkBox1, "{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SetRJ1(this.checkBox3, "{d93ed569-3b3e-4bff-8355-3c44f6a52bb5}");
        }
        private void GetDev()
        {
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\Applicability");
            try
            {
                string[] names = reg.GetValueNames();
                string name = reg.GetValueNames().FirstOrDefault(t => t == "BranchName");
                if (string.IsNullOrEmpty(name))
                {
                    this.checkBox2.Text = "Normal";
                    this.checkBox2.Checked = false;
                    this.comboBox2.SelectedIndex = 0;
                }
                else
                {
                    if (reg.GetValue("BranchName").ToString().ToLower().Contains("dev"))
                    {
                        this.checkBox2.Text = reg.GetValue("BranchName").ToString();
                        this.checkBox2.Checked = true;
                        this.comboBox2.SelectedIndex = 2;
                    }
                    else if (reg.GetValue("BranchName").ToString().ToLower().Contains("releasepreview"))
                    {
                        this.checkBox2.Text = reg.GetValue("BranchName").ToString();
                        this.checkBox2.Checked = false;
                        this.comboBox2.SelectedIndex = 1;
                    }
                    else if (reg.GetValue("BranchName").ToString().ToLower().Contains("Beta"))
                    {
                        this.checkBox2.Text = reg.GetValue("BranchName").ToString();
                        this.checkBox2.Checked = false;
                        this.comboBox2.SelectedIndex = 3;
                    }
                    else
                    {
                        this.checkBox2.Text = reg.GetValue("BranchName").ToString();
                        this.checkBox2.Checked = false;
                        this.comboBox2.SelectedIndex = 1;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            flag3 = false;
            flag4 = false;
        }
        private void DevCombobox()
        {
            List<CBListItem> list = new List<CBListItem>();
            CBListItem cb0 = new CBListItem() { Name = "Normal", Value = "0" };
            CBListItem cb1 = new CBListItem() { Name = "ReleasePreview", Value = "1" };
            CBListItem cb2 = new CBListItem() { Name= "Dev" , Value="2"};
            //CBListItem cb3 = new CBListItem() { Name = "Beta", Value = "3" }; //beta会自动跳成ReleasePreview
            list.Add(cb0); list.Add(cb1); list.Add(cb2); //list.Add(cb3);
            this.comboBox2.DataSource = list;
            this.comboBox2.DisplayMember = "Name";
            this.comboBox2.ValueMember = "Value";
            this.comboBox2.SelectedIndex = -1;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flag4)
            {
                return;
            }
            CBListItem drv = (CBListItem)this.comboBox2.SelectedItem;
            if (drv == null)
            {
                return;
            }
            SetDevRoot(drv.Name);
            //object drv = this.comboBox2.SelectedItem;
            //if (string.IsNullOrEmpty(drv))
            //{
            //    return;
            //}
            //SetDevRoot(drv);
        }

        private void DevChange()
        {
            if (flag3)
            {
                return;
            }
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\Applicability
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection
            if (this.checkBox2.Checked)
            {
                SetDevRoot("Dev");
            }
            else
            {
                SetDevRoot("ReleasePreview");
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            DevChange();
        }
        private void SetDevRoot(string value)
        {
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", true);
            Microsoft.Win32.RegistryKey reg1 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\UI\Selection", true);
            try
            {
                string name = reg.GetValueNames().FirstOrDefault(t => t == "BranchName");
                if (string.IsNullOrEmpty(name))
                {
                    reg.CreateSubKey("BranchName");
                    reg.SetValue("BranchName", value);
                    reg1.SetValue("UIBranch", value);
                }
                else
                {
                    reg.SetValue("BranchName", value);
                    reg1.SetValue("UIBranch", value);
                }
                this.checkBox2.Text = reg.GetValue("BranchName").ToString();
            }
            catch (Exception ex)
            {

            }
        }
        
        /// <summary>
        /// 调用PowerShell运行TPM脚本
        /// </summary>
        private void SetTPM()
        {
            //https://github.com/AveYo/MediaCreationTool.bat/tree/main/bypass11
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                PowerShell ps = PowerShell.Create();
                
                ps.Runspace = runspace;
                ps.AddScript(Resources.tpmstr1);
                ps.Invoke();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetTPM();
        }

        private string Runcmd(string cmdstr)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            //p.StandardInput.WriteLine(cmdstr + "&exit");
            p.StandardInput.WriteLine(cmdstr + "&exit");
            p.StandardInput.AutoFlush = true;
            string outstr = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return outstr;
        }
        private void RebootExplorer()
        {
            //Process[] ps = Process.GetProcessesByName("explorer.exe");
            //foreach (Process item in ps)
            //{
            //    item.Kill();
            //}
            //Process.Start("taskkill", "/F /IM explorer.exe");
            Process.Start("c://windows//explorer.exe");
        }
        private void Reboot()
        {
            Runcmd("taskkill /f /im explorer.exe");
            // Runcmd("start c://windows//explorer.exe");
            RebootExplorer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetMoSetup(1);
        }
        private void SetMoSetup(int value)
        {
            //屏蔽TPM value为1 移除屏蔽为 0
            //HKEY_LOCAL_MACHINE\SYSTEM\Setup\MoSetup
            //HKEY_LOCAL_MACHINE\SYSTEM\Setup\LabConfig
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\vdsldr.exe
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"SYSTEM\Setup\MoSetup", true);
            Microsoft.Win32.RegistryKey reg1 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"SYSTEM\Setup", true);
            Microsoft.Win32.RegistryKey reg2 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", true);
            try
            {
                string degbugvalue = "cmd /q Skip TPM Check on Dynamic Update (c) AveYo, 2021 /d/x/r>nul (erase /f/s/q %systemdrive%\\$windows.~bt\\appraiserres.dll&md 11&cd 11&ren vd.exe vdsldr.exe&robocopy \"../\" \"./\" \"vdsldr.exe\"&ren vdsldr.exe vd.exe&start vd -Embedding)&rem;";
                string name = reg.GetValueNames().FirstOrDefault(t => t == "AllowUpgradesWithUnsupportedTPMOrCPU");
                //if (string.IsNullOrEmpty(name))
                //{
                //    reg.CreateSubKey("AllowUpgradesWithUnsupportedTPMOrCPU");
                //}
                reg.SetValue("AllowUpgradesWithUnsupportedTPMOrCPU", value, Microsoft.Win32.RegistryValueKind.DWord);

                string name1 = reg1.GetValueNames().FirstOrDefault(t => t == "LabConfig");
                if (string.IsNullOrEmpty(name1))
                {
                    reg1.CreateSubKey("LabConfig");
                }
                reg1.OpenSubKey("LabConfig", true).SetValue("BypassSecureBootCheck", value, Microsoft.Win32.RegistryValueKind.DWord);

                //string name2 = reg2.GetValueNames().FirstOrDefault(t => t == "BypassTPMCheck");
                //if (string.IsNullOrEmpty(name2))
                //{
                //    reg1.CreateSubKey("BypassTPMCheck");
                //}
                reg1.OpenSubKey("LabConfig", true).SetValue("BypassTPMCheck", value, Microsoft.Win32.RegistryValueKind.DWord);

                string name3 = reg2.GetValueNames().FirstOrDefault(t => t == "vdsldr.exe");
                if (string.IsNullOrEmpty(name3))
                {
                    reg2.CreateSubKey("vdsldr.exe");
                }
                //string dn = reg2.OpenSubKey("vdsldr.exe", true).GetValueNames().FirstOrDefault(t => t == "Debugger");
                //if (string.IsNullOrEmpty(dn))
                //{
                //    reg2.OpenSubKey("vdsldr.exe", true).CreateSubKey("Debugger", true);
                //}
                reg2.OpenSubKey("vdsldr.exe", true).SetValue("Debugger", degbugvalue);
            }
            catch (Exception ex)
            {

            }
        }
        private void ShiFangZY()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP")+ "\\去除水印及弹窗.exe";
            if (!File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    fs.Write(Resources.去除水印及弹窗, 0, Resources.去除水印及弹窗.Length);
                }
            }
            Process.Start(path);
        }

        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {
            //计算机\HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{ab0b37ec-56f6-4a0e-a8fd-7a8bf7c2da96}\InProcServer32
            //@= "%SystemRoot%\system32\explorerframe.dll"

            //原始文件： explorerframe.dll
            //X86去水印文件：ExplorerFrames32.dll
            //X64去水印文件：ExplorerFrames64.dll

            //%SystemRoot%\system32\LicensingUI.exe.mui, 这里就是存储试用信息 ，去除过期弹窗 ，删掉或重命名
            //C:\Windows\Branding\Basebrd\zh-CN\basebrd.dll.mui 去除预览版
            //%SystemRoot%\system32\winver.exe.mui     去掉过期日期       
            ShiFangZY();
        }


        private void toolStripStatusLabel6_Click(object sender, EventArgs e)
        {
            //powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 
            //powercfg /q
            //rundll32.exe shell32.dll,Control_RunDLL powercfg.cpl,,1
            //"C:\WINDOWS\system32\rundll32.exe" shell32.dll,Control_RunDLL PowerCfg.cpl @0,/editplan:381b4222-f694-41f0-9685-ff5bb260df2e
            /*
             *电源方案 GUID: 381b4222-f694-41f0-9685-ff5bb260df2e  (平衡)
             *电源方案 GUID: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c  (高性能)
             *电源方案 GUID: a1841308-3541-4fab-bc81-f71556f20b4a  (节能)
             *电源方案 GUID: e9a42b02-d5df-448d-aa00-03f14749eb61  (卓越性能)
             */
            Runcmd("rundll32.exe shell32.dll,Control_RunDLL powercfg.cpl,,1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetControl(sender as Button, "控制面板", "imageres.dll,-27", "explorer.exe shell:::{21EC2020-3AEA-1069-A2DD-08002B30309D}");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            SetControl(sender as Button, "程序和功能", "", "Rundll32.exe shell32.dll,Control_RunDLL appwiz.cpl,,0");
        }
        private void SetControl(Button button, string right_name,string icon, string command)
        {
            //HKEY_CLASSES_ROOT\Directory\Background\shell\控制面板
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.ClassesRoot, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"Directory\Background\shell", true);
            try
            {
                string name = reg.GetSubKeyNames().FirstOrDefault(t => t == right_name);
                if (button.Text.Equals($"添加右键{right_name}"))
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        reg.CreateSubKey(right_name, true);
                        //reg.OpenSubKey("控制面板", true).SetValue("Icon", "\"C:\\Windows\\System32\\control.exe\"");
                        reg.OpenSubKey(right_name, true).SetValue("Icon", icon);
                        string name1 = reg.OpenSubKey(right_name, true).GetSubKeyNames().FirstOrDefault(t => t == "command");
                        if (string.IsNullOrEmpty(name1))
                        {
                            reg.OpenSubKey(right_name, true).CreateSubKey("command", true);
                        }
                        //reg.OpenSubKey("控制面板", true).OpenSubKey("command", true).SetValue("", "\"C:\\Windows\\System32\\control.exe\"");
                        reg.OpenSubKey(right_name, true).OpenSubKey("command", true).SetValue("", command);
                        button.Text = $"删除右键{right_name}";
                    }
                    else
                    {
                        MessageBox.Show($"{right_name}添加失败");
                    }
                }
                else if (button.Text.Equals($"删除右键{right_name}"))
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        //string[] names1 = reg.OpenSubKey("控制面板", true).GetSubKeyNames();
                        //if (names1.Length > 0)
                        //{
                        //    foreach (var item in names1)
                        //    {
                        //        string[] names2 = reg.OpenSubKey("控制面板", true).OpenSubKey(item, true).GetValueNames();
                        //        if (names2.Length > 0)
                        //        {
                        //            foreach (var item1 in names2)
                        //            {
                        //                reg.OpenSubKey("控制面板", true).OpenSubKey(item, true).DeleteSubKey(item1);
                        //            }
                        //        }
                        //        reg.OpenSubKey("控制面板", true).DeleteSubKey(item);
                        //    }
                        //}
                        reg.DeleteSubKeyTree(right_name);
                        button.Text = $"添加右键{right_name}";
                    }
                    else
                    {
                        MessageBox.Show($"{right_name}删除失败");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void GetControl(Button button, string right_name)
        {
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.ClassesRoot, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(@"Directory\Background\shell", true);
            try
            {
                string name = reg.GetSubKeyNames().FirstOrDefault(t => t == right_name);
                if (string.IsNullOrEmpty(name))
                {
                    button.Text = $"添加右键{right_name}";
                }
                else
                {
                    button.Text = $"删除右键{right_name}";
                }
            }
            catch (Exception ex)
            {

            }
        }
        

        private void toolStripStatusLabel8_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string name = "去除水印及弹窗";
            string path = System.Environment.GetEnvironmentVariable("TEMP") + $"\\{name}.exe";
            if (File.Exists(path))
            {
                int ps = Process.GetProcessesByName(name).Length;
                if (ps > 0)
                {
                    MessageBox.Show("去水印工具在运行，请先关闭!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
                File.Delete(path);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.ClassesRoot, Microsoft.Win32.RegistryView.Registry64);
            Microsoft.Win32.RegistryKey reg1 = reg.OpenSubKey(".txt", true);
            if (reg1 == null)
            {
                reg.CreateSubKey(".txt", true);
                reg1 = reg.OpenSubKey(".txt", true);
                reg1.SetValue("", "txtfilelegacy");
                reg1.SetValue("Content Type", "text/plain");
                reg1.SetValue("PerceivedType", "text");
            }
            Microsoft.Win32.RegistryKey reg2 = reg1.CreateSubKey("ShellNew", true);
            reg2.SetValue("NullFile", "");

            Microsoft.Win32.RegistryKey reg3 = reg.OpenSubKey("txtfilelegacy", true);
            if (reg3 == null)
            {
                reg.CreateSubKey("txtfilelegacy", true);
                reg3 = reg.OpenSubKey("txtfilelegacy", true);
                reg3.CreateSubKey("shell", true);
                Microsoft.Win32.RegistryKey reg4 = reg3.OpenSubKey("shell", true).CreateSubKey("printto", true);
                reg4.SetValue("NeverDefault", "");
                reg4.CreateSubKey("command", true);
                //%SystemRoot%\system32\notepad.exe /pt "%1" "%2" "%3" "%4"
                reg4.OpenSubKey("command", true).SetValue("", "%SystemRoot%\\system32\\notepad.exe /pt \"%1\" \"%2\" \"%3\" \"%4\"");
            }
            reg3.SetValue("", "文本文档");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Reboot();
        }

        
    }
}
