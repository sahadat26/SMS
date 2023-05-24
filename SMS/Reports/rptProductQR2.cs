using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using SMS.Models;
using System.Collections.Generic;

namespace SMS.Reports
{
    public partial class rptProductQR2 : DevExpress.XtraReports.UI.XtraReport
    {
        public rptProductQR2(IEnumerable<SAPProduct> Products)
        {
            InitializeComponent();
            obSP.DataSource = Products;
        }

    }
}
