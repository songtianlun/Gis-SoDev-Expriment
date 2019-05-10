using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SuperMap.Data;
using SuperMap.UI;
using SuperMap.Mapping;
using SuperMap.Data.Conversion;
using System.IO;

namespace _2_DatasetManager
{
    class SampleRun
    {
        private Workspace m_workspace;
        private MapControl m_mapControl;

        private Datasource m_srcDatasource;
        private Datasource m_desDatasource;

        private DatasetVector m_sourceRegion;
        private DatasetImage m_sourceImg;

        private DataExport m_dataExport;
        private DataImport m_dataImport;

        private String m_mapName = "";
        private readonly String m_queryObjectLayerName = "Ocean";
        private readonly String m_queriedLayerName = "World";


        /// <summary>
        /// 根据workspace和map构造 SampleRun对象
		/// Initialize the SampleRun object with the specified workspace and map
        /// </summary>
        public SampleRun(Workspace workspace, MapControl mapControl)
        {
            try
            {
                m_workspace = workspace;
                m_mapControl = mapControl;

                m_mapControl.Map.Workspace = workspace;
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
                WorkspaceConnectionInfo conInfo = new WorkspaceConnectionInfo(@"..\..\SampleData\DataExchange\DataExchange.smwu");

                m_workspace.Open(conInfo);
                m_srcDatasource = m_workspace.Datasources["SrcDatasource"];
                m_desDatasource = m_workspace.Datasources["DesDatasource"];

                m_sourceRegion = m_srcDatasource.Datasets["China400"] as DatasetVector;
                m_sourceImg = m_srcDatasource.Datasets["Day"] as DatasetImage;

                m_dataExport = new DataExport();
                m_dataImport = new DataImport();

                this.m_mapControl.Map.Layers.Add(m_sourceRegion, true);
                this.m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 设置导出设置
		/// Make the ExportSettings
        /// </summary>
        private void SetExportSettings(SuperMap.Data.Conversion.FileType type, String targetFilePath)
        {
            try
            {
                ExportSetting setting = new ExportSetting();
                setting.TargetFilePath = targetFilePath;
                setting.TargetFileType = type;
                setting.IsOverwrite = true;

                if (type == FileType.SIT)
                {
                    setting.SourceData = m_sourceImg;
                    m_dataExport.ExportSettings.Add(setting);
                }
                else
                {
                    setting.SourceData = m_sourceRegion;
                    m_dataExport.ExportSettings.Add(setting);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导出为Tab
        /// Export to Tab
        /// </summary>
        public void ExportToTab()
        {
            try
            {
                String targetDirectory = @"..\..\SampleData\DataExchange\TabExport";

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                String targetFilePath = targetDirectory + @"\ExportTab.tab";
                this.SetExportSettings(FileType.TAB, targetFilePath);

                m_mapControl.Map.Layers.Clear();
                m_dataExport.Run();
                m_mapControl.Map.Layers.Add(m_sourceRegion, true);
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();

                System.Diagnostics.Process.Start("explorer.exe", "/n,/select, " + targetFilePath);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导出为Shape
		/// Export to Shape
        /// </summary>
        public void ExportToShape()
        {
            try
            {
                String targetDirectory = @"..\..\SampleData\DataExchange\ShpExport";

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                String targetFilePath = targetDirectory + @"\ExportShape.shp";
                this.SetExportSettings(FileType.SHP, targetFilePath);

                m_mapControl.Map.Layers.Clear();
                m_dataExport.Run();
                m_mapControl.Map.Layers.Add(m_sourceRegion, true);
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();

                System.Diagnostics.Process.Start("explorer.exe", "/n,/select, " + targetFilePath);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导出为Sit
		/// Export to Sit
        /// </summary>
        public void ExportToSit()
        {
            try
            {
                String targetDirectory = @"..\..\SampleData\DataExchange\SitExport";

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                String targetFilePath = targetDirectory + @"\SitExport.sit";
                this.SetExportSettings(FileType.SIT, targetFilePath);

                m_mapControl.Map.Layers.Clear();
                m_dataExport.Run();
                m_mapControl.Map.Layers.Add(m_sourceImg, true);
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();

                System.Diagnostics.Process.Start("explorer.exe", "/n,/select, " + targetFilePath);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导入为Wor
		/// Import to Wor
        /// </summary>
        public void ImportToWor()
        {
            try
            {
                m_dataImport.ImportSettings.Clear();

                ImportSettingWOR worSetting = new ImportSettingWOR();
                worSetting.ImportMode = ImportMode.Overwrite;
                worSetting.SourceFilePath = @"..\..\SampleData\DataExchange\WorImport\Jingjin.wor";
                worSetting.TargetDatasource = m_desDatasource;
                worSetting.TargetWorkspace = m_workspace;

                m_dataImport.ImportSettings.Add(worSetting);
                m_dataImport.Run();

                m_mapControl.Map.Close();
                m_mapControl.Map.Open(m_workspace.Maps[0]);
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导入为Dwg
		/// Import to Dwg
        /// </summary>
        public void ImportToDwg()
        {
            try
            {
                m_dataImport.ImportSettings.Clear();

                ImportSettingDWG dwgSetting = new ImportSettingDWG();
                dwgSetting.ImportMode = ImportMode.Overwrite;
                dwgSetting.SourceFilePath = @"..\..\SampleData\DataExchange\DwgImport\Polyline.dwg";
                dwgSetting.TargetDatasource = m_desDatasource;
                dwgSetting.ImportingAsCAD = false;

                m_dataImport.ImportSettings.Add(dwgSetting);
                m_dataImport.Run();

                DatasetVector importResult = m_desDatasource.Datasets["PolylineL"] as DatasetVector;

                m_mapControl.Map.Layers.Clear();
                m_mapControl.Map.Layers.Add(importResult, true);
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导入为Dwg,保留参数化对象
		/// Import to Dwg, and reserve the parameterized objects
        /// </summary>
        public void ImportToDwg1()
        {
            try
            {
                m_dataImport.ImportSettings.Clear();

                ImportSettingDWG dwgSetting = new ImportSettingDWG();
                dwgSetting.ImportMode = ImportMode.Overwrite;
                dwgSetting.SourceFilePath = @"..\..\SampleData\DataExchange\DwgImport\ParametricPart.dwg";
                dwgSetting.TargetDatasource = m_desDatasource;
                dwgSetting.ImportingAsCAD = true;
                dwgSetting.KeepingParametricPart = true;

                m_dataImport.ImportSettings.Add(dwgSetting);
                m_dataImport.Run();

                DatasetVector importResult = m_desDatasource.Datasets["ParametricPart"] as DatasetVector;

                m_mapControl.Map.Layers.Clear();
                m_mapControl.Map.Layers.Add(importResult, true);
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 导入为Img
		/// Import as Img
        /// </summary>
        public void ImportToImg()
        {
            try
            {
                m_dataImport.ImportSettings.Clear();

                ImportSettingIMG imgSetting = new ImportSettingIMG();
                imgSetting.ImportMode = ImportMode.Overwrite;
                imgSetting.SourceFilePath = @"..\..\SampleData\DataExchange\ImgImport\Multibands.img";
                imgSetting.TargetDatasource = m_desDatasource;
                imgSetting.MultiBandImportMode = MultiBandImportMode.MultiBand;

                m_dataImport.ImportSettings.Add(imgSetting);
                m_dataImport.Run();

                DatasetImage importResult = m_desDatasource.Datasets["Multibands"] as DatasetImage;
                LayerSettingImage layerSetting = new LayerSettingImage();
                layerSetting.DisplayBandIndexes = new Int32[] { 3, 2, 1 };
                layerSetting.DisplayColorSpace = ColorSpaceType.RGB;

                m_mapControl.Map.Layers.Clear();
                m_mapControl.Map.Layers.Add(importResult, layerSetting, true);
                m_mapControl.Map.IsDynamicProjection = false;
                m_mapControl.Map.ViewEntire();
                m_mapControl.Map.Refresh();
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
