using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;




namespace _6502
{
    
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        Thread cputh; 

        public Form1()
        {
            InitializeComponent();
            richTextBox1.Text = thecpu.strregs();

           
        }

        _6502cpu thecpu = new _6502cpu();

        private void button1_Click(object sender, EventArgs e)
        {
            thecpu.cycle();
            richTextBox1.Text = thecpu.strregs();
            textBoxMem.Text = thecpu.strmem((ushort)numericUpDown1.Value, (ushort)numericUpDown2.Value, (int)numericUpDown4.Value);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Regex rgx = new Regex("[^a-fA-f0-9-]");
            richTextBox3.Text = rgx.Replace(richTextBox3.Text, "");
            for (int i = 0; i < richTextBox3.Text.Length; i += 3)
            {
                if (richTextBox3.Text.Substring(i, 2).Length == 2)
                {
                    //richTextBox2.Text += "\n" + (i - 2) / 3;
                    thecpu.memory[(i ) / 3] = Convert.ToByte(richTextBox3.Text.Substring(i, 2), 16);
                }
                
                richTextBox3.Text = richTextBox3.Text.Insert(i, " ");
            }
            textBoxMem.Text = thecpu.strmem((ushort)numericUpDown1.Value, (ushort)numericUpDown2.Value, (int)numericUpDown4.Value);

        }


        //Thread cpuThread;
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            cputh = new Thread(runthread);
        }
        bool run = false;
        
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            button2.Enabled = run;
            button1.Enabled = run;
            button4.Enabled = run;

            run = !run;
            int cycle = 0;
            //while (run)
            //{
                
                //if (cycle > 150000) { Console.WriteLine(thecpu.strregs()); }
                
                //richTextBox1.Text = thecpu.strregs();
                //textBoxMem.Text = thecpu.strmem(0x01f0, 0x01ff + 64);
                //if ((cycle % 100) == 0)
                //{
                    //Application.DoEvents();
                //}
                //cycle++;
                //Console.Title = cycle.ToString();
            //}
            if (cputh != null){
                if (cputh.ThreadState == ThreadState.Running) {
                    cputh.Suspend();
                }
                else
                {
                    cputh = new Thread(runthread);
                    cputh.Start();
                }
            }
            
            
            
        }

        public void runthread()
        {
            while (true)
            {
                thecpu.cycle();
            }
        }

        //int cycle = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {

            label1.Text = "PC: " + String.Format("{0:X4}", thecpu.getPC());
            richTextBox1.Text = thecpu.strregs();
            button5.PerformClick();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.ReadAllBytes(openFileDialog1.FileNames[0]).CopyTo(thecpu.memory, (int)numericUpDown3.Value);
                //textBoxMem.Text = thecpu.strmem(0x01f0,0x01ff + 64);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBoxMem.Text = thecpu.strmem((ushort)numericUpDown1.Value, (ushort)numericUpDown2.Value, (int)numericUpDown4.Value, true);
            }
            else
            {
                textBoxMem.Text = thecpu.strmem((ushort)numericUpDown1.Value, (ushort)numericUpDown2.Value, (int)numericUpDown4.Value);
            }
            
            
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value <= numericUpDown1.Value)
            {
                numericUpDown2.Value = numericUpDown1.Value ;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value >= numericUpDown2.Value)
            {
                numericUpDown1.Value = numericUpDown2.Value ;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            thecpu.setPC((ushort)numericUpDown3.Value);
        }


    }
}
