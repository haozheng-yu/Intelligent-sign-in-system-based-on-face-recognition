using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using AForge;
using AForge.Video.DirectShow;
using AForge.Video;
using AForge.Controls;
using AForge.Imaging; 

namespace ArcsoftFaceTest
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        public Form5(string s, string p)
        {
            InitializeComponent();
            personId = s;			//所选择的联系人的ID
            starpath = p;
            button3.Enabled = false;
        }

        public FilterInfoCollection USE_Webcams = null;
        public VideoCaptureDevice cam = null;
        public VideoSourcePlayer videoSource = null;
        public int end = 0; //检测摄像是否结束
        public string picaddress;//图片地址
        public string personId;
        public string starpath;

        private void Form5_Load(object sender, EventArgs e)
        {
            try
            {
                ///---实例化对象  
                USE_Webcams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                ///---摄像头数量大于0  
                if (USE_Webcams.Count > 0)
                {
                    ///---禁用按钮  
                    this.button1.Enabled = true;
                    ///---实例化对象  
                    cam = new VideoCaptureDevice(USE_Webcams[0].MonikerString);
                    videoSource = new VideoSourcePlayer();
                    videoSource.VideoSource = cam;
                    videoSource.Start();

                    ///---绑定事件  
                    cam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                }
                else
                {
                    ///--没有摄像头  
                    this.button1.Enabled = false;
                    ///---提示没有摄像头  
                    MessageBox.Show("没有摄像头外设");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void Cam_NewFrame(object obj, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            ///---throw new NotImplementedException();  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text == "开始")
                {
                    ///--  
                    button1.Text = "停止";
                    button3.Enabled = true;
                    ///---启动摄像头  
                    //                  cam.Start();
                    videoSource.Start();

                }
                else
                {
                    ///--设置按钮提示字样  
                    button1.Text = "开始";
                    ///--停止摄像头捕获图像  
                    cam.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button1.Text == "停止")
            {
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                    videoSource.GetCurrentVideoFrame().GetHbitmap(),
                                    IntPtr.Zero,
                                    Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());
                PngBitmapEncoder pE = new PngBitmapEncoder();
                pE.Frames.Add(BitmapFrame.Create(bitmapSource));
                string picName = GetImagePath() + "\\" + "tempcheckimage" + ".jpg";
                picaddress = picName;
                label1.Text = "储存成功";
                //tempaddress = picName;
                if (File.Exists(picName))
                {
                    File.Delete(picName);
                }
                using (Stream stream = File.Create(picName))
                {
                    pE.Save(stream);
                    stream.Close();
                }

                //拍照完成后关摄像头并刷新同时关窗体
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
                end = 1;
                Form6 form6 = new Form6(personId, picaddress);
                starpath = form6.starpath;
                form6.ShowDialog();
            }

        }

        private string GetImagePath()
        {
            string personImgPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
                         + Path.DirectorySeparatorChar.ToString() + "PersonImg";
            if (!Directory.Exists(personImgPath))
            {
                Directory.CreateDirectory(personImgPath);
            }

            return personImgPath;
        }

    }
}
