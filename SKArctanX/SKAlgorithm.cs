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
        public SKAlgorithm() 
        {
            init_talor_special();
        }

        /// <summary>
        /// 利用泰勒展开法计算arctan(x)
        /// <para>当x十分接近1时（>0.9)采用特别的算法（在1处泰勒展开）以减少计算时间</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="digit">有效位数</param>
        /// <param name="no_special">强制不使用特别算法</param>
        /// <returns></returns>
        public SKSpecialDecimal Talor(SKSpecialDecimal _x, int digit,bool no_special = false)
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
            else if (x.compare_to(0.9) > 0 && !no_special) //泰勒特别算法
            {
                SKSpecialDecimal x_a = x - (new SKSpecialDecimal(1, x.get_digit() + APPEND_ADD));
                SKSpecialDecimal f_a = new SKSpecialDecimal(pi) / new SKSpecialDecimal(4, digit + APPEND_ADD);
                SKSpecialDecimal acc_mul = new SKSpecialDecimal(x_a);
                SKSpecialDecimal ret = new SKSpecialDecimal(f_a);
                int i = 0;
                while (true)
                {
                    if (i > 45)
                        return Talor(x, digit, true);//超过了精度要求，只能用非特别算法慢慢算了
                    SKSpecialDecimal tmp_ret = ret + talor_special[i] * acc_mul;
                    SKSpecialDecimal eps = new SKSpecialDecimal(1);
                    eps.mul_10(tmp_ret.get_exp() - digit - 1);
                    if (talor_special[i].compare_to(0) != 0 && SKSpecialDecimal.abs(ret - tmp_ret).compare_to(eps) < 0)
                        break;
                    ret = tmp_ret;
                    acc_mul = acc_mul * x_a;
                    i++;
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
                }
                ret.cut(digit);
                return ret;
            }
        }
        /// <summary>
        /// 利用泰勒展开法计算arctan(x)
        /// <para>当x十分接近1时（>0.9)采用特别的算法（在1处泰勒展开）以减少计算时间</para>
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
        /// 速度太慢，仍有bug
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public SKSpecialDecimal Simpson(SKSpecialDecimal _x, int digit, System.Windows.Forms.ProgressBar pb = null)
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
                SKSpecialDecimal ret = new SKSpecialDecimal(0);
                SKSpecialDecimal h = new SKSpecialDecimal(1,digit + APPEND_ADD);
                h.mul_10(-(x.get_exp() + digit) / 4 - 2);
                //这基本可作为结果的精度要求，注意到x很小时，arctanx趋近于x
                //因此log(x)与log(arctanx)有相似的值
                //具体分析详见报告
                SKSpecialDecimal _n = x / h;
                int n = SKSpecialDecimal.floor(_n);
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
                ret = ret + part_a + part_b + arctan_1(x);
                ret = h / (new SKSpecialDecimal(6, h.get_digit())) * ret;
                ret.cut(digit);
                return ret;
            }
        }
        /// <summary>
        /// 利用复化辛普森公式展开法计算arctan(x)
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public SKSpecialDecimal Simpson(double x, int digit,System.Windows.Forms.ProgressBar pb = null)
        {
            return Simpson(new SKSpecialDecimal(x, digit), digit, pb);
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
            return (new SKSpecialDecimal(1, _x.get_digit() + APPEND_ADD)) / (SKSpecialDecimal.pow(_x, 2) + (new SKSpecialDecimal(1, _x.get_digit() + LIMIT_DIGIT)));
        }

        /// <summary>
        /// 常数与默认增加的位数
        /// </summary>
        private const int APPEND_ADD = LIMIT_DIGIT / 20;
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

        #region TalorSpecial
        //泰勒公式对于x十分接近1的时候的收敛速度较慢，此处采用一些特殊的技巧来进行收敛处理。
        //仅限上限为100位，否则本列表应当继续加长
        private SKSpecialDecimal[] talor_special = new SKSpecialDecimal[49];
        private void init_talor_special()
        {
            for (int i = 0; i < 49; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("1");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("2");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 1:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-1");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("4");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 2:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("1");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("12");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 3:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 4:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-1");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("40");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 5:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("1");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("48");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 6:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-1");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("112");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 7:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 8:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("512409557603043135");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("147573952589676412928");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 9:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-3689348814741910005");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1180591620717411303424");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 10:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("26831627743577531175");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("18889465931478580854784");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 11:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 12:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-2906071681765935288375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("4835703278458516698824704");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 13:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("43175922129093898319175");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("77371252455336267181195264");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 14:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-322380218563901110287675");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1237940039285380274899124224");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 15:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 16:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("2275625072215772430528375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("19807040628566084398385987584");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 17:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-68774446626965565583222125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("633825300114114700748351602688");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 18:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("521237911278054838549807125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("10141204801825835211973625643008");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 19:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 20:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-30182157148291178329987813125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1298074214633706907132624082305024");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 21:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("921927709256894192161125691875");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("41538374868278621028243970633760768");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 22:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-14109502332975073256230215091875");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1329227995784915872903807060280344576");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 23:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 24:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("830767497365572445474442601329375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("170141183460469231731687303715884105728");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 25:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-25562076842017610577693061160484375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("5444517870735015415413993718908291383296");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 26:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("393845332084419511763474778161015625");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("174224571863520493293247799005065324265472");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 27:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 28:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-23467749442823344672710423434223140625");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("22300745198530623141535718272648361505980416");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 29:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("1451871432196004274652020957310755703125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1427247692705959881058285969449495136382746624");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 30:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-22480589917873614957764416873573047890625");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("45671926166590716193865151022383844364247891968");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 31:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 32:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("337890078765615520040673968816556728671875");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1461501637330902918203684832716283019655932542976");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 33:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-10494468328720293085354192594199609732109375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("46768052394588893382517914646921056628989841375232");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 34:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("326228044047076533447075247374727125844453125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("2993155353253689176481146537402947624255349848014848");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 35:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 36:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-39500044252186571007334074339603030462624609375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("766247770432944429179173513575154591809369561091801088");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 37:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("1230738220910234254605139202096679972717512109375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("24519928653854221733733552434404946937899825954937634816");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 38:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-38373786580175503212062192640355489595210019140625");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1569275433846670190958947355801916604025588861116008628224");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 39:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 40:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("9344485005279811921409454243696152063624887283203125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("803469022129495137770981046170581301261101496891396417650688");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 41:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-583807825091767279799545494316429009621446526376953125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("51422017416287688817342786954917203280710495801049370729644032");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 42:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("9123694382829477625819735093081832708470712283978515625");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("1645504557321206042154969182557350504982735865633579863348609024");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 43:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 44:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-1115929197402076647520134035913455590018054492437462890625");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("421249166674228746791672110734681729275580381602196445017243910144");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 45:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("279467485958085298717442972477822837873133672787387470703125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("107839786668602559178668060348078522694548577690162289924414440996864");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 46:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("-4376341907769165182788712809991931185475801165645144990234375");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("3450873173395281893717377931138512726225554486085193277581262111899648");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                    case 47:
                        {
                            talor_special[i] = new SKSpecialDecimal(0);
                            break;
                        }
                    case 48:
                        {
                            SKSpecialDecimal fractions = new SKSpecialDecimal("537307610553863210776083212711278261885010503804893602314453125");
                            fractions[LIMIT_DIGIT + 1] = (byte)0;
                            SKSpecialDecimal numerator = new SKSpecialDecimal("883423532389192164791648750371459257913741948437809479060803100646309888");
                            numerator[LIMIT_DIGIT + 1] = (byte)0;
                            talor_special[i] = fractions / numerator;
                            //talor_special[i][LIMIT_DIGIT + 1] = (byte)0;
                            break;
                        }
                }
            }
        }
        #endregion
    }
}
