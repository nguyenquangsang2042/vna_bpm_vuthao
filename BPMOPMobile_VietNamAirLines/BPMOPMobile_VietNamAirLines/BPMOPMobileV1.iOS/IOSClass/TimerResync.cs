using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Timers;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using System.Threading.Tasks;

namespace BPMOPMobileV1.iOS.IOSClass
{
    public class TimerResync
    {
        public Timer timerReSync;
        public int _count_reSync;

        public TimerResync()
        {
            if (timerReSync == null)
            {
                timerReSync = new Timer();
                timerReSync.Interval = 1000;
                timerReSync.Elapsed += TimerReSync_Elapsed;
                _count_reSync = CmmVariable.SysConfig.DelaySynsTime;
            }
        }

        private static TimerResync instance;
        public static TimerResync Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimerResync();
                }
                return instance;
            }
        }

        public void startTimerReSync()
        {
            var values = NSUserDefaults.StandardUserDefaults.StringForKey("ActiveUser");
            if (!string.IsNullOrEmpty(values))
            {
                timerReSync.Start();
            }
        }
        private async void TimerReSync_Elapsed(object sender, ElapsedEventArgs e)
        {
            _count_reSync--;
            if (_count_reSync == 0)
            {
                timerReSync.Stop();
                _count_reSync = CmmVariable.SysConfig.DelaySynsTime;

                ProviderBase p_base = new ProviderBase();
                await Task.Run(() =>
                {
                    p_base.UpdateAllDynamicData(true);

                    startTimerReSync();
                });

            }
        }
    }
}