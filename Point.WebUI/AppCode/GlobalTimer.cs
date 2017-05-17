﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
namespace Point.WebUI
{
    public class GlobalTimer
    {
        private static Timer captureTimer = null;
        private readonly static object catptureLock = new object();

        private static int CatpureTimeSpan
        {
            get
            {
                int catpureTimeSpan = 5 * 60 * 60 * 1000;
                var t = FaceHand.Common.AppSetting.Default.GetItem("TimerExcuteSpan");
                try
                {
                    catpureTimeSpan = Convert.ToInt32(t) * 60 * 1000;
                }
                catch
                {

                }
                return catpureTimeSpan;
            }
        }

        public static void Init()
        {
            FaceHand.Common.Util.SystemLoger.Current.Write(string.Format("【定时器】{0}: 启动...", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

            if (captureTimer == null)
            {
                lock (catptureLock)
                {
                    if (captureTimer == null)
                    {
                        captureTimer = new Timer(new TimerCallback(CaptureTimerCallback), null, CatpureTimeSpan, Timeout.Infinite);
                    }
                }
            }
        }

        public static void CaptureTimerCallback(object sender)
        {
            try
            {
                var cfgs = DAL.Instance.SelectConfigList();
                FaceHand.Common.Util.SystemLoger.Current.Write(string.Format("【定时器】{0}:执行...", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                if (cfgs != null && cfgs.Count() > 0)
                {
                    foreach (var cfg in cfgs)
                    {
                        var maxId = DAL.Instance.SelectMaxRefId(cfg.RefId);
                        DataCaptureHelper.Capture(cfg, maxId);
                    }
                }
            }
            catch (Exception ex)
            {
                FaceHand.Common.Analyze.LogProvider.Current.Write("抓取数据出错:" + ex);
            }
            finally
            {
                captureTimer.Change(CatpureTimeSpan, Timeout.Infinite);
            }
        }
    }
}