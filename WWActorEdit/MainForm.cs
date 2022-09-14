﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;
using Blue.Windows;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using WWActorEdit.Forms;
using WWActorEdit.Kazari;
using WWActorEdit.Kazari.DZx;
using WWActorEdit.Kazari.DZB;
using WWActorEdit.Kazari.J3Dx;

namespace WWActorEdit
{
    public partial class MainForm : Form
    {
        //A list of currently loaded .arc Archives
        public List<ZeldaArc> Rooms = new List<ZeldaArc>();
        //Shortcut to the 'Stage' one if it is loaded.
        public ZeldaArc Stage;

        //idek
        IDZxChunkElement SelectedDZRChunkElement;

        //Has the GL Control been loaded? Used to prevent rendering before GL is Initialized.
        private bool _glContextLoaded;

        //Used for "dockable" WinForms
        private StickyWindow _stickyWindow;

        public MainForm()
        {
            //Initialize the WinForm
            InitializeComponent();

            _stickyWindow = new StickyWindow(this);
        }

        #region GLControl
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl.IsIdle == true)
            {
                RenderFrame();
            }
        }

        void glControl_Load(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(Application_Idle);

            Helpers.Enable3DRendering(new SizeF(glControl.Width, glControl.Height));

            _glContextLoaded = true;
        }

        void glControl_Paint(object sender, PaintEventArgs e)
        {
            RenderFrame();
        }

        void glControl_Resize(object sender, EventArgs e)
        {
            if (_glContextLoaded == false) return;

            Helpers.Enable3DRendering(new SizeF(glControl.Width, glControl.Height));
            glControl.Invalidate();
        }

        void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            Helpers.Camera.KeysDown[e.KeyValue] = true;
        }

        void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            Helpers.Camera.KeysDown[e.KeyValue] = false;
        }

        void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Helpers.Camera.Mouse.LDown = true;
                    break;
                case MouseButtons.Right:
                    Helpers.Camera.Mouse.RDown = true;
                    break;
                case MouseButtons.Middle:
                    Helpers.Camera.Mouse.MDown = true;
                    break;
            }

            Helpers.Camera.Mouse.Center = new Vector2(e.X, e.Y);

            if (Helpers.Camera.Mouse.LDown == true)
            {
                if (Helpers.Camera.Mouse.Center != Helpers.Camera.Mouse.Move)
                    Helpers.Camera.MouseMove(Helpers.Camera.Mouse.Move);
                else
                    Helpers.Camera.MouseCenter(Helpers.Camera.Mouse.Move);
            }
        }

        void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            Helpers.Camera.Mouse.Move = new Vector2(e.X, e.Y);

            if (Helpers.Camera.Mouse.LDown == true)
            {
                if (Helpers.Camera.Mouse.Center != Helpers.Camera.Mouse.Move)
                    Helpers.Camera.MouseMove(Helpers.Camera.Mouse.Move);
                else
                    Helpers.Camera.MouseCenter(Helpers.Camera.Mouse.Move);
            }
        }

        void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Helpers.Camera.Mouse.LDown = false;
                    break;
                case MouseButtons.Right:
                    Helpers.Camera.Mouse.RDown = false;
                    break;
                case MouseButtons.Middle:
                    Helpers.Camera.Mouse.MDown = false;
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Instead of faking a Paint event inside the Application.Idle we'll just put
        /// the drawing into its own function and call it in both Application.Idle
        /// and in the Paint event of the GL control.
        /// </summary>
        private void RenderFrame()
        {
            if (_glContextLoaded == false) return;

            GL.ClearColor(Color.DodgerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Helpers.Enable3DRendering(new SizeF(glControl.Width, glControl.Height));

            Helpers.Camera.Position();


            GL.Scale(0.005f, 0.005f, 0.005f);

            /* Models */
            if (renderModelsToolStripMenuItem.Checked == true)
            {
                foreach (ZeldaArc A in Rooms)
                {
                    GL.PushMatrix();
                    GetGlobalTranslation(A);
                    GetGlobalRotation(A);

                    foreach (J3Dx M in A.J3Dxs)
                    {
                        /* Got model translation from Stage? (ex. rooms in sea) */
                        if (A.GlobalTranslation != Vector3.Zero || A.GlobalRotation != 0)
                        {
                            /* Perform translation */
                            GL.Translate(A.GlobalTranslation);
                            GL.Rotate(A.GlobalRotation, 0, 1, 0);
                        }
                        M.Render();
                    }
                    GL.PopMatrix();
                }
            }

            /* Actors, 1st pass */
            if (renderRoomActorsToolStripMenuItem.Checked == true)
            {
                foreach (ZeldaArc A in Rooms)
                {
                    if (A.DZRs != null) foreach (DZx D in A.DZRs) D.Render();
                    if (A.DZSs != null) foreach (DZx D in A.DZSs) D.Render();
                }
            }
            if (renderStageActorsToolStripMenuItem.Checked == true && Stage != null)
            {
                if (Stage.DZRs != null) foreach (DZx D in Stage.DZRs) D.Render();
                if (Stage.DZSs != null) foreach (DZx D in Stage.DZSs) D.Render();
            }

            /* Collision */
            if (renderCollisionToolStripMenuItem.Checked == true)
                foreach (ZeldaArc A in Rooms) foreach (DZB D in A.DZBs) D.Render();

            /* Actors, 2nd pass */
            if (renderRoomActorsToolStripMenuItem.Checked == true)
            {
                foreach (ZeldaArc A in Rooms)
                {
                    if (A.DZRs != null) foreach (DZx D in A.DZRs) D.Render();
                    if (A.DZSs != null) foreach (DZx D in A.DZSs) D.Render();
                }
            }
            if (renderStageActorsToolStripMenuItem.Checked == true && Stage != null)
            {
                if (Stage.DZRs != null) foreach (DZx D in Stage.DZRs) D.Render();
                if (Stage.DZSs != null) foreach (DZx D in Stage.DZSs) D.Render();
            }

            Helpers.Camera.KeyUpdate();
            glControl.SwapBuffers();
        }

        /// <summary>
        /// In a 'Stage', there is data that is indexed by Room number. The actual rooms don't store
        /// this data internally, it is only by file name. So we're going to strip apart the filename
        /// to get the room number.
        /// </summary>
        /// <param name="NewArc"></param>
        private void GetRoomNumber(ZeldaArc NewArc)
        {
            int roomNumber = 0;
            
            //We're going to trim the Filepath down to just name - ie: "Room0.arc / R00_00.arc"
            string fileName = Path.GetFileName(NewArc.Filename);

            //If it starts with "Room" then it's (probably) a Windwaker Archive.
            if (fileName.Substring(0, 4).ToLower() == "room")
            {
                //Use Regex here to grab what is between "Room" and ".arc", since it goes up to "Room23.arc"
                string[] numbers = Regex.Split(fileName, @"\D+");
                string trimmedNumbers = String.Join("", numbers);
                trimmedNumbers = trimmedNumbers.Trim();

                roomNumber = int.Parse(trimmedNumbers);
            }
            //If it starts with R ("Rxx_00, xx being Room Number"), it's Twlight Princess
            else if (fileName.Substring(0, 1).ToLower() == "r")
            {
                //I *think* these follow the Rxx_00 pattern, where xx is the room number. _00 can change, xx might be 1 or 3, who knows!
                
                //We're going to use RegEx here to make sure we only grab what is between R and _00 which could be multipl.e
                string[] numbers = Regex.Split(fileName.Substring(0, fileName.Length - 6), @"\D+");
                string trimmedNumbers = String.Join("", numbers);
                trimmedNumbers = trimmedNumbers.Trim();

                roomNumber = int.Parse(trimmedNumbers);
            }
            else
            {
                InvalidRoomNumber popup = new InvalidRoomNumber();
                popup.DescriptionLabel.Text =
                    "Failed to determine room number from file name." + Environment.NewLine + "Expected: Room<x>.arc or R<xx>_00, got: " +
                    fileName;
                popup.ShowDialog(this);

                roomNumber = (int) popup.roomNumberSelector.Value;
                Console.WriteLine("User chose: " + roomNumber);
            }

            NewArc.RoomNumber = roomNumber;
        }

        /// <summary>
        /// This is a terribly placed/named function, but I'm going to leave it for now until I fully understand it.
        /// My best guess is that the "Stage" file contains the translation/rotation of each individual room. These
        /// can be loaded in any order in WindViewer, so my guess is that they're just set every frame instead of
        /// when a Stage/Room is loaded. Weird.
        /// </summary>
        /// <param name="A"></param> 
        private void GetGlobalTranslation(ZeldaArc A)
        {
            if (Stage != null)
            {
                foreach (DZx D in Stage.DZSs)
                {
                    foreach (DZx.FileChunk Chunk in D.Chunks)
                    {
                        foreach (IDZxChunkElement ChunkElement in Chunk.Data.Where(C => C is MULT && ((MULT)C).RoomNumber == A.RoomNumber))
                        {
                            A.GlobalTranslation = new Vector3(((MULT)ChunkElement).Translation.X, 0.0f, ((MULT)ChunkElement).Translation.Y);
                        }
                    }
                }
            }
        }

        private void GetGlobalRotation(ZeldaArc A)
        {
            if (Stage != null)
            {
                foreach (DZx D in Stage.DZSs)
                {
                    foreach (DZx.FileChunk Chunk in D.Chunks)
                    {
                        foreach (IDZxChunkElement ChunkElement in Chunk.Data.Where(C => C is MULT && ((MULT)C).RoomNumber == A.RoomNumber))
                        {
                            A.GlobalRotation = ((MULT)ChunkElement).Rotation;
                        }
                    }
                }
            }
        }

        private void LoadRARC(string Filename, bool IsRoom = true, bool IgnoreModels = false)
        {
            if (Filename != string.Empty)
            {
                ZeldaArc NewArc = new ZeldaArc(Filename, treeView1, IgnoreModels);

                if (IsRoom == true)
                {
                    GetRoomNumber(NewArc);
                    GetGlobalTranslation(NewArc);
                    GetGlobalRotation(NewArc);
                    Rooms.Add(NewArc);
                }
                else
                    Stage = NewArc;

                if (NewArc.Archive.IsCompressed == true)
                    MessageBox.Show(string.Format("RARC archive '{0}' is compressed; changes cannot be saved!", Path.GetFileName(NewArc.Archive.Filename)), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    saveChangesToolStripMenuItem.Enabled = true;
            }
        }

        private void UnloadAllRARCs()
        {
            foreach (ZeldaArc A in Rooms) A.Clear();
            if (Stage != null) Stage.Clear();

            Rooms = new List<ZeldaArc>();
            Stage = null;

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.EndUpdate();

            saveChangesToolStripMenuItem.Enabled = false;

            toolStripStatusLabel1.Text = "Ready";
        }

        private void openRoomRARCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] Files = Helpers.ShowOpenFileDialog("GameCube/Wii RARC archives (*.arc; *.rarc)|*.arc; *.rarc|All Files (*.*)|*.*", true);
            Array.Sort(Files);

            if (Files.Length == 1 && Files[0] == string.Empty) return;

            Helpers.MassEnableDisable(this.Controls, false);

            if (Files.Length > 5 && (renderRoomActorsToolStripMenuItem.Checked == true || renderStageActorsToolStripMenuItem.Checked == true))
            {
                if (MessageBox.Show("Rendering 5+ rooms while actor rendering is enabled might very likely cause heavy slowdown. Disable actor rendering?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    renderRoomActorsToolStripMenuItem.Checked = false;
                    renderStageActorsToolStripMenuItem.Checked = false;
                }
            }

            treeView1.BeginUpdate();

            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Maximum = Files.Length;

            foreach (string F in Files)
            {
                toolStripStatusLabel1.Text = string.Format("Loading '{0}'...", Path.GetFileName(F));
                LoadRARC(F, true);
                toolStripProgressBar1.PerformStep();
                Application.DoEvents();
            }

            toolStripProgressBar1.Visible = false;

            treeView1.EndUpdate();

            toolStripStatusLabel1.Text = string.Format("Loaded {0} room files. Ready!", Files.Length);

            Helpers.MassEnableDisable(this.Controls, true);
        }

        private void openStageRARCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] Files = Helpers.ShowOpenFileDialog("GameCube/Wii RARC archives (*.arc; *.rarc)|*.arc; *.rarc|All Files (*.*)|*.*");
            if (Files[0] == string.Empty) return;

            LoadRARC(Files[0], false);

            toolStripStatusLabel1.Text = "Loaded stage file. Ready!";

            foreach (ZeldaArc A in Rooms)
            {
                GetGlobalTranslation(A);
                GetGlobalRotation(A);
            }
        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ZeldaArc A in Rooms) A.Save();
            if (Stage != null) Stage.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.Camera.Initialize(new Vector3(0.0f, 0.0f, -5.0f));
            UnloadAllRARCs();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object Selected = ((TreeView)sender).SelectedNode.Tag;
            Console.WriteLine("TreeView clicked, Selected: " + Selected);

            if (SelectedDZRChunkElement != null)
            {
                SelectedDZRChunkElement.Highlight = false;
                if (SelectedDZRChunkElement.EditControl != null)
                    SelectedDZRChunkElement.EditControl.Dispose();
            }

            Panel TargetPanel = (Selected is IDZxChunkElement && (IDZxChunkElement)Selected is Generic) ? panel2 : panel1;
            Panel OtherPanel = (Selected is IDZxChunkElement && (IDZxChunkElement)Selected is Generic) ? panel1 : panel2;
            OtherPanel.Visible = false;

            TargetPanel.SuspendLayout();

            if (Selected is IDZxChunkElement)
            {
                SelectedDZRChunkElement = (IDZxChunkElement)Selected;

                SelectedDZRChunkElement.Highlight = true;
                SelectedDZRChunkElement.ShowControl(TargetPanel);

                if (autoCenterCameraToolStripMenuItem.Checked && (SelectedDZRChunkElement is Generic) == false)
                {
                    Vector3 CamPos = Vector3.Zero;
                    if (SelectedDZRChunkElement is ACTR)
                        CamPos = (-((ACTR)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is PLYR)
                        CamPos = (-((PLYR)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is RPPN)
                        CamPos = (-((RPPN)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is SHIP)
                        CamPos = (-((SHIP)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is TGDR)
                        CamPos = (-((TGDR)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is TRES)
                        CamPos = (-((TRES)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is MULT)
                        CamPos = (-(new Vector3(((MULT)SelectedDZRChunkElement).Translation.X, 0.0f, ((MULT)SelectedDZRChunkElement).Translation.Y) * 0.005f));

                    Helpers.Camera.Initialize(CamPos - new Vector3(0, 0, 2.0f));
                }
            }
            else
                TargetPanel.Visible = false;

            TargetPanel.ResumeLayout();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime BuildDate = new DateTime(2000, 1, 1).AddDays(AppVersion.Build).AddSeconds(AppVersion.Revision * 2);

            MessageBox.Show(
                Application.ProductName + " - Written 2012 by xdaniel - Build " + BuildDate.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
                "Improvements by LordNed, Abahbob, Pho, and Sage of Mirrors" + Environment.NewLine + 
                Environment.NewLine +
                "RARC, Yaz0 and J3Dx/BMD documentation by thakis" + Environment.NewLine +
                "DZB and DZx documentation by Sage of Mirrors, Twili, fkualol, xdaniel, et al." + Environment.NewLine +
                Environment.NewLine +
                "Greetings to The GCN's WW hacking thread!",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void showReadmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Well, there's no Readme yet actually... Some points of interest:" + Environment.NewLine +
                Environment.NewLine +
                "- While moving the camera around, hold Space to speed it up, or Shift to slow it down (good idea from Kargaroc)" + Environment.NewLine +
                "- Saving is not yet fully tested; it worked for me and I couldn't spot any obvious errors in the code but still, tread carefully" + Environment.NewLine +
                "- In case an exception occurs, my exception handler should pop up instead of Windows' mean dialog box; press Ctrl+C" + Environment.NewLine +
                "  there to copy everything into the clipboard, then message me or post that on The GCN or whatever" + Environment.NewLine +
                "- The icon was made by Sage of Mirrors, thanks!" + Environment.NewLine +
                "- I'm thinking of posting the source code for this as well, but I haven't decided yet; I'd rather first write some" + Environment.NewLine +
                "  documentation about WW's file formats, so that the source makes more sense and such" + Environment.NewLine +
                "- Here's hoping I didn't forget anyone in the About box credits :P",
                "Readme?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void environmentLightingEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnvironmentLightingEditorForm popup = new EnvironmentLightingEditorForm(this);
            popup.Show(this);
        }

        private void exitEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitEditor popup = new ExitEditor(this);
            popup.Show(this);
        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/pho/WindViewer/wiki/");
        }

        private void spawnEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpawnpointEditor popup = new SpawnpointEditor(this);
            popup.Show(this);
        }

        private void floatConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FloatConverter popup = new FloatConverter();
            popup.Show(this);
        }
    }
}
