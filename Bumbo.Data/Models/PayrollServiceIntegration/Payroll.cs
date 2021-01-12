using System.Collections.Generic;

namespace Bumbo.Data.Models.PayrollServiceIntegration
{
    public class Payroll
    {
        public static readonly string DossierId = "4F98EFF7-733D-4AD9-8382-035319401484";
        public List<PayrollItem> Items { get; set; } = new List<PayrollItem>();
    }
}