using Point.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Point.Common.Util
{

    public class ChineseLunisolarDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public DateTime ToDate()
        {
            try
            {
                return new DateTime(Year, Month, Day);
            }
            catch
            {
                throw new BusinessException("农历转换为公历失败");
            }
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}-{2}", Year, Month, Day);
        }

    }

    public static class DateTimeExt
    {
        /// <summary>
        /// 将日期时间置为当天最小时间
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime ToCurrentDayMinVal(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
        }

        /// <summary>
        /// 将日期时间置为当天最大时间
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime ToCurrentDayMaxVal(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);
        }

        ///<summary>
        /// 公历转农历
        ///</summary>
        ///<param name="d">公历日期时间</param>
        ///<returns>农历日期</returns>
        public static ChineseLunisolarDate AsChineseDate(this DateTime d)
        {
            try
            {
                ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();
                var year = calendar.GetYear(d);
                var month = calendar.GetMonth(d);
                var day = calendar.GetDayOfMonth(d);
                int leapMonth = calendar.GetLeapMonth(year);
                month = leapMonth > 0 && leapMonth <= month ? month - 1 : month;

                return new ChineseLunisolarDate { Year = year, Month = month, Day = day };
            }
            catch
            {
                throw new BusinessException("公历转换为农历失败");
            }
        }

        /// <summary>
        /// 农历转公历
        /// </summary>
        /// <param name="d">农历日期时间</param>
        /// <returns>公历日期</returns>
        public static DateTime AsGregorianDate(this DateTime d)
        {
            return AsGregorianDate(d.Year, d.Month, d.Day);
        }

        /// <summary>
        /// 农历转公历
        /// </summary>
        /// <param name="d">农历日期（字符串格式，2月30日）</param>
        /// <returns></returns>
        public static DateTime AsGregorianDate(this string d)
        {
            if (string.IsNullOrWhiteSpace(d))
                throw new ArgumentNullException("d");

            var dArr = d.Split(new char[] { '-', '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (dArr.Length != 3)
                throw new ArgumentException("参数错误");

            int year = Convert.ToInt32(dArr[0]);
            int month = Convert.ToInt32(dArr[1]);
            int day = Convert.ToInt32(dArr[2]);

            return AsGregorianDate(year, month, day);
        }

        private static DateTime AsGregorianDate(int year, int month, int day)
        {
            bool IsLeapMonth = IsLeapYear(year);

            ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();

            if (year < 1902 || year > 2100)
                throw new BusinessException("年超过范围");
            if (month < 1 || month > 12)
                throw new BusinessException("月份超过范围");
            if (day < 1 || day > calendar.GetDaysInMonth(year, month))
                throw new BusinessException("天数超过范围");

            int num1 = 0, num2 = 0;
            int leapMonth = calendar.GetLeapMonth(year);

            if (((leapMonth == month + 1) && IsLeapMonth) || (leapMonth > 0 && leapMonth <= month))
                num2 = month;
            else
                num2 = month - 1;

            while (num2 > 0)
            {
                num1 += calendar.GetDaysInMonth(year, num2--);
            }

            DateTime dt = GetLunarNewYearDate(year);
            return dt.AddDays(num1 + day - 1);
        }

        /// <summary>
        /// 转为格林尼治时间
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime AsUtcDate(this DateTime d)
        {
            if (d.Kind == DateTimeKind.Unspecified)
            {
                return new DateTime(d.Ticks, DateTimeKind.Utc);
            }

            return d.ToUniversalTime();
        }

        /// <summary>
        /// 转换为java时间（ticks）
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long AsJavaDataTicks(this DateTime d)
        {//java的1986-04-04 01:00:00到1991-09-14 22:59:59这段日期-1小时
            if (d.Ticks >= new DateTime(1986, 4, 4, 1, 0, 0).Ticks && d.Ticks < new DateTime(1991, 9, 14, 23, 0, 0).Ticks)
            {
                d = d.AddTicks(-3600 * 1000 * 10000L);
            }

            TimeSpan ts = new TimeSpan(d.Ticks - new DateTime(1970, 1, 1, 8, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
            // TimeSpan ts = new TimeSpan(d.AsUtcDate().Ticks - new DateTime(1970, 1, 1, 8, 0, 0).AsUtcDate().Ticks);
            // return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 将NET时间转为Unix时间戳
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long AsUnixTimestamp(this DateTime d)
        {
            return (long)(d - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        /// <summary>
        /// 将Unix时间戳转为NET时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimestamp(long date)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtDateTime.AddSeconds(date).ToLocalTime();
        }

        /// <summary>
        /// java时间转为C#时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime JavaDateAsCSharpDate(this long time)
        {//java的1986-04-04 01:00:00到1991-09-14 22:59:59这段日期+1小时
            DateTime dt = new DateTime(time * 10000 + new DateTime(1970, 1, 1, 0, 0, 0).Ticks).ToLocalTime();
            if (dt.Ticks >= new DateTime(1986, 4, 4, 1, 0, 0).Ticks && dt.Ticks < new DateTime(1991, 9, 14, 23, 0, 0).Ticks)
            {
                dt = dt.AddTicks(3600 * 1000 * 10000L);
            }
            return dt;
        }

        /// <summary>
        /// 判断一年中是否闰月
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>是否闰月</returns>
        public static bool IsLeapYear(int year)
        {
            return (year % 400 == 0 || (year % 4 == 0 && year % 400 != 0));
        }

        /// <summary>
        /// 周中文显示名称
        /// </summary>
        public static string[] WeekCnName = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

        /// <summary>
        /// 
        /// </summary>
        public static string[] WeekCnName2 = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

        /// <summary>
        /// 月份中文显示名称
        /// </summary>
        public static string[] MonthCnName = new string[] { "", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };

        private static readonly int[] __MaxDays = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// 获取一个月的第一天
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime GetFirstDay(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1);
        }

        /// <summary>
        /// 获取一个月的最后一天
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime GetLastDay(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, __MaxDays[d.Month]);
        }

        /// <summary>
        /// 获取指定月的最大天数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int GetMaxDayByMonth(this DateTime d)
        {
            return __MaxDays[d.Month];
        }

        /// <summary>
        /// 获取指定月的最大天数
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static int GetMaxDayByMonth(int month)
        {
            return __MaxDays[month];
        }

        /// <summary>
        /// 获取指定月份节假日的天数
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static int GetHolidayCount(int year, int month)
        {

            var count = 0;
            for (int i = 1, maxDay = GetMaxDayByMonth(month); i <= maxDay; i++)
            {
                var d = new DateTime(year, month, i);
                var w = (int)d.DayOfWeek;

                if (w == 0 || w == 6)
                    count++;

            }

            return count;

        }

        /// <summary>
        /// 获取指定日期的季度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime d)
        {
            var season = 0;
            var month = d.Month;
            if (month >= 1 && month <= 3)
                season = 1;
            else if (month >= 4 && month <= 6)
                season = 2;
            else if (month >= 7 && month <= 9)
                season = 3;
            else
                season = 4;
            return season;
        }

        public static int GetWeek(this DateTime d)
        {
            var gc = new System.Globalization.GregorianCalendar();
            var first_day = new DateTime(d.Year, 1, 1);

            System.Globalization.CalendarWeekRule rule;

            switch (first_day.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    {
                        rule = CalendarWeekRule.FirstDay;
                        break;
                    }
                default:
                    {
                        rule = CalendarWeekRule.FirstFullWeek;
                        break;
                    }
            }
            return gc.GetWeekOfYear(d, rule, DayOfWeek.Monday);
        }


        public static void GetQuarterStartTimeAndEndTime(this DateTime curDate, out DateTime startTime, out DateTime endTime)
        {
            startTime = curDate.ToCurrentDayMinVal();
            endTime = curDate.ToCurrentDayMaxVal();

            var year = curDate.Year;
            var quarter = curDate.GetQuarter();
            switch (quarter)
            {
                case 1:
                    startTime = new DateTime(year, 1, 1);
                    endTime = new DateTime(year, 3, 31).ToCurrentDayMaxVal(); break;
                case 2:
                    startTime = new DateTime(year, 4, 1);
                    endTime = new DateTime(year, 6, 30).ToCurrentDayMaxVal(); break;
                case 3:
                    startTime = new DateTime(year, 7, 1);
                    endTime = new DateTime(year, 9, 30).ToCurrentDayMaxVal(); break;
                case 4:
                    startTime = new DateTime(year, 10, 1);
                    endTime = new DateTime(year, 12, 31).ToCurrentDayMaxVal(); break;
            }
        }

        public static void GetWeekStartTimeAndEndTime(this DateTime curDate, out DateTime startTime, out DateTime endTime)
        {
            var week = Convert.ToInt32(curDate.DayOfWeek);
            week = week == 0 ? 7 : week;
            int firstDay = -week + 1;
            int lastDay = 7 - week;
            startTime = curDate.AddDays(firstDay).ToCurrentDayMinVal();
            endTime = curDate.AddDays(lastDay).ToCurrentDayMaxVal();
        }

        public static void GetWeekStartTime(this DateTime curDate, out DateTime startTime)
        {
            int sw = Convert.ToInt32(curDate.DayOfWeek);
            startTime = curDate.AddDays((1 - (sw == 0 ? 7 : sw))).ToCurrentDayMinVal();
        }

        public static int GetTimeRangeWeekQty(DateTime startTime, DateTime endTime)
        {
            int sw = Convert.ToInt32(startTime.DayOfWeek);
            int ew = Convert.ToInt32(endTime.DayOfWeek);
            startTime = startTime.AddDays((1 - (sw == 0 ? 7 : sw)));
            endTime = endTime.AddDays((7 - (ew == 0 ? 7 : ew)));
            int weekQty = Convert.ToInt32(Math.Ceiling(endTime.Subtract(startTime).Days / 7.0));
            return weekQty;
        }

        #region private


        /// <summary>
        /// 获取指定年份春节当日（正月初一）的公历日期
        /// </summary>
        /// <param name="year">指定的年份</param>
        private static DateTime GetLunarNewYearDate(int year)
        {
            ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();
            DateTime dt = new DateTime(year, 1, 1);
            int cnYear = calendar.GetYear(dt);
            int cnMonth = calendar.GetMonth(dt);

            int num1 = 0;
            int num2 = calendar.IsLeapYear(cnYear) ? 13 : 12;

            while (num2 >= cnMonth)
            {
                num1 += calendar.GetDaysInMonth(cnYear, num2--);
            }

            num1 = num1 - calendar.GetDayOfMonth(dt) + 1;
            return dt.AddDays(num1);
        }

        #endregion

        /// <summary>
        /// 计算年龄
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int CalcAge(this DateTime birthDate)
        {
            var now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || now.Month == birthDate.Month && now.Day < birthDate.Day)
                age--;

            return age;
        }
    }

    public class ChinaCalendar
    {
        //默认系统当前日期
        private DateTime dtvalue = Convert.ToDateTime(DateTime.Now.ToShortDateString());

        //用来计算农历的初始日期
        private DateTime baseDate = new DateTime(1900, 1, 31);

        public int chinaYear;       //农历年
        public int chinaMonth;      //农历月
        public int doubleMonth;     //闰月
        public bool isLeap;         //是否闰月标记
        public int chinaDay;        //农历日

        /// <summary>
        /// 获取或设置类中应用日期  
        /// </summary>
        public DateTime GetDatetime
        {
            get { return dtvalue; }
            set
            {
                dtvalue = Convert.ToDateTime(value.ToShortDateString());
                InitializeValue();
            }
        }

        /// <summary>
        /// 获取该年的属相（生肖）
        /// </summary>
        public string GetAnimal
        {
            get { return Animal(); }
        }

        /// <summary>
        ///  获取农历年（天干 地支）
        /// </summary>
        public string GetChinaYear
        {
            get { return ChinaYear(); }
        }

        /// <summary>
        /// 获取农历月或闰月
        /// </summary>
        public string GetChinaMonth
        {
            get { return ChinaMonth(); }
        }

        /// <summary>
        /// 获取农历日
        /// </summary>
        public string GetChinaDay
        {
            get { return ChinaDay(); }
        }

        #region 农历的静态数据

        private static int[] ChinaCalendarInfo = { 0x04bd8,0x04ae0,0x0a570,0x054d5,0x0d260,0x0d950,0x16554,0x056a0,0x09ad0,0x055d2,

                                           0x04ae0,0x0a5b6,0x0a4d0,0x0d250,0x1d255,0x0b540,0x0d6a0,0x0ada2,0x095b0,0x14977,

                                           0x04970,0x0a4b0,0x0b4b5,0x06a50,0x06d40,0x1ab54,0x02b60,0x09570,0x052f2,0x04970,

                                           0x06566,0x0d4a0,0x0ea50,0x06e95,0x05ad0,0x02b60,0x186e3,0x092e0,0x1c8d7,0x0c950,

                                           0x0d4a0,0x1d8a6,0x0b550,0x056a0,0x1a5b4,0x025d0,0x092d0,0x0d2b2,0x0a950,0x0b557,

                                           0x06ca0,0x0b550,0x15355,0x04da0,0x0a5b0,0x14573,0x052b0,0x0a9a8,0x0e950,0x06aa0,

                                           0x0aea6,0x0ab50,0x04b60,0x0aae4,0x0a570,0x05260,0x0f263,0x0d950,0x05b57,0x056a0,

                                           0x096d0,0x04dd5,0x04ad0,0x0a4d0,0x0d4d4,0x0d250,0x0d558,0x0b540,0x0b6a0,0x195a6,

                                           0x095b0,0x049b0,0x0a974,0x0a4b0,0x0b27a,0x06a50,0x06d40,0x0af46,0x0ab60,0x09570,

                                           0x04af5,0x04970,0x064b0,0x074a3,0x0ea50,0x06b58,0x055c0,0x0ab60,0x096d5,0x092e0,

                                           0x0c960,0x0d954,0x0d4a0,0x0da50,0x07552,0x056a0,0x0abb7,0x025d0,0x092d0,0x0cab5,

                                           0x0a950,0x0b4a0,0x0baa4,0x0ad50,0x055d9,0x04ba0,0x0a5b0,0x15176,0x052b0,0x0a930,

                                           0x07954,0x06aa0,0x0ad50,0x05b52,0x04b60,0x0a6e6,0x0a4e0,0x0d260,0x0ea65,0x0d530,

                                           0x05aa0,0x076a3,0x096d0,0x04bd7,0x04ad0,0x0a4d0,0x1d0b6,0x0d250,0x0d520,0x0dd45,

                                           0x0b5a0,0x056d0,0x055b2,0x049b0,0x0a577,0x0a4b0,0x0aa50,0x1b255,0x06d20,0x0ada0,

                                           0x14b63

                                         };

        private static string[] Animals = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        private static string[] dayStr1 = { "日", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        private static string[] dayStr2 = { "初", "十", "廿", "卅", "□" };

        private static string[] chinaMonthName = { "正月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "冬月", "腊月" };

        private static string[] Gan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        private static string[] Zhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        //  private static string[] solarTerm = { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };

        private static int[] sTermInfo = { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };

        private static string[] solarTermName ={
            "小寒", "大寒", "立春", "雨水","惊蛰", "春分", "清明", "谷雨","立夏", "小满", "芒种", "夏至",
            "小暑", "大暑", "立秋", "处暑","白露", "秋分", "寒露", "霜降","立冬", "小雪", "大雪", "冬至"};


        #endregion

        #region 构造函数

        public ChinaCalendar()
        { InitializeValue(); }

        public ChinaCalendar(DateTime date)
        {
            dtvalue = Convert.ToDateTime(date.ToShortDateString());
            InitializeValue();
        }

        #endregion

        private void InitializeValue()
        {
            TimeSpan timeSpan = dtvalue - baseDate;
            int sumdays = Convert.ToInt32(timeSpan.TotalDays);   //86400000=1000*24*60*60
            int tempdays = 0;

            //计算农历年
            for (chinaYear = 1900; chinaYear < 2050 && sumdays > 0; chinaYear++)
            {
                tempdays = ChinaYearDays(chinaYear);
                sumdays -= tempdays;
            }

            if (sumdays < 0)
            {
                sumdays += tempdays;
                chinaYear--;
            }

            //计算闰月
            doubleMonth = DoubleMonth(chinaYear);
            isLeap = false;

            //计算农历月
            for (chinaMonth = 1; chinaMonth < 13 && sumdays > 0; chinaMonth++)
            {
                //闰月
                if (doubleMonth > 0 && chinaMonth == (doubleMonth + 1) && isLeap == false)
                {
                    --chinaMonth;
                    isLeap = true;
                    tempdays = DoubleMonthDays(chinaYear);
                }
                else
                {
                    tempdays = MonthDays(chinaYear, chinaMonth);
                }

                //解除闰月
                if (isLeap == true && chinaMonth == (doubleMonth + 1))
                {
                    isLeap = false;
                }
                sumdays -= tempdays;
            }

            //计算农历日
            if (sumdays == 0 && doubleMonth > 0 && chinaMonth == doubleMonth + 1)
            {
                if (isLeap)
                {
                    isLeap = false;
                }
                else
                {
                    isLeap = true;
                    --chinaMonth;
                }
            }

            if (sumdays < 0)
            {
                sumdays += tempdays;
                --chinaMonth;
            }

            chinaDay = sumdays + 1;

            //计算节气
            ComputeSolarTerm();
        }

        ///<summary>
        ///返回农历年的总天数
        ///</summary>
        ///<param name="year">农历年</param>
        ///<returns></returns>
        private int ChinaYearDays(int year)
        {
            int i, sum = 348;
            for (i = 0x8000; i > 0x8; i >>= 1)
            {
                sum += ((ChinaCalendarInfo[year - 1900] & i) != 0) ? 1 : 0;
            }
            return (sum + DoubleMonthDays(year));
        }

        ///<summary>
        ///返回农历年闰月月份1-12 , 没闰返回0
        ///</summary>
        ///<param name="year">农历年</param>
        ///<returns></returns>
        public static int DoubleMonth(int year)
        {
            return (ChinaCalendarInfo[year - 1900] & 0xf);
        }

        ///<summary>
        ///返回农历年闰月的天数
        ///</summary>
        ///<param name="year">农历年</param>
        ///<returns></returns>
        private int DoubleMonthDays(int year)
        {
            if (DoubleMonth(year) != 0)
                return (((ChinaCalendarInfo[year - 1900] & 0x10000) != 0) ? 30 : 29);
            else
                return (0);
        }

        ///</summary>
        ///返回农历年月份的总天数
        ///</summary>
        ///<param name="year">农历年</param>
        ///<param name="month">农历月</param>
        ///<returns></returns>
        private int MonthDays(int year, int month)
        {
            return (((ChinaCalendarInfo[year - 1900] & (0x10000 >> month)) != 0) ? 30 : 29);
        }

        //计算属相（生肖）
        private string Animal()
        {
            return Animals[(chinaYear - 4) % 60 % 12];
        }

        //生成农历年字符串
        public string ChinaYear()
        {
            return (Gan[(chinaYear - 4) % 60 % 10] + Zhi[(chinaYear - 4) % 60 % 12] + "年");
        }

        //生成农历月字符串
        private string ChinaMonth()
        {
            if (isLeap == true)
            {
                return "闰" + chinaMonthName[chinaMonth - 1];
            }
            else
            {
                return chinaMonthName[chinaMonth - 1];
            }
        }

        //生成农历日字符串
        private string ChinaDay()
        {
            string s;
            switch (chinaDay)
            {
                case 10:
                    s = "初十";
                    break;
                case 20:
                    s = "二十";
                    break;
                case 30:
                    s = "三十";
                    break;
                default:
                    s = dayStr2[chinaDay / 10];
                    s += dayStr1[chinaDay % 10];
                    break;
            }
            return (s);
        }

        #region 节气

        public struct SolarTerm
        {
            /// <summary>
            /// 节气的名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 节气的时间
            /// </summary>
            public DateTime DateTime;
        }

        private SolarTerm[] solarTerm = new SolarTerm[2];

        /// <summary>
        /// 返回指定日期的月份两个节气的名称及时间的SolarTerm数组
        /// </summary>
        public SolarTerm[] GetSolarTerm
        {
            get
            {
                return solarTerm;
            }
        }

        /// <summary>
        /// 返回指定日期的节气名,没有节气名则返回空字符串
        /// </summary>
        public string GetTermName
        {
            get
            {
                foreach (SolarTerm sterm in solarTerm)
                    if (dtvalue.Day == sterm.DateTime.Day)
                    {
                        return sterm.Name;
                    }

                return "";
            }
        }

        // 计算节气
        private void ComputeSolarTerm()
        {
            int year = dtvalue.Year;
            int month = dtvalue.Month;

            for (int n = month * 2 - 1; n <= month * 2; n++)
            {
                double Termdays = Term(year, n, true);
                double mdays = AntiDayDifference(year, Math.Floor(Termdays));
                double sm1 = Math.Floor(mdays / 100);
                int hour = (int)Math.Floor((double)Tail(Termdays) * 24);
                int minute = (int)Math.Floor((double)(Tail(Termdays) * 24 - hour) * 60);
                int tMonth = (int)Math.Ceiling((double)n / 2);
                int day = (int)mdays % 100;

                solarTerm[n - month * 2 + 1].Name = solarTermName[n - 1];
                solarTerm[n - month * 2 + 1].DateTime = new DateTime(year, tMonth, day, hour, minute, 0);
            }
        }


        //返回y年第n个节气（如小寒为1）的日差天数值（pd取值真假，分别表示平气和定气）
        private double Term(int y, int n, bool pd)
        {
            //儒略日
            double juD = y * (365.2423112 - 6.4e-14 * (y - 100) * (y - 100) - 3.047e-8 * (y - 100)) + 15.218427 * n + 1721050.71301;

            //角度
            double tht = 3e-4 * y - 0.372781384 - 0.2617913325 * n;

            //年差实均数
            double yrD = (1.945 * Math.Sin(tht) - 0.01206 * Math.Sin(2 * tht)) * (1.048994 - 2.583e-5 * y);

            //朔差实均数
            double shuoD = -18e-4 * Math.Sin(2.313908653 * y - 0.439822951 - 3.0443 * n);

            double vs = (pd) ? (juD + yrD + shuoD - EquivalentStandardDay(y, 1, 0) - 1721425) : (juD - EquivalentStandardDay(y, 1, 0) - 1721425);
            return vs;
        }


        // 返回阳历y年日差天数为x时所对应的月日数（如y=2000，x=274时，返回1001(表示10月1日，即返回100*m+d)）
        private double AntiDayDifference(int y, double x)
        {
            int m = 1;
            for (int j = 1; j <= 12; j++)
            {
                int mL = DayDifference(y, j + 1, 1) - DayDifference(y, j, 1);
                if (x <= mL || j == 12)
                {
                    m = j;
                    break;
                }
                else
                    x -= mL;
            }
            return 100 * m + x;
        }


        // 返回x的小数尾数，若x为负值，则是1-小数尾数
        private double Tail(double x)
        {
            return x - Math.Floor(x);
        }


        // 返回等效标准天数（y年m月d日相应历种的1年1月1日的等效(即对Gregorian历与Julian历是统一的)天数）
        private double EquivalentStandardDay(int y, int m, int d)
        {
            //Julian的等效标准天数
            double v = (y - 1) * 365 + Math.Floor((double)((y - 1) / 4)) + DayDifference(y, m, d) - 2;

            if (y > 1582)
            {//Gregorian的等效标准天数
                v += -Math.Floor((double)((y - 1) / 100)) + Math.Floor((double)((y - 1) / 400)) + 2;
            }
            return v;
        }


        // 返回阳历y年m月d日的日差天数（在y年年内所走过的天数，如2000年3月1日为61）
        private int DayDifference(int y, int m, int d)
        {
            int ifG = IfGregorian(y, m, d, 1);
            int[] monL = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (ifG == 1)
                if ((y % 100 != 0 && y % 4 == 0) || (y % 400 == 0))
                    monL[2] += 1;
                else
                    if (y % 4 == 0)
                    monL[2] += 1;
            int v = 0;
            for (int i = 0; i <= m - 1; i++)
            {
                v += monL[i];
            }
            v += d;
            if (y == 1582)
            {
                if (ifG == 1)
                    v -= 10;
                if (ifG == -1)
                    v = 0;  //infinity 
            }
            return v;
        }


        // 判断y年m月(1,2,..,12,下同)d日是Gregorian历还是Julian历
        //（opt=1,2,3分别表示标准日历,Gregorge历和Julian历）,是则返回1，是Julian历则返回0，
        // 若是Gregorge历所删去的那10天则返回-1
        private int IfGregorian(int y, int m, int d, int opt)
        {
            if (opt == 1)
            {
                if (y > 1582 || (y == 1582 && m > 10) || (y == 1582 && m == 10 && d > 14))
                    return (1);     //Gregorian
                else
                    if (y == 1582 && m == 10 && d >= 5 && d <= 14)
                    return (-1);  //空
                else
                    return (0);  //Julian
            }

            if (opt == 2)
                return (1);     //Gregorian
            if (opt == 3)
                return (0);     //Julian
            return (-1);
        }

        #endregion 节气
    }

    /// <summary>
    /// 星期枚举
    /// </summary>
    public enum WeekDay
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }
}
