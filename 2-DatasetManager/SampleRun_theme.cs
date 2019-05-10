///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------版权声明----------------------------
//
// 此文件为 SuperMap iObjects .NET 的示范代码
// 版权所有：北京超图软件股份有限公司
//------------------------------------------------------------------
//
//-----------------------SuperMap iObjects .NET 示范程序说明--------------------------
//
// 1、范例简介：示范如何使用ThemeDotDensity
// 2、示例数据：安装目录\SampleData\ThematicMaps\ThematicMaps.smwu； 
// 3、关键类型/成员:
//        Workspace.Open 方法
//        Map.Open 方法
//        Map.Layers.Add 方法
//        Map.Layers.Remove 方法
//        ThemeDotDensity.DotExpression 属性
//        ThemeDotDensity.Value 属性
//        ThemeDotDensity.Style 属性
// 4、使用步骤：
//  (1)通过CheckBox 来控制专题图层是否显示。
//  (2)浏览地图上显示的点密度专题图。
//---------------------------------------------------------------------------------------
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
//------------------------------Copyright Statement----------------------------
//
// This is the Sample Code of SuperMap iObjects .NET 
// SuperMap Software Co., Ltd. All Rights Reserved. 
//------------------------------------------------------------------
//
//-----------------------The description of SuperMap iObjects .NET Sample Code --------------------------
//
//1.Sample Code Description:This sample demonstrates how to make dot density map. 
//2.Sample Data:Installation directory\ThematicMaps\ThematicMaps.smwu 
//3.Key Classes/Methods:
//        Workspace.Open method
//        Map.Open method
//        Map.Layers.Add method
//        Map.Layers.Remove method
//        ThemeDotDensity.DotExpression property
//        ThemeDotDensity.Value property
//        ThemeDotDensity.Style property
//4.Procedures:
//  (1)Whether to display the theme layer or not can be controlled by the CheckBox. 
//  (2)Browse the dot density map. 
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
    public class SampleRun_theme
    {
        private Workspace m_workspace;
        private MapControl m_mapControl;
        private DatasetVector m_dataset;
        private Datasource m_datasource;
        private Layer m_layer;
        private Layer m_layerThemeGraphBar3D;

        private DatasetVector m_pointDataset;
        // 添加的图层的名称
        // The name of the adding layer
        private String m_themeLayerName;
        // 构建子项时属性的数组
        // Initialize the array of property
        private String[] m_itemNames;
        private Color[] m_itemForeColors;
        private Color[] m_itemBackColors;

        private List<LabelItem> m_items;

        public List<LabelItem> Items
        {
            get { return m_items; }
        }


        /// <summary>
        /// 根据workspace和mapControl构造 SampleRun对象
        /// Initialize the object of SampleRun based on the workspace and the map
        /// </summary>
        public SampleRun_theme(Workspace workspace, MapControl mapControl)
        {
            m_workspace = workspace;

            m_mapControl = mapControl;
            m_mapControl.Map.Workspace = m_workspace;

            Initialize();
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
                WorkspaceConnectionInfo conInfo = new WorkspaceConnectionInfo(@"..\..\SampleData\ThematicMaps\ThematicMaps.smwu");

                m_workspace.Open(conInfo);
                m_datasource = m_workspace.Datasources[0];

                m_dataset = m_workspace.Datasources["Thematicmaps"].Datasets["BaseMap_R"] as DatasetVector;
                if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                {
                    m_mapControl.Map.Open("Beijing-Tianjin Area Division Map");
                }
                else
                {
                    m_mapControl.Map.Open("京津地区地图");
                }
                m_mapControl.Map.Refresh();

                // 调整mapControl的状态
                // Adjust the condition of mapControl
                m_mapControl.Action = SuperMap.UI.Action.Pan;
                m_mapControl.IsWaitCursorEnabled = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 设置ThemeDotDensity的属性，添加点密度专题图图层到地图
        /// Set the ThemeDotDensity property and add the dot density theme layer into the map
        /// </summary>
        public void AddThemeDotDensityLayer()
        {
            try
            {
                ThemeDotDensity dotDensity = new ThemeDotDensity();
                dotDensity.DotExpression = "Pop_Density99";
                dotDensity.Value = 0.00030;

                GeoStyle geostyle = new GeoStyle();
                geostyle.LineColor = Color.Red;
                geostyle.MarkerSize = new Size2D(0.8, 0.8);

                dotDensity.Style = geostyle;
                // 将制作好的专题图添加到地图中显示
                // Display the theme in the map
                Layer themeLayer = m_mapControl.Map.Layers.Add(m_dataset, dotDensity, true);
                m_mapControl.Map.Layers.MoveDown(0);
                m_themeLayerName = themeLayer.Name;

                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 添加等级符号专题图
        /// Add the theme of graduated symbol
        /// </summary>
        public void ThemeRangeThemeDisplay()
        {
            try
            {
                Map map = m_mapControl.Map;
                Layers layers = map.Layers;
                Int32 count = layers.Count;

                Datasources datasources = m_workspace.Datasources;
                Datasource datasource = datasources[0];
                Datasets datasets = datasource.Datasets;
                DatasetVector datasetVector = datasets["BaseMap_R"] as DatasetVector;

                ThemeGraduatedSymbol themeGraduatedSymbol = new ThemeGraduatedSymbol();
                themeGraduatedSymbol.Expression = "Urban";
                themeGraduatedSymbol.BaseValue = 150.00000;
                themeGraduatedSymbol.GraduatedMode = GraduatedMode.SquareRoot;
                themeGraduatedSymbol.IsFlowEnabled = false;

                GeoStyle geoStyle = themeGraduatedSymbol.PositiveStyle;
                geoStyle.LineColor = Color.Pink;

                m_layer = layers.Add(datasetVector, themeGraduatedSymbol, true);
                m_themeLayerName = m_layer.Name;

                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 设置是否显示三维柱状统计图
        /// Whether to display the theme of Bar3D graph or not
        /// </summary>
        /// <param name="value"></param>
        public void addThemeGraphBar3DVisible()
        {
            // 将等待光标设置为无效
            // IsWaitCursorEnabled property is set to be false
            m_mapControl.IsWaitCursorEnabled = false;
            // 设置反走样
            // IsAntialias property is set to be true
            m_mapControl.Map.IsAntialias = true;
            // 设置压盖时仍然显示
            // IsOverlapDisplayed property is set to be true
            m_mapControl.Map.IsOverlapDisplayed = true;
            // 打开地图即可同时显示三维柱状图,饼图,三维玫瑰图
            // Display the theme graphs in the map

            Boolean value = true;
            try
            {
                if (m_layerThemeGraphBar3D == null)
                {
                    // 构造统计专题图
                    // Create the theme layer of Pie graph
                    ThemeGraph graphBar3D = new ThemeGraph();

                    // 初始化子项，及子项的风格
                    // Initialize the theme items and set the style
                    GeoStyle geoStyle = new GeoStyle();
                    geoStyle.LineWidth = 0.1;
                    geoStyle.FillGradientMode = FillGradientMode.Linear;

                    ThemeGraphItem item0 = new ThemeGraphItem();
                    item0.GraphExpression = "Pop_Rate95";
                    geoStyle.FillForeColor = Color.FromArgb(231, 154, 0);
                    item0.UniformStyle = geoStyle;

                    ThemeGraphItem item1 = new ThemeGraphItem();
                    item1.GraphExpression = "Pop_Rate99";
                    geoStyle.FillForeColor = Color.FromArgb(70, 153, 255);
                    item1.UniformStyle = geoStyle;



                    // 添加子项
                    // Add the items into the theme
                    graphBar3D.Add(item0);
                    graphBar3D.Add(item1);


                    // 设置偏移量，和非固定偏移，即随图偏移
                    // Set the offset value 
                    graphBar3D.IsOffsetFixed = false;
                    graphBar3D.OffsetY = "13000";

                    // 设置非流动显示
                    //IsFlowEnabled property is set to be false
                    graphBar3D.IsFlowEnabled = false;

                    // 设置非避让方式显示
                    // IsOverlapAvoided property is set to be true
                    graphBar3D.IsOverlapAvoided = true;

                    // 设置统计图显示的最大值和最小值,和非固定大小，即随图缩放
                    // Set the MaxGraphSize and MinGraphSize property
                    graphBar3D.MaxGraphSize = 100000.0000;
                    graphBar3D.MinGraphSize = 50000.0000;

                    // 显示统计图文本，并设置文本显示模式
                    // Display the text of theme graph 
                    graphBar3D.IsGraphTextDisplayed = true;
                    graphBar3D.GraphTextFormat = ThemeGraphTextFormat.Value;

                    // 设置统计图类型
                    // Set the type of theme graph
                    graphBar3D.GraphType = ThemeGraphType.Bar3D;

                    TextStyle textStyle = graphBar3D.GraphTextStyle;
                    textStyle.IsSizeFixed = false;
                    textStyle.FontHeight = 10000;

                    // 添加三维柱状统计图图层
                    // Add the theme layer of Pie graph into the map
                    m_layerThemeGraphBar3D = m_mapControl.Map.Layers.Add(m_dataset, graphBar3D, true);
                }

                // 设置图层是否可显示，并刷新地图
                // Whether to display the theme layer or not .Refresh the map
                m_layerThemeGraphBar3D.IsVisible = value;
                this.m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 添加分段标签专题图层
        /// Add the theme of label range
        /// </summary>
        public void AddThemeLabelLayer()
        {
            m_pointDataset = m_workspace.Datasources[0].Datasets["BaseMap_P"] as DatasetVector;

            // 初始化构建子项对象属性的数组
            // Initialize the array of property
            if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
            {
                m_itemNames = new String[4] { "First class city", "Second class city", "Third class city", "Fourth class city" };
            }
            else
            {
                m_itemNames = new String[4] { "第一等级城市", "第二等级城市", "第三等级城市", "第四等级城市" };
            }
            m_itemForeColors = new Color[4] { Color.Red, Color.Green, Color.Black, Color.Blue };
            m_itemBackColors = new Color[4] { Color.FromArgb(255, 235, 189), Color.YellowGreen, Color.White, Color.LightGray };

            try
            {
                // 创建分段标签专题图层，设置标签表达式和分段表达式
                //Create the theme of label range, set the label expression and range expression
                ThemeLabel themeLabel = new ThemeLabel();

                if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                {
                    themeLabel.LabelExpression = "name_en";
                }
                else
                {
                    themeLabel.LabelExpression = "name";
                }
                themeLabel.RangeExpression = "ADCLASS";

                for (Int32 i = 0; i < m_itemNames.Length; i++)
                {
                    // 通过内部方法构建子项，并添加到标签专题图
                    // Create items and add them into the theme
                    themeLabel.AddToTail(AddItemToThemeLabel(i));
                }
                // 添加专题图层到地图
                // Add the theme layer into the map
                Layer layer = m_mapControl.Map.Layers.Add(m_pointDataset, themeLabel, true);
                m_themeLayerName = layer.Name;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 构建分段标签专题图子项
        /// Initialize items of the theme
        /// </summary>
        /// <param name="index">子项的序号</param>
        /// <param name="index">Index of the item</param>
        /// <returns>构建出的子项对象</returns>
        /// <returns>The objects of items</returns>
        private ThemeLabelItem AddItemToThemeLabel(Int32 index)
        {
            ThemeLabelItem item = new ThemeLabelItem();
            item.Caption = m_itemNames[index];
            item.Start = index + 1;
            item.End = index + 2;
            item.IsVisible = true;

            TextStyle textStyle = new TextStyle();
            textStyle.FontName = "微软雅黑";
            textStyle.ForeColor = m_itemForeColors[index];
            textStyle.BackColor = m_itemBackColors[index];
            textStyle.FontHeight = 9 - 0.6 * (1 + index);
            textStyle.Outline = true;
            textStyle.IsSizeFixed = true;

            item.Style = textStyle;

            return item;
        }

        /// <summary>
        /// 构造分段专题图并添加分段专题图图层
        /// Initialize the theme and add the theme layer to the map
        /// </summary>
        public void AddThemeRangeLayer()
        {
            try
            {
                // 构造分段专题图对象并设置分段字段表达式
                // Initialize the theme of range and set the range expression
                ThemeRange themeRange = new ThemeRange();
                themeRange.RangeExpression = "UrbanRural";

                GeoStyle style = new GeoStyle();
                style.LineColor = Color.White;
                style.LineWidth = 0.3;
                // 初始化分段专题图子项并设置各自的风格
                // Initialize the theme items and set them styles
                ThemeRangeItem item0 = new ThemeRangeItem();
                item0.Start = double.MinValue;
                item0.End = 50;
                style.FillForeColor = Color.FromArgb(209, 182, 210);
                item0.Style = style;

                ThemeRangeItem item1 = new ThemeRangeItem();
                item1.Start = 50;
                item1.End = 60;
                style.FillForeColor = Color.FromArgb(205, 167, 183);
                item1.Style = style;

                ThemeRangeItem item2 = new ThemeRangeItem();
                item2.Start = 60;
                item2.End = 70;
                style.FillForeColor = Color.FromArgb(183, 128, 151);
                item2.Style = style;

                ThemeRangeItem item3 = new ThemeRangeItem();
                item3.Start = 70;
                item3.End = 90;
                style.FillForeColor = Color.FromArgb(164, 97, 136);
                item3.Style = style;
                // 将分段专题图子项依次添加到分段专题图
                // Add the items to the theme of range
                themeRange.AddToHead(item0);
                themeRange.AddToTail(item1);
                themeRange.AddToTail(item2);
                themeRange.AddToTail(item3);
                // 添加分段专题图层
                // Add the theme layer to the map
                Layers layers = m_mapControl.Map.Layers;
                Int32 index = layers.IndexOf(m_dataset.Name + "@" + m_datasource.Alias);
                m_layer = layers.Insert(index, m_dataset, themeRange);
                m_themeLayerName = m_layer.Name;

                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 显示单值专题图
        /// Display the theme of unique
        /// </summary>
        public void themeuniqueDisplay(Boolean isDisplayThemeLayer)
        {
            m_items = new List<LabelItem>();
            try
            {
                // 先移除图层
                // Remove the theme layer
                m_mapControl.Map.Layers.Remove(m_themeLayerName);

                if (isDisplayThemeLayer)
                {
                    // 当只有普通图层时，添加专题图层
                    // Add the theme layer when there is only normal layer
                    DatasetVector datasetVector = m_workspace.Datasources[0].Datasets["Landuse_R"] as DatasetVector;
                    // 设置单值专题图
                    // Set the theme of unique
                    ThemeUnique themeunique = new ThemeUnique();

                    if (SuperMap.Data.Environment.CurrentCulture == "zh-CN")
                    {
                        themeunique.UniqueExpression = "LandType";
                    }
                    else
                    {
                        themeunique.UniqueExpression = "LandType_en";
                    }

                    // 实例化若干单值专题图子项,分别设置每个子项的属性
                    // Make many examples of the theme item and set them properties

                    Color itemColor = Color.FromArgb(206, 101, 156);
                    String itemName = "";
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "City";
                    }
                    else
                    {
                        itemName = "城市";
                    }
                    GeoStyle geostyle1 = new GeoStyle();
                    geostyle1.FillForeColor = itemColor;
                    geostyle1.FillOpaqueRate = 80;
                    geostyle1.LineSymbolID = 5;

                    ThemeUniqueItem item1 = new ThemeUniqueItem();
                    item1.Caption = itemName;
                    item1.IsVisible = true;
                    item1.Style = geostyle1;
                    item1.Unique = itemName;

                    LabelItem item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(181, 178, 181);
                    GeoStyle geostyle2 = new GeoStyle();
                    geostyle2.LineSymbolID = 5;
                    geostyle2.FillForeColor = itemColor;
                    geostyle2.FillOpaqueRate = 80;
                    ThemeUniqueItem item2 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Non-irrigated Field";
                    }
                    else
                    {
                        itemName = "旱地";
                    }

                    item2.Caption = itemName;
                    item2.IsVisible = true;
                    item2.Style = geostyle2;
                    item2.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(255, 255, 115);
                    GeoStyle geostyle3 = new GeoStyle();
                    geostyle3.LineSymbolID = 5;
                    geostyle3.FillForeColor = itemColor;
                    geostyle3.FillOpaqueRate = 45;
                    ThemeUniqueItem item3 = new ThemeUniqueItem();

                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Irrigated Field";
                    }
                    else
                    {
                        itemName = "水浇地";
                    }

                    item3.Caption = itemName;
                    item3.IsVisible = true;
                    item3.Style = geostyle3;
                    item3.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(254, 175, 136);
                    GeoStyle geostyle4 = new GeoStyle();
                    geostyle4.LineSymbolID = 5;
                    geostyle4.FillForeColor = itemColor;
                    geostyle4.FillOpaqueRate = 100;
                    ThemeUniqueItem item4 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Paddy Field";
                    }
                    else
                    {
                        itemName = "水田";
                    }

                    item4.Caption = itemName;
                    item4.IsVisible = true;
                    item4.Style = geostyle4;
                    item4.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(115, 77, 0);
                    GeoStyle geostyle5 = new GeoStyle();
                    geostyle5.LineSymbolID = 5;
                    geostyle5.FillForeColor = itemColor;
                    geostyle5.FillOpaqueRate = 100;
                    ThemeUniqueItem item5 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Desert";
                    }
                    else
                    {
                        itemName = "沙漠";
                    }

                    item5.Caption = itemName;
                    item5.IsVisible = true;
                    item5.Style = geostyle5;
                    item5.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(173, 170, 0);
                    GeoStyle geostyle6 = new GeoStyle();
                    geostyle6.LineSymbolID = 5;
                    geostyle6.FillForeColor = Color.FromArgb(173, 170, 0);
                    geostyle6.FillOpaqueRate = 100;
                    ThemeUniqueItem item6 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Marshes";
                    }
                    else
                    {
                        itemName = "沼泽";
                    }

                    item6.Caption = itemName;
                    item6.IsVisible = true;
                    item6.Style = geostyle6;
                    item6.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(151, 219, 242);
                    GeoStyle geostyle7 = new GeoStyle();
                    geostyle7.LineSymbolID = 5;
                    geostyle7.FillForeColor = itemColor;
                    geostyle7.FillOpaqueRate = 100;
                    ThemeUniqueItem item7 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Lake and Reservoir";
                    }
                    else
                    {
                        itemName = "湖泊水库";
                    }

                    item7.Caption = itemName;
                    item7.IsVisible = true;
                    item7.Style = geostyle7;
                    item7.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(90, 138, 66);
                    GeoStyle geostyle8 = new GeoStyle();
                    geostyle8.LineSymbolID = 5;
                    geostyle8.FillForeColor = itemColor;
                    geostyle8.FillOpaqueRate = 50;
                    ThemeUniqueItem item8 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Bush";
                    }
                    else
                    {
                        itemName = "灌丛";
                    }

                    item8.Caption = itemName;
                    item8.IsVisible = true;
                    item8.Style = geostyle8;
                    item8.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(0, 113, 74);
                    GeoStyle geostyle9 = new GeoStyle();
                    //geostyle9.LineWidth = 0.01;
                    geostyle9.LineSymbolID = 5;
                    geostyle9.FillForeColor = itemColor;
                    geostyle9.FillOpaqueRate = 60;
                    ThemeUniqueItem item9 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Timber Forest";
                    }
                    else
                    {
                        itemName = "用材林";
                    }

                    item9.Caption = itemName;
                    item9.IsVisible = true;
                    item9.Style = geostyle9;
                    item9.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(0, 170, 132);
                    GeoStyle geostyle10 = new GeoStyle();
                    geostyle10.LineSymbolID = 5;
                    geostyle10.FillForeColor = itemColor;
                    geostyle10.FillOpaqueRate = 80;
                    ThemeUniqueItem item10 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Economic Forest";
                    }
                    else
                    {
                        itemName = "经济林";
                    }

                    item10.Caption = itemName;
                    item10.IsVisible = true;
                    item10.Style = geostyle10;
                    item10.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    itemColor = Color.FromArgb(90, 179, 40);
                    GeoStyle geostyle11 = new GeoStyle();
                    geostyle11.LineSymbolID = 5;
                    geostyle11.FillForeColor = itemColor;
                    geostyle11.FillOpaqueRate = 30;
                    ThemeUniqueItem item11 = new ThemeUniqueItem();
                    if (SuperMap.Data.Environment.CurrentCulture != "zh-CN")
                    {
                        itemName = "Grassland";
                    }
                    else
                    {
                        itemName = "草地";
                    }

                    item11.Caption = itemName;
                    item11.IsVisible = true;
                    item11.Style = geostyle11;
                    item11.Unique = itemName;

                    item = new LabelItem(itemColor, itemName);
                    m_items.Add(item);

                    // 向themeunique对象中逐个添加单值专题图子项
                    // Add all of the items to the object of themeunique
                    themeunique.Add(item1);
                    themeunique.Add(item2);
                    themeunique.Add(item3);
                    themeunique.Add(item4);
                    themeunique.Add(item5);
                    themeunique.Add(item6);
                    themeunique.Add(item7);
                    themeunique.Add(item8);
                    themeunique.Add(item9);
                    themeunique.Add(item10);
                    themeunique.Add(item11);

                    // 将制作好的专题图添加到地图中显示
                    // Display the theme in the map
                    Layer layer = this.m_mapControl.Map.Layers.Add(datasetVector, themeunique, true);
                    m_themeLayerName = layer.Name;
                }

                this.m_mapControl.Map.Refresh();
            }

            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }


        public class LabelItem
        {
            Color m_color;
            String m_caption;

            public LabelItem(Color color, String caption)
            {
                m_color = color;
                m_caption = caption;
            }

            public Color Color
            {
                get { return m_color; }
            }

            public String Caption
            {
                get { return m_caption; }
            }
        }

        /// <summary>
        /// 设置专题图的属性，添加专题图图层到地图
        /// Set the property of the theme and add the theme to the map
        /// </summary>
        public void AddUniformStyleThemeLabelLayer()
        {
            try
            {
                ThemeLabel themeLabel = new ThemeLabel();

                TextStyle textStyle = new TextStyle();
                textStyle.ForeColor = Color.FromArgb(115, 0, 74);
                textStyle.BackColor = Color.FromArgb(231, 227, 231);
                textStyle.Bold = true;
                textStyle.Outline = true;
                textStyle.FontHeight = 8;
                if (SuperMap.Data.Environment.CurrentCulture == "zh-CN")
                {
                    themeLabel.LabelExpression = "Name";
                }
                else
                {
                    themeLabel.LabelExpression = "Name_en";
                }

                // 设置统一风格
                // Set the UniformStyle value
                themeLabel.UniformStyle = textStyle;
                // 将制作好的专题图添加到地图中显示
                // Display the theme layer in the map
                Layer themeLayer = m_mapControl.Map.Layers.Add(m_dataset, themeLabel, true);
                m_themeLayerName = themeLayer.Name;

                m_mapControl.Map.Refresh();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

    }
}


