using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Object_Sound_Recognition
{
    public partial class Form1 : TahirCCreativeControls.Form
    {


        private double[] pcmValues;
        private NAudio.Wave.WaveInEvent wvin;
        private int buffersRead = 0;
        public Form1()
        {
            InitializeComponent();

            PlotInitialize();
        }
        private void OnDataAvailable(object sender, NAudio.Wave.WaveInEventArgs args)
        {
            int bytesPerSample = wvin.WaveFormat.BitsPerSample / 8;
            int samplesRecorded = args.BytesRecorded / bytesPerSample;
            int buffersToDisplay = 80_000 / samplesRecorded;
            int offset = (buffersRead % buffersToDisplay) * samplesRecorded;
            
            for (int i = 0; i < samplesRecorded; i++)
                pcmValues[i + offset] = BitConverter.ToInt16(args.Buffer, i * bytesPerSample);

            buffersRead += 1;
        }

        private void PlotInitialize(int pointCount = 8_000 * 10)
        {
            pcmValues = new double[pointCount];
            ucGraph.plt.Clear();
            ucGraph.plt.PlotSignal(pcmValues, sampleRate: 8000, markerSize: 0);
            ucGraph.plt.PlotVLine(0, color: Color.Red, lineWidth: 2);
            ucGraph.plt.PlotHLine(0, color: Color.Black, lineWidth: 1);
            ucGraph.plt.YLabel("Amplitude (%)");
            ucGraph.plt.XLabel("Time (seconds)");
            ucGraph.Render();
        }

        private void AudioMonitorInitialize(int DeviceIndex = 0, int sampleRate = 8000, int bitRate = 16,
                int channels = 1, int bufferMilliseconds = 20, bool start = true)
        {
            if (wvin == null)
            {
                wvin = new NAudio.Wave.WaveInEvent();
                wvin.DeviceNumber = DeviceIndex;
                wvin.WaveFormat = new NAudio.Wave.WaveFormat(sampleRate, bitRate, channels);
                wvin.DataAvailable += OnDataAvailable;
                wvin.BufferMilliseconds = bufferMilliseconds;
                if (start)
                    wvin.StartRecording();
                
            }
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                btnStartStop.StateTracking.Border.Color2 = Color.Red;
                btnStartStop.StateTracking.Border.Color2 = Color.Red;
                btnStartStop.StatePressed.Back.Color1 = Color.Red;
                btnStartStop.StatePressed.Back.Color2 = Color.Red;
                btnStartStop.StatePressed.Border.Color1 = Color.Red;
                btnStartStop.StatePressed.Border.Color2 = Color.Red;
                ucGraph.plt.AxisAuto();
                ucGraph.Render(lowQuality: true);
                btnStartStop.Text = "Stop";

            }
            else
            {
                btnStartStop.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(145)))), ((int)(((byte)(192)))));
                btnStartStop.StateTracking.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(145)))), ((int)(((byte)(192)))));
                btnStartStop.StatePressed.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(145)))), ((int)(((byte)(192)))));
                btnStartStop.StatePressed.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(145)))), ((int)(((byte)(192)))));
                btnStartStop.StatePressed.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(145)))), ((int)(((byte)(192)))));
                btnStartStop.StatePressed.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(145)))), ((int)(((byte)(192)))));
                btnStartStop.Text = "Start";

            }
        }
        bool isRunning = false;
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                if (wvin != null)
                {
                    wvin.StopRecording();
                    wvin = null;
                }
            }
            else
            {
                AudioMonitorInitialize();
            }
            isRunning = !isRunning;
        }
    }
}
