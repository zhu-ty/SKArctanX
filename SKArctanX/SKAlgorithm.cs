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
        const int LIMIT_DIGIT = 100;
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SKAlgorithm() { }

        /// <summary>
        /// 利用泰勒展开法计算arctan(x)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="digit">有效位数</param>
        /// <returns></returns>
        public SKSpecialDecimal Talor(SKSpecialDecimal _x, int digit)
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
                SKSpecialDecimal ret = Talor(x, digit);
                ret.inverse();
                return ret;
            }
            else if (cmp_1_ans > 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal((pi / (new SKSpecialDecimal(2, x.get_digit())) - Talor((new SKSpecialDecimal(1, x.get_digit())) / x, digit)));
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
                SKSpecialDecimal ret = new SKSpecialDecimal(x);
                SKSpecialDecimal tmp_last = null;
                bool postive = false;
                int i = 3;
                while (true)
                {
                    SKSpecialDecimal tmp = new SKSpecialDecimal(x);
                    tmp = SKSpecialDecimal.pow(tmp, i);
                    tmp = tmp / (new SKSpecialDecimal(i, x.get_digit()));
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
                    else
                        ret = ret + tmp;
                    tmp_last = tmp;
                }
                ret.cut(digit);
                return ret;
            }
        }
        /// <summary>
        /// 利用泰勒展开法计算arctan(x)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="digit">有效位数</param>
        /// <returns></returns>
        public SKSpecialDecimal Talor(double x, int digit)
        {
            return Talor(new SKSpecialDecimal(x,digit + APPEND_ADD), digit);
        }

        /// <summary>
        /// 利用复化辛普森公式展开法计算arctan(x)
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public SKSpecialDecimal Simpson(SKSpecialDecimal _x, int digit)
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
                SKSpecialDecimal ret = Simpson(x, digit);
                ret.inverse();
                return ret;
            }
            else if (cmp_1_ans > 0)
            {
                SKSpecialDecimal ret = new SKSpecialDecimal((pi / (new SKSpecialDecimal(2, x.get_digit())) - Simpson((new SKSpecialDecimal(1, x.get_digit())) / x, digit)));
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
                SKSpecialDecimal ret = new SKSpecialDecimal(x);
                SKSpecialDecimal h = new SKSpecialDecimal(1);
                h.mul_10(-(x.get_exp() + digit + APPEND_ADD) / 4 - 1);
                //这基本可作为结果的精度要求，注意到x很小时，arctanx趋近于x
                //因此log(x)与log(arctanx)有相似的值
                //具体分析详见报告

                return ret;
            }
        }
        /// <summary>
        /// 利用复化辛普森公式展开法计算arctan(x)
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public SKSpecialDecimal Simpson(double x, int digit)
        {
            return Simpson(new SKSpecialDecimal(x, digit + APPEND_ADD), digit);
        }
        /// <summary>
        /// 返回arctanx一阶导的取值
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private SKSpecialDecimal arctan_1(SKSpecialDecimal x)
        {
            return (new SKSpecialDecimal(1, x.get_digit() + APPEND_ADD)) / (SKSpecialDecimal.pow(x, 2) * (new SKSpecialDecimal(1, x.get_digit() + APPEND_ADD)));
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
