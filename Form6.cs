using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using AmFaceVerify;

namespace ArcsoftFaceTest
{
    public partial class Form6 : Form
    {
        OpenFileDialog openFile = new OpenFileDialog();

        public string personId;            	//联系人的ID

        public string starpath = @"..\..\";//相对路径

        public string tempaddress;         //签到临时图片的路径

        public int check = 0;

        byte[] firstFeature;

        byte[] secondFeature;

        //人脸检测引擎
        IntPtr detectEngine = IntPtr.Zero;

        //人脸识别引擎
        IntPtr regcognizeEngine = IntPtr.Zero;

        //拖拽线程
        private Thread threadMultiExec;

        //构造函数
        public Form6()
        {
            InitializeComponent();
        }

        public Form6(string s, string a)			//读取联系人
        {
            InitializeComponent();
            personId = s;			//所选择的联系人的ID
            tempaddress = a;
        }

        //把图片转成byte[]
        private byte[] getBGR(Bitmap image, ref int width, ref int height, ref int pitch)
        {
            //Bitmap image = new Bitmap(imgPath);

            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat);

            IntPtr ptr = data.Scan0;

            int ptr_len = data.Height * Math.Abs(data.Stride);

            byte[] ptr_bgr = new byte[ptr_len];

            Marshal.Copy(ptr, ptr_bgr, 0, ptr_len);

            width = data.Width;

            height = data.Height;

            pitch = Math.Abs(data.Stride);

            int line = width * 3;

            int bgr_len = line * height;

            byte[] bgr = new byte[bgr_len];

            for (int i = 0; i < height; ++i)
            {
                Array.Copy(ptr_bgr, i * pitch, bgr, i * line, line);
            }

            pitch = line;

            image.UnlockBits(data);

            return bgr;
        }

