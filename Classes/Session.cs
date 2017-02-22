using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TCEE.XML;

namespace TCEE
{
    public static class Session
    {
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

        public static ToolTip ToolTip1 = new ToolTip();
        
        public static System.Windows.Forms.Form Form1;
        public static TabControl tabControl1;
        public static Button btSave;
        public static Button btLoad;
        public static Button btGenerate;
        public static RadioButton rbSummerSkin;
        public static RadioButton rbWinterSkin;
        public static ListBox lbGroups;
        public static Button btCopyBO3s;
        public static CheckBox cbDeleteRegion;
    }
}
