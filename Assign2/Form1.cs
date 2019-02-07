using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assign2 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();


        }

        private void guildRoster_Click(object sender, EventArgs e)
        {
            foreach (KeyValuePair<uint, string> entry in guildList)
            {
                Console.WriteLine(entry.Value.ToString());
            }
        }
    }
}