        //选择第一张照片
        private void button4_Click(object sender, EventArgs e)
        {

                this.pictureBox1.Image = null;

                Image image = new Bitmap(starpath + @"\Picture\" + personId + ".jpg");

                this.pictureBox1.Image = new Bitmap(image);

                image.Dispose();

                firstFeature = detectAndExtractFeature(this.pictureBox1.Image, 1);

        }

        //选择第二张照片
        private void button2_Click(object sender, EventArgs e)
        {

            //openFile.Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.png|所有文件|*.*;";

            //openFile.Multiselect = false;

            //openFile.FileName = "";


            this.pictureBox2.Image = null;

            Image image = new Bitmap(tempaddress);

            this.pictureBox2.Image = new Bitmap(image);

            image.Dispose();

            secondFeature = detectAndExtractFeature(this.pictureBox2.Image, 2);

            //File.Delete(tempaddress);

        }

        //提取识别出的人脸
        public static Bitmap CutFace(Bitmap srcImage, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (srcImage == null)
            {
                return null;
            }

            int w = srcImage.Width;

            int h = srcImage.Height;

            if (StartX >= w || StartY >= h)
            {
                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

                Graphics g = Graphics.FromImage(bmpOut);

                g.DrawImage(srcImage, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);

                g.Dispose();

                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        //获取相似度
        private void button3_Click(object sender, EventArgs e)
        {
            float similar = 0f;

            AFR_FSDK_FACEMODEL localFaceModels = new AFR_FSDK_FACEMODEL();

            IntPtr firstFeaturePtr = Marshal.AllocHGlobal(firstFeature.Length);

            Marshal.Copy(firstFeature, 0, firstFeaturePtr, firstFeature.Length);

            localFaceModels.lFeatureSize = firstFeature.Length;

            localFaceModels.pbFeature = firstFeaturePtr;

            IntPtr secondFeaturePtr = Marshal.AllocHGlobal(secondFeature.Length);

            Marshal.Copy(secondFeature, 0, secondFeaturePtr, secondFeature.Length);

            AFR_FSDK_FACEMODEL localFaceModels2 = new AFR_FSDK_FACEMODEL();

            localFaceModels2.lFeatureSize = secondFeature.Length;

            localFaceModels2.pbFeature = secondFeaturePtr;

            IntPtr firstPtr = Marshal.AllocHGlobal(Marshal.SizeOf(localFaceModels));

            Marshal.StructureToPtr(localFaceModels, firstPtr, false);

            IntPtr secondPtr = Marshal.AllocHGlobal(Marshal.SizeOf(localFaceModels2));

            Marshal.StructureToPtr(localFaceModels2, secondPtr, false);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            int result = AmFaceVerify.AFR_FSDK_FacePairMatching(regcognizeEngine, firstPtr, secondPtr, ref similar);

            stopwatch.Stop();

            setControlText(this.label1, "相似度:" + similar.ToString() + " 耗时:" + stopwatch.ElapsedMilliseconds.ToString() + "ms");

            if (similar > 0.6)
            {

                label6.Text = "签到成功于" + DateTime.Now.ToString();

                DataSet myDataSet = new DataSet();
                myDataSet.ReadXml(starpath + @"\AddressList.xml");

                int ipersonid = Convert.ToInt32(personId);

                for (int num = 13; num > 9; num--)                
                {
                    myDataSet.Tables["PersonList"].Rows[ipersonid - 1][num] = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][num-1];                
                }
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][9] = DateTime.Now.ToString();
                myDataSet.WriteXml(starpath + @"\AddressList.xml");

            }

            else

                label6.Text = "人脸识别与用户图片不符";

            //this.label1.Text = "相似度:" + similar.ToString() + " 耗时:" + stopwatch.ElapsedMilliseconds.ToString() + "ms";

            localFaceModels = new AFR_FSDK_FACEMODEL();

            Marshal.FreeHGlobal(firstFeaturePtr);

            Marshal.FreeHGlobal(secondFeaturePtr);

            Marshal.FreeHGlobal(firstPtr);

            Marshal.FreeHGlobal(secondPtr);

            localFaceModels2 = new AFR_FSDK_FACEMODEL();
        }

        //检测人脸、提取特征
        private byte[] detectAndExtractFeature(Image imageParam, int firstSecondFlg)
        {
            byte[] feature = null;

            try
            {
                Console.WriteLine();

                Console.WriteLine("############### Face Detect Start #########################");

                int width = 0;

                int height = 0;

                int pitch = 0;

                Bitmap bitmap = new Bitmap(imageParam);

                byte[] imageData = getBGR(bitmap, ref width, ref height, ref pitch);

                //GCHandle hObject = GCHandle.Alloc(imageData, GCHandleType.Pinned);

                //IntPtr imageDataPtr = hObject.AddrOfPinnedObject();

                IntPtr imageDataPtr = Marshal.AllocHGlobal(imageData.Length);

                Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);

                ASVLOFFSCREEN offInput = new ASVLOFFSCREEN();

                offInput.u32PixelArrayFormat = 513;

                offInput.ppu8Plane = new IntPtr[4];

                offInput.ppu8Plane[0] = imageDataPtr;

                offInput.i32Width = width;

                offInput.i32Height = height;

                offInput.pi32Pitch = new int[4];

                offInput.pi32Pitch[0] = pitch;

                AFD_FSDK_FACERES faceRes = new AFD_FSDK_FACERES();

                IntPtr offInputPtr = Marshal.AllocHGlobal(Marshal.SizeOf(offInput));

                Marshal.StructureToPtr(offInput, offInputPtr, false);

                IntPtr faceResPtr = Marshal.AllocHGlobal(Marshal.SizeOf(faceRes));

                //Marshal.StructureToPtr(faceRes, faceResPtr, false);

                Console.WriteLine("StartTime:{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));

                Stopwatch watchTime = new Stopwatch();

                watchTime.Start();
                //人脸检测
                int detectResult = AmFaceVerify.AFD_FSDK_StillImageFaceDetection(detectEngine, offInputPtr, ref faceResPtr);

                watchTime.Stop();

                if (firstSecondFlg == 1)
                {
                    setControlText(this.label5, String.Format("检测耗时:{0}ms", watchTime.ElapsedMilliseconds));

                    //this.label5.Text = String.Format("检测耗时:{0}ms", watchTime.ElapsedMilliseconds);
                }
                else if (firstSecondFlg == 2)
                {
                    setControlText(this.label2, String.Format("检测耗时:{0}ms", watchTime.ElapsedMilliseconds));

                    //this.label2.Text = String.Format("检测耗时:{0}ms", watchTime.ElapsedMilliseconds);
                }

                object obj = Marshal.PtrToStructure(faceResPtr, typeof(AFD_FSDK_FACERES));

                faceRes = (AFD_FSDK_FACERES)obj;

                Console.WriteLine("  Face Count:{0}", faceRes.nFace);

                for (int i = 0; i < faceRes.nFace; i++)
                {
                    MRECT rect = (MRECT)Marshal.PtrToStructure(faceRes.rcFace + Marshal.SizeOf(typeof(MRECT)) * i, typeof(MRECT));

                    int orient = (int)Marshal.PtrToStructure(faceRes.lfaceOrient + Marshal.SizeOf(typeof(int)) * i, typeof(int));

                    if (i == 0)
                    {
                        Image image = CutFace(bitmap, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);

                        if (firstSecondFlg == 1)
                        {
                            this.pictureBox3.Image = image;
                        }
                        else if (firstSecondFlg == 2)
                        {
                            this.pictureBox4.Image = image;
                        }
                    }

                    Console.WriteLine("    left:{0} top:{1} right:{2} bottom:{3} orient:{4}", rect.left, rect.top, rect.right, rect.bottom, orient);
                }

                Console.WriteLine("  EndTime:{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));

                Console.WriteLine("############### Face Detect End   #########################");

                if (faceRes.nFace > 0)
                {
                    Console.WriteLine();

                    Console.WriteLine("############### Face Recognition Start #########################");

                    AFR_FSDK_FACEINPUT faceResult = new AFR_FSDK_FACEINPUT();

                    int orient = (int)Marshal.PtrToStructure(faceRes.lfaceOrient, typeof(int));

                    faceResult.lfaceOrient = orient;

                    faceResult.rcFace = new MRECT();

                    MRECT rect = (MRECT)Marshal.PtrToStructure(faceRes.rcFace, typeof(MRECT));

                    faceResult.rcFace = rect;

                    IntPtr faceResultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(faceResult));

                    Marshal.StructureToPtr(faceResult, faceResultPtr, false);

                    AFR_FSDK_FACEMODEL localFaceModels = new AFR_FSDK_FACEMODEL();

                    IntPtr localFaceModelsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(localFaceModels));

                    //Marshal.StructureToPtr(localFaceModels, localFaceModelsPtr, false);

                    watchTime.Start();

                    int extractResult = AmFaceVerify.AFR_FSDK_ExtractFRFeature(regcognizeEngine, offInputPtr, faceResultPtr, localFaceModelsPtr);

                    Marshal.FreeHGlobal(faceResultPtr);

                    Marshal.FreeHGlobal(offInputPtr);

                    watchTime.Stop();

                    if (firstSecondFlg == 1)
                    {
                        setControlText(this.label3, String.Format("抽取特征耗时:{0}ms", watchTime.ElapsedMilliseconds));

                        //this.label3.Text = String.Format("抽取特征耗时:{0}ms", watchTime.ElapsedMilliseconds);
                    }
                    else if (firstSecondFlg == 2)
                    {
                        setControlText(this.label4, String.Format("抽取特征耗时:{0}ms", watchTime.ElapsedMilliseconds));

                        //this.label4.Text = String.Format("抽取特征耗时:{0}ms", watchTime.ElapsedMilliseconds);
                    }

                    object objFeature = Marshal.PtrToStructure(localFaceModelsPtr, typeof(AFR_FSDK_FACEMODEL));

                    Marshal.FreeHGlobal(localFaceModelsPtr);

                    localFaceModels = (AFR_FSDK_FACEMODEL)objFeature;

                    feature = new byte[localFaceModels.lFeatureSize];

                    Marshal.Copy(localFaceModels.pbFeature, feature, 0, localFaceModels.lFeatureSize);

                    localFaceModels = new AFR_FSDK_FACEMODEL();

                    Console.WriteLine("############### Face Recognition End   #########################");

                }

                bitmap.Dispose();

                imageData = null;

                Marshal.FreeHGlobal(imageDataPtr);

                offInput = new ASVLOFFSCREEN();

                faceRes = new AFD_FSDK_FACERES();

                //Marshal.FreeHGlobal(faceResPtr);
            }
            catch (Exception e)
            {
                LogHelper.WriteErrorLog("detect", e.Message + "\n" + e.StackTrace);
            }
            return feature;
        }

