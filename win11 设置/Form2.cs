using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using win11_设置.Properties;

namespace win11_设置
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private List<string> cmbitem = new List<string>() { "右键菜单组1", "右键菜单组2", "右键菜单组3", "右键菜单组4", "右键菜单组5" };
        private int num = 0;
        private void Form2_Load(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "1、本工具集成火绒右键菜单管理\r\n2、 本工具提供右键菜单相关联注册表地址";
            this.comboBox1.DataSource = cmbitem;
            this.comboBox1.SelectedIndex = 0;
            // GetGroup();
        }
        private void GetGroup()
        {
            RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"Directory\Background\shell", true);
            string[] names = regkey.GetSubKeyNames();
            foreach (string item in names)
            {
                if (item.Contains("右键菜单组"))
                {
                    this.comboBox1.Items.Add(item);
                    int temp = int.Parse(Regex.Match(item, "\\d").Value);
                    if (temp > num)
                    {
                        num = temp;
                    }
                }
            }
            if (this.comboBox1.Items.Count>0)
            {
                this.comboBox1.SelectedIndex = 0;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //num++;
            //this.comboBox1.Items.Add("右键菜单组" + num);
            //CreateGroup("右键菜单组" + num);
            //this.openFileDialog1.Filter = "可执行程序|*.exe|所有文件|*.*";
            this.openFileDialog1.Filter = "可执行程序|*.exe";
            this.openFileDialog1.FilterIndex = 1;
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                CreateGroup((string)this.comboBox1.SelectedValue, this.openFileDialog1.FileName);
            }
            
        }
        private void CreateGroup(string groupname, string filename)
        {
            //HKEY_CLASSES_ROOT\Directory\Background\shell
            
            RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"Directory\Background\shell", true);
            string name = regkey.GetSubKeyNames().FirstOrDefault(t=> t== groupname);
            RegistryKey regkey1 = null;
            if (string.IsNullOrEmpty(name))
            {
                regkey.CreateSubKey(groupname, true);
                regkey.OpenSubKey(groupname, true).SetValue("Icon", "networkexplorer.dll,2");
                regkey.OpenSubKey(groupname, true).SetValue("MUIVerb", "");
                regkey.OpenSubKey(groupname, true).SetValue("SubCommands", "");
                regkey1 = regkey.OpenSubKey(groupname, true).CreateSubKey("Shell", true);
            }
            else
            {
                regkey1 = regkey.OpenSubKey(groupname, true).OpenSubKey("Shell", true);
            }
            AddRK(filename, regkey1);
        }

        private void AddRK(string filename, RegistryKey regkey)
        {
            //HKEY_CLASSES_ROOT\Directory\Background\shell
            int a = filename.LastIndexOf("\\");
            int b = filename.LastIndexOf(".exe");
            string fn = filename.Substring(a + 1, b - a - 1);
            string name = regkey.GetSubKeyNames().FirstOrDefault(t => t == fn);
            if (string.IsNullOrEmpty(name))
            {
                regkey.CreateSubKey(fn, true);
            }
            regkey.OpenSubKey(fn, true).SetValue("Icon", filename);
            string cmm = regkey.OpenSubKey(fn, true).GetSubKeyNames().FirstOrDefault(t => t == "command");
            if (string.IsNullOrEmpty(cmm))
            {
                regkey.OpenSubKey(fn, true).CreateSubKey("command", true);
            }
            regkey.OpenSubKey(fn, true).OpenSubKey("command", true).SetValue("", filename);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "可执行程序|*.exe";
            this.openFileDialog1.FilterIndex = 1;
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"Directory\Background\shell", true);
                AddRK(this.openFileDialog1.FileName, regkey);
            }
        }

        private void AddRKOpenwith(string filename, RegistryKey regkey)
        {
            //HKEY_CLASSES_ROOT\*\shell\NotePad2
            int a = filename.LastIndexOf("\\");
            int b = filename.LastIndexOf(".exe");
            string fn = filename.Substring(a + 1, b - a - 1);
            string name = regkey.GetSubKeyNames().FirstOrDefault(t => t == fn);
            if (string.IsNullOrEmpty(name))
            {
                regkey.CreateSubKey(fn, true);
            }
            regkey.OpenSubKey(fn, true).SetValue("", $"用 &{fn} 打开");
            regkey.OpenSubKey(fn, true).SetValue("Icon", filename);
            string cmm = regkey.OpenSubKey(fn, true).GetSubKeyNames().FirstOrDefault(t => t == "command");
            if (string.IsNullOrEmpty(cmm))
            {
                regkey.OpenSubKey(fn, true).CreateSubKey("command", true);
            }
            regkey.OpenSubKey(fn, true).OpenSubKey("command", true).SetValue("", $"\"{filename}\" \"%1\"");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //HKEY_CLASSES_ROOT\*\shell\NotePad2
            this.openFileDialog1.Filter = "可执行程序|*.exe";
            this.openFileDialog1.FilterIndex = 1;
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"*\shell", true);
                AddRKOpenwith(this.openFileDialog1.FileName, regkey);
            }
        }

        private void AddRoot()
        {
            RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"*\shell", true);
            string fn = "runas";
            string name = regkey.GetSubKeyNames().FirstOrDefault(t => t == fn);
            if (string.IsNullOrEmpty(name))
            {
                regkey.CreateSubKey(fn, true);
            }
            regkey.OpenSubKey(fn, true).SetValue("", "管理员获取所有权");
            regkey.OpenSubKey(fn, true).SetValue("Icon", "imageres.dll,102");
            regkey.OpenSubKey(fn, true).SetValue("NoWorkingDirectory", "");
            string[] names = regkey.OpenSubKey(fn, true).GetSubKeyNames();
            string cmm = names.FirstOrDefault(t => t == "command");
            if (string.IsNullOrEmpty(cmm))
            {
                regkey.OpenSubKey(fn, true).CreateSubKey("command", true);
            }
            regkey.OpenSubKey(fn, true).OpenSubKey("command", true).SetValue("", "cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F");
            //string icmm = regkey.OpenSubKey(fn, true).OpenSubKey("command", true).GetSubKeyNames().FirstOrDefault(t => t == "IsolatedCommand");
            //if (string.IsNullOrEmpty(icmm))
            //{
            //    regkey.OpenSubKey(fn, true).OpenSubKey("command", true).CreateSubKey("IsolatedCommand", true);
            //}
            regkey.OpenSubKey(fn, true).OpenSubKey("command", true).SetValue("IsolatedCommand", "cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddRoot();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //记忆功能把展开的路径保存在HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit里名称为LastKey的Value里
            RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true);
            regkey.SetValue("LastKey", @"计算机\HKEY_CLASSES_ROOT\Directory\Background\shell");
            Process[] p = Process.GetProcessesByName("regedit");
            foreach (var item in p)
            {
                item.Kill();
            }
            System.Threading.Thread.Sleep(1000);
            Process.Start("regedit.exe");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //记忆功能把展开的路径保存在HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit里名称为LastKey的Value里
            RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true);
            regkey.SetValue("LastKey", @"计算机\HKEY_CLASSES_ROOT\*\shell");
            Process[] p = Process.GetProcessesByName("regedit");
            foreach (var item in p)
            {
                item.Kill();
            }
            System.Threading.Thread.Sleep(1000);
            Process.Start("regedit.exe");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShiFangZY();
        }
        private void ShiFangZY()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP") + "\\RightClickMan.exe";
            if (!File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    fs.Write(Resources.RightClickMan, 0, Resources.RightClickMan.Length);
                }
            }
            Process.Start(path);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP") + "\\RightClickMan.exe";
            if (File.Exists(path))
            {
                int ps = Process.GetProcessesByName("RightClickMan").Length;
                if (ps > 0)
                {
                    MessageBox.Show("火绒右键菜单管理工具在运行，请先关闭!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
                File.Delete(path);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64).OpenSubKey(".DEFAULT\\Control Panel\\Keyboard",true);
            if (key != null) 
            {
                key.SetValue("InitialKeyboardIndicators", "80000002"); //默认值2147473648   小键盘80000002
                key.Close();
            }

        }
        //pdf 右键合并
        //[HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\Foxit_ConvertToPDF]
        //@="{C5269811-4A29-4818-A4BB-111F9FC63A5F}"
        //[HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\Adobe.Acrobat.ContextMenu]
        //@="{A6595CD1-BF77-430A-A452-18696685F7C7}"
    }
}
