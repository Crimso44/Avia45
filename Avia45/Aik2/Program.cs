using Aik2.Dto;
using AutoMapper;
using System;
using System.Windows.Forms;

namespace Aik2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitializeAutomapper();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void InitializeAutomapper()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Arts, ArtDto>();
                cfg.CreateMap<ArtDto, Arts>();
                cfg.CreateMap<vwArts, ArtDto>();
                cfg.CreateMap<Crafts, CraftDto>();
                cfg.CreateMap<CraftDto, Crafts>()
                    .ForMember(d => d.CText, o => o.Ignore());
                cfg.CreateMap<vwCrafts, CraftDto>();
                cfg.CreateMap<CraftDto, vwCrafts>();
                cfg.CreateMap<Pics, PicDto>();
                cfg.CreateMap<PicDto, Pics>()
                    .ForMember(d => d.Text, o => o.Ignore());
                cfg.CreateMap<vwPics, PicDto>();
                cfg.CreateMap<PicDto, vwPics>();
                cfg.CreateMap<Links, LinkDto>();
                cfg.CreateMap<LinkDto, Links>();
                cfg.CreateMap<Report, ReportDto>();
                cfg.CreateMap<ReportDto, Report>();
                cfg.CreateMap<vwReport, ReportDto>();
                cfg.CreateMap<ReportDto, vwReport>();
                cfg.CreateMap<Countries, CountryDto>();
                cfg.CreateMap<Mags, MagDto>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
