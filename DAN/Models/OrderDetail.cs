//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderDetail
    {
        public long Id { get; set; }
        public int Quantum { get; set; }
        public decimal Price { get; set; }
        public string Pname { get; set; }
        public long PId { get; set; }
        public long OId { get; set; }
    
        public virtual Order Order { get; set; }
    }
}