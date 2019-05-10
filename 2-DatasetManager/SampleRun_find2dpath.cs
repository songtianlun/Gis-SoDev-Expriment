///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------版权声明----------------------------
//
// 此文件为 SuperMap iObjects .NET 的示范代码
// 版权所有：北京超图软件股份有限公司
//------------------------------------------------------------------
//
//-----------------------SuperMap iObjects .NET 示范程序说明--------------------------
//
//1、范例简介：示范如何进行最佳路径分析
//2、示例数据：安装目录/SampleData/City/Changchun.udb
//3、关键类型/成员: 
//     MapControl.MouseDown 事件
//     MapControl.GeometrySelected 事件
//     Timer.Tick  事件
//     TransportationAnalystSetting.NetworkDataset 属性
//     TransportationAnalystSetting.EdgeIDField 属性
//     TransportationAnalystSetting.NodeIDField 属性
//     TransportationAnalystSetting.Tolerance 属性
//     TransportationAnalystSetting.WeightFieldInfo 属性
//     TransportationAnalystSetting.FNodeIDField 属性
//     TransportationAnalystSetting.TNodeIDField 属性
//     TransportationAnalyst.Load 方法
//     TransportationAnalyst.FindPPath 方法
//     TransportationAnalyst.AnalystSetting 属性
//     TransportationAnalystParameter.BarrierEdges 属性
//     TransportationAnalystParameter.BarrierNodes 属性
//     TransportationAnalystParameter.WeightName 属性
//     TransportationAnalystParameter.Points 属性
//     TransportationAnalystParameter.NodesReturn 属性
//     TransportationAnalystParameter.EdgesReturn 属性
//     TransportationAnalystParameter.PathGuidesReturn 属性
//     TransportationAnalystParameter.StopIndexesReturn 属性
//     TransportationAnalystParameter.RoutesReturn 属性
//     TransportationAnalystResult.Routes 属性
//     TransportationAnalystResult.PathGuides 属性
//4、使用步骤：
//  (1)选取路径分析经过点及障碍点（或障碍边）
//  (2)分析得出结果，导引可模拟进行路径行驶
//---------------------------------------------------------------------------------------
//------------------------------Copyright Statement----------------------------
//
// SuperMap iObjects .NET Sample Code
// Copyright: SuperMap Software Co., Ltd. All Rights Reserved.
//------------------------------------------------------------------
//
//-----------------------Description--------------------------
//
// 1. Introduction: This sample show you how to make optimal path analysis
//2. Sample Data: Installation directory\SampleData\City\Changchun.udb
//3. Key classes and members 
//      MapControl.MouseDown event
//     MapControl.GeometrySelected event
//     Timer.Tick event
//     TransportationAnalystSetting.NetworkDataset property
//     TransportationAnalystSetting.EdgeIDField property
//     TransportationAnalystSetting.NodeIDField property
//     TransportationAnalystSetting.Tolerance property
//     TransportationAnalystSetting.WeightFieldInfo property
//     TransportationAnalystSetting.FNodeIDField property
//     TransportationAnalystSetting.TNodeIDField property
//     TransportationAnalyst.Load method
//     TransportationAnalyst.FindPPath method
//     TransportationAnalyst.AnalystSetting property
//     TransportationAnalystParameter.BarrierEdges property
//     TransportationAnalystParameter.BarrierNodes property
//     TransportationAnalystParameter.WeightName property
//     TransportationAnalystParameter.Points property
//     TransportationAnalystParameter.NodesReturn property
//     TransportationAnalystParameter.EdgesReturn property
//     TransportationAnalystParameter.PathGuidesReturn property
//     TransportationAnalystParameter.StopIndexesReturn property
//     TransportationAnalystParameter.RoutesReturn property
//     TransportationAnalystResult.Routes property
//     TransportationAnalystResult.PathGuides property
//4. Steps:
// (1) Select points along the path, and barrier nodes
// (2) Get the result
//---------------------------------------------------------------------------------------
///////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SuperMap.Data;
using SuperMap.UI;
using SuperMap.Mapping;
using SuperMap.Analyst.NetworkAnalyst;

namespace _2_DatasetManager
{
	class SampleRun_find2dpath
    {
		private static String m_datasetName = "RoadNet";
		private static String m_nodeID = "SmNodeID";
		private static String m_edgeID = "SmEdgeID";
		private SelectMode m_selectMode;
		private MapControl m_mapControl;
		private Workspace m_workspace;
		private DatasetVector m_datasetLine;
		private DatasetVector m_datasetPoint;
		private Layer m_layerLine;
		private Layer m_layerPoint;
		private TrackingLayer m_trackingLayer;
		private Point2Ds m_Points;
		private GeoStyle m_style;
		private List<Int32> m_barrierNodes;
		private List<Int32> m_barrierEdges;
		private TransportationAnalyst m_analyst;
		private TransportationAnalystResult m_result;
		private DataGridView m_dataGridView;
		private Timer m_timer;
		private int m_count;
		private int m_flag;