        //初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化人脸检测引擎

            int detectSize = 40 * 1024 * 1024;

            IntPtr pMem = Marshal.AllocHGlobal(detectSize);

            //1-1
            //string appId = "4tnYSJ68e8wztSo4Cf7WvbyMZduHwpqtThAEM3obMWbE";

            //1-1
            //string sdkKey = "Cgbaq34izc8PA2Px26x8qqWTQn2P5vxijaWKdUrdCwYT";

            //1-n
            string appId = "J5F2t36cGS73ya4hsNrKDEASouMcGntrRwhx61QPDzJo";

            //1-n
            string sdkKey = "ESo5b4i7wfC5MZHQNvsAH7PBfjwUPF9dT8RAXqup1gYH";

            //人脸检测引擎初始化
            int retCode = AmFaceVerify.AFD_FSDK_InitialFaceEngine(appId, sdkKey, pMem, detectSize, ref detectEngine, 5, 50, 1);

            //获取人脸检测引擎版本
            IntPtr versionPtr = AmFaceVerify.AFD_FSDK_GetVersion(detectEngine);

            AFD_FSDK_Version version = (AFD_FSDK_Version)Marshal.PtrToStructure(versionPtr, typeof(AFD_FSDK_Version));

            Console.WriteLine("lCodebase:{0} lMajor:{1} lMinor:{2} lBuild:{3} Version:{4} BuildDate:{5} CopyRight:{6}", version.lCodebase, version.lMajor, version.lMinor, version.lBuild, Marshal.PtrToStringAnsi(version.Version), Marshal.PtrToStringAnsi(version.BuildDate), Marshal.PtrToStringAnsi(version.CopyRight));

