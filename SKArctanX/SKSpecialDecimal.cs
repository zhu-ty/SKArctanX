using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKArctanX
{
    //特殊地，零只有一位精度
    /// <summary>
    /// 有符号高精度
    /// </summary>
    class SKSpecialDecimal
    {
        public SKSpecialDecimal() { }
        /// <summary>
        /// 用指定的double数初始化高精度小数，默认精度15位
        /// <para>超过15位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public SKSpecialDecimal(double x, int precision = 15)
        {
            reset(x);
        }
        /// <summary>
        /// 用指定的int数初始化高精度小数，默认精度为9位
        /// <para>超过9位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public SKSpecialDecimal(int x, int precision = 9)
        {
            reset(x);
        }
        /// <summary>
        /// 用指定的string初始化高精度小数
        /// </summary>
        /// <param name="x"></param>
        public SKSpecialDecimal(string x)
        {
            reset(x);
        }
        /// <summary>
        /// 生成一个新的高精度数，复制
        /// </summary>
        /// <param name="origin"></param>
        public SKSpecialDecimal(SKSpecialDecimal origin)
        {
            clear();
            foreach (byte tmp in origin.data)
                data.Add(tmp);
            positive = origin.positive;
            exp_10 = origin.exp_10;
        }
        /// <summary>
        /// 清除data、positive与exp_10
        /// </summary>
        public void clear()
        {
            data.Clear();
            positive = true;
            exp_10 = 0;
        }
        /// <summary>
        /// 用指定的double数初始化高精度小数，默认精度15位
        /// <para>超过15位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public void reset(double x, int precision = 15)
        {
            clear();
            if (x < 0) positive = false;
            else if (x == 0)
            {
                exp_10 = 0;
                data.Add(0);
                return;
            }
            x = Math.Abs(x);
            exp_10 = Convert.ToInt32(Math.Floor(Math.Log10(x)));
            double tmp = (x * Math.Pow(10, -Math.Floor(Math.Log10(x))));
            for (int i = 0; i < precision; i++)
            {
                if (i >= 15)
                    data.Add(0);
                else
                    data.Add(Convert.ToByte(Math.Floor(tmp * Math.Pow(10, i)) % 10));
            }
        }
        /// <summary>
        /// 用指定的int数初始化高精度小数，默认精度为9位
        /// <para>超过9位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public void reset(int x, int precision = 9)
        {
            clear();
            if (x < 0) positive = false;
            else if (x == 0)
            {
                exp_10 = 0;
                data.Add(0);
                return;
            }
            exp_10 = Convert.ToInt32(Math.Floor(Math.Log10(x)));
            double tmp = (x * Math.Pow(10, -Math.Floor(Math.Log10(x))));
            for (int i = 0; i < precision; i++)
            {
                if (i >= 9)
                    data.Add(0);
                else
                    data.Add(Convert.ToByte(Math.Floor(tmp * Math.Pow(10, i)) % 10));
            }
        }
        /// <summary>
        /// 用指定的string初始化高精度小数
        /// </summary>
        /// <param name="x"></param>
        public void reset(string x)
        {
            clear();
            if (x[0] == '-')
            {
                positive = false;
                x = x.Substring(1);
            }
            int find_comma = x.IndexOf('.');
            if (find_comma == -1)
                exp_10 = x.Length - 1;
            else
            {
                exp_10 = find_comma - 1;
                x = x.Replace(".", "");
            }
            for (int i = 0; i < x.Length; i++)
            {
                byte tmp = 0;
                if (!byte.TryParse(x[i].ToString(), out tmp))
                {
                    clear();
                    return;
                }
                data.Add(tmp);
            }
        }
        /// <summary>
        /// 裁剪有效位数
        /// </summary>
        /// <param name="x">剩余的有效位数</param>
        public void cut(int x)
        {
            if (x < 0)
                x = 0;
            if (x > data.Count)
                throw (new Exception("Cut To Long Exception"));
            if (x < data.Count)
                data.RemoveRange(x, data.Count - x);
        }
        /// <summary>
        /// 取反
        /// </summary>
        public void inverse()
        {
            positive = !positive;
        }
        /// <summary>
        /// 去除前缀零
        /// </summary>
        public void fix()
        {
            if (get_digit() < 2)
                return;
            int zero_count = 0;
            foreach (byte c in data)
            {
                if (c == 0)
                    zero_count++;
                else
                    break;
            }
            if (zero_count == get_digit())//这个数是零
            {
                data.RemoveRange(1, data.Count - 1);
                exp_10 = 0;
            }
            else
            {
                data.RemoveRange(0, zero_count);
                exp_10 = exp_10 - zero_count;
            }
        }
        /// <summary>
        /// 获得有效位数
        /// </summary>
        /// <returns></returns>
        public int get_digit()
        {
            return data.Count;
        }
        /// <summary>
        /// 获得科学计数法中10的幂次
        /// </summary>
        /// <returns></returns>
        public int get_exp()
        {
            return exp_10;
        }
        /// <summary>
        /// 返回是否是正数
        /// </summary>
        /// <returns></returns>
        public bool get_positive()
        {
            return positive;
        }
        /// <summary>
        /// 返回位数上的值，高位在前，若超过索引将返回0
        /// <para>请注意，单独设定某一位的值可能会无意提高精度</para>
        /// </summary>
        /// <param name="index">该位的索引</param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= get_digit())
                    return 0;
                return data[index];
            }
            set
            {
                if (value > 9)
                    throw new Exception("某一位的值不应该超过9");
                if (index < 0)
                    throw new Exception("不允许设置索引小于零的某一位的值");
                else if (index >= get_digit())
                {
                    int tmp = get_digit();
                    for (int i = 0; i < index - tmp; i++)
                        data.Add(0);
                    data.Add(value);
                }
                else
                    data[index] = value;
            }
        }
        /// <summary>
        /// 返回与另外某数的比较（不考虑精度）
        /// <para>若大于该数，返回1，相等则返回0，否则返回-1</para>
        /// <para>若某数不存在位数（有效位数为0），返回0</para>
        /// <para>当且仅当有效位数、各位数及符号位全部一致时，才判定相等</para>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int compare_to(SKSpecialDecimal x)
        {
            if (get_digit() == 0 || x.get_digit() == 0)
                return 0;
            if (positive && (!x.positive))
                return 1;
            else if ((!positive) && x.positive)
                return -1;
            if (get_exp() > x.get_exp())
                return (positive) ? 1 : -1;
            else if (get_exp() < x.get_exp())
                return (positive) ? -1 : 1;
            for (int i = 0; i < Math.Min(get_digit(), x.get_digit()); i++)
            {
                if(this[i] > x[i])
                    return (positive) ? 1 : -1;
                else if(this[i] < x[i])
                    return (positive) ? -1 : 1;
            }
            if(get_digit() > x.get_digit())//虽然前面全都相等了，但哥有效位数较多，故绝对值较大
                return (positive) ? 1 : -1;
            else if(get_digit() < x.get_digit())
                return (positive) ? -1 : 1;
            return 0;
        }

        public override string ToString()
        {
            string ans = string.Empty;
            if (data.Count == 0)
                return ans;
            if (!positive)
                ans += "-";
            ans += data[0].ToString();
            if (data.Count > 1)
            {
                ans += ".";
                for (int i = 1; i < data.Count; i++)
                    ans += data[i].ToString();
            }
            if (exp_10 != 0)
            {
                ans += " * 10^";
                if (exp_10 < 0)
                    ans += "(" + exp_10.ToString() + ")";
                else
                    ans += exp_10.ToString();
            }
            return ans;
        }

        /// <summary>
        /// 返回绝对值
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static SKSpecialDecimal abs(SKSpecialDecimal a)
        {
            SKSpecialDecimal a_abs = new SKSpecialDecimal(a);
            a_abs.positive = true;
            return a_abs;
        }

        /// <summary>
        /// 加法，有效位数取决于绝对误差最大的数
        /// <para>例：1.001+100.1=101.1</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator +(SKSpecialDecimal a, SKSpecialDecimal b)
        {
            SKSpecialDecimal a_copy = new SKSpecialDecimal(a);
            SKSpecialDecimal b_copy = new SKSpecialDecimal(b);
            //越小越好，表示末位所在的位置
            int a_min_bit = a_copy.exp_10 - a_copy.get_digit() + 1;
            int b_min_bit = b_copy.exp_10 - b_copy.get_digit() + 1;
            if (a_min_bit > b_min_bit)//以a的精度为准
                b_copy.cut(b_copy.get_digit() - a_min_bit + b_min_bit);
            else//以b的精度为准
                a_copy.cut(a_copy.get_digit() - b_min_bit + a_min_bit);
            return Add(a_copy,b_copy);
        }
        public static SKSpecialDecimal operator +(SKSpecialDecimal a, double b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a + bb;
        }
        public static SKSpecialDecimal operator +(double a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa + b;
        }
        public static SKSpecialDecimal operator +(SKSpecialDecimal a, int b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a + bb;
        }
        public static SKSpecialDecimal operator +(int a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa + b;
        }

        /// <summary>
        /// 减法，有效位数与加法类似，取决于绝对误差最大的数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator -(SKSpecialDecimal a, SKSpecialDecimal b)
        {
            SKSpecialDecimal b_copy = new SKSpecialDecimal(b);
            b_copy.inverse();
            return (a + b_copy);
        }
        public static SKSpecialDecimal operator -(SKSpecialDecimal a, double b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a - bb;
        }
        public static SKSpecialDecimal operator -(double a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa - b;
        }
        public static SKSpecialDecimal operator -(SKSpecialDecimal a, int b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a - bb;
        }
        public static SKSpecialDecimal operator -(int a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa - b;
        }

        /// <summary>
        /// 不考虑精度、误差情况下的加法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static SKSpecialDecimal Add(SKSpecialDecimal a, SKSpecialDecimal b)
        {

            if (a.get_digit() == 0)
                return new SKSpecialDecimal(b);
            else if (b.get_digit() == 0)
                return new SKSpecialDecimal(a);
            SKSpecialDecimal ans = new SKSpecialDecimal();
            int compare_abs = abs(a).compare_to(abs(b));

            ans.exp_10 = Math.Max(a.get_exp(), b.get_exp()) + 1;
            int up_range = ans.exp_10;
            int down_range = Math.Min(a.get_exp() - a.get_digit(), b.get_exp() - b.get_digit()) + 1;
            int range = up_range - down_range + 1;
            int carry = 0;
            if (a.positive == b.positive)
            {
                ans.positive = a.positive;
                for (int i = range - 1; i > -1; i--)
                {
                    byte tmp = (byte)((a[i - ans.get_exp() + a.get_exp()] + b[i - ans.get_exp() + b.get_exp()] + carry));
                    ans[i] = (byte)(tmp % 10);
                    if (tmp > 9)
                        carry = 1;
                    else
                        carry = 0;
                }
            }
            else if (compare_abs == 0)
                ans[0] = 0;
            else
            {
                SKSpecialDecimal bigger, smaller;
                if (compare_abs > 0)
                {
                    ans.positive = a.positive;
                    bigger = a;
                    smaller = b;
                }
                else
                {
                    ans.positive = b.positive;
                    bigger = b;
                    smaller = a;
                }
                for (int i = range - 1; i > -1; i--)
                {
                    sbyte tmp = (sbyte)(((sbyte)bigger[i - ans.get_exp() + bigger.get_exp()] - (sbyte)smaller[i - ans.get_exp() + smaller.get_exp()] - carry));
                    ans[i] = (byte)((tmp + 10) % 10);
                    if (tmp < 0)
                        carry = 1;
                    else
                        carry = 0;
                }
            }
            ans.fix();
            return ans;
        }
        /// <summary>
        /// 存储数据，其长度表示有效位数，高位在前
        /// <para>科学计数法，十进制</para>
        /// </summary>
        private List<byte> data = new List<byte>();
        /// <summary>
        /// 表示10的几次方
        /// </summary>
        private int exp_10 = 0;
        /// <summary>
        /// 是否为正数
        /// </summary>
        private bool positive = true;
    }
}
