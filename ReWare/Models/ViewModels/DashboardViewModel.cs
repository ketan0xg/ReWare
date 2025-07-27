using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReWare.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Points { get; set; }

        public List<Item> MyItems { get; set; }
        public List<Swap> MySwaps { get; set; }
        public List<PointsTransaction> MyTransactions { get; set; }

    }

}