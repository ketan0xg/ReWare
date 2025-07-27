using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReWare.Models
{
    public class ItemImage
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ImagePath { get; set; }

        public virtual Item Item { get; set; }
    }
}