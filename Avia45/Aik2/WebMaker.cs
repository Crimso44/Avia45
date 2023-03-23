using Aik2.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Aik2.Util;

namespace Aik2
{
    public class WebMaker
    {
        AiKEntities _ctx;

        private List<Pair<int>>[] elements;
        private List<TChunk>[] chunks;
        private List<string> lRuCountries;
        private List<string> lEnCountries;
        private List<string> lSources;
        private List<string> lSourceIds;
        private List<string> lSourceNames;
        private int[] cnts = new int[7];
        private string[] picids = new string[7];
        private bool[] plans = new bool[5];
        private List<Pair<int>> lBicPics;
        private List<Pair<int>> lMagPics;
        private string templ;
        private string upl;
        private string sPath;
        int PrevId;
        string PrevName;
        int NextId;
        string NextName;

        int FirstId;
        string FirstName;
        List<Pair<int>> lAlso;
        List<string> lAlsoYears;
        int AlsoId;
        string AlsoName;
        List<PicArt> pics;
        List<string> sitemap;
        TChunk chunk;
        bool is_stop = false;
        Label _lInfo;

        string s;
        string ss;
        int mains;
        int ii;
        int cnt;

        private class PicArt
        {
            public PicDto p;
            public ArtDto a;
            public CraftDto c;
        }

        private class CraftList
        {
            public CraftDto c;
            public CraftDto cc;
            public CountryDto ci;
            public ArtDto a;
            public MagDto m;
            public int cCnt;
            public int aCnt;
        }


        private class TChunk {
            public int iBeg; public int iEnd;
            public int cnt; public int elmts;
            public string sBeg; public string sEnd;
            public TChunk()
            {
                cnt = 0;
                elmts = 0;
            }
        }

        private class CraftData
        {
            public int CraftId;
            public string CraftText;

            public CraftData(int craftId)
            {
                CraftId = craftId;
            }
        }

        private class ArtData
        {
            public int ArtId;
            public Dictionary<int, CraftData> Crafts;

            public ArtData(int artId)
            {
                ArtId = artId;
                Crafts = new Dictionary<int, CraftData>();
            }
        }

        private string GetMK(int year) {
            var Result = "";
            switch (year) {
                case 11: Result = "Истребители Второй мировой войны"; break;
                case 12: Result = "Бомбардировщики Второй мировой войны"; break;
                case 13: Result = "Ближние разведчики, корректировщики и штурмовики Второй мировой войны"; break;
                case 14: Result = "Гидросамолеты Второй мировой войны"; break;
                case 15: Result = "Морские самолеты палубного и берегового базирования Второй мировой войны"; break;
                case 16: Result = "Военно-транспортные самолеты Второй мировой войны"; break;
                case 17: Result = "Дальние и высотные разведчики Второй мировой войны"; break;
            }
            return Result;
        }

        string sx; string sy; string sMag; string sYear; string sMonth;
        Dictionary<int, ArtData> ArtTexts;

        private void GetYearArt(bool yr, bool my = false)
        {
            var ii = sy.IndexOf("-");
            if (ii >= 0) {
                sYear = sy.Substring(ii + 1);
                sMonth = sy.Substring(0, ii);
                if (my) sMonth = "";
            } else {
                if (yr) {
                    sYear = sy;
                    sMonth = "";
                } else {
                    sMonth = sy;
                    sYear = "";
                }
            }
        }

        int GetArtId(string s) {
            var ii = s.IndexOf("№");
            if (ii >= 0) {
                sx = s.Substring(0, ii - 1);
                sy = s.Substring(ii + 2);
            } else {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                var sr = new string(charArray);
                ii = s.Length - sr.IndexOf(" ") - 1;
                sx = s.Substring(0, ii);
                sy = s.Substring(ii + 1);
            }

            if (sx == "Air Enthusiast") {
                sMag = "AE";
                GetYearArt(false);
            } else if (sx == "Air Pictorial") {
                sMag = "AI";
                GetYearArt(false);
            } else if (sx == "АэроХобби") {
                sMag = "AH";
                GetYearArt(false);
            } else if (sx == "Авиация и Космонавтика") {
                sMag = "AK";
                GetYearArt(false);
            } else if (sx == "АвиаМастер") {
                sMag = "AM";
                GetYearArt(false);
            } else if (sx == "Air International") {
                sMag = "AN";
                GetYearArt(false);
            } else if (sx == "АвиаПарк") {
                sMag = "AP";
                GetYearArt(false);
            } else if (sx == "АС") {
                sMag = "AS";
                GetYearArt(false);
            } else if (sx == "Авиация и Время") {
                sMag = "AV";
                GetYearArt(false);
            } else if (sx == "Flight") {
                sMag = "FT";
                GetYearArt(false);
            } else if (sx == "Aviation Historian") {
                sMag = "HI";
                GetYearArt(false);
            } else if (sx == "История Авиации") {
                sMag = "IA";
                GetYearArt(false);
            } else if (sx == "In Action") {
                sMag = "IN";
                GetYearArt(true);
            } else if (sx == "Jane's All the World Aircraft") {
                sMag = "JS";
                GetYearArt(true);
            } else if (sx == "Jane's Encyclopedia of") {
                sMag = "JS";
                sMonth = "";
                sYear = "1980";
            } else if (sx == "Мир Авиации") {
                sMag = "MA";
                GetYearArt(false);
            } else if (sx == "My photos") {
                sMag = "ME";
                GetYearArt(true, true);
            } else if (sx.Substring(0, Math.Min(sx.Length, "Мировая Авиация".Length)) == "Мировая Авиация") {
                sMag = "MM";
                sy = sy.Substring(0, 3).Trim();
                GetYearArt(false);
            } else if (sx == "Aeroplane Monthly") {
                sMag = "MY";
                GetYearArt(false);
            } else
                MessageBox.Show("Unknown Mag: " + sx);

            var qry = _ctx.Arts.Where(x => x.Mag == sMag);
            if (!string.IsNullOrEmpty(sYear))
                qry = qry.Where(x => x.IYear.ToString() == sYear);
            if (!string.IsNullOrEmpty(sMonth)) {
                if (sMonth.Length == 1) sMonth = "0" + sMonth;
                qry = qry.Where(x => x.IMonth == sMonth);
            } if (sMonth == "")
                qry = qry.OrderBy(x => x.IMonth).ThenBy(x => x.Author).ThenBy(x => x.Name);
            else
                qry = qry.OrderBy(x => x.Author).ThenBy(x => x.Name);
            var art = qry.ToList();
            if (!art.Any())
            {
                MessageBox.Show($"Art not found: {sMag} {sYear} {sMonth}");
                return -1;
            }
            else
                return art[0].ArtId;
        }


        private string GetYear(ArtDto a) {
            var Result = " ";
            if ((a.IYear ?? 0) != 0) {
                if (a.IYear > 1800)
                    Result += a.IYear.ToString() + "-";
                else {
                    Result += a.IYear.ToString();
                    return Result;
                }
            }
            Result += a.IMonth.Trim();
            return Result;
        }


        private string GetArtName2(ArtDto a) {
            var s = a.Author;
            if (s == "-") s = "";
            var ss = a.Name;
            if ((!string.IsNullOrEmpty(ss)) && (ss != "-")) {
                if (!string.IsNullOrEmpty(s)) s += " - ";
                s += ss;
            }
            ss = a.Serie;
            if (!string.IsNullOrEmpty(ss)) s += $"  /{ss}/";
            if ((a.NN ?? 0) != 0) s += $" ({a.NN})";
            return s;
        }


        private string GetArtName(ArtDto a, bool full = false, bool ww1 = false) {
            var Result = "";
            var s = a.Mag.Trim();
            switch (s) {
                case "AE":
                    Result = "Air Enthusiast" + GetYear(a); break;
                case "AH":
                    Result = "АэроХобби" + GetYear(a); break;
                case "AI":
                    Result = "Air Pictorial" + GetYear(a); break;
                case "AK":
                    Result = "Авиация и Космонавтика" + GetYear(a); break;
                case "AM":
                    Result = "АвиаМастер" + GetYear(a); break;
                case "AN":
                    Result = "Air International" + GetYear(a); break;
                case "AP":
                    Result = "АвиаПарк" + GetYear(a); break;
                case "AS":
                    Result = "АС" + GetYear(a); break;
                case "AV":
                    Result = "Авиация и Время" + GetYear(a); break;
                case "FT":
                    Result = "Flight" + GetYear(a); break;
                case "HI":
                    Result = "Aviation Historian" + GetYear(a); break;
                case "IA":
                    Result = "История Авиации" + GetYear(a); break;
                case "IN":
                    Result = "In Action " + a.IYear.ToString(); break;
                case "JS":
                    Result = "Jane's All the World Aircraft " + a.IYear.ToString(); break;
                case "MA":
                    Result = "Мир Авиации" + GetYear(a); break;
                case "ME":
                    Result = "My photos" + GetYear(a); break;
                case "MK":
                    Result = "Моделист-Конструктор " + GetMK(a.IYear.Value); break;
                case "MM":
                    Result = "Мировая Авиация" + GetYear(a); break;
                case "MY":
                    Result = "Aeroplane Monthly" + GetYear(a); break;
                case "OS6":
                    Result = "Изд-во Osprey"; break;
                case "SC":
                    Result = "Изд-во Schiffer"; break;
                default:
                    Result = s + GetYear(a); break;
            }

            if (full) {
                s = GetArtName2(a);
                if (!string.IsNullOrEmpty(s)) {
                    if (ww1) Result = s;
                    else
                        Result += " / " + s;
                }
            }
            return Result;
        }

        private string GetMagName(string s)
        {
            string Result;
            switch (s) {
                case "AE":
                    Result = "Air Enthusiast"; break;
                case "AH":
                    Result = "АэроХобби"; break;
                case "AI":
                    Result = "Air Pictorial"; break;
                case "AK":
                    Result = "Авиация и Космонавтика"; break;
                case "AM":
                    Result = "АвиаМастер"; break;
                case "AN":
                    Result = "Air International"; break;
                case "AP":
                    Result = "АвиаПарк"; break;
                case "AS":
                    Result = "АС"; break;
                case "AV":
                    Result = "Авиация и Время"; break;
                case "FT":
                    Result = "Flight"; break;
                case "HI":
                    Result = "Aviation Historian"; break;
                case "IA":
                    Result = "История Авиации"; break;
                case "IN":
                    Result = "In Action"; break;
                case "JS":
                    Result = "Jane's All the World Aircraft"; break;
                case "MA":
                    Result = "Мир Авиации"; break;
                case "ME":
                    Result = "My photos"; break;
                case "MK":
                    Result = "Моделист-Конструктор"; break;
                case "MM":
                    Result = "Мировая Авиация"; break;
                case "MY":
                    Result = "Aeroplane Monthly"; break;
                case "OS6":
                    Result = "Изд-во Osprey"; break;
                case "SC":
                    Result = "Изд-во Schiffer"; break;
                default:
                    if (s.IndexOf("{") >= 0) {
                        var i = s.IndexOf("{");
                        Result = s.Substring(0, i);
                    } else
                        Result = s;
                    break;
            }
            return Result;
        }


        private string GetEnCountry(string s) {
            var Result = "???";
            var i = lRuCountries.IndexOf(s);
            if (i < 0)
            {
                MessageBox.Show("GetEnCountry: " + s);
            }
            else
            {
                Result = lEnCountries[i];
            }
            return Result;
        }


        private string GetSpanRuEn(string sRu, string sEn) {
            return
                "<span class=\"ru\">" + sRu + "</span>" +
                "<span class=\"en\" style=\"display:none\">" + sEn + "</span>";
        }


        private string GetSpanCountry(string s) {
            return GetSpanRuEn(s, GetEnCountry(s));
        }

        private string GetCraftName(CraftDto q, bool nospan = false) {
            var s = q.Construct;
            var ss = q.Name;
            if (!string.IsNullOrEmpty(ss) && (ss != "-")) {
                if (!string.IsNullOrEmpty(s)) s += " - ";
                s += ss;
            }
            ss = q.IYear.ToString();
            if (!string.IsNullOrEmpty(ss) && (ss != "-")) {
                if (!string.IsNullOrEmpty(s)) s += " - ";
                s += ss;
            }
            if (nospan) {
                ss = q.Country;
            }
            else {
                ss = GetSpanCountry(q.Country);
            }
            if (!string.IsNullOrEmpty(ss) && (ss != "-"))
            {
                if (!string.IsNullOrEmpty(s)) s += " - ";
                s += ss;
            }
            return s;
        }

