using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HttpCodeLib;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BackgrandScanner
{
    public partial class Form1 : Form
    {
        public static int iCount = 0;
        public static int success = 0;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public string ErrorLogger(string Message)
        {
            StreamWriter sw = new StreamWriter("Error.log", true);
            sw.WriteLine(DateTime.Now + "     " + Message);
            sw.Flush();
            sw.Close();
            return "";
        }

        public string DicCount(string Dic)
        {
            try
            {
                StreamReader sr = new StreamReader("setting/" + Dic + ".txt");
                string line;
                List<string> DicList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    DicList.Add(line);
                }
                return Convert.ToString(DicList.Count());
            }
            catch (Exception ex)
            {
                //MessageBox.Show("配置文件加载出错，请检查错误日志： Error.log！");
                ErrorLogger(ex.Message);
                return "";
            }
        }

        public string DicAdd(string Dic, int i)
        {
            try
            {
                StreamReader sr = new StreamReader("setting/" + Dic + ".txt");
                string line;
                List<string> DicList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace("#", "%23");
                    DicList.Add(line);
                }
                return DicList[i];
            }
            catch (Exception ex)
            {

                ErrorLogger(ex.Message);
                return "";
            }
        }

        public int HTTPStatusCode(string URL)
        {
            try
            {
                string url = URL;//请求地址
                string res = string.Empty;//请求结果,请求类型不是图片时有效

                System.Net.CookieContainer cc = new System.Net.CookieContainer();//自动处理Cookie对象
                HttpHelpers helper = new HttpHelpers();//发起请求对象
                HttpItems items = new HttpItems();//请求设置对象
                HttpResults hr = new HttpResults();//请求结果
                items.URL = url; //设置请求地址
                items.Timeout = trackBar1.Value * 1000;
                items.Container = cc;//自动处理Cookie时,每次提交时对cc赋值即可
                hr = helper.GetHtml(items);//发起请求
                res = hr.Html;//得到请求结果
                return (int)hr.StatusCode;
            }
            catch (Exception ex)
            {
                ErrorLogger(ex.Message);
                return 0;
            }
        }

        public void Scanner(object book)
        {
            try
            {

                //Application.DoEvents();


                Book b = (Book)book;
                string URL = b.url;
                string Path = b.path;
                if (URL.EndsWith("/"))
                {
                    URL = URL.Substring(0, URL.Length - 1);
                }
                label5.Text = URL + Path;
                int HTTPStatus = HTTPStatusCode(URL + Path);
                if (HTTPStatus == 200)
                {
                    if (checkBox7.Checked == true)
                    {
                        success++;
                        ListViewItem lvii = new ListViewItem(); //定义listview项目变量
                        lvii.ForeColor = Color.Green;
                        lvii.Text = Convert.ToString(success); //第一格
                        lvii.SubItems.Add(URL + Path); //第二格
                        lvii.SubItems.Add(Convert.ToString(HTTPStatus));
                        this.listView1.Items.Add(lvii); //将数据执行插入
                    }
                }
                else if (HTTPStatus == 403)
                {
                    if (checkBox8.Checked == true)
                    {
                        success++;
                        ListViewItem lvii = new ListViewItem(); //定义listview项目变量
                        lvii.ForeColor = Color.Red;

                        lvii.Text = Convert.ToString(success); //第一格
                        lvii.SubItems.Add(URL + Path); //第二格
                        lvii.SubItems.Add(Convert.ToString(HTTPStatus));
                        this.listView1.Items.Add(lvii); //将数据执行插入
                    }
                }
                else if (HTTPStatus >= 301 && HTTPStatus <= 309)
                {
                    if (checkBox9.Checked == true)
                    {
                        success++;
                        ListViewItem lvii = new ListViewItem(); //定义listview项目变量
                        lvii.Text = Convert.ToString(success); //第一格
                        lvii.SubItems.Add(URL + Path); //第二格
                        lvii.SubItems.Add(Convert.ToString(HTTPStatus));
                        this.listView1.Items.Add(lvii); //将数据执行插入
                    }
                }
                else
                {

                }
                Interlocked.Increment(ref iCount);
                progressBar1.Value = iCount;
                int mission = listBox1.Items.Count - b.i;
                label4.Text = Convert.ToString(mission);
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    label5.Text = "扫描完成...";
                    label4.Text = "0";
                }


            }
            catch (Exception ex)
            {
                ErrorLogger(ex.Message);
                Interlocked.Increment(ref iCount);
                progressBar1.Value = iCount;
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    label5.Text = "扫描完成...";
                }
            }
        }

        public struct Book
        {
            public string url;
            public string path;
            public int i;
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader("url.txt");
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace(" ", "");
                    listBox1.Items.Add(line);
                }
                label2.Text = label4.Text = Convert.ToString(listBox1.Items.Count);

            }
            catch (Exception ex)
            {
                MessageBox.Show("请将链接放入url.txt！");
                ErrorLogger(ex.Message);
                //throw;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ThreadPool.SetMaxThreads(trackBar2.Value, trackBar2.Value);
                progressBar1.Value = 0;
                iCount = 0;
                success = 0;
                int missiontotal = 0;
                listView1.Items.Clear();
                List<string> path = new List<string>();
                if (checkBox1.Checked == true)
                {
                    missiontotal += Convert.ToInt32(DicCount("ASP"));
                    for (int i = 0; i < Convert.ToInt32(DicCount("ASP")); i++)
                    {
                        path.Add(DicAdd("ASP", i));
                    }
                }
                if (checkBox2.Checked == true)
                {
                    missiontotal += Convert.ToInt32(DicCount("ASPX"));
                    for (int i = 0; i < Convert.ToInt32(DicCount("ASPX")); i++)
                    {
                        path.Add(DicAdd("ASPX", i));
                    }
                }
                if (checkBox3.Checked == true)
                {
                    missiontotal += Convert.ToInt32(DicCount("DIR"));
                    for (int i = 0; i < Convert.ToInt32(DicCount("DIR")); i++)
                    {
                        path.Add(DicAdd("DIR", i));
                    }
                }
                if (checkBox4.Checked == true)
                {
                    missiontotal += Convert.ToInt32(DicCount("MDB"));
                    for (int i = 0; i < Convert.ToInt32(DicCount("MDB")); i++)
                    {
                        path.Add(DicAdd("MDB", i));
                    }
                }
                if (checkBox5.Checked == true)
                {
                    missiontotal += Convert.ToInt32(DicCount("PHP"));
                    for (int i = 0; i < Convert.ToInt32(DicCount("PHP")); i++)
                    {
                        path.Add(DicAdd("PHP", i));
                    }
                }
                if (checkBox6.Checked == true)
                {
                    missiontotal += Convert.ToInt32(DicCount("JSP"));
                    for (int i = 0; i < Convert.ToInt32(DicCount("JSP")); i++)
                    {
                        path.Add(DicAdd("JSP", i));
                    }
                }
                progressBar1.Minimum = 0;
                progressBar1.Maximum = missiontotal * listBox1.Items.Count;


                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    for (int a = 0; a < missiontotal; a++)
                    {

                        Book book = new Book();
                        book.path = path[a];
                        book.url = Convert.ToString(listBox1.Items[i]);
                        book.i = i;
                        //Thread thread = new Thread(new ParameterizedThreadStart(Scanner));
                        //thread.Start(book);
                        ThreadPool.QueueUserWorkItem(Scanner, book);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger(ex.Message);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            checkBox1.Text = "ASP:" + DicCount("ASP");
            checkBox2.Text = "ASPX:" + DicCount("ASPX");
            checkBox3.Text = "DIR:" + DicCount("DIR");
            checkBox4.Text = "MDB:" + DicCount("MDB");
            checkBox5.Text = "PHP:" + DicCount("PHP");
            checkBox6.Text = "JSP:" + DicCount("JSP");
            textBox1.Text = Convert.ToString(trackBar1.Value);
            textBox2.Text = Convert.ToString(trackBar2.Value);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = Convert.ToString(trackBar1.Value);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = Convert.ToString(trackBar2.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button3.Text = Convert.ToString(progressBar1.Value);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(listView1.SelectedItems[0].SubItems[1].Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int count = listView1.Items.Count; //计算任务数量

                StreamWriter _wstream = new StreamWriter("Scan_ok.csv"); //写入的文件名
                _wstream.WriteLine("网站地址,HTTP状态");
                for (int i = 0; i < count; i++) //循序写入文件
                {
                    string data = listView1.Items[i].SubItems[1].Text + "," + listView1.Items[i].SubItems[2].Text; //把任务结果赋值给data

                    _wstream.Write(data); //写入
                    _wstream.WriteLine(); //换行

                }
                _wstream.Flush();
                _wstream.Close();
                MessageBox.Show("成功保存到Scan_ok.csv！");
            }
            catch (Exception ex)
            {
                ErrorLogger(ex.Message);
                MessageBox.Show("保存出错，请检查程序错误日志Error.log！");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.wtfsec.org");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string value = "";
            if (InputBox("添加链接", "请输入链接：", ref value) == DialogResult.OK)
            {
                listBox1.Items.Add(value);
            }
        }
    }
}
