using SuperMap.Data;
using SuperMap.Mapping;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2_DatasetManager
{
    public partial class Form1 : Form
    {
        public NowData nowdata;
        private SampleRun m_sampleRun;// 数据交换\地图查询
        private SampleRun_quare m_sampleRun_quare;// 数据交换\地图查询
        private SampleRun_theme m_sampleRun_theme;
        private SampleRun_PrjTran m_sampleRun_prjtran;//投影转换
        private SampleRun_Topo m_sampleRun_Topo;//拓扑处理
        private SampleRun_Buffer samplerun_buffer; //缓冲区查询
        private SampleRun_find2dpath samplerun_find2dpath; //网络分析之最佳路径
        private SampleRun_findclosest samplerun_findclosest; // 网络分析之最近设施分析
        private SampleRun_findmtspath samplerun_findmtspath; // 网络分析之物流配送分析

        private Boolean ThemeDotDensityDisplay = false;
        private Boolean ThemeRangDisplay = false;
        public Form1()
        {
            InitializeComponent();
            workspaceControl1.WorkspaceTree.BeforeNodeContextMenuStripShow += new BeforeNodeContextMenuStripShowEventHandler(WorkspaceTree_BeforeNodeContextMenuStripShow);
            initfirstmap();
            nowdata = new NowData(workspace1, mapControl1); // 实例化当前数据对象
        }
        private void initfirstmap()
        {
            stateText.Text = "打开数据中，请等待.....";
            statusBar.Update();
            workspace1.Open(new WorkspaceConnectionInfo(@"..\..\City\Changchun.smwu"));

            //建立MapControl与Workspace的连接
            mapControl1.Map.Workspace = workspace1;
            workspaceControl1.WorkspaceTree.Workspace = workspace1;

            // 为地图控件添加鼠标点击事件
            mapControl1.GeometrySelected += new SuperMap.UI.GeometrySelectedEventHandler(m_mapControl_GeometrySelected);
            //判断工作空间中是否有地图
            if (workspace1.Maps.Count == 0)
            {
                MessageBox.Show("当前工作空间中不存在地图!");
                return;
            }

            //打开第一幅地图
            Map map = mapControl1.Map;
            map.Workspace = workspace1;
            map.Open(workspace1.Maps[0]);

            // 将地图关联到二维图层树，使其管理其中的地图图层
            layersControl1.Map = map;

            //刷新地图窗口
            mapControl1.Map.Refresh();
            stateText.Text = "请选择";
        }

        private void 打开工作空间ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateText.Text = "打开数据中，请等待.....";
            statusBar.Update();
            //设置公用打开对话框
            //openFileDialog1.Filter = "SuperMap 工作空间文件(*.smwu)|*.smwu";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SuperMap 工作空间文件(*.smwu)|*.smwu";
            //避免连续打开工作空间导致程序异常   
            mapControl1.Map.Close();
            workspace1.Close();
            mapControl1.Map.Refresh();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                    workspace1.Save();
                    workspace1.Close();
                    workspace1.Open(new WorkspaceConnectionInfo(openFileDialog.FileName));
                Console.WriteLine(openFileDialog.FileName);
                //建立MapControl与Workspace的连接
                mapControl1.Map.Workspace = workspace1;
                    workspaceControl1.WorkspaceTree.Workspace = workspace1;
                    //判断工作空间中是否有地图
                    if (workspace1.Maps.Count == 0)
                    {
                        MessageBox.Show("当前工作空间中不存在地图!");
                        return;
                    }

                //打开第一幅地图
                    Map map = mapControl1.Map;
                    map.Workspace = workspace1;
                    map.Open(workspace1.Maps[0]);

                // 将地图关联到二维图层树，使其管理其中的地图图层
                layersControl1.Map = map;

                //刷新地图窗口
                mapControl1.Map.Refresh();
                stateText.Text = "请选择";


                ////定义打开工作空间文件名
                //String fileName = openFileDialog1.FileName;
                ////打开工作空间文件
                //WorkspaceConnectionInfo connectionInfo = new WorkspaceConnectionInfo(fileName);
                ////打开工作空间
                //workspace1.Open(connectionInfo);
                ////建立MapControl与Workspace的连接
                //mapControl1.Map.Workspace = workspace1;
                //workspaceControl1.WorkspaceTree.Workspace = workspace1;
                ////判断工作空间中是否有地图
                //if (workspace1.Maps.Count == 0)
                //{
                //    MessageBox.Show("当前工作空间中不存在地图!");
                //    return;
                //}
                ////通过名称打开工作空间中的地图
                //mapControl1.Map.Open("世界地图");
                ////刷新地图窗口
                //mapControl1.Map.Refresh();
            }

            //try
            //{
            //    OpenFileDialog openFileDialog = new OpenFileDialog();

            //    if (openFileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        DialogResult saveResult = DialogResult.No;
            //        if (workspace1.IsModified)
            //        {
            //            saveResult = MessageBox.Show("当前工作空间需要关闭，是否保存？", "保存工作空间", MessageBoxButtons.YesNoCancel);
            //        }

            //        if (saveResult == DialogResult.Yes)
            //        {
            //            workspace1.Save();
            //            workspace1.Close();
            //            workspace1.Open(new WorkspaceConnectionInfo(openFileDialog.FileName));
            //        }
            //        else if (saveResult == DialogResult.No)
            //        {
            //            workspace1.Close();
            //            workspace1.Open(new WorkspaceConnectionInfo(openFileDialog.FileName));
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //Trace.WriteLine(ex.Message);
            //}

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mapControl1.Dispose();
            workspace1.Close();
            workspace1.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            mapControl1.Action = SuperMap.UI.Action.Select;
        }

        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mapControl1_Load(object sender, EventArgs e)
        {

        }

        private void workspaceControl1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripRefresh_Click(object sender, EventArgs e)
        {
            mapControl1.Map.Refresh();
        }

        private void toolStripPan_Click(object sender, EventArgs e)
        {
            mapControl1.Action = SuperMap.UI.Action.Pan;
        }

        private void toolStripZoomIn_Click(object sender, EventArgs e)
        {
            mapControl1.Action = SuperMap.UI.Action.ZoomIn;
        }

        private void toolStripZoomOut_Click(object sender, EventArgs e)
        {
            mapControl1.Action = SuperMap.UI.Action.ZoomOut;
        }

        private void toolStripZoomFree_Click(object sender, EventArgs e)
        {
            mapControl1.Action = SuperMap.UI.Action.ZoomFree;
        }

        private void toolStripViewEntire_Click(object sender, EventArgs e)
        {
            mapControl1.Map.ViewEntire();
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            workspace1.Save();
        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SuperMap 工作空间文件(*.smwu)|*.smwu";
            saveFileDialog.FilterIndex = 0;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                workspace1.SaveAs(new WorkspaceConnectionInfo(saveFileDialog.FileName));
            }
        }

        private void MenuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 向地图或场景中添加数据的响应事件
        /// toolStripMenuItemAddData_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAddData_Click(object sender, EventArgs e)
        {
            AddData();
        }

        //查看属性表
        /// <summary>
        /// 向地图或场景中添加数据的响应事件
        /// toolStripMenuItemAddData_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void toolStripMenuItemShowData_Click(object sender, EventArgs e)
        {

            WorkspaceTreeNodeBase node = workspaceControl1.WorkspaceTree.SelectedNode as WorkspaceTreeNodeBase;
            Dataset dataset = node.GetData() as Dataset;
            nowdata.setDataset(dataset);//将数据集传入对象

            Form fm = new recordset(nowdata);
            fm.Text = dataset.Name;
            fm.ShowDialog();

            
        }


        /// <summary>
        /// 每次查询完成以后将查询按钮置为不可用
        /// The Query button is not useable when the query operation is over
        /// </summary>
        private void SetDisable()
        {
            try
            {
                m_toolStripButtonContain.Enabled = false;
                m_toolStripButtonIntersect.Enabled = false;
                m_toolStripButtonDisjoint.Enabled = false;
            }
            catch (Exception ex)
            {
                // Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 工作空间树右键菜单弹出前事件
        /// The click event of the context menu when right click the workspace tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkspaceTree_BeforeNodeContextMenuStripShow(object sender, BeforeNodeContextMenuStripShowEventArgs e)
        {
            try
            {
                ToolStripMenuItem toolStripMenuItemAddData = new ToolStripMenuItem();
                toolStripMenuItemAddData.Click += new EventHandler(toolStripMenuItemAddData_Click);
                toolStripMenuItemAddData.Text = "添加到地图";
                ToolStripMenuItem toolStripMenuItemShowData = new ToolStripMenuItem();
                toolStripMenuItemShowData.Click += new EventHandler(toolStripMenuItemShowData_Click);
                toolStripMenuItemShowData.Text = "查看属性表";
                ToolStripMenuItem toolStripMenuItemOpenMap = new ToolStripMenuItem("Open Map");
                if (SuperMap.Data.Environment.CurrentCulture == "zh-CN")
                {
                    toolStripMenuItemOpenMap.Text = "打开地图";
                }
                toolStripMenuItemOpenMap.Click += new EventHandler(toolStripMenuItemAddData_Click);

                ContextMenuStrip contextMenuStripWorkspaceTree = new ContextMenuStrip();
                WorkspaceTreeNodeBase treeNode = e.Node as WorkspaceTreeNodeBase;
                if ((treeNode.NodeType & WorkspaceTreeNodeDataType.Dataset) != WorkspaceTreeNodeDataType.Unknown)
                {
                    contextMenuStripWorkspaceTree.Items.AddRange(new ToolStripItem[] { toolStripMenuItemAddData, toolStripMenuItemShowData });
                }
                else if (treeNode.NodeType == WorkspaceTreeNodeDataType.MapName)
                {
                    contextMenuStripWorkspaceTree.Items.AddRange(new ToolStripItem[] { toolStripMenuItemOpenMap });
                }
                workspaceControl1.WorkspaceTree.NodeContextMenuStrips[treeNode.NodeType] = contextMenuStripWorkspaceTree;
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 添加数据集到地图或场景
        /// Add the dataset to the map or the scene
        /// </summary>
        private void AddData()
        {
            try
            {
                WorkspaceTreeNodeBase node = workspaceControl1.WorkspaceTree.SelectedNode as WorkspaceTreeNodeBase;
                WorkspaceTreeNodeDataType type = node.NodeType;
                if ((type & WorkspaceTreeNodeDataType.Dataset) != WorkspaceTreeNodeDataType.Unknown)
                {
                    type = WorkspaceTreeNodeDataType.Dataset;
                }
                switch (type)
                {
                    case WorkspaceTreeNodeDataType.Dataset:
                        {
                            Dataset dataset = node.GetData() as Dataset;

                            if (layersControl1.Map != null)
                            {
                                layersControl1.Map.Layers.Add(dataset, true);
                                layersControl1.Map.Refresh();
                            }
                        }
                        break;
                    case WorkspaceTreeNodeDataType.MapName:
                        {
                            String mapName = node.GetData() as String;

                            if (layersControl1.Map != null)
                            {
                                mapControl1.Map.Open(mapName);
                                mapControl1.Map.Refresh();
                            }
                        }
                        break;
                    case WorkspaceTreeNodeDataType.SceneName:
                        {
                            String sceneName = node.GetData() as String;
                        }
                        break;
                    case WorkspaceTreeNodeDataType.SymbolMarker:
                        {
                            SymbolLibraryDialog.ShowDialog(workspace1.Resources, SymbolType.Marker);
                        }
                        break;
                    case WorkspaceTreeNodeDataType.SymbolLine:
                        {
                            SymbolLibraryDialog.ShowDialog(workspace1.Resources, SymbolType.Line);
                        }
                        break;
                    case WorkspaceTreeNodeDataType.SymbolFill:
                        {
                            SymbolLibraryDialog.ShowDialog(workspace1.Resources, SymbolType.Fill);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 处理GeometrySelected事件
		/// Manage the GeometrySelected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_mapControl_GeometrySelected(object sender, SuperMap.UI.GeometrySelectedEventArgs e)
        {
            try
            {
                if (e.Count != 0)
                {
                    m_toolStripButtonContain.Enabled = true;
                    m_toolStripButtonIntersect.Enabled = true;
                    m_toolStripButtonDisjoint.Enabled = true;
                    
                }
            }
            catch (Exception ex)
            {
                // Trace.WriteLine(ex.Message);
            }
        }

        private void 数据交换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 导出TABToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ExportToTab();
        }

        private void 导出SHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ExportToShape();
        }

        private void 导出SITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ExportToSit();
        }

        private void wOR导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToWor();
        }

        private void dWG导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToDwg();
        }

        private void iMG导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToImg();
        }

        private void 保留参数化对象导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToDwg1();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            //获取选择集
            Selection[] selection = mapControl1.Map.FindSelection(true);

            //判断选择集是否为空
            if (selection == null || selection.Length == 0)
            {
                MessageBox.Show("请选择要查询属性的空间对象");
                return;
            }

            //将选择集转换为记录
            Recordset recordset = selection[0].ToRecordset();

            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();

            for (int i = 0; i < recordset.FieldCount; i++)
            {
                //定义并获得字段名称
                String fieldName = recordset.GetFieldInfos()[i].Name;

                //将得到的字段名称添加到dataGridView列中
                this.dataGridView1.Columns.Add(fieldName, fieldName);
            }

            //初始化row
            DataGridViewRow row = null;

            //根据选中记录的个数，将选中对象的信息添加到dataGridView中显示
            while (!recordset.IsEOF)
            {
                row = new DataGridViewRow();
                for (int i = 0; i < recordset.FieldCount; i++)
                {
                    //定义并获得字段值
                    Object fieldValue = recordset.GetFieldValue(i);

                    //将字段值添加到dataGridView中对应的位置
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    if (fieldValue != null)
                    {
                        cell.ValueType = fieldValue.GetType();
                        cell.Value = fieldValue;
                    }

                    row.Cells.Add(cell);
                }

                this.dataGridView1.Rows.Add(row);

                recordset.MoveNext();
            }
            this.dataGridView1.Update();

            recordset.Dispose();

        }

        private void 分离查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                m_sampleRun_quare.Query(SuperMap.Data.SpatialQueryMode.Intersect);
                SetDisable();
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
            }
        }

        private void 包含查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                m_sampleRun_quare.Query(SuperMap.Data.SpatialQueryMode.Contain);
                SetDisable();
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
            }
        }

        private void 查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void 初始化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_quare = new SampleRun_quare(workspace1, mapControl1);
        }

        private void m_toolStripButtonDisjoint_Click(object sender, EventArgs e)
        {
            try
            {
                m_sampleRun_quare.Query(SuperMap.Data.SpatialQueryMode.Disjoint);
                SetDisable();
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
            }
        }

        private void m_toolStripButtonCityPeople_Click(object sender, EventArgs e)
        {
            //查询选区内包含capital的城市人口
            String m_queryObjectLayerName = "Ocean";
            String m_queriedLayerName = "Capital";
            SuperMap.Data.SpatialQueryMode mode = SuperMap.Data.SpatialQueryMode.Contain;
            try
            {
                // 获取地图中的选择集，并转换为记录集
                // Get the dataset in the map, and convert it to recordset.
                Selection[] selections = mapControl1.Map.FindSelection(true);
                Selection selection = selections[0];
                Recordset recordset = selection.ToRecordset();

                // 设置查询参数
                // Set the query parameter.
                QueryParameter parameter = new QueryParameter();
                parameter.SpatialQueryObject = recordset;
                parameter.SpatialQueryMode = mode;

                // 对指定查询的图层进行查询
                // Query the specified layer.
                Layer layer = this.GetLayerByCaption(m_queriedLayerName);
                DatasetVector dataset = layer.Dataset as DatasetVector;

                Recordset recordset2 = dataset.Query(parameter);

                String[] values = { "POP", "CAPITAL_CH", "COUNTRY_CH" };
                // 加载人口数据到窗口
                Form fm = new ShowRecordset(recordset2,dataset,values);
                fm.Text = dataset.Name;
                fm.ShowDialog();

                //layer.Selection.FromRecordset(recordset2);

                ////layer.Selection.Style.LineColor = Color.Red;
                ////layer.Selection.Style.LineWidth = 0.6;
                //layer.Selection.Style.LineColor = Color.Red;
                //layer.Selection.SetStyleOptions(StyleOptions.FillSymbolID, true);
                //layer.Selection.Style.FillSymbolID = 1;
                //layer.Selection.IsDefaultStyleEnabled = false;

                recordset2.Dispose();

                // 刷新地图
                // Refresh the map.
                //mapControl1.Map.Refresh();

                recordset.Dispose();
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
            }
        }
        private Layer GetLayerByCaption(String layerCaption)
        {
            Layers layers = mapControl1.Map.Layers;
            Layer result = null;

            foreach (Layer layer in layers)
            {
                if (String.Compare(layer.Caption, layerCaption, true) == 0)
                {
                    result = layer;
                    break;
                }
            }

            return result;
        }

        private void 初始化ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_theme = new SampleRun_theme(workspace1, mapControl1);
        }

        private void 点密度专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.AddThemeDotDensityLayer();
        }

        private void 等级符号专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.ThemeRangeThemeDisplay();

        }

        private void inittheme()
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_theme = new SampleRun_theme(workspace1, mapControl1);
        }

        private void 统计专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.addThemeGraphBar3DVisible();
        }

        private void 分段标签专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.AddThemeLabelLayer();
        }

        private void 分段专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.AddThemeRangeLayer();
        }

        private void 单值专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.themeuniqueDisplay(true);
            
        }

        private void 统一风格标签专题图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inittheme();
            m_sampleRun_theme.AddUniformStyleThemeLabelLayer(); 
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void 初始化ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_prjtran = new SampleRun_PrjTran(workspace1, mapControl1);
        }

        private void 高斯克吕格投影转换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String prjinfo;
                this.Cursor = Cursors.WaitCursor;
                prjinfo = m_sampleRun_prjtran.TransformPrj(1);
                this.Cursor = Cursors.Arrow;
                MessageBox.Show( "成功将数据集转换为如下投影：\n"  + prjinfo, "转换成功", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void uTM投影转换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String prjinfo;
                this.Cursor = Cursors.WaitCursor;
                prjinfo = m_sampleRun_prjtran.TransformPrj(2);
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("成功将数据集转换为如下投影：\n" + prjinfo, "转换成功", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 兰勃托投影转换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String prjinfo;
                this.Cursor = Cursors.WaitCursor;
                prjinfo = m_sampleRun_prjtran.TransformPrj(3);
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("成功将数据集转换为如下投影：\n" + prjinfo, "转换成功", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 初始化ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            stateText.Text = "打开数据中，请等待.....";
            statusBar.Update();
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_Topo = new SampleRun_Topo(workspace1, mapControl1);
            stateText.Text = "请选择";
        }

        private void 拓扑构面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                    stateText.Text = "拓扑构面中，请等待.....";
                    statusBar.Update();
                    this.Cursor = Cursors.WaitCursor;
                    if (m_sampleRun_Topo.LineToRegion())
                    {
                        stateText.Text = "拓扑构面成功完成";
                    }
                    else
                    {
                        stateText.Text = "拓扑构面失败";
                    }
                
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                // Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 拓扑处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                    stateText.Text = "拓扑处理中，请等待.....";
                    statusBar.Update();
                    this.Cursor = Cursors.WaitCursor;

                    if (m_sampleRun_Topo.TopoProcess())
                    {
                        stateText.Text = "拓扑处理成功完成";
                    }
                    else
                    {
                        stateText.Text = "拓扑处理失败";
                    }

                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 拓扑检查ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                    stateText.Text = "拓扑检查中，请等待.....";
                    statusBar.Update();
                    this.Cursor = Cursors.WaitCursor;

                    if (m_sampleRun_Topo.TopoCheck())
                    {
                        stateText.Text = "拓扑检查成功完成";
                    }
                    else
                    {
                        stateText.Text = "拓扑检查失败";
                    }
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 缓冲查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 创建缓冲区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            samplerun_buffer.CreatBuffer();
            //添加缓冲区对象
            Dataset quaredataset = workspace1.Datasources[0].Datasets["School"];
            mapControl1.Map.Layers.Add(quaredataset, true);

            MessageBox.Show("请点击缓冲区查询，开始缓冲区查询！\n 当前查询目标：地铁沿线周边100m的学校", "缓冲区创建完毕", MessageBoxButtons.OK);
        }

        private void 初始化ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun = new SampleRun(workspace1, mapControl1);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void 初始化ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun = new SampleRun(workspace1, mapControl1);
        }

        private void 导出TABToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ExportToTab();
        }

        private void 导出SHPToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ExportToShape();
        }

        private void 导出SITToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ExportToSit();
        }

        private void wOR导入ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToWor();
        }

        private void dWG导入ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToDwg();
        }

        private void iMG导入ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToImg();
        }

        private void 保留参数化对象导入ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_sampleRun.ImportToDwg1();
        }

        private void 初始化ToolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_prjtran = new SampleRun_PrjTran(workspace1, mapControl1);
        }

        private void 高斯克吕格投影转换ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                String prjinfo;
                this.Cursor = Cursors.WaitCursor;
                prjinfo = m_sampleRun_prjtran.TransformPrj(1);
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("成功将数据集转换为如下投影：\n" + prjinfo, "转换成功", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void uTM投影转换ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                String prjinfo;
                this.Cursor = Cursors.WaitCursor;
                prjinfo = m_sampleRun_prjtran.TransformPrj(2);
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("成功将数据集转换为如下投影：\n" + prjinfo, "转换成功", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 兰勃托投影转换ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                String prjinfo;
                this.Cursor = Cursors.WaitCursor;
                prjinfo = m_sampleRun_prjtran.TransformPrj(3);
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("成功将数据集转换为如下投影：\n" + prjinfo, "转换成功", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 初始化ToolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            stateText.Text = "打开数据中，请等待.....";
            statusBar.Update();
            mapControl1.Map.Close();
            workspace1.Close();
            m_sampleRun_Topo = new SampleRun_Topo(workspace1, mapControl1);
            stateText.Text = "请选择";
        }

        private void 拓扑构面ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                stateText.Text = "拓扑构面中，请等待.....";
                statusBar.Update();
                this.Cursor = Cursors.WaitCursor;
                if (m_sampleRun_Topo.LineToRegion())
                {
                    stateText.Text = "拓扑构面成功完成";
                }
                else
                {
                    stateText.Text = "拓扑构面失败";
                }

                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                // Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 拓扑处理ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                stateText.Text = "拓扑处理中，请等待.....";
                statusBar.Update();
                this.Cursor = Cursors.WaitCursor;

                if (m_sampleRun_Topo.TopoProcess())
                {
                    stateText.Text = "拓扑处理成功完成";
                }
                else
                {
                    stateText.Text = "拓扑处理失败";
                }

                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 拓扑检查ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                stateText.Text = "拓扑检查中，请等待.....";
                statusBar.Update();
                this.Cursor = Cursors.WaitCursor;

                if (m_sampleRun_Topo.TopoCheck())
                {
                    stateText.Text = "拓扑检查成功完成";
                }
                else
                {
                    stateText.Text = "拓扑检查失败";
                }
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message + " \n请尝试初始化!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 缓冲区查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_buffer.QuarebuBuffer();

            //查询选中图层信息到信息栏
            //获取选择集
            Selection[] selection = mapControl1.Map.FindSelection(true);

            //判断选择集是否为空
            if (selection == null || selection.Length == 0)
            {
                MessageBox.Show("请选择要查询属性的空间对象");
                return;
            }

            //将选择集转换为记录
            Recordset recordset = selection[0].ToRecordset();

            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();

            for (int i = 0; i < recordset.FieldCount; i++)
            {
                //定义并获得字段名称
                String fieldName = recordset.GetFieldInfos()[i].Name;

                //将得到的字段名称添加到dataGridView列中
                this.dataGridView1.Columns.Add(fieldName, fieldName);
            }

            //初始化row
            DataGridViewRow row = null;

            //根据选中记录的个数，将选中对象的信息添加到dataGridView中显示
            while (!recordset.IsEOF)
            {
                row = new DataGridViewRow();
                for (int i = 0; i < recordset.FieldCount; i++)
                {
                    //定义并获得字段值
                    Object fieldValue = recordset.GetFieldValue(i);

                    //将字段值添加到dataGridView中对应的位置
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    if (fieldValue != null)
                    {
                        cell.ValueType = fieldValue.GetType();
                        cell.Value = fieldValue;
                    }

                    row.Cells.Add(cell);
                }

                this.dataGridView1.Rows.Add(row);

                recordset.MoveNext();
            }
            this.dataGridView1.Update();

            MessageBox.Show("查询结果总数：" + recordset.FieldCount  +  "\n 数据信息输出在地图下方信息栏！", "查询完毕", MessageBoxButtons.OK);
            

            recordset.Dispose();

            
        }

        private void 初始化ToolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            mapControl1.Map.Close();
            workspace1.Close();
            samplerun_buffer = new SampleRun_Buffer(workspace1, mapControl1);

            //添加缓冲区对象
            Dataset quaredataset = workspace1.Datasources[0].Datasets["Railway"];
            mapControl1.Map.Layers.Add(quaredataset, true);

            MessageBox.Show("请选择图中Railway线路作为创建缓冲区对象！" , "缓冲区准备完毕", MessageBoxButtons.OK);

        }

        private void 初始化ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            stateText.Text = "打开数据中，请等待.....";
            statusBar.Update();
            mapControl1.Map.Close();
            workspace1.Close();

            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("序号", "序号");
            dataGridView1.Columns.Add("导引", "导引");
            dataGridView1.Columns.Add("耗费", "耗费");
            dataGridView1.Columns.Add("距离", "距离");
            

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }
            mapControl1.Dock = DockStyle.Fill;

            samplerun_find2dpath = new SampleRun_find2dpath(workspace1, mapControl1,dataGridView1);
            stateText.Text = "请选择";
        }

        private void 设置经过点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_find2dpath.SetSelectMode(SampleRun_find2dpath.SelectMode.SelectPoint, false);
        }

        private void 设置障碍ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_find2dpath.SetSelectMode(SampleRun_find2dpath.SelectMode.SelectBarrier, true);
        }

        private void 分析ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (samplerun_find2dpath.Analyst())
            {
                EnabledControl(false);
            }
        }
        private void EnabledControl(bool isEnabled)
        {
            初始化ToolStripMenuItem6.Enabled = isEnabled;
            设置经过点ToolStripMenuItem.Enabled = isEnabled;
            分析ToolStripMenuItem1.Enabled = isEnabled;
            停止ToolStripMenuItem.Enabled = !isEnabled;
            导引ToolStripMenuItem.Enabled = !isEnabled;
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_find2dpath.Stop();
        }

        private void 导引ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_find2dpath.Play();
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                samplerun_find2dpath.Clear();

                SampleRun_find2dpath.SelectMode mode = SampleRun_find2dpath.SelectMode.SelectPoint;
                samplerun_find2dpath.SetSelectMode(mode, false);

                //if (radioButtonBarrier.Checked)
                //{
                //    mode = SampleRun_find2dpath.SelectMode.SelectBarrier;
                //    samplerun_find2dpath.SetSelectMode(mode, true);
                //}

                EnabledControl(true);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
            }
        }

        private void 初始化ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            stateText.Text = "打开数据中，请等待.....";
            statusBar.Update();
            mapControl1.Map.Close();
            workspace1.Close();

            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("序号", "序号");
            dataGridView1.Columns.Add("导引", "导引");
            dataGridView1.Columns.Add("耗费", "耗费");
            dataGridView1.Columns.Add("距离", "距离");


            samplerun_findclosest = new SampleRun_findclosest(workspace1, mapControl1, dataGridView1);

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }
           
            //mapControl1.Paint += new PaintEventHandler(m_mapControl_Paint);
            //for (int i = 0; i < dataGridView1.Columns.Count; i++)
            //{
            //    dataGridView1.Columns[i].ReadOnly = true;
            //}
            mapControl1.Dock = DockStyle.Fill;

            
            stateText.Text = "请选择";
        }
        /// <summary>
        /// 没有选择对象的时候表格清空。
        /// Clear the table if there is no object selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_mapControl_Paint(object sender, PaintEventArgs e)
        {
            //try
            //{
            //    if (mapControl1.Map.Layers[0].Selection.Count < 1)
            //    {
            //        dataGridView1.Columns.Clear();
            //        dataGridView1.Rows.Clear();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //Trace.WriteLine(ex.Message);
            //}
        }

        private void 选取设施点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_findclosest.FindFacilityNodes();
            samplerun_findclosest.ChangeAction(SampleRun_findclosest.SelectMode.SELECTPOINT);
        }

        private void 选取障碍点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_findclosest.FindBarrierNodes();
            samplerun_findclosest.ChangeAction(SampleRun_findclosest.SelectMode.SELECTBARRIER);
        }

        private void 选取事件点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_findclosest.FindEventNode();
            samplerun_findclosest.ChangeAction(SampleRun_findclosest.SelectMode.SELECTEVENT);
        }

        private void 分析ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            samplerun_findclosest.StartAnalyst();
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            samplerun_findclosest.ClearAll();
        }

        private void statusBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void splitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void 初始化ToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("序号", "序号");
            dataGridView1.Columns.Add("导引", "导引");
            dataGridView1.Columns.Add("耗费", "耗费");
            dataGridView1.Columns.Add("距离", "距离");
            

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }
            mapControl1.Dock = DockStyle.Fill;

            samplerun_findmtspath = new SampleRun_findmtspath(workspace1, mapControl1, comboBox1, dataGridView1);
        }

        private void 配送中心点ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            samplerun_findmtspath.SetSelectMode(SampleRun_findmtspath.SelectMode.SelectCenter, false);
        }

        private void 设置障碍ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            samplerun_findmtspath.SetSelectMode(SampleRun_findmtspath.SelectMode.SelectBarrier, true);
        }

        private void 配送目的地ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            samplerun_findmtspath.SetSelectMode(SampleRun_findmtspath.SelectMode.SelectTarget, false);
        }

        private void 分析ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            samplerun_findmtspath.Analyst();
        }

        private void 清除ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                samplerun_findmtspath.Clear();
                SampleRun_findmtspath.SelectMode mode = SampleRun_findmtspath.SelectMode.SelectCenter;
                //if (radioButtonBarrier.Checked)
                //{
                //    mode = SampleRun.SelectMode.SelectBarrier;
                //    m_sampleRun.SetSelectMode(mode, true);
                //}
                //if (radioButtonPoint.Checked)
                //{
                //    mode = SampleRun.SelectMode.SelectTarget;
                //    m_sampleRun.SetSelectMode(mode, false);
                //}
                //EnabledControl(true);
            }
            catch (Exception ex)
            {
               // Trace.WriteLine(ex.Message);
            }
        }

        private void 导引ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            samplerun_findmtspath.Play();
        }

        private void 停止ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            samplerun_findmtspath.Stop();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            samplerun_findmtspath.SetSelectMode(SampleRun_findmtspath.SelectMode.SelectCenter, false);
        }
    }



    public class NowData
    {
        private Workspace workplace;
        private MapControl mapcontrol;
        private Datasources datasources;

        //private Datasource datasource;
        private Datasets datasets;
        private DatasetVector datasetvector;
        private Dataset dataset;
        public NowData(Workspace workplace1, MapControl mapcontrol1)
        {
            workplace = workplace1;
            mapcontrol = mapcontrol1;
            datasources = workplace1.Datasources;
        }
        //public void setDatasource(Datasource datasource1) { datasource = datasource1; }
        public Datasource getDatasource(String datasourcename) { return datasources[datasourcename]; }
        public void setDataset(Dataset dataset1) { dataset = dataset1; }
        public Dataset getDataset() { return dataset; }
    }
}
