using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OTGEdit.XML;
using OTGEdit.Utils;
using System.Drawing;
using System.Threading;

namespace OTGEdit
{
    public static class Session
    {
        public static int ClosedHeight = 142;
        public static int ClosedWidth = 950;        

        public static int OpenedHeight = 743;
        public static int OpenedWidth = 950;

        public static DirectoryInfo VersionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\");

        // TODO: Pass most of these as method parameters instead of using static fields

        public static SettingsType SettingsType = null;
        public static VersionConfig VersionConfig;

        public static string DestinationConfigsDir = "";
        public static string SourceConfigsDir = "";

        public static List<string> BiomeNames = new List<string>();

        public static WorldConfig WorldConfigDefaultValues;
        public static WorldConfig WorldConfig1;
        public static Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> WorldSettingsInputs = new Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>>();

        public static bool IgnorePropertyInputChangedWorld = false;
        public static bool IgnoreOverrideCheckChangedWorld = false;

        public static List<Group> BiomeGroups = new List<Group>();
        public static Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> BiomeSettingsInputs = new Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>>();

        public static ToolTip ToolTip1 = new ToolTip() { AutoPopDelay = 32000, InitialDelay = 500, ReshowDelay = 0, ShowAlways = true, IsBalloon = true };
        public static ToolTip ToolTip2 = new ToolTip() { AutoPopDelay = 32000, InitialDelay = 0, ReshowDelay = 0, ShowAlways = true }; //AutomaticDelay = 0, 

        public static Form1 Form1;
        public static TabControl tabControl1;
        public static Panel panel2;
        public static Panel panel3;
        public static Button btSave;
        public static Button btLoad;
        public static Button btGenerate;
        public static ListBox lbGroups;
        public static Button btCopyBO3s;
        public static CheckBox cbDeleteRegion;

        public static void Init()
        {
            CreateProgressBar();

            Form1.FormClosing += Form1_FormClosing;
        }

        static void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            progressBarForm.Invoke(
                (MethodInvoker)(
                    () =>
                    {
                        Application.ExitThread();
                    }
                )
            );
        }

        private static Form progressBarForm;
        private static Thread progressBarThread;
        private static void CreateProgressBar()
        {
            // Create a form and keep it running in a seperate thread.
            progressBarThread = new Thread(
                new ThreadStart(
                    (Action)delegate
                    {
                        Form form = new System.Windows.Forms.Form();
                        form.Text = "Please wait.";
                        form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.MinimizeBox = false;
                        form.MaximizeBox = false;
                        form.ControlBox = false;

                        form.ClientSize = new Size(117, 40);

                        ProgressBar progressBar = new ProgressBar();
                        progressBar.Minimum = 0;
                        progressBar.Maximum = 100;
                        progressBar.Text = "Loading...";
                        progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                        progressBar.Location = new Point(8, 8);
                        progressBar.AutoSize = true;
                        progressBar.Style = ProgressBarStyle.Marquee;

                        form.Controls.AddRange(new Control[] { progressBar });

                        form.Show();
                        form.Hide();

                        progressBarForm = form;

                        System.Windows.Forms.Application.Run();
                    }
                )
            );
            progressBarThread.Start();
        }

        public static void ShowProgessBox()
        {
            progressBarForm.Invoke(
                (MethodInvoker)(
                    () =>
                    {
                        // TODO: Potential threading errors when accessing Form1?
                        Control controlToFocusOn = Form1.ActiveForm ?? Session.Form1;
                        progressBarForm.Location = new Point(controlToFocusOn.Location.X + (controlToFocusOn.ClientSize.Width / 2) - (progressBarForm.Width / 2), controlToFocusOn.Location.Y + (controlToFocusOn.ClientSize.Height / 2) - (progressBarForm.Height / 2) + 24);
                        progressBarForm.Show();
                        progressBarForm.Activate();
                    }
                )
            );
        }

        public static void HideProgessBox()
        {
            progressBarForm.Invoke(
                (MethodInvoker)(
                    () =>
                    {
                        progressBarForm.Hide();
                    }
                )
            );            
        }
    }
}
