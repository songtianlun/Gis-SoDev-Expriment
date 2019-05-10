///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------版权声明----------------------------
//
// 此文件为 SuperMap iObjects .NET 的示范代码
// 版权所有：北京超图软件股份有限公司
//------------------------------------------------------------------
//
//-----------------------SuperMap iObjects .NET 示范程序说明--------------------------
//
//1、范例简介：示范如何使用CoordSysTranslator进行投影转换
//2、示例数据：安装目录/SampleData/China/China400.smwu；
//3、关键类型/成员: 
//      CoordSysTranslator.Convert 方法
//      PrjCoordSys
//      
//4、使用步骤：
//   (1)点击高斯-克吕格投影转换按钮，显示投影转换结果。
//   (2)点击 UTM 投影转换按钮，显示投影转换结果。
//   (3)点击兰勃托投影转换按钮，显示投影转换结果。
//   
//---------------------------------------------------------------------------------------
///////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------Copyright Statement----------------------------
//
// SuperMap iObjects .NET Sample Code
// Copyright: SuperMap Software Co., Ltd. All Rights Reserved.
//------------------------------------------------------------------
//
//-----------------------Description--------------------------
//
//1. Sample Code Description: This sample demonstrates how to transform the projection coordinate system.
//2. Data: SampleData/China/China400.smwu；
//3. Key classes and members 
//      CoordSysTranslator.Convert method
//      PrjCoordSys
//      
//4. Steps:
//1. Click Gauss-Kruger
//2. Click UTM
//3. Click Lambert
//   
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
using SuperMap.Analyst.SpatialAnalyst;

namespace _2_DatasetManager
{
    public class SampleRun_Buffer
    {
        private Workspace m_workspace;
        private MapControl m_srcMapControl;
        //private MapControl m_targMapControl;
        private Dataset m_dataset;
        private Dataset m_bufferDataset;
        private String m_bufferDataName;
        DatasetVector result; //缓冲区
        private readonly String m_queriedLayerName = "School@changchun#1";


