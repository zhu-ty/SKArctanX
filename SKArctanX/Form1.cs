using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKArctanX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SKSpecialDecimal c1 = new SKSpecialDecimal("2.533345");
            SKSpecialDecimal c2 = new SKSpecialDecimal("1.00000");

            //c2.cut(2);
            textBox1.Text += (c1/c2).ToString();
            //textBox1.Text += (c1 * c2).ToString();
            textBox1.Text += "\r\n";
        }
    }
}
