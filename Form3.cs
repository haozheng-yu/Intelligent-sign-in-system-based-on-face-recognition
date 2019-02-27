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
using System.Windows.Media.Imaging;

namespace ArcsoftFaceTest
{
    public partial class Form3 : Form
    {
        string personId;            	//联系人的ID
        bool insertdate = false;    //用于判断是更新联系人还是新建联系人，为“true”是新建联系人
        string starpath = @"..\..\";//相对路径
        string picaddress;
        bool pictureboxclick = false;
        
        OpenFileDialog openFileDialog1 = new OpenFileDialog();

        public Form3()
        {
            InitializeComponent();
        }
        public Form3(string s)			//读取联系人
        {
            InitializeComponent();
            personId = s;			//所选择的联系人的ID
            ReadList();				//读取分组
            ReadPerson();			//读取联系人
            ShowPicture(personId);		//读取照片
        }

        public Form3(bool b)    			//当b为True时新建联系人
        {
            InitializeComponent();
            ReadList();				//读取分组
            insertdate = b;			//是否新建联系人
            button3.Enabled = false;
        }

        private void ReadList()   					//读取所有分组
        {
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(starpath + @"List.xml");
            XmlNodeList xnl = xmldocument.SelectSingleNode("List").ChildNodes;
            foreach (XmlNode xd in xnl)
            {
                XmlElement xe = (XmlElement)xd;
                comboBox1.Items.Add(xe.InnerText);	//加载分组
            }
        }

        private void ReadPerson()
        {
            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(starpath + @"\AddressList.xml");
            int ipersonid = Convert.ToInt32(personId);
            textBox1.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][1].ToString();	//读取姓名
            /*读取出生时间*/
            dateTimePicker1.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][2].ToString();
            textBox2.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][3].ToString();	//电话
            textBox3.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][4].ToString();	//Email
            textBox4.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][5].ToString();	//QQ
            comboBox2.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][6].ToString();//性别
            textBox5.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][7].ToString();	//地址
            richTextBox1.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][8].ToString();	//备注
            comboBox1.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][14].ToString();//所在分组
            label10.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][9].ToString();
            label11.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][10].ToString();
            label12.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][11].ToString();
            label13.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][12].ToString();
            label14.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][13].ToString();
        }

        private void ShowPicture(string userId)
        {
            if (!Directory.Exists(starpath + @"\Picture"))				//创建存放图片的文件夹
            {
                Directory.CreateDirectory(starpath + @"\Picture");
            }
            string[] files = Directory.GetFiles(starpath + @"\Picture");	//读取Picture文件夹中的所有图片
            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);			//得到文件名+后缀名
                if (filename == userId + ".jpg")
                {
                    FileStream fs = new FileStream(starpath + @"\Picture\" + userId + ".jpg", FileMode.Open, FileAccess.Read);
                    pictureBox1.Image = Image.FromStream(fs);
                    fs.Close();
                }

            }
        }

        private void SavePicture(string userId)
        {
            if (!Directory.Exists(starpath + @"\Picture"))				//如果没存放图片的文件夹则创建
            { Directory.CreateDirectory(starpath + @"\Picture"); }
            if (File.Exists(starpath + @"\Picture\" + personId + ".jpg"))	//如果有此图片则删除
            { File.Delete(starpath + @"\Picture\" + personId + ".jpg"); }
            if (File.Exists(starpath + @"\Picture\" + personId + ".TXT"))	//如果有此图片说明则删除
            { File.Delete(starpath + @"\Picture\" + personId + ".TXT"); }
            try
            {
                pictureBox1.Image.Save(starpath + @"\Picture\" + personId + ".jpg",
                System.Drawing.Imaging.ImageFormat.Jpeg);		//保存图片
                pictureBox1.Image.Dispose();//释放pictureBox占用的tempsaveimage，否则该进程将持续占用

            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }

        }

        public void SaveText(string userId)
        {
            /*创建一个文件流，用以写入或者创建一个StreamWriter*/
            FileStream fs = new FileStream(starpath + @"\Picture\" + personId + ".TXT",
            FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Flush();
            /*使用StreamWriter往文件中写入内容*/
            sw.BaseStream.Seek(0, SeekOrigin.Begin);
            sw.Write(richTextBox1.Text);							//把richTextBox1中的内容写入文件
            /*关闭此文件*/
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(starpath + @"\AddressList.xml");
            if (insertdate)         						//如果插入新数据
            {

                DataRow myRow = myDataSet.Tables["PersonList"].NewRow();
                Form1.allPersonNum++;					//记录数加1
                personId = Form1.allPersonNum.ToString();

                myRow["ID"] = Form1.allPersonNum.ToString();
                myRow["姓名"] = textBox1.Text.Trim();
                myRow["出生时间"] = dateTimePicker1.Text.Trim();
                myRow["电话"] = textBox2.Text.Trim();
                myRow["Email"] = textBox3.Text.Trim();
                myRow["QQ"] = textBox4.Text.Trim();
                myRow["性别"] = comboBox2.Text.Trim();
                myRow["地址"] = textBox5.Text.Trim();
                myRow["备注"] = richTextBox1.Text.Trim();
                myRow["所在分组"] = comboBox1.Text;
                myDataSet.Tables["PersonList"].Rows.Add(myRow);
                button3.Enabled = false;
            }
            else                						//更新数据
            {
                pictureBox1.Enabled = false;
                int ipersonid = Convert.ToInt32(personId);
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][1] = textBox1.Text.Trim();	//更新姓名
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][2] = dateTimePicker1.Text;	//出生时间
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][3] = textBox2.Text.Trim();	//电话
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][4] = textBox3.Text.Trim();	//Email
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][5] = textBox4.Text.Trim();	//QQ
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][6] = comboBox2.Text;	//性别
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][7] = textBox5.Text.Trim();	//地址
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][8] = richTextBox1.Text.Trim();	//备注
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][14] = comboBox1.Text.Trim();//所在分组
            }
            myDataSet.WriteXml(starpath + @"\AddressList.xml");
            if (pictureboxclick)
            {
                SavePicture(personId);
            }
            MessageBox.Show("保存成功!");
            this.Close();								//关闭窗口
            //File.Delete(picaddress);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image.Dispose();
                Form4 form4 = new Form4();
                form4.ShowDialog(); 
                if (form4.end == 1)
                {
                    pictureboxclick = true;
                    Image image = new Bitmap(form4.picaddress);
                    picaddress = form4.picaddress;
                    pictureBox1.Image = image;
                    pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                    image = null; 
                }

            }
            finally
            { openFileDialog1.Dispose(); }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            //textBox6.ReadOnly = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(personId, starpath);
            form5.ShowDialog();
           
        }
    }
}
