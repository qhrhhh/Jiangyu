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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private String fileName;
        private List<String> jyList = new List<string>();
        private int sumjy = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                this.fileName = openFileDialog1.FileName;
            }
            
        }
        public void WriteTxt(String path,String txt)
        {
            FileStream fs = new FileStream(path, FileMode.Append,FileAccess.Write);
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

        private void button2_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            string line;
            List<JYData> l = new List<JYData>();
            int yearnum = int.Parse(yeare.Text) - int.Parse(yearb.Text) + 1;
            double[,] yjy = new double[yearnum,25];
            double[,] qsjy = new double[yearnum, 25];
            double[] mjy = new double[yjy.Length * 2];
            double[,] yuedaym = new double[yearnum, 25];
            line = sr.ReadLine();
            int ryln = 0;
            while ((line = sr.ReadLine()) != null)
            {
                jyList.Add(line);
                String[] data = line.Split('\t');
                JYData jy = new JYData(data[0], "", data[1], data[2], data[3], data[4]);
                l.Add(jy);
                //int beginyear = int.Parse(yearb.Text);
                if (int.Parse(data[1]) < int.Parse(yearb.Text)) continue;
                if (int.Parse(data[1]) > int.Parse(yeare.Text)) continue;
                yjy[int.Parse(data[1]) - int.Parse(yearb.Text),0] += double.Parse(data[4]);
                if(double.Parse(data[4])>double.Parse(qsyl.Text))
                    qsjy[int.Parse(data[1]) - int.Parse(yearb.Text), 0] += double.Parse(data[4]);
                int index = (int.Parse(data[2]) - 1) * 2 + 1;
                if (int.Parse(data[3]) > 15)
                    index += 1;
                if (double.Parse(data[4]) > double.Parse(qsyl.Text)) {
                    ryln++;
                    qsjy[int.Parse(data[1]) - int.Parse(yearb.Text), index] += double.Parse(data[4]);
                }
                yjy[int.Parse(data[1]) - int.Parse(yearb.Text), index] += double.Parse(data[4]);

            }
            double rylsum = 0;
            double a = 0.3101; 
            double rby = 0;
            for (int i = 0; i < yearnum; i++)
            {
                rylsum += qsjy[i, 0];
                for(int j = 1; j < 25; j++)
                {
                    if (j > 4 && j < 10) a = 0.3937;
                    rby+=Math.Pow(Convert.ToDouble(qsjy[i, j]), a);
                    
                }
            }
            double rylp = rylsum / ryln;
            double rylb = 0.6243 + (27.346 / rylp);
            double ryla = 21.239 * Math.Pow(rylb, -7.3967);
            WriteTxt("日雨量(自定义).txt", ryla.ToString()+"\t"+rylb+"\t"+rylp + "\n");
            rby = rby / yearnum;            
            for (int i = 0; i < yjy.GetLength(0); i++) {
                String outString = (i + int.Parse(yearb.Text)).ToString() + "\t"+yjy[i, 0].ToString() + "\t";
                String outqsString = (i + int.Parse(yearb.Text)).ToString() + "\t" + qsjy[i, 0].ToString() + "\t";
                for (int j = 1; j < 25; j++)
                {
                    outString += yjy[i, j].ToString() + "\t";
                    outqsString += qsjy[i, j].ToString() + "\t";
                }

                outqsString += rby.ToString() + "\t" +rby/qsjy[i,0]+ "\t";
                WriteTxt("降雨量.txt", outString + "\n");
                WriteTxt("侵蚀降雨量.txt", outqsString + "\n");
            }
            button2.Text = "完成";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    public class JYData
    {
        private String zdname;
        private String zdnum;
        private String year;
        private String month;
        private String day;
        private String jyl;

        public JYData(){
            zdname = "";
            zdnum = "";
            year = "";
            month = "";
            day = "";
            jyl = "";
        }
        public JYData(String a,String b,String c ,String d,String e,String f)
        {
            zdname =a;
            zdnum = b;
            year = c;
            month = d;
            day = e;
            jyl = f;
        }
   
        public string Zdname { get => zdname; set => zdname = value; }
        public string Zdnum { get => zdnum; set => zdnum = value; }
        public string Year { get => year; set => year = value; }
        public string Month { get => month; set => month = value; }
        public string Jyl { get => jyl; set => jyl = value; }
        public string Day { get => day; set => day = value; }
    }
}
