using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

using SMS.Repositories;

namespace SMS.Reports
{
    public partial class SubProduct : DevExpress.XtraReports.UI.XtraReport
    {
        public SubProduct()
        {
            InitializeComponent();
            //JobRepository job = new JobRepository();
            //objProductLine.DataSource = job.Get((int)prmJobID.Value).Products;
        }

    }
}
