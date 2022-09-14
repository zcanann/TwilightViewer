using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit.Kazari.DZx
{
    public class RPAT : IDZxChunkElement
    {
        ushort _numPoints;
        ushort _unknown1;
        byte _unknown2;
        byte _unknown3;
        UInt32 _firstEntryOffset;

        public ushort NumPoints { get { return _numPoints; } set { _numPoints = value; HasChanged = true; } }
        public ushort Unknown1 { get { return _unknown1; } set { _unknown1 = value; HasChanged = true; } }
        public byte Unknown2 { get { return _unknown2; } set { _unknown2 = value; HasChanged = true; } }
        public byte Unknown3 { get { return _unknown3; } set { _unknown3 = value; HasChanged = true; } }
        public UInt32 FirstEntryOffset { get { return _firstEntryOffset; } set { _firstEntryOffset = value; HasChanged = true; } }

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; Node.ForeColor = (value == true ? System.Drawing.Color.Red : System.Drawing.SystemColors.WindowText); } }

        public bool Highlight { get; set; }
        public System.Drawing.Color RenderColor { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public J3Dx.J3Dx MatchedModel { get; set; }
        public DZB.DZB MatchedCollision { get; set; }

        public RPAT(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color), ZeldaArc ParentZA = null)
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _numPoints = Helpers.Read16(SrcData, Offset + 0x0);
            _unknown1 = Helpers.Read16(SrcData, Offset + 0x2);
            _unknown2 = Helpers.Read8(SrcData, Offset + 0x4);
            _unknown3 = Helpers.Read8(SrcData, Offset + 0x5);
            _firstEntryOffset = Helpers.Read32(SrcData, Offset + 0x8);

            SrcOffset += 0xC;

            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}: {1}", Offset, _numPoints), this);
            ParentNode.BackColor = RenderColor;
            ParentNode.Nodes.Add(Node);
        }

        public void StoreChanges()
        {
            byte[] Data = ParentFile.GetFileData();

            Helpers.Overwrite16(ref Data, Offset + 0x0, _numPoints);
            Helpers.Overwrite16(ref Data, Offset + 0x2, _unknown1);
            Helpers.Overwrite8(ref Data, Offset + 0x4, _unknown2);
            Helpers.Overwrite8(ref Data, Offset + 0x5, _unknown3);
            Helpers.Overwrite32(ref Data, Offset + 0x8, _firstEntryOffset);

            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new RPATControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }
    }
}
