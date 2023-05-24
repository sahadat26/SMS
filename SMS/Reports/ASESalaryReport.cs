using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using SMS.Models;
using System.Collections.Generic;
namespace SMS.Reports
{
    public partial class ASESalaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ASESalaryReport(IEnumerable<SC_ASEApprovedData> obj)
        {
            InitializeComponent();
            objectDataSource1.DataSource = obj;
        }

    }
}
