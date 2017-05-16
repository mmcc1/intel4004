using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intel4004;
using System.Threading;

namespace Intel_MCS_4_Emulator
{
    public partial class Form1 : Form
    {
        Intel4004AppStore appStore;
        //EmulatorEngine ee;
        Thread engine;
        string selectedApp;

        public Form1()
        {
            InitializeComponent();
            //ee = new EmulatorEngine();
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //engine = new Thread(ee.Execute);
            //engine.Start();
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if(result == DialogResult.OK)
            {
                if(openFileDialog1.FileName.Split('.')[1] == "i4s")
                {
                    appStore = Utils.ReadAppStore(openFileDialog1.FileName);
                }
                else if (openFileDialog1.FileName.Split('.')[1] == "i4a")
                {
                    if (appStore == null)
                    {
                        appStore = new Intel4004AppStore();
                        appStore.Apps = new List<Intel4004App>();
                    }

                    appStore.Apps.Add(Utils.ReadApp(openFileDialog1.FileName));
                }
                else
                {
                    MessageBox.Show("Not a valid emulator file.");
                    return;
                }
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (saveFileDialog1.FileName.Split('.')[1] == "i4s")
                {
                    Utils.SaveAppStore(saveFileDialog1.FileName, appStore);
                }
                else if (openFileDialog1.FileName.Split('.')[1] == "i4a")
                {
                    Utils.SaveApp(saveFileDialog1.FileName, appStore.Apps.Where(x => x.Name == selectedApp).Single());
                }
                else
                {
                    MessageBox.Show("Not a valid emulator file.");
                    return;
                }
            }
        }


        /// <summary>
        /// Reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            //ee.Reset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ee.Terminate();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            string data = appStore.Apps.Where(x => x.Name == comboBox5.SelectedText).Select(x => x.Data).Single();
            /*
                if (endAddr != 0) { alert("Press RESET before loading"); return; }
                var i, j = false, newAddress;
                address = address || 0;
                address = parseInt(address, 16);
                data = data.toUpperCase();
                for (i = 0; i < data.length; ++i)
                {

                    if (data[i] == "*" && data[i + 1] == "=" && data[i + 2] == "$")
                    {
                        address = parseInt(data[i + 3] + data[i + 4] + data[i + 5] + data[i + 6], 16);

                        i += 7;
                        j = false;
                    }

                    if ((data[i] >= "0" && data[i] <= "9") || (data[i] >= "A" && data[i] <= "F"))
                        if (j === false)
                            j = parseInt(data[i], 16);
                        else
                        {
                            prom[address++ % 0x1000] = (j * 0x10 + parseInt(data[i], 16));
                            j = false;
                        }
                }
                endAddr = address;
                generateDisArray();
                show();
                */
        }

        private string FormatForTextBox(string input, bool assembly, bool ascii)
        {
            //TODO: Format
            return input;
        }
    }
}
