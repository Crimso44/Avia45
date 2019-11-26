using Aik2.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                cfg.CreateMap<Crafts, CraftDto>();
                cfg.CreateMap<CraftDto, Crafts>();
                cfg.CreateMap<vwCrafts, CraftDto>();
                cfg.CreateMap<CraftDto, vwCrafts>();
                cfg.CreateMap<Pics, PicDto>();
                cfg.CreateMap<PicDto, Pics>();
                cfg.CreateMap<vwPics, PicDto>();
                cfg.CreateMap<PicDto, vwPics>();
            });
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
