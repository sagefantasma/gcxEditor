using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcxEditorGUI.Object_Controls
{
    internal class SharedUCBehaviors
    {
        public static void UC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender is Control senderControl)
                {
                    senderControl.Cursor = Cursors.Hand;
                }
            }
            //only handle left mouse drag
        }
    }
}
