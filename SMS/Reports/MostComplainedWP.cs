﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using SMS.Models;
using System.Collections.Generic;

/// <summary>
/// Summary description for IndPerformance
/// </summary>
/// 
namespace SMS.Reports
{
    public class MostComplainedWP : DevExpress.XtraReports.UI.XtraReport
    {
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private PageHeaderBand PageHeader;
        private XRPictureBox xrPictureBox1;
        private XRLabel xrLabelName;
        private XRTable xrTable4;
        private XRTableRow xrTableRow13;
        private XRTableCell xrTableCellStart;
        private XRTableCell xrTableCell42;
        private XRTableCell xrTableCellEnd;
        private CalculatedField calTotalHour;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objChart;
        private XRChart xrChart2;
        private CalculatedField calTotalCompleteJobs;
        private CalculatedField calPendingJobs;
        private DetailReportBand DetailReport;
        private DetailBand Detail1;
        private PageFooterBand PageFooter;
        private XRPageInfo xrPageInfo1;
        private XRLabel xrLabelEmployee;
        private CalculatedField calProduct;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MostComplainedWP(IEnumerable<BarChartModel> ListChart, DateTime Start, DateTime End, string Name)
        {
            InitializeComponent();
            //
            // TODO: Add constructor logic here
            //
            
            xrTableCellStart.Text = Start.ToString("dd MMM, yy");
            xrTableCellEnd.Text = End.ToString("dd MMM, yy");
            xrLabelName.Text = "Most Complained Warranty Product";
            xrLabelEmployee.Text = Name;
            objChart.DataSource = ListChart;
            

        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraCharts.SideBySideBarSeriesView sideBySideBarSeriesView1 = new DevExpress.XtraCharts.SideBySideBarSeriesView();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel2 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraCharts.ChartTitle chartTitle1 = new DevExpress.XtraCharts.ChartTitle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MostComplainedWP));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrChart2 = new DevExpress.XtraReports.UI.XRChart();
            this.objChart = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrTable4 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow13 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellStart = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell42 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellEnd = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabelName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.calTotalHour = new DevExpress.XtraReports.UI.CalculatedField();
            this.calTotalCompleteJobs = new DevExpress.XtraReports.UI.CalculatedField();
            this.calPendingJobs = new DevExpress.XtraReports.UI.CalculatedField();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabelEmployee = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.calProduct = new DevExpress.XtraReports.UI.CalculatedField();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 16.66667F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.Visible = false;
            // 
            // xrChart2
            // 
            this.xrChart2.AutoLayout = true;
            this.xrChart2.BorderColor = System.Drawing.Color.Black;
            this.xrChart2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrChart2.DataSource = this.objChart;
            xyDiagram1.AxisX.Label.Font = new System.Drawing.Font("Tahoma", 10F);
            xyDiagram1.AxisX.Label.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisY.Label.Font = new System.Drawing.Font("Tahoma", 10F);
            xyDiagram1.AxisY.Label.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            xyDiagram1.AxisY.Label.TextPattern = "{V:#,#}";
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            xyDiagram1.DefaultPane.EnableAxisXScrolling = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisXZooming = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisYScrolling = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisYZooming = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.Rotated = true;
            this.xrChart2.Diagram = xyDiagram1;
            this.xrChart2.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center;
            this.xrChart2.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.TopOutside;
            this.xrChart2.Legend.Direction = DevExpress.XtraCharts.LegendDirection.RightToLeft;
            this.xrChart2.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            this.xrChart2.LocationFloat = new DevExpress.Utils.PointFloat(0.0002622604F, 0F);
            this.xrChart2.Name = "xrChart2";
            this.xrChart2.PaletteName = "Aspect";
            series1.ArgumentDataMember = "Series2";
            sideBySideBarSeriesLabel1.Font = new System.Drawing.Font("Tahoma", 10F);
            sideBySideBarSeriesLabel1.Position = DevExpress.XtraCharts.BarSeriesLabelPosition.Top;
            sideBySideBarSeriesLabel1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            sideBySideBarSeriesLabel1.TextPattern = "{V:#,#}";
            series1.Label = sideBySideBarSeriesLabel1;
            series1.Name = "Product";
            series1.ShowInLegend = false;
            series1.ValueDataMembersSerializable = "Value1";
            sideBySideBarSeriesView1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(150)))), ((int)(((byte)(70)))));
            series1.View = sideBySideBarSeriesView1;
            this.xrChart2.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            sideBySideBarSeriesLabel2.Font = new System.Drawing.Font("Tahoma", 10F);
            sideBySideBarSeriesLabel2.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.xrChart2.SeriesTemplate.Label = sideBySideBarSeriesLabel2;
            this.xrChart2.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            this.xrChart2.SeriesTemplate.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.True;
            this.xrChart2.SizeF = new System.Drawing.SizeF(1066F, 590.625F);
            chartTitle1.Dock = DevExpress.XtraCharts.ChartTitleDockStyle.Bottom;
            chartTitle1.Text = "Number of Complain";
            this.xrChart2.Titles.AddRange(new DevExpress.XtraCharts.ChartTitle[] {
            chartTitle1});
            // 
            // objChart
            // 
            this.objChart.DataSource = typeof(SMS.Models.BarChartModel);
            this.objChart.Name = "objChart";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 44F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 43F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable4,
            this.xrLabelName,
            this.xrPictureBox1});
            this.PageHeader.HeightF = 70.83334F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrTable4
            // 
            this.xrTable4.BorderColor = System.Drawing.Color.Silver;
            this.xrTable4.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable4.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTable4.LocationFloat = new DevExpress.Utils.PointFloat(829.2917F, 38.95834F);
            this.xrTable4.Name = "xrTable4";
            this.xrTable4.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100F);
            this.xrTable4.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow13});
            this.xrTable4.SizeF = new System.Drawing.SizeF(226.7082F, 21.875F);
            this.xrTable4.StylePriority.UseBorderColor = false;
            this.xrTable4.StylePriority.UseBorders = false;
            this.xrTable4.StylePriority.UseFont = false;
            this.xrTable4.StylePriority.UsePadding = false;
            // 
            // xrTableRow13
            // 
            this.xrTableRow13.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellStart,
            this.xrTableCell42,
            this.xrTableCellEnd});
            this.xrTableRow13.Name = "xrTableRow13";
            this.xrTableRow13.Weight = 0.875D;
            // 
            // xrTableCellStart
            // 
            this.xrTableCellStart.Name = "xrTableCellStart";
            this.xrTableCellStart.StylePriority.UseTextAlignment = false;
            this.xrTableCellStart.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellStart.Weight = 1.0314934255117421D;
            // 
            // xrTableCell42
            // 
            this.xrTableCell42.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell42.Name = "xrTableCell42";
            this.xrTableCell42.StylePriority.UseBorders = false;
            this.xrTableCell42.StylePriority.UseTextAlignment = false;
            this.xrTableCell42.Text = "To";
            this.xrTableCell42.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell42.Weight = 0.32219081374905129D;
            // 
            // xrTableCellEnd
            // 
            this.xrTableCellEnd.Multiline = true;
            this.xrTableCellEnd.Name = "xrTableCellEnd";
            this.xrTableCellEnd.StylePriority.UseTextAlignment = false;
            this.xrTableCellEnd.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellEnd.Weight = 0.97914461398468255D;
            // 
            // xrLabelName
            // 
            this.xrLabelName.BackColor = System.Drawing.Color.LightSteelBlue;
            this.xrLabelName.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabelName.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabelName.ForeColor = System.Drawing.Color.Black;
            this.xrLabelName.LocationFloat = new DevExpress.Utils.PointFloat(698.0419F, 0F);
            this.xrLabelName.Multiline = true;
            this.xrLabelName.Name = "xrLabelName";
            this.xrLabelName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 5, 0, 0, 100F);
            this.xrLabelName.SizeF = new System.Drawing.SizeF(357.9581F, 38.95834F);
            this.xrLabelName.StylePriority.UseBackColor = false;
            this.xrLabelName.StylePriority.UseBorders = false;
            this.xrLabelName.StylePriority.UseFont = false;
            this.xrLabelName.StylePriority.UseForeColor = false;
            this.xrLabelName.StylePriority.UsePadding = false;
            this.xrLabelName.StylePriority.UseTextAlignment = false;
            this.xrLabelName.Text = "Report Heading";
            this.xrLabelName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrPictureBox1
            // 
            this.xrPictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("xrPictureBox1.Image")));
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(239.5833F, 32.29167F);
            this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.AutoSize;
            // 
            // calTotalHour
            // 
            this.calTotalHour.Expression = "[HH] + [OTH] + [TH] + [WH]";
            this.calTotalHour.Name = "calTotalHour";
            // 
            // calTotalCompleteJobs
            // 
            this.calTotalCompleteJobs.Expression = "Sum([CompleteJobs])";
            this.calTotalCompleteJobs.Name = "calTotalCompleteJobs";
            // 
            // calPendingJobs
            // 
            this.calPendingJobs.Expression = "Sum([PendingJobs])";
            this.calPendingJobs.Name = "calPendingJobs";
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1});
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrChart2});
            this.Detail1.HeightF = 591.6667F;
            this.Detail1.Name = "Detail1";
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabelEmployee,
            this.xrPageInfo1});
            this.PageFooter.HeightF = 38.54167F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLabelEmployee
            // 
            this.xrLabelEmployee.BackColor = System.Drawing.Color.Transparent;
            this.xrLabelEmployee.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabelEmployee.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabelEmployee.ForeColor = System.Drawing.Color.Black;
            this.xrLabelEmployee.LocationFloat = new DevExpress.Utils.PointFloat(832.0002F, 9.583282F);
            this.xrLabelEmployee.Multiline = true;
            this.xrLabelEmployee.Name = "xrLabelEmployee";
            this.xrLabelEmployee.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 5, 0, 0, 100F);
            this.xrLabelEmployee.SizeF = new System.Drawing.SizeF(233.9998F, 28.95834F);
            this.xrLabelEmployee.StylePriority.UseBackColor = false;
            this.xrLabelEmployee.StylePriority.UseBorders = false;
            this.xrLabelEmployee.StylePriority.UseFont = false;
            this.xrLabelEmployee.StylePriority.UseForeColor = false;
            this.xrLabelEmployee.StylePriority.UsePadding = false;
            this.xrLabelEmployee.StylePriority.UseTextAlignment = false;
            this.xrLabelEmployee.Text = "Employee Name";
            this.xrLabelEmployee.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPageInfo1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPageInfo1.Format = "{0:dd-MMM-yy hh:mm tt}";
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 15.54165F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(215.125F, 23F);
            this.xrPageInfo1.StylePriority.UseBorders = false;
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // calProduct
            // 
            this.calProduct.DataSource = this.objChart;
            this.calProduct.Expression = "Concat([Series3],\' - \',[Series1],\' - \',[Series2] )";
            this.calProduct.Name = "calProduct";
            // 
            // MostComplainedWP
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.DetailReport,
            this.PageFooter});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calTotalHour,
            this.calTotalCompleteJobs,
            this.calPendingJobs,
            this.calProduct});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objChart});
            this.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(51, 52, 44, 43);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Version = "15.2";
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

    }
}
