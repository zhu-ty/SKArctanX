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
        SKAlgorithm ska = new SKAlgorithm();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked || checkBox2.Checked || checkBox3.Checked || checkBox4.Checked)
            {
                SKSpecialDecimal x = new SKSpecialDecimal(textBox_x.Text);
                if (x.get_digit() == 0)
                {
                    MessageBox.Show("x输入有误，请确认字符串。");
                    return;
                }
                double seconds = 0;
                bool[] cal = new bool[4];
                int[] dig = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    cal[i] = false;
                    dig[i] = 0;
                }
                if (checkBox1.Checked)
                {
                    bool suc = int.TryParse(textBox_t_1.Text, out dig[0]);
                    if (suc == false || dig[0] < 1 || dig[0] > 50)
                    {
                        MessageBox.Show("泰勒方法的有效位数输入错误！");
                        return;
                    }
                    cal[0] = true;
                    seconds += (dig[0] / 10.0D) * 0.33325;
                }
                if (checkBox2.Checked)
                {
                    bool suc = int.TryParse(textBox_r_1.Text, out dig[1]);
                    if (suc == false || dig[1] < 1 || dig[1] > 50)
                    {
                        MessageBox.Show("龙贝格方法的有效位数输入错误！");
                        return;
                    }
                    cal[1] = true;
                    seconds += (dig[1] / 10.0D) * 1.380475;
                }
                if (checkBox3.Checked)
                {
                    bool suc = int.TryParse(textBox_c_1.Text, out dig[2]);
                    if (suc == false || dig[2] < 1 || dig[2] > 20)
                    {
                        MessageBox.Show("复化柯特斯方法的有效位数输入错误！");
                        return;
                    }
                    cal[2] = true;
                    seconds += Math.Pow(10, dig[2] / 6.0D) * 0.05199;
                }
                if (checkBox4.Checked)
                {
                    bool suc = int.TryParse(textBox_s_1.Text, out dig[3]);
                    if (suc == false || dig[3] < 1 || dig[3] > 16)
                    {
                        MessageBox.Show("复化辛普森方法的有效位数输入错误！");
                        return;
                    }
                    cal[3] = true;
                    seconds += Math.Pow(10, dig[3] / 4.0D) * 0.02807;
                }
                MessageBox.Show("点击确定后开始计算！\n"+"计算约耗时："+((int)seconds).ToString()+"秒（经验估计）\n实际耗时以下方进度条或实际时间消耗为准。");
                DateTime dt = DateTime.Now;
                for (int i = 0; i < 4; i++)
                {
                    if (cal[i])
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    textBox_t_2.Text = ska.Talor(x, dig[i], progressBar1).ToString();
                                    progressBar1.Value = 0;
                                    break;
                                }
                            case 1:
                                {
                                    textBox_r_2.Text = ska.Romberg(x, dig[i], progressBar1).ToString();
                                    progressBar1.Value = 0;
                                    break;
                                }
                            case 2:
                                {
                                    textBox_c_2.Text = ska.Cotes(x, dig[i], progressBar1).ToString();
                                    progressBar1.Value = 0;
                                    break;
                                }
                            case 3:
                                {
                                    textBox_s_2.Text = ska.Simpson(x, dig[i], progressBar1).ToString();
                                    progressBar1.Value = 0;
                                    break;
                                }
                        }
                    }
                }
                textBox1.Text = (DateTime.Now - dt).TotalSeconds.ToString();
            }
            else
            {
                MessageBox.Show("请至少选择一种方法！（勾选相应的计算框）");
                return;
            }
        }
    }
}
