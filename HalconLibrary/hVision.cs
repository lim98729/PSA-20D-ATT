using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconDotNet;
using DefineLibrary;

namespace HalconLibrary
{
    public class hVision
    {
        public static halcon cam1 = new halcon();
        public static halcon cam2 = new halcon();
        public static halcon cam3 = new halcon();
        public static halcon cam4 = new halcon();
        
        #region closeAllFramegrabbers
        public static bool closeAllFramegrabbers()
        {
            try
            {
                HOperatorSet.CloseAllFramegrabbers();
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region clearAllModels
        public static bool clearAllModels()
        {
            try
            {
                HOperatorSet.ClearAllNccModels();
                HOperatorSet.ClearAllShapeModels();
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        #region live
        public static bool isLive
        {
            get
            {
                if (isLive1 || isLive2 || isLive3 || isFind4) return true;
                return false;
            }
        }
        public static bool isLive1, isLive2, isLive3, isLive4;
        public static bool live1Stop, live2Stop, live3Stop, live4Stop;
        public static bool live1()
        {
            if (!cam1.isActivate) return false;
            Thread th = new Thread(live1Thread);
            th.Name = "live1Thread";
            th.Priority = ThreadPriority.Lowest;
            th.IsBackground = true;
            th.Start();
            return true;
        }
        public static bool live2()
        {
            if (!cam2.isActivate) return false;
            Thread th = new Thread(live2Thread);
            th.Name = "live2Thread";
            th.Priority = ThreadPriority.Lowest;
            th.IsBackground = true;
            th.Start();
            return true;
        }
        public static bool live3()
        {
            if (!cam3.isActivate) return false;
            Thread th = new Thread(live3Thread);
            th.Name = "live3Thread";
            th.Priority = ThreadPriority.Lowest;
            th.IsBackground = true;
            th.Start();
            return true;
        }
        public static bool live4()
        {
            if (!cam4.isActivate) return false;
            Thread th = new Thread(live4Thread);
            th.Name = "live4Thread";
            th.Priority = ThreadPriority.Lowest;
            th.IsBackground = true;
            th.Start();
            return true;
        }
      
        static void live1Thread()
        {
            isLive1 = true; live1Stop = false;
            live1Req = true;
            while (true)
            {
                live1Control();
                if (live1Req == false) break;
            }
            isLive1 = false;
        }
        static void live2Thread()
        {
            isLive2 = true; live2Stop = false;
            live2Req = true;
            while (true)
            {
                live2Control();
                if (live2Req == false) break;
            }
            isLive2 = false;
        }
        static void live3Thread()
        {
            isLive3 = true; live3Stop = false;
            live3Req = true;
            while (true)
            {
                live3Control();
                if (live3Req == false) break;
            }
            isLive3 = false;
        }
        static void live4Thread()
        {
            isLive4 = true; live4Stop = false;
            live4Req = true;
            while (true)
            {
                live4Control();
                if (live4Req == false) break;
            }
            isLive4 = false;
        }

        //static double live1Temp, live2Temp, live3Temp, live4Temp;
        static bool live1Req, live2Req, live3Req, live4Req;
        static int live1Sqc, live2Sqc, live3Sqc, live4Sqc;
        //static double live1Fps, live2Fps, live3Fps, live4Fps;
        static halcon_timer live1Dwell = new halcon_timer();
        static halcon_timer live2Dwell = new halcon_timer();
        static halcon_timer live3Dwell = new halcon_timer();
        static halcon_timer live4Dwell = new halcon_timer();
        static void live1Control()
        {
            if (!live1Req) return;

            switch (live1Sqc)
            {
                case 0:
                    live1Sqc++; break;
                case 1:
                    if (live1Stop) { live1Sqc++; break; }
                    //live1Dwell.Reset();
                    //if (cam1.refresh_req) break;
                    cam1.still();
                    //live1Temp = live1Dwell.Elapsed;
                    //live1Fps = Math.Round(1000 / live1Temp, 2);
                    //cam1.messageStatus("Live : " + live1Fps.ToString() + "fps");
                    //cam1.messageResult("Live : " + live1Fps.ToString() + "fps");
                    //live1Sqc--;
                    break;
                case 2:
                    live1Sqc = 0;
                    live1Req = false;
                    break;
            }
        }
        static void live2Control()
        {
            if (!live2Req) return;

            switch (live2Sqc)
            {
                case 0:
                    live2Sqc++; break;
                case 1:
                    if (live2Stop) { live2Sqc++; break; }
                    //live2Dwell.Reset();
                    cam2.still();
                    //live2Temp = live2Dwell.Elapsed;
                    //live2Fps = Math.Round(1000 / live2Temp, 2);
                    //cam2.messageResult("Live : " + live2Fps.ToString() + "fps");
                    break;
                case 2:
                    live2Sqc = 0;
                    live2Req = false;
                    break;
            }
        }
        static void live3Control()
        {
            if (!live3Req) return;

            switch (live3Sqc)
            {
                case 0:
                    live3Sqc++; break;
                case 1:
                    if (live3Stop) { live3Sqc++; break; }
                    //live3Dwell.Reset();
                    cam3.still();
                    //live3Temp = live3Dwell.Elapsed;
                    //live3Fps = Math.Round(1000 / live3Temp, 2);
                    //cam3.messageResult("Live : " + live3Fps.ToString() + "fps");
                    break;
                case 2:
                    live3Sqc = 0;
                    live3Req = false;
                    break;
            }
        }
        static void live4Control()
        {
            if (!live4Req) return;

            switch (live4Sqc)
            {
                case 0:
                    live4Sqc++; break;
                case 1:
                    if (live4Stop) { live4Sqc++; break; }
                    //live4Dwell.Reset();
                    cam4.still();
                    //live4Temp = live4Dwell.Elapsed;
                    //live4Fps = Math.Round(1000 / live4Temp, 2);
                    //cam4.messageResult("Live : " + live4Fps.ToString() + "fps");
                    break;
                case 2:
                    live4Sqc = 0;
                    live4Req = false;
                    break;
            }
        }
        #endregion

        #region find
        public static bool isFind
        {
            get
            {
                if (isFind1 || isFind2 || isFind3 || isFind4) return true;
                return false;
            }
        }
        public static bool isFind1, isFind2, isFind3, isFind4;
        public static bool find1Stop, find2Stop, find3Stop, find4Stop;

        public static bool find1()
        {
            try
            {
                if (!cam1.isActivate) return false;
                Thread th = new Thread(find1Thread);
                th.Priority = ThreadPriority.BelowNormal;
                th.Name = "find1Thread";
                th.Start();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find1 Exception : " + ex.ToString());
                return false;
            }
        }
        public static bool find2()
        {
            try
            {
                if (!cam2.isActivate) return false;
                Thread th = new Thread(find2Thread);
                th.Priority = ThreadPriority.BelowNormal;
                th.Name = "find2Thread";
                th.Start();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find2 Exception : " + ex.ToString());
                return false;
            }
        }
        public static bool find3()
        {
            try
            {
                if (!cam3.isActivate) return false;
                Thread th = new Thread(find3Thread);
                th.Priority = ThreadPriority.BelowNormal;
                th.Name = "find3Thread";
                th.Start();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find3 Exception : " + ex.ToString());
                return false;
            }
        }
        public static bool find4()
        {
            try
            {
                if (!cam4.isActivate) return false;
                Thread th = new Thread(find4Thread);
                th.Priority = ThreadPriority.BelowNormal;
                th.Name = "find4Thread";
                th.Start();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find4 Exception : " + ex.ToString());
                return false;
            }
        }

        static void find1Thread()
        {
            try
            {
                isFind1 = true; find1Stop = false;
                find1Req = true;
                while (true)
                {
                    Thread.Sleep(1); Application.DoEvents();
                    find1Control();
                    if (find1Req == false) break;
                }
                isFind1 = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find1Thread Exception : " + ex.ToString());
            }
        }
        static void find2Thread()
        {
            try
            {
                isFind2 = true; find2Stop = false;
                find2Req = true;
                while (true)
                {
                    Thread.Sleep(1); Application.DoEvents();
                    find2Control();
                    if (find2Req == false) break;
                }
                isFind2 = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find2Thread Exception : " + ex.ToString());
            }
        }
        static void find3Thread()
        {
            try
            {
                isFind3 = true; find3Stop = false;
                find3Req = true;
                while (true)
                {
                    Thread.Sleep(1); Application.DoEvents();
                    find3Control();
                    if (find3Req == false) break;
                }
                isFind3 = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find3Thread Exception : " + ex.ToString());
            }
        }
        static void find4Thread()
        {
            try
            {
                isFind4 = true; find4Stop = false;
                find4Req = true;
                while (true)
                {
                    Thread.Sleep(1); Application.DoEvents();
                    find4Control();
                    if (find4Req == false) break;
                }
                isFind4 = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("find4Thread Exception : " + ex.ToString());
            }
        }

        static bool find1Req, find2Req, find3Req, find4Req;
        static int find1Sqc, find2Sqc, find3Sqc, find4Sqc;
        static halcon_timer find1Dwell = new halcon_timer();
        static halcon_timer find2Dwell = new halcon_timer();
        static halcon_timer find3Dwell = new halcon_timer();
        static halcon_timer find4Dwell = new halcon_timer();
        static int find1Index, find2Index, find3Index, find4Index;
        static int find1ModelNum, find2ModelNum, find3ModelNum, find4ModelNum;
        static RetValue find1Ret,find2Ret,find3Ret,find4Ret;
        static void find1Control()
        {
            if (!find1Req) return;

            switch (find1Sqc)
            {
                case 0:
                    if (find1Stop) { find1Sqc = 100; break; }
                    find1ModelNum = -1;
                    find1Dwell.Reset();
                    find1Sqc++; break;
                case 1:
                    if (find1Dwell.Elapsed < 10) break;
                    if (cam1.refresh_req) break;
                    if (cam1.grab() == false) { find1Sqc = 100; break; }
                    find1Index = 0;
                    find1Sqc = 10; break;

                case 10:
                    if (cam1.model[find1Index].isCreate == "true") { find1ModelNum = find1Index; find1Index++; find1Sqc = 20; break; }
                    find1Index++;
                    if (find1Index < cam1.MODEL_MAX_CNT) break;
                    find1Sqc++; break;
                case 11:
                    if (find1ModelNum == -1)
                    {
                        cam1.refresh_reqMode = REFRESH_REQMODE.IMAGE;
                        cam1.refresh_req = true;
                    }
                    find1Sqc = 0; break;

                case 20:
                    cam1.findModel(find1ModelNum, out find1Ret.message, out find1Ret.s);
                    if (find1Ret.message == RetMessage.OK)
                    {
                        cam1.refresh_req = true;
                        cam1.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
                        cam1.refresh_reqModelNumber = find1ModelNum;
                    }
                    else
                    {
                        cam1.refresh_req = true;
                        cam1.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
                        cam1.refresh_errorMessage = find1Ret.s;
                    }
                    find1Sqc++; break;
                case 21:
                    if (cam1.refresh_req) break;
                    find1Dwell.Reset();
                    find1Sqc++; break;
                case 22:
                    if (find1Dwell.Elapsed < 30) break;
                    find1Sqc = 0; break;

                case 100:
                    find1Sqc = 0;
                    find1Req = false;
                    break;
            }
        }

        static void find2Control()
        {
            if (!find2Req) return;

            switch (find2Sqc)
            {
                case 0:
                    if (find2Stop) { find2Sqc = 100; break; }
                    find2ModelNum = -1;
                    find2Dwell.Reset();
                    find2Sqc++; break;
                case 1:
                    if (find2Dwell.Elapsed < 10) break;
                    if (cam2.refresh_req) break;
                    if (cam2.grab() == false) { find2Sqc = 100; break; }
                    find2Index = 0;
                    find2Sqc = 10; break;

                case 10:
                    if (cam2.model[find2Index].isCreate == "true") { find2ModelNum = find2Index; find2Index++; find2Sqc = 20; break; }
                    find2Index++;
                    if (find2Index < cam2.MODEL_MAX_CNT) break;
                    find2Sqc++; break;
                case 11:
                    if (find2ModelNum == -1)
                    {
                        cam2.refresh_reqMode = REFRESH_REQMODE.IMAGE;
                        cam2.refresh_req = true;
                    }
                    find2Sqc = 0; break;

                case 20:
                    cam2.findModel(find2ModelNum, out find2Ret.message, out find2Ret.s);
                    if (find2Ret.message == RetMessage.OK)
                    {
                        cam2.refresh_req = true;
                        cam2.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
                        cam2.refresh_reqModelNumber = find2ModelNum;
                    }
                    else
                    {
                        cam2.refresh_req = true;
                        cam2.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
                        cam2.refresh_errorMessage = find2Ret.s;
                    }
                    find2Sqc++; break;
                case 21:
                    if (cam2.refresh_req) break;
                    find2Dwell.Reset();
                    find2Sqc++; break;
                case 22:
                    if (find2Dwell.Elapsed < 30) break;
                    find2Sqc = 0; break;

                case 100:
                    find2Sqc = 0;
                    find2Req = false;
                    break;
            }
        }

        static void find3Control()
        {
            if (!find3Req) return;

            switch (find3Sqc)
            {
                case 0:
                    if (find3Stop) { find3Sqc = 100; break; }
                    find3ModelNum = -1;
                    find3Dwell.Reset();
                    find3Sqc++; break;
                case 1:
                    if (find3Dwell.Elapsed < 10) break;
                    if (cam3.refresh_req) break;
                    if (cam3.grab() == false) { find3Sqc = 100; break; }
                    find3Index = 0;
                    find3Sqc = 10; break;

                case 10:
                    if (cam3.model[find3Index].isCreate == "true") { find3ModelNum = find3Index; find3Index++; find3Sqc = 20; break; }
                    find3Index++;
                    if (find3Index < cam3.MODEL_MAX_CNT) break;
                    find3Sqc++; break;
                case 11:
                    if (find3ModelNum == -1)
                    {
                        cam3.refresh_reqMode = REFRESH_REQMODE.IMAGE;
                        cam3.refresh_req = true;
                    }
                    find3Sqc = 0; break;

                case 20:
                    cam3.findModel(find3ModelNum, out find3Ret.message, out find3Ret.s);
                    if (find3Ret.message == RetMessage.OK)
                    {
                        cam3.refresh_req = true;
                        cam3.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
                        cam3.refresh_reqModelNumber = find3ModelNum;
                    }
                    else
                    {
                        cam3.refresh_req = true;
                        cam3.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
                        cam3.refresh_errorMessage = find3Ret.s;
                    }
                    find3Sqc++; break;
                case 21:
                    if (cam3.refresh_req) break;
                    find3Dwell.Reset();
                    find3Sqc++; break;
                case 22:
                    if (find3Dwell.Elapsed < 30) break;
                    find3Sqc = 0; break;

                case 100:
                    find3Sqc = 0;
                    find3Req = false;
                    break;
            }
        }

        static void find4Control()
        {
            if (!find4Req) return;

            switch (find4Sqc)
            {
                case 0:
                    if (find4Stop) { find4Sqc = 100; break; }
                    find4ModelNum = -1;
                    find4Dwell.Reset();
                    find4Sqc++; break;
                case 1:
                    if (find4Dwell.Elapsed < 10) break;
                    if (cam4.refresh_req) break;
                    if (cam4.grab() == false) { find4Sqc = 100; break; }
                    find4Index = 0;
                    find4Sqc = 10; break;

                case 10:
                    if (cam4.model[find4Index].isCreate == "true") { find4ModelNum = find4Index; find4Index++; find4Sqc = 20; break; }
                    find4Index++;
                    if (find4Index < cam4.MODEL_MAX_CNT) break;
                    find4Sqc++; break;
                case 11:
                    if (find4ModelNum == -1)
                    {
                        cam4.refresh_reqMode = REFRESH_REQMODE.IMAGE;
                        cam4.refresh_req = true;
                    }
                    find4Sqc = 0; break;

                case 20:
                    cam4.findModel(find4ModelNum, out find4Ret.message, out find4Ret.s);
                    if (find4Ret.message == RetMessage.OK)
                    {
                        cam4.refresh_req = true;
                        cam4.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
                        cam4.refresh_reqModelNumber = find4ModelNum;
                    }
                    else
                    {
                        cam4.refresh_req = true;
                        cam4.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
                        cam4.refresh_errorMessage = find4Ret.s;
                    }
                    find4Sqc++; break;
                case 21:
                    if (cam4.refresh_req) break;
                    find4Dwell.Reset();
                    find4Sqc++; break;
                case 22:
                    if (find4Dwell.Elapsed < 30) break;
                    find4Sqc = 0; break;

                case 100:
                    find4Sqc = 0;
                    find4Req = false;
                    break;
            }
        }

     
        #endregion

        public static void cancelDraw()
        {
            HalconAPI.CancelDraw();
        }

    }


}
