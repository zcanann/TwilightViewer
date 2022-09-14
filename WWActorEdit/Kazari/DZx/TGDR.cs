﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace WWActorEdit.Kazari.DZx
{
    public class TGDR : IDZxChunkElement
    {
        string _Name;
        uint _Parameters;
        Vector3 _Position;
        ushort _Unknown1;
        ushort _RotationY;
        ushort _Unknown2;
        ushort _Unknown3;
        uint _Unknown4;

        float _Size;

        public string Name { get { return _Name; } set { _Name = value; HasChanged = true; } }
        public uint Parameters { get { return _Parameters; } set { _Parameters = value; HasChanged = true; } }
        public Vector3 Position { get { return _Position; } set { _Position = value; HasChanged = true; } }
        public ushort Unknown1 { get { return _Unknown1; } set { _Unknown1 = value; HasChanged = true; } }
        public ushort RotationY { get { return _RotationY; } set { _RotationY = value; HasChanged = true; } }
        public ushort Unknown2 { get { return _Unknown2; } set { _Unknown2 = value; HasChanged = true; } }
        public ushort Unknown3 { get { return _Unknown3; } set { _Unknown3 = value; HasChanged = true; } }
        public uint Unknown4 { get { return _Unknown4; } set { _Unknown4 = value; HasChanged = true; } }

        public float Size { get { return _Size; } set { _Size = value; HasChanged = true; } }

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

        public TGDR(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color), ZeldaArc ParentZA = null)
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _Name = Helpers.ReadString(SrcData, SrcOffset, 8);
            _Parameters = Helpers.Read32(SrcData, SrcOffset + 8);
            _Position = new Vector3(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x0C)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x10)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x14)));

            _Unknown1 = Helpers.Read16(SrcData, SrcOffset + 0x18);
            _RotationY = Helpers.Read16(SrcData, SrcOffset + 0x1A);
            _Unknown2 = Helpers.Read16(SrcData, SrcOffset + 0x1C);

            _Unknown3 = Helpers.Read16(SrcData, SrcOffset + 0x1E);
            _Unknown4 = Helpers.Read32(SrcData, SrcOffset + 0x20);

            _Size = Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x20));

            SrcOffset += 0x24;


            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}: {1}", Offset, _Name), this);
            ParentNode.BackColor = RenderColor;
            ParentNode.Nodes.Add(Node);

            GLID = GL.GenLists(1);
            GL.NewList(GLID, ListMode.Compile);

            if (ParentZA != null)
            {
                MatchedModel = ParentZA.J3Dxs.Find(x => x.FileEntry.FileName.StartsWith(_Name));
                MatchedCollision = ParentZA.DZBs.Find(x => x.Name.StartsWith(_Name));
            }

            //Helpers.DrawFramedCube(new Vector3d(15, 15, 15));
            Helpers.DrawFramedCube(new Vector3d(1, 1, 1));


            GL.EndList();
        }

        public void StoreChanges()
        {
            byte[] Data = ParentFile.GetFileData();

            Helpers.WriteString(ref Data, Offset, _Name, 8);
            Helpers.Overwrite32(ref Data, Offset + 0x08, _Parameters);
            Helpers.Overwrite32(ref Data, Offset + 0x0C, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.X), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x10, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Y), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x14, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Z), 0));
            Helpers.Overwrite16(ref Data, Offset + 0x18, _Unknown1);
            Helpers.Overwrite16(ref Data, Offset + 0x1A, _RotationY);
            Helpers.Overwrite16(ref Data, Offset + 0x1C, _Unknown2);
            Helpers.Overwrite16(ref Data, Offset + 0x1E, _Unknown3);
            Helpers.Overwrite32(ref Data, Offset + 0x20, _Unknown4);

            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(_Position);

            string size = _Unknown4.ToString("X8");
            int width, length, height;
            double widthScale, lenghtScale, heightScale;

            int.TryParse(size.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out width);
            int.TryParse(size.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out height);
            int.TryParse(size.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out length);


            /*
            widthScale = Math.Sqrt(width);
            lenghtScale = Math.Sqrt(length);
            heightScale = Math.Sqrt(height);
            */

            if (_Name == "scnChg" || _Name == "noChgRM" || _Name == "CamArea" || _Name == "CamAreC" || _Name == "SwAreaS" || _Name == "SwAreaC" || _Name == "ClearB" || _Name == "Hjump" || _Name == "Hstop" || _Name == "TagEvC" || _Name == "EvtArea" || _Name == "hider" || _Name == "Stream" || _Name == "mirror") //Rectangle or Cuboid (Quader)
            {
                width = (width * 7);
                height = (height * 7);
                length = (length * 7);

                widthScale = width;
                heightScale = height;
                lenghtScale = length;
            }
            else //Circle or Cylinder or Sphere (Kugel)
            {
                //width = (width * 7);
                //height = (height * 7);
                //length = (length * 7);

                //widthScale = width;
                //heightScale = height;
                //lenghtScale = length;

                widthScale = 15;
                heightScale = 15;
                lenghtScale = 15;
            }

            if (!Highlight)
            {
                GL.Scale(new Vector3d(15, 15, 15));
            }
            else
            {
                GL.Scale(new Vector3d(widthScale, heightScale, lenghtScale));
            }


            GL.Rotate(_RotationY, 0, 1, 0);
            GL.Color4((Highlight ? System.Drawing.Color.Red : RenderColor));
            if (MatchedModel != null) MatchedModel.Render();
            if (MatchedCollision != null) MatchedCollision.Render();
            if (GL.IsList(GLID) == true) GL.CallList(GLID);
            GL.PopMatrix();
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new TGDRControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }
    }
}
