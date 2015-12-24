//SKAlgorithm
//Author: ShadowK
//E-mail: zhu.shadowk@gmail.com
//Different ways to calculate arctanx.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKArctanX
{
    class SKAlgorithm
    {
        #region PI_and_e
        /// <summary>
        /// 常数pi的string
        /// </summary>
        const string string_SK_pi = @"3.
                                1415926535 8979323846 2643383279 5028841971 6939937510 
                                5820974944 5923078164 0628620899 8628034825 3421170679 
                                8214808651 3282306647 0938446095 5058223172 5359408128 
                                4811174502 8410270193 8521105559 6446229489 5493038196 
                                4428810975 6659334461 2847564823 3786783165 2712019091 
                                4564856692 3460348610 4543266482 1339360726 0249141273 
                                7245870066 0631558817 4881520920 9628292540 9171536436 
                                7892590360 0113305305 4882046652 1384146951 9415116094 
                                3305727036 5759591953 0921861173 8193261179 3105118548 
                                0744623799 6274956735 1885752724 8912279381 8301194912";
        /// <summary>
        /// 常数e的string
        /// </summary>
        const string string_SK_e = @"2.
                                7182818284 5904523536 0287471352 6624977572 4709369995 
                                9574966967 6277240766 3035354759 4571382178 5251664274
                                2746639193 2003059921 8174135966 2904357290 0334295260
                                5956307381 3232862794 3490763233 8298807531 9525101901
                                1573834187 9307021540 8914993488 4167509244 7614606680
                                8226480016 8477411853 7423454424 3710753907 7744992069
                                5517027618 3860626133 1384583000 7520449338 2656029760
                                6737113200 7093287091 2744374704 7230696977 2093101416
                                9283681902 5515108657 4637721112 5238978442 5056953696
                                7707854499 6996794686 4454905987 9316368892 3009879312";
        #endregion

        /// <summary>
        /// 最大有效位数限制
        /// </summary>
        public const int LIMIT_DIGIT = 100;
        /// <summary>
        /// 辛普森算法限制
        /// </summary>
        public const int SIMPSON_LIMIT = 16;

        public const double TALOR_THR = 0.7;
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SKAlgorithm() 
        {
            
        }

        /// <summary>
        /// 利用泰勒展开法计算arctan(x)
        /// <para>当x接近1时（> <see cref="TALOR_THR"/>)采用在1处泰勒展开的方法以减少计算时间</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="digit">有效位数</param>
        /// <param name="pb">进度条（可选）</param>
        /// <returns></returns>
        public SKSpecialDecimal Talor(SKSpecialDecimal _x, int digit, System.Windows.Forms.ProgressBar pb = null)
        {
            if (digit <= 0 || digit > LIMIT_DIGIT)
                return new SKSpecialDecimal();
            if (_x.compare_to(0) == 0)
                return new SKSpecialDecimal(0);
            SKSpecialDecimal x = new SKSpecialDecimal(_x);
            x[digit + APPEND_ADD] = (byte)0;
            int cmp_1_ans = x.compare_to(1);
            if (!x.get_positive())
            {
                x.inverse();
                SKSpecialDecimal ret = Talor(x, digit, pb);
                ret.inverse();
                return ret;
            }
            else if (cmp_1_ans > 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal((pi / (new SKSpecialDecimal(2, x.get_digit())) - Talor((new SKSpecialDecimal(1, x.get_digit())) / x, digit, pb)));
                ret.cut(digit);
                return ret;
            }
            else if (cmp_1_ans == 0)
            {
                SKSpecialDecimal ret = pi / new SKSpecialDecimal(4, digit + APPEND_ADD);
                ret.cut(digit);
                return ret;
            }
            else if (x.compare_to(TALOR_THR) > 0) //应当从1处展开
            {
                SKSpecialDecimal ret = pi / new SKSpecialDecimal(4, digit + APPEND_ADD);
                bool postive = false;
                SKSpecialDecimal x_base = SKSpecialDecimal.pow(new SKSpecialDecimal(1, digit + APPEND_ADD) - x, 4) / (new SKSpecialDecimal(4, digit + APPEND_ADD));
                SKSpecialDecimal acc_mul = new SKSpecialDecimal(1,x_base.get_digit() + 1);
                int i = 0;
                while (true)
                {
                    SKSpecialDecimal i_add_part_1 = acc_mul * (new SKSpecialDecimal(1, digit + APPEND_ADD) - x) / (new SKSpecialDecimal(2, digit + APPEND_ADD));
                    if (!postive)
                        i_add_part_1.inverse();
                    postive = !postive;
                    acc_mul = acc_mul * x_base;
                    SKSpecialDecimal i_add_part_2 =
                        (new SKSpecialDecimal(1, digit + APPEND_ADD)) / (new SKSpecialDecimal(4 * i + 1, digit + APPEND_ADD)) +
                        (new SKSpecialDecimal(1, digit + APPEND_ADD) - x) / (new SKSpecialDecimal(4 * i + 2, digit + APPEND_ADD)) +
                        SKSpecialDecimal.pow((new SKSpecialDecimal(1, digit + APPEND_ADD) - x), 2) / (new SKSpecialDecimal(8 * i + 6, digit + APPEND_ADD));
                    SKSpecialDecimal i_add = i_add_part_1 * i_add_part_2;
                    i++;
                    SKSpecialDecimal ret_tmp = ret + i_add;
                    SKSpecialDecimal eps = new SKSpecialDecimal(1);
                    eps.mul_10(ret_tmp.get_exp() - digit - 1);
                    if (SKSpecialDecimal.abs(SKSpecialDecimal.abs(ret_tmp) - SKSpecialDecimal.abs(ret)).compare_to(eps) < 0)
                        break;
                    ret = ret_tmp;
                    if (pb != null)
                    {
                        if (i == 1)
                        {
                            pb.Value = 0;
                            pb.Maximum = digit;
                        }
                        else
                            pb.Value = (ret.get_exp() - i_add.get_exp() > digit) ? digit : ret.get_exp() - i_add.get_exp();
                    }
                }
                return ret;
            }
            else
            {
                SKSpecialDecimal ret = new SKSpecialDecimal(x);
                SKSpecialDecimal tmp_last = null;
                SKSpecialDecimal acc_mul = new SKSpecialDecimal(x);
                bool postive = false;
                int i = 3;
                while (true)
                {
                    //tmp = SKSpecialDecimal.pow(tmp, i);
                    acc_mul = acc_mul * x * x;
                    SKSpecialDecimal tmp = acc_mul / (new SKSpecialDecimal(i, x.get_digit()));
                    i = i + 2;
                    if (!postive)
                        tmp.inverse();
                    postive = !postive;
                    if (tmp_last != null)
                    {
                        SKSpecialDecimal eps = new SKSpecialDecimal(1);
                        eps.mul_10(ret.get_exp() - digit - 1);
                        if (SKSpecialDecimal.abs(tmp + tmp_last).compare_to(eps) < 0)
                            break;
                    }
                    ret = ret + tmp;
                    tmp_last = tmp;
                    if (pb != null)
                    {
                        if (i == 5)
                        {
                            pb.Value = 0;
                            pb.Maximum = digit;
                        }
                        else
                            pb.Value = (ret.get_exp() - tmp.get_exp() > digit) ? digit : ret.get_exp() - tmp.get_exp();
                    }
                }
                ret.cut(digit);
                return ret;
            }
        }
        /// <summary>
        /// 利用泰勒展开法计算arctan(x)
        /// <para>当x接近1时（> <see cref="TALOR_THR"/>)采用在1处泰勒展开的方法以减少计算时间</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="digit">有效位数</param>
        /// <param name="pb">进度条（可选）</param>
        /// <returns></returns>
        public SKSpecialDecimal Talor(double x, int digit, System.Windows.Forms.ProgressBar pb = null)
        {
            return Talor(new SKSpecialDecimal(x, digit + APPEND_ADD), digit, pb);
        }

        /// <summary>
        /// 利用复化辛普森公式展开法计算arctan(x)
        /// <para>速度较慢，最大有效位置有额外的限制</para>
        /// <para>参见<see cref="SKAlgorithm.SIMPSON_LIMIT"/></para>
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <param name="pb">进度条（可选）</param>
        /// <returns></returns>
        public SKSpecialDecimal Simpson(SKSpecialDecimal _x, int digit, System.Windows.Forms.ProgressBar pb = null)
        {
            if (digit <= 0 || digit > LIMIT_DIGIT || digit > SIMPSON_LIMIT)
                return new SKSpecialDecimal();
            if (_x.compare_to(0) == 0)
                return new SKSpecialDecimal(0);
            SKSpecialDecimal x = new SKSpecialDecimal(_x);
            //x[digit + APPEND_ADD] = (byte)0;
            int cmp_1_ans = x.compare_to(1);
            if (!x.get_positive())
            {
                x.inverse();
                SKSpecialDecimal ret = Simpson(x, digit, pb);
                ret.inverse();
                return ret;
            }
            else if (cmp_1_ans > 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal((pi / (new SKSpecialDecimal(2, x.get_digit())) - Simpson((new SKSpecialDecimal(1, x.get_digit())) / x, digit, pb)));
                ret.cut(digit);
                return ret;
            }
            else if (cmp_1_ans == 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal(pi) / new SKSpecialDecimal(4, digit + APPEND_ADD);
                ret.cut(digit);
                return ret;
            }
            else
            {
                SKSpecialDecimal ret;
                /*
                SKSpecialDecimal h = new SKSpecialDecimal(1,digit + APPEND_ADD);
                h.mul_10(-(-x.get_exp() + digit / 4));
                for (int i = 0; i < (x.get_exp() + digit) % 4; i++)
                    h = h / (new SKSpecialDecimal(1.8, digit + APPEND_ADD));
                */
                double need = x.get_exp() - (digit + 2.0D) / 4 + 1;
                double hh = Math.Pow(10, need);
                SKSpecialDecimal h = new SKSpecialDecimal(hh, digit + APPEND_ADD);
                //这基本可作为结果的精度要求，注意到x很小时，arctanx趋近于x
                //因此log(x)与log(arctanx)有相似的值
                //具体分析详见报告
                SKSpecialDecimal _n = x / h;
                int n = SKSpecialDecimal.floor(_n);
                h = x / (new SKSpecialDecimal(n, x.get_digit()));
                if (pb != null)
                    pb.Maximum = n / 10;
                SKSpecialDecimal part_a = new SKSpecialDecimal(0);
                SKSpecialDecimal part_b = new SKSpecialDecimal(0);
                SKSpecialDecimal x_a_now = h /(new SKSpecialDecimal(2, h.get_digit()));
                SKSpecialDecimal x_b_now = new SKSpecialDecimal(h);
                part_a = part_a + arctan_1(x_a_now);
                x_a_now = x_a_now + h;
                for (int k = 1; k < n; k++)
                {
                    SKSpecialDecimal add_part_a = arctan_1(x_a_now);
                    SKSpecialDecimal add_part_b = arctan_1(x_b_now);
                    part_a = part_a + add_part_a;
                    part_b = part_b + add_part_b;
                    x_a_now = x_a_now + h;
                    x_b_now = x_b_now + h;
                    if (pb != null && k % 10 == 0)
                        pb.PerformStep();
                }
                part_a = part_a * (new SKSpecialDecimal(4, part_a.get_digit()));
                part_b = part_b * (new SKSpecialDecimal(2, part_b.get_digit()));
                ret = (new SKSpecialDecimal(1,part_a.get_digit() +APPEND_ADD)) + part_a + part_b + arctan_1(x);
                ret = h / (new SKSpecialDecimal(6, h.get_digit())) * ret;
                ret.cut(digit);
                return ret;
            }
        }
        /// <summary>
        /// 利用复化辛普森公式展开法计算arctan(x)
        /// <para>速度较慢，最大有效位置有额外的限制</para>
        /// <para>参见<see cref="SKAlgorithm.SIMPSON_LIMIT"/></para>
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <param name="pb">进度条（可选）</param>
        /// <returns></returns>
        public SKSpecialDecimal Simpson(double x, int digit,System.Windows.Forms.ProgressBar pb = null)
        {
            return Simpson(new SKSpecialDecimal(x, digit), digit, pb);
        }
        /// <summary>
        /// 科特斯法，还有bug
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        public SKSpecialDecimal Cotes(SKSpecialDecimal _x, int digit, System.Windows.Forms.ProgressBar pb = null)
        {
            if (digit <= 0 || digit > LIMIT_DIGIT)
                return new SKSpecialDecimal();
            if (_x.compare_to(0) == 0)
                return new SKSpecialDecimal(0);
            SKSpecialDecimal x = new SKSpecialDecimal(_x);
            //x[digit + APPEND_ADD] = (byte)0;
            int cmp_1_ans = x.compare_to(1);
            if (!x.get_positive())
            {
                x.inverse();
                SKSpecialDecimal ret = Cotes(x, digit, pb);
                ret.inverse();
                return ret;
            }
            else if (cmp_1_ans > 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal((pi / (new SKSpecialDecimal(2, x.get_digit())) - Cotes((new SKSpecialDecimal(1, x.get_digit())) / x, digit, pb)));
                ret.cut(digit);
                return ret;
            }
            else if (cmp_1_ans == 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal(pi) / new SKSpecialDecimal(4, digit + APPEND_ADD);
                ret.cut(digit);
                return ret;
            }
            else
            {
                SKSpecialDecimal ret;
                /*
                SKSpecialDecimal h = new SKSpecialDecimal(1, digit + APPEND_ADD);
                h.mul_10(-(-x.get_exp() + digit / 6));
                for (int i = 0; i < (x.get_exp() + digit) % 6; i++)
                    h = h / (new SKSpecialDecimal(1.5, digit + APPEND_ADD));
                */
                double need = x.get_exp() - (digit + 1.0D) / 6 + 1;
                double hh = Math.Pow(10, need);
                SKSpecialDecimal h = new SKSpecialDecimal(hh, digit + APPEND_ADD);
                //这基本可作为结果的精度要求，注意到x很小时，arctanx趋近于x
                //因此log(x)与log(arctanx)有相似的值
                //具体分析详见报告
                SKSpecialDecimal _n = x / h;
                int n = SKSpecialDecimal.floor(_n);
                h = x / (new SKSpecialDecimal(n, x.get_digit()));
                if (pb != null)
                    pb.Maximum = n / 5;
                SKSpecialDecimal part_a = new SKSpecialDecimal(0);
                SKSpecialDecimal part_b = new SKSpecialDecimal(0);
                SKSpecialDecimal part_c = new SKSpecialDecimal(0);
                SKSpecialDecimal part_d = new SKSpecialDecimal(0);
                SKSpecialDecimal x_a_now = h / (new SKSpecialDecimal(4, h.get_digit()));
                SKSpecialDecimal x_b_now = h / (new SKSpecialDecimal(2, h.get_digit()));
                SKSpecialDecimal x_c_now = h / (new SKSpecialDecimal(4, h.get_digit())) * (new SKSpecialDecimal(3, h.get_digit()));
                SKSpecialDecimal x_d_now = new SKSpecialDecimal(h);
                part_a = part_a + arctan_1(x_a_now);
                x_a_now = x_a_now + h;
                part_b = part_b + arctan_1(x_b_now);
                x_b_now = x_b_now + h;
                part_c = part_c + arctan_1(x_c_now);
                x_c_now = x_c_now + h;
                for (int k = 1; k < n; k++)
                {
                    part_a = part_a + arctan_1(x_a_now);
                    part_b = part_b + arctan_1(x_b_now);
                    part_c = part_c + arctan_1(x_c_now);
                    part_d = part_d + arctan_1(x_d_now);

                    x_a_now = x_a_now + h;
                    x_b_now = x_b_now + h;
                    x_c_now = x_c_now + h;
                    x_d_now = x_d_now + h;
                    if (pb != null && k % 5 == 0)
                        pb.PerformStep();
                }
                part_a = part_a * (new SKSpecialDecimal(32, part_a.get_digit()));
                part_b = part_b * (new SKSpecialDecimal(12, part_b.get_digit()));
                part_c = part_c * (new SKSpecialDecimal(32, part_c.get_digit()));
                part_d = part_d * (new SKSpecialDecimal(14, part_d.get_digit()));
                ret = (new SKSpecialDecimal(7, part_a.get_digit() + APPEND_ADD)) + part_a + part_b + part_c + part_d + (new SKSpecialDecimal(7, x.get_digit())) * arctan_1(x);
                ret = h / (new SKSpecialDecimal(90, h.get_digit())) * ret;
                ret.cut(digit);
                return ret;
            }
        }
        public SKSpecialDecimal Cotes(double x, int digit, System.Windows.Forms.ProgressBar pb = null)
        {
            return Cotes(new SKSpecialDecimal(x, digit), digit, pb);
        }


        /// <summary>
        /// 返回arctanx一阶导的取值
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private SKSpecialDecimal arctan_1(SKSpecialDecimal _x)
        {
            //double x = _x.to_double();
            //double _ret = 1 / (1 + x * x);
            //return new SKSpecialDecimal(_ret, _x.get_digit() + APPEND_ADD);
            return (new SKSpecialDecimal(1, _x.get_digit() + APPEND_ADD)) / (SKSpecialDecimal.pow(_x, 2) + (new SKSpecialDecimal(1, _x.get_digit() + SIMPSON_LIMIT + APPEND_ADD)));
        }

        /// <summary>
        /// 常数与默认增加的位数
        /// </summary>
        private const int APPEND_ADD = LIMIT_DIGIT / 10;
        /// <summary>
        /// arctanx的在[0,1]上的4阶导最大值
        /// </summary>
        //private SKSpecialDecimal ARCTAN_4_MAX = new SKSpecialDecimal(24, LIMIT_DIGIT);
        /// <summary>
        /// arctanx的在[0,1]上的6阶导最大值
        /// </summary>
        //private SKSpecialDecimal ARCTAN_6_MAX = new SKSpecialDecimal(800, LIMIT_DIGIT);
        /// <summary>
        /// 常数pi
        /// </summary>
        private SKSpecialDecimal pi = new SKSpecialDecimal(string_SK_pi);
        /// <summary>
        /// 常数e
        /// </summary>
        private SKSpecialDecimal e = new SKSpecialDecimal(string_SK_e);
    }
}
