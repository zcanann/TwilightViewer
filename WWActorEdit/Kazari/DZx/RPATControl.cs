using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit.Kazari.DZx
{
    public partial class RPATControl : UserControl
    {
        RPAT Actor;

        public RPATControl(RPAT ThisActor)
        {
            InitializeComponent();

            Actor = ThisActor;

            this.SuspendLayout();
            UpdateControl();
            this.ResumeLayout();
        }

        void UpdateControl()
        {
            AttachDetachEvents(false);

            textBox_numPoints.Text = Actor.NumPoints.ToString("X4");
            textBox_u1.Text = Actor.Unknown1.ToString("X4");
            textBox_u2.Text = Actor.Unknown2.ToString("X2");
            textBox_u3.Text = Actor.Unknown3.ToString("X2");
            textBox_offset.Text = Actor.FirstEntryOffset.ToString("X8");

            AttachDetachEvents(true);
        }

        void AttachDetachEvents(bool Attach)
        {
            if (Attach == true)
            {
                textBox_numPoints.TextChanged += new EventHandler(textBox_numPoints_TextChanged);
                textBox_u1.TextChanged += new EventHandler(textBox_u1_TextChanged);
                textBox_u2.TextChanged += new EventHandler(textBox_u2_TextChanged);
                textBox_u3.TextChanged += new EventHandler(textBox_u3_TextChanged);
                textBox_offset.TextChanged += new EventHandler(textBox_offset_TextChanged);
            }
            else
            {
                textBox_numPoints.TextChanged -= new EventHandler(textBox_numPoints_TextChanged);
                textBox_u1.TextChanged -= new EventHandler(textBox_u1_TextChanged);
                textBox_u2.TextChanged -= new EventHandler(textBox_u2_TextChanged);
                textBox_u3.TextChanged -= new EventHandler(textBox_u3_TextChanged);
                textBox_offset.TextChanged -= new EventHandler(textBox_offset_TextChanged);
            }
        }

        private void textBox_numPoints_TextChanged(object sender, EventArgs e)
        {
            Actor.NumPoints = ushort.Parse(textBox_numPoints.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void textBox_u1_TextChanged(object sender, EventArgs e)
        {
            Actor.Unknown1 = ushort.Parse(textBox_u1.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void textBox_u2_TextChanged(object sender, EventArgs e)
        {
            Actor.Unknown2 = byte.Parse(textBox_u2.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void textBox_u3_TextChanged(object sender, EventArgs e)
        {
            Actor.Unknown3 = byte.Parse(textBox_u3.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void textBox_offset_TextChanged(object sender, EventArgs e)
        {
            Actor.FirstEntryOffset = UInt32.Parse(textBox_offset.Text, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
