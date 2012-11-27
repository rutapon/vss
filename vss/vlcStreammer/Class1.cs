using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;

namespace vlcStreammer
{
    public class Class1
    {
        public static void vlcSave(string localPath, string rootPath)
        {
            //Thread t = new Thread(() =>
            //{
            if (!Directory.Exists(rootPath + "\\data1"))
            {
                Directory.CreateDirectory(rootPath + "\\data1");//dst=http{mux=ffmpeg{mux=flv},dst=:8080/}
            }
                if (!Directory.Exists(localPath + "\\data2"))
                {
                    Directory.CreateDirectory(localPath + "\\data2");//dst=http{mux=ffmpeg{mux=flv},dst=:8080/}
                }
                File.WriteAllText(rootPath + "ff", "");
                

                //string output = ":sout=file:" + localPath + "abc.mp4";

                // string output = ":sout=#transcode{vcodec=h264,vb=0,scale=0,acodec=mpga,ab=128,channels=2,samplerate=44100}:duplicate{dst=file{dst=abc.mp4}";//,dst=display}
                // string output = ":demux=dump :demuxdump-file=output.mp4"; 
                try
                {
                    File.WriteAllText(rootPath + "b1", "");

                    var factory = new MediaPlayerFactory();
                    File.WriteAllText(rootPath + "b2", "");

                    var player = factory.CreatePlayer<IDiskPlayer>();

                    File.WriteAllText(rootPath + "b3", "");

                    string fileName = localPath + "abc";
                    //string fileName = localPath + DateTime.Now.ToLongTimeString();
                    //string fileName = localPath + "\\" + DateTime.Now.ToLongTimeString();
                    //string fileName =  DateTime.Now.ToLongTimeString();
                    //string output = ":sout=#transcode{demux=dump}:duplicate{dst=file{dst=" + fileName + ".mp4},dst=rtp{sdp=rtsp://:8554/}}";
                    //string output = ":sout=#transcode{demux=dump,channels=1}:duplicate{dst=file{dst=" + fileName + ".mp4},dst=rtp{sdp=rtsp://:8554/}}";
                    //string output = ":sout=#transcode{demux=dump,channels=1}:duplicate{dst=file{dst=" + fileName + ".mp4},dst=rtp{sdp=rtsp://:8554/}}";
                    string output = ":sout=#transcode{demux=dump,channels=1}:duplicate{dst=file{dst=" + fileName + ".mp4}}";


                    var media = factory.CreateMedia<IMedia>(rootPath + "_5_19_01.mp4", output);
                    //rtsp://178.218.212.102:1935/live/Stream2
                    //delay 15~25 sec
                    //have 4 streams 0:A 1:V 2:A 3:V
                    //channnels=0: rec->Audio stream->Audio
                    //channnels=1: rec->Audio+Video stream->Audio+Video
                    //channnels=4: rec->Audio stream->Audio+Video
                    

                    player.Open(media);
                    media.Parse(true);

                    player.Play();

                    Thread.Sleep(50000);
                    player.Stop();
                    File.WriteAllText("fin", "");
                }
                catch (Exception ex){
                    File.WriteAllText(rootPath + "ex" + ex.Message, "");
                }
            //});

            //t.Start();
        }
    }
}
