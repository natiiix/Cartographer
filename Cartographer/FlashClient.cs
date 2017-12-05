using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Cartographer
{
    public class FlashClient
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        private class KeyWrapper
        {
            private bool pressed;
            private Action<uint> sendKeyEventToFlash;

            public bool Pressed
            {
                get => pressed;

                set
                {
                    if (value != pressed)
                    {
                        // Update state
                        pressed = value;

                        // Send the new state to the Flash window
                        sendKeyEventToFlash(KeyStateToEvent(pressed));
                    }
                }
            }

            public KeyWrapper(Action<uint> sendKeyEventToFlash)
            {
                pressed = false;
                this.sendKeyEventToFlash = sendKeyEventToFlash;
            }

            private uint KeyStateToEvent(bool state) => state ? WM_KEYDOWN : WM_KEYUP;
        }

        private const double MOVE_THRESHOLD = 0.5;

        private IntPtr flashPtr;
        private KeyWrapper keyW;
        private KeyWrapper keyA;
        private KeyWrapper keyS;
        private KeyWrapper keyD;

        public FlashClient()
        {
            //flashPtr = GetFlashHandle();
            flashPtr = WinApi.GetForegroundWindow();

            keyW = new KeyWrapper(x => SendKeyEventToFlash(Keys.W, x));
            keyA = new KeyWrapper(x => SendKeyEventToFlash(Keys.A, x));
            keyS = new KeyWrapper(x => SendKeyEventToFlash(Keys.S, x));
            keyD = new KeyWrapper(x => SendKeyEventToFlash(Keys.D, x));
        }

        public bool MoveInDirection(double x, double y)
        {
            // Up
            keyW.Pressed = y < -MOVE_THRESHOLD;

            // Left
            keyA.Pressed = x < -MOVE_THRESHOLD;

            // Down
            keyS.Pressed = y > MOVE_THRESHOLD;

            // Right
            keyD.Pressed = x > MOVE_THRESHOLD;

            return Math.Abs(x) > MOVE_THRESHOLD || Math.Abs(y) > MOVE_THRESHOLD;
        }

        public void StopMoving()
        {
            keyW.Pressed = false;
            keyA.Pressed = false;
            keyS.Pressed = false;
            keyD.Pressed = false;
        }

        private void SendKeyEventToFlash(Keys key, uint keyEvent)
        {
            WinApi.PostMessage(flashPtr, keyEvent, (IntPtr)key, IntPtr.Zero);
        }

        //private static bool IsFlashProcess(Process proc) => proc.ProcessName.ToLower().StartsWith("flashplayer");

        //private static IntPtr GetFlashHandle() => Process.GetProcesses().Single(IsFlashProcess).MainWindowHandle;
    }
}