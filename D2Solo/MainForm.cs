using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace D2Solo
{
    public partial class MainForm : Form
    {
        Process process;
        List<string> displayNameList = new List<string>();
        public MainForm()
        {
            InitializeComponent();
            InitPowershell();
        }

        private void InitPowershell()
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            process.Start();
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine("Output: " + e.Data);
                }
            };
            process.BeginOutputReadLine();
            ExecuteCmd("powershell");
        }

        private void ExecuteCmd(string cmd)
        {
            process.StandardInput.WriteLine(cmd);
        }

        private void AddFirewallRule(string direction, string protocol)
        {
            var displayName = $"D2Solo-{direction}-{protocol}-Block";
            ExecuteCmd($"New-NetFirewallRule -DisplayName \"{displayName}\" -Direction {direction} -RemotePort 27000-27100 -Protocol {protocol} -Action Block");
            displayNameList.Add(displayName);
        }

        private void RemoveFirewallRule(string displayName)
        {
            ExecuteCmd($"Remove-NetFirewallRule -DisplayName \"{displayName}\"");
        }

        private void SetStatus(bool isActivated)
        {
            if (isActivated)
            {
                OnOffBtn.Text = "OFF";
                StatusLb.Text = "ACTIVATED";
                StatusLb.ForeColor = Color.Lime;
                return;
            }
            OnOffBtn.Text = "ON";
            StatusLb.Text = "DEACTIVATED";
            StatusLb.ForeColor = Color.White;
        }

        private void OnOffBtn_Click(object sender, EventArgs e)
        {
            if (OnOffBtn.Text == "ON")
            {
                ExecuteCmd("(New-Object -ComObject HNetCfg.FwPolicy2).RestoreLocalFirewallDefaults()");
                AddFirewallRule(DIRECTION_TYPE.OUTBOUND, PROTOCOL_TYPE.TCP);
                AddFirewallRule(DIRECTION_TYPE.OUTBOUND, PROTOCOL_TYPE.UDP);
                AddFirewallRule(DIRECTION_TYPE.INBOUND, PROTOCOL_TYPE.TCP);
                AddFirewallRule(DIRECTION_TYPE.INBOUND, PROTOCOL_TYPE.UDP);
                SetStatus(true);
                return;
            }

            displayNameList.ForEach(x =>
            {
                RemoveFirewallRule(x);
            });
            displayNameList.Clear();
            SetStatus(false);
        }
    }
}
