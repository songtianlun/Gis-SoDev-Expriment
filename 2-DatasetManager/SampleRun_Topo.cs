///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------版权声明----------------------------
//
// 此文件为 SuperMap iObjects .NET 的示范代码
// 版权所有：北京超图软件股份有限公司
//------------------------------------------------------------------
//
//-----------------------SuperMap iObjects .NET 示范程序说明--------------------------
//
//1、范例简介：示范如何使用Topology对数据集的处理，包括线数据集构面、线数据集拓扑检查、处理等
//2、示例数据：安装目录\SampleData\Topo\TopoProcessing.smwu；
//3、关键类型/成员: 
//      TopologyProcessing.BuildRegions方法
//      TopologyProcessing.Clean方法
//      TopologyValidator.Preprocess 方法
//      TopologyValidator.Validate 方法
//      
//4、使用步骤：
//   (1)点击菜单上拓扑构面按钮，执行拓扑构面操作，并显示结果
//   (2)点击菜单上拓扑检查按钮，执行拓扑检查，并显示结果
//   (3)点击菜单上拓扑处理按钮，执行拓扑处理，并显示结果
//---------------------------------------------------------------------------------------
//------------------------------Copyright Statement----------------------------
//
// SuperMap iObjects .NET Sample Code
// Copyright: SuperMap Software Co., Ltd. All Rights Reserved.
//------------------------------------------------------------------
//
//-----------------------Description--------------------------
//
//1. Functions: we use topology to build regions by dataset, topology checking, processing, etc.
//2. Data: \SampleData\Topo\TopoProcessing.smwu
//3. Key classes and members 
//      TopologyProcessing.BuildRegions method
//      TopologyProcessing.Clean method
//      TopologyValidator.Preprocess method
//      TopologyValidator.Validate method
//      
//4. Steps:
// (1) Click Build Regions to generate regions
// (2) Click Check Topo to check topology.
// (3) Click Process Topo to process topology.
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
using SuperMap.Data.Topology;
using SuperMap.Mapping;

namespace _2_DatasetManager
{
    public class SampleRun_Topo
    {
        private Workspace m_workspace;
        private MapControl m_mapControl;
        private Dataset m_dataset;

        private String m_datasetName;
        private String m_createRegionName;
        private String m_processDatasetName;
        private String m_checkDataName;
        private String m_bufDatasetName;

        private DatasetVector m_bufDataset;
        private DatasetVector m_resultDataset;

        private TopologyProcessingOptions m_topoOptions;

