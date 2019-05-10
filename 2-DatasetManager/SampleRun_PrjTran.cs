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

namespace _2_DatasetManager
{
    public class SampleRun_PrjTran
    {
        private Workspace m_workspace;
        private MapControl m_srcMapControl;
        //private MapControl m_targMapControl;
        private Dataset m_dataset;
        private Dataset m_processDataset;

        private String m_bufPrjDataName;

        private String m_openfa = "";
        private String m_currentprojectioninfo = "";
        private String m_type = "";
        private String m_name = "";
        private String m_longitude = "";
        private String m_latitude = "";
        private String m_firparallel = "";
        private String m_secparallel = "";
        private String m_offsetX = "";
        private String m_offsetY = "";
        private String m_scalefactor = "";
        private String m_azimuth = "";
        private String m_firlongitude = "";
        private String m_seclongitude = "";
        private String m_geocoordsys = "";
        private String m_geodatum = "";
        private String m_datumspheroid = "";
        private String m_majorradius = "";
        private String m_flattening = "";
        private String m_centralmeridian = "";
        /// <summary>
        /// 根据workspace和map构造 SampleRun对象
		/// Initialize the SampleRun object with the specified workspace and map
        /// </summary>
        public SampleRun_PrjTran(Workspace workspace, MapControl srcMapControl)
        {
            try
            {
                m_workspace = workspace;
                m_srcMapControl = srcMapControl;
                InitializeCultureResources();
                Initialize();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void InitializeCultureResources()
        {
            if (SuperMap.Data.Environment.CurrentCulture == "zh-CN")
            {
                m_openfa = "数据打开失败！";
                m_currentprojectioninfo = "当前投影信息：";
                m_type = "投影类型：";
                m_name = "投影方式：";
                m_longitude = "中央经线：";
                m_latitude = "原点纬线：";
                m_firparallel = "标准纬线(1)：";
                m_secparallel = "标准纬线(2)：";
                m_offsetX = "水平偏移量：";
                m_offsetY = "垂直偏移量：";
                m_scalefactor = "比例因子：";
                m_azimuth = "方位角：";
                m_firlongitude = "第一点经线：";
                m_seclongitude = "第二点经线：";
                m_geocoordsys = "地理坐标系：";
                m_geodatum = "大地参照系：";
                m_datumspheroid = "参考椭球体：";
                m_majorradius = "椭球长半轴：";
                m_flattening = "椭球扁率：";
                m_centralmeridian = "本初子午线";
            }
            else
            {
                m_openfa = "Failed to open the data!";
                m_currentprojectioninfo = "CurrentProjection:";
                m_type = "Projection Type:";
                m_name = "Name of the Projection object:";
                m_longitude = "Longitude of origin:";
                m_latitude = "Latitude of origin:";
                m_firparallel = "First standard parallel:";
                m_secparallel = "Second standard parallel:";
                m_offsetX = "Horizontal offset:";
                m_offsetY = "Vertical offset:";
                m_scalefactor = "Scale factor:";
                m_azimuth = "Azimuth:";
                m_firlongitude = "Longitude of the first point:";
                m_seclongitude = "Longitude of the second point:";
                m_geocoordsys = "Geographic coordinate system:";
                m_geodatum = "Name of the GeoDatum:";
                m_datumspheroid = "Spheroid object of the datum:";
                m_majorradius = "Major radius of spheroid:";
                m_flattening = "Flattening of spheroid:";
                m_centralmeridian = "Central meridian:";
            }
        }

        /// <summary>
        /// 打开需要的工作空间文件及地图
		/// Open the workspace and the map
        /// </summary>
        private void Initialize()
        {
            if (m_workspace != null)
            {
                try
                {
                    m_workspace.Open(new WorkspaceConnectionInfo(@"../../SampleData/China/China400.smwu"));

                    m_dataset = m_workspace.Datasources[0].Datasets["County_R"];

                    m_srcMapControl.Map.Layers.Add(m_dataset, true);
                    m_srcMapControl.Map.ViewEntire();

                    m_bufPrjDataName = "bufPrj";
                }
                catch (Exception ex)
                {
                    Trace.Write(ex.Message);
                }
            }
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
                m_processDataset = m_dataset.Datasource.CopyDataset(m_dataset, name, m_dataset.EncodeType);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 获取指定数据集的投影描述信息
        /// </summary>
        public String GetPrjStr(Dataset dataset)
        {
            StringBuilder prjStrBuilder = new StringBuilder();

            if (dataset == null)
            {
				MessageBox.Show(m_openfa);
            }

            PrjCoordSys crtPrjSys = dataset.PrjCoordSys;
			prjStrBuilder.AppendLine(m_currentprojectioninfo);
            prjStrBuilder.Append(this.GetEarthPrjStr(crtPrjSys));

            return prjStrBuilder.ToString();
        }


        /// <summary>
        /// 获取投影坐标系的描述信息
		/// Get the description information of the projected coordinate system
        /// </summary>
        private String GetEarthPrjStr(PrjCoordSys crtPrj)
        {
            StringBuilder prjStrBuilder = new StringBuilder();
            try
            {
                PrjParameter crtPrjParm = crtPrj.PrjParameter;
				prjStrBuilder.AppendLine(m_type + crtPrj.Name);
               	prjStrBuilder.AppendLine(m_name + crtPrj.Projection.Name);
               	prjStrBuilder.AppendLine(m_longitude + crtPrjParm.CentralMeridian.ToString("0.000000"));
               	prjStrBuilder.AppendLine(m_latitude + crtPrjParm.CentralParallel.ToString());
               	prjStrBuilder.AppendLine(m_firparallel + crtPrjParm.StandardParallel1.ToString());
               	prjStrBuilder.AppendLine(m_secparallel + crtPrjParm.StandardParallel2.ToString());
               	prjStrBuilder.AppendLine(m_offsetX + crtPrjParm.FalseEasting.ToString(".0000"));
               	prjStrBuilder.AppendLine(m_offsetY + crtPrjParm.FalseNorthing.ToString(".0000"));
               	prjStrBuilder.AppendLine(m_scalefactor + crtPrjParm.ScaleFactor.ToString());
               	prjStrBuilder.AppendLine(m_azimuth+ crtPrjParm.Azimuth.ToString(".0000"));
               	prjStrBuilder.AppendLine(m_firlongitude + crtPrjParm.FirstPointLongitude.ToString());
               	prjStrBuilder.AppendLine(m_seclongitude + crtPrjParm.SecondPointLongitude.ToString());
               	prjStrBuilder.AppendLine(m_geocoordsys + crtPrj.GeoCoordSys.Name);
               	prjStrBuilder.AppendLine(m_geodatum + crtPrj.GeoCoordSys.GeoDatum.Name);
               	prjStrBuilder.AppendLine(m_datumspheroid + crtPrj.GeoCoordSys.GeoDatum.GeoSpheroid.Name);
               	prjStrBuilder.AppendLine(m_majorradius + crtPrj.GeoCoordSys.GeoDatum.GeoSpheroid.Axis.ToString(".00"));
               	prjStrBuilder.AppendLine(m_flattening + crtPrj.GeoCoordSys.GeoDatum.GeoSpheroid.Flatten.ToString());
                prjStrBuilder.AppendLine(m_centralmeridian + crtPrj.GeoCoordSys.GeoPrimeMeridian.LongitudeValue.ToString("0.000000"));
            	
			}
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return prjStrBuilder.ToString();
        }

        /// <summary>
        /// 转换主函数
		/// Main function of transformation
        /// </summary>
        /// <param name="type">对应不同的投影类型 Projection type</param>
        /// <returns>转换后投影的描述信息 The description information of projection after transformed</returns>
        public String TransformPrj(int type)
        {
            try
            {
                m_srcMapControl.Map.Layers.Clear();
                this.CopyDataset(m_bufPrjDataName);

                PrjCoordSys gaussPrjSys = this.GetTargetPrjCoordSys(type);
                Boolean result = CoordSysTranslator.Convert(m_processDataset, gaussPrjSys, new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);

                m_srcMapControl.Map.Layers.Add(m_processDataset, true);
                m_srcMapControl.Map.Center = m_srcMapControl.Map.Bounds.Center;
                m_srcMapControl.Map.Scale = m_srcMapControl.Map.Scale;
                m_srcMapControl.Map.Refresh();

                return this.GetPrjStr(m_processDataset);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        // 按照不同的投影类型，初始化投影坐标系
		// Initialize the projection coordinates by the different projection type.
        private PrjCoordSys GetTargetPrjCoordSys(int type)
        {
            PrjCoordSys targetPrjCoordSys = null;
            PrjParameter parameter = null;
            Projection projection = null;

            switch (type)
            {
                case 1:
                    {
                        targetPrjCoordSys = new PrjCoordSys(PrjCoordSysType.UserDefined);
                        projection = new Projection(ProjectionType.GaussKruger);
                        targetPrjCoordSys.Projection = projection;
                        parameter = new PrjParameter();
                        parameter.CentralMeridian = 110;
                        parameter.StandardParallel1 = 20;
                        parameter.StandardParallel2 = 40;
                        targetPrjCoordSys.PrjParameter = parameter;
                    }
                    break;
                case 2:
                    {
                        targetPrjCoordSys = new PrjCoordSys(PrjCoordSysType.UserDefined);
                        projection = new Projection(ProjectionType.TransverseMercator);
                        targetPrjCoordSys.Projection = projection;
                        parameter = new PrjParameter();
                        parameter.CentralMeridian = 110;
                        parameter.StandardParallel1 = 0;
                        targetPrjCoordSys.PrjParameter = parameter;
                    }
                    break;
                case 3:
                    {
                        targetPrjCoordSys = new PrjCoordSys(PrjCoordSysType.UserDefined);
                        projection = new Projection(ProjectionType.LambertConformalConic);
                        targetPrjCoordSys.Projection = projection;
                        parameter = new PrjParameter();
                        parameter.CentralMeridian = 110;
                        parameter.StandardParallel1 = 30;
                        targetPrjCoordSys.PrjParameter = parameter;
                    }
                    break;
                default:
                    break;
            }
            return targetPrjCoordSys;
        }


    }
}


