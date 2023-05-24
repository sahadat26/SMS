using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class VMCont
    {
        public SC_ASEContribution Cont { get; set; }
        public List<SC_ASEContribution> Conts { get; set; }

        public VMCont()
        {
            Cont = new SC_ASEContribution();
            Conts = new List<SC_ASEContribution>();
        }

        //-----------------------crud contribution-----------------------
        public void AddCont()
        {
            Cont.SL = Conts.Count + 1;
            if(Cont.ASEID==0)
            {
                throw new Exception("No ASE Selected");
            }
            else if(Conts.Any(c=>c.ASEID==Cont.ASEID))
            {
                throw new Exception("ASE ID: "+Cont.ASEID+" already exist!");
            }
            else if(Cont.ContrAmount<=0)
            {
                throw new Exception("0 amount not accepted");
            }
            
            Conts.Add(Cont);
            Cont = new SC_ASEContribution();
        }

        public void DeleteCont(int Index)
        {
            var _cont = Conts.Where(d => d.SL == Index).FirstOrDefault();
            Conts.Remove(_cont);
            int i = 1;
            foreach (var d in Conts)
            {
                d.SL = i;
                i++;
            }
        }

        public void EditCont(int Index)
        {
            Cont = Conts.Where(c=>c.SL==Index).FirstOrDefault();
            Cont.ContrPerc = Cont.DisConrPerc;
        }

        public void UpdateCont()
        {
            var updateObject = Conts.Where(c => c.SL == Cont.SL).FirstOrDefault();
            if (updateObject.ASEID!=Cont.ASEID&& Conts.Any(c => c.ASEID == Cont.ASEID))
            {
                throw new Exception("ASE ID: " + Cont.ASEID + " already exist!");
            }
            else if (Cont.ContrAmount <= 0)
            {
                throw new Exception("Amount cannot be zero");
            }
            updateObject.ASEID=Cont.ASEID;
            updateObject.ContrAmount = Cont.ContrAmount;
            Cont = new SC_ASEContribution();
        }
    }
}