		/// <summary>
		/// 选择模式枚举。
		/// Select mode enum
		/// </summary>
		public enum SelectMode
		{
			SelectPoint, SelectBarrier, None
		}

		/// <summary>
		/// 根据workspace、mapControl及DataGridView构造SampleRun对象。
		/// Initialize the SampleRun object with the specified workspace, mapControl, and dataGridView.
		/// </summary>
		public SampleRun_find2dpath(Workspace workspace, MapControl mapControl,
				DataGridView dataGridView)
		{
			m_workspace = workspace;
			m_mapControl = mapControl;
			m_dataGridView = dataGridView;

			m_mapControl.Map.Workspace = workspace;
			Initialize();
		}

		/// <summary>
		/// 打开网络数据集并初始化相应变量。
		/// Open the network dataset and initialize variables
		/// </summary>
		private void Initialize()
		{
			try
			{
				// 打开数据源,得到点线数据集
				// Open datasource and get the point , line datasets
				DatasourceConnectionInfo connectionInfo = new DatasourceConnectionInfo(
								 @"..\..\SampleData\City\Changchun.udb", "FindPath2D", "");
				connectionInfo.EngineType = EngineType.UDB;
				m_workspace.Datasources.Open(connectionInfo);
				m_datasetLine = (DatasetVector)m_workspace.Datasources[0].Datasets[m_datasetName];
				m_datasetPoint = m_datasetLine.ChildDataset;
				m_trackingLayer = m_mapControl.Map.TrackingLayer;
				m_trackingLayer.IsAntialias = true;

				// 初始化各变量
				// Initialzie variables
				m_flag = 1;
				m_Points = new Point2Ds();
				m_style = new GeoStyle();
				m_barrierNodes = new List<Int32>();
				m_barrierEdges = new List<Int32>();
				m_selectMode = SelectMode.SelectPoint;

				m_timer = new Timer();
				m_timer.Interval = 200;
				m_timer.Enabled = false;

				// 加载点数据集及线数据集并设置各自风格
				// Add point, line datasets and set their styles
				m_layerLine = m_mapControl.Map.Layers.Add(m_datasetLine, true);
				m_layerLine.IsSelectable = false;
				LayerSettingVector lineSetting = (LayerSettingVector)m_layerLine.AdditionalSetting;
				GeoStyle lineStyle = new GeoStyle();
				lineStyle.LineColor = Color.LightGray;
				lineStyle.LineWidth = 0.1;
				lineSetting.Style = lineStyle;

				m_layerPoint = m_mapControl.Map.Layers.Add(m_datasetPoint, true);
				LayerSettingVector pointSetting = (LayerSettingVector)m_layerPoint.AdditionalSetting;
				GeoStyle pointStyle = new GeoStyle();
				pointStyle.LineColor = Color.DarkGray;
				pointStyle.MarkerSize = new Size2D(2.5, 2.5);
				pointSetting.Style = pointStyle;

				// 调整mapControl的状态
				// Adjust the status of mapControl
				m_mapControl.Action = SuperMap.UI.Action.Select;
				m_mapControl.Map.IsAntialias = true;
				m_mapControl.IsWaitCursorEnabled = false;
				m_mapControl.Map.Refresh();

				m_mapControl.MouseDown += new MouseEventHandler(m_mapControl_MouseDown);
				m_mapControl.GeometrySelected += new GeometrySelectedEventHandler(m_mapControl_GeometrySelected);
				m_timer.Tick += new EventHandler(m_timer_Tick);
				Load();

			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// MapControl MouseDown事件。
		/// MouseDown event of MapControl
		/// </summary>
		private void m_mapControl_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Left)
				{
					Point point = new Point(e.X, e.Y);
					Point2D mapPoint = m_mapControl.Map.PixelToMap(point);
					if (m_mapControl.Map.Bounds.Contains(mapPoint))
					{
						if ((m_mapControl.Action == SuperMap.UI.Action.Select
								|| m_mapControl.Action == SuperMap.UI.Action.Select2)
								&& m_selectMode == SelectMode.SelectPoint)
						{
							AddPoint(mapPoint);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 加载环境设置对象。
		// Add the TransportationAnalystSetting object
		/// </summary>
		public void Load()
		{
			try
			{
				// 设置网络分析基本环境，这一步骤需要设置分析权重、节点、弧段标识字段、容限
				// Set the basic environment of network analysis, including weight, node, edge, tolerance.
				TransportationAnalystSetting setting = new TransportationAnalystSetting();
				setting.NetworkDataset = m_datasetLine;
				setting.EdgeIDField = m_edgeID;
				setting.NodeIDField = m_nodeID;
                if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                {
                    setting.EdgeNameField = "roadName_en";
                }
                else
                {
                    setting.EdgeNameField = "roadName";
                }
				setting.Tolerance = 89;

				WeightFieldInfos weightFieldInfos = new WeightFieldInfos();
				WeightFieldInfo weightFieldInfo = new WeightFieldInfo();
				weightFieldInfo.FTWeightField = "smLength";
				weightFieldInfo.TFWeightField = "smLength";
				weightFieldInfo.Name = "length";
				weightFieldInfos.Add(weightFieldInfo);
				setting.WeightFieldInfos = weightFieldInfos;
				setting.FNodeIDField = "SmFNode";
				setting.TNodeIDField = "SmTNode";

				// 构造交通网络分析对象，加载环境设置对象
				// Build the TransportationAnalyst object , and add environment setting object
				m_analyst = new TransportationAnalyst();
				m_analyst.AnalystSetting = setting;
				m_analyst.Load();
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 进行最短路径分析。
		/// Optimal path analysis
		/// </summary>
		/// <returns></returns>
		public bool Analyst()
		{
			try
			{
				m_count = 0;
				TransportationAnalystParameter parameter = new TransportationAnalystParameter();

				// 设置障碍点及障碍边
				// Set barrier nodes and edges
				int[] barrierEdges = new int[m_barrierEdges.Count];
				for (int i = 0; i < barrierEdges.Length; i++)
				{
					barrierEdges[i] = m_barrierEdges[i];
				}
				parameter.BarrierEdges = barrierEdges;

				int[] barrierNodes = new int[m_barrierNodes.Count];
				for (int i = 0; i < barrierNodes.Length; i++)
				{
					barrierNodes[i] = m_barrierNodes[i];
				}
				parameter.BarrierNodes = barrierNodes;
				parameter.WeightName = "length";

				// 设置最佳路径分析的返回对象
				// Set the return object of the optimal path analysis
				parameter.Points = m_Points;
				parameter.IsNodesReturn = true;
				parameter.IsEdgesReturn = true;
				parameter.IsPathGuidesReturn = true;
				parameter.IsRoutesReturn = true;

				// 进行分析并显示结果
				// Analyze
				m_result = m_analyst.FindPath(parameter, false);
				if (m_result == null)
				{
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        MessageBox.Show("Failed");
                    }
                    else
                    {
                        MessageBox.Show("分析失败");
                    }
					return false;
				}
				ShowResult();
				FillDataGridView(0);
				m_selectMode = SelectMode.None;
				return true;
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message);
				return false;
			}
		}

		/// <summary>
		/// 显示结果。
		/// Show result
		/// </summary>
		public void ShowResult()
		{
			try
			{
				// 删除原有结果
				// Delete the original result
				int count = m_trackingLayer.Count;
				for (int i = 0; i < count; i++)
				{
					int index = m_trackingLayer.IndexOf("result");
					if (index != -1)
					{
						m_trackingLayer.Remove(index);
					}
				}
				FillDataGridView(0);
				for (int i = 0; i < m_result.Routes.Length; i++)
				{
					GeoLineM geoLineM = m_result.Routes[0];
					m_style.LineColor = Color.Blue;
					m_style.LineWidth = 1;
					geoLineM.Style = m_style;
					m_trackingLayer.Add(geoLineM, "result");
				}
				m_mapControl.Map.RefreshTrackingLayer();

			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 填充DataGridView。
		/// Fill DataGridView
		/// </summary>
		/// <param name="pathNum"></param>
		public void FillDataGridView(Int32 pathNum)
		{
			try
			{
				// 清除原数据，添加初始点信息
				// Clear original data and add start point information
				m_dataGridView.Rows.Clear();
				Object[] objs = new Object[4];
				objs[0] = m_dataGridView.RowCount;
                if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                {
                    objs[1] = "Start";
                }
                else
                {
                    objs[1] = "从起始点出发";
                }
				objs[2] = "--";
				objs[3] = "--";
				m_dataGridView.Rows.Add(objs);

				// 得到行驶导引对象，根据导引子项类型的不同进行不同的填充
				// Get the PathGuide object, and make different fill according to different items
				PathGuide[] pathGuides = m_result.PathGuides;
				PathGuide pathGuide = pathGuides[pathNum];

				for (int j = 1; j < pathGuide.Count; j++)
				{
					PathGuideItem item = pathGuide[j];
					objs[0] = m_dataGridView.RowCount;

                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        // 导引子项为站点的添加方式
                        // If the item is a stop
                        if (item.IsStop)
					    {
						    String side = "None";
						    if (item.SideType == SideType.Left)
							    side = "Left";
						    if (item.SideType == SideType.Right)
							    side = "Right";
						    if (item.SideType == SideType.Middle)
							    side = "On the road";
						    String dis = item.Distance.ToString();
                            if (item.Index == -1 && item.ID == -1)
                            {
                                continue;
                            }
						    if (j != pathGuide.Count - 1)
						    {
							    objs[1] = "Arrive at [" + item.Index + " route], on the " + side
									    + dis;
						    }
						    else
						    {
							    objs[1] = "Arrive at destination " + side + dis;
						    }
						    objs[2] = "";
						    objs[3] = "";
						    m_dataGridView.Rows.Add(objs);
					    }
                        // 导引子项为弧段的添加方式
					    // If the item is an edge
                        if (item.IsEdge)
					    {
						    String direct = "Go ahead";
						    if (item.DirectionType == DirectionType.East)
							    direct = "East";
						    if (item.DirectionType == DirectionType.West)
							    direct = "West";
						    if (item.DirectionType == DirectionType.South)
							    direct = "South";
						    if (item.DirectionType == DirectionType.North)
							    direct = "North";

						    String weight = item.Weight.ToString();
						    String roadName = item.Name;
                            if (weight.Equals("0") && roadName.Equals(""))
                            {
                                objs[1] = "Go " + direct + " " + item.Length;
                                objs[2] = weight;
                                objs[3] = item.Length;
                                m_dataGridView.Rows.Add(objs);
                            }
                            else
                            {
                                String roadString = roadName.Equals("") ? "Anonymous road" : roadName;
                                objs[1] = "Go along with [" + roadString + "], " + direct + " direction"
                                        + item.Length;
                                objs[2] = weight;
                                objs[3] = item.Length;
                                m_dataGridView.Rows.Add(objs);
                            }
					    }
                    }
                    else
                    {
                        // 导引子项为站点的添加方式
                        // If the item is a stop
                        if (item.IsStop)
					    {
						    String side = "无";
						    if (item.SideType == SideType.Left)
							    side = "左侧";
						    if (item.SideType == SideType.Right)
							    side = "右侧";
						    if (item.SideType == SideType.Middle)
							    side = "道路上";
						    String dis = item.Distance.ToString();
                            if (item.Index == -1 && item.ID == -1)
                            {
                                continue;
                            }
						    if (j != pathGuide.Count - 1)
						    {
							    objs[1] = "到达[" + item.Index + "号路由点],在道路" + side
									    + dis;
						    }
						    else
						    {
							    objs[1] = "到达终点,在道路" + side + dis;
						    }
						    objs[2] = "";
						    objs[3] = "";
						    m_dataGridView.Rows.Add(objs);
					    }
                        // 导引子项为弧段的添加方式
					    // If the item is an edge
					    if (item.IsEdge)
					    {
						    String direct = "直行";
						    if (item.DirectionType == DirectionType.East)
							    direct = "东";
						    if (item.DirectionType == DirectionType.West)
							    direct = "西";
						    if (item.DirectionType == DirectionType.South)
							    direct = "南";
						    if (item.DirectionType == DirectionType.North)
							    direct = "北";

						    String weight = item.Weight.ToString();
						    String roadName = item.Name;
                            if (weight.Equals("0") && roadName.Equals(""))
                            {
                                objs[1] = "朝" + direct + "行走" + item.Length;
                                objs[2] = weight;
                                objs[3] = item.Length;
                                m_dataGridView.Rows.Add(objs);
                            }
                            else
                            {
                                String roadString = roadName.Equals("") ? "匿名路段" : roadName;
                                objs[1] = "沿着[" + roadString + "],朝" + direct + "行走"
                                        + item.Length;
                                objs[2] = weight;
                                objs[3] = item.Length;
                                m_dataGridView.Rows.Add(objs);
                            }
					    }
                    }										
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 添加分析经过点。
		/// Add points along the path
		/// </summary>
		public void AddPoint(Point2D mapPoint)
		{
			try
			{
				// 在跟踪图层上添加点
				// Add points on the tracking layer
				m_Points.Add(mapPoint);
				GeoPoint geoPoint = new GeoPoint(mapPoint);
				m_style.LineColor = Color.Green;
				m_style.MarkerSize = new Size2D(8, 8);
				geoPoint.Style = m_style;
				m_trackingLayer.Add(geoPoint, "Point");

				// 在跟踪图层上添加文本对象
				// Add text objects on the tracking layer
				TextPart part = new TextPart();
				part.X = geoPoint.X;
				part.Y = geoPoint.Y;
				part.Text = m_flag.ToString();
				m_flag++;
				GeoText geoText = new GeoText(part);
				m_trackingLayer.Add(geoText, "Point");
				m_mapControl.Map.RefreshTrackingLayer();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 对象选择事件。
		/// GeometrySelectedEvent
		/// </summary>
		public void m_mapControl_GeometrySelected(Object sender, SuperMap.UI.GeometrySelectedEventArgs e)
		{
			if (m_selectMode != SelectMode.SelectBarrier)
				return;
			Selection selection = m_layerPoint.Selection;
			if (selection.Count <= 0)
			{
				selection = m_layerLine.Selection;
			}
			m_style.LineColor = Color.Red;
			m_style.MarkerSize = new Size2D(6, 6);
			m_style.LineWidth = 0.5;
			Recordset recordset = selection.ToRecordset();
			try
			{
				Geometry geometry = recordset.GetGeometry();

				// 捕捉到点时，将捕捉到的点添加到障碍点列表中
				// If a point is snapped, the point is added to the barrier list
				if (geometry.Type == GeometryType.GeoPoint)
				{
					GeoPoint geoPoint = (GeoPoint)geometry;
					int id = recordset.GetID();
					m_barrierNodes.Add(id);

					geoPoint.Style = m_style;
					m_trackingLayer.Add(geoPoint, "barrierNode");
				}

				// 捕捉到线时，将线对象添加到障碍线列表中
				// If a line is snapped, the line is added to the barrier list
				if (geometry.Type == GeometryType.GeoLine)
				{
					GeoLine geoLine = (GeoLine)geometry;
					int id = recordset.GetID();
					m_barrierEdges.Add(id);

					geoLine.Style = m_style;
					m_trackingLayer.Add(geoLine, "barrierEdge");
				}
				m_mapControl.Map.Refresh();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
			finally
			{
				recordset.Dispose();
			}
		}

		/// <summary>
		/// 开始导引。
		/// Start guiding
		/// </summary>
		public void Play()
		{
			m_timer.Start();
		}

		/// <summary>
		/// 停止导引。
		/// Stop guiding
		/// </summary>
		public void Stop()
		{
			m_timer.Stop();
		}

		/// <summary>
		/// 清除分析结果。
		/// Clear the analysis result
		/// </summary>
		public void Clear()
		{
			try
			{
				if (m_timer != null)
					m_timer.Stop();

				m_dataGridView.Rows.Clear();
				m_flag = 1;
				m_Points = new Point2Ds();
				m_barrierNodes = new List<Int32>();
				m_barrierEdges = new List<Int32>();
				m_mapControl.Map.Layers[0].Selection.Clear();
				m_mapControl.Map.Layers[1].Selection.Clear();
				m_mapControl.Map.TrackingLayer.Clear();
				m_mapControl.Map.Refresh();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 设置选择模式
		/// Set the select mode
		/// </summary>
		/// <param name="mode"></param>
		public void SetSelectMode(SelectMode mode, Boolean canSelectLine)
		{
			m_selectMode = mode;
			m_layerLine.IsSelectable = canSelectLine;
		}

		/// <summary>
		/// 进行行驶导引。
		/// Path guide
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_timer_Tick(object sender, EventArgs e)
		{
			try
			{
				int index = m_trackingLayer.IndexOf("playPoint");
				if (index != -1)
				{
					m_trackingLayer.Remove(index);
				}

				GeoLineM lineM = m_result.Routes[0];
				PointM pointM = lineM.GetPart(0)[m_count];

				// 构造模拟对象
				// Build simulation object
				GeoPoint point = new GeoPoint(pointM.X, pointM.Y);
				GeoStyle style = new GeoStyle();
				style.LineColor = Color.Red;
				style.MarkerSize = new Size2D(5, 5);
				point.Style = style;
				m_trackingLayer.Add(point, "playPoint");

				// 跟踪对象
				// Tracking object
				m_count++;
				if (m_count >= lineM.GetPart(0).Count)
				{
					m_count = 0;
				}
				m_mapControl.Map.RefreshTrackingLayer();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}
	}
}




