using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using SMS.Repositories;


namespace SMS.Reports
{
    public partial class SubMHCost : DevExpress.XtraReports.UI.XtraReport
    {
        public SubMHCost()
        {
            InitializeComponent();
            //JobRepository job = new JobRepository();
            //objManPower.DataSource = job.Get((int)prmJobID.Value).ManpowerCosts;

        }

        

    }
}