        private void LoadPic(int iType, CraftDto craft1)
        {
            var bigpic = ""; var magpic = ""; var bigpictype = "";
            templ = File.ReadAllText(sPath + "Site\\Craft.htm");
            while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");
            templ = new StringBuilder(templ)
                .Replace("%Country%", GetSpanCountry(craft1.Country))
                .Replace("%Construct%", craft1.Construct)
                .Replace("%Name%", craft1.Name)
                .Replace("%Year%", craft1.IYear.ToString())
                .Replace("%Vert%", craft1.Vert ?? false ? GetSpanRuEn("Вертолет", "Helicopter") : "")
                .Replace("%UAV%", craft1.Uav ?? false ? GetSpanRuEn("Беспилотный", "Unmanned") : "")
                .Replace("%Glider%", craft1.Glider ?? false ? GetSpanRuEn("Планер", "Glider") : "")
                .Replace("%Single%", craft1.Single ?? false ? GetSpanRuEn("Единственный экземпляр", "One-off model") : "")
                .Replace("%LL%", craft1.LL ?? false ? GetSpanRuEn("Летающая лодка", "Flying boat") : "")
                .Replace("%Proj%", craft1.Proj ?? false ? GetSpanRuEn(" (проект)", " (project)") : "")
                .Replace("%Type%", craft1.Type)
                .Replace("%PrevName%", PrevName)
                .Replace("%PrevLink%", $"Craft{PrevId}.htm")
                .Replace("%NextName%", NextName)
                .Replace("%NextLink%", $"Craft{NextId}.htm")
                .Replace("%AlsoLink%", AlsoId > 0 ? $"Craft{AlsoId}.htm" : "")
                .Replace("%AlsoName%", AlsoName)
                .Replace("%ID%", craft1.CraftId.ToString())
                .Replace("%SeeAlso%", AlsoId > 0 ? GetSpanRuEn("Смотри также: ", "See also: ") : "")
                .ToString();
            var ss = "";
            var craftsSame = _ctx.Crafts.Where(x => x.Same == craft1.CraftId).OrderBy(x => x.IYear).ThenBy(x => x.Construct).ThenBy(x => x.Name).ToList();
            if (craftsSame.Any()) {
                var isCountry = craftsSame.Any(x => x.Country != craft1.Country);
                var isYear = craftsSame.Any(x => x.IYear != craft1.IYear);
                if (isCountry || isYear)
                {
                    ss += "&nbsp;&nbsp; &nbsp;&nbsp;<span class='smaller blue'>";
                    if (isCountry)
                    {
                        ss += GetSpanCountry(craft1.Country);
                    }
                    if (isYear)
                    {
                        if (isCountry)
                        {
                            ss += ", ";
                        }
                        ss += craft1.IYear.ToString();
                    }
                    ss += "</span>";
                }
                foreach (var craftSame in craftsSame) {
                    ss += 
                        "</div><div class='caption'><span class='smaller'>" +
                        craftSame.Construct + " " + craftSame.Name;
                    if (isCountry || isYear)
                    {
                        ss += "&nbsp;&nbsp; &nbsp;&nbsp;<span class='blue'>";
                        if (isCountry)
                        {
                            ss += GetSpanCountry(craftSame.Country);
                        }
                        if (isYear)
                        {
                            if (isCountry)
                            {
                                ss += ", ";
                            }
                            ss += craftSame.IYear.ToString();
                        }
                        ss += "</span>";
                    }
                }
                ss += "</span>";
            }
            templ = templ.Replace("%Same%", ss);

            var i = templ.IndexOf("%TextBegin%");
            var sBeg = templ.Substring(0, i);
            var sEnd = templ.Substring(i + 11);
            i = sEnd.IndexOf("%TextEnd%");
            var sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 9);
            ss = craft1.CText?.Trim() ?? "";
            var sxx = ""; var sxy = "";
            var cnt = 0;
            var sspace = "";
            string sx;
            if (!string.IsNullOrEmpty(ss)) {
                ss = new StringBuilder(ss)
                    .Replace("\n\r ", "<br>&nbsp;&nbsp;")
                    .Replace("\n ", "<br>&nbsp;&nbsp;")
                    .Replace("\n\r", "<br>")
                    .Replace("\n", "<br>")
                    .Replace("<br><br><br><br>", "<br><br><br>")
                    .ToString();
                i = ss.IndexOf("/*");
                if (i >= 0) {
                    while (i >= 0) {
                        var sxz = "";
                        var j = ss.IndexOf("{");
                        if ((j > i) || (j < 0)) {
                            sx = craft1.Name;
                        } else {
                            var k = ss.IndexOf("}");
                            sx = ss.Substring(j + 1, k - j - 1);
                            ss = ss.Substring(0, j) + ss.Substring(k + 1);
                            i = ss.IndexOf("/*");
                        }
                        var st = ss.Substring(0, i).Trim();
                        if (!string.IsNullOrEmpty(st)) {
                            j = st.IndexOf("<");
                            while (j >= 0) {
                                var k = st.IndexOf(">");
                                st = st.Substring(0, j).Trim() + st.Substring(k + 1);
                                j = st.IndexOf("<");
                            }

                            if (st.Replace("&nbsp;", "") != "") {
                                MessageBox.Show(craft1.Construct + " " + craft1.Name + "\n" + st);
                            }
                        }

                        ss = ss.Substring(i + 2);

                        if (sx.Length >= 6 && sx.Substring(0, 6) == "Flight") {
                            i = 0;
                            while ((i < ss.Length) && (i < 100) && (ss[i] != '<')) {
                                sxz = sxz + ss[i];
                                i++;
                            }
                            if ((i < 100) && (!string.IsNullOrEmpty(sxz))) {
                                sxz = "<br/><span style=\"font-size:smaller\">" + sxz + "</span>";
                            } else {
                                sxz = "";
                            }
                        }

                        sxy +=
                            $"<div id=\"ltabs-{cnt}\" " +
                            $"class=\"w500 text_menu\" onclick=\"ShowText(&#39;tabs-{cnt}&#39;)\">" +
                            sx + sxz + "</div>\n";
                        sxx +=
                            $"<div id=\"tabs-{cnt}\" class=\"storyline\">\n" +
                            "<span class=\"tabhead\">\n" +
                            "<h3>" + sx + "</h3></span>\n";
                        i = ss.IndexOf("*/");
                        var sn = ss.Substring(0, i);
                        if ((cnt == 0) && (sn.Length > 512)) {
                            j = 256;
                            while ((j < sn.Length) && (sn[j] != ' ')) j++;


                            sn = sn.Substring(0, j) +
                               "<span id=\"more_text\" class=\"red pointer hidden\" onclick=\"DoMoreText()\"> " + GetSpanRuEn("Дальше", "More") + "&gt;&gt;&gt;</span>\n" +
                               "<span id=\"long_text\">" + sn.Substring(j + 1) + "</span>";
                        }
                        sxx += sn + "\n</div>\n";
                        ss = ss.Substring(i + 2);
                        cnt++;

                        i = ss.IndexOf("/*");
                    }
                    if (ss.Trim() != "") {
                        MessageBox.Show("x " + craft1.Construct + " " + craft1.Name + "\n" + ss);
                    }
                    sxy = "<div class=\"dark\">\n" + sxy + "</div>";
                    if (cnt > 1) {
                        sxy = "<div> " +
                            "<span class=\"ru small\">Описание:</span>" +
                            "<span class=\"en small\" style=\"display:none\">Description:</span>\n" +
                            "</div> " + sxy;
                    }
                } else {
                    sxx =
                        "<div id=\"tabs-0\" class=\"storyline\">\n";
                    if ((cnt == 0) && (ss.Length > 512)) {
                        var j = 256;
                        while ((j < ss.Length) && (ss[j] != ' ')) j++;


                        ss = ss.Substring(0, j) +
                            "<span id=\"more_text\" class=\"red pointer hidden\" onclick=\"DoMoreText()\"> " + GetSpanRuEn("Дальше", "More") + "&gt;&gt;&gt;</span>\n" +
                            "<span id=\"long_text\">" + ss.Substring(j + 1) + "</span>";
                    }
                    sxx += ss + "\n</div>\n";
                    sxy =
                      "<div class=\"dark\">\n" +
                      "<div id=\"ltabs-0\" class=\"w500 text_menu\" onclick=\"ShowText(&#39;tabs-0&#39;)\">" + GetSpanRuEn("Описание", "Description") + "\n" +
                      "</div></div>\n";
                    cnt = 1;
                }
                sBeg += sMid.Replace("%Text%", sxx);
                sspace = "class=\"space\"";
            }

            sEnd = sEnd.Replace("%ClassSpace%", sspace);

            if (!string.IsNullOrEmpty(craft1.Wiki) ||
                !string.IsNullOrEmpty(craft1.Airwar) ||
                craft1.FlyingM.HasValue) {
                sxy += "<div class=\"clear\"></div><div class=\"dark\">";
            }

            if (!string.IsNullOrEmpty(craft1.Wiki)) {
                sxy +=
                    "<div class=\"w500 wiki\">\n" +
                    $"<a href=\"{ craft1.Wiki}\" target=\"_blank\" rel=\"nofollow\">" +
                    "<img src=\"/Site/Partners/Wiki_ico.gif\">&nbsp;Wikipedia</a>\n" +
                    "</div>\n";
            }
            if (!string.IsNullOrEmpty(craft1.Airwar)) {
                sxy +=
                   "<div class=\"w500 wiki\">\n" +
                   $"<a href=\"{craft1.Airwar}\" target=\"_blank\" rel=\"nofollow\">" +
                   "<img src=\"/Site/Partners/airwar_ico.gif\">&nbsp;" + GetSpanRuEn("Уголок Неба", "Airwar") + "</a>\n" +
                   "</div>\n";
            }
            if (craft1.FlyingM.HasValue) {
                sxy +=
                   "<div class=\"w500 wiki\">\n" +
                   $"<a style=\"background-color:#e6e6de; color: black; font-size: 16px; font-family:'Times New Roman'\" href=\"http://flyingmachines.ru/Site2/Crafts/Craft{craft1.FlyingM}.htm\" target=\"_blank\" rel=\"nofollow\">" +
                   "TheirFlyingMachines</a>\n" +
                   "</div>\n";
            }

            if (!string.IsNullOrEmpty(craft1.Wiki) ||
                !string.IsNullOrEmpty(craft1.Airwar) ||
                craft1.FlyingM.HasValue)
            {
                sxy += "</div>";
            }

            sBeg = sBeg.Replace("%Description%", sxy);
            templ = sBeg + sEnd;

            i = templ.IndexOf("%begin%");
            sBeg = templ.Substring(0, i);
            sEnd = templ.Substring(i + 7);
            i = sEnd.IndexOf("%end%");
            sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 5);

            var sb = new StringBuilder(sBeg);
            var ix = 0;
            if (pics.Any()) {
                foreach (var pic in pics) {
                    if (bigpic == "") {
                        bigpic = pic.p.Path.Replace("\\", "/");
                        bigpictype = pic.p.Type?.Trim() ?? "";
                    } else if ((bigpictype == "s") && ((pic.p.Type?.Trim() ?? "") != "s")) {
                        bigpic = pic.p.Path.Replace("\\", "/");
                        bigpictype = pic.p.Type?.Trim() ?? "";
                    }

                    if (magpic != "-") {
                        var sm = pic.p.Path.Substring(0, 2);
                        if (magpic == "") magpic = sm;
                        if (magpic != sm) magpic = "-";
                    }

                    ss = pic.p.Type?.Trim() ?? "";
                    if ((iType == 0) ||
                        ((iType == 1) && (ss == "s")) ||
                        ((iType == 2) && (ss == "fc")) ||
                        ((iType == 3) && (ss == "f")) ||
                        ((iType == 4) && (ss == "c")) ||
                        ((iType == 5) && ((ss == "fd") || (ss == "fr"))) ||
                        ((iType == 6) && (ss != "s") && (ss != "fc") && (ss != "f") && (ss != "c") && (ss != "fd") && (ss != "fr"))) {

                        sx = pic.p.Grp;
                        if (!string.IsNullOrEmpty(sx)) {
                            sx =
                                $"<div class=\"pic_group{(ix == 0 ? " first_group" : "")}\">" +
                                $"{(sx == "-" ? "&nbsp;" : sx)}</div>";
                            sb.Append(sx);
                        }

                        ix++;

                        ss = pic.p.Text ?? "";
                        ss = new StringBuilder(ss)
                            .Replace("\n\r ", "<br>&nbsp;&nbsp;")
                            .Replace("\n ", "<br>&nbsp;&nbsp;")
                            .Replace("\n\r", "<br>")
                            .Replace("\n", "<br>")
                            .ToString();

                        sx = new StringBuilder(sMid)
                            .Replace("%PicPath%", pic.p.Path.Replace("\\", "/"))
                            .Replace("%ArtLink%", $"../Arts/Art{pic.p.ArtId}.htm")
                            .Replace("%Art%", GetArtName(pic.a, true))
                            .Replace("%PicText%", ss)
                            .ToString();

                        sb.Append(sx);
                    }

                }
            } else {
                sb.Append("<tr><td>" + GetSpanRuEn("Нет фотографий", "No photos") + "</td></tr>");
            }

            lBicPics.InsertPairSortedById(new Pair<int> { Name = bigpic, Id = craft1.CraftId });
            lMagPics.InsertPairSortedById(new Pair<int> { Name = magpic, Id = craft1.CraftId });

            templ = (sb.ToString() + sEnd).Replace("%BigPicPath%", bigpic);

