using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitJoy();
        }

        //REF http://www.cnblogs.com/kingthy/archive/2009/03/25/1421838.html
        //REFVhttps://yal.cc/c-sharp-joystick-tracking-via-winmm-dll/
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYCAPS
        {
            public ushort wMid;
            public ushort wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public int wXmin;
            public int wXmax;
            public int wYmin;
            public int wYmax;
            public int wZmin;
            public int wZmax;
            public int wNumButtons;
            public int wPeriodMin;
            public int wPeriodMax;
            public int wRmin;
            public int wRmax;
            public int wUmin;
            public int wUmax;
            public int wVmin;
            public int wVmax;
            public int wCaps;
            public int wMaxAxes;
            public int wNumAxes;
            public int wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szOEMVxD;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFOEX
        {
            public Int32 dwSize; // Size, in bytes, of this structure.
            public Int32 dwFlags; // Flags indicating the valid information returned in this structure.
            public Int32 dwXpos; // Current X-coordinate.
            public Int32 dwYpos; // Current Y-coordinate.
            public Int32 dwZpos; // Current Z-coordinate.
            public Int32 dwRpos; // Current position of the rudder or fourth joystick axis.
            public Int32 dwUpos; // Current fifth axis position.
            public Int32 dwVpos; // Current sixth axis position.
            public Int32 dwButtons; // Current state of the 32 joystick buttons (bits)
            public Int32 dwButtonNumber; // Current button number that is pressed.
            public Int32 dwPOV; // Current position of the point-of-view control (0..35,900, deg*100)
            public Int32 dwReserved1; // Reserved; do not use.
            public Int32 dwReserved2; // Reserved; do not use.
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFO
        {
            public Int32 wXpos; // Current X-coordinate.
            public Int32 wYpos; // Current Y-coordinate.
            public Int32 wZpos; // Current Z-coordinate.
            public Int32 wButtons; // Current state of joystick buttons.
        }
        [DllImport("winmm.dll")]
        public static extern Int32 joyGetPos(Int32 uJoyID, ref JOYINFO pji);
        [DllImport("winmm.dll")]
        public static extern Int32 joyGetPosEx(Int32 uJoyID, ref JOYINFOEX pji);
        [DllImport("winmm.dll")]
        public static extern int joyGetDevCaps(int uJoyID, ref JOYCAPS pjc, int cbjc);

        public struct DeviceJoyInfo
        {
            public bool JoyEx;
            public int ButtonCount;
            public int ID;
            public int Button_old;
            public int Way_X_old;
            public int Way_Y_old;
        }

        public struct joystickEvent
        {
            public int event_type; //0:方向鍵觸發 1:一般按鈕觸發
            public int joystick_id;//發生於哪個遊戲手把

            public int button_id;//如果是一般按鈕觸發,發生在哪顆按鈕
            public int button_event;//0:鬆開 1:壓下

            public int way_type; //0:x方向鍵盤 1:y方向鍵盤
            public int way_value;

        }

        List<DeviceJoyInfo> joyinfo_list = new List<DeviceJoyInfo>();
        JOYCAPS joycap = new JOYCAPS();
        JOYINFO js = new JOYINFO();
        JOYINFOEX jsx = new JOYINFOEX();
        int JOYCAPS_size;
        int PeriodMin = 0;
        unsafe public void InitJoy()
        {
            Stopwatch st = new Stopwatch();
            st.Restart();
            JOYCAPS_size = Marshal.SizeOf(typeof(JOYCAPS));

            for (int i = 0; i < 256; i++)
            {
                if (joyGetDevCaps(i, ref joycap, JOYCAPS_size) == 0)
                {
                    DeviceJoyInfo info = new DeviceJoyInfo();

                    //set id
                    info.ID = i;

                    //check joyex
                    if (joyGetPosEx(i, ref jsx) == 0)
                    {
                        info.JoyEx = true;
                        info.Way_X_old = jsx.dwXpos;
                        info.Way_Y_old = jsx.dwYpos;
                    }
                    else if (joyGetPos(i, ref js) == 0)
                    {
                        info.JoyEx = false;
                        info.Way_X_old = js.wXpos;
                        info.Way_Y_old = js.wYpos;
                    }
                    else continue; //裝置功能失效

                    //set button count
                    info.ButtonCount = joycap.wNumButtons;

                    info.Button_old = 0;

                    if (joycap.wPeriodMin > PeriodMin)
                        PeriodMin = joycap.wPeriodMin;

                    joyinfo_list.Add(info);
                }
            }
            //取出所有目前連線遊戲手把中最慢的PeriodMin然後+5ms
            PeriodMin += 5;
            new Thread(polling_listener).Start();
            st.Stop();
            Console.WriteLine("init joypad infor : " + st.ElapsedMilliseconds + " ms");
        }

        List<joystickEvent> joy_event_captur()
        {
            List<joystickEvent> event_list = new List<joystickEvent>();
            for (int i_button = 0; i_button < joyinfo_list.Count(); i_button++)
            {
                DeviceJoyInfo button_inf = joyinfo_list[i_button];
                int button_id = button_inf.ID;
                int button_count = button_inf.ButtonCount;

                int button_now, X_now, Y_now;
                if (button_inf.JoyEx == false)
                {
                    joyGetPos(button_id, ref js);
                    button_now = js.wButtons;
                    X_now = js.wXpos;
                    Y_now = js.wYpos;
                }
                else
                {
                    joyGetPosEx(button_id, ref jsx);
                    button_now = jsx.dwButtons;
                    X_now = jsx.dwXpos;
                    Y_now = jsx.dwYpos;
                }

                int button_old = button_inf.Button_old;
                int X_old = button_inf.Way_X_old;
                int Y_old = button_inf.Way_Y_old;

                button_inf.Button_old = button_now;
                button_inf.Way_X_old = X_now;
                button_inf.Way_Y_old = Y_now;

                joyinfo_list[i_button] = button_inf;
                if (button_old != button_now || button_now != 0)
                {
                    for (int i = 0; i < button_count; i++)
                    {
                        if ((button_now & 1) != 0)
                        {
                            joystickEvent event_item = new joystickEvent();
                            event_item.event_type = 1;
                            event_item.joystick_id = button_inf.ID;
                            event_item.button_id = i + 1;
                            event_item.button_event = 1;
                            event_list.Add(event_item);
                        }
                        else
                        {
                            if ((button_now & 1) != (button_old & 1))
                            {
                                joystickEvent event_item = new joystickEvent();
                                event_item.event_type = 1;
                                event_item.joystick_id = button_inf.ID;
                                event_item.button_id = i + 1;
                                event_item.button_event = 0;
                                event_list.Add(event_item);
                            }
                        }
                        button_now >>= 1;
                        button_old >>= 1;
                    }
                }

                if (X_old != X_now || (X_now != 32767 && X_now != 32511 && X_now != 32254))
                {
                    if ((X_now != 32767 && X_now != 32511))
                    {
                        joystickEvent event_item = new joystickEvent();
                        event_item.event_type = 0;
                        event_item.joystick_id = button_inf.ID;
                        event_item.way_type = 0;
                        event_item.way_value = X_now;
                        event_list.Add(event_item);

                    }
                    else
                    {
                        joystickEvent event_item = new joystickEvent();
                        event_item.event_type = 0;
                        event_item.joystick_id = button_inf.ID;
                        event_item.way_type = 0;
                        event_item.way_value = X_now;
                        event_list.Add(event_item);
                    }
                }

                if (Y_old != Y_now || (Y_now != 32767 && Y_now != 32511 && Y_now != 32254))
                {
                    if ((Y_now != 32767 && Y_now != 32511))
                    {
                        joystickEvent event_item = new joystickEvent();
                        event_item.event_type = 0;
                        event_item.joystick_id = button_inf.ID;
                        event_item.way_type = 1;
                        event_item.way_value = Y_now;
                        event_list.Add(event_item);
                    }
                    else
                    {
                        joystickEvent event_item = new joystickEvent();
                        event_item.event_type = 0;
                        event_item.joystick_id = button_inf.ID;
                        event_item.way_type = 1;
                        event_item.way_value = Y_now;
                        event_list.Add(event_item);
                    }
                }

            }
            return event_list;
        }

        bool app_running = true;
        void polling_listener()
        {
            while (app_running)
            {
                Thread.Sleep(PeriodMin);
                List<joystickEvent> event_list = joy_event_captur();

                foreach (joystickEvent joy_event in event_list)
                {

                    if (joy_event.event_type == 0) //方向鍵觸發
                    {
                        if (joy_event.way_type == 0) //x
                        {
                            Console.WriteLine("裝置 " + joy_event.joystick_id + " ,X " + joy_event.way_value);
                        }
                        else//y
                        {
                            Console.WriteLine("裝置 " + joy_event.joystick_id + " ,Y " + joy_event.way_value);
                        }

                    }
                    else //一般按鈕觸發
                    {
                        if (joy_event.button_event == 0)
                        {
                            Console.WriteLine("裝置 " + joy_event.joystick_id + " ,按鈕 " + joy_event.button_id + " 放開");
                        }
                        else
                        {
                            Console.WriteLine("裝置 " + joy_event.joystick_id + " ,按鈕 " + joy_event.button_id + " 壓下");
                        }
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            app_running = false;
        }
    }
}
