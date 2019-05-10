using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperMap.Data;
using System.Collections;

namespace _2_DatasetManager
{
    public partial class ShowRecordset : Form
    {
        Recordset recordset;
        DatasetVector dataset;
        String [] values;
        ArrayList valuenum = new ArrayList();
        public ShowRecordset(Recordset recordset1, DatasetVector dataset1, String [] Values1)
        {
            InitializeComponent();
            recordset = recordset1;
            dataset = dataset1;
            values = Values1;
        }
        public ShowRecordset(Recordset recordset1, DatasetVector dataset1)
        {
            InitializeComponent();
            recordset = recordset1;
            dataset = dataset1;
        }
        public ShowRecordset()
        {
            InitializeComponent();
        }

        private void ShowRecordset_Load(object sender, EventArgs e)
        {
            if (values == null)
            {
                ShowAllRecordInfoTolistview();
            }
            else
            {
                //显示指定字段信息
                SelectValueRecordInfoTolistview();
            }
        }

        private void ShowAllRecordInfoTolistview()
        {
            //设置属性
            listView1.GridLines = true;  //显示网格线
            listView1.FullRowSelect = true;  //显示全行
            listView1.MultiSelect = false;  //设置只能单选
            listView1.View = View.Details;  //设置显示模式为详细
            listView1.HoverSelection = true;  //当鼠标停留数秒后自动选择

            //填充表头
            int minlength = 60;
            listView1.Columns.Add("序号", minlength);
            for (int i = 0; i < dataset.FieldCount; i++)
            {
                listView1.Columns.Add(dataset.FieldInfos[i].Name, minlength);  //相当于上面的添加列名的步骤
            }
            //填充数据
            recordset.MoveFirst();
            this.listView1.BeginUpdate();  //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 
            for (int i = 0; i < recordset.RecordCount; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = i.ToString();
                for (int j = 0; j < dataset.FieldCount; j++)
                {
                    String info;
                    if (recordset.GetFieldValue(j) != null)
                    {
                        info = recordset.GetFieldValue(j).ToString();
                    }
                    else
                    {
                        info = "null";
                    }
                    lvi.SubItems.Add(info);
                }
                this.listView1.Items.Add(lvi);
                recordset.MoveNext();

            }
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }
        private void SelectValueRecordInfoTolistview()
        {
            //设置属性
            listView1.GridLines = true;  //显示网格线
            listView1.FullRowSelect = true;  //显示全行
            listView1.MultiSelect = false;  //设置只能单选
            listView1.View = View.Details;  //设置显示模式为详细
            listView1.HoverSelection = true;  //当鼠标停留数秒后自动选择

            //检验字段名称并查询所在i
            
            
            for (int i = 0; i < dataset.FieldCount; i++)
            {
                for(int j =0; j< values.Length; j++)
                {
                    if(dataset.FieldInfos[i].Name == values[j])
                    {
                        valuenum.Add(i);
                    }
                }
            }
            if (valuenum.Count < 1)
            {
                MessageBox.Show("数据源中不存在人口数据集，查询结果为空", "查询结果");
                this.Close();//关闭当前对话框
            }


            //填充表头
            int minlength = 60;
            listView1.Columns.Add("序号", minlength);
            for (int i = 0; i < valuenum.Count; i++)
            {
                int valueint = (int)valuenum[i];
                listView1.Columns.Add(dataset.FieldInfos[valueint].Name, minlength);  //相当于上面的添加列名的步骤
            }

            //填充数据
            recordset.MoveFirst();
            this.listView1.BeginUpdate();  //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 
            for (int i = 0; i < recordset.RecordCount; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = i.ToString();
                for (int j = 0; j < valuenum.Count; j++)
                {
                    String info;
                    int valueint = (int)valuenum[j];
                    if (recordset.GetFieldValue(valueint) != null)
                    {
                        info = recordset.GetFieldValue(valueint).ToString();
                    }
                    else
                    {
                        info = "null";
                    }
                    lvi.SubItems.Add(info);
                }
                this.listView1.Items.Add(lvi);
                recordset.MoveNext();

            }
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            String loginfo = "本次查询共查询到数据" + recordset.RecordCount + "条！";
        }
    }
}
