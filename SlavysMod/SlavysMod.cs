using System;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;
using System.Drawing;
using System.Threading.Tasks;

namespace SlavysMod
{
    public class Main : Script
    { 
        private readonly Server httpServer = new Server();
        private bool isServerRunning = false;
        private bool showUI = true;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void OnTick(object sender, EventArgs e)
        {
            StartServerIfNeeded();
            if (showUI)
                DisplayScreenText();
            CommandSystem();
            UpdateSystems();
        }

        private void StartServerIfNeeded()
        {
            if (!isServerRunning)
            {
                httpServer.Start();
                isServerRunning = true;
            }
        }

        private void DisplayScreenText()
        {
            Utils.DrawTextOnScreen(
                $"Use this as a debug/console",
                new PointF(10, 50),
                Alignment.Left
            );
        }

        private void UpdateSystems()
        {
            /* Mod mechanic systems go here */
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Numpad keys are intended for debugging
            switch (e.KeyCode)
            {
                case Keys.NumPad7:
                    showUI = !showUI; // Toggle UI
                    break;
                default:
                    break;
            }
        }

        // Server commands from the POST/GET requests are handled here
        private void CommandSystem()
        {
            Commands cmd = httpServer.GetCommand();
            if (cmd == null) return;

            switch (cmd.command)
            {
                case "sample_endpoint":
                    /* do something */
                    break;
                default:
                    break;
            }
        }

        /* Create private methods for each command below to maintain readability and organization */
    }
}
