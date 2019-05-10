///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------版权声明----------------------------
//
// 此文件为 SuperMap iObjects .NET 的示范代码
// 版权所有：北京超图软件股份有限公司
//------------------------------------------------------------------
//
//-----------------------SuperMap iObjects .NET 示范程序说明--------------------------
//
// 1、范例简介：示范如何对数据进行空间查询，并在MapControl中展示出来
// 2、示例数据：安装目录\SampleData\World\World.smwu；
// 3、关键类型/成员: 
//      Workspace.Open 方法
//      QueryParameter.SpatialQueryObject 属性
//      SpatialQueryMode.Contain 枚举
//      SpatialQueryMode.Intersect 枚举
//      SpatialQueryMode.Disjoint 枚举
//      DatasetVector.Query 方法
//      Layer.Selection 属性
//      
// 4、使用步骤：
//   (1)在地图上选择对象作为查询对象。
//   (2)点击相应的按钮进行相关的查询，查询结果在地图中以选择集的方式展现出来。
//---------------------------------------------------------------------------------------
//------------------------------Copyright Statement----------------------------
//
// This is the Sample Code of SuperMap iObjects .NET 
// SuperMap Software Co., Ltd. All Rights Reserved. 
//------------------------------------------------------------------
//
//-----------------------The description of SuperMap iObjects .NET Sample Code --------------------------
//
//1.Sample Code Description:This sample demonstrates how to implement spatial query and display the result in the mapcontrol. 
//2.Sample Data:Installation directory\SampleData\World\World.smw
//3.Key Classes/Methods: 
//      QueryParameter Class
//      QueryParameter.SpatialQueryObject Property
//      SpatialQueryMode.Contain Enum
//      SpatialQueryMode.Intersect Enum
//      SpatialQueryMode.Disjoint Enum
//      DatasetVector.Query method
//      Layer.Selection prperty
//      
//4.Procedures:
//   (1)Select an object in the map as query object.
//   (2)Click the buttons to implement corresponding query, the result will be displayed in the style of selection
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

namespace _2_DatasetManager
{
    public class SampleRun_quare
    {
        private Workspace m_workspace;
        private MapControl m_mapControl;

        // 查询用到的数据
		// Data for querying.
        private  String m_mapName = "";
        private readonly String m_queryObjectLayerName = "Ocean";
        private readonly String m_queriedLayerName = "World";
        
        /// <summary>
        /// 根据workspace构造SampleRun对象
        /// Initialize the SampleRun object with the specified workspace
        /// </summary>
        public SampleRun_quare(Workspace workspace, MapControl mapControl)
        {
            m_workspace = workspace;
            m_mapControl = mapControl;
           
            m_mapControl.Map.Workspace = workspace;
            InitializeCultureResources();
            Initialize();
        }

        private void InitializeCultureResources()
        {
            if (SuperMap.Data.Environment.CurrentCulture == "zh-CN")
            {
                m_mapName = "世界地图";
            }
            else
            {
                m_mapName = "WorldMap";
            }
        }

        /// <summary>
        /// 打开需要的工作空间文件及地图
		/// Open the workspace and the map
        /// </summary>
        private void Initialize()
        {
            try
            {


                // 打开工作空间及地图
				// Open the workspace and the map
                WorkspaceConnectionInfo conInfo = new WorkspaceConnectionInfo(@"..\..\SampleData\World\World.smwu");
                m_workspace.Open(conInfo);

                this.m_mapControl.Map.Open(m_mapName);
                this.m_mapControl.Map.Refresh();

                // 调整mapControl的状态
                // Adjust the condition of mapControl
                for (int i = 0; i < m_mapControl.Map.Layers.Count; i++)
                {
                    Layer layer = m_mapControl.Map.Layers[i];
                    if (layer.Caption!=m_queryObjectLayerName)
                    {
                        layer.IsSelectable = false;
                    }
                    else
                    {
                        layer.IsSelectable = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 按照各种算子进行查询
		/// Query with the specified parameters
        /// </summary>
        /// <param name="mode"></param>
        public void Query(SpatialQueryMode mode)
        {
            try
            {
                // 获取地图中的选择集，并转换为记录集
				// Get the dataset in the map, and convert it to recordset.
                Selection[] selections = m_mapControl.Map.FindSelection(true);
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
                layer.Selection.FromRecordset(recordset2);

                layer.Selection.Style.LineColor = Color.Red;
                layer.Selection.Style.LineWidth = 0.6;
                layer.Selection.SetStyleOptions(StyleOptions.FillSymbolID, true);
                layer.Selection.Style.FillSymbolID = 1;
                layer.Selection.IsDefaultStyleEnabled = false;
               
                recordset2.Dispose();

                // 刷新地图
				// Refresh the map.
                m_mapControl.Map.Refresh();

                recordset.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private Layer GetLayerByCaption(String layerCaption)
        {
            Layers layers = m_mapControl.Map.Layers;
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
    }
}


