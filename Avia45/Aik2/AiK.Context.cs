﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aik2
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AiKEntities : DbContext
    {
        public AiKEntities()
            : base("name=AiKEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Links> Links { get; set; }
        public virtual DbSet<Mags> Mags { get; set; }
        public virtual DbSet<Pics> Pics { get; set; }
        public virtual DbSet<WordLinks> WordLinks { get; set; }
        public virtual DbSet<Words> Words { get; set; }
        public virtual DbSet<vwCrafts> vwCrafts { get; set; }
        public virtual DbSet<vwPics> vwPics { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<vwReport> vwReport { get; set; }
        public virtual DbSet<vwArts> vwArts { get; set; }
        public virtual DbSet<vwReportMags> vwReportMags { get; set; }
        public virtual DbSet<Arts> Arts { get; set; }
        public virtual DbSet<Crafts> Crafts { get; set; }
        public virtual DbSet<vwSerials> vwSerials { get; set; }
        public virtual DbSet<Serials> Serials { get; set; }
    }
}
