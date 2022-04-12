using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeAreDevs_API;
using KrnlAPI;

namespace ScarPloit
{
    public partial class scarploit : Form
    {
        readonly ExploitAPI WeAreDevsAPI = new ExploitAPI();

        public static void PopulateListBox(ListBox lsb, string Folder, string FileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Folder);
            FileInfo[] Files = dinfo.GetFiles(FileType);
            foreach (FileInfo file in Files)
            {
                lsb.Items.Add(file.Name);
            }
        }

        WebClient wc = new WebClient();
        private string defPath = Application.StartupPath + "//Monaco//";

        private void AddIntel(string label, string kind, string detail, string insertText)
        {
            string text = "\"" + label + "\"";
            string text2 = "\"" + kind + "\"";
            string text3 = "\"" + detail + "\"";
            string text4 = "\"" + insertText + "\"";
            scriptbox.Document.InvokeScript("AddIntellisense", new object[]
            {
                label,
                kind,
                detail,
                insertText
            });
        }

        private void AddGlobalF()
        {
            string[] array = File.ReadAllLines(this.defPath + "//globalf.txt");
            foreach (string text in array)
            {
                bool flag = text.Contains(':');
                if (flag)
                {
                    this.AddIntel(text, "Function", text, text.Substring(1));
                }
                else
                {
                    this.AddIntel(text, "Function", text, text);
                }
            }
        }

        private void AddGlobalV()
        {
            foreach (string text in File.ReadLines(this.defPath + "//globalv.txt"))
            {
                this.AddIntel(text, "Variable", text, text);
            }
        }

        private void AddGlobalNS()
        {
            foreach (string text in File.ReadLines(this.defPath + "//globalns.txt"))
            {
                this.AddIntel(text, "Class", text, text);
            }
        }

        private void AddMath()
        {
            foreach (string text in File.ReadLines(this.defPath + "//classfunc.txt"))
            {
                this.AddIntel(text, "Method", text, text);
            }
        }

        private void AddBase()
        {
            foreach (string text in File.ReadLines(this.defPath + "//base.txt"))
            {
                this.AddIntel(text, "Keyword", text, text);
            }
        }

