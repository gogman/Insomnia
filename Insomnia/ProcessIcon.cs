#region File Information

// Solution: Insomnia
// Project Name: Insomnia
// 
// File: ProcessIcon.cs
// Created: 2021/12/14 @ 12:25 PM
// Updated: 2021/12/16 @ 3:24 PM

#endregion

#region using

using System;
using System.Drawing;
using System.Windows.Forms;

using Insomnia.Properties;

#endregion

namespace Insomnia
{
    public class ProcessIcon : IDisposable
    {
        public EventHandler<EventArgs> DoubleClick;

        public ProcessIcon()
        {
            notifyIcon = new NotifyIcon();
        }

        public ContextMenuStrip LeftClickContextMenu { get; set; }

        public NotifyIcon notifyIcon { get; set; }

        public ContextMenuStrip RightClickContextMenu { get; set; }

        public void Display()
        {
            notifyIcon.MouseClick += OnIconMouseClick;
            notifyIcon.DoubleClick += OnDoubleClick;
            notifyIcon.Text =   Application.ProductName ;
            notifyIcon.Icon = new Icon(Resources.AppIcon, 40, 40);
            notifyIcon.Visible = true;
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }

        private void OnDoubleClick(object? sender, EventArgs e)
        {
            EventHandler<EventArgs> doubleClick = DoubleClick;

            doubleClick?.Invoke(sender, e);
        }

        private void OnIconMouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:

                    LeftClickContextMenu?.Hide();

                    if (RightClickContextMenu != null)
                    {
                        if (RightClickContextMenu.Visible)
                        {
                            RightClickContextMenu.Hide();
                        }
                        else
                        {
                            RightClickContextMenu?.Show(Cursor.Position.X, Cursor.Position.Y);
                        }
                    }

                    break;

                case MouseButtons.Left:

                    RightClickContextMenu?.Hide();

                    if (LeftClickContextMenu != null)
                    {
                        if (LeftClickContextMenu.Visible)
                        {
                            LeftClickContextMenu.Hide();
                        }
                        else
                        {
                            LeftClickContextMenu?.Show(Cursor.Position.X, Cursor.Position.Y);
                        }
                    }

                    break;
            }
        }
    }
}