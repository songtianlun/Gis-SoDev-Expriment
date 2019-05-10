// ============================================================================>
// ------------------------------版权声明----------------------------
// 此文件为SuperMap iObjects .NET 的示范代码 
// 版权所有：北京超图软件股份有限公司
// ----------------------------------------------------------------
// ---------------------SuperMap iObjects .NET 示范程序说明------------------------
// 
// 1、范例简介：示范最近设施查找分析的使用
// 2、示例数据：安装目录\SampleData\City\Changchun.udb
// 3、关键类型/成员: 
// 		MapControl.MouseDown 事件
// 		MapControl.MouseMove 事件
//      TransportationAnalystSetting.NetworkDataset 属性
//      TransportationAnalystSetting.EdgeIDField 属性
//      TransportationAnalystSetting.NodeIDField 属性
//      TransportationAnalystSetting.Tolerance 属性
//      TransportationAnalystSetting.WeightFieldInfo 属性
//      TransportationAnalystSetting.FNodeIDField 属性
//      TransportationAnalystSetting.TNodeIDField 属性
//      TransportationAnalyst.Load 方法
//      TransportationAnalyst.FindClosestFacility 方法
//      TransportationAnalyst.AnalystSetting 属性
//      TransportationAnalystParameter.Points 属性
//      TransportationAnalystParameter.BarrierEdges 属性
//      TransportationAnalystParameter.BarrierNodes 属性
//      TransportationAnalystParameter.WeightName 属性
//      TransportationAnalystParameter.NodesReturn 属性
//      TransportationAnalystParameter.EdgesReturn 属性
//      TransportationAnalystParameter.PathGuidesReturn 属性
//      TransportationAnalystParameter.StopIndexesReturn 属性
//      TransportationAnalystParameter.RoutesReturn 属性
//      TransportationAnalystResult.Routes 属性
//      TransportationAnalystResult.PathGuides 属性
// 4、使用步骤：
//   (1)选取设施点、障碍点及事件点
//   (2)分析得出结果
// ------------------------------------------------------------------------------
// ============================================================================>
// Company: 北京超图软件股份有限公司
//------------------------------Copyright Statement----------------------------
//
// SuperMap iObjects .NET Sample Code
// Copyright: SuperMap Software Co., Ltd. All Rights Reserved.
//------------------------------------------------------------------
//
//-----------------------Description--------------------------
// 
// 1. Introduction: Cloesest Facility Analysis
// 2. Data: \SampleData\City\Changchun.udb
// 3. Key classes and members: 
// 	//	MapControl.MouseDown event
// 	//	MapControl.MouseMove event
//      TransportationAnalystSetting.NetworkDataset property
//      TransportationAnalystSetting.EdgeIDField property
//      TransportationAnalystSetting.NodeIDField property
//      TransportationAnalystSetting.Tolerance property
//      TransportationAnalystSetting.WeightFieldInfo property
//      TransportationAnalystSetting.FNodeIDField property
//      TransportationAnalystSetting.TNodeIDField property
//      TransportationAnalyst.Load method
//      TransportationAnalyst.FindClosestFacility method
//      TransportationAnalyst.AnalystSetting property
//      TransportationAnalystParameter.Points property
//      TransportationAnalystParameter.BarrierEdges property
//      TransportationAnalystParameter.BarrierNodes property
//      TransportationAnalystParameter.WeightName property
//      TransportationAnalystParameter.NodesReturn property
//      TransportationAnalystParameter.EdgesReturn property
//      TransportationAnalystParameter.PathGuidesReturn property
//      TransportationAnalystParameter.StopIndexesReturn property
//      TransportationAnalystParameter.RoutesReturn property
//      TransportationAnalystResult.Routes property
//      TransportationAnalystResult.PathGuides property
// 4. Steps:
//   (1) Select facility, barriers, events
//   (2) Get the result
// ------------------------------------------------------------------------------
// ============================================================================>
// Company: SuperMap Software Co., Ltd.
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
	public class SampleRun_findclosest
	{
		private Workspace m_workspace;
		private MapControl m_mapControl;
		private DataGridView m_dataGridView;
		private SelectMode m_selectMode;
		private Boolean m_selectEventNode;
		private Boolean m_selectBarrier;
		private Boolean m_selectFacilityNode;
		private List<Int32> m_barrierEdges;
		private List<Int32> m_barrierNodes;
		private List<Int32> m_nodesList;
		private DatasetVector m_datasetLine;
		private Recordset m_recordset;
		private DatasetVector m_datasetPoint;
		private Point2Ds m_Points;
		private TrackingLayer m_trackingLayer;
		private Layer m_layerLine;
		private Layer m_layerPoint;
		private GeoPoint m_geoPoint;
		private static String m_datasetName = "RoadNet";
		private static String m_nodeID = "SmNodeID";
		private static String m_edgeID = "SmEdgeID";
		private Int32 m_eventNode;
		private Point m_mousePoint;
		private TransportationAnalyst m_analyst;
		private TransportationAnalystResult m_analystResult;
		/**
		 * 选择模式枚举
		 * Select mode enum
		 */
		public enum SelectMode
		{
			SELECTPOINT, SELECTBARRIER, SELECTEVENT, SELECTPAN, NONE
		}

		/// <summary>
		/// 根据workspace和mapControl构造 SampleRun对象。
		/// Initialize the SampleRun object with the specified workspace, mapControl
		/// </summary>
		/// <param name="workspace"></param>
		/// <param name="mapControl"></param>
		/// <param name="dataGridView"></param>
		public SampleRun_findclosest(Workspace workspace, MapControl mapControl, DataGridView dataGridView)
		{
			try
			{
				m_workspace = workspace;
				m_mapControl = mapControl;
				m_dataGridView = dataGridView;

				m_mapControl.Map.Workspace = workspace;
				Initialize();               
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}
		/// <summary>
		/// 初始化控件及数据。
		/// Initialize data and control
		/// </summary>
		private void Initialize()
		{
			try
			{
				DatasourceConnectionInfo connectionInfo = new DatasourceConnectionInfo(
                    @"..\..\SampleData\City\Changchun.udb", "findClosestFacility", "");
				connectionInfo.EngineType = EngineType.UDB;
				m_workspace.Datasources.Open(connectionInfo);
				m_datasetLine = (DatasetVector)m_workspace.Datasources[0]
						.Datasets[m_datasetName] as DatasetVector;
				m_datasetPoint = m_datasetLine.ChildDataset;
				m_selectFacilityNode = true;
				m_selectBarrier = false;
				m_selectEventNode = false;
				m_Points = new Point2Ds();
				m_barrierEdges = new List<Int32>();
				m_barrierNodes = new List<Int32>();
				m_selectMode = SelectMode.SELECTPOINT;
				m_nodesList = new List<Int32>();
				m_trackingLayer = m_mapControl.Map.TrackingLayer;

				// 加载点数据集及线数据集并设置各自风格
				// Add point, line datasets and set their styles
				m_layerLine = m_mapControl.Map.Layers.Add(m_datasetLine, true);
				LayerSettingVector lineSetting = (LayerSettingVector)m_layerLine
						.AdditionalSetting;
				GeoStyle lineStyle = new GeoStyle();
				lineStyle.LineColor = Color.LightGray;
				lineStyle.LineWidth = 0.1;
				lineSetting.Style = lineStyle;

				m_layerPoint = m_mapControl.Map.Layers.Add(
						m_datasetPoint, true);
				LayerSettingVector pointSetting = (LayerSettingVector)m_layerPoint
						.AdditionalSetting;
				GeoStyle pointStyle = new GeoStyle();
				pointStyle.LineColor = Color.DarkGray;
				pointStyle.MarkerSize = new Size2D(2.5, 2.5);
				pointSetting.Style = pointStyle;

				// 调整mapControl的状态
				// Adjust the status of mapControl
				m_mapControl.Action = SuperMap.UI.Action.Select;
				m_mapControl.IsWaitCursorEnabled = false;
                m_mapControl.Map.Refresh();
				m_mapControl.MouseDown += new MouseEventHandler(m_mapControl_MouseDown);
				m_mapControl.MouseMove += new MouseEventHandler(m_mapControl_MouseMove);

                // 加载模型
                // Add model
                Load();
			}
			catch (System.Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 加载图层。
		/// Add layer
		/// </summary>
		private void Load()
		{
			try
			{
				// 设置网络分析基本环境，这一步骤需要设置　分析权重、节点、弧段标识字段、容限
				// Set the basic environment of network analysis, including weight, node, edge, tolerance.
                TransportationAnalystSetting setting = new TransportationAnalystSetting();
                setting.NetworkDataset = m_datasetLine;
                setting.EdgeIDField = m_edgeID;               
                if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                {
                    setting.EdgeNameField = "roadName_en";
                }
                else
                {
                    setting.EdgeNameField = "roadName";
                }
                setting.NodeIDField = m_nodeID;
                setting.Tolerance = 0.01559;

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
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 清空跟踪层。
		/// Clear tracking layer
		/// </summary>
		public void ClearAll()
		{
			try
			{
				m_dataGridView.Rows.Clear();
				m_mapControl.Map.Layers[0].Selection.Clear();
				m_mapControl.Map.Layers[1].Selection.Clear();
				m_mapControl.Map.TrackingLayer.Clear();

				m_nodesList.RemoveRange(0, m_nodesList.Count);
				m_barrierEdges.RemoveRange(0, m_barrierEdges.Count);
				m_barrierNodes.RemoveRange(0, m_barrierNodes.Count);
                m_eventNode = -1;
				m_mapControl.Map.Refresh();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 更换鼠标状态。
		/// Change the mouse status
		/// </summary>
		/// <param name="value"></param>
		public void ChangeAction(SelectMode mode)
		{
			try
			{
                if (mode == SelectMode.SELECTPAN)
				{
					m_mapControl.Map.Layers[0].IsEditable = false;
				}
                m_selectMode = mode;
				m_mapControl.Map.Refresh();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 开始分析。
		/// Analyze
		/// </summary>
		public void StartAnalyst()
		{
			try
			{
                int index = m_trackingLayer.IndexOf("route");
                if (index != -1)
                {
                    m_trackingLayer.Remove(index);
                }
                m_mapControl.Map.Refresh();
               
				TransportationAnalystParameter parameter = new TransportationAnalystParameter();
				// 设置设施点			
				// Set facilities			
				int[] facilityNodes = new int[m_nodesList.Count];
				for (int i = 0; i < facilityNodes.Length; i++)
				{
					facilityNodes[i] = m_nodesList[i];
				}
				parameter.Nodes = facilityNodes;
				parameter.WeightName = "length";
				parameter.IsEdgesReturn = true;
				parameter.IsNodesReturn = true;
				parameter.IsRoutesReturn = true;
				parameter.IsPathGuidesReturn = true;

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

				// 进行分析，这里设置查找设施点的数量为1
				// The facility number here is 1
				m_analystResult = m_analyst.FindClosestFacility(parameter,
						m_eventNode, 1, true, 0);
				if (m_analystResult == null)
				{
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        MessageBox.Show("Failed");
                    }
                    else
                    {
                        MessageBox.Show("分析失败");
                    }
					return;
				}
				else
				{
					ShowResult();
					FillResultTable(0);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
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
				GeoLineM[] geoLineMs = m_analystResult.Routes;
				GeoLineM geoLineM = geoLineMs[0];
				GeoStyle style = new GeoStyle();
				style.LineColor = Color.Blue;
				style.LineWidth = 1;
				geoLineM.Style = style;
                for (Int32 i = 0; i < m_trackingLayer.Count; i++ )
                {
                    // 清除上次结果
                    // Clear the last result
                    if (m_trackingLayer.Get(i).Type == GeometryType.GeoLineM)
                    {
                        m_trackingLayer.Remove(i);
                    }
                }
                m_trackingLayer.Add(geoLineM, "route");
                
				m_mapControl.Map.Refresh();
			}
			catch (System.Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 显示结果。
		/// Show result
		/// </summary>
		/// <param name="pathNum"></param>
		public void FillResultTable(Int32 pathNum)
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
				PathGuide[] pathGuides = m_analystResult.PathGuides;
				PathGuide pathGuide = pathGuides[pathNum];

				for (int j = 1; j < pathGuide.Count; j++)
				{
					PathGuideItem item = pathGuide[j];
					objs[0] = m_dataGridView.RowCount;
					// 导引子项为站点的添加方式
					// If the item is a stop
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
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
                    }
                    else
                    {
                        if (item.IsStop)
                        {
                            String side = "无";
                            if (item.SideType == SideType.Left)
                                side = "左侧";
                            if (item.SideType == SideType.Right)
                                side = "右侧";
                            if (item.SideType == SideType.Middle)
                                side = "上";
                            String dis = item.Distance.ToString();

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
                    }
					
					// 导引子项为弧段的添加方式
					// If the item is a edge
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
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
                            String roadString = roadName.Equals("") ? "Anonymous road" : roadName;
                            objs[1] = "Go along with [" + roadString + "], " + direct + " direction"
                                    + weight;
                            objs[2] = weight;
                            objs[3] = item.Length;
                            m_dataGridView.Rows.Add(objs);
                        }
                    }
                    else
                    {
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
                            String roadString = roadName.Equals("") ? "匿名路段" : roadName;
                            objs[1] = "沿着[" + roadString + "],朝" + direct + "行走"
                                    + weight;
                            objs[2] = weight;
                            objs[3] = item.Length;
                            m_dataGridView.Rows.Add(objs);
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
		/// 选择设施点。
		/// Select facility
		/// </summary>
		public void FindFacilityNodes()
		{
			try
			{
				Recordset recordset = m_layerPoint.Selection.ToRecordset();
				Geometry geometry = recordset.GetGeometry();
				m_selectFacilityNode = true;
				m_selectBarrier = false;
				m_selectEventNode = false;
				int id = m_recordset.GetID();
				if (geometry.Type == GeometryType.GeoPoint)
				{
					m_nodesList.Add(m_recordset.GetID());
					m_trackingLayer.Add(m_geoPoint, "FacilityNode" + id);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 选择障碍点。
		/// Select barriers
		/// </summary>
		public void FindBarrierNodes()
		{
			try
			{
				Recordset recordset = m_layerPoint.Selection.ToRecordset();
				Geometry geometry = recordset.GetGeometry();
				m_selectFacilityNode = false;
				m_selectBarrier = true;
				m_selectEventNode = false;
				Int32 id = m_recordset.GetID();
				if (geometry.Type == GeometryType.GeoPoint)
				{
					m_nodesList.Add(m_recordset.GetID());
					m_trackingLayer.Add(m_geoPoint, "FacilityNode" + id);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 选择事件点。
		/// Select events
		/// </summary>
		public void FindEventNode()
		{
			try
			{
				Recordset recordset = m_layerPoint.Selection.ToRecordset();
				Geometry geometry = recordset.GetGeometry();
				m_selectFacilityNode = false;
				m_selectBarrier = false;
				m_selectEventNode = true;
				m_eventNode = m_recordset.GetID();
				if (geometry.Type == GeometryType.GeoPoint)
				{
					int index = m_trackingLayer.IndexOf("EventNode");
					if (index != -1)
						m_trackingLayer.Remove(index);
					m_trackingLayer.Add(m_geoPoint, "EventNode");
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// MapControl MouseMove事件。
		/// MapControl MouseMove event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_mapControl_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				m_mapControl.DoMouseMove(e);
				if (m_mapControl.Action == SuperMap.UI.Action.Select
						|| m_mapControl.Action == SuperMap.UI.Action.Select2)
				{
					// 获取鼠标点对应的地图点
					// Get the map point that corresponds to the mouse point
					m_mousePoint = new Point(e.X, e.Y);
					Point2D point2D = m_mapControl.Map.PixelToMap(m_mousePoint);

					// 根据当前比例尺设置捕捉框的大小
					// Set the snap box size according to the current scale
					double scale = (3 * 10E-4) / m_mapControl.Map.Scale;
					Selection selection = m_layerPoint.HitTest(point2D, 4 / 3 * scale);
					int index = m_trackingLayer.IndexOf("geoLine");
					if (index != -1)
						m_trackingLayer.Remove(index);
					if (selection != null && selection.Count > 0)
					{
						Recordset recordset = selection.ToRecordset();
						GeoPoint geoPoint = (GeoPoint)recordset.GetGeometry();
						recordset.Dispose();
						double pointX = geoPoint.X;
						double pointY = geoPoint.Y;

						// 构造捕捉框
						// Build snap box
						Point2Ds point2Ds = new Point2Ds();
						point2Ds.Add(new Point2D(pointX - scale, pointY - scale));
						point2Ds.Add(new Point2D(pointX + scale, pointY - scale));
						point2Ds.Add(new Point2D(pointX + scale, pointY + scale));
						point2Ds.Add(new Point2D(pointX - scale, pointY + scale));
						point2Ds.Add(new Point2D(pointX - scale, pointY - scale));
						GeoLine geoLine = new GeoLine(point2Ds);

						// 刷新地图
						// Refresh the map
						m_mapControl.SelectionTolerance = 2;
						m_trackingLayer.Add(geoLine, "geoLine");
						m_mapControl.Map.Refresh();
					}
				}
			}
			catch (System.Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// MapControl MouseDown事件
		/// MapControl MouseDown event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_mapControl_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Left)
				{
					Selection selection = m_layerPoint.Selection;
					if (selection == null || selection.Count == 0)
					{
						selection = m_layerLine.Selection;
					}
					if (m_mapControl.Action == SuperMap.UI.Action.Select && e.Clicks == 1
						&& (m_selectMode == SelectMode.SELECTPOINT || m_selectMode == SelectMode.SELECTBARRIER
						|| m_selectMode == SelectMode.SELECTEVENT))
					{
						if (selection.Count <= 0)
						{					
                            if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                            {
                                MessageBox.Show("The coordinates exceed the tolerance. Invalid!");
                            }
                            else
                            {
                                MessageBox.Show("坐标点超出选择容限，不能作为分析点");
                            }
						}
						else
						{
							//根据选择的不同，构造点对象
							m_recordset = selection.ToRecordset();
							Geometry geometry = m_recordset.GetGeometry();
							AddPoint(geometry);
							m_recordset.Dispose();
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 添加分析经过点。
		/// Add points
		/// </summary>
		/// <param name="geometry"></param>
		public void AddPoint(Geometry geometry)
		{
			try
			{
				// 在跟踪图层上添加点
				// Add points on the tracking layer
				GeoPoint geoPoint = new GeoPoint();
				if (geometry.Type == GeometryType.GeoPoint)
				{
					geoPoint = (GeoPoint)geometry;
				}
				else
				{
					geoPoint = new GeoPoint(((GeoLine)geometry).InnerPoint);
				}
				GeoStyle style = new GeoStyle();
				if (m_selectFacilityNode)
				{
					style.MarkerSize = new Size2D(10, 10);
					style.LineColor = Color.LightGreen;
					geoPoint.Style = style;
					int id = m_recordset.GetID();
					if (geometry.Type == GeometryType.GeoPoint)
					{
						m_nodesList.Add(m_recordset.GetID());
						m_trackingLayer.Add(geoPoint, "FacilityNode" + id);
					}
				}
				else if (m_selectBarrier)
				{
					style.LineColor = Color.Red;
					style.MarkerSymbolID = 8622;
					style.MarkerSize = new Size2D(10, 10);
					geoPoint.Style = style;

					// 构造障碍点
					// Build barriers
					Int32 id = m_recordset.GetID();
					if (geometry.Type == GeometryType.GeoPoint)
					{
						m_barrierNodes.Add(m_recordset.GetID());
						m_trackingLayer.Add(geoPoint, "barrierNode" + id);
					}
					else
					{
						m_barrierEdges.Add(m_recordset.GetID());
						m_trackingLayer.Add(geoPoint, "barrierEdge" + id);
					}

					m_mapControl.Map.Refresh();
				}
				else if (m_selectEventNode)
				{
					style.MarkerSize = new Size2D(10, 10);
					style.LineColor = Color.Orange;
					geoPoint.Style = style;
					m_eventNode = m_recordset.GetID();
					if (geometry.Type == GeometryType.GeoPoint)
					{
						int index = m_trackingLayer.IndexOf("EventNode");
						if (index != -1)
						{
							m_trackingLayer.Remove(index);
							m_mapControl.Map.Refresh();
						}
						m_trackingLayer.Add(geoPoint, "EventNode");
					}
                    else
                    {
                        int index = m_trackingLayer.IndexOf("EventNode");
                        if (index != -1)
                        {
                            m_trackingLayer.Remove(index);
                            m_mapControl.Map.Refresh();
                        }
                        m_eventNode = -1;                       
                        if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                        {
                            MessageBox.Show("Failed. Reselect, please.");
                        }
                        else
                        {
                            MessageBox.Show("选择失败，请重新选择。");
                        }
                    }  
				}
				geoPoint.Style = style;

				m_mapControl.Map.Refresh();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}
	}
}


