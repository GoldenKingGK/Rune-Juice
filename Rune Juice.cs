using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Timers;
using System.Web;
using System.IO;
using System.Collections;
using System.Text;

namespace Rune_Juice
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox_Resolution.Text = "1280X720";


            // The ToolTip setting. You can do this as many times as you want
            helperTip.SetToolTip(pictureBox_Top, "Top");
            helperTip.SetToolTip(pictureBox_Jng, "Jungle");
            helperTip.SetToolTip(pictureBox_Mid, "Middle");
            helperTip.SetToolTip(pictureBox_Adc, "Marksman");
            helperTip.SetToolTip(pictureBox_Sup, "Support");

            helperTip.SetToolTip(pictureBox_Mode, "Mute Sound!");

            helperTip.SetToolTip(pictureBox_UpdateAvailable, "Update Available!");

            listView_MostFreqPri.Columns.Add("Primary");
            listView_MostFreqSec.Columns.Add("Secondary");
            listView_MostWinsPri.Columns.Add("Primary");
            listView_MostWinsSec.Columns.Add("Secondary");
            ResizeListViewColumns(listView_MostFreqPri, "Primary");
            ResizeListViewColumns(listView_MostFreqSec, "Secondary");
            ResizeListViewColumns(listView_MostWinsPri, "Primary");
            ResizeListViewColumns(listView_MostWinsSec, "Secondary");



            CheckAppVersion();






            //UpdateRuneTreeColor("Primary", listView_MostFreqPri);
            //UpdateRuneTreeColor("Secondary", listView_MostFreqSec);
            //UpdateRuneTreeColor("Primary", listView_MostWinsPri);
            //UpdateRuneTreeColor("Secondary", listView_MostWinsSec);


            //listView1.Columns[0].ImageKey = 0;









            comboBox_League.Text = "Platinum+";
            comboBox_Ping.Text = "NA";
            createChampionList("FILL");

            uint CurrVol = 0;
            // At this point, CurrVol gets assigned the volume
            waveOutGetVolume(IntPtr.Zero, out CurrVol);
            // Calculate the volume
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            // Get the volume on a scale of 1 to 10 (to fit the trackbar)
            int trackWave = CalcVol / (ushort.MaxValue / 10);
            //trackWave.Value 



            // Calculate the volume that's being set. BTW: this is a trackbar!
            int NewVolume = ((ushort.MaxValue / 10) * trackWave);
            NewVolume = 10000;
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            // Set the volume
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

            System.IO.Stream str = Properties.Resources.Intro;
            snd = new System.Media.SoundPlayer(str);
            snd.Play();

            // System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"c:\mywavfile.wav");
            // player.Play();


        }

        System.Media.SoundPlayer snd = new System.Media.SoundPlayer(Properties.Resources.Intro);

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);














        private void ResizeListViewColumns(ListView lv, string s_ColumnName)
        {
            /*
            ImageList Imagelist = new ImageList();
            int i_Picture = 0;

            Imagelist.ImageSize = new Size(22, 22);
            Imagelist.ColorDepth = ColorDepth.Depth32Bit;
            Imagelist.Images.Add(Rune_Juice.Properties.Resources.RuneDomination_icon);

            lv.LargeImageList = Imagelist;
            lv.SmallImageList = Imagelist;
            */
            //listView1.Columns.Add("a", "b", 120, HorizontalAlignment.Center, 0);


            lv.Items.Clear();
            //lv.Columns.Add(s_ColumnName);
            // ("a", s_ColumnName, 120, HorizontalAlignment.Left, 1);
            //lv.Columns.Add(new ListViewGroup { ImageIndex = 0, Text = s_ColumnName });

            foreach (ColumnHeader column in lv.Columns)
            {
                column.Width = 155;
                column.Text = s_ColumnName;
            }

            /*
            lv.Columns.Add(s_ColumnName);

            for (int i = 1; i < 5; i++)
            {

                Imagelist.Images.Add(GetRuneImage(a_MostFreq[i]));
                listView_MostFreqPri.Items.Add(new ListViewItem { ImageIndex = i_Picture++, Text = a_MostFreq[i] });

            }


            */


        }
        #region Initialize Variables

        string s_URL = "http://champion.gg";

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Size GetControlSize(IntPtr hWnd)
        {
            RECT pRect;
            Size cSize = new Size();
            // get coordinates relative to window
            GetWindowRect(hWnd, out pRect);

            cSize.Width = pRect.Right - pRect.Left;
            cSize.Height = pRect.Bottom - pRect.Top;

            return cSize;
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filler)
                : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
            {
                cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }

        }

        [DllImport("user32")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        #endregion Initialize Variables

        private int[] GetCorrectMousePosition(int x, int y)
        {
            int[] mousePosition = new int[2];
            if (x == 0)
                x = 1;
            if (y == 0)
                y = 1;

            var p = System.Diagnostics.Process.GetProcessesByName("LeagueClientUx");
            if (p.Length == 0) return mousePosition;

            WINDOWINFO wi = new WINDOWINFO(false);
            GetWindowInfo(p[0].MainWindowHandle, ref wi);

            double centerX = (wi.rcWindow.Left + wi.rcWindow.Right) / 2;
            double centerY = (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2;

            double defaultResolutionX = 1600;
            double defaultResolutionY = 900;


            double clientResolutionX = 1600;
            double clientResolutionY = 900;

            if (comboBox_Resolution.Text == "1280X720")
            {
                clientResolutionX = 1280;
                clientResolutionY = 720;
            }
            else if (comboBox_Resolution.Text == "1024X576")
            {
                clientResolutionX = 1024;
                clientResolutionY = 576;
            }



            double keystonesX = (x / defaultResolutionX) * 1000;
            double keystonesY = (y / defaultResolutionY) * 1000;



            double calcX = clientResolutionX * keystonesX / 1000;
            double calcY = clientResolutionY * keystonesY / 1000;

            mousePosition[0] = (int)(centerX + calcX) + 1;
            mousePosition[1] = (int)(centerY + calcY) + 1;


            return mousePosition;

        }
        // FUNCTION: CREATES RUNE PAGE
        string[] a_ChampionInfo;
        private void CreateRunePage(string sMostPlaystyle)
        {


            if (a_ChampionInfo[0] == "")
            {
                return;
            }

            // var processToRun = new ProcessStartInfo(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
            //  TestStack.White.Application app = TestStack.White.Application.Launch(processToRun);

            IntPtr hWnd = FindWindow("RCLIENT", null); // this gives you the handle of the window you need.

            // then use this handle to bring the window to focus or forground(I guessed you wanted this).

            // sometimes the window may be minimized and the setforground function cannot bring it to focus so:

            /*use this ShowWindow(IntPtr handle, int nCmdShow);
            *there are various values of nCmdShow 3, 5 ,9. What 9 does is: 
            *Activates and displays the window. If the window is minimized or maximized, *the system restores it to its original size and position. An application *should specify this flag when restoring a minimized window */
            ShowWindow(hWnd, 9);
            //The bring the application to focus
            SetForegroundWindow(hWnd);

            // you wanted to bring the application to focus every 2 or few second
            // call other window as done above and recall this window again.

            Thread.Sleep(100);



            var p = System.Diagnostics.Process.GetProcessesByName("LeagueClientUx");

            if (p.Length == 0) return;

            WINDOWINFO wi = new WINDOWINFO(false);
            GetWindowInfo(p[0].MainWindowHandle, ref wi);




            if (b_InChampSelect)
            {
                MouseInputs(a_ChampionInfo, "ChampSelect");
            }
            else
            {
                MouseInputs(a_ChampionInfo, "");
            }

        }



        //END FUNCTION: CREATES RUNE PAGE






        private void SelectPrimaryRune (string s_Rune)
        {
            int[] aMouseMovement = { 0, 0 };
            if (b_InChampSelect & b_NewRunePage)
            {
                switch (s_Rune)
                {
                    case "Precision":
                        aMouseMovement = GetCorrectMousePosition(-500, 0);
                        break;
                    case "Domination":
                        aMouseMovement = GetCorrectMousePosition(-250, 0);
                        break;
                    case "Sorcery":
                        aMouseMovement = GetCorrectMousePosition(0, 0);
                        break;
                    case "Resolve":
                        aMouseMovement = GetCorrectMousePosition(250, 0);
                        break;
                    case "Inspiration":
                        aMouseMovement = GetCorrectMousePosition(500, 0);
                        break;
                }
            }
            else if (!b_InChampSelect & b_NewRunePage)
            {
                switch (s_Rune)
                {
                    case "Precision":
                        aMouseMovement = GetCorrectMousePosition(-600, 0);
                        break;
                    case "Domination":
                        aMouseMovement = GetCorrectMousePosition(-300, 0);
                        break;
                    case "Sorcery":
                        aMouseMovement = GetCorrectMousePosition(-100, 0);
                        break;
                    case "Resolve":
                        aMouseMovement = GetCorrectMousePosition(100, 0);
                        break;
                    case "Inspiration":
                        aMouseMovement = GetCorrectMousePosition(300, 0);
                        break;
                }
            }
            else if (b_InChampSelect & !b_NewRunePage)
            {
                // Click Toggle View
                aMouseMovement = GetCorrectMousePosition(-575, 400);
                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                switch (s_Rune)
                {
                    case "Precision":
                        aMouseMovement = GetCorrectMousePosition(-500, -200);
                        break;
                    case "Domination":
                        aMouseMovement = GetCorrectMousePosition(-450, -200);
                        break;
                    case "Sorcery":
                        aMouseMovement = GetCorrectMousePosition(-400, -200);
                        break;
                    case "Resolve":
                        aMouseMovement = GetCorrectMousePosition(-350, -200);
                        break;
                    case "Inspiration":
                        aMouseMovement = GetCorrectMousePosition(-300, -200);
                        break;
                }
            }
            else if (!b_InChampSelect & !b_NewRunePage)
            {
                // Click Toggle View
                aMouseMovement = GetCorrectMousePosition(-700, 400);
                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                switch (s_Rune)
                {
                    case "Precision":
                        aMouseMovement = GetCorrectMousePosition(-650, -200);
                        break;
                    case "Domination":
                        aMouseMovement = GetCorrectMousePosition(-600, -200);
                        break;
                    case "Sorcery":
                        aMouseMovement = GetCorrectMousePosition(-550, -200);
                        break;
                    case "Resolve":
                        aMouseMovement = GetCorrectMousePosition(-500, -200);
                        break;
                    case "Inspiration":
                        aMouseMovement = GetCorrectMousePosition(-450, -200);
                        break;
                }
            }
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
        }


    




        private void MouseInputs(string[] aMostWins, string s_Mode)
        {
            int[] aMouseMovement;

            if (s_Mode == "ChampSelect")
            {
                #region MouseInputs for Champion Select

                for (int i = 0; i < aMostWins.Length; i++)
                {
                    string sRune = aMostWins[i];
                    if (i == 0)
                    {
                        SelectPrimaryRune(sRune);
                        Thread.Sleep(1000);
                    }
                    else if (i < 5)
                    {
                        switch (sRune)
                        {
                            case "Press the Attack":
                            case "Electrocute":
                            case "Summon Aery":
                            case "Grasp of the Undying":
                            case "Unsealed Spellbook":
                                aMouseMovement = GetCorrectMousePosition(-500, -60);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Lethal Tempo":
                            case "Predator":
                            case "Arcane Comet":
                            case "Aftershock":
                            case "Glacual Augment":
                                aMouseMovement = GetCorrectMousePosition(-400, -60);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                //SendLeftClick((wi.rcWindow.Left + wi.rcWindow.Right) / 2 - 400, (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2 - 60);
                                break;

                            case "Fleet Footwork":
                            case "Dark Harvest":
                            case "Phase Rush":
                            case "Guardian":
                            case "Kleptomancy":
                                aMouseMovement = GetCorrectMousePosition(-300, -60);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                //SendLeftClick((wi.rcWindow.Left + wi.rcWindow.Right) / 2 - 200, (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2 - 60);
                                break;



                            case "Cheap Shot":
                            case "Overheal":
                            case "Nullifying Orb":
                            case "Unflinching":
                            case "Hextech Flashtraption":
                                aMouseMovement = GetCorrectMousePosition(-500, +60);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                //SendLeftClick((wi.rcWindow.Left + wi.rcWindow.Right) / 2 - 500, (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2 + 60);
                                break;



                            case "Taste of Blood":
                            case "Triumph":
                            case "Manaflow Band":
                            case "Demolish":
                            case "Biscuit Delivery":
                                aMouseMovement = GetCorrectMousePosition(-400, 60);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;




                            case "Sudden Impact":
                            case "Presence of Mind":
                            case "The Ultimate Hat":
                            case "Font of Life":
                            case "Perfect Timing":
                                aMouseMovement = GetCorrectMousePosition(-300, 60);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;



                            case "Zombie Ward":
                            case "Transcendence":
                            case "Iron Skin":
                            case "Magical Footwear":
                            case "Legend: Alacrity":
                                aMouseMovement = GetCorrectMousePosition(-500, 200);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ghost Poro":
                            case "Celerity":
                            case "Mirror Shell":
                            case "Future's Market":
                            case "Legend: Tenacity":
                                aMouseMovement = GetCorrectMousePosition(-400, 200);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Eyeball Collection":
                            case "Absolute Focus":
                            case "Conditioning":
                            case "Minion Dematerializer":
                            case "Legend: Bloodline":
                                aMouseMovement = GetCorrectMousePosition(-300, 200);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;


                            case "Ravenous Hunter":
                            case "Scorch":
                            case "Overgrowth":
                            case "Cosmic Insight":
                            case "Coup de Grace":
                                aMouseMovement = GetCorrectMousePosition(-500, 300);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ingenious Hunter":
                            case "Waterwalking":
                            case "Revitalize":
                            case "Approach Velocity":
                            case "Cut Down":
                                aMouseMovement = GetCorrectMousePosition(-400, 300);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Relentless Hunter":
                            case "Gathering Storm":
                            case "Second Wind":
                            case "Celestial Body":
                            case "Last Stand":
                                aMouseMovement = GetCorrectMousePosition(-300, 300);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;
                        }
                    }
                    else if (i == 5)
                    {
                        if (aMostWins[0] == "Precision")
                        {
                            switch (sRune)
                            {
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(0, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Domination")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(0, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Sorcery")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(0, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Resolve")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(0, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Inspiration")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(0, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                    }
                    else if (i > 5)
                    {
                        switch (sRune)
                        {
                            case "Cheap Shot":
                            case "Nullifying Orb":
                            case "Unflinching":
                            case "Hextech Flashtraption":
                            case "Overheal":
                                aMouseMovement = GetCorrectMousePosition(-75, -100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Taste of Blood":
                            case "Triumph":
                            case "Manaflow Band":
                            case "Demolish":
                            case "Biscuit Delivery":
                                aMouseMovement = GetCorrectMousePosition(0, -100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Sudden Impact":
                            case "Presence of Mind":
                            case "The Ultimate Hat":
                            case "Font of Life":
                            case "Perfect Timing":
                                aMouseMovement = GetCorrectMousePosition(100, -100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Zombie Ward":
                            case "Transcendence":
                            case "Iron Skin":
                            case "Magical Footwear":
                            case "Legend: Alacrity":
                                aMouseMovement = GetCorrectMousePosition(-75, 0);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ghost Poro":
                            case "Celerity":
                            case "Mirror Shell":
                            case "Future's Market":
                            case "Legend: Tenacity":
                                aMouseMovement = GetCorrectMousePosition(0, 0);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Eyeball Collection":
                            case "Absolute Focus":
                            case "Conditioning":
                            case "Minion Dematerializer":
                            case "Legend: Bloodline":
                                aMouseMovement = GetCorrectMousePosition(100, 0);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;


                            case "Ravenous Hunter":
                            case "Coup de Grace":
                            case "Scorch":
                            case "Overgrowth":
                            case "Cosmic Insight":
                                aMouseMovement = GetCorrectMousePosition(-75, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ingenious Hunter":
                            case "Waterwalking":
                            case "Revitalize":
                            case "Approach Velocity":
                            case "Cut Down":
                                aMouseMovement = GetCorrectMousePosition(0, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;


                            case "Relentless Hunter":
                            case "Gathering Storm":
                            case "Second Wind":
                            case "Celestial Body":
                            case "Last Stand":
                                aMouseMovement = GetCorrectMousePosition(100, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;
                        }
                    }
                }
                #endregion MouseInputs for Champion Select

                aMouseMovement = GetCorrectMousePosition(-615, -300);
                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                Thread.Sleep(500);
                KeyboardInput();



                Thread.Sleep(500);
                //SAVE
                aMouseMovement = GetCorrectMousePosition(-200, -300);
                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);


            }
            else
            {
                #region MouseInputs for Collections Screen
                for (int i = 0; i < aMostWins.Length; i++)
                {


                    string sRune = aMostWins[i];

                    if (i == 0)
                    {
                        SelectPrimaryRune(sRune);
                        Thread.Sleep(500);
                    }
                    else if (i < 5)
                    {
                        switch (sRune)
                        {
                            case "Press the Attack":
                            case "Electrocute":
                            case "Summon Aery":
                            case "Grasp of the Undying":
                            case "Unsealed Spellbook":
                                aMouseMovement = GetCorrectMousePosition(-600, -20);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Lethal Tempo":
                            case "Predator":
                            case "Arcane Comet":
                            case "Aftershock":
                            case "Glacual Augment":
                                aMouseMovement = GetCorrectMousePosition(-525, -20);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                //SendLeftClick((wi.rcWindow.Left + wi.rcWindow.Right) / 2 - 400, (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2 - 60);
                                break;

                            case "Fleet Footwork":
                            case "Dark Harvest":
                            case "Phase Rush":
                            case "Guardian":
                            case "Kleptomancy":
                                aMouseMovement = GetCorrectMousePosition(-450, -20);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                //SendLeftClick((wi.rcWindow.Left + wi.rcWindow.Right) / 2 - 200, (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2 - 60);
                                break;



                            case "Cheap Shot":
                            case "Overheal":
                            case "Nullifying Orb":
                            case "Unflinching":
                            case "Hextech Flashtraption":
                                aMouseMovement = GetCorrectMousePosition(-600, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                //SendLeftClick((wi.rcWindow.Left + wi.rcWindow.Right) / 2 - 500, (wi.rcWindow.Top + wi.rcWindow.Bottom) / 2 + 60);
                                break;



                            case "Taste of Blood":
                            case "Triumph":
                            case "Manaflow Band":
                            case "Demolish":
                            case "Biscuit Delivery":
                                aMouseMovement = GetCorrectMousePosition(-525, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;




                            case "Sudden Impact":
                            case "Presence of Mind":
                            case "The Ultimate Hat":
                            case "Font of Life":
                            case "Perfect Timing":
                                aMouseMovement = GetCorrectMousePosition(-450, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;



                            case "Zombie Ward":
                            case "Transcendence":
                            case "Iron Skin":
                            case "Magical Footwear":
                            case "Legend: Alacrity":
                                aMouseMovement = GetCorrectMousePosition(-600, 200);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ghost Poro":
                            case "Celerity":
                            case "Mirror Shell":
                            case "Future's Market":
                            case "Legend: Tenacity":
                                aMouseMovement = GetCorrectMousePosition(-525, 200);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Eyeball Collection":
                            case "Absolute Focus":
                            case "Conditioning":
                            case "Minion Dematerializer":
                            case "Legend: Bloodline":
                                aMouseMovement = GetCorrectMousePosition(-450, 200);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;


                            case "Ravenous Hunter":
                            case "Scorch":
                            case "Overgrowth":
                            case "Cosmic Insight":
                            case "Coup de Grace":
                                aMouseMovement = GetCorrectMousePosition(-600, 300);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ingenious Hunter":
                            case "Waterwalking":
                            case "Revitalize":
                            case "Approach Velocity":
                            case "Cut Down":
                                aMouseMovement = GetCorrectMousePosition(-525, 300);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Relentless Hunter":
                            case "Gathering Storm":
                            case "Second Wind":
                            case "Celestial Body":
                            case "Last Stand":
                                aMouseMovement = GetCorrectMousePosition(-450, 300);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;
                        }
                    }
                    else if (i == 5)
                    {
                        if (aMostWins[0] == "Precision")
                        {
                            switch (sRune)
                            {
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(-200, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(-150, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(-50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Domination")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-200, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(-150, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(-50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Sorcery")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-200, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(-150, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(-50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Resolve")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-200, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(-150, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Inspiration":
                                    aMouseMovement = GetCorrectMousePosition(-50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                        else if (aMostWins[0] == "Inspiration")
                        {
                            switch (sRune)
                            {
                                case "Precision":
                                    aMouseMovement = GetCorrectMousePosition(-200, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Domination":
                                    aMouseMovement = GetCorrectMousePosition(-150, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Sorcery":
                                    aMouseMovement = GetCorrectMousePosition(-100, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                                case "Resolve":
                                    aMouseMovement = GetCorrectMousePosition(-50, -200);
                                    SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                    break;
                            }
                        }
                    }
                    else if (i > 5)
                    {
                        switch (sRune)
                        {
                            case "Cheap Shot":
                            case "Nullifying Orb":
                            case "Unflinching":
                            case "Hextech Flashtraption":
                            case "Overheal":
                                aMouseMovement = GetCorrectMousePosition(-200, -100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Taste of Blood":
                            case "Triumph":
                            case "Manaflow Band":
                            case "Demolish":
                            case "Biscuit Delivery":
                                aMouseMovement = GetCorrectMousePosition(-140, -100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Sudden Impact":
                            case "Presence of Mind":
                            case "The Ultimate Hat":
                            case "Font of Life":
                            case "Perfect Timing":
                                aMouseMovement = GetCorrectMousePosition(-50, -100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Zombie Ward":
                            case "Transcendence":
                            case "Iron Skin":
                            case "Magical Footwear":
                            case "Legend: Alacrity":
                                aMouseMovement = GetCorrectMousePosition(-200, 0);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ghost Poro":
                            case "Celerity":
                            case "Mirror Shell":
                            case "Future's Market":
                            case "Legend: Tenacity":
                                aMouseMovement = GetCorrectMousePosition(-140, 0);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Eyeball Collection":
                            case "Absolute Focus":
                            case "Conditioning":
                            case "Minion Dematerializer":
                            case "Legend: Bloodline":
                                aMouseMovement = GetCorrectMousePosition(-50, 0);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;


                            case "Ravenous Hunter":
                            case "Coup de Grace":
                            case "Scorch":
                            case "Overgrowth":
                            case "Cosmic Insight":
                                aMouseMovement = GetCorrectMousePosition(-200, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;

                            case "Ingenious Hunter":
                            case "Waterwalking":
                            case "Revitalize":
                            case "Approach Velocity":
                            case "Cut Down":
                                aMouseMovement = GetCorrectMousePosition(-140, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;


                            case "Relentless Hunter":
                            case "Gathering Storm":
                            case "Second Wind":
                            case "Celestial Body":
                            case "Last Stand":
                                aMouseMovement = GetCorrectMousePosition(-50, 100);
                                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                                break;
                        }
                    }
                }
                #endregion MouseInputs for Collections Screen

                // Type in Champion
                aMouseMovement = GetCorrectMousePosition(-725, -300);
                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
                Thread.Sleep(500);
                KeyboardInput();




                Thread.Sleep(500);
                //SAVE
                aMouseMovement = GetCorrectMousePosition(-275, -300);
                SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            }

        }



        string s_Top = "TOP";
        string s_Jng = "JNG";
        string s_Mid = "MID";
        string s_Adc = "ADC";
        string s_Sup = "SUP";
        string s_Fill = "FILL";

        List<Champion> championList = new List<Champion>();

        private string getSelectedLeague()
        {
            string s_League = "";
            try
            {
                s_League = comboBox_League.SelectedItem.ToString();
                switch (s_League)
                {
                    case "Platinum+":
                        s_League = "platplus";
                        break;
                    case "Platinum":
                        s_League = "plat";
                        break;
                    case "Gold":
                        s_League = "gold";
                        break;
                    case "Silver":
                        s_League = "silver";
                        break;
                    case "Bronze":
                        s_League = "bronze";
                        break;
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return s_League;
        }

        private string getSelectedPing()
        {
            string s_Ping = "";
            try
            {
                s_Ping = comboBox_Ping.SelectedItem.ToString();
                switch (s_Ping)
                {
                    case "NA":
                        s_Ping = "104.160.131.3";
                        break;
                    case "EUNE":
                        s_Ping = "104.160.142.3";
                        break;
                    case "EUW":
                        s_Ping = "104.160.141.3";
                        break;
                    case "LAN":
                        s_Ping = "104.160.136.3";
                        pictureBox_Mode.Visible = true;
                        System.IO.Stream str = Properties.Resources.iPappy;
                        snd = new System.Media.SoundPlayer(str);
                        snd.Play();
                        break;
                    case "BR":
                        s_Ping = "104.160.152.3";
                        break;
                    case "OCE":
                        s_Ping = "104.160.156.1";
                        break;
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return s_Ping;
        }

        private void createChampionList(string sRole)
        {

            try
            {
                championList = new List<Champion>();
                string str = new WebClient().DownloadString("http://champion.gg/statistics" + "/?league=" + getSelectedLeague());

                string sFilterTop = "\"Top\",\"title\":\"";
                string sFilterJng = "\"Jungle\",\"title\":\"";
                string sFilterMid = "\"Middle\",\"title\":\"";
                string sFilterAdc = "\"ADC\",\"title\":\"";
                string sFilterSup = "\"Support\",\"title\":\"";
                string sFilterLast = "\",\"general\":";

                string s_FilterChampions = "\"title\":\"";
                string s_FilterChampionsEnd = "\",\"";
                string s_FilterBans = "\"banRate\":";
                string s_FilterBansEnd = ",\"";



                IEnumerable<string> eChampionsTop = GetSubStrings(str, sFilterTop, sFilterLast);
                IEnumerable<string> eChampionsJng = GetSubStrings(str, sFilterJng, sFilterLast);
                IEnumerable<string> eChampionsMid = GetSubStrings(str, sFilterMid, sFilterLast);
                IEnumerable<string> eChampionsAdc = GetSubStrings(str, sFilterAdc, sFilterLast);
                IEnumerable<string> eChampionsSup = GetSubStrings(str, sFilterSup, sFilterLast);

                IEnumerable<string> e_Champions = GetSubStrings(str, s_FilterChampions, s_FilterChampionsEnd);
                IEnumerable<string> e_Bans = GetSubStrings(str, s_FilterBans, s_FilterBansEnd);

                string roles = "";

                string[] a_Champions = e_Champions.ToArray();
                string[] a_Bans = e_Bans.ToArray();
             




                listView_Bans.Clear();
                listView_Bans.Items.Clear();
                listView_Bans.GridLines = true;
                listView_Bans.View = View.Details;
                listView_Bans.Columns.Add("Champion", 150);
                listView_Bans.Columns.Add("Ban Rate", 150);
                for (int i = 0; i < a_Bans.Length; i++)
                {
                    if (!listView_Bans.Items.ContainsKey(a_Champions[i]))
                    { 
                        ListViewItem lv = new ListViewItem();
                        lv.Text = a_Champions[i];
                        lv.Name = a_Champions[i];
                        double d_Bans = Convert.ToDouble(a_Bans[i]) * 100;
                        string s_Bans = string.Format("{0:F2}", d_Bans) + " %";
                        lv.SubItems.Add(s_Bans);


                        //counterList.Add(new List<string>() { a_Champion[i], a_WinRate[i] } );
                        listView_Bans.Items.Add(lv);
                    }
                }
                listView_Bans.ListViewItemSorter = new ListViewItemComparer(1, SortOrder.Descending);
                listView_Bans.Visible = true;

                listView_Bans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView_Bans.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);


                










                championList.Add(new Champion("", roles));
                if (sRole == s_Top || sRole == s_Fill)
                {
                    foreach (string sChampion in eChampionsTop)
                    {
                        championList.Add(new Champion(sChampion, s_Top));
                    }
                }
                if (sRole == s_Sup || sRole == s_Fill)
                {
                    foreach (string sChampion in eChampionsSup)
                    {
                        championList.Add(new Champion(sChampion, s_Sup));
                    }
                }
                if (sRole == s_Adc || sRole == s_Fill)
                {
                    foreach (string sChampion in eChampionsAdc)
                    {
                        championList.Add(new Champion(sChampion, s_Adc));
                    }
                }
                if (sRole == s_Mid || sRole == s_Fill)
                {
                    foreach (string sChampion in eChampionsMid)
                    {
                        championList.Add(new Champion(sChampion, s_Mid));
                    }
                }
                if (sRole == s_Jng || sRole == s_Fill)
                {
                    foreach (string sChampion in eChampionsJng)
                    {
                        championList.Add(new Champion(sChampion, s_Jng));
                    }
                }






                //List<Champion>=
                //championList.DistinctBy(p => p.Name);
                //IEnumerable<Champion> distinctList = championList.DistinctBy(x => x.Name);
                // var noDupsList = new HashSet<Champion>(championList).ToList();


                List<Champion> distinctChampionList = championList.GroupBy(Champion => Champion.Name).Select(g => g.First()).ToList();


                comboBox_Champion.DataSource = distinctChampionList;

                comboBox_Champion.ValueMember = "Name";
                comboBox_Champion.DisplayMember = "Name";



            }
            catch (Exception error)
            {
                Console.WriteLine(error);

                MessageBox.Show("Unable to get Champion List could not be found.");

            }
        }


        private void getChampionRoles(string sSelectedChampion)//string[] a_Roles)
        {
            // List<Champion> championRoles = championList.GroupBy(Champion => Champion.Roles).Select(g => g.First()).ToList();
            //List<Champion> championRoles = championList.Where(Champion => Champion.Name == sSelectedChampion).ToList();

            /*
            List<Champion> query = from p in championList
                        where p.Name == "Akali"
                        select p;

           */
            //   foreach (var item in championList)
            //     Console.WriteLine(item.Roles.ToString());
            //comboBox_Roles.Items.Clear();

            /*
            for(int dsaf = 0; dsaf < a_Roles.Length; dsaf++ )
            {
                comboBox_Roles.Items.Insert(dsaf, a_Roles[dsaf]);
                Console.WriteLine(a_Roles[dsaf]);
            }
            */
            //comboBox_Roles.Text = a_Roles[0];        // foreach (var item in championRoles)
            // Console.WriteLine(item.Roles.ToString());

            List<Champion> championRoles = championList.Where(Champion => Champion.Name == sSelectedChampion).ToList();
            comboBox_Roles.DataSource = championRoles;
            comboBox_Roles.ValueMember = "Roles";
            comboBox_Roles.DisplayMember = "Roles";

        }

        public string GetSelectedSummoner()
        {
            string s_SelectedSummoner = "";
            try
            {
                s_SelectedSummoner = textBox_Summoner.Text;//((comboBox_Summoner as ComboBox).SelectedItem as Champion).Name as string;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return s_SelectedSummoner;
        }

        public string SelectedChampion
        {
            get
            {
                string sSelectedChampion = "";
                try
                {
                    Champion champ_SelectedChampion = championList.Find(x => string.Equals(x.Name, comboBox_Champion.Text));
                    sSelectedChampion = champ_SelectedChampion.Name;
                    
                    if (((comboBox_Champion as ComboBox).SelectedItem as Champion).Name as string != null)
                    {


                        sSelectedChampion = ((comboBox_Champion as ComboBox).SelectedItem as Champion).Name as string;
                        //Champion champ_SelectedChampion = championList.Find(x => string.Equals(x.Name, comboBox_Champion.Text));

                        Console.Write("1");
                        Console.WriteLine(sSelectedChampion);
                    }
                    else if (championList.Exists(x => string.Equals(x.Name, comboBox_Champion.Text, StringComparison.OrdinalIgnoreCase)) && comboBox_Champion.Text != "")
                    {
                        Console.Write("2");
                        sSelectedChampion = champ_SelectedChampion.Name;

                        Console.Write("2");
                        Console.WriteLine(sSelectedChampion);
                    }

                    /*
                    if (championList.Exists(x => string.Equals(x.Name, comboBox_Champion.Text, StringComparison.OrdinalIgnoreCase)))
                    {
                        Champion champ_SelectedChampion = championList.Find(x => string.Equals(x.Name, comboBox_Champion.Text));
                        sSelectedChampion = champ_SelectedChampion.Name;
                        Console.WriteLine(sSelectedChampion);
                    }
                    else
                    {
                        sSelectedChampion = ((comboBox_Champion as ComboBox).SelectedItem as Champion).Name as string;
                        Console.WriteLine("2");
                    }
                    */
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
                return sSelectedChampion;
            }
        }

        private string SelectedRole
        {
            get
            {
                string sSelectedRole = "";
                try
                {
                    sSelectedRole = ((comboBox_Roles as ComboBox).SelectedItem as Champion).Roles as string;
                    switch (sSelectedRole)
                    {
                        case "TOP":
                            sSelectedRole = "TOP";
                            break;
                        case "JNG":
                            sSelectedRole = "JNG";
                            break;
                        case "MID":
                            sSelectedRole = "MID";
                            break;
                        case "ADC":
                            sSelectedRole = "ADC";
                            break;
                        case "SUP":
                            sSelectedRole = "SUP";
                            break;
                        default:
                            sSelectedRole = "";
                            break;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return sSelectedRole;
            }
        }

        private string SelectedRoleFullName
        {
            get
            {
                string sSelectedRole = "";
                try
                {
                    sSelectedRole = ((comboBox_Roles as ComboBox).SelectedItem as Champion).Roles as string;
                    switch (sSelectedRole)
                    {
                        case "TOP":
                            sSelectedRole = "Top";
                            break;
                        case "JNG":
                            sSelectedRole = "Jungle";
                            break;
                        case "MID":
                            sSelectedRole = "Middle";
                            break;
                        case "ADC":
                            sSelectedRole = "ADC";
                            break;
                        case "SUP":
                            sSelectedRole = "Support";
                            break;
                        default:
                            sSelectedRole = "";
                            break;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return sSelectedRole;
            }
        }




        private void GetChampionImages(PictureBox pictureBox, string s_PictureURL, string s_PictureType)
        {
            try
            {
                if (s_PictureType == "Champion")
                {
                    helperTip.SetToolTip(pictureBox, "http://leagueoflegends.wikia.com/wiki/" + SelectedChampion);
                    var request = WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + s_PictureURL);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream()) pictureBox.Image = Bitmap.FromStream(stream);
                }
                else if (s_PictureType == "Item/Spell")
                {
                    var request = WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + s_PictureURL);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream()) pictureBox.Image = Bitmap.FromStream(stream);
                }
                else if (s_PictureType == "SummonerSpell")
                {
                    string s_SummonerSpell = GetSubstring(s_PictureURL, 'r', '.');
                    switch (s_SummonerSpell)
                    {
                        case "Dot":
                            s_SummonerSpell = "Ignite - 210s\n" +
                                "Deals 70 - 410 true damage over 5 seconds, applying Grievous Wounds icon Grievous Wounds and revealing them for the duration.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerDot;
                            break;
                        case "Heal":
                            s_SummonerSpell = "Heal - 240s\n" +
                                "Heals the caster and the target nearest to the cursor for 90 - 345 health, increasing movement speed by 30% for 1s.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerHeal;
                            break;
                        case "Flash":
                            s_SummonerSpell = "Flash - 300s\n" +
                                "Teleports your champion a short distance toward your cursor's location.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerFlash;
                            break;
                        case "Teleport":
                            s_SummonerSpell = "Teleport - 300s\n" +
                                "After channeling for 4.5 seconds, your champion flashes to target turret, minion or ward.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerTeleport;
                            break;
                        case "Smite":
                            s_SummonerSpell = "Smite - 90s\n" +
                                "Deals 390 - 1000 true damage to a monster or enemy minion.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerSmite;
                            break;
                        case "Exhaust":
                            s_SummonerSpell = "Exhaust - 210s\n" +
                                "Exhausts target enemy champion, slowing them by 30%, and reducing their damage dealt by 40% for 2.5s.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerExhaust;
                            break;
                        case "Boost":
                            s_SummonerSpell = "Cleanse - 210s\n" +
                                "Removes all disables and summoner spells debuffs affecting your champion, as well as granting 65% Tenacity for 3s.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerBoost;
                            break;
                        case "Haste":
                            s_SummonerSpell = "Ghost - 180s\n" +
                                "Gain increased movement speed. Grants a maximum of 28% - 45% movement speed after accelerating for 2s.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerHaste;
                            break;
                        case "Barrier":
                            s_SummonerSpell = "Barrier - 180s\n" +
                                "Gain increased movement speed. Grants a maximum of 28% - 45% movement speed after accelerating for 2s.";
                            pictureBox.Image = Rune_Juice.Properties.Resources.SummonerBarrier;
                            break;
                        case "Clarity":
                            s_SummonerSpell = "Clarity - 180s\n" +
                                "Restores 50 % maximum mana to you and 25 % maximum mana to nearby allies.";
                            break;
                    }
                    helperTip.SetToolTip(pictureBox, s_SummonerSpell);
                }
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Visible = true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }


        private void GetMasteryInfo()
        {
            string s_SelectedSummoner = GetSelectedSummoner();

            string s_FilterChampion = "<a href=\"/champion?champion=";
            string sFilterEndChampion = "/a>";
            string sData = "";


            string s_FilterChest = "<img src=\"/img/chest.png\" class=\"";
            string s_FilterChestEnd = "\">";




            string s_FilterChestAndMastery = "<td data-value=\"";
            string s_FilterChestAndMasteryEnd = "\" data-tooltip=\"";


            string s_FilterLevel = "<td>";
            string s_FilterLevelEnd = "</td>";




            // Download Champion Info
            try
            {
                s_URL = "https://championmasterylookup.derpthemeus.com/summoner?summoner=" + s_SelectedSummoner + "&region=NA";
                //Console.WriteLine(s_URL);
                sData = new WebClient().DownloadString(s_URL);

                IEnumerable<string> e_Champions = GetSubStrings(sData, s_FilterChampion, sFilterEndChampion);
                IEnumerable<string> e_Chests = GetSubStrings(sData, s_FilterChest, s_FilterChestEnd);
                IEnumerable<string> e_ChestsMastery = GetSubStrings(sData, s_FilterChestAndMastery, s_FilterChestAndMasteryEnd);
                IEnumerable<string> e_Level = GetSubStrings(sData, s_FilterLevel, s_FilterLevelEnd);


                string[] a_Champions = e_Champions.ToArray();
                string[] a_Chests = e_Chests.ToArray();
                string[] a_ChestsMastery = e_ChestsMastery.ToArray();
                string[] a_Level = e_Level.ToArray();



                listView_Mastery.Clear();



                listView_Mastery.Columns.Add("");

                listView_Mastery.Columns.Add("Champion");
                listView_Mastery.Columns.Add("Level");
                listView_Mastery.Columns.Add("Progress");
                ImageList Imagelist = new ImageList();
                listView_Mastery.SmallImageList = Imagelist;
                Imagelist.ImageSize = new Size(20, 20);
                Imagelist.ColorDepth = ColorDepth.Depth32Bit;

                // Start index for pictures
                int i_Picture = 0;
                Imagelist.Images.Add(Rune_Juice.Properties.Resources.chest);
                Imagelist.Images.Add(Rune_Juice.Properties.Resources.noChest);

                listView_Mastery.Columns[0].ImageIndex = i_Picture;







                char[] charsToTrim = { '>' };
                for (int i = 0; i < a_Chests.Length; i++)
                {
                    String s = a_Champions[i];
                    String searchString = ">";
                    int startIndex = s.IndexOf(searchString) + 1;
                    searchString = "<";
                    int endIndex = s.IndexOf(searchString) - 1;
                    String substring = s.Substring(startIndex, endIndex + searchString.Length - startIndex);
                    //Console.WriteLine(substring);
                    //




                    String myString = substring;
                    String myEncodedString;
                    // Encode the string.
                    myEncodedString = HttpUtility.HtmlEncode(myString);
                    StringWriter myWriter = new StringWriter();
                    // Decode the encoded string.
                    HttpUtility.HtmlDecode(substring, myWriter);





                    /*
                    Console.Write(i + " ");
                    Console.Write(myWriter.ToString());
                    Console.Write(" has " + a_Chests[i]);
                    Console.WriteLine(" Progress: " + a_ChestsMastery[i]);
                    */

                    /*

                    ListViewItem first = new ListViewItem { ImageIndex = i_Picture, Text = a_ChestsMastery[i] };
                    ListViewItem sec = new ListViewItem { Text = myWriter.ToString() };
                    ListViewItem[] kite = new ListViewItem[] { first, sec };

                    
                    listView_Mastery.Items.AddRange(kite);

                    */

                    //listView_Mastery.Items.Add(new ListViewItem { ImageIndex = i_Picture });

                    // listView_Mastery.
                    if (a_Chests[i] == "chest")
                    {
                        i_Picture = 0;

                    }
                    else
                    {
                        i_Picture = 1;
                    }





                    ListViewItem item = new ListViewItem();
                    item.ImageIndex = i_Picture;// (new ListViewItem { ImageIndex = i_Picture });
                    item.SubItems.Add(myWriter.ToString());
                    item.SubItems.Add(a_Level[i]);

                    switch (a_ChestsMastery[i])
                    {
                        case "700":
                            item.SubItems.Add("MASTERED");
                            break;
                        case "603":
                            item.SubItems.Add("3/3 Tokens");
                            break;
                        case "602":
                            item.SubItems.Add("2/3 Tokens");
                            break;
                        case "601":
                            item.SubItems.Add("1/3 Tokens");
                            break;
                        case "600":
                            item.SubItems.Add("0/3 Tokens");
                            //item.SubItems.Add(new ListViewItem.ListViewSubItem { ForeColor = Color.Green, Text = "afds" });
                            break;
                        case "502":
                            item.SubItems.Add("2/2 Tokens");
                            //item.SubItems.Add(new ListViewItem.ListViewSubItem { ForeColor = Color.Green, Text = "afds" });
                            break;
                        case "501":
                            item.SubItems.Add("1/2 Tokens");
                            //item.SubItems.Add(new ListViewItem.ListViewSubItem { ForeColor = Color.Green, Text = "afds" });
                            break;
                        case "500":
                            item.SubItems.Add("0/2 Tokens");
                            //item.SubItems.Add(new ListViewItem.ListViewSubItem { ForeColor = Color.Green, Text = "afds" });
                            break;
                        //item.SubItems.Add(new ListViewItem { ImageIndex = i_Picture });
                        //item.SubItems.Add("MASTERED");
                        //listView_Mastery.Items.Add(new ListViewItem { Text = "lol", ImageIndex = i_Picture,  });
                        default:
                            //item.SubItems.Add(new ListViewItem.ListViewSubItem { Tag = Color.Green, Text = "afds" });
                            item.SubItems.Add(" " + a_ChestsMastery[i] + " % until level " + (Convert.ToInt32(a_Level[i]) + 1));
                            break;

                    }



                    listView_Mastery.Items.Add(item);









                }

                listView_Mastery.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView_Mastery.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                listView_Mastery.Columns[0].Width = 35;
                listView_Mastery.Columns[3].Width = 142;
                if (SelectedChampion != "")
                {
                    try
                    {
                        ListViewItem item = listView_Mastery.FindItemWithText(SelectedChampion);//Define item here;
                        int n = listView_Mastery.Items.IndexOf(item);
                        listView_Mastery.Items.RemoveAt(n);
                        listView_Mastery.Items.Insert(0, item);
                        listView_Mastery.FindItemWithText(SelectedChampion).Selected = true;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }







            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void PictureBox_SearchSummoner_Click(object sender, EventArgs e)
        {
            GetMasteryInfo();

        }
        string[] a_Champion;
        string[] a_WinRate;
        string[] a_Pictures;
        string[] a_MostFreq = new string[10];
        string[] a_MostWins = new string[10];
        string[] a_Abilities;
        string[] a_Patch;


        public string[] GetChampionInfo(string s_MostPlaystyle, bool b_GetRoles)
        {
            a_MostFreq[0] = "";
            a_MostWins[0] = "";

            // Gets the Selected Champion from the input box
            string s_SelectedChampion = SelectedChampion;


            if (b_GetRoles)
            {
                getChampionRoles(s_SelectedChampion);
            }

            string s_SelectedRole = SelectedRoleFullName;

            if (s_SelectedChampion == "")
            {
                return a_MostFreq;
            }

            string s_FilterChampionImages = "ddragon.leagueoflegends.com/cdn/";
            string sFilterEndChampionImages = "\" class=\"";
            string sFilterPrimary = " color=\"#9faafc\">";
            string sFilterSecondary = " color=\"#d44242\">";
            string sFilterEnd = "</d";
            string sData = "";

            string s_FilterWinRate = ",\"winRate\":0";
            string s_FilterEndWinRate = ",\"key\":\"";

            string s_FilterChampion = ",\"key\":\"";
            string s_FilterEndChampion = "\",\"statScore";



            string s_FilterAbilities = "Data " + SelectedChampion + "/";
            string s_FilterEndAbilities = ",";


            string s_FilterPatch = "var currentPatch = \"";
            string s_FilterEndpatch = "\"";




            var counterList = new List<List<string>>();

            // Download Champion Info
            try
            {
                if (s_SelectedChampion == "Dr. Mundo")
                {
                    s_SelectedChampion = "DrMundo";
                }
                s_URL = "http://champion.gg/champion/" + s_SelectedChampion + "/" + SelectedRoleFullName + "/?league=" + getSelectedLeague();
                Console.WriteLine(s_URL);
                //sData = new WebClient().DownloadString(s_URL);

                var client = new WebClient();

                Uri StringToUri = new Uri(s_URL);
                client.DownloadStringAsync(StringToUri);

                client.DownloadStringCompleted += (sender, e) =>
                {
                    sData = e.Result;
                    //do something with results 
                    IEnumerable<string> ePrimaryRunes = GetSubStrings(sData, sFilterPrimary, sFilterEnd);
                    IEnumerable<string> eSecondaryRunes = GetSubStrings(sData, sFilterSecondary, sFilterEnd);
                    IEnumerable<string> e_ChampionImage = GetSubStrings(sData, s_FilterChampionImages, sFilterEndChampionImages);
                    IEnumerable<string> e_WinRate = GetSubStrings(sData, s_FilterWinRate, s_FilterEndWinRate);
                    IEnumerable<string> e_Champion = GetSubStrings(sData, s_FilterChampion, s_FilterEndChampion);



                    IEnumerable<string> e_Patch = GetSubStrings(sData, s_FilterPatch, s_FilterEndpatch);




                    a_Patch = e_Patch.ToArray();

                    s_CurrentPatch = a_Patch[0];
                    //int i_TrimAmount = 2;
                    //for (int i = 0; i < )


                    //s_CurrentPatch = s_CurrentPatch.TrimEnd(s_CurrentPatch[s_CurrentPatch.Length-1]);

                    s_CurrentPatch = s_CurrentPatch.Substring(0, 4);



                    string data = DownloadAbilities();

                    IEnumerable<string> e_Abilities = GetSubStrings(data, s_FilterAbilities, s_FilterEndAbilities);








                    a_Abilities = e_Abilities.ToArray();


                    a_Champion = e_Champion.ToArray();
                    a_WinRate = e_WinRate.ToArray();
                    a_Pictures = e_ChampionImage.ToArray();

                    a_Pictures = e_ChampionImage.ToArray();






                    int iFreq = 0;
                    int iWins = 0;
                    foreach (string pri in ePrimaryRunes)
                    {
                        if (iFreq < 5)
                        {
                            a_MostFreq[iFreq++] = pri;
                        }
                        else if (iWins < 5)
                        {
                            a_MostWins[iWins++] = pri;
                        }
                    }
                    int iCount = 0;
                    foreach (string sec in eSecondaryRunes)
                    {
                        if (iCount < 3)
                        {
                            a_MostFreq[iFreq++] = sec;
                            iCount++;
                        }
                        else if (iCount < 6)
                        {
                            a_MostWins[iWins++] = sec;
                            iCount++;
                        }
                    }
                    SetChampion(a_Pictures);
                    SetRunes(a_MostWins, a_MostFreq);
                    // Abilities
                    if (tabControl1.SelectedIndex == 0)
                    {
                        SetCounters(a_Champion, a_WinRate);
                    }
                    // Counters
                    else if (tabControl1.SelectedIndex == 1)
                    {
                        SetAbilities(a_Pictures, a_Abilities);
                    }
                    // Item Builds
                    else if (tabControl1.SelectedIndex == 2)
                    {
                        SetItems(a_Pictures);
                    }
                };


            }
            catch (Exception error)
            {
                Console.WriteLine(error);

                MessageBox.Show("Unable to find page for " + s_SelectedChampion + ".");

            }

            if (s_MostPlaystyle == "Freq")
            {
                return a_MostFreq;
            }
            else
            {
                return a_MostWins;
            }

        }

        private void SetRunes(string[] a_MostWins, string[] a_MostFreq)
        {
            UpdateRuneTreeColor(a_MostFreq[0], listView_MostFreqPri);
            UpdateRuneTreeColor(a_MostFreq[5], listView_MostFreqSec);
            UpdateRuneTreeColor(a_MostWins[0], listView_MostWinsPri);
            UpdateRuneTreeColor(a_MostWins[5], listView_MostWinsSec);


            ResizeListViewColumns(listView_MostFreqPri, a_MostFreq[0]);
            ResizeListViewColumns(listView_MostFreqSec, a_MostFreq[5]);
            ResizeListViewColumns(listView_MostWinsPri, a_MostWins[0]);
            ResizeListViewColumns(listView_MostWinsSec, a_MostWins[5]);


            ImageList Imagelist = new ImageList();
            listView_MostFreqPri.SmallImageList = Imagelist;
            listView_MostFreqSec.SmallImageList = Imagelist;
            listView_MostWinsPri.SmallImageList = Imagelist;
            listView_MostWinsSec.SmallImageList = Imagelist;
            Imagelist.ImageSize = new Size(20, 20);
            Imagelist.ColorDepth = ColorDepth.Depth32Bit;

            // Start index for pictures
            int i_Picture = 0;
            Imagelist.Images.Add(GetRuneImage(a_MostFreq[i_Picture++]));
            listView_MostFreqPri.Columns[0].ImageIndex = 0;
            for (int i = 1; i < 5; i++)
            {
                Imagelist.Images.Add(GetRuneImage(a_MostFreq[i]));
                listView_MostFreqPri.Items.Add(new ListViewItem { ImageIndex = i_Picture++, Text = a_MostFreq[i] });
            }
            Imagelist.Images.Add(GetRuneImage(a_MostFreq[5]));
            listView_MostFreqSec.Columns[0].ImageIndex = i_Picture++;

            for (int i = 6; i < 8; i++)
            {
                Imagelist.Images.Add(GetRuneImage(a_MostFreq[i]));
                listView_MostFreqSec.Items.Add(new ListViewItem { ImageIndex = i_Picture++, Text = a_MostFreq[i] });
            }

            Imagelist.Images.Add(GetRuneImage(a_MostWins[0]));
            listView_MostWinsPri.Columns[0].ImageIndex = i_Picture++;
            for (int i = 1; i < 5; i++)
            {
                Imagelist.Images.Add(GetRuneImage(a_MostWins[i]));
                listView_MostWinsPri.Items.Add(new ListViewItem { ImageIndex = i_Picture++, Text = a_MostWins[i] });
            }

            Imagelist.Images.Add(GetRuneImage(a_MostWins[5]));
            listView_MostWinsSec.Columns[0].ImageIndex = i_Picture++;
            for (int i = 6; i < 8; i++)
            {
                Imagelist.Images.Add(GetRuneImage(a_MostWins[i]));
                listView_MostWinsSec.Items.Add(new ListViewItem { ImageIndex = i_Picture++, Text = a_MostWins[i] });
            }
            button_BuildFreqPage.Visible = true;
            button_BuildWinsPage.Visible = true;
            listView_MostFreqPri.Visible = true;
            listView_MostFreqSec.Visible = true;
            listView_MostWinsPri.Visible = true;
            listView_MostWinsSec.Visible = true;


        }



        private Image GetRuneImage(string s_Rune)
        {
            Image image = Rune_Juice.Properties.Resources.refresh;
            switch (s_Rune)
            {
                case "Precision":
                    image = Rune_Juice.Properties.Resources.Rune_Precision_icon;
                    break;
                case "Domination":
                    image = Rune_Juice.Properties.Resources.RuneDomination_icon;
                    break;
                case "Sorcery":
                    image = Rune_Juice.Properties.Resources.Rune_Sorcery_icon;
                    break;
                case "Inspiration":
                    image = Rune_Juice.Properties.Resources.Rune_Inspiration_icon;
                    break;
                case "Resolve":
                    image = Rune_Juice.Properties.Resources.Rune_Resolve_icon;
                    break;
                case "Press the Attack":
                    image = Rune_Juice.Properties.Resources.Rune_Press_the_Attack_rune;
                    break;
                case "Overheal":
                    image = Rune_Juice.Properties.Resources.Rune_Overheal_rune;
                    break;
                case "Legend: Alacrity":
                    image = Rune_Juice.Properties.Resources.Rune_Legend__Alacrity_rune;
                    break;
                case "Coup de Grace":
                    image = Rune_Juice.Properties.Resources.Rune_Coup_de_Grace_rune;
                    break;
                case "Lethal Tempo":
                    image = Rune_Juice.Properties.Resources.Rune_Lethal_Tempo_rune;
                    break;
                case "Triumph":
                    image = Rune_Juice.Properties.Resources.Rune_Triumph_rune;
                    break;
                case "Legend: Tenacity":
                    image = Rune_Juice.Properties.Resources.Rune_Legend__Tenacity_rune;
                    break;
                case "Cut Down":
                    image = Rune_Juice.Properties.Resources.Rune_Cut_Down_rune;
                    break;
                case "Fleet Footwork":
                    image = Rune_Juice.Properties.Resources.Rune_Fleet_Footwork_rune;
                    break;
                case "Presence of Mind":
                    image = Rune_Juice.Properties.Resources.Rune_Presence_of_Mind_rune;
                    break;
                case "Legend: Bloodline":
                    image = Rune_Juice.Properties.Resources.Rune_Legend__Bloodline_rune;
                    break;
                case "Last Stand":
                    image = Rune_Juice.Properties.Resources.Rune_Last_Stand_rune;
                    break;
                case "Electrocute":
                    image = Rune_Juice.Properties.Resources.Rune_Electrocute_rune;
                    break;
                case "Cheap Shot":
                    image = Rune_Juice.Properties.Resources.Rune_Cheap_Shot_rune;
                    break;
                case "Zombie Ward":
                    image = Rune_Juice.Properties.Resources.Rune_Zombie_Ward_rune;
                    break;
                case "Ravenous Hunter":
                    image = Rune_Juice.Properties.Resources.Rune_Ravenous_Hunter_rune;
                    break;
                case "Predator":
                    image = Rune_Juice.Properties.Resources.Rune_Predator_rune;
                    break;
                case "Taste of Blood":
                    image = Rune_Juice.Properties.Resources.Rune_Taste_of_Blood_rune;
                    break;
                case "Ghost Poro":
                    image = Rune_Juice.Properties.Resources.Rune_Ghost_Poro_rune;
                    break;
                case "Ingenious Hunter":
                    image = Rune_Juice.Properties.Resources.Rune_Ingenious_Hunter_rune;
                    break;
                case "Dark Harvest":
                    image = Rune_Juice.Properties.Resources.Rune_Dark_Harvest_rune;
                    break;
                case "Sudden Impact":
                    image = Rune_Juice.Properties.Resources.Rune_Sudden_Impact_rune;
                    break;
                case "Eyeball Collection":
                    image = Rune_Juice.Properties.Resources.Rune_Eyeball_Collection_rune;
                    break;
                case "Relentless Hunter":
                    image = Rune_Juice.Properties.Resources.Rune_Relentless_Hunter_rune;
                    break;
                case "Summon Aery":
                    image = Rune_Juice.Properties.Resources.Rune_Summon_Aery_rune;
                    break;
                case "Nullifying Orb":
                    image = Rune_Juice.Properties.Resources.Rune_Nullifying_Orb_rune;
                    break;
                case "Transcendence":
                    image = Rune_Juice.Properties.Resources.Rune_Transcendence_rune;
                    break;
                case "Scorch":
                    image = Rune_Juice.Properties.Resources.Rune_Scorch_rune;
                    break;
                case "Arcane Comet":
                    image = Rune_Juice.Properties.Resources.Rune_Arcane_Comet_rune;
                    break;
                case "Manaflow Band":
                    image = Rune_Juice.Properties.Resources.Rune_Manaflow_Band_rune;
                    break;
                case "Celerity":
                    image = Rune_Juice.Properties.Resources.Rune_Celerity_rune;
                    break;
                case "Waterwalking":
                    image = Rune_Juice.Properties.Resources.Rune_Waterwalking_rune;
                    break;
                case "Phase Rush":
                    image = Rune_Juice.Properties.Resources.Rune_Phase_Rush_rune;
                    break;
                case "The Ultimate Hat":
                    image = Rune_Juice.Properties.Resources.Rune_The_Ultimate_Hat_rune;
                    break;
                case "Absolute Focus":
                    image = Rune_Juice.Properties.Resources.Rune_Absolute_Focus_rune;
                    break;
                case "Gathering Storm":
                    image = Rune_Juice.Properties.Resources.Rune_Gathering_Storm_rune;
                    break;
                case "Grasp of the Undying":
                    image = Rune_Juice.Properties.Resources.Rune_Grasp_of_the_Undying_rune;
                    break;
                case "Unflinching":
                    image = Rune_Juice.Properties.Resources.Rune_Unflinching_rune;
                    break;
                case "Iron Skin":
                    image = Rune_Juice.Properties.Resources.Rune_Iron_Skin_rune;
                    break;
                case "Overgrowth":
                    image = Rune_Juice.Properties.Resources.Rune_Overgrowth_rune;
                    break;
                case "Aftershock":
                    image = Rune_Juice.Properties.Resources.Rune_Aftershock_rune;
                    break;
                case "Demolish":
                    image = Rune_Juice.Properties.Resources.Rune_Demolish_rune;
                    break;
                case "Mirror Shell":
                    image = Rune_Juice.Properties.Resources.Rune_Mirror_Shell_rune;
                    break;
                case "Revitalize":
                    image = Rune_Juice.Properties.Resources.Rune_Revitalize_rune;
                    break;
                case "Guardian":
                    image = Rune_Juice.Properties.Resources.Rune_Guardian_rune;
                    break;
                case "Font of Life":
                    image = Rune_Juice.Properties.Resources.Rune_Font_of_Life_rune;
                    break;
                case "Conditioning":
                    image = Rune_Juice.Properties.Resources.Rune_Conditioning_rune;
                    break;
                case "Second Wind":
                    image = Rune_Juice.Properties.Resources.Rune_Second_Wind_rune;
                    break;
                case "Unsealed Spellbook":
                    image = Rune_Juice.Properties.Resources.Rune_Unsealed_Spellbook_rune;
                    break;
                case "Hextech Flashtraption":
                    image = Rune_Juice.Properties.Resources.Rune_Hextech_Flashtraption_rune;
                    break;
                case "Magical Footwear":
                    image = Rune_Juice.Properties.Resources.Rune_Magical_Footwear_rune;
                    break;
                case "Cosmic Insight":
                    image = Rune_Juice.Properties.Resources.Rune_Cosmic_Insight_rune;
                    break;
                case "Glacial Augment":
                    image = Rune_Juice.Properties.Resources.Rune_Glacial_Augment_rune;
                    break;
                case "Biscuit Delivery":
                    image = Rune_Juice.Properties.Resources.Rune_Biscuit_Delivery_rune;
                    break;
                case "Future's Market":
                    image = Rune_Juice.Properties.Resources.Rune_Future_s_Market_rune;
                    break;
                case "Approach Velocity":
                    image = Rune_Juice.Properties.Resources.Rune_Approach_Velocity_rune;
                    break;
                case "Kleptomancy":
                    image = Rune_Juice.Properties.Resources.Rune_Kleptomancy_rune;
                    break;
                case "Perfect Timing":
                    image = Rune_Juice.Properties.Resources.Rune_Perfect_Timing_rune;
                    break;
                case "Minion Dematerializer":
                    image = Rune_Juice.Properties.Resources.Rune_Minion_Dematerializer_rune;
                    break;
                case "Celestial Body":
                    image = Rune_Juice.Properties.Resources.Rune_Celestial_Body_rune;
                    break;
            }
            return image;
        }







        private void SetCounters(string[] a_Champion, string[] a_WinRate)
        {
            listView_WinRate.Clear();
            listView_WinRate.Items.Clear();
            listView_WinRate.GridLines = true;
            listView_WinRate.View = View.Details;
            listView_WinRate.Columns.Add("Opponent", 150);
            listView_WinRate.Columns.Add(SelectedChampion + " Win Rate", 150);
            for (int i = 0; i < a_Champion.Length; i++)
            {
                ListViewItem lv = new ListViewItem();
                lv.Text = a_Champion[i];

                double d_WinRate = Convert.ToDouble(a_WinRate[i]) * 100;
                int i_WinRate = (int)d_WinRate;
                lv.SubItems.Add(i_WinRate.ToString() + "%");


                //counterList.Add(new List<string>() { a_Champion[i], a_WinRate[i] } );
                listView_WinRate.Items.Add(lv);
            }
            listView_WinRate.ListViewItemSorter = new ListViewItemComparer(1);
            listView_WinRate.Visible = true;
        }

        private void SetAbilities(string[] a_Pictures, string[] a_Abilities)
        {
            int i_index = 5;
            GetChampionImages(pictureBox_SkillFreqQ, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_SkillFreqW, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_SkillFreqE, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_SkillFreqR, a_Pictures[i_index++], "Item/Spell");




            helperTip.SetToolTip(pictureBox_SkillFreqQ, a_Abilities[1]);
            helperTip.SetToolTip(pictureBox_SkillFreqW, a_Abilities[2]);
            helperTip.SetToolTip(pictureBox_SkillFreqE, a_Abilities[3]);
            helperTip.SetToolTip(pictureBox_SkillFreqR, a_Abilities[4]);

        }

        private string DownloadAbilities()
        {
            var client = new WebClient();
            string data = "";
            try
            {
                data = client.DownloadString(address: "http://leagueoflegends.wikia.com/wiki/" + SelectedChampion + "/Abilities");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return data;
        }

        private void SetItems(string[] a_Pictures)
        {
            int i_index = 13;
            GetChampionImages(pictureBox_Item1, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item2, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item3, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item4, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item5, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item6, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item7, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item8, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item9, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item10, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item11, a_Pictures[i_index++], "Item/Spell");
            GetChampionImages(pictureBox_Item12, a_Pictures[i_index++], "Item/Spell");

        }
        private void SetChampion(string[] a_Pictures)
        {
            GetChampionImages(pictureBox_Champion, a_Pictures[0], "Champion");
            GetChampionImages(pictureBox_WinsF, a_Pictures[1], "SummonerSpell");
            GetChampionImages(pictureBox_WinsD, a_Pictures[2], "SummonerSpell");
            GetChampionImages(pictureBox_FreqF, a_Pictures[3], "SummonerSpell");
            GetChampionImages(pictureBox_FreqD, a_Pictures[4], "SummonerSpell");
        }

        private int sortColumn = -1;
        private void ColumnClick(object o, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView_WinRate.Sorting = SortOrder.Descending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView_WinRate.Sorting == SortOrder.Descending)
                    listView_WinRate.Sorting = SortOrder.Ascending;
                else
                    listView_WinRate.Sorting = SortOrder.Descending;
            }

            // Call the sort method to manually sort.
            listView_WinRate.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView_WinRate.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView_WinRate.Sorting);
        }


        private void ColumnClick_Mastery(object o, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView_Mastery.Sorting = SortOrder.Descending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView_Mastery.Sorting == SortOrder.Descending)
                    listView_Mastery.Sorting = SortOrder.Ascending;
                else
                    listView_Mastery.Sorting = SortOrder.Descending;
            }

            // Call the sort method to manually sort.
            listView_Mastery.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView_Mastery.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView_Mastery.Sorting);
        }





        private string GetSubstring(string s_String, Char charRange, Char lastChar)
        {
            String s = s_String;
            int startIndex = s.IndexOf(charRange);
            int endIndex = s.LastIndexOf(lastChar);
            int length = endIndex - startIndex + 1;
            return s.Substring(startIndex + 1, length - 2);
        }



        class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                        ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
                //return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }

        public void UpdateRuneTreeColor(string s_Rune, ListView listView)
        {
            //Console.WriteLine(s_Rune);
            switch (s_Rune)
            {
                case "Precision":
                    //listView.BackColor = System.Drawing.Color.Yellow;
                    //colorListViewHeader(listView, System.Drawing.Color.Goldenrod, SystemColors.Info);
                    listView.BackColor = System.Drawing.Color.Goldenrod;
                    break;
                case "Domination":
                    listView.BackColor = System.Drawing.Color.Firebrick;
                    break;
                case "Sorcery":
                    listView.BackColor = System.Drawing.Color.Indigo;
                    //colorListViewHeader(listView, System.Drawing.Color.Indigo, SystemColors.Info);
                    break;
                case "Resolve":
                    listView.BackColor = System.Drawing.Color.DarkGreen;
                    break;
                case "Inspiration":
                    listView.BackColor = System.Drawing.Color.CornflowerBlue;
                    break;
                case "Primary":

                    listView.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case "Secondary":

                    listView.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
            }
        }





        private void button_BuildFreqPage_Click(object sender, EventArgs e)
        {
            a_ChampionInfo = GetChampionInfo("Freq", false);
            CreateRunePagePrompt("Freq");

        }

        private void button_BuildWinsPage_Click(object sender, EventArgs e)
        {
            a_ChampionInfo = GetChampionInfo("Wins", false);
            CreateRunePagePrompt("Wins");
        }

        private void CreateRunePagePrompt(string s_PlayMode)
        {
            // Gets the Selected Champion from the input box
            string s_SelectedChampion = SelectedChampion;
            DialogResult dr_Prompt;

            string s_ChampionSelect = checkBox_Mode.Text;
            string s_SelectedRole = SelectedRole;
            bool b_InChampionSelect = b_InChampSelect;
            if (b_InChampionSelect)
            {
                s_ChampionSelect = checkBox_Mode.Text;
            }
            else
            {
                s_ChampionSelect = checkBox_Mode.Text;
            }

            if (s_SelectedChampion == "")
            {

                MessageBox.Show(
                    "Select a Champion to continue.\n" +
                    "Confirm that the correct Mode, Champion, and Role are selected.\n\n" +
                    "\tRune Page Build : \t" + s_ChampionSelect + "\n" +
                    "\tChampion/Role : \t" + "?????" + "/?????" + "\n" +
                    "\tPlaystyle : \t\tMost " + s_PlayMode +
                    "\n",
                    "Champion Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            s_PageName = SelectedRole + "|" + SelectedChampion + "|" + s_CurrentPatch;

            /*
            s_PageName = Microsoft.VisualBasic.Interaction.InputBox(
                "\nDO NOT MOVE THE CURSOR WHILE THE PAGE IS BEING CREATED!\n\n" +
                s_ChampionSelect + s_ChampionSelect + "\n" +
                "\tChampion:        " + s_SelectedChampion + "\n" +
                "\tRole:            " + s_SelectedRole + "\n" +
                "\tPlaystyle:       Most " + s_PlayMode + "\n" +
                "\tPatch:           " + s_CurrentPatch +
                "\n\nPress \"OK\" to create the Rune Page.",
                       "Create Rune Page?",
                       s_PageName,
                       -1,
                       -1);
                       */

            string input = "Do not move the cursor while the page is being created.\n" +
                "\nChampion: " + s_SelectedChampion + "\n" +
                "Role: " + s_SelectedRole + "\n" +
                "Playstyle: Most " + s_PlayMode + "\n" +
                "Patch: " + s_CurrentPatch +
                "\n\nPress \"New\" to create a Rune Page." +
                "\nPress \"Edit\" to edit a Rune Page.";
            DialogResult dr_Result = ShowInputDialog(ref input, s_PageName);

            /*
            dr_Prompt = MessageBox.Show(
                "Do not move the cursor while the page is being created.\n\n" +
                "\tRune Page Build : \t" + s_ChampionSelect + "\n" +
                "\tChampion/Role : \t" + s_SelectedChampion + s_SelectedRole + "\n" +
                "\tPlaystyle : \t\tMost " + s_PlayMode +
                "\n\nPress \"OK\" to create the Rune Page.",
                "Creating Rune Page...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                */
            Console.WriteLine(dr_Result);
            if (dr_Result == DialogResult.OK)
            {
                b_NewRunePage = true;
                CreateRunePage(s_PlayMode);
            }
            else if (dr_Result == DialogResult.Yes)
            {
                b_NewRunePage = false;
                CreateRunePage(s_PlayMode);
            }
        }

 


        private static DialogResult ShowInputDialog(ref string input, string s_PageName)
        {
            System.Drawing.Size size = new System.Drawing.Size(250, 250);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            inputBox.BackgroundImage = Rune_Juice.Properties.Resources.SR;
            inputBox.ForeColor = SystemColors.Info;
            inputBox.ClientSize = size;
            inputBox.Text = "Creating Rune Page...";

            System.Windows.Forms.Label label = new Label();
            label.BackColor = Color.Transparent;
            label.Size = new System.Drawing.Size(size.Width - 5, 160);
            label.Location = new System.Drawing.Point(5, 5);
            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label.Text = input;
            inputBox.Controls.Add(label);



            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.BackColor = System.Drawing.Color.DarkSlateGray;
            textBox.ForeColor = SystemColors.Info;
            textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 170);
            textBox.Text = s_PageName;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.BackColor = System.Drawing.Color.DarkSlateGray;
            okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&New";
            okButton.Location = new System.Drawing.Point(5, 205);
            inputBox.Controls.Add(okButton);

            Button editButton = new Button();
            editButton.BackColor = System.Drawing.Color.DarkSlateGray;
            editButton.Cursor = System.Windows.Forms.Cursors.Hand;
            editButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            editButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            editButton.Name = "editButton";
            editButton.Size = new System.Drawing.Size(75, 23);
            editButton.Text = "&Edit";
            editButton.Location = new System.Drawing.Point(size.Width/2 - 37, 205);
            inputBox.Controls.Add(editButton);

            Button cancelButton = new Button();
            cancelButton.BackColor = System.Drawing.Color.DarkSlateGray;
            cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width-80, 205);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }



        #region Unused Functions





        private IEnumerable<string> GetSubStrings(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }



        #endregion Unused Functions








        #region Support Functions

        // FUNCTION: SEND LEFT CLICK
        public void SendLeftClick(int x, int y)
        {
            int old_x, old_y;
            old_x = Cursor.Position.X;
            old_y = Cursor.Position.Y;
            Thread.Sleep(500);
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);// UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0); //, UIntPtr.Zero);
                                                         //SetCursorPos(old_x, old_y);
        }
        //END FUNCTION: SEND LEFT CLICK


        #endregion Support Functions







        #region UI Functions  


        private void comboBox_Ping_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                aTimer.Stop();
                button_Ping.BackColor = System.Drawing.Color.DarkSlateGray;
                button_Ping.Invoke(new MethodInvoker(delegate { button_Ping.Text = "Check Ping"; }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }



        private void comboBox_League_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string s_Champion = SelectedChampion;
            string s_Role = SelectedRole;

            createChampionList("FILL");

            comboBox_Champion.Text = s_Champion;
            comboBox_Roles.Text = s_Role;
            GetChampionInfo("", true);

        }

        private void ComboBox_Champion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Console.WriteLine("Select Champion from droplist.");
            GetChampionInfo("", true);
            //comboBox_Champion.ResetText();
        }

        private void ComboBox_Roles_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Console.WriteLine("Select Role from droplist.");
            GetChampionInfo("", false);
        }

        private void ComboBox_Champion_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Press enter key from Champion Box.");
            comboBox_Champion.DroppedDown = false;
            if (e.KeyCode == Keys.Enter)
            {
                GetChampionInfo("", true);
            }
        }
        
        private void Button_Toggle_Click(object sender, EventArgs e)
        {
        }


        #endregion UI Functions

        bool b_Top = true;
        bool b_Jng = true;
        bool b_Mid = true;
        bool b_Adc = true;
        bool b_Sup = true;
        bool b_Fill = true;


        private void ToggleRole(PictureBox pictureBox, bool b_Selected, string sRole)
        {
            if (pictureBox == null)
            {
                throw new ArgumentNullException(nameof(pictureBox));
            }

            comboBox_Roles.DataSource = null;
            comboBox_Roles.Items.Clear();
            comboBox_Champion.DataSource = null;
            comboBox_Champion.Items.Clear();
            if (b_Selected == false)
            {
                b_Selected = true;
                pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                createChampionList(s_Fill);
            }
            else
            {
                b_Selected = false;
                pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                createChampionList(sRole);
            }

        }

        private void ToggleChampionSelect(PictureBox pictureBox, bool b_Selected)
        {
            Color c_BackColor = System.Drawing.Color.DarkSlateGray;
            if (b_Selected == false)
            {
                b_Selected = true;
                c_BackColor = System.Drawing.Color.DarkSlateGray;
                BackgroundImage = Rune_Juice.Properties.Resources.SR;
                button_ChampionSelectMode.Image = Rune_Juice.Properties.Resources.Mode_On;
                comboBox_Resolution.BackColor = c_BackColor;
                comboBox_Champion.BackColor = c_BackColor;
                comboBox_Roles.BackColor = c_BackColor;
                comboBox_Summoner.BackColor = c_BackColor;
                comboBox_League.BackColor = c_BackColor;
                comboBox_Ping.BackColor = c_BackColor;


                tabPage_Abilities.BackColor = c_BackColor;
                tabPage_Counters.BackColor = c_BackColor;

                checkBox_Mode.BackColor = c_BackColor;

                button_BuildFreqPage.BackColor = c_BackColor;
                button_BuildWinsPage.BackColor = c_BackColor;
                button_Snap2Client.BackColor = c_BackColor;
                button_URLChampionGG.BackColor = c_BackColor;
                button_Ping.BackColor = c_BackColor;
            }
            else
            {
                b_Selected = false;
                c_BackColor = System.Drawing.Color.DarkSlateGray;
                BackgroundImage = Rune_Juice.Properties.Resources.Collection;

                button_ChampionSelectMode.Image = Rune_Juice.Properties.Resources.Mode_Off;

                comboBox_Resolution.BackColor = c_BackColor;
                comboBox_Champion.BackColor = c_BackColor;
                comboBox_Roles.BackColor = c_BackColor;
                comboBox_Summoner.BackColor = c_BackColor;
                comboBox_League.BackColor = c_BackColor;
                comboBox_Ping.BackColor = c_BackColor;

                tabPage_Abilities.BackColor = c_BackColor;
                tabPage_Counters.BackColor = c_BackColor;

                checkBox_Mode.BackColor = c_BackColor;

                button_BuildFreqPage.BackColor = c_BackColor;
                button_BuildWinsPage.BackColor = c_BackColor;
                button_Snap2Client.BackColor = c_BackColor;
                button_URLChampionGG.BackColor = c_BackColor;
                button_Ping.BackColor = c_BackColor;
            }

        }

        private void TurnOffRoles()
        {
            if (b_Top == false)
            {
                b_Top = true;
                pictureBox_Top.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            if (b_Jng == false)
            {
                b_Jng = true;
                pictureBox_Jng.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            if (b_Mid == false)
            {
                b_Mid = true;
                pictureBox_Mid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }

            if (b_Adc == false)
            {
                b_Adc = true;
                pictureBox_Adc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            if (b_Sup == false)
            {
                b_Sup = true;
                pictureBox_Sup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
        }


        private void pictureBox_Top_Click(object sender, EventArgs e)
        {
            if (b_Top)
            {
                TurnOffRoles();
                ToggleRole(pictureBox_Top, b_Top, s_Top);
                b_Top = false;
                //turnOff();
                // pictureBox_Top.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }
            else
            {
                ToggleRole(pictureBox_Top, b_Top, s_Top);
                b_Top = true;
                //pictureBox_Top.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            }
        }
        private void pictureBox_Jng_Click(object sender, EventArgs e)
        {
            if (b_Jng)
            {
                TurnOffRoles();
                ToggleRole(pictureBox_Jng, b_Jng, s_Jng);
                b_Jng = false;
            }
            else
            {
                ToggleRole(pictureBox_Jng, b_Jng, s_Jng);
                b_Jng = true;
            }
        }

        private void pictureBox_Mid_Click(object sender, EventArgs e)
        {
            if (b_Mid)
            {
                TurnOffRoles();
                ToggleRole(pictureBox_Mid, b_Mid, s_Mid);
                b_Mid = false;
            }
            else
            {
                ToggleRole(pictureBox_Mid, b_Mid, s_Mid);
                b_Mid = true;
            }
        }

        private void pictureBox_Adc_Click(object sender, EventArgs e)
        {
            if (b_Adc)
            {
                TurnOffRoles();
                ToggleRole(pictureBox_Adc, b_Adc, s_Adc);
                b_Adc = false;
            }
            else
            {
                ToggleRole(pictureBox_Adc, b_Adc, s_Adc);
                b_Adc = true;
            }
        }

        private void pictureBox_Sup_Click(object sender, EventArgs e)
        {
            if (b_Sup)
            {
                TurnOffRoles();
                ToggleRole(pictureBox_Sup, b_Sup, s_Sup);
                b_Sup = false;
            }
            else
            {
                ToggleRole(pictureBox_Sup, b_Sup, s_Sup);
                b_Sup = true;
            }
        }




        private void ShowLeagueClient()
        {
            try
            {

                Process[] processes = Process.GetProcessesByName("LeagueClientUx");
                Process lol = processes[0];
                IntPtr ptr = lol.MainWindowHandle;
                IntPtr hWnd = FindWindow("RCLIENT", null); // this gives you the handle of the window you need.

                // then use this handle to bring the window to focus or forground(I guessed you wanted this).

                // sometimes the window may be minimized and the setforground function cannot bring it to focus so:
                ShowWindow(ptr, 9);
                /*use this ShowWindow(IntPtr handle, int nCmdShow);
                *there are various values of nCmdShow 3, 5 ,9. What 9 does is: 
                *Activates and displays the window. If the window is minimized or maximized, *the system restores it to its original size and position. An application *should specify this flag when restoring a minimized window */
                ShowWindow(hWnd, 9);
                //The bring the application to focus
                SetForegroundWindow(hWnd);

                // you wanted to bring the application to focus every 2 or few second
                // call other window as done above and recall this window again.

                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void Attach2Client()
        {
            try
            {

                Process[] processes = Process.GetProcessesByName("LeagueClientUx");
                Process lol = processes[0];
                IntPtr ptr = lol.MainWindowHandle;
                RECT NotepadRect = new RECT();
                GetWindowRect(ptr, out NotepadRect);

                this.Location = new Point(NotepadRect.Left - 310, NotepadRect.Top);
                IntPtr hWnd = FindWindow("RCLIENT", null); // this gives you the handle of the window you need.

                // then use this handle to bring the window to focus or forground(I guessed you wanted this).

                // sometimes the window may be minimized and the setforground function cannot bring it to focus so:
                ShowWindow(ptr, 9);
                /*use this ShowWindow(IntPtr handle, int nCmdShow);
                *there are various values of nCmdShow 3, 5 ,9. What 9 does is: 
                *Activates and displays the window. If the window is minimized or maximized, *the system restores it to its original size and position. An application *should specify this flag when restoring a minimized window */
                ShowWindow(hWnd, 9);
                //The bring the application to focus
                SetForegroundWindow(hWnd);

                // you wanted to bring the application to focus every 2 or few second
                // call other window as done above and recall this window again.

                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void button_Snap2Client_Click(object sender, EventArgs e)
        {
            ShowLeagueClient();
            Thread.Sleep(500);
            Attach2Client();
        }






        private void linkLabel_URL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(s_URL);
        }




        string s_CurrentPatch;

        private string s_PageName;
        [STAThread]
        private void KeyboardInput()
        {
            //Process[] processes = Process.GetProcessesByName("iexplore");
            Process[] processes = Process.GetProcessesByName("LeagueClientUx");
            Process lol = processes[0];
            SetForegroundWindow(lol.MainWindowHandle);
            Thread.Sleep(100);

            SendKeys.Send("^a");
            Thread.Sleep(100);
            SendKeys.Send(s_PageName);
            Thread.Sleep(100);
            SendKeys.Send("{ENTER}");

        }


        private void pictureBox_Search_Click(object sender, EventArgs e)
        {
            GetChampionInfo("", false);
        }



        //List view header formatters
        public static void colorListViewHeader(ListView list, Color backColor, Color foreColor)
        {
            list.OwnerDraw = true;
            list.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => headerDraw(sender, e, backColor, foreColor)
                );
            list.DrawItem += new DrawListViewItemEventHandler(bodyDraw);
        }
        private static void headerDraw(object sender, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor)
        {
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            e.Graphics.DrawString(e.Header.Text, e.Font, new SolidBrush(foreColor), e.Bounds);
        }
        private static void bodyDraw(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }


        /*
        public static void PlayMp3FromUrl(string url)
        {
            using (Stream ms = new MemoryStream())
            {
                using (Stream stream = WebRequest.Create(url)
                    .GetResponse().GetResponseStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                ms.Position = 0;
                
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
        }
        */
        int i_SongTrack = 0;
        private void pictureBox_Mode_Click(object sender, EventArgs e)
        {
            /*
            uint CurrVol = 0;
            // At this point, CurrVol gets assigned the volume
            waveOutGetVolume(IntPtr.Zero, out CurrVol);
            // Calculate the volume
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            // Get the volume on a scale of 1 to 10 (to fit the trackbar)
            int trackWave = CalcVol / (ushort.MaxValue / 10);
            //trackWave.Value 



            // Calculate the volume that's being set. BTW: this is a trackbar!
            int NewVolume = ((ushort.MaxValue / 10) * trackWave);
            NewVolume = 3000;
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            // Set the volume
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

            */



            pictureBox_Mode.Visible = false;
            snd.Stop();
            //player.Stop();

            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\Nick\Documents\Visual Studio 2017\Projects\Rune_Juice\Rune_Juice\bin\Star Wars Anakin Shooting Stars.wav");



            /*

            switch (i_SongTrack)
            {
                case 0:
                    player = new System.Media.SoundPlayer(@"C:\Users\Nick\Documents\Visual Studio 2017\Projects\Rune_Juice\Rune_Juice\bin\Star Wars Anakin Shooting Stars.wav");
                    break;
                case 1:
                    player = new System.Media.SoundPlayer(@"C:\Users\Nick\Documents\Visual Studio 2017\Projects\Rune_Juice\Rune_Juice\bin\Starcraft Terran Theme 3.wav");
                    break;
                case 2:
                    player = new System.Media.SoundPlayer(@"C:\Users\Nick\Documents\Visual Studio 2017\Projects\Rune_Juice\Rune_Juice\bin\Legends Never Die-Ingame Version.wav");
                    break;
                case 3:
                    player = new System.Media.SoundPlayer(@"C:\Users\Nick\Documents\Visual Studio 2017\Projects\Rune_Juice\Rune_Juice\bin\Samurai Champloo Opening Credits.wav");
                    break;
                case 4:
                    player = new System.Media.SoundPlayer(@"C:\Users\Nick\Documents\Visual Studio 2017\Projects\Rune_Juice\Rune_Juice\bin\One Punch Man Opening Full Version.wav");
                    break;

            }
            i_SongTrack++;
            if (i_SongTrack == 4)
                i_SongTrack = 0;
            player.Stop();
            //player.Play();
            */





            /*
            if (b_Fill)
            {
                ToggleChampionSelect(pictureBox_Mode, b_Fill);
                b_InChampSelect = false;
                checkBox_Mode.Text = "Collection Mode";
                helperTip.SetToolTip(pictureBox_Mode, "Create Rune Page from the Collection Tab.");
                button_ChampionSelectMode.Text = "Champion Select Mode: Off";
                b_Fill = false;
            }
            else
            {
                ToggleChampionSelect(pictureBox_Mode, b_Fill);
                checkBox_Mode.Text = "In Champ Select";
                b_InChampSelect = true;
                helperTip.SetToolTip(pictureBox_Mode, "Create Rune Page while in Champion Select.");
                button_ChampionSelectMode.Text = "Champion Select Mode: On";
                
                b_Fill = true;
            }
            */
        }

        private void checkBox_Mode_CheckedChanged(object sender, EventArgs e)
        {
            if (b_Fill)
            {
                b_Fill = false;
                b_InChampSelect = false;
            }
            else
            {
                b_Fill = true;
                b_InChampSelect = true;
            }
        }



        #region UI ELEMENT - PING LEAGUE OF LEGENDS SERVERS

        System.Timers.Timer aTimer = new System.Timers.Timer();
        // Set Reset Time interval in ms
        int i_Timer = 16000;







        /*
        public void button_Ping_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
            long i_Ping = 0;

            for (int i = 0; i < 2; i++)
            {
                i_Ping = i_Ping + PingLeague();
            }
            i_Ping = i_Ping / 2;
            button_Ping.Text = i_Ping.ToString() + " ms";
            if (i_Ping <= 150)
            {
                button_Ping.BackColor = System.Drawing.Color.Green;
            }
            else if (i_Ping > 150 & i_Ping < 300)
            {
                button_Ping.BackColor = System.Drawing.Color.Yellow;
            }
            else if (i_Ping >= 300)
            {
                button_Ping.BackColor = System.Drawing.Color.Red;
            }
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = i_Timer;
            aTimer.Enabled = true;
        }


        */







        private void ping_Complete(object sender, PingCompletedEventArgs k)
        {
            PingReply reply = k.Reply;
            if (reply.Status == IPStatus.Success)
            {
                long i_Ping = reply.RoundtripTime;

                button_Ping.Text = i_Ping.ToString() + " ms";
                if (i_Ping <= 150)
                {
                    button_Ping.BackColor = System.Drawing.Color.Green;
                }
                else if (i_Ping > 150 & i_Ping < 300)
                {
                    button_Ping.BackColor = System.Drawing.Color.Orange;
                }
                else if (i_Ping >= 300)
                {
                    button_Ping.BackColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                Console.WriteLine(" (FAILED)");
            }
        }

        private async void button_Ping_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = i_Timer;
            aTimer.Enabled = true;

            string s_RiotServer = getSelectedPing();
            byte[] buffer = Encoding.ASCII.GetBytes(".");
            PingOptions options = new PingOptions(50, true);
            AutoResetEvent reset = new AutoResetEvent(false);
            Ping ping = new Ping();
            ping.PingCompleted += new PingCompletedEventHandler(ping_Complete);

            var reply = await ping.SendPingAsync(s_RiotServer, 5000, buffer, options);

        }

        // Specify what you want to happen when the Elapsed event is raised.
        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                //button_Ping.Text = "Check Ping";
                button_Ping.BackColor = System.Drawing.Color.DarkSlateGray;
                button_Ping.Invoke(new MethodInvoker(delegate { button_Ping.Text = "Check Ping"; }));
                pictureBox_Mode.Invoke(new MethodInvoker(delegate {
                    pictureBox_Mode.Visible = false;
                    snd.Stop();
                }));

                pictureBox_UpdateAvailable.Invoke(new MethodInvoker(delegate {
                    pictureBox_UpdateAvailable.Image = Rune_Juice.Properties.Resources.UpdateAvailable;
                    helperTip.SetToolTip(pictureBox_UpdateAvailable, "Check for Update");
                }));
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        #endregion UI ELEMENT - PING LEAGUE OF LEGENDS SERVERS


        private void listView_MostWinsPri_ItemActivate(Object sender, EventArgs e)
        {

            MessageBox.Show("You are in the ListView.ItemActivate event.");

        }



        Point mLastPos = new Point(-1, -1);
        private void listView_MostWinsPri_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView_MostWinsPri.HitTest(e.X, e.Y);
            if (mLastPos != e.Location)
            {
                if (info.Item != null && info.SubItem != null)
                {
                    GetRuneInfo(info.Item.Text);
                    //helperTip.ToolTipTitle = info.Item.Text;
                    //helperTip.Show(, info.Item.ListView, e.X, e.Y, 20000);
                }
                else
                {
                    GetRuneInfo(info.Item.Text);
                    //helperTip.SetToolTip(listView_MostWinsPri, string.Empty);
                }
            }
            mLastPos = e.Location;
        }

        private string GetRuneInfo(string s_Rune)
        {
            string s_RuneInfo = "";

            switch (s_Rune)
            {
                case "Press the Attack":
                    s_RuneInfo = "PASSIVE: Basic attacks against enemy champions apply stacks for 4 seconds. " +
                        "\nAttacking a new target removes all stacks from the previous target. " +
                                "\nApplying 3 stacks to a target deals 30 - 120 (based on level) bonus Adaptive damage and makes them Vulnerable," +
                                "\ncausing them to take 12% increased damage from all sources (except from true damage) for the next 6 seconds." +
                                "\n\nBasic attacks against a Vulnerable target do not apply stacks," +
                                "\nnor can you apply stacks to another target while one is Vulnerable." +
                                "\n\nADAPTIVE: Deals either physical or magic depending on which would deal the most damage.";
                    break;
                case "Triumph":
                    s_RuneInfo = "PASSIVE: Champion takedowns, after a 0.5 - second delay, restore 15 % of your missing health and grant an additional Gold 25.";
                    break;

            }
            MessageBox.Show(s_RuneInfo, s_Rune);
            return s_RuneInfo;


        }


        private void button_URLChampionGG_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(s_URL);
        }

        private void button_URLChampionGG1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(s_URL);
        }

        private void pictureBox_Champion_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://leagueoflegends.wikia.com/wiki/" + SelectedChampion);
        }

        private void TabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            try
            {
                // Abilities
                if (tabControl1.SelectedIndex == 0)
                {
                    SetCounters(a_Champion, a_WinRate);
                }
                // Counters
                else if (tabControl1.SelectedIndex == 1)
                {
                    SetAbilities(a_Pictures, a_Abilities);
                }
                // Item Builds
                else if (tabControl1.SelectedIndex == 2)
                {
                    SetItems(a_Pictures);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void pictureBox_SkillFreqQ_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://leagueoflegends.wikia.com/wiki/" + SelectedChampion + "/Abilities");
        }

        private void pictureBox_SkillFreqW_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start("http://leagueoflegends.wikia.com/wiki/" + SelectedChampion + "/Abilities");
        }

        private void pictureBox_SkillFreqE_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start("http://leagueoflegends.wikia.com/wiki/" + SelectedChampion + "/Abilities");
        }

        private void pictureBox_SkillFreqR_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://leagueoflegends.wikia.com/wiki/" + SelectedChampion + "/Abilities");

        }




        bool b_InChampSelect = true;
        bool b_NewRunePage = true;
        private void button_button_ChampionSelectMode_Click(object sender, EventArgs e)
        {
            if (b_Fill)
            {
                ToggleChampionSelect(pictureBox_Mode, b_Fill);
                checkBox_Mode.Text = "In Champ Select: OFF";
                b_InChampSelect = false;
                helperTip.SetToolTip(pictureBox_Mode, "Create Rune Page from the Collection Tab.");
                button_ChampionSelectMode.Text = "     Champion Select Mode: OFF";
                b_Fill = false;
            }
            else
            {
                ToggleChampionSelect(pictureBox_Mode, b_Fill);
                b_InChampSelect = true;
                checkBox_Mode.Text = "In Champ Select: ON";
                helperTip.SetToolTip(pictureBox_Mode, "Create Rune Page while in Champion Select.");
                button_ChampionSelectMode.Text = "     Champion Select Mode: ON";

                b_Fill = true;
            }
        }

        private void button_Update_Click(object sender, EventArgs e)
        {




            CheckAppVersion();
        }

        private bool CheckAppVersion()
        {
            bool b_UpToDate = false;
            string sData = "";
            string s_FilterAppVersion = "\"/GoldenKingGK/Rune-Juice/blob/master/Rune%20Juice%20";
            string s_FilterEndAppVersion = ".exe\" class=\"js-navigation-open\"";

            try
            {



                s_URL = "https://github.com/GoldenKingGK/Rune-Juice";
                //Console.WriteLine(s_URL);
                //sData = new WebClient().DownloadString(s_URL);

                var client = new WebClient();

                Uri StringToUri = new Uri(s_URL);
                client.DownloadStringAsync(StringToUri);

                client.DownloadStringCompleted += (sender, e) =>
                {
                    sData = e.Result;
                    //do something with results 
                    IEnumerable<string> e_AppVersion = GetSubStrings(sData, s_FilterAppVersion, s_FilterEndAppVersion);

                    string[] a_AppVersion = e_AppVersion.ToArray();


                    string s_AppVersion = Text;
                    string s_LastestVersion = "Rune Juice " + a_AppVersion[a_AppVersion.Length - 1];


                    string s_DownloadURL = "https://github.com/GoldenKingGK/Rune-Juice/raw/master/Rune%20Juice%20" + a_AppVersion[a_AppVersion.Length - 1] + ".exe";
                    if (s_LastestVersion == s_AppVersion)
                    {
                        helperTip.SetToolTip(pictureBox_UpdateAvailable, "Up to Date!");
                        pictureBox_UpdateAvailable.Image = Rune_Juice.Properties.Resources.UptoDate;
                        for (int i = 0; i < a_AppVersion.Length - 1; i++)
                        {
                            File.Delete(AppDomain.CurrentDomain.BaseDirectory + "Rune Juice " + a_AppVersion[i] + ".exe");
                        }
                        b_UpToDate = false;
                        aTimer.Stop();
                        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        aTimer.Interval = i_Timer;
                        aTimer.Enabled = true;
                    }
                    else
                    {
                        b_UpToDate = true;

                        DialogResult dr_Prompt = MessageBox.Show(
                            s_LastestVersion + " - Update avaiable for download from:\n" +
                             "\t" + s_DownloadURL +
                            "\nDo you wish to download the lastest version of Rune Juice?\n",
                            "Update Avaiable " + s_LastestVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr_Prompt == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(s_DownloadURL);
                            Application.Exit();
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return b_UpToDate;
        }

        private void pictureBox_UpdateAvailable_Click(object sender, EventArgs e)
        {
            CheckAppVersion();



            //var process = Process.Start("LeagueClientUx");
            //process.WaitForExit();

            // File.Delete(processPath); // Not strong enough (thanks to Binary Worrier)
            //WaitForProcessExit("LeagueClientUx");    // Will attempts anything to delete the file.

        }



        static bool WaitForProcessExit(string strProcessName, int iWaitTime)
        {
            Process[] aProcs = Process.GetProcessesByName(strProcessName);
            // Ist der Prozess noch aktiv?
            if (aProcs != null && aProcs.Length > 0)
            {
                // ... Wenn ja, auf Beenden warten.
                return aProcs[0].WaitForExit(iWaitTime);
            }

            return true;
        }





        private void textBox_Summoner_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox_Summoner.Text))
            {
                textBox_Summoner.SelectionStart = 0;
                textBox_Summoner.SelectionLength = textBox_Summoner.Text.Length;
            }
            // textBox_Summoner.SelectionStart = 0;
            //textBox_Summoner.SelectionLength = 20;
            textBox_Summoner.SelectAll();
            // textBox_Summoner.Focus();
        }


        private void textBox_Summoner_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetMasteryInfo();
            }
        }

        private void button_SearchSummoner_Click(object sender, EventArgs e)
        {
            GetMasteryInfo();
        }

        private void button_SearchChampion_Click(object sender, EventArgs e)
        {
            GetChampionInfo("", false);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string input = "hede";
            //ShowInputDialog(ref input, " FF");
            int[] aMouseMovement;



            /*
             * COLLECTION MODE
            //Toggle View
            aMouseMovement = GetCorrectMousePosition(-700, 400);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
            // Precision
            aMouseMovement = GetCorrectMousePosition(-650, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Domination
            aMouseMovement = GetCorrectMousePosition(-600, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Sorcery
            aMouseMovement = GetCorrectMousePosition(-550, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
            
            // Resolve
            aMouseMovement = GetCorrectMousePosition(-500, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            
            // Inspiration
            aMouseMovement = GetCorrectMousePosition(-450, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);





            /*
            // Champ Select Mode

            // Toggle View
            aMouseMovement = GetCorrectMousePosition(-575, 400);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Precision
            aMouseMovement = GetCorrectMousePosition(-500, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Domination
            aMouseMovement = GetCorrectMousePosition(-450, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Sorcery
            aMouseMovement = GetCorrectMousePosition(-400, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Resolve
            aMouseMovement = GetCorrectMousePosition(-350, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);

            // Inspiration
            aMouseMovement = GetCorrectMousePosition(-300, -200);
            SendLeftClick(aMouseMovement[0], aMouseMovement[1]);
            */









        }
    }
}