        /// <summary>
        /// 根据workspace和map构造 SampleRun对象
        /// Initialize the SampleRun object with the specified workspace and map
        /// </summary>
        public SampleRun_Topo(Workspace workspace, MapControl mapControl)
        {
            try
            {
                m_workspace = workspace;
                m_mapControl = mapControl;

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
            try
            {
                // 打开工作空间及地图
                // Open the workspace and the map.
                WorkspaceConnectionInfo conInfo = new WorkspaceConnectionInfo(@"../../SampleData/Topo/TopoProcessing.smwu");
                m_datasetName = "RoadLine";
                m_createRegionName = "LineToRegion";
                m_processDatasetName = "copyDataset";
                m_checkDataName = "checkTopo";
                m_bufDatasetName = "bufDataset";

                m_workspace.Open(conInfo);
                Datasets datasets = m_workspace.Datasources[0].Datasets;
                if (datasets.Contains(m_datasetName))
                {
                    this.m_mapControl.Map.Layers.Add(datasets[m_datasetName], true);
                    m_mapControl.Map.Refresh();
                    m_dataset = datasets[m_datasetName];
                }

                // 调整mapControl的状态
                // Adjust the status of mapControl
                m_mapControl.Action = SuperMap.UI.Action.Pan;

                SetOptions();

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 构造一个全为true的TopologyProcessOption
        /// Initialize a new object of TopologyProcessingOptions. All the properties are true.
        /// </summary>
        private void SetOptions()
        {
            m_topoOptions = new TopologyProcessingOptions();
            m_topoOptions.AreAdjacentEndpointsMerged = true;
            m_topoOptions.AreDuplicatedLinesCleaned = true;
            m_topoOptions.AreLinesIntersected = true;

            m_topoOptions.ArePseudoNodesCleaned = true;
            m_topoOptions.AreRedundantVerticesCleaned = true;
            m_topoOptions.AreUndershootsExtended = true;

            m_topoOptions.AreOvershootsCleaned = true;

        }

        /// <summary>
        /// 设置图层的一些属性
        /// Set the properties of the layer
        /// </summary>
        private void SetLayerStyle(Layer layer, Color color, Double width)
        {
            try
            {
                LayerSettingVector layerSetting = new LayerSettingVector();
                GeoStyle style = new GeoStyle();
                style.FillForeColor = Color.SkyBlue;
                style.LineColor = color;
                style.LineWidth = width;
                layerSetting.Style = style;
                layer.AdditionalSetting = layerSetting;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Clear all of the layer firstly,and then add new data into the layer.
        /// </summary>
        private void ResetDatasetAddMap()
        {
            try
            {
                m_mapControl.Map.Layers.Clear();
                m_mapControl.Map.Layers.Add(m_dataset, true);
                // 拓扑构面、处理等会改变数据,并要求数据为关闭状态，所以每次都使用新的数据
                // The operation such as Build Regions and Process Topo will change the data which is in the closed state, so every operation must use the new data
                m_dataset.Datasource.Datasets.Delete(m_bufDatasetName);
                m_bufDataset = (DatasetVector)m_dataset.Datasource.CopyDataset(m_dataset, m_bufDatasetName, m_dataset.EncodeType);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 拓扑构面
        /// Build regions
        /// </summary>
        public Boolean LineToRegion()
        {
            Boolean result = false;
            // 数据不为空才执行下面的操作
            // Implement the operation when the data is not null
            if (m_dataset != null)
            {
                try
                {
                    this.ResetDatasetAddMap();
                    m_dataset.Datasource.Datasets.Delete(m_createRegionName);

                    m_resultDataset = TopologyProcessing.BuildRegions(m_bufDataset, m_workspace.Datasources[0], m_createRegionName, m_topoOptions);

                    m_mapControl.Map.Layers.Add(m_resultDataset, true);
                    SetLayerStyle(m_mapControl.Map.Layers[0], Color.Red, 0.2);
                    m_mapControl.Map.Refresh();

                    result = true;

                }
                catch (Exception ex)
                {
                    Trace.Write(ex.Message);
                    result = false;
                }
            }
            return result;
        }


        /// <summary>
        /// 拓扑处理
        /// Process topology
        /// </summary>
        public Boolean TopoProcess()
        {
            Boolean result = false;
            if (m_dataset != null)
            {
                try
                {
                    this.ResetDatasetAddMap();
                    m_dataset.Datasource.Datasets.Delete(m_processDatasetName);

                    if (TopologyProcessing.Clean(m_bufDataset, m_topoOptions))
                    {
                        m_mapControl.Map.Layers.Add(m_bufDataset, true);
                        SetLayerStyle(m_mapControl.Map.Layers[0], Color.Red, 0.2);
                        m_mapControl.Map.Refresh();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Trace.Write(ex.Message);
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 拓扑检查
        /// Check topology
        /// </summary>
        public Boolean TopoCheck()
        {
            Boolean result = false;

            if (m_dataset != null)
            {
                try
                {
                    this.ResetDatasetAddMap();

                    TopologyDatasetRelationItem topoItem = new TopologyDatasetRelationItem(m_bufDataset);
                    TopologyDatasetRelationItem[] items = { topoItem };

                    // 拓扑预处理，这个需要先调用
                    // Topology Preprocessing
                    TopologyValidator.Preprocess(items, 2);

                    // 检查线相重叠
                    // Check the topology according to the rule of LineNoOverlap
                    m_dataset.Datasource.Datasets.Delete(m_checkDataName);
                    m_resultDataset = TopologyValidator.Validate(m_bufDataset, m_bufDataset, TopologyRule.LineNoOverlap, 2, null, m_dataset.Datasource, m_checkDataName);

                    m_mapControl.Map.Layers.Add(m_resultDataset, true);
                    this.SetLayerStyle(m_mapControl.Map.Layers[0], Color.Red, 0.5);
                    m_mapControl.Map.Refresh();

                    result = true;
                }
                catch (Exception ex)
                {
                    Trace.Write(ex.Message);
                    result = false;
                }
            }
            return result;

        }

    }
}


