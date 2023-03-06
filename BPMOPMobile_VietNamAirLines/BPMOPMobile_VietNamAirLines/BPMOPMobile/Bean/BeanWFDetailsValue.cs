using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWFDetailsValue : BeanBase
    {
        public string Lookup { get; set; }
        public int DonGia { get; set; }
        public int SoLuong { get; set; }
        public int TongTien { get; set; }
        public int ID { get; set; }

    }
}
