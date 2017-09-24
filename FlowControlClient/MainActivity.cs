using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System;

namespace FlowControlClient
{
    [Activity(Label = "FlowControlClient", MainLauncher = true,ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            TextView rvu = FindViewById<TextView>(Resource.Id.textView1);
            System.Threading.Thread mthread = new System.Threading.Thread(() => {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("34.227.227.173"), 3801);
                UdpClient mclient = new UdpClient();
                byte id = 0;
                byte[] buffer = new byte[1024];
                Stopwatch mwatch = new Stopwatch();
                mwatch.Start();
                long[] timestamps = new long[256];
                int bytesPerSample = 0;
                int sampleTime = 0;
                int bytesSent = 0;
                int errors = 0;
                while(true)
                {
                    buffer[0] = id;
                    mclient.Send(buffer, buffer.Length,ep);
                    bytesSent += buffer.Length;
                    if(bytesPerSample > 0 && (bytesSent+buffer.Length)>bytesPerSample)
                    {
                        System.Threading.Thread.Sleep(sampleTime);
                        bytesSent = 0;
                        
                    }
                    timestamps[id] = mwatch.ElapsedMilliseconds;
                    if(mclient.Available>0)
                    {
                        IPEndPoint mref = new IPEndPoint(IPAddress.Any, 0);
                        byte[] me = mclient.Receive(ref mref);
                        byte a = me[0] < id ? me[0] : id;
                        byte b = me[0] < id ? id : me[0];
                        bytesPerSample = Math.Abs(a - b)*buffer.Length;
                        sampleTime = (int)Math.Abs(timestamps[me[0]]-timestamps[id]);
                        bytesSent = 0;
                        errors++;
                        RunOnUiThread(() => {
                            rvu.Text = "Errors: "+errors;
                        });
                    }
                    id++;
                }
            });
            mthread.Start();
        }
    }
}

