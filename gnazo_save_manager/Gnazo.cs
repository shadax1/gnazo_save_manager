using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace gnazo_app
{
    public partial class gnazo_app : Form
    {
        #region misc
        static ProcessMemory pm = new ProcessMemory();
        string dFile, dFile2;
        static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string path = appdata + @"\FkGnazo\pos.cfg";

        public gnazo_app()
        {
            InitializeComponent();
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Save_manager_Load(object sender, EventArgs e)
        {
            string x, y;

            if (!Directory.Exists(appdata + @"\FkGnazo"))
                Directory.CreateDirectory(appdata + @"\FkGnazo");
            comboFile.Text = "0";

            if (File.Exists(path)) //check if pos.cfg exists
            {
                using (StreamReader sr = System.IO.File.OpenText(path))
                {
                    x = sr.ReadLine();
                    y = sr.ReadLine();
                }
                Location = new Point(int.Parse(x), int.Parse(y)); //place application at the same position it was in the last time
            }

            new Thread(Speed) { IsBackground = true }.Start();
            new Thread(HP) { IsBackground = true }.Start();
            new Thread(FrameCount) { IsBackground = true }.Start();
            new Thread(PreviousRoom) { IsBackground = true }.Start();
        }

        private void Save_manager_FormClosed(object sender, FormClosedEventArgs e)
        {
            using (StreamWriter sw = System.IO.File.CreateText(path)) //saving application's position upon closing
            {
                sw.WriteLine(Location.X);
                sw.WriteLine(Location.Y);
            }
        }
        #endregion

        #region threads
        private void Speed()
        {
            byte[] buffer = new byte[2];
            int result = 0;
            while (true)
            {
                buffer = pm.ReadGnazo(0x928596, buffer); //read x speed value
                result = BitConverter.ToInt16(buffer, 0);

                //initial value is between -16xxx and 16xxx
                if (result > 0)
                    result = result - (65536 / 4) - 100;
                else if (result < 0)
                    result = Math.Abs(result + (65536 / 4)) - 100;
                else
                    result = 0;

                labelSpeed.Invoke((MethodInvoker)delegate() { labelSpeed.Text = result.ToString(); });
                Thread.Sleep(1);
            }
        }

        private void HP()
        {
            byte[] buffer = new byte[2];
            int result = 0;

            while (true)
            {
                buffer = pm.ReadGnazo(0xB8D0EC, buffer); //read HP of current character
                result = BitConverter.ToInt16(buffer, 0);
                buffer = pm.ReadGnazo(0x8533D8 + (44 * result) + 0, buffer);
                result = BitConverter.ToInt16(buffer, 0);
                labelHP.Invoke((MethodInvoker)delegate() { labelHP.Text = result.ToString(); });
                Thread.Sleep(50);
            }
        }

        private void FrameCount()
        {
            //int seconds = 0, attack = 0, total = 60, previous = 0, loading = 0;  var timespan = new TimeSpan();
            int result = 0;
            byte[] buffer = new byte[2];

            while (true)
            {
                buffer = pm.ReadGnazo(0xB8D0E4, buffer); //read global timer
                result = BitConverter.ToInt16(buffer, 0);

                labelCurrentFrames.Invoke((MethodInvoker)delegate() { labelCurrentFrames.Text = result.ToString(); });

                //seconds = total / 60; //frames / 60 = seconds
                //timespan = TimeSpan.FromSeconds(seconds);
                //labelRoomTimer.Invoke((MethodInvoker)delegate { labelRoomTimer.Text = timespan.ToString(@"hh\:mm\:ss"); });
                Thread.Sleep(10);
            }
        }

        private void PreviousRoom()
        {
            byte[] buffer = new byte[2];
            int result = 0;

            while (true) //waits for the loading flag and stores the current frames to labelPrevious
            {
                buffer = pm.ReadGnazo(0x4408E4, buffer);
                result = BitConverter.ToInt16(buffer, 0);
                if (result == 1)
                {
                    labelPreviousFrames.Invoke((MethodInvoker)delegate () { labelPreviousFrames.Text = labelCurrentFrames.Text; });
                    Thread.Sleep(1000);
                }
                Thread.Sleep(10);
            }
        }

        bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0; //check if bit at position 'pos' from byte 'b' is set or not
        }
        #endregion

        #region buttons
        private void LoadSave_Click(object sender, EventArgs e)
        {
            int foo = int.Parse(comboFile.Text);
            foo++;

            dFile = @"file\data" + foo.ToString() + ".dat";
            dFile2 = @"file\setdata" + foo.ToString() + ".dat";

            if (listBox.Text != "")
            {
                string sFile = @"saves\" + listBox.Text + "A.dat";
                string sFile2 = @"saves\" + listBox.Text + "B.dat";

                if(File.Exists(sFile) && File.Exists(sFile2))
                {
                    File.Delete(dFile);
                    File.Delete(dFile2);

                    File.Copy(sFile, dFile);
                    File.Copy(sFile2, dFile2);
                    TextDone.Text = listBox.Text + " loaded";
                }
                else
                    TextDone.Text = "Save doesn't exist!";
            }
            else
                TextDone.Text = "Select a save!";
        }

        private void Button99_Click(object sender, EventArgs e)
        {
            byte[] buffer = BitConverter.GetBytes(99);
            pm.WriteGnazo(0x84FFDC, buffer);
        }
        #endregion

        #region radio
        private void radio1_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("Daiyousei");
            listBox.Items.Add("24");
            listBox.Items.Add("Remilia");
            listBox.Items.Add("113");
            listBox.Items.Add("Meiling");
            listBox.Items.Add("30");
        }

        private void radio2_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("Cirno");
            listBox.Items.Add("25");
            listBox.Items.Add("Rumia");
            listBox.Items.Add("9");
            listBox.Items.Add("Alice");
            listBox.Items.Add("16");
            listBox.Items.Add("13");
            listBox.Items.Add("The Jump");
        }

        private void radio3_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("207");
            listBox.Items.Add("205 1");
            listBox.Items.Add("205 2");
            listBox.Items.Add("206");
            listBox.Items.Add("208 1");
            listBox.Items.Add("208 2");
            listBox.Items.Add("208 3");
            listBox.Items.Add("209");
            listBox.Items.Add("210 1");
            listBox.Items.Add("Mokou");
            listBox.Items.Add("210 2");
        }

        private void radio4_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("Kaguya");
            listBox.Items.Add("163");
            listBox.Items.Add("161");
            listBox.Items.Add("160");
            listBox.Items.Add("Eirin");
            listBox.Items.Add("158");
        }

        private void radio5_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("Game over");
            listBox.Items.Add("Reisen");
            listBox.Items.Add("139");
            listBox.Items.Add("137");
            listBox.Items.Add("136");
            listBox.Items.Add("Tewi");
            listBox.Items.Add("135");
            listBox.Items.Add("133");
            listBox.Items.Add("144");
            listBox.Items.Add("146");
            listBox.Items.Add("145");
            listBox.Items.Add("147");
            listBox.Items.Add("148 1");
            listBox.Items.Add("Flandre");
            listBox.Items.Add("148 2");
        }

        private void radio6_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("128");
            listBox.Items.Add("Yuyuko");
            listBox.Items.Add("129");
            listBox.Items.Add("118");
            listBox.Items.Add("Youmu");
            listBox.Items.Add("92");
            listBox.Items.Add("68");
            listBox.Items.Add("Sakuya");
            listBox.Items.Add("67");
        }

        private void radio7_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("38");
            listBox.Items.Add("39");
            listBox.Items.Add("Patchouli");
            listBox.Items.Add("40");
            listBox.Items.Add("85");
            listBox.Items.Add("Nitori");
            listBox.Items.Add("79");
            listBox.Items.Add("51");
            listBox.Items.Add("Keine");
        }

        private void radio8_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("165");
            listBox.Items.Add("166");
            listBox.Items.Add("167");
            listBox.Items.Add("169");
            listBox.Items.Add("171");
            listBox.Items.Add("173");
            listBox.Items.Add("174");
            listBox.Items.Add("176");
            listBox.Items.Add("Aya");
            listBox.Items.Add("197");
            listBox.Items.Add("Kanako 1");
            listBox.Items.Add("Kanako 2");
            listBox.Items.Add("200");
            listBox.Items.Add("Sanae");
            listBox.Items.Add("189");
        }

        private void radio9_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("126");
            listBox.Items.Add("93");
            listBox.Items.Add("Suwako");
            listBox.Items.Add("220");
            listBox.Items.Add("94");
            listBox.Items.Add("95");
            listBox.Items.Add("96");
            listBox.Items.Add("97");
            listBox.Items.Add("98");
            listBox.Items.Add("99");
            listBox.Items.Add("Yukari");
        }

        private void radioUser_CheckedChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("Save 1");
            listBox.Items.Add("Save 2");
            listBox.Items.Add("Save 3");
            listBox.Items.Add("Save 4");
            listBox.Items.Add("Save 5");
            listBox.Items.Add("Save 6");
            listBox.Items.Add("Save 7");
            listBox.Items.Add("Save 8");
            listBox.Items.Add("Save 9");
            listBox.Items.Add("Save 10");
        }
        #endregion
    }
}
