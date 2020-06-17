using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace jiangyu
{
    public partial class 降雨侵蚀力计算软件 : Form
    {
        public 降雨侵蚀力计算软件()
        {
            InitializeComponent();
        }
        private String fileName;
        private List<String> jyList = new List<string>();
        private String saveName;
        private int sumjy = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                this.fileName = openFileDialog1.FileName;
            }
            
        }
        public void WriteTxtAdd(String path,String txt)
        {
            FileStream fs = new FileStream(path, FileMode.Append,FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(txt);
            sw.Close();
            fs.Close();
        }

        public void WriteTxt(String path, String txt)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(txt);
            sw.Close();
            fs.Close();
        }

        private void yearb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && (e.KeyChar != 8) && (e.KeyChar != 46))
                e.Handled = true;
        }

        private void yeare_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && (e.KeyChar != 8) && (e.KeyChar != 46))
                e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && (e.KeyChar != 8) && (e.KeyChar != 46))
                e.Handled = true;
            if (e.KeyChar == 46)                       //小数点
            {
                if (qsyl.Text.Length <= 0)
                    e.Handled = true;           //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(qsyl.Text, out oldf);
                    b2 = float.TryParse(qsyl.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }
        /// <summary>
        /// 年份 年降雨量 24个半月降雨量 
        /// </summary>
        private double[,] yjy; //
     
        /// <summary>
        /// 年侵蚀雨量 24个半月侵蚀雨量 年侵蚀降雨天数 24半月侵蚀降雨天数
        /// </summary>
        private double[,] qsjy; //  
        /// <summary>
        /// 站点编号
        /// </summary>
        private String zdbh = ""; //站点编号
        private int yearnum = 0;
        private double[,] jyqsl;
        private double a = 0.3101;
        private double b = 1.7265;
        private void button2_Click(object sender, EventArgs e)
        {
            //读文件
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            string line;
            //第一行略过
            line = sr.ReadLine();
            
            yearnum = int.Parse(yeare.Text) - int.Parse(yearb.Text) + 1; //总年数
            yjy = new double[yearnum, 25];
            qsjy = new double[yearnum, 50];
            jyqsl =new double[yearnum, 25];

            while ((line = sr.ReadLine()) != null)
            {

                String path = this.saveName + "/大于10mm的数据" + ".txt";
                String[] data = line.Split('\t');

                if (zdbh == "")
                    zdbh = data[0];
                else
                {
                    if (zdbh != data[0])
                    {
                        PrintoutFile();
                        QslOut();
                        ZsQslOut();
                        GjQslOut();
                        zdbh = data[0];
                        WriteTxtAdd(path, line+"\n");
                        yjy = new double[yearnum, 25];
                        qsjy = new double[yearnum, 50];
                        
                    }
                }
                //int beginyear = int.Parse(yearb.Text);
                //去除规定年份外的数据
                if (int.Parse(data[1]) < int.Parse(yearb.Text)) continue;
                if (int.Parse(data[1]) > int.Parse(yeare.Text)) continue;

                yjy[int.Parse(data[1]) - int.Parse(yearb.Text), 0] += double.Parse(data[4]);

                if (double.Parse(data[4]) >= double.Parse(qsyl.Text))
                {

                    qsjy[int.Parse(data[1]) - int.Parse(yearb.Text), 0] += double.Parse(data[4]);
                }
                int index = (int.Parse(data[2]) - 1) * 2 + 1;
                if (int.Parse(data[3]) > 15)
                    index += 1;
                if (double.Parse(data[4]) >= double.Parse(qsyl.Text))
                {
                    WriteTxtAdd(path, line + "\n");
                    qsjy[int.Parse(data[1]) - int.Parse(yearb.Text), index] += double.Parse(data[4]);
                    qsjy[int.Parse(data[1]) - int.Parse(yearb.Text), 25]+=1;
                    qsjy[int.Parse(data[1]) - int.Parse(yearb.Text), 25+index-1]+=1;
                }
                yjy[int.Parse(data[1]) - int.Parse(yearb.Text), index] += double.Parse(data[4]);

            }
            PrintoutFile();
            QslOut();
            ZsQslOut();
            GjQslOut();
            //double rylp = rylsum / ryln;
            //double rylb = 0.6243 + (27.346 / rylp);
            //double ryla = 21.239 * Math.Pow(rylb, -7.3967);
            //WriteTxtAdd(this.saveName + "/日雨量(自定义).txt", ryla.ToString()+"\t"+rylb+"\t"+rylp + "\n");
            //rby = rby / yearnum;            
            //for (int i = 0; i < yjy.GetLength(0); i++) {
            //    String outString = (i + int.Parse(yearb.Text)).ToString() + "\t"+yjy[i, 0].ToString() + "\t";
            //    String outqsString = (i + int.Parse(yearb.Text)).ToString() + "\t" + qsjy[i, 0].ToString() + "\t";
            //    for (int j = 1; j < 25; j++)
            //    {
            //        outString += yjy[i, j].ToString() + "\t";
            //        outqsString += qsjy[i, j].ToString() + "\t";
            //    }

            //    outqsString += rby.ToString() + "\t" +rby/qsjy[i,0]+ "\t";
            //    WriteTxtAdd(this.saveName+"/降雨量.txt", outString + "\n");
            //    WriteTxtAdd(this.saveName + "/侵蚀降雨量.txt", outqsString + "\n");
            //}
            MessageBox.Show("计算完成");
        }
        /// <summary>
        /// 计算降雨侵蚀力并输出
        /// </summary>
        private void QslOut()
        {
            jyqsl = new double[yearnum, 25];
            for (int i = 0; i < yearnum; i++)
            {
                for (int j = 1; j < 25; j++)
                {
                    if (j > 4 && j < 10)
                        a = 0.3937;
                    else
                        a = 0.3101;
                    b = 1.7265;
                    double qsl = Math.Round(a * Math.Pow(Convert.ToDouble(qsjy[i, j]), b),4);
                    jyqsl[i, j] = qsl;
                    jyqsl[i, 0] += qsl;
                }
            }
            PrintQslFile();
        }
        private void ZsQslOut()
        {
            jyqsl = new double[yearnum, 25];
            double y = 0;
            double d = 0;
            for (int i = 0; i < yearnum; i++)
            {
                y+= qsjy[i, 0];
                d += qsjy[i, 25];
            }
            double py = y / yearnum;
            double pd = y / d;
            b = 0.8363 + 18.144 / pd + 24.455 / py;
            a=21.586 * Math.Pow(b, -7.1891);
            for (int i = 0; i < yearnum; i++)
            {
                for (int j = 1; j < 25; j++)
                {
                   
                    double qsl = Math.Round(a * Math.Pow(Convert.ToDouble(qsjy[i, j]), b), 4);
                    jyqsl[i, j] = qsl;
                    jyqsl[i, 0] += qsl;
                }
            }
            PrintZsQslFile();
        }
        private void GjQslOut()
        {
            jyqsl = new double[yearnum, 25];
            double y = 0;
            double d = 0;
            for (int i = 0; i < yearnum; i++)
            {
                y += qsjy[i, 0];
                d += qsjy[i, 25];
            }
            double py = y / yearnum;
            double pd = y / d;
            b = 0.6243 + 27.346 / pd;
            a = 21.239 * Math.Pow(b, -7.3967);
            for (int i = 0; i < yearnum; i++)
            {
                for (int j = 1; j < 25; j++)
                {

                    double qsl = Math.Round(a * Math.Pow(Convert.ToDouble(qsjy[i, j]), b), 4);
                    jyqsl[i, j] = qsl;
                    jyqsl[i, 0] += qsl;
                }
            }
            PrintGjQslFile();
        }
        private void PrintGjQslFile()
        {
            String path = this.saveName + "/改进降雨侵蚀力"  + ".txt";
            WriteTxtAdd(path, "站点编号或名称\t年度\tNJY\tJ1\tJ2\tJ3\tJ4\tJ5\tJ6\tJ7\tJ8\tJ9\tJ10\tJ11\tJ12\tJ13\tJ14\tJ15\tJ16\tJ17\tJ18\tJ19\tJ20\tJ21\tJ22\tJ23\tJ24\ta\tb\n");
            for (int i = 0; i < jyqsl.GetLength(0); i++)
            {
                String outString = zdbh + "\t\t";
                outString += (i + int.Parse(yearb.Text)).ToString() + "\t" + jyqsl[i, 0].ToString() + "\t";

                for (int j = 1; j < 25; j++)
                {
                    outString += jyqsl[i, j].ToString() + "\t" ;
                }
                outString += a + "\t" + b + "\t";
                //outqsString += rby.ToString() + "\t" + rby / qsjy[i, 0] + "\t";
                WriteTxtAdd(path, outString + "\n");
            }
        }
        private void PrintZsQslFile()
        {
            String path = this.saveName + "/章氏降雨侵蚀力"  + ".txt";
            WriteTxtAdd(path, "站点编号或名称\t年度\tNJY\tJ1\tJ2\tJ3\tJ4\tJ5\tJ6\tJ7\tJ8\tJ9\tJ10\tJ11\tJ12\tJ13\tJ14\tJ15\tJ16\tJ17\tJ18\tJ19\tJ20\tJ21\tJ22\tJ23\tJ24\t章a\t章b\n");
            for (int i = 0; i < jyqsl.GetLength(0); i++)
            {
                String outString = zdbh + "\t\t";
                outString += (i + int.Parse(yearb.Text)).ToString() + "\t" + jyqsl[i, 0].ToString() + "\t";

                for (int j = 1; j < 25; j++)
                {
                    outString += jyqsl[i, j].ToString() + "\t";
                }

                outString += a + "\t" + b + "\t";
                //outqsString += rby.ToString() + "\t" + rby / qsjy[i, 0] + "\t";
                WriteTxtAdd(path, outString + "\n");
            }
        }
        private void PrintQslFile()
        {
            String path = this.saveName + "/逐日雨量"  + ".txt";
            WriteTxtAdd(path, "站点编号或名称\t年度\tNJY\tJ1\tJ2\tJ3\tJ4\tJ5\tJ6\tJ7\tJ8\tJ9\tJ10\tJ11\tJ12\tJ13\tJ14\tJ15\tJ16\tJ17\tJ18\tJ19\tJ20\tJ21\tJ22\tJ23\tJ24\n") ;
            for (int i = 0; i < jyqsl.GetLength(0); i++)
            {
                String outString = zdbh + "\t\t";
                outString += (i + int.Parse(yearb.Text)).ToString() + "\t" + jyqsl[i, 0].ToString() + "\t";
   
                for (int j = 1; j < 25; j++)
                {
                    outString += jyqsl[i, j].ToString() + "\t";
                }

                //outqsString += rby.ToString() + "\t" + rby / qsjy[i, 0] + "\t";
                WriteTxtAdd(path, outString  + "\n");
            }
        }
        /// <summary>
        /// 输出文件
        /// </summary>
        private void PrintoutFile()
        {
            String path = this.saveName + "/数据统计"  + ".txt";
            WriteTxtAdd(path, "站点编号或名称\t年度\t年降雨量\tJ1\tJ2\tJ3\tJ4\tJ5\tJ6\tJ7\tJ8\tJ9\tJ10\tJ11\tJ12\tJ13\tJ14\tJ15\tJ16\tJ17\tJ18\tJ19\tJ20\tJ21\tJ22\tJ23\tJ24\t" +
                "年侵蚀降雨量\tQSJ1\tQSJ2\tQSJ3\tQSJ4\tQSJ5\tQSJ6\tQSJ7\tQSJ8\tQSJ9\tQSJ10\tQSJ11\tQSJ12\tQSJ13\tQSJ14\tQSJ15\tQSJ16\tQSJ17\tQSJ18\tQSJ19\tQSJ20\tQSJ21\tQSJ22\tQSJ23\tQSJ24\t" +
                "年侵蚀降雨天数\tQST1\tQST2\tQST3\tQST4\tQST5\tQST6\tQST7\tQST8\tQST9\tQST10\tQST11\tQST12\tQST13\tQST14\tQST15\tQST16\tQST17\tQST18\tQST19\tQST20\tQST21\tQST22\tQST23\tQST24\n");
           
            for (int i = 0; i < yjy.GetLength(0); i++)
            {
                String outString = zdbh + "\t\t";
                outString += (i + int.Parse(yearb.Text)).ToString() + "\t" + yjy[i, 0].ToString() + "\t";
                String outqsString =  qsjy[i, 0].ToString() + "\t\t";
                String outtString= qsjy[i, 25].ToString() + "\t\t";
                for (int j = 1; j < 25; j++)
                {
                    outString += yjy[i, j].ToString() + "\t";
                    outqsString += qsjy[i, j].ToString() + "\t";
                    outtString+=qsjy[i,j+25].ToString() + "\t";
                }

                //outqsString += rby.ToString() + "\t" + rby / qsjy[i, 0] + "\t";
                WriteTxtAdd(path, outString+ outqsString+outtString + "\n");
            }
            
        }
        private void PrintoutQslFile()
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                this.saveName = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