        /// <summary>
        /// 根据workspace和map构造 SampleRun对象
        /// Initialize the SampleRun object with the specified workspace and map
        /// </summary>
        public SampleRun_Buffer(Workspace workspace, MapControl srcMapControl)
        {
            
            try
            {
                m_workspace = workspace;
                m_srcMapControl = srcMapControl;
                Initialize();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

       

        /// <summary>
        /// 打开需要的工作空间文件及地图
		/// Open the workspace and the map
        /// </summary>
        private void Initialize()
        {
            m_workspace.Open(new WorkspaceConnectionInfo(@"..\..\City\Changchun.smwu"));

            //建立MapControl与Workspace的连接
            m_srcMapControl.Map.Workspace = m_workspace;
            
            //判断工作空间中是否有地图
            if (m_workspace.Maps.Count == 0)
            {
                MessageBox.Show("当前工作空间中不存在地图!");
                return;
            }

            //打开第一幅地图
            Map map = m_srcMapControl.Map;
            map.Workspace = m_workspace;
            map.Open(m_workspace.Maps[0]);

 
            //刷新地图窗口
            m_srcMapControl.Map.Refresh();


            m_bufferDataName = "buffer";
            m_dataset = m_workspace.Datasources[0].Datasets["Frame_R"];
            //if (m_workspace != null)
            //{
            //    try
            //    {
            //        m_workspace.Open(new WorkspaceConnectionInfo(@"../../SampleData/China/China400.smwu"));

            //        m_dataset = m_workspace.Datasources[0].Datasets["County_R"];

            //        m_srcMapControl.Map.Layers.Add(m_dataset, true);
            //        m_srcMapControl.Map.ViewEntire();

            //        m_bufPrjDataName = "bufPrj";
            //    }
            //    catch (Exception ex)
            //    {
            //        Trace.Write(ex.Message);
            //    }
            //}
        }

        /// <summary>
        /// 删除临时数据集，并创建新数据
		/// Delete the temporary dataset and create new data
        /// </summary>
        private void CopyDataset(String name)
        {
            try
            {
                m_dataset.Datasource.Datasets.Delete(name);

                DatasetVectorInfo datasetVectorInfo = new DatasetVectorInfo();
                datasetVectorInfo.Type = DatasetType.Region;
                datasetVectorInfo.IsFileCache = true;
                datasetVectorInfo.Name = name;

                // 创建矢量数据集
                m_bufferDataset = m_dataset.Datasource.Datasets.Create(datasetVectorInfo);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void CreatBuffer()
        {
            
            try
            {
                // 获取地图中的选择集，并转换为记录集
                // Get the dataset in the map, and convert it to recordset.
                Selection[] selections = m_srcMapControl.Map.FindSelection(true);
                Selection selection = selections[0];
                Recordset recordset = selection.ToRecordset();

                this.CopyDataset(m_bufferDataName);

                //PrjCoordSys gaussPrjSys = this.GetTargetPrjCoordSys(type);
                //Boolean result = CoordSysTranslator.Convert(m_processDataset, gaussPrjSys, new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);


                //设置缓冲区分析参数
                BufferAnalystParameter bufferAnalystParam = new BufferAnalystParameter();
                bufferAnalystParam.EndType = BufferEndType.Flat;
                bufferAnalystParam.LeftDistance = 100;
                bufferAnalystParam.RightDistance = 100;

                this.result = m_bufferDataset as DatasetVector;
                //调用创建矢量数据集缓冲区方法
                Boolean istrue = BufferAnalyst.CreateBuffer(recordset, result, bufferAnalystParam, false, true);

                Recordset recordset2 = result.GetRecordset(false, SuperMap.Data.CursorType.Dynamic);

                m_srcMapControl.Map.Layers.Add(result, true);
                m_srcMapControl.Map.Center = result.Bounds.Center;
                m_srcMapControl.Map.Scale = m_srcMapControl.Map.Scale;
                m_srcMapControl.Map.Refresh();

                recordset2.Dispose();

                // 刷新地图
                // Refresh the map.
                m_srcMapControl.Map.Refresh();

                recordset.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void QuarebuBuffer()
        {
            Recordset recordset = result.GetRecordset(false, SuperMap.Data.CursorType.Dynamic); //获取缓冲区记录集

            // 设置查询参数
            // Set the query parameter.
            QueryParameter parameter = new QueryParameter();
            parameter.SpatialQueryObject = recordset;
            parameter.SpatialQueryMode = SuperMap.Data.SpatialQueryMode.Contain; //包含查询


            // 对指定查询的图层进行查询
            // Query the specified layer.
            Layer layer = this.GetLayerByCaption(m_queriedLayerName);
            DatasetVector quare_dataset = layer.Dataset as DatasetVector;
            Recordset recordset2 = quare_dataset.Query(parameter);
            layer.Selection.FromRecordset(recordset2);

            layer.Selection.Style.LineColor = Color.Red;
            layer.Selection.Style.LineWidth = 0.6;
            layer.Selection.SetStyleOptions(StyleOptions.FillSymbolID, true);
            layer.Selection.Style.FillSymbolID = 1;
            layer.Selection.IsDefaultStyleEnabled = false;

            // 刷新地图
            // Refresh the map.
            m_srcMapControl.Map.Refresh();
        }



        /// <summary>
        /// 转换主函数
        /// Main function of transformation
        /// </summary>
        /// <param name="type">对应不同的投影类型 Projection type</param>
        /// <returns>转换后投影的描述信息 The description information of projection after transformed</returns>
        //public String TransformPrj(int type)
        //{
        //    try
        //    {
        //        m_srcMapControl.Map.Layers.Clear();
        //        this.CopyDataset(m_bufPrjDataName);

        //        PrjCoordSys gaussPrjSys = this.GetTargetPrjCoordSys(type);
        //        Boolean result = CoordSysTranslator.Convert(m_processDataset, gaussPrjSys, new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);

        //        m_srcMapControl.Map.Layers.Add(m_processDataset, true);
        //        m_srcMapControl.Map.Center = m_srcMapControl.Map.Bounds.Center;
        //        m_srcMapControl.Map.Scale = m_srcMapControl.Map.Scale;
        //        m_srcMapControl.Map.Refresh();

        //        return this.GetPrjStr(m_processDataset);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(ex.Message);
        //    }
        //    return null;
        //}

        // 按照不同的投影类型，初始化投影坐标系
        // Initialize the projection coordinates by the different projection type.
        //private PrjCoordSys GetTargetPrjCoordSys(int type)
        //{
        //    PrjCoordSys targetPrjCoordSys = null;
        //    PrjParameter parameter = null;
        //    Projection projection = null;

        //    switch (type)
        //    {
        //        case 1:
        //            {
        //                targetPrjCoordSys = new PrjCoordSys(PrjCoordSysType.UserDefined);
        //                projection = new Projection(ProjectionType.GaussKruger);
        //                targetPrjCoordSys.Projection = projection;
        //                parameter = new PrjParameter();
        //                parameter.CentralMeridian = 110;
        //                parameter.StandardParallel1 = 20;
        //                parameter.StandardParallel2 = 40;
        //                targetPrjCoordSys.PrjParameter = parameter;
        //            }
        //            break;
        //        case 2:
        //            {
        //                targetPrjCoordSys = new PrjCoordSys(PrjCoordSysType.UserDefined);
        //                projection = new Projection(ProjectionType.TransverseMercator);
        //                targetPrjCoordSys.Projection = projection;
        //                parameter = new PrjParameter();
        //                parameter.CentralMeridian = 110;
        //                parameter.StandardParallel1 = 0;
        //                targetPrjCoordSys.PrjParameter = parameter;
        //            }
        //            break;
        //        case 3:
        //            {
        //                targetPrjCoordSys = new PrjCoordSys(PrjCoordSysType.UserDefined);
        //                projection = new Projection(ProjectionType.LambertConformalConic);
        //                targetPrjCoordSys.Projection = projection;
        //                parameter = new PrjParameter();
        //                parameter.CentralMeridian = 110;
        //                parameter.StandardParallel1 = 30;
        //                targetPrjCoordSys.PrjParameter = parameter;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    return targetPrjCoordSys;
        //}

        private Layer GetLayerByCaption(String layerCaption)
        {
            Layers layers = m_srcMapControl.Map.Layers;
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


