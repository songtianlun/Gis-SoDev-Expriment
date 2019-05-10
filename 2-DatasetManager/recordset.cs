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

namespace _2_DatasetManager
{
    public partial class recordset : Form
    {
        NowData nowdata;
        //重载构造函数以传递值
        public recordset(NowData nowdata1)
        {
            InitializeComponent();
            nowdata = nowdata1;
        }
        public recordset()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void recordset_Load(object sender, EventArgs e)
        {
            //Form1 form1 = new Form1();
            Dataset dataset = nowdata.getDataset();
            DatasetVector datasetvector = (DatasetVector)dataset as DatasetVector;
            Recordset recordset = datasetvector.GetRecordset(false, CursorType.Dynamic);
            Object value = recordset.GetValues();//获取字段

            //设置属性
            listView1.GridLines = true;  //显示网格线
            listView1.FullRowSelect = true;  //显示全行
            listView1.MultiSelect = false;  //设置只能单选
            listView1.View = View.Details;  //设置显示模式为详细
            listView1.HoverSelection = true;  //当鼠标停留数秒后自动选择

            //填充表头
            int minlength = 60;
            listView1.Columns.Add("序号", minlength);
            for (int i = 0; i < datasetvector.FieldCount; i++)
            {
                listView1.Columns.Add(datasetvector.FieldInfos[i].Name, minlength);  //相当于上面的添加列名的步骤
            }
            //填充数据
            recordset.MoveFirst();
            this.listView1.BeginUpdate();  //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 
            for (int i = 0; i < recordset.RecordCount; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = i.ToString();
                for (int j = 0; j < datasetvector.FieldCount; j++)
                {
                    String info;
                    if (recordset.GetFieldValue(j) != null)
                    {
                        info = recordset.GetFieldValue(j).ToString();
                    }else
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
    }
}
