using System;
using System.Collections.Generic;
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
        public static void vlcSave()
        {
            Thread t = new Thread(() =>
            {
                var factory = new MediaPlayerFactory();
                var player = factory.CreatePlayer<IDiskPlayer>();

                 //string output = ":sout=file:abc.mp4";

                // string output = ":sout=#transcode{vcodec=h264,vb=0,scale=0,acodec=mpga,ab=128,channels=2,samplerate=44100}:duplicate{dst=file{dst=abc.mp4}";//,dst=display}
                // string output = ":demux=dump :demuxdump-file=output.mp4"; 
                try
                {
                    if (!Directory.Exists("data"))
                    {
                        Directory.CreateDirectory("data");//dst=http{mux=ffmpeg{mux=flv},dst=:8080/}
                    }
                    
                    string fileName = "data\\" + DateTime.Now.ToLongTimeString();
                    string output = ":sout=#transcode{demux=dump}:duplicate{dst=file{dst=" + fileName + ".mp4},dst=rtp{sdp=rtsp://:8554/}}";

                    var media = factory.CreateMedia<IMedia>("rtsp://172.30.245.167:42624/ufirststream", output);

                    player.Open(media);
                    media.Parse(true);

                    player.Play();

                    Thread.Sleep(50000);
                    player.Stop();
                    File.WriteAllText("fin", "");
                }
                catch (Exception ex){
                    File.WriteAllText("ex"+ex.Message,"");
                }
            });

            t.Start();
        }
    }
}
