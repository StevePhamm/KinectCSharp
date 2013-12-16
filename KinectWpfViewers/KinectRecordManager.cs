using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinect.Toolbox.Record;
using Coding4Fun.Kinect.Wpf.Controls;
using Microsoft.Kinect;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Samples.Kinect.WpfViewers
{

    class KinectRecordManager
    {
        private KinectSensor KinectSensor;
        private KinectRecorder KinectSkeletonRecorder;
        private KinectRecorder KinectColorRecorder;
        private KinectRecorder KinectDepthRecorder;
        private BufferedStream SkeletonRecordStream;
        private BufferedStream ColorRecordStream;
        private BufferedStream DepthRecordStream;

        public String RecordPath { get; set;}
        public String RecordFileName { get; set; }
        public bool Recording { get; private set; }


        public KinectRecordManager()
        {
            Recording = false;
            RecordPath = "";
            RecordFileName = "";
            KinectSensor = KinectSensor.KinectSensors.FirstOrDefault(e => e.Status == KinectStatus.Connected);
            KinectSensor.AllFramesReady += this.OnAllFramesReady;
        }

        private void OnAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (Recording)
            {
                using (SkeletonFrame SkeletonFrame = e.OpenSkeletonFrame())
                {
                    if (SkeletonFrame != null)
                        KinectSkeletonRecorder.Record(SkeletonFrame);
                }
                using (ColorImageFrame ColorImageFrame = e.OpenColorImageFrame())
                {
                    if (ColorImageFrame != null)
                        KinectColorRecorder.Record(ColorImageFrame);
                }
                using (DepthImageFrame DepthImageFrame = e.OpenDepthImageFrame())
                {
                    if (DepthImageFrame != null)
                        KinectDepthRecorder.Record(DepthImageFrame);
                }
            }
        }

        public void ToggleRecord()
        {
            if (!Recording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        public void StartRecording()
        {
            if (!Recording)
            {
                System.Diagnostics.Debug.WriteLine("Start recording...");
                string skeletonFile = Path.Combine(RecordPath, RecordFileName + "_skeleton.data");
                string colorFile = Path.Combine(RecordPath, RecordFileName + "_color.data.");
                string depthFile = Path.Combine(RecordPath, RecordFileName + "_depth.data");
                SkeletonRecordStream = new BufferedStream(new FileStream(skeletonFile, FileMode.Create));
                ColorRecordStream = new BufferedStream(new FileStream(colorFile, FileMode.Create));
                DepthRecordStream = new BufferedStream(new FileStream(depthFile, FileMode.Create));

                KinectSkeletonRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, SkeletonRecordStream);
                KinectColorRecorder = new KinectRecorder(KinectRecordOptions.Color, ColorRecordStream);
                KinectDepthRecorder = new KinectRecorder(KinectRecordOptions.Depth, DepthRecordStream);
                Recording = true;
            }
        }

        public void StopRecording()
        {
            if (Recording)
            {
                System.Diagnostics.Debug.WriteLine("Stop recording...");
                SkeletonRecordStream.Flush();
                ColorRecordStream.Flush();
                DepthRecordStream.Flush();

                KinectSkeletonRecorder.Stop();
                KinectColorRecorder.Stop();
                KinectDepthRecorder.Stop();
                Recording = false;
            }
        }
    }
}
