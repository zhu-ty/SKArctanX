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
            SKSpecialDecimal c1 = new SKSpecialDecimal("133.24");
            SKSpecialDecimal c2 = new SKSpecialDecimal("1.00243");
            SKSpecialDecimal c3 = new SKSpecialDecimal("-1.00100000");
            SKSpecialDecimal c4 = new SKSpecialDecimal("-100.1");
            SKSpecialDecimal c5 = new SKSpecialDecimal(12.3, 30);
            c1.reset(2.333, 100);

            textBox1.Text += c1.ToString();
            textBox1.Text += "\r\n";
            textBox1.Text += c5.ToString();
            textBox1.Text += "\r\n";
            textBox1.Text += (c1 - c5).ToString();
            textBox1.Text += "\r\n";

            textBox1.Text += (c1 + c2).ToString();
            textBox1.Text += "\r\n";
            textBox1.Text += (c3 + c4).ToString();
            textBox1.Text += "\r\n";
            textBox1.Text += (c3 + c2).ToString();
            textBox1.Text += "\r\n";
        }
    }
}