            //Marshal.FreeHGlobal(versionPtr);

            #endregion

            #region 初始化人脸识别引擎

            int recognizeSize = 40 * 1024 * 1024;

            IntPtr pMemDetect = Marshal.AllocHGlobal(recognizeSize);

            //1-1
            //string appIdDetect = "4tnYSJ68e8wztSo4Cf7WvbyMZduHwpqtThAEM3obMWbE";

            //1-1
            //string sdkKeyDetect = "Cgbaq34izc8PA2Px26x8qqWaaBHbPD7wWMcTU6xe8VRo";

            //1-n
            string appIdDetect = "J5F2t36cGS73ya4hsNrKDEASouMcGntrRwhx61QPDzJo";

            //1-n
            string sdkKeyDetect = "ESo5b4i7wfC5MZHQNvsAH7PZ9wix6zmV77BBYSA97FRg";

            //人脸识别引擎初始化
            retCode = AmFaceVerify.AFR_FSDK_InitialEngine(appIdDetect, sdkKeyDetect, pMemDetect, recognizeSize, ref regcognizeEngine);

            //获取人脸识别引擎版本
            IntPtr versionPtrDetect = AmFaceVerify.AFR_FSDK_GetVersion(regcognizeEngine);

            AFR_FSDK_Version versionDetect = (AFR_FSDK_Version)Marshal.PtrToStructure(versionPtrDetect, typeof(AFR_FSDK_Version));

            Console.WriteLine("lCodebase:{0} lMajor:{1} lMinor:{2} lBuild:{3} lFeatureLevel:{4} Version:{5} BuildDate:{6} CopyRight:{7}", versionDetect.lCodebase, versionDetect.lMajor, versionDetect.lMinor, versionDetect.lBuild, versionDetect.lFeatureLevel, Marshal.PtrToStringAnsi(versionDetect.Version), Marshal.PtrToStringAnsi(versionDetect.BuildDate), Marshal.PtrToStringAnsi(versionDetect.CopyRight));

            #endregion
        }

        //拖拽事件
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the data from the DataObject-Container into a string list
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (fileList.Length >= 2)
            {
                this.threadMultiExec = new Thread(new ParameterizedThreadStart(multiCompare));

                this.threadMultiExec.Start(new object[] { fileList });

                this.threadMultiExec.IsBackground = true;
            }

        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the Dataformat of the data can be accepted
            // (we only accept file drops from Explorer, etc.)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Okay
            }
            else
            {
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
            }
        }

        //多线程设置PictureBox的图像
        private void setPictureBoxControlImage(PictureBox control, Bitmap value)
        {
            control.Invoke(new Action<PictureBox, Bitmap>((ct, v) => { ct.Image = v; }), new object[] { control, value });
        }

        //多线程设置控件的文本
        private void setControlText(Control control, string value)
        {
            control.Invoke(new Action<Control, string>((ct, v) => { ct.Text = v; }), new object[] { control, value });
        }

        //比对多个图片
        private void multiCompare(object args)
        {
            object[] objs = args as object[];

            string[] fileList = (string[])objs[0];

            for (int i = 0; i < fileList.Length; i++)
            {

                Image image = Image.FromFile(fileList[i]);

                //this.pictureBox1.Image = null;

                //this.pictureBox1.Image = new Bitmap(image);

                setPictureBoxControlImage(this.pictureBox1, new Bitmap(image));

                firstFeature = detectAndExtractFeature(image, 1);

                image.Dispose();

                if (firstFeature == null)
                {

                    continue;
                }

                if (i + 1 >= fileList.Length)
                {

                    continue;
                }

                Image image2 = Image.FromFile(fileList[++i]);

                //this.pictureBox2.Image = null;

                // this.pictureBox2.Image = new Bitmap(image2);

                setPictureBoxControlImage(this.pictureBox2, new Bitmap(image2));

                secondFeature = detectAndExtractFeature(image2, 2);

                image2.Dispose();

                if (secondFeature == null)
                {

                    continue;
                }

                button3_Click(null, null);

                setControlText(this.label6, "正在处理:" + (i + 1).ToString());

                //label6.Text = "正在处理:" + (i + 1).ToString();

                //this.Update();

                Thread.Sleep(10);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
