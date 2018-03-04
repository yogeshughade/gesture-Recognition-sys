using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
//Reference path for the following assemblies Aforge
using AForge.Video;
using AForge.Video.DirectShow;

namespace GestureRecognition
{
    public partial class MainForm : Form
    {
        //list of video devices
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;
        Bitmap newFrame;

        // arraylists for storing the border coordinates
        ArrayList borderX = new ArrayList();
        ArrayList borderY = new ArrayList();

        // features used for gesture recognition
        double objectArea, objectCompactness, objectPx, objectPy;
        double xCOG, yCOG, borderLength;
        double xMax, xMin, yMax, yMin; 

        // matrix containing the processing image
        byte[,] procImage;
        byte[,] procImage1 = new byte[480, 640];
        byte[,] procImage2 = new byte[480, 640];

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                // disable stop button
                stopButton.Enabled = false;

                // check if no devices were found
                if (videoDevices.Count == 0)
                {
                    throw new Exception();
                }

                // list for devices
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    string cameraName = i + " : " + videoDevices[i].Name;

                    cameraComboBox.Items.Add(cameraName);
                }
                cameraComboBox.SelectedIndex = 0;
            }
            catch
            {
                // don't let the aplication start
                startButton.Enabled = false;
                cameraComboBox.Items.Add("No cameras found");

                cameraComboBox.SelectedIndex = 0;
                cameraComboBox.Enabled = false;
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            newFrame = (Bitmap) eventArgs.Frame.Clone();

            // process the frame
            ProcessFrame();
       
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource != null)
            {
                // stop the video source
                videoSource.SignalToStop();
            }
        }

        private void ProcessFrame()
        {
            // get the threshold for binarization
            int threshold = Convert.ToInt16(thresholdNumeric.Value);

            // make a copy of the frame for processing
            Bitmap temp = new Bitmap(newFrame);

            // use bitmap data to acces image pixels
            // returned format is BGR
            BitmapData bmData = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // get stride
            int stride = bmData.Stride;

            // get the adress of first pixel
            IntPtr Scan0 = bmData.Scan0;

            // it's unsafe when using pointers
            unsafe
            {
                int width = temp.Width;
                byte* p = (byte*)Scan0;
                int nOffset = stride - temp.Width * 3;

                int finished;

                // binarize image
                // start a number of threads equal to sistem processor count
                Task[] t = new Task[Environment.ProcessorCount];
                for (int x = 0; x < Environment.ProcessorCount; x++)
                {
                    int xTemp = x;
                    byte* pTemp = p;
                    t[x] = Task.Factory.StartNew(() => BinarizeImage(pTemp, xTemp, width, threshold));
                    p += stride;
                }
                for (int x = Environment.ProcessorCount; x < temp.Height; x++)
                {
                    int xTemp = x;
                    byte* pTemp = p;
                    // wait for any task to finish
                    finished = Task.WaitAny(t);
                    t[finished] = Task.Factory.StartNew(() => BinarizeImage(pTemp, xTemp, width, threshold));

                    // add stride to go to next line
                    p += stride;
                }
                //wait for all the threads to complete before moving forward
                Task.WaitAll(t);

                // filter some of the noise and border de image
                procImage2[0, 0] = procImage2[temp.Height - 1, 0] = procImage2[0, temp.Width - 1] = procImage2[temp.Height - 1, temp.Width - 1] = 0; 
                
                // Paralel filter, starting a number of threads equal to number of logical processor on the sistem
                // reusing the t variable from above
                for (int x = 0; x < Environment.ProcessorCount; x++)
                {
                    procImage2[x + 1, 0] = procImage2[x + 1, temp.Width - 1] = 0;
                    int xTemp = x;
                    t[x] = Task.Factory.StartNew(() => filter(xTemp + 1, width));
                }
               
                for (int x = Environment.ProcessorCount + 1; x < temp.Height - 1; x++)
                {
                    procImage2[x, 0] = procImage2[x, temp.Width - 1] = 0;
                    finished = Task.WaitAny(t);
                    int xTemp = x;
                    t[finished] = Task.Factory.StartNew(() => filter(xTemp, width));

                }
                Task.WaitAll(t);


                // copy the image back
                p = (byte*)Scan0;

                for (int x = 0; x < temp.Height; x++)
                {
                    for (int y = 0; y < temp.Width; y++)
                    {
                        p[0] = p[1] = p[2] = procImage2[x, y];

                        // go to next pixel
                        p += 3;
                    }
                    // compensate for the stride offset
                    p += nOffset;
                }

            }
            temp.UnlockBits(bmData);

            //display the image
            webcamImage.Image = temp.Clone() as Image;
        }

        unsafe private void BinarizeImage(byte* q, int x, int width,int threshold)
        {
            for (int y = 0; y < width; y++)
            {
                //B = p[0];
                //G = p[1];
                //R = p[2];

                // use only green channel for binarization, got best results
                if (q[1] > threshold)
                    procImage1[x, y] = 0;
                else
                    procImage1[x, y] = 255;

                // go to next pixel
                q += 3;
            }
        }

        // two threshold filter
        private void filter(int x, int width)
        {
            for (int y = 1; y < width - 1; y++)
            {
                procImage2[0, y] = procImage2[480 - 1, y] = 0;
                if (procImage1[x - 1, y - 1] + procImage1[x - 1, y] + procImage1[x - 1, y + 1] + procImage1[x, y - 1] + procImage1[x, y + 1] +
                    procImage1[x + 1, y - 1] + procImage1[x + 1, y] + procImage1[x + 1, y + 1] <= 510)
                {
                    procImage2[x, y] = 0;
                }
                if (procImage1[x - 1, y - 1] + procImage1[x - 1, y] + procImage1[x - 1, y + 1] + procImage1[x, y - 1] + procImage1[x, y + 1] +
                    procImage1[x + 1, y - 1] + procImage1[x + 1, y] + procImage1[x + 1, y + 1] >= 1785)
                {
                    procImage2[x, y] = 255;
                }
            }
        }

        private void CaptureAndProcces()
        {
            procImage = new byte[480, 640];
            // list of pixel positions for counter clockwise search
            int[,] position = new int[8, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };
            byte indexPos;
            int count;
            bool stopVar;

            // clear the list of border points
            borderX.Clear();
            borderY.Clear();

            // wait for image to be availabe and make a copy of the frame for processing
            while (webcamImage.Image == null);
            
            // get the frame to be captured
            Bitmap temp = new Bitmap(webcamImage.Image as Bitmap);
            BitmapData bmData = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // get stride
            int stride = bmData.Stride;
            // get the adress of first pixel
            IntPtr Scan0 = bmData.Scan0;

            // copy the information from the image to the helper matrix
            unsafe
            {
                byte* p = (byte*)Scan0;
                int nOffset = stride - temp.Width * 3;

                for (int x = 0; x < temp.Height; x++)
                {
                    for (int y = 0; y < temp.Width; y++)
                    {
                        procImage[x, y] = p[0];

                        // go to next pixel
                        p += 3;
                    }
                    // compensate for the stride offset
                    p += nOffset;
                }
            }

            stopVar = false;
            // get the image border
            // we start from 50 to lose some noise from the margins
            // and assume only one object in picture
            for (int i = 50; i < temp.Height; i++)
            {
                for (int j = 50; j < temp.Width; j++)
                {
                    if (procImage[i, j] == 255)
                    {
                        borderX.Add(i);
                        borderY.Add(j);
                        stopVar = true;
                        break;
                    }
                }
                if (stopVar)
                    break;
            }

            // start from left highside corner
            indexPos = 0;
            count = 0;
            
            // check if any object is present
            if (stopVar == true)
            {
                // get the border of the object
                do
                {
                    // search in counter clock wise direction for another pixel
                    while (procImage[(int)borderX[count] + position[indexPos % 8, 0], (int)borderY[count] + position[indexPos % 8, 1]] != 255 && indexPos < 18)
                    {
                        indexPos++;
                    }
                    // if single pixel detected maybe message
                    if (indexPos >= 18)
                        break;

                    // add the pixel to the list
                    count++;
                    borderX.Add((int)borderX[count - 1] + position[indexPos % 8, 0]);
                    borderY.Add((int)borderY[count - 1] + position[indexPos % 8, 1]);

                    // find the next counter clockwise position after the butlast pixel
                    if ((int)borderX[count - 1] - (int)borderX[count] == -1 && (int)borderY[count - 1] - (int)borderY[count] == 0)
                    {
                        indexPos = 0;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == -1 && (int)borderY[count - 1] - (int)borderY[count] == -1)
                    {
                        indexPos = 1;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == 0 && (int)borderY[count - 1] - (int)borderY[count] == -1)
                    {
                        indexPos = 2;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == 1 && (int)borderY[count - 1] - (int)borderY[count] == -1)
                    {
                        indexPos = 3;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == 1 && (int)borderY[count - 1] - (int)borderY[count] == 0)
                    {
                        indexPos = 4;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == 1 && (int)borderY[count - 1] - (int)borderY[count] == 1)
                    {
                        indexPos = 5;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == 0 && (int)borderY[count - 1] - (int)borderY[count] == 1)
                    {
                        indexPos = 6;
                    }
                    else if ((int)borderX[count - 1] - (int)borderX[count] == -1 && (int)borderY[count - 1] - (int)borderY[count] == 1)
                    {
                        indexPos = 7;
                    }
                    if (count > 3000)
                    {
                        break;
                    }
                } while (((int)borderX[0] != (int)borderX[count]) || ((int)borderY[0] != (int)borderY[count]));
            }

            // check if the object found is too big or too small
            if (count > 100 && count <= 3000)
            {

                // remove last item from border list because it's useless
                borderX.RemoveAt(count);
                borderY.RemoveAt(count);

                textBox.AppendText("Border count=" + count.ToString() + "\n");

                // initialize variables used for feature extraction
                borderLength = 0;
                objectArea = 0;
                xCOG = 0;
                yCOG = 0;
                xMax = 0;
                xMin = temp.Height;
                yMax = 0;
                yMin = temp.Width;

                // now we process the list and compute the features
                Parallel.Invoke(() => GetFeatureSet1(count), () => GetFeatureSet2(count));

                // get the final features
                objectArea /= 2;
                xCOG /= 6 * objectArea;
                yCOG /= 6 * objectArea;
                objectPx = (xMax - xCOG) / (xCOG - xMin);
                objectPy = (yMax - yCOG) / (yCOG - yMin);
                objectCompactness = 4 * Math.PI * objectArea / (borderLength * borderLength);
                objectArea /= temp.Height * temp.Width;

                // print the result
                textBox.AppendText("Compactness=" + objectCompactness.ToString() + "\n");
                textBox.AppendText("Area=" + objectArea.ToString() + "\n");
                textBox.AppendText("Px=" + objectPx.ToString() + "   Py=" + objectPy.ToString() + "\n");
            }
            else
                textBox.AppendText("Object extraction error\n");

            // copy the processed image back
            unsafe
            {
                int width = temp.Width;
                byte* p = (byte*)Scan0;
                int nOffset = stride - temp.Width * 3;

                int finished;
                Task[] t = new Task[Environment.ProcessorCount];
                for (int x = 0; x < Environment.ProcessorCount; x++)
                {
                    int xTemp = x;
                    byte* pTemp = p;
                    t[x] = Task.Factory.StartNew(() => CopyImageBack(pTemp,xTemp,width) );
                    p += stride;
                }
                for (int x = Environment.ProcessorCount; x < temp.Height; x++)
                {
                    int xTemp = x;
                    byte* pTemp = p;
                    finished = Task.WaitAny(t);
                    t[finished] = Task.Factory.StartNew(() => CopyImageBack(pTemp, xTemp, width) );
 
                    // add stride to go to next line
                    p += stride;
                }
                Task.WaitAll(t);
            }
            temp.UnlockBits(bmData);
            capturedImage.Image = temp.Clone() as Image;
        }

        unsafe private void CopyImageBack(byte* q,int x, int width)
        {
            for (int y = 0; y < width; y++)
            {
                if (procImage[x, y] == 128)
                {
                    q[1] = 220;
                    q[0] = q[2] = 0;
                }
                else
                {
                    q[0] = q[1] = q[2] = procImage[x, y];
                }

                // go to next pixel
                q += 3;
            }
        }

        // compute xMax, xMin, border length, area and change border colour
        private void GetFeatureSet1(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // compute xMax, xMin
                if ((int)borderX[i] > xMax)
                    xMax = (int)borderX[i];
                if ((int)borderX[i] < xMin)
                    xMin = (int)borderX[i];
                if (i == 0)
                {
                    // border length
                    borderLength += Math.Sqrt(((int)borderX[count - 1] - (int)borderX[i]) * ((int)borderX[count - 1] - (int)borderX[i])
                        + ((int)borderY[count - 1] - (int)borderY[i]) * ((int)borderY[count - 1] - (int)borderY[i]));
                    // area
                    objectArea += (int)borderX[count - 1] * (int)borderY[i] - (int)borderX[i] * (int)borderY[count - 1];
                }
                else
                {
                    // border length
                    borderLength += Math.Sqrt(((int)borderX[i - 1] - (int)borderX[i]) * ((int)borderX[i - 1] - (int)borderX[i])
                        + ((int)borderY[i - 1] - (int)borderY[i]) * ((int)borderY[i - 1] - (int)borderY[i]));
                    // area
                    objectArea += (int)borderX[i - 1] * (int)borderY[i] - (int)borderX[i] * (int)borderY[i - 1];
                }
                // mark the border with green color
                procImage[(int)borderX[i], (int)borderY[i]] = 128;
            }
        }

        // compute yMax, yMin, xCOG, yCOG (center of gravity)
        private void GetFeatureSet2(int count)
        {
            // compute yMax, yMin
            for (int i = 0; i < count; i++)
            {
                if ((int)borderY[i] > yMax)
                    yMax = (int)borderY[i];
                if ((int)borderY[i] < yMin)
                    yMin = (int)borderY[i];

                if (i == 0)
                {
                    // xCOG and yCOG
                    xCOG += ((int)borderX[count - 1] * (int)borderY[i] - (int)borderX[i] * (int)borderY[count - 1]) * ((int)borderX[count - 1] + (int)borderX[i]);
                    yCOG += ((int)borderX[count - 1] * (int)borderY[i] - (int)borderX[i] * (int)borderY[count - 1]) * ((int)borderY[count - 1] + (int)borderY[i]);
                }
                else
                {
                    // xCOG and yCOG
                    xCOG += ((int)borderX[i - 1] * (int)borderY[i] - (int)borderX[i] * (int)borderY[i - 1]) * ((int)borderX[i - 1] + (int)borderX[i]);
                    yCOG += ((int)borderX[i - 1] * (int)borderY[i] - (int)borderX[i] * (int)borderY[i - 1]) * ((int)borderY[i - 1] + (int)borderY[i]);
                }
            }
        }

        private void GetGesture()
        {
            recognizedGesture.Clear();
            // the conditions for recognizing the three gestures
            if (objectCompactness < 0.25)
                recognizedGesture.AppendText("Stop\n");
            else if (objectPy > 1.4)
                recognizedGesture.AppendText("Right\n");
            else if (objectPy < 0.75)
                recognizedGesture.AppendText("Left\n");
            else
                recognizedGesture.AppendText("No gesture");
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            //VideoCapabilities[] videoCap;
            // create video source
            videoSource = new VideoCaptureDevice(videoDevices[cameraComboBox.SelectedIndex].MonikerString);
            videoSource.DesiredFrameRate = 5;
            videoSource.DesiredFrameSize = new Size(480, 640);

            //videoCap = videoSource.VideoCapabilities;
            //debugText.Text = videoCap[0].FrameSize.ToString();

            // set NewFrame event handler
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

            // start the video source
            videoSource.Start();

            // enable/disable buttons
            startButton.Enabled = false;
            stopButton.Enabled = true;
            timer1.Enabled = true;

            //textBox.AppendText(Environment.ProcessorCount.ToString());
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            // signal to stop
            videoSource.SignalToStop();

            //enable/disable buttons
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            CaptureAndProcces();
            GetGesture();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stopButton.Enabled == true)
            {
                CaptureAndProcces();
                GetGesture();
            }
        }


    }
}
