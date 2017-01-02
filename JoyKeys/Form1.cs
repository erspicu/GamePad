using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace JoyKeys
{
    public partial class Form1 : Form
    {
        private Core.Joystick joystick;
        public Form1()
        {
            InitializeComponent();


            Console.WriteLine("b  " + this.Handle.ToString());
        }
        protected override void OnLoad(EventArgs e)
        {

            Console.WriteLine("a  "  + this.Handle.ToString() );

            base.OnLoad(e);
            joystick = new JoyKeys.Core.Joystick();
            joystick.Click += new EventHandler<JoyKeys.Core.JoystickEventArgs>(joystick_Click);
            joystick.Register(this.Handle, Core.API.JOYSTICKID1);
           // joystick.Register(this.Handle, Core.API.JOYSTICKID2);



            Console.WriteLine("a---  " + base.Handle.ToString());
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            Console.WriteLine("b");

            joystick.UnRegister(Core.API.JOYSTICKID1);
            joystick.UnRegister(Core.API.JOYSTICKID2);
            base.OnClosing(e);
        }
        void joystick_Click(object sender, JoyKeys.Core.JoystickEventArgs e)
        {


            Console.WriteLine("111");

            if (e.JoystickId == Core.API.JOYSTICKID1)
            {
                this.Text = "1号手柄";
            }
            else if (e.JoystickId == Core.API.JOYSTICKID2)
            {
                this.Text = "2号手柄";
            }

            int x = 1;
            int y = 1;
            if ((e.Buttons & JoyKeys.Core.JoystickButtons.UP) == JoyKeys.Core.JoystickButtons.UP) y--;
            if ((e.Buttons & JoyKeys.Core.JoystickButtons.Down) == JoyKeys.Core.JoystickButtons.Down) y++;
            if ((e.Buttons & JoyKeys.Core.JoystickButtons.Left) == JoyKeys.Core.JoystickButtons.Left) x--;
            if ((e.Buttons & JoyKeys.Core.JoystickButtons.Right) == JoyKeys.Core.JoystickButtons.Right) x++;

            if (x == 0 && y == 0) this.label1.TextAlign = ContentAlignment.TopLeft;
            if (x == 1 && y == 0) this.label1.TextAlign = ContentAlignment.TopCenter;
            if (x == 2 && y == 0) this.label1.TextAlign = ContentAlignment.TopRight;

            if (x == 0 && y == 1) this.label1.TextAlign = ContentAlignment.MiddleLeft;
            if (x == 1 && y == 1) this.label1.TextAlign = ContentAlignment.MiddleCenter;
            if (x == 2 && y == 1) this.label1.TextAlign = ContentAlignment.MiddleRight;

            if (x == 0 && y == 2) this.label1.TextAlign = ContentAlignment.BottomLeft;
            if (x == 1 && y == 2) this.label1.TextAlign = ContentAlignment.BottomCenter;
            if (x == 2 && y == 2) this.label1.TextAlign = ContentAlignment.BottomRight;
            this.label1.Text = "+";

            this.label2.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B1) == JoyKeys.Core.JoystickButtons.B1) ? Color.Red : SystemColors.Control;
            this.label3.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B2) == JoyKeys.Core.JoystickButtons.B2) ? Color.Red : SystemColors.Control;
            this.label4.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B3) == JoyKeys.Core.JoystickButtons.B3) ? Color.Red : SystemColors.Control;
            this.label5.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B4) == JoyKeys.Core.JoystickButtons.B4) ? Color.Red : SystemColors.Control;
            this.label6.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B5) == JoyKeys.Core.JoystickButtons.B5) ? Color.Red : SystemColors.Control;
            this.label7.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B6) == JoyKeys.Core.JoystickButtons.B6) ? Color.Red : SystemColors.Control;
            this.label8.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B7) == JoyKeys.Core.JoystickButtons.B7) ? Color.Red : SystemColors.Control;
            this.label9.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B8) == JoyKeys.Core.JoystickButtons.B8) ? Color.Red : SystemColors.Control;
            this.label10.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B9) == JoyKeys.Core.JoystickButtons.B9) ? Color.Red : SystemColors.Control;
            this.label11.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B10) == JoyKeys.Core.JoystickButtons.B10) ? Color.Red : SystemColors.Control;


            this.label12.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B11) == JoyKeys.Core.JoystickButtons.B10) ? Color.Red : SystemColors.Control;
            this.label13.BackColor = ((e.Buttons & JoyKeys.Core.JoystickButtons.B12) == JoyKeys.Core.JoystickButtons.B10) ? Color.Red : SystemColors.Control;

        }
    }
}
