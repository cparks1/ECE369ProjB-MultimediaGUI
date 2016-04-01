﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets; // For network programming
using System.Net; // For network programming

namespace ProjectBMultimediaGUI
{
    public partial class Form1 : Form
    {
        private delegate void ObjectDelegate(object obj);

        const int PUBLISH_PORT_NUMBER = 8030; // Port number used for publish (UDP communications)
        const int TCP_PORT_NUMBER = 8031; // Port number used for the rest of communications (TCP communications)
        UdpClient pub = new UdpClient(PUBLISH_PORT_NUMBER, AddressFamily.InterNetwork); // Creates a new UDP client capable of communicating on a network on port defined by const, via IPv4 addressing

        bool isClosing = false; // Used to determine if program is closing

        public Form1()
        {
            InitializeComponent();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        { Application.Exit(); }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fopen = new OpenFileDialog();
            fopen.CheckFileExists = true; fopen.CheckPathExists = true; fopen.Filter = "WAV Files|*.wav";
            fopen.ShowDialog();
        }

        private void playpauseBUT_MouseClick(object sender, MouseEventArgs e)
        {
            (sender as Button).ImageIndex = ((sender as Button).ImageIndex == 0) ? 1 : 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            butIML.ImageSize = new Size(playpauseBUT.Size.Width-1,playpauseBUT.Size.Height-1); // This ensures the play and pause buttons are always the same size as the button they are encased in
            stopBUT.Size = playpauseBUT.Size; // Ensures the stop button is the same size as the play/pause button.
            stopBUT.Location = new Point(stopBUT.Location.X, playpauseBUT.Location.Y);

            pub.AllowNatTraversal(false); // Disables the ability for the program to communicate with the outside world, for security purposes
            try { pub.BeginReceive(new AsyncCallback(RecvPub), null); }
            catch(Exception err) { MessageBox.Show("An error occurred!\n"+err.ToString()); Application.Exit(); }
        }

        private void stopBUT_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void mainMS_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.Black), mainMS.Left, mainMS.Bottom-1, mainMS.Right, mainMS.Bottom-1); // Draws a border on the bottom of the main menu strip.
        }

        private void RecvPub(IAsyncResult res) // Function used to handle received UDP messages
        {
            IPEndPoint recv = new IPEndPoint(IPAddress.Any, PUBLISH_PORT_NUMBER);
            byte[] message = null;
            string dmessage;
            if (!isClosing)
                message = pub.EndReceive(res, ref recv);

            if(message != null) // If a message was received
            {
                ObjectDelegate del = new ObjectDelegate(HandleMsg);
                del.Invoke(message);

                dmessage = Encoding.ASCII.GetString(message);
                HandleMsg(dmessage);
            }

            if (!isClosing)
                pub.BeginReceive(new AsyncCallback(RecvPub), null);
        }

        private void HandleMsg(object obj) // Used for handling UDP messages
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;
        }
    }
}