            i = templ.IndexOf("%xbegin%");
            sBeg = templ.Substring(0, i);
            sEnd = templ.Substring(i + 8);
            i = sEnd.IndexOf("%xend%");
            sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 6);
            i = sMid.IndexOf("%xmid1%");
            var sMid2 = sMid.Substring(i + 7);
            sMid = sMid.Substring(0, i);
            i = sMid2.IndexOf("%xmid2%");
            var sMid3 = sMid2.Substring(i + 7);
            sMid2 = sMid2.Substring(0, i);
            i = sMid3.IndexOf("%xmid3%");
            var sMid4 = sMid3.Substring(i + 7);
            sMid3 = sMid3.Substring(0, i);

            sb = new StringBuilder(sBeg);
            if (lAlso.Count > 0) {
                sb.Append(sMid);
                foreach (var lAls in lAlso) {
                    if (craft1.CraftId == lAls.Id)
                        sx = sMid3;
                    else
                        sx = sMid2;
                    sx = new StringBuilder(sx)
                        .Replace("%SameName%", lAls.Name)
                        .Replace("%SameLink%", $"Craft{lAls.Id}.htm")
                        .ToString();
                    sb.Append(sx);
                }
                sb.Append(sMid4);
            }

            templ = sb.ToString() + sEnd;


            if (iType == 0) {
                if (cnt == 0)
                    sx = "";
                else
                    sx =
                        "<div class=\"dark\">\n" +
                        "<div class=\"w500 photo\" onclick=\"ShowPics()\">\n" +
                        GetSpanRuEn("Фотографии", "Photos") + "\n" +
                        "</div></div>\n";
            } else {
                sx = "<div>\n" +
                    "<span class=\"ru small\">Фотографии:</span>" +
                    "<span class=\"en small\" style=\"display:none\">Photos:</span>\n" +
                    "</div>\n" +
                    "<div class=\"dark\">";
                for (i = 1; i <= 6; i++)
                {
                    if (cnts[i] != 0)
                    {
                        if (i == iType)
                        {
                            sx += "<div class=\"w500 photo active\" onclick=\"ShowPics()\">\n";
                        }
                        else
                        {
                            s = $"-{i}";
                            if (i == mains) s = "";
                            sx +=
                                "<div class=\"w500 photo\">\n" +
                                $"<a class=\"en_href\" href=\"Craft{craft1.CraftId}{s}.htm#pics\">\n";
                        }
                        sx += $"<img src=\"../../Images6m/{picids[i]}\"><br>\n";
                        switch (i)
                        {
                            case 1: sx += GetSpanRuEn("Боковые проекции", "Sideviews"); break;
                            case 2: sx += GetSpanRuEn("Цветные фото", "Color photos"); break;
                            case 3: sx += GetSpanRuEn("Ч/б фото", "B/w photos"); break;
                            case 4: sx += GetSpanRuEn("Кабина", "Cockpit"); break;
                            case 5: sx += GetSpanRuEn("Обломки", "Wrecks"); break;
                            case 6:
                                ss = ""; var sw = "";
                                if (plans[1]) ss += "модели, ";
                                if (plans[2]) ss += "рисунки, ";
                                if (plans[1] || plans[2]) sw += "models, ";
                                if (plans[3])
                                {
                                    ss += "схемы, ";
                                    sw += "plans, ";
                                }
                                if (plans[4])
                                {
                                    ss += "чертежи, ";
                                    sw += "drawings, ";
                                }
                                ss = ss.Substring(0, ss.Length - 2);
                                sw = sw.Substring(0, sw.Length - 2);
                                ss = ss.Substring(0, 1).ToUpper() + ss.Substring(1);
                                sw = sw.Substring(0, 1).ToUpper() + sw.Substring(1);
                                sx += GetSpanRuEn(ss, sw);
                                break;
                        }
                        sx += $" ({cnts[i]})";
                        if (i != iType) sx += "</a>";
                        sx += "</div>\n";
                    }
                }
                sx += "</div>\n";
            }
            templ = templ.Replace("%Types%", sx);


            sx = $"-{iType}";
            if (iType == mains) sx = "";

            s = sPath + $"Site\\Crafts\\Craft{craft1.CraftId}{sx}.htm";
            ss = sPath + $"Upload\\Site\\Crafts\\Craft{craft1.CraftId}{sx}.htm";
            var is_upl = false;
            if (File.Exists(s)) {
                var res = true;
                try
                {
                    upl = File.ReadAllText(s);
                    res = (templ != upl);
                } catch (Exception e) {
                    MessageBox.Show(e.Message + " " + s);
                }
                if (res) {
                    File.WriteAllText(ss, templ, Encoding.UTF8);
                    is_upl = true;
                }
            } else {
                File.WriteAllText(ss, templ, Encoding.UTF8);
                is_upl = true;
            }
            if (is_upl)
            {
                File.WriteAllText(s, templ, Encoding.UTF8);
            }
            AddSitemap($"Site\\Crafts\\Craft{craft1.CraftId}{sx}.htm", s, is_upl);

        }

        private void AddSitemap(string s, string sPath, bool is_upl) {
            DateTime d;
            if (is_upl) d = DateTime.Now;
            else
                d = File.GetLastWriteTime(sPath);
            var ss = d.ToString("yyyy-MM-dd");
            sitemap.Add(
                          "<url><loc>http://aviadejavu.ru/" + s.Replace("\\", "/") + "</loc>" +
                          "<lastmod>" + ss + "</lastmod>" +
                          "</url>");
        }


        private string GetPrVal(CraftList craft, string indField, bool is_craft)
        {
            s = "";

            if (indField == "Photos")
            {
                s = (is_craft ? craft.cCnt : craft.aCnt).ToString();
            }
            else if (indField == "TextLen")
            {
                s = craft.c.CText?.Length.ToString();
            }
            else if (is_craft)
            {
                switch (indField)
                {
                    case "Country": s = craft.c.Country; break;
                    case "Eng": s = craft.ci.Eng; break;
                    case "Construct": s = craft.c.Construct; break;
                    case "Name": s = craft.c.Name; break;
                    case "IYear": s = craft.c.IYear.ToString(); break;
                    case "Wings": s = craft.c.Wings; break;
                    case "Engines": s = craft.c.Engines; break;
                    default: MessageBox.Show($"indField craft {indField}"); break;
                }
            }
            else
            {
                switch (indField)
                {
                    case "IYear": s = craft.a.IYear.ToString(); break;
                    case "Template": s = craft.m?.Template ?? ""; break;
                    case "Author": s = craft.a.Author; break;
                    case "Name": s = craft.a.Name; break;
                    case "Serie": s = craft.a.Serie; break;
                    default: MessageBox.Show($"indField art {indField}"); break;
                }
            }
            return s;
        }

        private void CreatePage(List<CraftList> crafts, string sTempl, string sPage, bool is_craft, int nn, int xx, string indField, int xtype) {
            // xtype: 0 - 10 стандартных страниц; 1 - по названию; 2 - 3 первые буквы

            _lInfo.Text = $"{(is_craft ? "Crafts" : "Arts")} ... {indField} {nn}";
            Application.DoEvents();
            if (is_stop) { return; };


            string s;
            int craftInd = 0; //!!!??? -1;

            templ = File.ReadAllText(sPath + "Site\\" + sTempl);
            while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");

            var sBeg = templ;
            var sEnd = "";
            var sMid = "";
            var i = templ.IndexOf("%begin%");
            if (i >= 0)
            {
                sBeg = templ.Substring(0, i);
                sEnd = templ.Substring(i + 7);
                i = sEnd.IndexOf("%end%");
                sMid = sEnd.Substring(0, i);
                sEnd = sEnd.Substring(i + 5);
            }

            var d = 0;
            if (!is_craft) d = 10;

            var ch = chunks[xx];
            var chunksize = (crafts.Count / 10) + 1;
            if (xtype > 0) {
                if (nn == 0) {
                    var ll = elements[xx];
                    foreach (var craft in crafts) {
                        s = GetPrVal(craft, indField, is_craft);
                        if (xtype == 2) s = s.Substring(0, Math.Min(3, s.Length));
                        var el = ll.FirstOrDefault(x => x.Name == s); i = el == null ? -1 : ll.IndexOf(el);
                        if (i < 0) { ll.Add(new Pair<int>() { Name = s, Id = 0 }); i = ll.Count - 1; }
                        ll[i].Id = ll[i].Id + 1;
                    }

                    chunk = new TChunk();
                    for (i = 0; i < ll.Count; i++) {
                        if (chunk.cnt == 0) {
                            chunk.iBeg = i;
                            chunk.sBeg = ll[i].Name;
                        }
                        if (ll[i].Id >= chunksize) {
                            if (chunk.cnt > 0) {
                                ch.Add(chunk);
                                chunk = new TChunk();
                                chunk.iBeg = i;
                                chunk.sBeg = ll[i].Name;
                            }
                            chunk.cnt = ll[i].Id;
                            chunk.elmts = 1;
                            chunk.iEnd = i;
                            chunk.sEnd = ll[i].Name;
                            ch.Add(chunk);
                            chunk = new TChunk();
                        } else {
                            chunk.cnt += ll[i].Id;
                            chunk.elmts++;
                            chunk.iEnd = i;
                            chunk.sEnd = ll[i].Name;
                        }
                    }
                    if (chunk.cnt > 0) ch.Add(chunk);

                    i = 0;
                    while (i < ch.Count) {
                        chunk = ch[i];
                        if ((chunk.cnt > Math.Floor(chunksize * 1.5)) && (chunk.elmts > 1)) {
                            var minichunks = Math.Round((float)chunk.cnt / chunksize);
                            if (minichunks == 0) minichunks = 1;
                            var minichunksize = (chunk.cnt / minichunks) + 1;

                            var chcnt = 1;
                            var ch1 = new TChunk();
                            ch1.iBeg = chunk.iBeg;
                            ch1.sBeg = chunk.sBeg;
                            ch1.iEnd = chunk.iBeg;
                            ch1.sEnd = chunk.sBeg;
                            ch.Insert(i + chcnt, ch1);
                            var j = chunk.iBeg;
                            while ((j <= chunk.iEnd) && (chcnt < minichunks)) {
                                if ((ch1.cnt + ll[j].Id) > minichunksize) {
                                    chcnt++;
                                    var d1 = ch1.cnt + ll[j].Id - minichunksize;
                                    var d2 = minichunksize - ch1.cnt;
                                    if (d1 > d2) {
                                        ch1 = new TChunk();
                                        ch1.iBeg = j;
                                        ch1.sBeg = ll[j].Name;
                                        ch1.iEnd = j;
                                        ch1.sEnd = ll[j].Name;
                                        ch1.cnt = ll[j].Id;
                                        ch1.elmts = 1;
                                        ch.Insert(i + chcnt, ch1);
                                    } else {
                                        ch1.cnt += ll[j].Id;
                                        ch1.iEnd = j;
                                        ch1.sEnd = ll[j].Name;
                                        ch1.elmts++;
                                        ch1 = new TChunk();
                                        ch1.iBeg = j + 1;
                                        ch1.iEnd = j + 1;
                                        if ((j + 1) < ll.Count) {
                                            ch1.sBeg = ll[j + 1].Name;
                                            ch1.sEnd = ll[j + 1].Name;
                                        }
                                        ch.Insert(i + chcnt, ch1);
                                    }
                                } else {
                                    ch1.cnt += ll[j].Id;
                                    ch1.iEnd = j;
                                    ch1.sEnd = ll[j].Name;
                                    ch1.elmts++;
                                }
                                j++;
                            }
                            while (j <= chunk.iEnd) {

                                ch1.cnt += ll[j].Id;
                                ch1.iEnd = j;
                                ch1.sEnd = ll[j].Name;
                                ch1.elmts++;

                                j++;
                            }
                            if (ch1.cnt == 0) {
                                ch.Remove(ch1);
                                chcnt--;
                            }

                            ch.Remove(chunk);
                            i += chcnt - 1;
                        }

                        i++;
                    }
                    craftInd = 0;

                }
                else {
                    chunk = ch[nn - 1];
                    if (chunk.sBeg == "")
                        craftInd = 0;
                    else
                    {
                        craftInd = crafts.IndexOf(crafts.First(x =>
                        {
                            var sv = GetPrVal(x, indField, is_craft);
                            return string.Compare(sv, chunk.sBeg) >= 0;
                        }));
                    }
                    cnt = chunk.cnt;
                }
            } else {
                if (nn > 0) {
                    cnt = chunksize;
                    craftInd = chunksize * (nn - 1);
                }
            }

            var sb = new StringBuilder(sBeg);
            var sl = new List<string>();
            while (craftInd < crafts.Count) {
                var craft = crafts[craftInd];
                if (is_craft) {

                    var bigpic = "";
                    var iEl = lBicPics.BinaryIndexOfPairById(craft.c.CraftId);
                    if (iEl >= 0)
                    {
                        var el = lBicPics[iEl];
                        bigpic = el.Name;
                    }
                    else MessageBox.Show($"BigPic {craft.c.CraftId}");

                    var magpic = "";
                    var iMl = lMagPics.BinaryIndexOfPairById(craft.c.CraftId);
                    if (iMl >= 0)
                    {
                        var ml = lMagPics[iMl];
                        magpic = ml.Name;
                    }
                    else MessageBox.Show($"MagPic {craft.c.CraftId}");

                    var sSame = craft.c.Same.HasValue 
                        ? $"{craft.cc?.Country} - {craft.cc?.Construct} - {craft.cc?.Name} - {craft.cc?.IYear}"
                        : "";
                    var sCraftLink = (craft.c.Same.HasValue ? (craft.cc?.CraftId ?? -1) : craft.c.CraftId).ToString();
                    var magCode = (magpic == "") || (magpic == "-") ? "" : magpic;
                    var magName = "";
                    var magDiv = magCode != "" ? GetMagPic(magpic, out magName) : "";
                    var sTextLen = (craft.c.Same.HasValue ? craft.cc?.CText?.Length ?? 0 : craft.c.CText?.Length ?? 0).ToString();

                    // %Country% %CountryEn% %Construct% %Name% %Year% 
                    // %Vert%UAV%Glider%LL%Single%Proj% 
                    // %Wings% %Engines% %Type% %Photos% 
                    // %MagCode% %MagName% %Textlen%
                    // %Wiki% %Airwar% %FlyingM%
                    // %MainPicPath% %Same% %CraftLink%
                    sl.Add(
                        $"{craft.c.Country}~{GetEnCountry(craft.c.Country)}~{craft.c.Construct}~{craft.c.Name}~{craft.c.IYear}~"+
                        $"{(craft.c.Vert ?? false ? "В" : "")}{(craft.c.Uav ?? false ? "Б" : "")}{(craft.c.Glider ?? false ? "П" : "")}{(craft.c.Single ?? false ? "1" : "")}{(craft.c.LL ?? false ? "Л" : "")}{(craft.c.Proj ?? false ? "Х" : "")}~" +
                        $"{craft.c.Wings}~{craft.c.Engines}~{craft.c.Type}~{craft.cCnt}~" +
                        $"{magCode}~{magName}~{sTextLen}~" +
                        $"{craft.c.Wiki + craft.cc?.Wiki}~{craft.c.Airwar + craft.cc?.Airwar}~{craft.c.FlyingM}{craft.cc?.FlyingM}~" +
                        $"{bigpic}~{sSame}~{sCraftLink}");
                }
                else {
                    sb.Append(new StringBuilder(sMid)
                        .Replace("%Date%", GetYear(craft.a))
                        .Replace("%Mag%", GetArtName(craft.a))
                        .Replace("%Author%", craft.a.Author)
                        .Replace("%Name%", craft.a.Name)
                        .Replace("%Serie%", craft.a.Serie)
                        .Replace("%NN%", (craft.a.NN ?? 0) == 0 ? "" : craft.a.NN.ToString())
                        .Replace("%Photos%", craft.aCnt.ToString())
                        .Replace("%ArtLink%", $"../Arts/Art{craft.a.ArtId}.htm")
                        .ToString());
                }
                craftInd++;
                if (nn > 0) {
                    cnt--;
                    if (cnt == 0) break;
                }

                Application.DoEvents();
                if (is_stop) { return; };
            }

            templ = sb.ToString() + sEnd;

            sBeg = templ;

            s = "";
            if (xtype == 0)
                ii = 10;
            else
                ii = ch.Count;
            for (i = 0; i <= ii; i++) {
                if ((i > 0) && (xtype > 0))
                    chunk = ch[i - 1];
                var sx = "<span>";
                var sw = "";
                if (i == 0)
                    ss = GetSpanRuEn("Все", "All");
                else {
                    if (xtype == 0) {
                        ss = i.ToString();
                        sx = "<span>";
                    } else {
                        string s1;
                        string s2;
                        if ((indField == "Mag") || (indField == "Template")) {
                            s1 = GetMagName(chunk.sBeg);
                            s2 = GetMagName(chunk.sEnd);
                        } else {
                            s1 = chunk.sBeg;
                            s2 = chunk.sEnd;
                        }

                        if (s1 == s2)
                            ss = s1 == "" ? "&nbsp;" : s1;
                        else
                            ss = (s1 == "" ? "?" : s1) + " - " + s2;
                        sx = $"<span class=\"ru\" title=\"Записей: {chunk.cnt}\">";
                        sw = $"<span class=\"en\" style=\"display: none\" title=\"Records: {chunk.cnt}\">";
                    }
                }
                string sww;
                if (i == nn) {
                    sww = "<b>" + ss + "</b></span>";
                } else {
                    var iw = xx;
                    if (iw == 10) iw = 1;
                    sww = $"<a class=\"en_href\" href=\"{(is_craft ? "Crafts" : "Arts")}{(iw - d)}{(i == 0 ? "" : "-" + i.ToString())}.htm\">{ss}</a></span>";
                }
                ss = sx + sww;
                if (!string.IsNullOrEmpty(sw)) ss += sw + sww;
                s += "<td>" + ss + "</td>";
            }
            sBeg = sBeg.Replace("%pager%", s);

            s = "";
            for (i = 1; i <= 8; i++) {
                if (is_craft || (i < 7)) {
                    ss = (i == (xx - d) ? "b" : $"a class=\"en_href\" href=\"{(is_craft ? "Crafts" : "Arts")}{i}{(is_craft ? "" : "-1")}.htm\"");
                    var sss = (i == (xx - d) ? "/b" : "/a");
                    sBeg = new StringBuilder(sBeg)
                        .Replace($"%Sort{i}%", ss)
                        .Replace($"%SortX{i}%", sss)
                        .ToString();
                }
            }

            templ = sBeg;

            sPage = "Indexes\\" + sPage;
            if (nn > 0) sPage = sPage.Replace(".htm", $"-{nn}.htm");
            s = sPath + "Site\\" + sPage;
            ss = sPath + "Upload\\Site\\" + sPage;
            Directory.CreateDirectory(Path.GetDirectoryName(s));
            Directory.CreateDirectory(Path.GetDirectoryName(ss));
            var is_upl = false;
            if (File.Exists(s)) {
                upl = File.ReadAllText(s);
                if (templ != upl) {
                    File.WriteAllText(ss, templ, Encoding.UTF8);
                    is_upl = true;
                }
            } else {
                File.WriteAllText(ss, templ, Encoding.UTF8);
                is_upl = true;
            }
            if (is_upl)
            {
                File.WriteAllText(s, templ, Encoding.UTF8);
            }
            AddSitemap("Site\\" + sPage, s, is_upl);

            if (is_craft)
            {
                s = s.Replace(".htm", ".dat");
                ss = ss.Replace(".htm", ".dat");
                is_upl = false;
                templ = string.Join("\n", sl.ToArray());
                if (File.Exists(s))
                {
                    upl = File.ReadAllText(s);
                    if (templ != upl)
                    {
                        File.WriteAllText(ss, templ, Encoding.UTF8);
                        is_upl = true;
                    }
                }
                else
                {
                    File.WriteAllText(ss, templ, Encoding.UTF8);
                    is_upl = true;
                }
                if (is_upl)
                {
                    File.WriteAllText(s, templ, Encoding.UTF8);
                }

            }

            Application.DoEvents();

        }

        private string GetMagPic(string magpic, out string Result) {
            Result = "";
            var div = "";
            switch (magpic) {
                case "AE": Result = "Air Enthusiast"; break;
                case "AH": Result = "АэроХобби"; break;
                case "AI": Result = "Air Pictorial"; break;
                case "AK": Result = "Авиация и Космонавтика"; break;
                case "AM": Result = "АвиаМастер"; break;
                case "AN": Result = "Air International"; break;
                case "AP": Result = "АвиаПарк"; break;
                case "AS": Result = "АС"; break;
                case "AV": Result = "Авиация и Время"; break;
                case "FT": Result = "Flight"; break;
                case "HI": Result = "Aviation Historian"; break;
                case "IA": Result = "История Авиации"; break;
                case "IN": Result = "In Action"; break;
                case "JS": Result = "Jane's All the World Aircraft"; break;
                case "MA": Result = "Мир Авиации"; break;
                case "ME": Result = "My photos"; break;
                case "MK": Result = "Моделист-Конструктор"; break;
                case "MM": Result = "Мировая Авиация"; break;
                case "MY": Result = "Aeroplane Monthly"; break;
                case "OS": Result = "Изд-во Osprey"; break;
                case "SC": Result = "Изд-во Schiffer"; break;
            }

            if (!string.IsNullOrEmpty(Result))
                div = "&nbsp;" +
                    $"<span class=\"ru smallest\" title=\"Фото: {Result}\">{magpic}</span>" +
                    $"<span class=\"en smallest\" title=\"Photos: {Result}\" style=\"display:none\">{magpic}</span>";
            return div;
        }

        private void LoadDir(string sPath, string ssPath) {

            _lInfo.Text = sPath;
            Application.DoEvents();
            if (is_stop) { return; };

            var sPath1 = sPath.Replace("\\..\\", "\\Upload\\");
            var ssPath1 = ssPath.Replace("\\..\\", "\\Upload\\");
            var dirs = Directory.GetDirectories(sPath);
            foreach (var dir in dirs) {
                var sr = Path.GetFileName(dir);
                LoadDir(sPath + "\\" + sr, ssPath + "\\" + sr);
            }
            var files = Directory.GetFiles(sPath, "*.jpg");
            foreach (var f in files) {
                var file = Path.GetFileName(f);
                if (File.Exists(ssPath + "\\" + file)) {
                    continue;
                    /*var fi = new FileInfo(ssPath + "\\" + file);
                    if (fi.Length != 0) {
                        var dnew = fi.CreationTime;
                        var fiOld = new FileInfo(sPath + "\\" + file);
                        var dold = fiOld.CreationTime;
                        if (dold < dnew) continue;
                    }*/
                }

                Directory.CreateDirectory(ssPath);
                Directory.CreateDirectory(sPath1);
                Directory.CreateDirectory(ssPath1);

                const int wMax = 200;
                const int hMax = 150;

                using (var srcImage = Image.FromFile(sPath + "\\" + file))
                {
                    var w = wMax;
                    var h = (srcImage.Height * wMax) / srcImage.Width;
                    if (h > hMax)
                    {
                        h = hMax;
                        w = (srcImage.Width * hMax) / srcImage.Height;
                    }

                    using (var newImage = new Bitmap(w, h))
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, w, h));

                        var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        var myEncoderParameters = new EncoderParameters(1);
                        var myEncoderParameter = new EncoderParameter(myEncoder, 80L);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        newImage.Save(ssPath + "\\" + file, jpgEncoder, myEncoderParameters);
                        newImage.Save(ssPath1 + "\\" + file, jpgEncoder, myEncoderParameters);
                        File.Copy(sPath + "\\" + file, sPath1 + "\\" + file);
                    }
                }
            }
        }


        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public void PrepareWeb6(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            if (!(MessageBox.Show("Prepare Web6 ?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;

            is_stop = false;
            _ctx = ctx;
            _lInfo = lInfo;
            string s;
            string ss;
            string sBeg;
            string sEnd;
            string sMid;
            int i;
            int ii;
            int cnt;


            lRuCountries = new List<string>();
            lEnCountries = new List<string>();

            sitemap = new List<string>();
            sitemap.Add(
              "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
              "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            elements = new List<Pair<int>>[21];
            chunks = new List<TChunk>[21];
            for (i = 1; i < 20; i++) {
                elements[i] = new List<Pair<int>>();
                //elements[i].CaseSensitive := True;
                chunks[i] = new List<TChunk>();
            }

            sPath = _imagesPath + "AviaDejaVu\\";

            Directory.CreateDirectory(sPath + "Upload\\Site");
            Directory.CreateDirectory(sPath + "Upload\\Site\\Arts");
            Directory.CreateDirectory(sPath + "Upload\\Site\\Crafts");


            var sPathi = sPath + "..\\Images6";
            var ssPath = sPathi + "m";
            var dirs = Directory.GetDirectories(sPathi, "*.");
            foreach (var dir in dirs) {
                var srx = Path.GetFileName(dir);
                LoadDir(sPathi + "\\" + srx, ssPath + "\\" + srx);
                if (is_stop) { return; }
            }


            lRuCountries.Clear();
            lEnCountries.Clear();
            var cntrs = _ctx.Countries.OrderBy(x => x.Rus).ToList();
            foreach (var cntr in cntrs) {
                lRuCountries.Add(cntr.Rus);
                lEnCountries.Add(cntr.Eng);
            }

            lBicPics = new List<Pair<int>>();
            lMagPics = new List<Pair<int>>();

            var IBQuery1 = _ctx.Crafts.Where(x => !x.Same.HasValue && x.Source == "6")
                .OrderBy(x => x.Country).ThenBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name)
                .Select(Mapper.Map<CraftDto>).ToList();

            PrevId = IBQuery1.Last().CraftId;
            PrevName = GetCraftName(IBQuery1.Last());

            FirstId = IBQuery1.First().CraftId;
            FirstName = GetCraftName(IBQuery1.First());
            lAlso = new List<Pair<int>>();
            lAlsoYears = new List<string>();

            var IBQuery2 = (
                from p in _ctx.Pics
                join a in _ctx.Arts on p.ArtId equals a.ArtId
                where !Const.Arts.Copyrighted.Contains(a.ArtId) && !(p.Copyright ?? false)
                orderby p.CraftId, p.NType, p.NNN
                select new { p, a });
            //'Select a.NN NN, p.*, a.* From Pics p Join Arts a on p.ArtId = a.ArtId Order By' +
            //'  CraftId, NType, NNN';

            var craftIdStr = "";

            for (var ibq1 = 0; ibq1 < IBQuery1.Count; ibq1++) {
                var ibq1c = IBQuery1[ibq1];

                if (ibq1 == IBQuery1.Count - 1) {
                    NextId = FirstId;
                    NextName = FirstName;
                } else {
                    NextId = IBQuery1[ibq1 + 1].CraftId;
                    NextName = GetCraftName(IBQuery1[ibq1 + 1]);
                }

                lAlso.Clear();
                lAlsoYears.Clear();

                if (ibq1c.SeeAlso.HasValue) {
                    var ibq = ibq1c;
                    while (ibq.SeeAlso.HasValue) {

                        ibq = IBQuery1.First(x => x.CraftId == ibq.SeeAlso);
                        AlsoId = ibq.CraftId;
                        AlsoName = GetCraftName(ibq);
                        if (lAlso.Any(x => x.Id == AlsoId)) break;
                        var ind = lAlsoYears.InsertStringSorted(ibq.IYear.ToString() + ibq.Construct + ibq.Name);
                        lAlso.Insert(ind, new Pair<int>() { Name = AlsoName, Id = AlsoId });
                    }
                }

                for (ii = 1; ii < 7; ii++) {
                    cnts[ii] = 0;
                    picids[ii] = "";
                    if (ii < 5) plans[ii] = false;
                }

                pics = IBQuery2.Where(x => x.p.CraftId == ibq1c.CraftId).ToList()
                    .Select(x => new PicArt()
                    {
                        p = Mapper.Map<PicDto>(x.p),
                        a = Mapper.Map<ArtDto>(x.a)
                    }).ToList();
                if (pics.Any()) {
                    foreach (var pic in pics) {
                        s = pic.p.Type?.Trim() ?? "";
                        switch (s) {
                            case "s":
                                i = 1;
                                break;
                            case "fc":
                                i = 2;
                                break;
                            case "f":
                                i = 3;
                                break;
                            case "c":
                                i = 4;
                                break;
                            case "fd":
                            case "fr":
                                i = 5;
                                break;
                            default:
                                i = 6;
                                switch (s) {
                                    case "m":
                                        plans[1] = true;
                                        break;
                                    case "dc":
                                    case "d":
                                        plans[2] = true;
                                        break;
                                    case "p":
                                        plans[3] = true;
                                        break;
                                    default:
                                        plans[4] = true;
                                        break;
                                }
                                break;
                        }
                        cnts[i]++;
                        if (picids[i] == "") {
                            picids[i] = pic.p.Path;
                        }
                    }

                    craftIdStr += ibq1c.CraftId.ToString("X6");
                }

                cnt = 0;
                for (i = 1; i <= 6; i++) {
                    cnt += cnts[i];
                }
                if (cnt < 100) {
                    mains = 0;
                    LoadPic(0, ibq1c);
                } else {
                    mains = 2;
                    if (cnts[2] == 0) {
                        mains = 3;
                        if (cnts[3] == 0) {
                            mains = 1;
                            if (cnts[1] == 0) {
                                mains = 4;
                                if (cnts[4] == 0) {
                                    mains = 5;
                                    if (cnts[5] == 0) {
                                        mains = 6;
                                    }
                                }
                            }
                        }
                    }

                    for (i = 1; i <= 6; i++)
                    {
                        if (cnts[i] > 0) LoadPic(i, ibq1c);
                    }
                }

                PrevId = ibq1c.CraftId;
                PrevName = GetCraftName(ibq1c);

                lInfo.Text = $"Craft {ibq1c.IYear} {ibq1c.Construct}";
                Application.DoEvents();
                if (is_stop) { break; };

            }


            File.WriteAllText(sPath + "Upload\\crafts.dat", craftIdStr);

            lInfo.Text = "Crafts...";
            Application.DoEvents();
            if (is_stop) { return; };

            var sames = _ctx.Crafts.Where(x => x.Same.HasValue && x.Source == "6").ToList();
            foreach (var sameCraft in sames) {
                var iEl = lBicPics.BinaryIndexOfPairById(sameCraft.Same.Value);
                if (iEl < 0)
                {
                    MessageBox.Show($"Same not found {sameCraft.CraftId} {sameCraft.Construct} {sameCraft.Name}");
                }
                else
                {
                    var same = lBicPics[iEl];
                    lBicPics.InsertPairSortedById(new Pair<int>()
                    {
                        Name = same.Name,
                        Id = sameCraft.CraftId
                    });
                    var iMl = lMagPics.BinaryIndexOfPairById(sameCraft.Same.Value);
                    same = lMagPics[iMl];
                    lMagPics.InsertPairSortedById(new Pair<int>()
                    {
                        Name = same.Name,
                        Id = sameCraft.CraftId
                    });
                }
            }

            lInfo.Text = "Crafts...!";
            Application.DoEvents();
            if (is_stop) { return; };

            var craftQry = (
                from c in _ctx.Crafts
                from p in _ctx.Pics.Where(p => c.CraftId == p.CraftId && !Const.Arts.Copyrighted.Contains(p.ArtId) && !(p.Copyright ?? false)).DefaultIfEmpty()
                group c by c into g
                select new { c = g.Key, cnt = g.Count() });

            var IBQuery1Crft = (
                from c in craftQry
                from cc in craftQry.Where(cc => c.c.Same == cc.c.CraftId).DefaultIfEmpty()
                from ci in _ctx.Countries.Where(ci => c.c.Country == ci.Rus).DefaultIfEmpty()
                where c.c.Source == "6"
                select new { c, cc, ci }).ToList()
                .Select(x => new CraftList()
                {
                    c = Mapper.Map<CraftDto>(x.c.c),
                    cc = x.cc == null ? null : Mapper.Map<CraftDto>(x.cc.c),
                    ci = Mapper.Map<CountryDto>(x.ci),
                    cCnt = x.c.cnt + (x.cc == null ? 0 : x.cc.cnt)
                }).ToList();

            var crftList = IBQuery1Crft.OrderBy(x => x.c.Country).ThenBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts1.htm", true, 0, 1, "Country", 1);
            for (i = 1; i <= chunks[1].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts1.htm", true, i, 1, "Country", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.ci.Eng).ThenBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts1En.htm", true, 0, 10, "Eng", 1);
            for (i = 1; i <= chunks[10].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts1En.htm", true, i, 10, "Eng", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts2.htm", true, 0, 2, "Construct", 1);
            for (i = 1; i <= chunks[2].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts2.htm", true, i, 2, "Construct", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.c.Name).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts3.htm", true, 0, 3, "Name", 2);
            for (i = 1; i <= chunks[3].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts3.htm", true, i, 3, "Name", 2);

            crftList = IBQuery1Crft.OrderBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts4.htm", true, 0, 4, "IYear", 1);
            for (i = 1; i <= chunks[4].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts4.htm", true, i, 4, "IYear", 1);

            crftList = IBQuery1Crft.OrderByDescending(x => x.cCnt).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts5.htm", true, 0, 5, "Photos", 0);
            for (i = 1; i <= 10; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts5.htm", true, i, 5, "Photos", 0);

            crftList = IBQuery1Crft.OrderByDescending(x => x.c.CText?.Length).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts6.htm", true, 0, 6, "TextLen", 0);
            for (i = 1; i <= 10; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts6.htm", true, i, 6, "TextLen", 0);

            crftList = IBQuery1Crft.OrderBy(x => x.c.Wings).ThenBy(x => x.c.Engines).ThenBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts7.htm", true, 0, 7, "Wings", 1);
            for (i = 1; i <= chunks[7].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts7.htm", true, i, 7, "Wings", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.c.Engines).ThenBy(x => x.c.Wings).ThenBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage(crftList, "Crafts.htm", "Crafts8.htm", true, 0, 8, "Engines", 1);
            for (i = 1; i <= chunks[8].Count; i++)
                CreatePage(crftList, "Crafts.htm", "Crafts8.htm", true, i, 8, "Engines", 1);


            lInfo.Text = "Arts...";
            Application.DoEvents();
            if (is_stop) { return; };


            var artQry = _ctx.vwArts.Where(x => x.Source == "6");

            var IBQuery1CrftX = (
                from a in _ctx.vwArts
                    //join m in _ctx.Mags on a.Mag equals m.Id
                where a.Source == "6"
                select new { a }).ToList();
            IBQuery1Crft = IBQuery1CrftX
                .Select(x => new CraftList()
                {
                    a = Mapper.Map<ArtDto>(x.a),
                    m = Mapper.Map<MagDto>(_ctx.Mags.FirstOrDefault(m => x.a.Mag.Trim() == m.Id.Trim())),//Mapper.Map<MagDto>(x.m),
                    aCnt = x.a.cnt ?? 0
                }).ToList();

            crftList = IBQuery1Crft.OrderBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.m?.Template).ThenBy(x => x.a.Author).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage(crftList, "Arts.htm", "Arts1.htm", false, 0, 11, "IYear", 1);
            for (i = 1; i <= chunks[11].Count; i++)
                CreatePage(crftList, "Arts.htm", "Arts1.htm", false, i, 11, "IYear", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.m?.Template).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Author).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage(crftList, "Arts.htm", "Arts2.htm", false, 0, 12, "Template", 1);
            for (i = 1; i <= chunks[12].Count; i++)
                CreatePage(crftList, "Arts.htm", "Arts2.htm", false, i, 12, "Template", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.a.Author).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.m?.Template).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage(crftList, "Arts.htm", "Arts3.htm", false, 0, 13, "Author", 1);
            for (i = 1; i <= chunks[13].Count; i++)
                CreatePage(crftList, "Arts.htm", "Arts3.htm", false, i, 13, "Author", 1);

            crftList = IBQuery1Crft.OrderBy(x => x.a.Name).ThenBy(x => x.a.Author).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.m?.Template).ThenBy(x => x.a.NN).ToList();
            CreatePage(crftList, "Arts.htm", "Arts4.htm", false, 0, 14, "Name", 0);
            for (i = 1; i <= 10; i++)
                CreatePage(crftList, "Arts.htm", "Arts4.htm", false, i, 14, "Name", 0);

            crftList = IBQuery1Crft.OrderBy(x => x.a.Serie).ThenBy(x => x.a.NN).ThenBy(x => x.a.Name).ThenBy(x => x.a.Author).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.m?.Template).ToList();
            CreatePage(crftList, "Arts.htm", "Arts5.htm", false, 0, 15, "Serie", 1);
            for (i = 1; i <= chunks[15].Count; i++)
                CreatePage(crftList, "Arts.htm", "Arts5.htm", false, i, 15, "Serie", 1);

            crftList = IBQuery1Crft.OrderByDescending(x => x.aCnt).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.m?.Template).ThenBy(x => x.a.Author).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage(crftList, "Arts.htm", "Arts6.htm", false, 0, 16, "Photos", 0);
            for (i = 1; i <= 10; i++)
                CreatePage(crftList, "Arts.htm", "Arts6.htm", false, i, 16, "Photos", 0);


            var ibq1A = artQry
                .OrderBy(x => x.IYear).ThenBy(x => x.IMonth).ThenBy(x => x.Mag).ThenBy(x => x.Author).ThenBy(x => x.Name).ThenBy(x => x.NN)
                .ToList()
                .Select(x => Mapper.Map<ArtDto>(x)).ToList();
            var ibq2A = (
                from p in _ctx.Pics
                join c in _ctx.Crafts on p.CraftId equals c.CraftId
                where !Const.Arts.Copyrighted.Contains(p.ArtId) && !(p.Copyright ?? false)
                orderby p.ArtId, p.CraftId, p.NType, p.NN
                select new { p, c });

            var sArt = "";
            var lArts = new List<Pair<int>>();
            var maxArt = 0; var lastArt = 0;
            for (var ibq1AaCnt = 0; ibq1AaCnt < ibq1A.Count; ibq1AaCnt++) {
                var ibq1Aa = ibq1A[ibq1AaCnt];

                if (sArt != GetArtName(ibq1Aa)) {
                    sArt = GetArtName(ibq1Aa);
                    lArts.Clear();

                    var ibq1AaCnt2 = ibq1AaCnt;
                    var ibq1Aa2 = ibq1A[ibq1AaCnt2];
                    while ((ibq1AaCnt2 < ibq1A.Count) && (sArt == GetArtName(ibq1A[ibq1AaCnt2]))) {
                        ibq1Aa2 = ibq1A[ibq1AaCnt2];
                        s = GetArtName2(ibq1Aa2) ?? "";
                        if (s.Trim() == "") s = "-";
                        lArts.Add(new Pair<int>() { Name = s, Id = ibq1Aa2.ArtId });

                        ibq1AaCnt2++;
                    }

                }
                if (maxArt < ibq1Aa.ArtId) {
                    maxArt = ibq1Aa.ArtId;
                    lastArt = lArts[0].Id;
                }

                templ = File.ReadAllText(sPath + "Site\\Art.htm");
                while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");

                i = templ.IndexOf("%begin%");
                sBeg = templ.Substring(0, i);
                sEnd = templ.Substring(i + 7);
                i = sEnd.IndexOf("%end%");
                sMid = sEnd.Substring(0, i);
                sEnd = sEnd.Substring(i + 5);

                var ibq2AP = ibq2A.Where(x => x.p.ArtId == ibq1Aa.ArtId).ToList()
                    .Select(x => new PicArt()
                    {
                        p = Mapper.Map<PicDto>(x.p),
                        c = Mapper.Map<CraftDto>(x.c)
                    }).ToList();

                if (ibq2AP.Any()) {
                    foreach (var ibq2APp in ibq2AP) {
                        ss = new StringBuilder(ibq2APp.p.Text ?? "")
                            .Replace("\n\r ", "<br>&nbsp;&nbsp;")
                            .Replace("\n ", "<br>&nbsp;&nbsp;")
                            .Replace("\n\r", "<br>")
                            .Replace("\n", "<br>")
                            .ToString();

                        var sx = new StringBuilder(sMid)
                            .Replace("%PicPath%", ibq2APp.p.Path.Replace("\\", "/"))
                            .Replace("%CraftLink%", $"../Crafts/Craft{ibq2APp.p.CraftId}.htm")
                            .Replace("%CraftName%",
                                ibq2APp.c.Construct + " " +
                                ibq2APp.c.Name + " - " +
                                GetSpanCountry(ibq2APp.c.Country) + " - " +
                                ibq2APp.c.IYear.ToString())
                            .Replace("%PicText%", ss)
                            .ToString();

                        sBeg += sx;

                    }
                } else {
                    sBeg += "<tr><td>" + GetSpanRuEn("Нет фотографий", "No photos") + "</td></tr>";
                }

                templ = sBeg + sEnd;

                templ = templ.Replace("%Art%", GetArtName(ibq1Aa));

                i = templ.IndexOf("%xbegin%");
                sBeg = templ.Substring(0, i);
                sEnd = templ.Substring(i + 8);
                i = sEnd.IndexOf("%xend%");
                sMid = sEnd.Substring(0, i);
                sEnd = sEnd.Substring(i + 6);
                i = sMid.IndexOf("%xmid%");
                var sMid2 = sMid.Substring(i + 6);
                sMid = sMid.Substring(0, i);

                for (i = 0; i < lArts.Count; i++) {
                    if (ibq1Aa.ArtId == lArts[i].Id)
                        sx = sMid;
                    else
                        sx = sMid2;
                    sx = new StringBuilder(sx)
                        .Replace("%ArtName%", lArts[i].Name)
                        .Replace("%ArtLink%", $"Art{lArts[i].Id}.htm")
                        .ToString();
                    sBeg += sx;
                }

                templ = sBeg + sEnd;

                templ = templ.Replace("%ArtName%", GetArtName2(ibq1Aa));

                s = sPath + $"Site\\Arts\\Art{ibq1Aa.ArtId}.htm";
                ss = sPath + $"Upload\\Site\\Arts\\Art{ibq1Aa.ArtId}.htm";
                var is_upl = false;
                if (File.Exists(s)) {
                    upl = File.ReadAllText(s);
                    if (templ != upl) {
                        File.WriteAllText(ss, templ, Encoding.UTF8);
                        is_upl = true;
                    }
                } else {
                    File.WriteAllText(ss, templ, Encoding.UTF8);
                    is_upl = true;
                }
                if (is_upl)
                {
                    File.WriteAllText(s, templ, Encoding.UTF8);
                }

                AddSitemap($"Site\\Arts\\Art{ibq1Aa.ArtId}.htm", s, is_upl);

                lInfo.Text = $"Art {ibq1Aa.IYear} {ibq1Aa.IMonth.Trim()} {ibq1Aa.Mag.Trim()}";
                Application.DoEvents();
                if (is_stop) { return; };

            }

            templ = File.ReadAllText(sPath + "Site\\index.htm");
            while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");

            i = templ.IndexOf("%begin%");
            sBeg = templ.Substring(0, i);
            sEnd = templ.Substring(i + 7);
            i = sEnd.IndexOf("%end%");
            sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 5);

            cnt = 0;
            var uplA = File.ReadAllLines(sPath + "History.ini");
            var uplL = new List<string>(uplA);
            i = 0;
            do {
                s = uplL[i];
                if (s[0] == ' ') {
                    s = uplL[i].Substring(1);
                    ii = s.IndexOf(" ");
                    cnt++;
                    s = s.Substring(ii + 1);
                    ii = s.IndexOf(" ");
                    ss = s.Substring(0, ii);
                    s = s.Substring(ii + 1);

                    ii = GetArtId(s);

                    sBeg += new StringBuilder(sMid)
                        .Replace("%HistDate%", ss)
                        .Replace("%HistName%", $"<a class=\"en_href\" href=\"Site\\Arts\\Art{ii}.htm\">{s}</a>")
                        .ToString();
                }
                i++;
            } while (cnt <= 50);

            templ = sBeg + sEnd;

            uplA = File.ReadAllLines(sPath + "counts.txt");
            uplL = new List<string>(uplA);
            cnt = int.Parse(uplL[3]);
            var RecordCount = artQry.Select(a => new { a.Mag, a.IYear, a.IMonth }).Distinct().Count();
            if (RecordCount > cnt) {
                uplL[7] = (RecordCount - cnt).ToString();
                uplL[3] = RecordCount.ToString();

                var newCnt = artQry.Count();
                cnt = int.Parse(uplL[2]);
                uplL[6] = (newCnt - cnt).ToString();
                uplL[2] = newCnt.ToString();

                newCnt = _ctx.Crafts.Where(x => x.Source == "6" && x.Same == null).Count();
                cnt = int.Parse(uplL[1]);
                uplL[5] = (newCnt - cnt).ToString();
                uplL[1] = newCnt.ToString();

                newCnt = (
                    from p in _ctx.Pics
                    join c in _ctx.Crafts on p.CraftId equals c.CraftId
                    where c.Source == "6"
                    select p.Path).Distinct().Count();
                cnt = int.Parse(uplL[0]);
                uplL[4] = (newCnt - cnt).ToString();
                uplL[0] = newCnt.ToString();

                File.WriteAllLines(sPath + "counts.txt", uplL.ToArray());
            }

            templ = new StringBuilder(templ)
                .Replace("%LastArtLink%", $"Site\\Arts\\Art{lastArt}.htm")
                .Replace("%PicCnt%", uplL[0])
                .Replace("%CraftCnt%", uplL[1])
                .Replace("%ArtCnt%", uplL[2])
                .Replace("%MagCnt%", uplL[3])
                .Replace("%PicCntAdd%", uplL[4])
                .Replace("%CraftCntAdd%", uplL[5])
                .Replace("%ArtCntAdd%", uplL[6])
                .Replace("%MagCntAdd%", uplL[7])
                .ToString();

            File.WriteAllText(sPath + "Upload\\index.htm", templ, Encoding.UTF8);
            AddSitemap("index.htm", "", true);


            sitemap.Add("</urlset>");
            File.WriteAllLines(sPath + "Upload\\Sitemap.xml", sitemap.ToArray(), Encoding.UTF8);

            MessageBox.Show("OK!");
        }

        private string GetStringDate(string s)
        {
            var mnthes = new string[13]
                    {"", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            var dd = s.Substring(0, 2);
            var mm = s.Substring(3, 2);
            var yy = s.Substring(6, 4);
            var last_char = dd[1];
            var abbrev = "th";
            if (dd[0] == '1')
                abbrev = "th";
            else if (last_char == '1')
                abbrev = "st";
            else if (last_char == '2')
                abbrev = "nd";
            else if (last_char == '3')
                abbrev = "rd";

            if (dd[0] == '0')
                dd = dd.Substring(1);

            return $"{mnthes[int.Parse(mm)]} {dd}{abbrev}, {int.Parse(yy) - 120}.";
        }


        void FindArtCraft(string artName, int artId, int craftId, out ArtData art, out CraftData crft)
        {
            try
            {
                if (!ArtTexts.ContainsKey(artId))
                {
                    ArtTexts[artId] = new ArtData(artId);
                }
                art = ArtTexts[artId];
                if (!art.Crafts.ContainsKey(craftId))
                {
                    art.Crafts[craftId] = new CraftData(craftId);
                }
                crft = art.Crafts[craftId];
            }
            catch 
            {
                art = null;
                crft = null;
                MessageBox.Show($"Art: {artName}");
            }
        }

        string GetFlightName(string line)
        {
            for(var i = line.Length - 1; i > 0; i--)
            {
                if (Char.IsDigit(line[i]))
                {
                    return $"Flight, {line.Substring(i - 3, 4)}";
                }
            }
            return null;
        }

        void LoadPic7(CraftDto craft1, List<PicArt> pics)
        {
            var lLog = new List<string>();

            int j, k;
            string st;

            var bigpic = ""; var bigpictype = "";

            templ = File.ReadAllText(sPath + "Site2\\Craft.htm");
            while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");
            templ = new StringBuilder(templ)
                .Replace("%Country%", craft1.Country)
                .Replace("%Construct%", craft1.Construct)
                .Replace("%Name%", craft1.Name)
                .Replace("%Year%", craft1.IYear.ToString())
                .Replace("%Vert%", craft1.Vert ?? false ? "Вертолет" : "")
                .Replace("%UAV%", craft1.Uav ?? false ? "Беспилотный" : "")
                .Replace("%Glider%", craft1.Glider ?? false ? "Планер" : "")
                .Replace("%Single%", craft1.Single ?? false ? "Единственный экземпляр" : "")
                .Replace("%LL%", craft1.LL ?? false ? "Летающая лодка" : "")
                .Replace("%Proj%", craft1.Proj ?? false ? " (проект)" : "")
                .Replace("%Type%", craft1.Type)
                .Replace("%PrevName%", PrevName)
                .Replace("%PrevLink%", $"Craft{PrevId}.htm")
                .Replace("%NextName%", NextName)
                .Replace("%NextLink%", $"Craft{NextId}.htm")
                .Replace("%ID%", craft1.CraftId.ToString())
                .ToString();

            var i = templ.IndexOf("%TextBegin%");
            var sBeg = templ.Substring(0, i);
            var sEnd = templ.Substring(i + 11);
            i = sEnd.IndexOf("%TextEnd%");
            var sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 9);
            ss = craft1.CText?.Trim() ?? "";
            var sxx = ""; var tabcnt = 0; var syy = "";
            string sx;
            CraftData crft = null;
            ArtData art = null;
            if (!string.IsNullOrEmpty(ss))
            {
                ss = new StringBuilder(ss)
                    .Replace("\n\r ", "<br>&nbsp;&nbsp;")
                    .Replace("\n ", "<br>&nbsp;&nbsp;")
                    .Replace("\n\r", "<br>")
                    .Replace("\n", "<br>")
                    .Replace("<br><br><br><br>", "<br><br><br>")
                    .ToString();
                sx = "???";
                j = 0;
                i = ss.IndexOf("/*");
                if (i < 0) {
                    if (ss.Trim().StartsWith("Flight,")) {
                        sx = "Flight";
                        j = 5748;
                    } else {
                        var IBQuery3 = _ctx.Pics.Where(p => p.CraftId == craft1.CraftId).Select(p => p.ArtId).Distinct().ToList();
                        if (!IBQuery3.Any()) {
                            MessageBox.Show($"EOF: {craft1.Construct} {craft1.Name}");
                            lLog.Add($"EOF: {craft1.Construct} {craft1.Name}");
                        } else {
                            j = IBQuery3[0];
                            for (k = 1; k < IBQuery3.Count; k++) {
                                /*if ((j == 4600) || (IBQuery3[k] == 4600)) {
                                    if (j == 4600 && IBQuery3[k] != 4600) 
                                        j = IBQuery3[k];
                                } else if (CheckFlight(j) && CheckFlight(IBQuery3[k])) {
                                    // Flights
                                    j = 5748;
                                } else*/ {
                                    j = 0;
                                    sx = "???";
                                    MessageBox.Show($"Too much: {craft1.Construct} {craft1.Name}");
                                    lLog.Add($"Too much: {craft1.Construct} {craft1.Name}");
                                    break;
                                };
                            };
                        };
                    };
                    if (j > 0) {
                        k = lSourceIds.IndexOf(j.ToString());
                        if (k >= 0) {
                            sx = lSourceNames[k];
                            if (!int.TryParse(lSourceIds[k], out var ind))
                            {
                                MessageBox.Show($"Unknown source: {sx}");
                            }
                            else
                            {
                                FindArtCraft(sx, ind, craft1.CraftId, out art, out crft);
                            }
                        } else {
                            k = lSources.InsertStringSorted("???");
                            lSourceNames.Insert(k, "???");
                            lSourceIds.Insert(k, j.ToString());
                        };
                    };
                };
                if (sx == "???")
                    sx = craft1.Name;
                sxx = "";
                do {
                    if (i >= 0) {
                        j = ss.IndexOf("{");
                        if ((j > i) || (j < 0)) {
                            sx = craft1.Name;
                        } else {
                            k = ss.IndexOf("}");
                            sx = ss.Substring(j + 1, k - j - 1).Trim();
                            ss = (j > 0 ? ss.Substring(0, j) : "") + ss.Substring(k + 1);
                            k = lSources.IndexOf(sx);
                            if (k >= 0) {
                                sx = lSourceNames[k];
                                if (!int.TryParse(lSourceIds[k], out var ind))
                                {
                                    MessageBox.Show($"Unknown source: {sx}");
                                }
                                else
                                {
                                    FindArtCraft(sx, ind, craft1.CraftId, out art, out crft);
                                }
                            }
                            else {
                                k = lSources.InsertStringSorted(sx);
                                lSourceNames.Insert(k, sx);
                                lSourceIds.Insert(k, "");
                            };

                            i = ss.IndexOf("/*");
                        };
                        st = ss.Substring(0, i).Trim();
                        if (!string.IsNullOrEmpty(st)) {
                            st = new StringBuilder(st)
                                .Replace("<br>", "")
                                .Replace("&nbsp;", "")
                                .ToString().Trim();
                            if (!string.IsNullOrEmpty(st)) {
                                MessageBox.Show($"{craft1.Construct} {craft1.Name}\n{st}");
                                lLog.Add($"{craft1.Construct} {craft1.Name}\n{st}");
                            };
                        };
                        ss = ss.Substring(i + 2);
                        syy += $"<li><a href='#tabs-{tabcnt}'>{sx}</a></li>\n";
                        i = ss.IndexOf("*/");
                        if (i < 0)
                        {
                            MessageBox.Show($"{craft1.Construct} {craft1.Name}\n*/");
                            lLog.Add($"{craft1.Construct} {craft1.Name}\n*/");
                            st = "";
                        }
                        else
                        {
                            st = ss.Substring(0, i);
                        }
                        if (st.IndexOf("/*") >= 0) {
                            MessageBox.Show($"{craft1.Construct} {craft1.Name}\n{st}");
                            lLog.Add($"{craft1.Construct} {craft1.Name}\n{st}");
                        };
                        sxx +=
                            $"<div id='tabs-{tabcnt}'><span class='hide_me'><h3><br>{sx}</h3><br></span>";
                        if ((sx.IndexOf("M.Goodall, A.Tagg") >= 0) || (sx.IndexOf("L.Opdyke") >= 0))
                            st = $"<div class='cpc'>Deleted by request of (c)Schiffer Publishing</div><div class='cpcc' style='display:none'>{st}</div>";
                        sxx += st + "\n</div>\n";

                        if (sx == craft1.Name) MessageBox.Show($"CraftName {sx}");
                        if (CheckFlight(art?.ArtId ?? 0) )
                        {
                            var lines = st.Split(new [] { "<br>" }, StringSplitOptions.None);
                            var newLines = new List<string>();
                            var kk = -1;
                            var artName = "";
                            foreach (var line in lines)
                            {
                                if (line.StartsWith("Flight, "))
                                {
                                    var newArtName = GetFlightName(line);
                                    if (artName != newArtName) {
                                        if (artName != "")
                                        {
                                            if (kk >= 0)
                                            {
                                                FindArtCraft(lSourceNames[kk], int.Parse(lSourceIds[kk]), craft1.CraftId, out art, out crft);
                                                crft.CraftText = string.Join("<br>", newLines);
                                            }
                                            else
                                            {
                                                MessageBox.Show($"Flight: {artName}");
                                            }
                                        }
                                        artName = newArtName;
                                        kk = lSources.IndexOf(artName);
                                        newLines.Clear();
                                    }
                                }
                                newLines.Add(line);
                            }
                            if (kk >= 0)
                            {
                                FindArtCraft(lSourceNames[kk], int.Parse(lSourceIds[kk]), craft1.CraftId, out art, out crft);
                                crft.CraftText = string.Join("<br>", newLines);
                            }
                            else
                            {
                                MessageBox.Show($"Flight: {artName}");
                            }
                        }
                        else if (crft != null && !string.IsNullOrEmpty(st)) 
                            crft.CraftText = st;

                        ss = ss.Substring(i + 2);
                    } else {
                        syy += $"<li><a href='#tabs-{tabcnt}'>{sx}</a></li>\n";
                        sxx +=
                            $"<div id='tabs-{tabcnt}'><span class='hide_me'><h3><br>{sx}</h3><br></span>";
                        if ((sx.IndexOf("M.Goodall, A.Tagg") >= 0) || (sx.IndexOf("L.Opdyke") >= 0)) 
                            ss = $"<div class='cpc'>Deleted by request of (c)Schiffer Publishing</div><div class='cpcc' style='display:none'>{ss}</div>";
                        sxx += ss + "\n</div>\n";

                        if (sx == craft1.Name) MessageBox.Show($"CraftName {sx}");
                        if (CheckFlight(art?.ArtId ?? 0))
                        {
                            var lines = ss.Split(new[] { "<br>" }, StringSplitOptions.None);
                            var newLines = new List<string>();
                            var kk = -1;
                            var artName = "";
                            foreach (var line in lines)
                            {
                                if (line.StartsWith("Flight, "))
                                {
                                    var newArtName = GetFlightName(line);
                                    if (artName != "" && artName != newArtName)
                                    {
                                        if (kk >= 0)
                                        {
                                            FindArtCraft(lSourceNames[kk], int.Parse(lSourceIds[kk]), craft1.CraftId, out art, out crft);
                                            crft.CraftText = string.Join("<br>", newLines);
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Flight: {artName}");
                                        }
                                    }
                                    artName = newArtName;
                                    kk = lSources.IndexOf(artName);
                                    newLines.Clear();
                                }
                                newLines.Add(line);
                            }
                            if (kk >= 0)
                            {
                                FindArtCraft(lSourceNames[kk], int.Parse(lSourceIds[kk]), craft1.CraftId, out art, out crft);
                                crft.CraftText = string.Join("<br>", newLines);
                            }
                            else
                            {
                                MessageBox.Show($"Flight: {artName}");
                            }
                        } else if (crft != null && !string.IsNullOrEmpty(ss)) 
                            crft.CraftText = ss;

                        ss = "";
                    };
                    tabcnt++;

                    i = ss.IndexOf("/*");
                    crft = null;
                } while (i >= 0);

                ss = new StringBuilder(ss)
                    .Replace("<br>", "")
                    .Replace("&nbsp;", "")
                    .ToString()
                    .Trim();
                if (!string.IsNullOrEmpty(ss)) {
                    MessageBox.Show($"{craft1.Construct} {craft1.Name}\n{ss}");
                    lLog.Add($"{craft1.Construct} {craft1.Name}\n{ss}");
                };

                ss = $"<ul>\n{syy}</ul>\n{sxx}";
                sBeg += sMid.Replace("%Text%", ss);
            };
            templ = sBeg + sEnd;

            i = templ.IndexOf("%begin%");
            sBeg = templ.Substring(0, i - 1);
            sEnd = templ.Substring(i + 7);
            i = sEnd.IndexOf("%end%");
            sMid = sEnd.Substring(0, i - 1);
            sEnd = sEnd.Substring(i + 5);

            if (pics.Any())
            {
                foreach (var pic in pics)
                {

                    FindArtCraft("Pics", pic.p.ArtId, pic.p.CraftId, out art, out crft);

                    if (bigpic == "") {
                        bigpic = pic.p.Path.Replace("\\", "/");
                        bigpictype = pic.p.Type;
                    } else if ((bigpictype == "s") && (pic.p.Type != "s")) {
                        bigpic = pic.p.Path.Replace("\\", "/");
                        bigpictype = pic.p.Type;
                    };

                    ss = pic.p.Type;

                    sx = sMid;
                    var spic = pic.p.Path.Replace("\\", "/");
                    var spicX = spic;
                    if ((spic.IndexOf("/GT-0/") >= 0) || (spic.IndexOf("/Opd-0/") >= 0))
                        spicX = $"copyright.jpg\" class='cp_i' cpi='../../Images7/{spic}'";
                    ss = new StringBuilder(pic.p.Text)
                        .Replace("\n\r ", "<br>&nbsp;&nbsp;")
                        .Replace("\n ", "<br>&nbsp;&nbsp;")
                        .Replace("\n\r", "<br>")
                        .Replace("\n", "<br>")
                        .ToString();
                    sx = new StringBuilder(sx)
                        .Replace("%PicPath%", spic)
                        .Replace("%PicPathX%", spicX)
                        .Replace("%ArtLink%", $"../Arts/Art{pic.a.ArtId}.htm")
                        .Replace("%Art%", GetArtName(pic.a, true, true))
                        .Replace("%PicText%", ss)
                        .ToString();

                    sBeg += sx;

                };
            } else {
                sBeg += "<tr><td>Нет фотографий</td></tr>";
                bigpic = "";
            };

            if ((bigpic.IndexOf("/GT-0/") >= 0) || (bigpic.IndexOf("/Opd-0/") >= 0))
                bigpic = $"copyright.jpg\" class='cp_ii' cpi='../../Images7/{bigpic}'";
            templ = (sBeg + sEnd).Replace("%BigPicPath%", bigpic);

            s = $"{sPath}Site2\\Crafts\\Craft{craft1.CraftId}.htm";
            ss = $"{sPath}Upload2\\Site2\\Crafts\\Craft{craft1.CraftId}.htm";
            if (File.Exists(s)) {
                upl = File.ReadAllText(s);
                if (templ != upl)
                    File.WriteAllText(ss, templ, Encoding.UTF8);
            }
            else {
                File.WriteAllText(ss, templ, Encoding.UTF8);
            };
            File.WriteAllText(s, templ, Encoding.UTF8);

        }


        private void SaveToFile(List<string> list, string file)
        {
            var tw = new StreamWriter(file);

            foreach (var s in list)
                tw.WriteLine(s);

            tw.Close();
        }


        private string CheckNull(string s)
        {
            return string.IsNullOrEmpty(s) ? "&nbsp;" : s;
        }


        public void PrepareWeb7(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            if (!(MessageBox.Show("Prepare Web7 ?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;

            is_stop = false;
            _ctx = ctx;
            _lInfo = lInfo;

            int i;
            string sBeg, sEnd, sMid;

            ArtTexts = new Dictionary<int, ArtData>();

            sPath = _imagesPath + "AviaDejaVu\\";

            Directory.CreateDirectory(sPath + "Upload2\\Site2");
            Directory.CreateDirectory(sPath + "Upload2\\Site2\\Arts");
            Directory.CreateDirectory(sPath + "Upload2\\Site2\\Crafts");

            var sPathi = sPath + "..\\Images7";
            var ssPath = sPathi + "m";
            var dirs = Directory.GetDirectories(sPathi, "*.");
            foreach (var dir in dirs)
            {
                var srx = Path.GetFileName(dir);
                LoadDir7(sPathi + "\\" + srx, ssPath + "\\" + srx);
                if (is_stop) { return; }
            }


            elements = new List<Pair<int>>[13];
            chunks = new List<TChunk>[13];
            for (i = 1; i < 13; i++) {
                elements[i] = new List<Pair<int>>();
                chunks[i] = new List<TChunk>();
            }

            lInfo.Text = "Crafts...";
            Application.DoEvents();
            if (is_stop) { return; };


            var craftQry = (
                from c in _ctx.Crafts
                join p in _ctx.Pics on c.CraftId equals p.CraftId
                where !Const.Arts.Copyrighted.Contains(p.ArtId) && !(p.Copyright ?? false)
                group c by c.CraftId into g
                select new { cid = g.Key, cnt = g.Count() });


            var IBQuery1Crft = (
                from c in _ctx.Crafts
                from ci in _ctx.Countries.Where(ci => c.Country == ci.Rus).DefaultIfEmpty()
                from cq in craftQry.Where(cq => c.CraftId == cq.cid).DefaultIfEmpty()
                where c.Source == "7"
                select new { c, ci, cq }).ToList()
                .Select(x => new CraftList()
                {
                    c = Mapper.Map<CraftDto>(x.c),
                    ci = Mapper.Map<CountryDto>(x.ci),
                    cCnt = x.cq?.cnt ?? 0
                }).ToList();

            var crftList = IBQuery1Crft.OrderBy(x => x.c.Country).ThenBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage7(crftList, "Crafts.htm", "Crafts1.htm", true, 0, 1, "Country", 2);
            for (i = 1; i <= chunks[1].Count; i++)
                CreatePage7(crftList, "Crafts.htm", "Crafts1.htm", true, i, 1, "Country", 2);

            crftList = IBQuery1Crft.OrderBy(x => x.c.Construct).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Name).ToList();
            CreatePage7(crftList, "Crafts.htm", "Crafts2.htm", true, 0, 2, "Construct", 2);
            for (i = 1; i <= chunks[2].Count; i++)
                CreatePage7(crftList, "Crafts.htm", "Crafts2.htm", true, i, 2, "Construct", 2);

            crftList = IBQuery1Crft.OrderBy(x => x.c.Name).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ToList();
            CreatePage7(crftList, "Crafts.htm", "Crafts3.htm", true, 0, 3, "Name", 2);
            for (i = 1; i <= chunks[3].Count; i++)
                CreatePage7(crftList, "Crafts.htm", "Crafts3.htm", true, i, 3, "Name", 2);

            crftList = IBQuery1Crft.OrderBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ThenBy(x => x.c.Name).ToList();
            CreatePage7(crftList, "Crafts.htm", "Crafts4.htm", true, 0, 4, "IYear", 1);
            for (i = 1; i <= chunks[4].Count; i++)
                CreatePage7(crftList, "Crafts.htm", "Crafts4.htm", true, i, 4, "IYear", 1);

            crftList = IBQuery1Crft.OrderByDescending(x => x.cCnt).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ThenBy(x => x.c.Name).ToList();
            CreatePage7(crftList, "Crafts.htm", "Crafts5.htm", true, 0, 5, "Photos", 0);
            for (i = 1; i <= 11; i++)
                CreatePage7(crftList, "Crafts.htm", "Crafts5.htm", true, i, 5, "Photos", 0);

            crftList = IBQuery1Crft.OrderByDescending(x => x.c.CText?.Length).ThenBy(x => x.c.IYear).ThenBy(x => x.c.Construct).ThenBy(x => x.c.Name).ToList();
            CreatePage7(crftList, "Crafts.htm", "Crafts6.htm", true, 0, 6, "TextLen", 0);
            for (i = 1; i <= 11; i++)
                CreatePage7(crftList, "Crafts.htm", "Crafts6.htm", true, i, 6, "TextLen", 0);



            lInfo.Text = "Arts...";
            Application.DoEvents();
            if (is_stop) { return; };

            var artQry = _ctx.vwArts.Where(x => x.Source == "7");

            var IBQuery1CrftX = (
                from a in _ctx.vwArts
                    //join m in _ctx.Mags on a.Mag equals m.Id
                where a.Source == "7"
                select new { a }).ToList();
            IBQuery1Crft = IBQuery1CrftX
                .Select(x => new CraftList()
                {
                    a = Mapper.Map<ArtDto>(x.a),
                    aCnt = x.a.cnt ?? 0
                }).ToList();

            crftList = IBQuery1Crft.OrderBy(x => x.a.Serie).ThenBy(x => x.a.NN).ThenBy(x => x.a.Name).ThenBy(x => x.a.Author).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Mag).ToList();
            CreatePage7(crftList, "Arts.htm", "Arts1.htm", false, 0, 1, "IYear", 0);

            crftList = IBQuery1Crft.OrderBy(x => x.a.Author).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Mag).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage7(crftList, "Arts.htm", "Arts2.htm", false, 0, 2, "Author", 0);

            crftList = IBQuery1Crft.OrderBy(x => x.a.Name).ThenBy(x => x.a.Author).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Mag).ThenBy(x => x.a.NN).ToList();
            CreatePage7(crftList, "Arts.htm", "Arts3.htm", false, 0, 3, "Name", 0);

            crftList = IBQuery1Crft.OrderBy(x => x.a.Mag).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Author).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage7(crftList, "Arts.htm", "Arts4.htm", false, 0, 4, "Mag", 0);

            crftList = IBQuery1Crft.OrderByDescending(x => x.aCnt).ThenBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Mag).ThenBy(x => x.a.Author).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage7(crftList, "Arts.htm", "Arts5.htm", false, 0, 5, "Photos", 0);

            crftList = IBQuery1Crft.OrderBy(x => x.a.IYear).ThenBy(x => x.a.IMonth).ThenBy(x => x.a.Mag).ThenBy(x => x.a.Author).ThenBy(x => x.a.Name).ThenBy(x => x.a.NN).ToList();
            CreatePage7(crftList, "Arts.htm", "Arts6.htm", false, 0, 6, "IYear", 0);

            var IBQuery1 = _ctx.Crafts.Where(x => !x.Same.HasValue && x.Source == "7")
                .OrderBy(x => x.Country).ThenBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name)
                .Select(Mapper.Map<CraftDto>).ToList();


            lSources = new List<string>();
            if (File.Exists("E:\\Live\\Avia\\AviaDejaVu\\Site2\\Sources.txt")) {
                var logFile = File.ReadAllLines("E:\\Live\\Avia\\AviaDejaVu\\Site2\\Sources.txt");
                lSources = new List<string>(logFile);
            }
            lSourceNames = new List<string>();
            if (File.Exists("E:\\Live\\Avia\\AviaDejaVu\\Site2\\SourceNames.txt")) {
                var logFile = File.ReadAllLines("E:\\Live\\Avia\\AviaDejaVu\\Site2\\SourceNames.txt");
                lSourceNames = new List<string>(logFile);
            }
            lSourceIds = new List<string>();
            if (File.Exists("E:\\Live\\Avia\\AviaDejaVu\\Site2\\SourceIds.txt")) {
                var logFile = File.ReadAllLines("E:\\Live\\Avia\\AviaDejaVu\\Site2\\SourceIds.txt");
                lSourceIds = new List<string>(logFile);
            }

            var lLog = new List<string>();

            var IBQuery2 = (
                from p in _ctx.Pics
                join a in _ctx.Arts on p.ArtId equals a.ArtId
                orderby p.CraftId, p.NType, p.NNN
                select new { p, a });

            for (var ibq1 = 0; ibq1 < IBQuery1.Count; ibq1++)
            {
                var ibq1c = IBQuery1[ibq1];


                if (ibq1 == IBQuery1.Count - 1)
                {
                    NextId = FirstId;
                    NextName = FirstName;
                }
                else
                {
                    NextId = IBQuery1[ibq1 + 1].CraftId;
                    NextName = GetCraftName(IBQuery1[ibq1 + 1], true);
                }

                pics = IBQuery2.Where(x => x.p.CraftId == ibq1c.CraftId).ToList()
                    .Select(x => new PicArt()
                    {
                        p = Mapper.Map<PicDto>(x.p),
                        a = Mapper.Map<ArtDto>(x.a)
                    }).ToList();
                LoadPic7(ibq1c, pics);

                PrevId = ibq1c.CraftId;
                PrevName = GetCraftName(ibq1c, true);

                lInfo.Text = $"Craft {ibq1c.IYear} {ibq1c.Construct}";
                Application.DoEvents();
                if (is_stop) { return; };

            }

            SaveToFile(lSources, "E:\\Live\\Avia\\AviaDejaVu\\Site2\\Sources.txt");
            SaveToFile(lSourceNames, "E:\\Live\\Avia\\AviaDejaVu\\Site2\\SourceNames.txt");
            SaveToFile(lSourceIds, "E:\\Live\\Avia\\AviaDejaVu\\Site2\\SourceIds.txt");
            SaveToFile(lLog, "E:\\Live\\Avia\\AviaDejaVu\\Site2\\Log.txt");


            var ibq1A = artQry
                .OrderBy(x => x.IYear).ThenBy(x => x.IMonth).ThenBy(x => x.Mag).ThenBy(x => x.Author).ThenBy(x => x.Name).ThenBy(x => x.NN)
                .ToList()
                .Select(x => Mapper.Map<ArtDto>(x)).ToList();

            var sArt = "";
            var lArts = new List<Pair<int>>();
            var maxArt = 0; var lastArt = 0;
            for (var ibq1AaCnt = 0; ibq1AaCnt < ibq1A.Count; ibq1AaCnt++)
            {
                var ibq1Aa = ibq1A[ibq1AaCnt];

                if (sArt != GetArtName(ibq1Aa))
                {
                    sArt = GetArtName(ibq1Aa);
                    lArts.Clear();

                    var ibq1AaCnt2 = ibq1AaCnt;
                    var ibq1Aa2 = ibq1A[ibq1AaCnt2];
                    while ((ibq1AaCnt2 < ibq1A.Count) && (sArt == GetArtName(ibq1A[ibq1AaCnt2])))
                    {
                        ibq1Aa2 = ibq1A[ibq1AaCnt2];
                        s = GetArtName2(ibq1Aa2) ?? "";
                        if (s.Trim() == "") s = "-";
                        lArts.Add(new Pair<int>() { Name = s, Id = ibq1Aa2.ArtId });

                        ibq1AaCnt2++;
                    }

                }
                if (maxArt < ibq1Aa.ArtId)
                {
                    maxArt = ibq1Aa.ArtId;
                    lastArt = lArts[0].Id;
                }

                templ = File.ReadAllText(sPath + "Site2\\Art.htm");
                while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");


                i = templ.IndexOf("%begin%");
                sBeg = templ.Substring(0, i);
                sEnd = templ.Substring(i + 7);
                i = sEnd.IndexOf("%end%");
                sMid = sEnd.Substring(0, i);
                sEnd = sEnd.Substring(i + 5);

                var art = ArtTexts[ibq1Aa.ArtId];
                var craftIds = art.Crafts.Keys;

                var ibq2AP = (
                    from c in _ctx.Crafts
                    from p in _ctx.Pics.Where(p => 
                        p.CraftId == c.CraftId &&
                        p.ArtId == ibq1Aa.ArtId &&
                        !Const.Arts.Copyrighted.Contains(p.ArtId) && !(p.Copyright ?? false)
                    ).DefaultIfEmpty()
                    where craftIds.Contains(c.CraftId)
                    orderby c.Country, c.Construct, c.IYear, c.Name, p.NType, p.NNN
                    select new { p, c })
                    .ToList()
                    .Select(x => new PicArt()
                    {
                        p = Mapper.Map<PicDto>(x.p),
                        c = Mapper.Map<CraftDto>(x.c)
                    }).ToList();

                var phCnt = 0;
                if (ibq2AP.Any())
                {
                    var curCraft = -1;
                    foreach (var ibq2APp in ibq2AP)
                    {
                        if (curCraft != ibq2APp.c.CraftId)
                        {
                            curCraft = ibq2APp.c.CraftId;
                            var craftLink = $"../Crafts/Craft{ibq2APp.c.CraftId}.htm";
                            var craftName = $"{ibq2APp.c.Country} {ibq2APp.c.Construct} {ibq2APp.c.Name} {ibq2APp.c.IYear}";
                            sBeg += $"<div class='tabletoprow'><h4><a class='cp' href='{craftLink}'>{craftName}</a></h4></div>";
                            if (art.Crafts.ContainsKey(curCraft))
                            {
                                var crft = art.Crafts[curCraft];
                                if (!string.IsNullOrEmpty(crft.CraftText))
                                {
                                    var st = crft.CraftText;
                                    if (st.Length > 2000)
                                    {
                                        st = $"<div class='scroll'>{st}</div>";
                                    }
                                    sBeg += $"<div class='tablerow'>{st}</div>";
                                }
                            }
                        }

                        if (ibq2APp.p != null)
                        {
                            ss = new StringBuilder(ibq2APp.p.Text ?? "")
                                .Replace("\n\r ", "<br>&nbsp;&nbsp;")
                                .Replace("\n ", "<br>&nbsp;&nbsp;")
                                .Replace("\n\r", "<br>")
                                .Replace("\n", "<br>")
                                .ToString();

                            var spic = ibq2APp.p.Path.Replace("\\", "/");
                            var spicX = spic;
                            if ((spic.IndexOf("/GT-0/") >= 0) || (spic.IndexOf("/Opd-0/") >= 0))
                                spicX = $"copyright.jpg\" class='cp_i' cpi='../../Images7/{spic}'";
                            var sx = new StringBuilder(sMid)
                                .Replace("%PicPath%", spic)
                                .Replace("%PicPathX%", spicX)
                                /*.Replace("%CraftLink%", $"../Crafts/Craft{ibq2APp.p.CraftId}.htm")
                                .Replace("%CraftName%",
                                    ibq2APp.c.Construct + " " +
                                    ibq2APp.c.Name + " - " +
                                    ibq2APp.c.Country + " - " +
                                    ibq2APp.c.IYear.ToString())*/
                                .Replace("%PicText%", ss)
                                .ToString();

                            sBeg += sx;
                            phCnt++;
                        }
                    }
                }
                else
                {
                    sBeg += "<tr><td>Нет фотографий</td></tr>";
                }


                templ = sBeg + sEnd;

                templ = templ.Replace("%Art%", GetArtName(ibq1Aa));

                /*i = templ.IndexOf("%xbegin%");

                sBeg = templ.Substring(0, i);
                sEnd = templ.Substring(i + 8);
                i = sEnd.IndexOf("%xend%");
                sMid = sEnd.Substring(0, i);
                sEnd = sEnd.Substring(i + 6);
                i = sMid.IndexOf("%xmid%");
                var sMid2 = sMid.Substring(i + 6);
                sMid = sMid.Substring(0, i);

                for (i = 0; i < lArts.Count; i++)
                {
                    if (ibq1Aa.ArtId == lArts[i].Id)
                        sx = sMid;
                    else
                        sx = sMid2;
                    sx = new StringBuilder(sx)
                        .Replace("%ArtName%", lArts[i].Name)
                        .Replace("%ArtLink%", $"Art{lArts[i].Id}.htm")
                        .ToString();
                    sBeg += sx;
                }

                templ = sBeg + sEnd;
                */

                templ = new StringBuilder(templ)
                    .Replace("%ArtName%", GetArtName2(ibq1Aa))
                    .Replace("%ArtSerie%", CheckNull(ibq1Aa.Serie))
                    .Replace("%ArtAuthor%", CheckNull(ibq1Aa.Author))
                    .Replace("%ArtNaim%", CheckNull(ibq1Aa.Name))
                    .Replace("%ArtPhoto%", phCnt.ToString())
                    .ToString();


                s = sPath + $"Site2\\Arts\\Art{ibq1Aa.ArtId}.htm";
                ss = sPath + $"Upload2\\Site2\\Arts\\Art{ibq1Aa.ArtId}.htm";
                if (File.Exists(s))
                {
                    upl = File.ReadAllText(s);
                    if (templ != upl)
                        File.WriteAllText(ss, templ, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText(ss, templ, Encoding.UTF8);
                }
                File.WriteAllText(s, templ, Encoding.UTF8);


                lInfo.Text = $"Art {ibq1Aa.IYear} {ibq1Aa.IMonth.Trim()} {ibq1Aa.Mag.Trim()}";
                Application.DoEvents();
                if (is_stop) { return; };

            }

            templ = File.ReadAllText(sPath + "Site2\\index.htm");
            while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");

            i = templ.IndexOf("%begin%");
            sBeg = templ.Substring(0, i);
            sEnd = templ.Substring(i + 7);
            i = sEnd.IndexOf("%end%");
            sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 5);

            cnt = 0;
            var uplA = File.ReadAllLines(sPath + "History2.ini");
            var uplL = new List<string>(uplA);
            i = 0;
            var s0 = "";
            do
            {
                s = uplL[i];
                if (s[0] == ' ')
                {
                    s = uplL[i].Substring(1);
                    ii = s.IndexOf(" ");
                    cnt++;
                    s = s.Substring(ii + 1);
                    ii = s.IndexOf(" ");
                    ss = s.Substring(0, ii);
                    s = s.Substring(ii + 1);

                    //!!! ii := GetArtId(s);

                    if (s0 == "")
                        s0 = GetStringDate(ss);

                    sBeg += new StringBuilder(sMid)
                        .Replace("%HistDate%", ss)
                        .Replace("%HistName%", s)
                        .ToString();
                }
                i++;
            } while (cnt <= 50 && cnt < uplL.Count);

            templ = (sBeg + sEnd).Replace("%StringDate%", s0);

            uplA = File.ReadAllLines(sPath + "counts2.txt");
            uplL = new List<string>(uplA);
            cnt = int.Parse(uplL[3]);
            var RecordCount = artQry.Select(a => new { a.Mag, a.IYear, a.IMonth }).Distinct().Count();
            if (RecordCount > cnt)
            {
                uplL[7] = (RecordCount - cnt).ToString();
                uplL[3] = RecordCount.ToString();

                var newCnt = artQry.Count();
                cnt = int.Parse(uplL[2]);
                uplL[6] = (newCnt - cnt).ToString();
                uplL[2] = newCnt.ToString();

                newCnt = _ctx.Crafts.Where(x => x.Source == "7").Count();
                cnt = int.Parse(uplL[1]);
                uplL[5] = (newCnt - cnt).ToString();
                uplL[1] = newCnt.ToString();

                newCnt = (
                    from p in _ctx.Pics
                    join c in _ctx.Crafts on p.CraftId equals c.CraftId
                    where c.Source == "7"
                    select p.Path).Distinct().Count();
                cnt = int.Parse(uplL[0]);
                uplL[4] = (newCnt - cnt).ToString();
                uplL[0] = newCnt.ToString();

                File.WriteAllLines(sPath + "counts2.txt", uplL.ToArray());
            }

            templ = new StringBuilder(templ)
                .Replace("%LastArtLink%", $"Site2\\Arts\\Art{lastArt}.htm")
                .Replace("%PicCnt%", uplL[0])
                .Replace("%CraftCnt%", uplL[1])
                .Replace("%ArtCnt%", uplL[2])
                .Replace("%MagCnt%", uplL[3])
                .Replace("%PicCntAdd%", uplL[4])
                .Replace("%CraftCntAdd%", uplL[5])
                .Replace("%ArtCntAdd%", uplL[6])
                .Replace("%MagCntAdd%", uplL[7])
                .ToString();

            File.WriteAllText(sPath + "Upload2\\index.htm", templ, Encoding.UTF8);

            //ForceDirectories(sPath+"Upload2\Images7");
            //CopyFile(PChar(sPath+"History2.ini"), PChar(sPath+"Upload2\Images7\History2.ini"), False);

            MessageBox.Show("OK!");
        }



        private void LoadDir7(string sPath, string ssPath)
        {

            _lInfo.Text = sPath;
            Application.DoEvents();
            if (is_stop) { return; };

            var sPath1 = sPath.Replace("\\..\\", "\\Upload2\\");
            var ssPath1 = ssPath.Replace("\\..\\", "\\Upload2\\");
            var dirs = Directory.GetDirectories(sPath);
            foreach (var dir in dirs)
            {
                var sr = Path.GetFileName(dir);
                LoadDir7(sPath + "\\" + sr, ssPath + "\\" + sr);
            }

            var files = Directory.GetFiles(sPath, "*.jpg").ToList();
            files.AddRange(Directory.GetFiles(sPath, "*.gif"));
            foreach (var f in files)
            {
                var file = Path.GetFileName(f);
                if (File.Exists(ssPath + "\\" + file))
                {
                    continue;
                }


                Directory.CreateDirectory(ssPath);
                Directory.CreateDirectory(sPath1);
                Directory.CreateDirectory(ssPath1);

                const int wMax = 200;
                const int hMax = 150;

                using (var srcImage = Image.FromFile(sPath + "\\" + file))
                {
                    var w = wMax;
                    var h = (srcImage.Height * wMax) / srcImage.Width;
                    if (h > hMax)
                    {
                        h = hMax;
                        w = (srcImage.Width * hMax) / srcImage.Height;
                    }

                    using (var newImage = new Bitmap(w, h))
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, w, h));

                        var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        var myEncoderParameters = new EncoderParameters(1);
                        var myEncoderParameter = new EncoderParameter(myEncoder, 80L);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        newImage.Save(ssPath + "\\" + file, jpgEncoder, myEncoderParameters);
                        newImage.Save(ssPath1 + "\\" + file, jpgEncoder, myEncoderParameters);
                        File.Copy(sPath + "\\" + file, sPath1 + "\\" + file);
                    }
                }
            }
        }



        private void CreatePage7(List<CraftList> crafts, string sTempl, string sPage, bool is_craft, int nn, int xx, string indField, int xtype)
        {
            // xtype: 0 - 10 стандартных страниц; 1 - по названию; 2 - 3 первые буквы

            _lInfo.Text = $"{(is_craft ? "Crafts" : "Arts")} ... {indField} {nn}";
            Application.DoEvents();
            if (is_stop) { return; };

            string s;
            int craftInd = 0; //!!!??? -1;

            templ = File.ReadAllText(sPath + "Site2\\" + sTempl);
            while (templ.IndexOf("  ") >= 0) templ = templ.Replace("  ", " ");

            var i = templ.IndexOf("%begin%");
            var sBeg = templ.Substring(0, i);
            var sEnd = templ.Substring(i + 7);
            i = sEnd.IndexOf("%end%");
            var sMid = sEnd.Substring(0, i);
            sEnd = sEnd.Substring(i + 5);

            var ch = chunks[xx];
            var chunksize = (crafts.Count / 10) + 1;
            if (xtype > 0)
            {
                if (nn == 0)
                {
                    var ll = elements[xx];
                    foreach (var craft in crafts)
                    {
                        s = GetPrVal(craft, indField, is_craft);
                        if (xtype == 2) s = s.Substring(0, Math.Min(3, s.Length));
                        var el = ll.FirstOrDefault(x => x.Name == s); i = el == null ? -1 : ll.IndexOf(el);
                        if (i < 0) { ll.Add(new Pair<int>() { Name = s, Id = 0 }); i = ll.Count - 1; }
                        ll[i].Id = ll[i].Id + 1;
                    }

                    chunk = new TChunk();
                    for (i = 0; i < ll.Count; i++)
                    {
                        if (chunk.cnt == 0)
                        {
                            chunk.iBeg = i;
                            chunk.sBeg = ll[i].Name;
                        }
                        if (ll[i].Id >= chunksize)
                        {
                            if (chunk.cnt > 0)
                            {
                                ch.Add(chunk);
                                chunk = new TChunk();
                                chunk.iBeg = i;
                                chunk.sBeg = ll[i].Name;
                            }
                            chunk.cnt = ll[i].Id;
                            chunk.elmts = 1;
                            chunk.iEnd = i;
                            chunk.sEnd = ll[i].Name;
                            ch.Add(chunk);
                            chunk = new TChunk();
                        }
                        else
                        {
                            chunk.cnt += ll[i].Id;
                            chunk.elmts++;
                            chunk.iEnd = i;
                            chunk.sEnd = ll[i].Name;
                        }
                    }
                    if (chunk.cnt > 0) ch.Add(chunk);

                    i = 0;
                    while (i < ch.Count)
                    {
                        chunk = ch[i];
                        if ((chunk.cnt > Math.Floor(chunksize * 1.5)) && (chunk.elmts > 1))
                        {
                            var minichunks = Math.Round((float)chunk.cnt / chunksize);
                            if (minichunks == 0) minichunks = 1;
                            var minichunksize = (chunk.cnt / minichunks) + 1;

                            var chcnt = 1;
                            var ch1 = new TChunk();
                            ch1.iBeg = chunk.iBeg;
                            ch1.sBeg = chunk.sBeg;
                            ch1.iEnd = chunk.iBeg;
                            ch1.sEnd = chunk.sBeg;
                            ch.Insert(i + chcnt, ch1);
                            var j = chunk.iBeg;
                            while ((j <= chunk.iEnd) && (chcnt < minichunks))
                            {
                                if ((ch1.cnt + ll[j].Id) > minichunksize)
                                {
                                    chcnt++;
                                    var d1 = ch1.cnt + ll[j].Id - minichunksize;
                                    var d2 = minichunksize - ch1.cnt;
                                    if (d1 > d2)
                                    {
                                        ch1 = new TChunk();
                                        ch1.iBeg = j;
                                        ch1.sBeg = ll[j].Name;
                                        ch1.iEnd = j;
                                        ch1.sEnd = ll[j].Name;
                                        ch1.cnt = ll[j].Id;
                                        ch1.elmts = 1;
                                        ch.Insert(i + chcnt, ch1);
                                    }
                                    else
                                    {
                                        ch1.cnt += ll[j].Id;
                                        ch1.iEnd = j;
                                        ch1.sEnd = ll[j].Name;
                                        ch1.elmts++;
                                        ch1 = new TChunk();
                                        ch1.iBeg = j + 1;
                                        ch1.iEnd = j + 1;
                                        if ((j + 1) < ll.Count)
                                        {
                                            ch1.sBeg = ll[j + 1].Name;
                                            ch1.sEnd = ll[j + 1].Name;
                                        }
                                        ch.Insert(i + chcnt, ch1);
                                    }
                                }
                                else
                                {
                                    ch1.cnt += ll[j].Id;
                                    ch1.iEnd = j;
                                    ch1.sEnd = ll[j].Name;
                                    ch1.elmts++;
                                }
                                j++;
                            }
                            while (j <= chunk.iEnd)
                            {

                                ch1.cnt += ll[j].Id;
                                ch1.iEnd = j;
                                ch1.sEnd = ll[j].Name;
                                ch1.elmts++;

                                j++;
                            }
                            if (ch1.cnt == 0)
                            {
                                ch.Remove(ch1);
                                chcnt--;
                            }

                            ch.Remove(chunk);
                            i += chcnt - 1;
                        }

                        i++;
                    }
                    craftInd = 0;

                }
                else
                {
                    chunk = ch[nn - 1];
                    if (chunk.sBeg == "")
                        craftInd = 0;
                    else
                    {
                        craftInd = crafts.IndexOf(crafts.First(x =>
                        {
                            var sv = GetPrVal(x, indField, is_craft);
                            return string.Compare(sv, chunk.sBeg) >= 0;
                        }));
                    }
                    cnt = chunk.cnt;
                }
            }
            else
            {
                if (nn > 0)
                {
                    cnt = chunksize;
                    craftInd = chunksize * (nn - 1);
                }
            }

            var sb = new StringBuilder(sBeg);
            while (craftInd < crafts.Count)
            {
                var craft = crafts[craftInd];
                if (is_craft)
                {
                    sb.Append(new StringBuilder(sMid)
                        .Replace("%Country%", CheckNull(craft.c.Country))
                        .Replace("%Construct%", CheckNull(craft.c.Construct))
                        .Replace("%Name%", CheckNull(craft.c.Name))
                        .Replace("%Year%", CheckNull(craft.c.IYear.ToString()))
                        .Replace("%Vert%", craft.c.Vert ?? false ? "<span title='Вертолет'>В</span>" : "")
                        .Replace("%UAV%", craft.c.Uav ?? false ? "<span title='Беспилотный'>Б</span>" : "")
                        .Replace("%Glider%", craft.c.Glider ?? false ? "<span title='Планер'>П</span>" : "")
                        .Replace("%Single%", craft.c.Single ?? false ? "<span title='Единственный экземпляр'>1</span>" : "")
                        .Replace("%Type%", CheckNull(craft.c.Type))
                        .Replace("%Photos%", CheckNull(craft.cCnt.ToString()))
                        .Replace("%Textlen%", CheckNull((craft.c.CText?.Length ?? 0).ToString()))
                        .Replace("%CraftLink%", $"../Crafts/Craft{craft.c.CraftId}.htm")
                        .ToString());

                }
                else
                {
                    sb.Append(new StringBuilder(sMid)
                        .Replace("%Date%", GetYear(craft.a))
                        .Replace("%Mag%", GetArtName(craft.a, false, true))
                        .Replace("%Author%", CheckNull(craft.a.Author))
                        .Replace("%Name%", CheckNull(craft.a.Name))
                        .Replace("%Serie%", CheckNull(craft.a.Serie))
                        .Replace("%NN%", (craft.a.NN ?? 0) == 0 ? "" : craft.a.NN.ToString())
                        .Replace("%Photos%", CheckNull(craft.aCnt.ToString()))
                        .Replace("%ArtLink%", $"../Arts/Art{craft.a.ArtId}.htm")
                        .ToString());
                }
                craftInd++;
                if (nn > 0)
                {
                    cnt--;
                    if (cnt == 0) break;
                }

                Application.DoEvents();
                if (is_stop) { return; };
            }

            templ = sb.ToString() + sEnd;

            sBeg = templ;


            s = "";
            if (xtype == 0)
                ii = 10;
            else
                ii = ch.Count;
            for (i = 0; i <= ii; i++)
            {
                if ((i > 0) && (xtype > 0))
                    chunk = ch[i - 1];
                var sx = "<span>";
                if (i == 0)
                    ss = "Все";
                else
                {
                    if (xtype == 0)
                    {
                        ss = i.ToString();
                        sx = "<span>";
                    }
                    else
                    {
                        string s1;
                        string s2;
                        if ((indField == "Mag") || (indField == "Template"))
                        {
                            s1 = GetMagName(chunk.sBeg);
                            s2 = GetMagName(chunk.sEnd);
                        }
                        else
                        {
                            s1 = chunk.sBeg;
                            s2 = chunk.sEnd;
                        }

                        if (s1 == s2)
                            ss = s1 == "" ? "&nbsp;" : s1;
                        else
                            ss = (s1 == "" ? "?" : s1) + " - " + s2;
                        sx = $"<span class='pageitem{(i == nn ? "selected" : "")}' title='Записей: {chunk.cnt}'>";
                    }
                }
                string sww;
                if (i == nn)
                {
                    sww = "<b>" + ss + "</b></span>";
                }
                else
                {
                    var iw = xx;
                    if (iw == 10) iw = 1;
                    sww = $"<a class='cp' href='{(is_craft ? "Crafts" : "Arts")}{xx}{(i == 0 ? "" : "-" + i.ToString())}.htm'>{ss}</a></span>";
                }
                ss = sx + sww;
                if (i != ii)
                    ss += "&nbsp;|&nbsp;";

                s += "<td>" + ss + "</td>";
            }

            sBeg = sBeg.Replace("%pager%", s);

            s = "";
            for (i = 1; i <= 6; i++)
            {
                ss = i == xx ? "" : $"<a class='cp' href='{(is_craft ? "Crafts" : "Arts")}{i}{(is_craft ? "-1" : "")}.htm'>";
                var sss = i == xx ? "&nbsp;<img src='../../imgs/arrdown.gif' width='12' height='6' border='0' />" : "</a>";
                sBeg = new StringBuilder(sBeg)
                        .Replace($"%Sort{i}%", ss)
                        .Replace($"%SortX{i}%", sss)
                        .ToString();
            }

            templ = sBeg;

            sPage = "Indexes\\" + sPage;
            if (nn > 0) sPage = sPage.Replace(".htm", $"-{nn}.htm");
            s = sPath + "Site2\\" + sPage;
            ss = sPath + "Upload2\\Site2\\" + sPage;
            Directory.CreateDirectory(Path.GetDirectoryName(s));
            Directory.CreateDirectory(Path.GetDirectoryName(ss));
            if (File.Exists(s))
            {
                upl = File.ReadAllText(s);
                if (templ != upl)
                {
                    File.WriteAllText(ss, templ, Encoding.UTF8);
                }
            }
            else
            {
                File.WriteAllText(ss, templ, Encoding.UTF8);
            }
            File.WriteAllText(s, templ, Encoding.UTF8);

        }

        private bool CheckFlight(int j)
        {
            return
                (j == 4667) || (j == 4668) || (j == 4669) || (j == 4676) || (j == 4689) || (j == 4713) || 
                (j == 4730) || (j == 4851) || (j == 4897) || (j == 4916) || (j == 4952) || (j == 5748);
        }

    }
}
