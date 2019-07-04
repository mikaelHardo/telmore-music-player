using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MusicPlayerNew
{
    public enum KeyModifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        WinKey = 8
    }

    public class Hook : Form
    {
        private readonly IntPtr _handle;
        private readonly Dictionary<int, Action> _actions = new Dictionary<int, Action>();
        private int _nextId = 0;

        public Hook()
        {
            _handle = this.Handle;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.

                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                var action = _actions[id];
                action();
            }
        }

        protected override void Dispose(bool disposing)
        {
            for (var i = 0; i < _nextId; i++)
            {
                UnregisterHotKey(_handle, i);       // Unregister hotkey with id 0 before closing the form. You might want to call this more than once with different id values if you are planning to register more than one hotkey.
            }
        }

        public void Register(KeyModifier modifier, int keyCode, Action callBack)
        {
            var nextId = _nextId++;

            _actions[nextId] = callBack;

            RegisterHotKey(_handle, nextId, (int)modifier, keyCode);
        }
    }
}
