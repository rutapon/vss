//    nVLC
//    
//    Author:  Roman Ginzburg
//
//    nVLC is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    nVLC is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.
//     
// ========================================================================
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;

namespace nVLC_Demo_WinForms
{
    public partial class Form1 : Form
    {
        IMediaPlayerFactory m_factory;
        IDiskPlayer m_player;
        IMedia m_media;

        public Form1()
        {
            InitializeComponent();

            textBox1.Text = "rtsp://localhost:8080/abc";

            m_factory = new MediaPlayerFactory();
            m_player = m_factory.CreatePlayer<IDiskPlayer>();

            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
            m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
            m_player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
            m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);

            m_player.WindowHandle = panel1.Handle;
            trackBar2.Value = m_player.Volume;

            UISync.Init(this);
        }

        void Events_PlayerStopped(object sender, EventArgs e)
        {
            UISync.Execute(() => InitControls());
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            UISync.Execute(() => InitControls());
        }

        private void InitControls()
        {
            trackBar1.Value = 0;
            lblTime.Text = "00:00:00";
            lblDuration.Text = "00:00:00";
        }

        void Events_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            UISync.Execute(() => lblTime.Text = TimeSpan.FromMilliseconds(e.NewTime).ToString().Substring(0, 8));
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            UISync.Execute(() => trackBar1.Value = (int)(e.NewPosition * 100));
        }

        private void LoadMedia()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        void Events_StateChanged(object sender, MediaStateChange e)
        {
            UISync.Execute(() => label1.Text = e.NewState.ToString());
        }

        void Events_DurationChanged(object sender, MediaDurationChange e)
        {
            UISync.Execute(() => lblDuration.Text = TimeSpan.FromMilliseconds(e.NewDuration).ToString().Substring(0, 8));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadMedia();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {

                m_media = m_factory.CreateMedia<IMedia>(textBox1.Text);
                m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
                m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);
                m_media.Events.ParsedChanged += new EventHandler<MediaParseChange>(Events_ParsedChanged);

                m_player.Open(m_media);
                m_media.Parse(true);

                m_player.Play();
            }
            else
            {
                errorProvider1.SetError(textBox1, "Please select media path first !");
            }
        }

        void Events_ParsedChanged(object sender, MediaParseChange e)
        {
            Console.WriteLine(e.Parsed);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            m_player.Volume = trackBar2.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            m_player.Position = (float)trackBar1.Value / 100;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            m_player.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_player.Pause();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_player.ToggleMute();

            button1.Text = m_player.Mute ? "Unmute" : "Mute";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();
        }

        private class UISync
        {
            private static ISynchronizeInvoke Sync;

            public static void Init(ISynchronizeInvoke sync)
            {
                Sync = sync;
            }

            public static void Execute(Action action)
            {
                Sync.BeginInvoke(action, null);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                // string output = ":sout=file:abc.mp4";

                // string output = ":sout=#transcode{vcodec=h264,vb=0,scale=0,acodec=mpga,ab=128,channels=2,samplerate=44100}:duplicate{dst=file{dst=abc.mp4}";//,dst=display}
                // string output1 = ":demux=dump :demuxdump-file=output.mp4"; 
                string output = ":sout=#transcode{demux=dump}:duplicate{dst=file{dst=output1.mp4}";//,dst=display}

                m_media = m_factory.CreateMedia<IMedia>(textBox1.Text, output);
                m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
                m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);
                m_media.Events.ParsedChanged += new EventHandler<MediaParseChange>(Events_ParsedChanged);

                m_player.Open(m_media);
                m_media.Parse(true);

                m_player.Play();
            }
            else
            {
                errorProvider1.SetError(textBox1, "Please select media path first !");
            }
        }

        bool run = false;
        int countSave = 0;
        private void button7_Click(object sender, EventArgs e)
        {

            run = true;
            Thread t = new Thread(() =>
            {

                int thisCount = countSave++;
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    var factory = new MediaPlayerFactory();
                    var player = factory.CreatePlayer<IDiskPlayer>();

                    player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
                    player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
                    player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
                    player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);


                    trackBar2.Value = player.Volume;

                    // string output = ":sout=file:abc.mp4";

                    // string output = ":sout=#transcode{vcodec=h264,vb=0,scale=0,acodec=mpga,ab=128,channels=2,samplerate=44100}:duplicate{dst=file{dst=abc.mp4}";//,dst=display}
                    // string output1 = ":demux=dump :demuxdump-file=output.mp4"; 

                    while (run)
                    {
                        try
                        {
                            string fileName = "data\\" + thisCount + "-" + DateTime.Now.ToLongTimeString();
                            string output = ":sout=#transcode{demux=dump}:duplicate{dst=file{dst=" + fileName + ".mp4},dst=rtp{sdp=rtsp://:42624/test},dst=http{mux=ffmpeg{mux=flv},dst=:48684/}}";//,dst=display} dst=rtp{sdp=rtsp://:8554/},dst=http{mux=ffmpeg{mux=flv},dst=:8080/}

                            var media = m_factory.CreateMedia<IMedia>(textBox1.Text, output);
                            media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
                            media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);
                            media.Events.ParsedChanged += new EventHandler<MediaParseChange>(Events_ParsedChanged);

                            player.Open(media);
                            media.Parse(true);

                            player.Play();

                            Thread.Sleep(60000);
                            player.Stop();
                        }
                        catch { }
                    }
                }
                else
                {
                    errorProvider1.SetError(textBox1, "Please select media path first !");
                }

            });

            t.Start();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            run = false;

        }

    }
}
