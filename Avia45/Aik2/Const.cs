﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aik2
{
    public static class Const
    {
        public static string rus = "АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеЁжзиклмнопрстуфхцчшщъыьэюя";
        public static string lat = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static class Columns
        {

            public static class Art
            {
                public const int ArtId = 0;
                public const int Mag = 1;
                public const int IYear = 2;
                public const int IMonth = 3;
                public const int Author = 4;
                public const int Name = 5;
                public const int NN = 6;
                public const int Serie = 7;
                public const int FullName = 8;
    
                public static int[] SortAuthor = { Art.Author, Art.Name, Art.IYear, Art.IMonth };
                public static int[] SortSerie = { Art.Serie, Art.Name, Art.IYear, Art.IMonth };
                public static int[] SortYear = { Art.IYear, Art.IMonth, Art.Author, Art.Name };

            }

            public static class Craft
            {
                public const int CraftId = 0;
                public const int Construct = 1;
                public const int Name = 2;
                public const int Country = 3;
                public const int IYear = 4;
                public const int Vert = 5;
                public const int Uav = 6;
                public const int Glider = 7;
                public const int LL = 8;
                public const int Single = 9;
                public const int Proj = 10;
                public const int Wings = 11;
                public const int Engines = 12;
                public const int Source = 13;
                public const int Type = 14;
                public const int SeeAlso = 15;
                public const int FullName = 16;
                public const int Wiki = 17;
                public const int Airwar = 18;
                public const int FlyingM = 19;
                public const int Same = 20;
                public const int SeeAlsoId = 21;
                public const int FlyingMId = 22;
                public const int SameId = 23;
            }

            public static class Pic
            {
                public const int PicId = 0;
                public const int XPage = 1;
                public const int NN = 2;
                public const int NNN = 3;
                public const int Type = 4;
                public const int NType = 5;
                public const int Path = 6;
                public const int Art = 7;
                public const int Craft = 8;
                public const int Grp = 9;
                public const int SText = 10;
                public const int ArtId = 11;
                public const int CraftId = 12;
            }
        }
    }
}