        public scarploit()
        {
            InitializeComponent();
            MainAPI.Load();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckInjected();
            WeAreDevsAPI.IsUpdated();
            scripts.Items.Clear();
            PopulateListBox(scripts, "./Scripts", "*.txt");
            PopulateListBox(scripts, "./Scripts", "*.lua");
            scripthubs.Items.Clear();
            PopulateListBox(scripthubs, "./Script Hubs", "*.txt");
            PopulateListBox(scripthubs, "./Script Hubs", "*.lua");

            WebClient wc = new WebClient
            {
                Proxy = null
            };
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                string friendlyName = AppDomain.CurrentDomain.FriendlyName;
                bool flag2 = registryKey.GetValue(friendlyName) == null;
                if (flag2)
                {
                    registryKey.SetValue(friendlyName, 11001, RegistryValueKind.DWord);
                }
                registryKey = null;
                friendlyName = null;
            }
            catch (Exception)
            {
            }
            scriptbox.Url = new Uri(string.Format("file:///{0}/Monaco/Monaco.html", Directory.GetCurrentDirectory()));
            scriptbox.Document.InvokeScript("SetTheme", new string[]
            {
                   "Dark"
            });
            AddBase();
            AddMath();
            AddGlobalNS();
            AddGlobalV();
            AddGlobalF();
        }

        private void attachbtn_Click(object sender, EventArgs e)
        {
            if (wadapi.Checked == true)
            {
                WeAreDevsAPI.LaunchExploit();
            }
            else if (krnlapien.Checked == true)
            {
                MainAPI.Inject();
            }
        }

        private void executebtn_Click(object sender, EventArgs e)
        {
            HtmlDocument document = scriptbox.Document;
            string scriptName = "GetText";
            object[] args = new string[0];
            object obj = document.InvokeScript(scriptName, args);
            string scripttext = obj.ToString();
            if (wadapi.Checked == true)
            {
                WeAreDevsAPI.SendLuaScript(scripttext);
            }
            else if(krnlapien.Checked == true)
            {
                MainAPI.Execute(scripttext);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            scriptbox.Document.InvokeScript("SetText", new object[]
            {
                "-- ScarSploit v2C by scar17off#3829"
            });
        }

        private void CheckInjected()
        {
            if (wadapi.Checked == true)
            {
                if (WeAreDevsAPI.isAPIAttached())
                {
                    status.Text = "Injection Status: Injected";
                }
                else
                {
                    status.Text = "Injection Status: Not Injected";
                }
            }
            else if(krnlapien.Checked == true)
            {
                if (MainAPI.IsAttached())
                {
                    status.Text = "Injection Status: Injected";
                }
                else
                {
                    status.Text = "Injection Status: Not Injected";
                }
            }
        }

        private void InjectedChecker_Tick(object sender, EventArgs e)
        {
            CheckInjected();
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            HtmlDocument document = scriptbox.Document;
            string scriptName = "GetText";
            object[] args = new string[0];
            object obj = document.InvokeScript(scriptName, args);
            string script = obj.ToString();

            try
            {
                var saveFileDialog1 = new SaveFileDialog
                {
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Scripts",
                    Filter = string.Format("{0}Text files (*.txt)|*.txt|Lua files (*.lua)|*.lua", "*.lua"),
                    RestoreDirectory = true,
                    ShowHelp = false,
                    CheckFileExists = true
                };
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) { File.WriteAllText(saveFileDialog1.FileName, script); }
            }
            catch
            {

            }
        }

        private void loadbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;

                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        var MainText = reader.ReadToEnd();
                        scriptbox.Document.InvokeScript("SetText", new object[]
                        {
                            MainText
                        });
                    }
                }
            }
        }

        private void discordbtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/59jvbFpCza");
        }

        private void websitebtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://scar17.glitch.me/scarsploit/main.html");
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            scripts.Items.Clear();
            PopulateListBox(scripts, "./Scripts", "*.txt");
            PopulateListBox(scripts, "./Scripts", "*.lua");
            scripthubs.Items.Clear();
            PopulateListBox(scripthubs, "./Script Hubs", "*.txt");
            PopulateListBox(scripthubs, "./Script Hubs", "*.lua");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void scripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.scripts.SelectedIndex != -1)
            {
                this.scriptbox.Document.InvokeScript("SetText", new object[1]
                {
                      (object) System.IO.File.ReadAllText("Scripts\\" + this.scripts.SelectedItem.ToString())
                });
            }
        }

        private void scripthubs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.scripthubs.SelectedIndex != -1)
            {
                this.scriptbox.Document.InvokeScript("SetText", new object[1]
                {
                      (object) System.IO.File.ReadAllText("Script Hubs\\" + this.scripthubs.SelectedItem.ToString())
                });
            }
        }
        
        private void siticoneButton10_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://krnl.ca/docs/");
        }

        private void waddocs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://wearedevs.net/d/Exploit%20API");
        }

        private void openfbtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", this.defPath);
        }

        private void siticoneCustomCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(siticoneCustomCheckBox1.Checked == true)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        private void loadshbtn_Click(object sender, EventArgs e)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            using (Stream stream = client.OpenRead("https://pastebin.com/raw/YRHRmsRH"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string key = reader.ReadLine();
                    if (shkey.Text == key)
                    {
                        if (wadapi.Checked == true)
                        {
                            WeAreDevsAPI.SendLuaScript("local env = getgenv()\nenv.scarhack = { }\nenv.scarhack.key = '" + shkey.Text + "'\nloadstring(game:GetHttp('https://github.com/scar17off/Roblox-Scripts/raw/main/ScarHack/ScarHack.lua'))()");
                        }
                        else if(krnlapien.Checked == true)
                        {
                            MainAPI.Execute("local env = getgenv()\nenv.scarhack = {}\nenv.scarhack.key = " + shkey.Text + "\nloadstring(game:GetHttp('https://github.com/scar17off/Roblox-Scripts/raw/main/ScarHack/ScarHack.lua'))())");
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
