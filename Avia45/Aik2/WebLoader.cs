using DevAge.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static Aik2.Const;
using static Aik2.Const.Columns;

namespace Aik2
{
    public class WebLoader
    {
        private string s, ss, su, text;
        private List<string> ls, lc, lcc, lct;
        private Label _lInfo;

        private string WriteTextX(string s)
        {
            s = s.Replace("\t", " ").Replace("  ", " ");
            return s;
        }


        private int GetTable(int start, StreamWriter M, string file)
        {
            int i, j, ii, jj, jx;
            var Result = s.Length;
            i = su.IndexOf("<TABLE", start + 1);
            if (i < 0) return Result;
            var sName = StrNoTags(s.Substring(start, i - start - 1));
            if (sName.StartsWith("Модификации")) {
                text += "\n\nМодификации:";
                ii = su.IndexOf("<TABLE", i + 1);
                if (ii < 0) ii = s.Length;
                j = su.IndexOf("<TR", i);
                while ((j >= 0) && (j < ii)) {
                    j = su.IndexOf("<TD", j);
                    jj = su.IndexOf("<TD", j + 1);
                    sName = StrNoTags(s.Substring(j, jj - j));
                    text += "\n  " + sName;
                    j = su.IndexOf("<TR", jj);
                    if (j < 0) j = ii;
                    sName = StrNoTags(s.Substring(jj, j - jj));
                    sName = sName.Replace("\r\n", " ");
                    do {
                        ss = sName;
                        sName = sName.Replace("  ", " ");
                    } while (ss != sName);
                    text += "    " + sName;
                }
                text += "\n";
                Result = ii;
            } else if (sName.StartsWith("ЛТХ")) {
                text += "\n\nЛТХ:";
                ii = su.IndexOf("<TABLE", i + 1);
                if (ii < 0) ii = s.Length;
                j = su.IndexOf("<TR", i);
                while ((j >= 0) && (j < ii)) {
                    j = su.IndexOf("<TD", j);
                    jj = su.IndexOf("<TD", j + 1);
                    sName = StrNoTags(s.Substring(j, jj - j));
                    text += $"\n{sName}\t";
                    j = su.IndexOf("<TR", jj);
                    if (j < 0) j = ii;
                    sName = StrNoTags(s.Substring(jj, j - jj));
                    sName = sName.Replace("\r\n", " ");
                    do {
                        ss = sName;
                        sName = sName.Replace("  ", " ");
                    } while (ss != sName);
                    text += sName;
                }
                Result = ii;
            } else if (sName.StartsWith("Доп.")) {
                var type = "f";
                ii = su.IndexOf("<TABLE", i + 1);
                if (ii < 0) ii = s.Length;
                jj = su.IndexOf("</TABLE", i + 1);
                if (jj < 0) jj = s.Length;
                while (ii < jj) {
                    j = su.IndexOf("<TR", ii);
                    while ((j > 0) && (j < jj)) {
                        j = su.IndexOf("<TD", j);
                        jx = su.IndexOf("<TD", j + 1);
                        sName = s.Substring(j, jx - j);
                        var suName = su.Substring(j, jx - j);
                        var k = suName.IndexOf("<A HREF=");
                        if (k < 0) {
                            M.WriteLine($"Pic?: {file}");
                            break;
                        }
                        var kk = suName.IndexOf(".JPG", k);
                        var kl = suName.IndexOf(".GIF", k);
                        if ((kk < 0) && (kl < 0)) {
                            M.WriteLine($"Pic??: {file}");
                            break;
                        }
                        if ((kk > kl) || (kk < 0))
                            if (kl >= 0) kk = kl;
                        ss = sName.Substring(k + 8, kk - k - 3);
                        ss = ss.Replace("\"", "");

                        k = ss.ToUpper().IndexOf("/IMAGE/");
                        if (k < 0) {
                            M.WriteLine($"Pic???: {file}");
                            break;
                        }
                        ss = ss.Substring(k + 7);
                        ls.Add(ss);
                        lc.Add(ss);
                        lcc.Add("");
                        lct.Add(type);

                        j = su.IndexOf("<TR", jx);
                        if (j < 0) j = ii;
                        if (j > jj)
                        {
                            var sx = StrNoTags(s.Substring(jj, j - jj));
                            if (sx.Contains("Схемы"))
                            {
                                type = "p";
                            }
                            else if (sx.Contains("Варианты окраски"))
                            {
                                type = "s";
                            }
                            j = jj;
                        }
                        sName = StrNoTags(ClearString(s.Substring(jx, j - jx)));

                        if (!string.IsNullOrEmpty(sName))
                            lcc[lcc.Count - 1] = sName;
                    }
                    i = jj;
                    ii = su.IndexOf("<TABLE", i + 1);
                    if (ii < 0) ii = s.Length;
                    jj = su.IndexOf("</TABLE", i + 1);
                    if (jj < 0) jj = s.Length;
                }
                Result = ii;
            } else if (sName.StartsWith("Список")) {
                text += "\n\nСписок источников:\n";
                ii = su.IndexOf("<TABLE", i + 1);
                if (ii < 0) ii = s.Length;
                jj = su.IndexOf("</TABLE", i + 1);
                if (jj < 0) jj = s.Length;
                if (ii > jj) ii = jj;
                sName = s.Substring(i, ii - i - 1);
                sName = sName.Replace("\r\n", " ")
                    .Replace("<br>", "\n")
                    .Replace("<BR>", "\n");
                sName = StrNoTags(sName);
                text += sName;
                Result = ii;
            } else {
                M.WriteLine($"Bad table: {file}\n{sName}");
                Result = su.IndexOf("<TABLE", i + 1);
                if (Result < 0) Result = s.Length;
            }
            return Result;
        }

        private string DoWriteText(int i)
        {
            var sName = s.Substring(0, i);
            sName = sName
                .Replace("\r\n", " ")
                .Replace("<br>", "\n\n")
                .Replace("<BR>", "\n\n")
                .Replace("<P ", "\n\n<P ")
                .Replace("<p ", "\n\n<p ");
            sName = StrNoTags(sName);
            do {
                ss = sName;
                sName = sName.Replace("  ", " ");
            } while (ss != sName);
            TrimAll(i);
            return WriteTextX(sName);
        }


        private void TrimAll(int i)
        {
            s = s.Substring(i);
            su = su.Substring(i);
        }


        private string StrNoTags(string s, bool is_br = false)
        {
            var Result = "";
            if (is_br)
            {
                s = s.Replace("\r\n", " ").Replace("\n", " ");
            }
            s = HttpUtility.HtmlDecode(s.Trim());
            if (!string.IsNullOrEmpty(s))
            {
                var i = s.IndexOf("<");
                var j = s.IndexOf("&");
                while ((i >= 0) || (j >= 0))
                {
                    if ((i >= 0) && ((i < j) || (j < 0)))
                    {
                        if (i > 0)
                            Result += s.Substring(0, i);
                        s = s.Substring(i + 1);
                        j = s.IndexOf(">");
                        if (is_br && (j >= 0))
                        {
                            var ss = s.Substring(0, j).ToUpper();
                            if ((ss == "BR") || (ss == "P") || (ss == "LI") || (ss.Substring(0, 2) == "P "))
                            {
                                Result = Result.Trim() + "\n";
                            }
                        }
                        if (j >= 0) s = s.Substring(j + 1);
                        else s = "";
                    }
                    else
                    {
                        Result += s.Substring(0, j);
                        var l = s.Length;
                        i = j + 1;
                        while ((i < l) && (s[i] > ' ') && !(";<".IndexOf(s[i]) >= 0)) /*IsAlNum(s[i])*/ i++;
                        if ((i >= l) || !(";<".IndexOf(s[i]) >= 0))
                        {
                            Result += "&";
                            s = s.Substring(j + 1);
                        }
                        else
                        {
                            if (s[i] == '<') i--;
                            s = " " + s.Substring(i + 1);
                        }
                    }
                    i = s.IndexOf("<");
                    j = s.IndexOf("&");
                }
                Result += s;
                if (!is_br)
                {
                    Result = Result.Trim();
                }
            }
            return Result;
        }

        private string ClearString(string s)
        {
            s = s.Replace("\t", " ").Replace("\n", " ").Replace("\r", " ");
            var i = s.IndexOf("  ");
            while (i >= 0)
            {
                s = s.Replace("  ", " ");
                i = s.IndexOf("  ");
            }
            return s;
        }



        public void Load1(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            _lInfo = lInfo;
            if (!(MessageBox.Show("Load Airwar?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;

            ctx.Database.ExecuteSqlCommand("Delete From dbo.WordLinks From dbo.WordLinks wl join dbo.Pics p on wl.PicId = p.PicId join dbo.Crafts c on c.CraftId = p.CraftId where c.Source = '1'");
            ctx.Database.ExecuteSqlCommand("Delete From dbo.WordLinks From dbo.WordLinks wl join dbo.Crafts c on c.CraftId = wl.CraftId where c.Source = '1'");
            ctx.Database.ExecuteSqlCommand("Delete From dbo.Pics From dbo.Pics p join dbo.Crafts c on c.CraftId = p.CraftId where c.Source = '1'");
            ctx.Database.ExecuteSqlCommand("Delete From dbo.Crafts where Source = '1'");
            ctx.Database.Connection.Close();
            ctx.Database.Connection.Open();

            var artId = ctx.Arts.Where(x => x.Name == "Airwar").Single().ArtId;
            var maxCraftId = ctx.Crafts.Max(c => c.CraftId);

            using (var M = File.CreateText(_imagesPath + "Bads.txt"))
            {

                ls = new List<string>();
                lc = new List<string>();
                lcc = new List<string>();
                lct = new List<string>();
                string sName;
                int i, j;

                var dirs = Directory.GetDirectories(_imagesPath + "Source1\\Pages\\enc", "*.");
                foreach (var dir in dirs)
                {
                    var files = Directory.GetFiles(dir, "*.html");
                    foreach (var file in files)
                    {
                        _lInfo.Text = file;
                        Application.DoEvents();
                        try
                        {
                            s = File.ReadAllText(file, Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage));
                            lc.Clear();
                            lcc.Clear();
                            lct.Clear();
                            su = s.ToUpper();
                            i = s.IndexOf("flags_small");
                            if (i < 0)
                            {
                                M.WriteLine($"No flag_small: {file}");
                                continue;
                            }
                            TrimAll(i);
                            i = s.IndexOf(">");
                            TrimAll(i + 1);
                            i = su.IndexOf("</TR");
                            sName = StrNoTags(s.Substring(0, i));
                            if (string.IsNullOrEmpty(sName))
                            {
                                M.WriteLine($"Bad name: {file}");
                                continue;
                            }
                            var craft = new Crafts()
                            {
                                CraftId = ++maxCraftId,
                                Source = "1",
                                Name = ClearString(sName)
                            };
                            var dirName = Path.GetFileName(dir).ToUpper();
                            if ((dirName.Length == 2 && dirName[1] == 'H') || dirName == "HELI")
                            {
                                craft.Vert = true;
                                //craft.Craft = False;
                            }
                            else
                            {
                                craft.Vert = false;
                                //craft.Craft = True;
                            }
                            if (dirName == "GLIDER")
                            {
                                craft.Uav = false;
                                craft.Glider = true;
                                //craft.MM = False;
                            }
                            else if (dirName == "BPLA")
                            {
                                craft.Uav = true;
                                craft.Glider = false;
                                //craft.MM = False;
                            }
                            else
                            {
                                craft.Uav = false;
                                craft.Glider = false;
                                //craft.MM = True;
                            }

                            var k = su.IndexOf("<IMG SRC");
                            if (k < 0)
                            {
                                M.WriteLine($"Pic?: {file}");
                                ///!!!??? or continue???
                                continue;
                            }
                            var kk = su.IndexOf(".JPG", k);
                            var kl = su.IndexOf(".GIF", k);
                            if ((kk == 0) && (kl == 0))
                            {
                                M.WriteLine($"Pic??: {file}");
                                ///!!!??? or continue???
                                continue;
                            }
                            if ((kk > kl) || (kk < 0))
                            {
                                if (kl >= 0) kk = kl;
                            }
                            ss = s.Substring(k + 8, kk - k - 3);
                            ss = ss.Replace("\"", "");
                            k = ss.ToUpper().IndexOf("/IMAGE/");
                            if (k < 0)
                            {
                                M.WriteLine($"Pic???: {file}");
                                ///!!!??? or continue???
                                continue;
                            }
                            ss = ss.Substring(k + 7);
                            ls.Add(ss);
                            lc.Add(ss);
                            lcc.Add("");
                            lct.Add("f");

                            i = s.IndexOf("Разработчик:");
                            if (i < 0)
                            {
                                M.WriteLine($"No construct: {file}");
                                continue;
                            }
                            TrimAll(i);
                            i = s.IndexOf("<");
                            TrimAll(i);
                            i = su.IndexOf("</TR>");
                            sName = StrNoTags(s.Substring(0, i));
                            if (string.IsNullOrEmpty(sName))
                            {
                                M.WriteLine($"Bad construct: {file}");
                                continue;
                            }
                            craft.Construct = sName;
                            i = s.IndexOf("Страна:");
                            if (i < 0)
                            {
                                M.WriteLine($"No country: {file}");
                                continue;
                            }
                            TrimAll(i);
                            i = s.IndexOf("<");
                            TrimAll(i);
                            i = su.IndexOf("</TR>");
                            sName = StrNoTags(s.Substring(0, i)).Trim();
                            if (string.IsNullOrEmpty(sName))
                            {
                                M.WriteLine($"Bad country: {file}");
                                continue;
                            }
                            if (sName.IndexOf(",") >= 0)
                                sName = "International";
                            if (sName.Length == 4)
                            {
                                if (sName[0] == sName[1] && sName[1] == sName[2])
                                    sName = "СССР";
                            }
                            if (sName.Length > 20)
                            {
                                M.WriteLine($"Too long country: {file}\n{sName}");
                                sName = sName.Substring(0, 20).Trim();
                            }
                            craft.Country = sName;
                            i = s.IndexOf("Первый полет:");
                            if (i >= 0)
                            {
                                TrimAll(i);
                                i = s.IndexOf("<");
                                TrimAll(i);
                                i = su.IndexOf("</TR>");
                                sName = StrNoTags(s.Substring(0, i));
                                if (!string.IsNullOrEmpty(sName))
                                {
                                    i = sName.IndexOf("?");
                                    if (i >= 0)
                                    {
                                        sName = sName.Substring(0, i);
                                        //FieldByName('YearBad').AsBoolean := True;
                                    }
                                    try
                                    {
                                        craft.IYear = int.Parse(sName);
                                    }
                                    catch { }
                                }
                            }
                            i = s.IndexOf("Тип:");
                            if (i < 0)
                            {
                                i = s.IndexOf("Type:");
                                if (i < 0)
                                {
                                    i = s.IndexOf("Тип :");
                                    if (i < 0)
                                    {
                                        i = s.IndexOf("Type :");
                                        if (i < 0)
                                        {
                                            M.WriteLine($"No type: {file}");
                                            //continue;
                                        }
                                    }
                                }
                            }
                            TrimAll(i);
                            i = s.IndexOf("<");
                            TrimAll(i);
                            i = su.IndexOf("</TR>");
                            sName = StrNoTags(ClearString(s.Substring(0, i)));
                            if (string.IsNullOrEmpty(sName))
                            {
                                M.WriteLine($"Bad type: {file}");
                                //continue;
                            }
                            craft.Type = sName;
                            i = su.IndexOf("<TABLE");
                            TrimAll(i);
                            i = su.IndexOf(">");
                            TrimAll(i + 1);
                            i = su.IndexOf("<TABLE");
                            TrimAll(i);
                            i = su.IndexOf(">");
                            TrimAll(i + 1);
                            i = su.IndexOf("<TABLE");
                            TrimAll(i);
                            i = su.IndexOf(">");
                            TrimAll(i + 1);
                            i = su.IndexOf("<TABLE");
                            if (i < 0)
                            {
                                M.WriteLine($"No text: {file}");
                                //continue;
                            }
                            text = DoWriteText(i);
                            while (true)
                            {
                                i = su.IndexOf("<TABLE");
                                j = su.IndexOf("</TABLE");
                                sName = su.Substring(0, j);
                                if (sName.IndexOf("TAB_CORNER_RIGHT.GIF") >= 0) break;
                                TrimAll(j);
                                i = su.IndexOf("<TABLE");
                                text += DoWriteText(i);
                            }
                            while (i >= 0)
                            {
                                var finish = GetTable(0, M, file);
                                TrimAll(finish);
                                i = su.IndexOf("<TABLE", 0);
                            }
                            craft.CText = text;

                            ctx.Crafts.Add(craft);
                            ctx.SaveChanges();

                            for (i = 0; i < lc.Count; i++)
                            {
                                var pic = new Pics()
                                {
                                    //Source = "1"
                                    CraftId = craft.CraftId,
                                    ArtId = artId,
                                    Path = lc[i].Replace("/", "\\"),
                                    Type = lct[i],
                                    NType = Util.GetNType(lct[i])
                                };
                                ctx.Pics.Add(pic);
                                if (!string.IsNullOrEmpty(lcc[i]))
                                {
                                    pic.Text = WriteTextX(lcc[i]);
                                }
                                ctx.SaveChanges();
                            }
                            //MessageBox.Show("XXX!");
                            Util.DetachAllEntities(ctx);
                        }
                        catch (Exception e)
                        {
                            M.WriteLine($"Exception in {file}:\n{e.Message}");
                            //MessageBox.Show(file + "\n" + e.Message);
                        }
                    }
                }
            }
            _lInfo.Text = "";
            MessageBox.Show("OK!");
        }

        List<string> GFile = new List<string>();
        int GFileI;

        private string FindString(string s, bool ucase = false) {
            var Result = "";
            var ss = ""; var su = "";
            while (!su.StartsWith(s) && !Eof2())
            {
                ss = ReadLn2();

                ss = ss.Trim();
                su = ucase ? ss.ToUpper() : ss;
            }
            if (su.StartsWith(s)) Result = ss;
            return Result;
        }

        private bool FindFirm(string su)
        {
            return (su.IndexOf("<CENTER") >= 0) && (su.IndexOf("<H") >= 0) && (su.IndexOf("<HR") < 0);
        }

        private int FindCraft(string ss)
        {
            var Result = ss.IndexOf(" = ");
            if (Result < 0)
                Result = ss.IndexOf(" - ");
            if (Result < 0)
                Result = ss.IndexOf(")= ");
            if (Result < 0)
                Result = ss.IndexOf(")- ");
            return Result;
        }

        string sc, st, stt, sn, sc1, imagesPath;
        List<string> lp;
        int iy, i, j, artId;
        bool was_first, was_year, new_craft;
        int q, cnt, bq;
        AiKEntities _ctx;

        private void ResetCraft()
        {
            sn = "";
            stt = "";
            iy = 0;
            sc1 = sc;
            ls.Clear();
            lp.Clear();
            was_year = false;
            bq = 0;
        }

        private bool IsDigit(char c)
        {
            return "0123456789".Contains(c);
        }

        private void SaveCraft(ref int maxCraftId)
        {
            int i;
            string sx, ssx;

            if (sn == "") {
                if (iy != 0)
                    sn = $"Model of {iy} y.";
                else {
                    if ((stt.IndexOf("SEE") < 0) && (stt.Trim() != "") && was_year)
                        sn = "???";
                }
            }

            if (sn == "") {
                ResetCraft();
            } else {
                ssx = ReadLn2();
                sx = ssx.ToUpper();
                if (sx.IndexOf("</TABLE>") < 0) {
                    GFileI--;
                    cnt++;
                    var craft = new Crafts()
                    {
                        CraftId = ++maxCraftId,
                        Source = "5"
                    };
                    if (iy == 0) {
                        ss = stt;
                        i = (" " + ss + "  ").IndexOf("19");
                        if (i < 0)
                            i = (" " + ss + "  ").IndexOf("20");
                        while (i >= 0) {
                            if (
                                IsDigit(ss[i + 1]) &&
                                IsDigit(ss[i + 2]) &&
                                (!IsDigit(ss[i - 2])) &&
                                (!IsDigit(ss[i + 3]))) {

                                iy = int.Parse(ss.Substring(i - 1, 4));
                                if ((iy > 1902) && (iy < 2010)) {
                                    craft.IYear = iy;
                                    break;
                                }
                            }
                            ss = ss.Substring(i);
                            i = (" " + ss + "  ").IndexOf("19");
                            if (i < 0)
                                i = (" " + ss + "  ").IndexOf("20");
                        }
                    } else
                        craft.IYear = iy;
                    craft.Construct = sc1;
                    craft.Name = sn.Length > 100 ? sn.Substring(0, 100) : sn;
                    //FieldByName("Country").AsString := "США";
                    craft.CText = stt.Trim();
                    _ctx.Crafts.Add(craft);
                    _ctx.SaveChanges();

                    for (i = 0; i < ls.Count; i++) {

                        if (File.Exists($"{imagesPath}Images5\\{lp[i]}")) {
                            var p = new Pics()
                            {
                                CraftId = craft.CraftId,
                                ArtId = artId,
                                Path = lp[i],
                                Text = ls[i]
                            };
                            _ctx.Pics.Add(p);
                        }
                    }
                    _ctx.SaveChanges();
                    ResetCraft();
                }
            }

        }

        private string ExtName(string s)
        {
            var i = s.Length - 1;
            while ((i >= 0) && (s[i] != '\"')) i--;
            return s.Substring(i + 1);
        }

        private bool Eof2()
        {
            return GFileI >= GFile.Count;
        }

        private string ReadLn2()
        {
            if (GFileI < GFile.Count)
                return GFile[GFileI++];
            return "";
        }

        public void Load5(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            _ctx = ctx;
            imagesPath = _imagesPath;

            _lInfo = lInfo;
            if (!(MessageBox.Show("Load Aerofiles?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;


            artId = ctx.Arts.Where(x => x.Name == "Aerofiles").Single().ArtId;

            ls = new List<string>();
            lp = new List<string>();

            ctx.Database.ExecuteSqlCommand("Delete From dbo.Pics From dbo.Pics p join dbo.Crafts c on c.CraftId = p.CraftId where c.Source = '5'");
            ctx.Database.ExecuteSqlCommand("Delete From dbo.Crafts where Source = '5'");
            ctx.Database.Connection.Close();
            ctx.Database.Connection.Open();

            var maxCraftId = _ctx.Crafts.Max(c => c.CraftId);
            using (var G = File.CreateText(_imagesPath + "Images5.lst"))
            {

                var files = Directory.GetFiles("D:\\download\\aerofiles.com", "_*.html");
                foreach (var file in files)
                {
                    _lInfo.Text = file;
                    Application.DoEvents();

                    for (q = 1; q < 3; q++) {

                        s = File.ReadAllText(file, Encoding.GetEncoding("UTF-8"));
                        GFile = s.Split('\n').ToList();
                        GFileI = 0;

                        was_first = false;

                        sc = ""; cnt = 0;
                        var srName = Path.GetFileName(file);
                        if (srName.Length > 8) {
                            s = FindString("<TITLE", true).Trim();
                            s = StrNoTags(s);
                            if (s != "")
                            {
                                if (s.Substring(1).StartsWith("merican airplanes")) s = s.Substring(19).Trim();
                                if (s.Length > 2 && s[s.Length - 2] == '-') 
                                    s = s.Substring(0, s.Length - 4).Trim();
                                if (s.Length > 5 && s.Substring(s.Length - 5, 4) == " to ") 
                                    s = s.Substring(0, s.Length - 7).Trim();
                                if (s.Length > 7 && s.Substring(s.Length - 7, 6) == " page ") 
                                    s = s.Substring(0, s.Length - 7).Trim();
                            }
                            i = s.IndexOf("-hyphenates");
                            if (i >= 0) 
                                s = s.Substring(0, i);
                            sc = s;
                        }
                        sc1 = s;

                        while (!Eof2()) {
                            s = ReadLn2();

                            su = s.ToUpper().Trim();
                            if (((s.IndexOf(".jpg") >= 0) && su.StartsWith("<A HREF")) || (s.IndexOf(" = ") >= 0)) break;

                            if (srName.Length <= 8) {
                                if (FindFirm(su)) {
                                    ss = StrNoTags(s);
                                    if (!string.IsNullOrEmpty(ss)) {
                                        sc = ss;
                                        sc1 = sc;
                                        s = ReadLn2();
                                        su = s.ToUpper();
                                        was_first = false;
                                    }
                                }
                            }

                        }

                        ResetCraft();

                        while (!Eof2())
                        {

                            i = s.IndexOf(".jpg");
                            if (i < 0)
                            {
                                i = s.ToLower().IndexOf(".gif");
                                if (i >= 0 && s.ToLower().IndexOf("cleardot.gif") >= 0)
                                {
                                    i = -1;
                                }
                            }
                            while (i >= 0) {
                                if (/*((q=2) or(Length(sr.Name) <= 8)) and*/(bq == 0) && (stt != ""))
                                    SaveCraft(ref maxCraftId);

                                ss = ExtName(s.Substring(0, i + 4));
                                if (ss.StartsWith("logo-")) {
                                    SaveCraft(ref maxCraftId);
                                    i = s.IndexOf("alt=");
                                    if (i < 0) {
                                        i = ss.IndexOf(".");
                                        ss = ss.Substring(5, i - 6);
                                        var sb = new StringBuilder(ss);
                                        sb[0] = sb[0].ToString().ToUpper()[0];
                                        ss = sb.ToString();
                                    } else {
                                        ss = s.Substring(i + 4);
                                        i = ss.IndexOf("logo");
                                        if (i < 0) {
                                            i = ss.IndexOf("\"");
                                            if (i < 0) {
                                                ss = sc;
                                            } else {
                                                ss = ss.Substring(0, i - 1).Trim();
                                            }
                                        } else {
                                            ss = ss.Substring(0, i - 1).Trim();
                                        }
                                    }
                                    sc = ss;
                                    sc1 = sc;
                                }

                                i = s.IndexOf(".jpg");
                                if (i < 0)
                                {
                                    i = s.ToLower().IndexOf(".gif");
                                    if (i >= 0 && s.ToLower().IndexOf("cleardot.gif") >= 0)
                                    {
                                        i = -1;
                                    }
                                }
                                ss = ExtName(s.Substring(0, i + 4));
                                lp.Add(ss);
                                G.WriteLine("http://www.aerofiles.com/" + ss);

                                ss = "";
                                st = StrNoTags(s);

                                s = ReadLn2();

                                su = s.ToUpper();
                                while ((s.IndexOf(".gif") < 0) && (su.IndexOf("<SPACER") < 0) &&
                                    (!FindFirm(su)) && (FindCraft(s) < 0) && !Eof2()) {
                                    st = StrNoTags(s);
                                    if (st != "")
                                        ss = (ss + " " + st).Trim();

                                    s = ReadLn2();
                                    su = s.ToUpper();
                                }
                                ls.Add(ss);

                                i = s.IndexOf(".jpg");
                                if (i < 0)
                                {
                                    i = s.ToLower().IndexOf(".gif");
                                    if (i >= 0 && s.ToLower().IndexOf("cleardot.gif") >= 0)
                                    {
                                        i = -1;
                                    }
                                }
                            }

                            if (FindFirm(su)) {
                                ss = StrNoTags(s);
                                if ((ss != "") && (!ss.StartsWith("SEE"))) {

                                    ResetCraft();

                                    sc = ss;
                                    sc1 = sc;

                                    s = ReadLn2();
                                    su = s.ToUpper();
                                    was_first = false;

                                }
                            }


                            st = StrNoTags(s);

                            ss = st + " ";
                            i = FindCraft(ss);
                            new_craft = false;
                            if (i >= 0) {

                                if (/*{((q=2) or(Length(sr.Name) <= 8)) and}*/(bq == 0) && (stt != "")) {
                                    SaveCraft(ref maxCraftId);
                                    ss = st + " ";
                                }

                                new_craft = true;
                                stt += "\n\n\n";
                                for (j = 0; j < bq; j++)
                                    stt += ". . ";

                                if (iy == 0) {
                                    was_first = true;
                                    was_year = true;
                                    j = i;
                                    while ((ss[j] == ' ') && (j > 3)) j--;
                                    if (ss[j] == ')') {
                                        while ((ss[j] != '(') && (j > 3)) j--;
                                        j--;
                                        while ((ss[j] == ' ') && (j > 3)) j--;
                                    }
                                    if ((IsDigit(ss[j]) || (ss[j] == '?')) &&
                                        (IsDigit(ss[j - 1]) || (ss[j - 1] == '?')) &&
                                        (IsDigit(ss[j - 2]) || (ss[j - 2] == '?')) &&
                                        (IsDigit(ss[j - 3]) || (ss[j - 3] == '?'))) {

                                        try
                                        {
                                            iy = int.Parse(ss.Substring(j - 3, 4));
                                        } catch { }

                                        if (sn == "") {
                                            ss = ss.Substring(0, j - 3).Trim();
                                            if (ss.EndsWith("c."))
                                                ss = ss.Substring(0, ss.Length - 2).Trim();
                                            sn = ss;
                                        }
                                    } else {
                                        sn = ss.Substring(0, i).Trim();
                                    }
                                    if (sn != "")
                                        if (sn[0] == '-') {
                                            i = sn.IndexOf(" ");
                                            s = i > 0 ? sn.Substring(0, i) : sn;
                                            i = sc.IndexOf(s);
                                            if (i >= 0) {
                                                j = i;
                                                while ((j >= 0) && (sc[j] != ',')) j--;
                                                ss = sc.Substring(j + 1, i - j - 1).Trim();
                                                sc1 = ss + s;
                                                sn = ss + sn;
                                                st = ss + st;
                                            } else {
                                                sc1 = sc + s;
                                                sn = sc + sn;
                                                st = sc + st;
                                            }
                                        }
                                }
                            }

                            if (st != "") {
                                if (stt == "")
                                    stt = st;
                                else if (new_craft)
                                    stt = stt + st;
                                else
                                    stt = stt + "\n" + st;
                            }

                            ss = su;
                            i = ss.IndexOf("<BLOCKQUOTE");
                            while (i >= 0) {

                                bq++;
                                ss = ss.Substring(i + 1);
                                i = ss.IndexOf("<BLOCKQUOTE");
                            }

                            ss = su;
                            i = ss.IndexOf("</BLOCKQUOTE");
                            while (i >= 0) {

                                bq--;
                                ss = ss.Substring(i + 1);
                                i = ss.IndexOf("</BLOCKQUOTE");
                            }

                            if ((!was_first) && (stt != "")) {
                                ResetCraft();
                            }


                            if (su.IndexOf("<HR") >= 0) {
                                SaveCraft(ref maxCraftId);
                            }

                            s = ReadLn2();
                            if (s.IndexOf("home.html") >= 0) {
                                SaveCraft(ref maxCraftId);
                                break;
                            }
                            su = s.ToUpper();
                            if (FindFirm(su)) {
                                ss = StrNoTags(s);
                                if (ss != "") {

                                    ResetCraft();

                                    sc = ss;
                                    sc1 = sc;
                                    s = ReadLn2();
                                    su = s.ToUpper();
                                    was_first = false;

                                }
                            }

                        }
                        if (cnt > 0) break;
                    }
                    SaveCraft(ref maxCraftId);
                }

            }

            _lInfo.Text = "";
            MessageBox.Show("OK!");
        }

        public void Load6(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            _ctx = ctx;
            imagesPath = _imagesPath;

            _lInfo = lInfo;
            if (!(MessageBox.Show("Load AviaDejavu?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;


            _lInfo.Text = "Deleting...";
            Application.DoEvents();

            ctx.Database.ExecuteSqlCommand("delete from WordLinks where PicId in (select PicId from Pics where CraftId in (select CraftId from Crafts where source = '6'))");
            ctx.Database.ExecuteSqlCommand("delete from Links where Pict1 in (select PicId from Pics where CraftId in (select CraftId from Crafts where source = '6'))" +
              " or Pict2 in (select PicId from Pics where CraftId in (select CraftId from Crafts where source = '6'))");
            ctx.Database.ExecuteSqlCommand("delete from WordLinks where CraftId in (select CraftId from Crafts where source = '6')");
            ctx.Database.ExecuteSqlCommand("delete from pics where CraftId in (select CraftId from Crafts where source = '6')");
            ctx.Database.ExecuteSqlCommand("delete from Crafts where source = '6'");
            ctx.Database.ExecuteSqlCommand("delete from arts where artid in (select artid from vwArts where cnt = 0 and mag<> 'Fl')");
            ctx.Database.Connection.Close();
            ctx.Database.Connection.Open();
            Util.DetachAllEntities(ctx);


            _lInfo.Text = "Preparing...";
            Application.DoEvents();

            var datFile = File.ReadAllLines("D:\\Restore\\Indexes\\Crafts1.dat");
            var datList = new List<string>(datFile);
            var datSplitList = datList.Select(d => d.Split('~'));

            var maxCraftId = ctx.Crafts.Max(c => c.CraftId);
            var maxCraftIdLst = datSplitList.Max(datStr => int.Parse(datStr[18]));
            if (maxCraftId < maxCraftIdLst) maxCraftId = maxCraftIdLst;

            var sameCrafts = new Dictionary<string, int>();

            foreach (var datStr in datSplitList)
            {
                var craft = new Crafts()
                {
                    CraftId = datStr[17] == "" ? int.Parse(datStr[18]) : ++maxCraftIdLst,
                    Source = "6",
                    Country = datStr[0],
                    Construct = datStr[2],
                    Name = datStr[3],
                    Vert = datStr[5].Contains("В"),
                    Uav = datStr[5].Contains("Б"),
                    Glider = datStr[5].Contains("П"),
                    Single = datStr[5].Contains("1"),
                    LL = datStr[5].Contains("Л"),
                    Proj = datStr[5].Contains("Х"),
                    Wings = datStr[6],
                    Engines = datStr[7],
                    Type = datStr[8],
                    Wiki = datStr[13],
                    Airwar = datStr[14],
                };

                if (datStr[4] != "")
                    craft.IYear = int.Parse(datStr[4]);
                if (datStr[15] != "")
                {
                    var craft7Id = int.Parse(datStr[15]);
                    var craft7 = ctx.Crafts.FirstOrDefault(c => c.CraftId == craft7Id);
                    if (craft7 != null)
                        craft.FlyingM = craft7Id;
                }

                _ctx.Crafts.Add(craft);
                _ctx.SaveChanges();
                Util.DetachAllEntities(ctx);

                if (datStr[17] != "")
                {
                    var idStr = $"{craft.Country}~{craft.Construct}~{craft.Name}~{craft.IYear}";
                    sameCrafts[idStr] = craft.CraftId;
                }

                var text = $"{craft.Country}~{craft.Construct}";
                if (text != _lInfo.Text)
                {
                    _lInfo.Text = text;
                    Application.DoEvents();
                }

            }

            foreach (var datStr in datSplitList)
            {
                if (datStr[17] != "")
                {
                    var idStr = $"{datStr[0]}~{datStr[2]}~{datStr[3]}~{datStr[4]}";
                    var mainId = sameCrafts[idStr];
                    var main = _ctx.Crafts.First(c => c.CraftId == mainId);
                    main.Same = int.Parse(datStr[18]);
                    _ctx.SaveChanges();
                    Util.DetachAllEntities(ctx);

                    var text = $"Same: {main.Country}~{main.Construct}";
                    if (text != _lInfo.Text)
                    {
                        _lInfo.Text = text;
                        Application.DoEvents();
                    }
                }
            }

            var artFile = File.ReadAllLines("D:\\Restore\\Indexes\\Arts2.htm");
            var artList = new List<string>(artFile);
            var i = 0;
            while (i < artList.Count)
            {
                if (FindString("<tr>", artList, ref i))
                {
                    var art = new Arts();

                    s = artList[i + 1];
                    var j = s.IndexOf("/Arts/Art");
                    var k = s.IndexOf(".htm");
                    art.ArtId = int.Parse(s.Substring(j + 9, k - j - 9));

                    var yearMonth = UntagString(s).Trim();
                    j = yearMonth.IndexOf("-");
                    if (j > 0)
                    {
                        art.IYear = int.Parse(yearMonth.Substring(0, j));
                        art.IMonth = yearMonth.Substring(j + 1);
                    }
                    else
                    {
                        art.IYear = int.Parse(yearMonth);
                    }

                    var magStr = UntagString(artList[i + 3]).Trim();
                    art.Mag = DecodeMag(magStr);

                    magStr = UntagString(artList[i + 5]).Trim();
                    art.Author = magStr;

                    magStr = UntagString(artList[i + 7]).Trim();
                    art.Name = magStr;

                    magStr = UntagString(artList[i + 9]).Trim();
                    art.Serie = magStr;

                    magStr = UntagString(artList[i + 11]).Trim();
                    if (magStr != "")
                        art.NN = int.Parse(magStr);

                    _ctx.Arts.Add(art);
                    _ctx.SaveChanges();
                    Util.DetachAllEntities(ctx);

                    var text = $"{art.Mag}~{art.IYear}";
                    if (text != _lInfo.Text)
                    {
                        _lInfo.Text = text;
                        Application.DoEvents();
                    }
                    i += 15;
                }

            }


            var nnn = ctx.Pics.Max(p => p.PicId) + 1;
            var nx = ctx.Pics.Max(p => p.NNN ?? 0) + 1;
            if (nnn < nx) nnn = nx;

            var files = Directory.GetFiles("D:\\Restore\\Crafts").OrderBy(x => x).ToList();
            //files = files.Where(f => f.Contains("20622")).ToList();
            foreach (var file in files)
            {
                var xfile = file;
                var sfile = file;
                i = xfile.IndexOf("-1.");
                if (i > 0)
                {
                    sfile = xfile.Substring(0, i) + ".htm";
                    if (files.Any(x => x == sfile))
                    {
                        xfile = sfile;
                    }
                }
                else if (xfile.IndexOf("-") < 0)
                {
                    sfile = xfile.Substring(0, xfile.Length - 4) + "-1.htm";
                    if (files.Any(x => x == sfile))
                    {
                        xfile = sfile;
                    }
                }

                var craftFile = File.ReadAllLines(xfile);
                var craftPage = new List<string>(craftFile);

                var nn = 3;
                sfile = xfile;
                i = sfile.IndexOf('-');
                if (i > 0)
                {
                    nn = int.Parse(sfile.Substring(i + 1, 1));
                    sfile = sfile.Substring(0, i) + sfile.Substring(i + 2);
                }
                else
                {
                    s = sfile.Substring(0, sfile.Length - 4) + "-";
                    if (files.Any(x => x.StartsWith(s)))
                    {
                        nn = 2;
                    }
                }
                var craftId = GetCraftId(sfile);

                var mainCraft = ctx.Crafts.FirstOrDefault(c => c.CraftId == craftId);

                if (mainCraft != null)
                {
                    i = 0;
                    if (FindString("<span class=\"ru small\">Варианты:</span>", craftPage, ref i, true))
                    {
                        Crafts prevCraft = mainCraft;
                        Crafts nextCraft = null;
                        while (FindString("<div class=\"w500\">", craftPage, ref i, true))
                        {
                            var nextCraftId = GetCraftId(craftPage[i + 1]);
                            nextCraft = ctx.Crafts.First(c => c.CraftId == nextCraftId);
                            nextCraft.SeeAlso = prevCraft.CraftId;
                            prevCraft = nextCraft;

                            i++;
                        }
                        if (nextCraft != null)
                        {
                            mainCraft.SeeAlso = nextCraft.CraftId;
                            _ctx.SaveChanges();
                        }
                    }

                    var k = 0;
                    i = 0;
                    var fullDescr = "";
                    while (FindString($"<div id=\"ltabs-{k}\"", craftPage, ref i, false, true))
                    {
                        i++;
                        FindString($"<div id=\"tabs-{k}\"", craftPage, ref i, false, true);
                        var i0 = i;
                        FindString($"</div>", craftPage, ref i);
                        var descr = "";
                        while(++i0 < i)
                        {
                            descr += craftPage[i0];
                        }
                        fullDescr += descr;

                        k++;
                        i = 0;
                    }
                    var isHead = fullDescr.Contains("<h3>");
                    fullDescr = fullDescr
                        .Replace("<br>", "\n")
                        .Replace("&nbsp;", " ")
                        .Replace("<h3>", "*/\n\n{")
                        .Replace("</h3>", "}\n\n/*")
                        .Replace("<span class=\"ru\">Дальше</span><span class=\"en\" style=\"display:none\">More</span>&gt;&gt;&gt;</span>", "");
                    if (isHead)
                    {
                        var n = fullDescr.IndexOf("*/\n\n{");
                        fullDescr = fullDescr.Substring(0, n) + fullDescr.Substring(n + 4) + "*/";
                    }
                    fullDescr = UntagString(fullDescr);

                    if (!string.IsNullOrEmpty(fullDescr))
                    {
                        mainCraft.CText = fullDescr;
                        _ctx.SaveChanges();
                    }


                    i = 0;
                    if (FindString("<div id=\"all_pics\"", craftPage, ref i, false, true))
                    {
                        var grp = "";
                        while (i < craftPage.Count)
                        {
                            if (craftPage[i].StartsWith("<div class=\"pic_group"))
                            {
                                grp = UntagString(craftPage[i]).Replace("&nbsp;", "-");
                            }

                            if (craftPage[i].Contains("<div class=\"lite\">"))
                            {
                                i += 2;
                                s = craftPage[i];
                                j = s.IndexOf("Images6/");
                                k = s.IndexOf(".jpg");
                                var picPath = s.Substring(j + 8, k - j - 4);

                                var pic = new Pics()
                                {
                                    CraftId = craftId,
                                    Path = picPath.Replace("/", "\\"),
                                    NNN = nnn++,
                                    Grp = grp
                                };

                                j = picPath.LastIndexOf("/");
                                picPath = picPath.Substring(j + 1);
                                picPath = picPath.Substring(0, picPath.Length - 4);
                                j = picPath.IndexOf("-");
                                if (j >= 1 && j <= 4 && (picPath.Length - j - 1) <= 5)
                                {
                                    pic.XPage = picPath.Substring(0, j);
                                    pic.NN = picPath.Substring(j + 1);
                                }

                                var type = "f";
                                switch(nn)
                                {
                                    case 1: type = "s"; break;
                                    case 2: type = "fc"; break;
                                    case 3: type = "f"; break;
                                    case 4: type = "c"; break;
                                    case 5: type = "fd"; break;
                                    case 6: type = "p"; break;
                                };
                                pic.Type = type;
                                pic.NType = Util.GetNType(type);

                                i += 5;
                                s = "";
                                while (craftPage[i].Trim() != "</div>")
                                {
                                    s += craftPage[i];
                                    i++;
                                }

                                pic.Text = s
                                    .Replace("<br>", "\n")
                                    .Replace("&nbsp;", " ")
                                    .Trim();

                                i += 2;
                                s = craftPage[i];
                                j = s.IndexOf("/Arts/Art");
                                k = s.IndexOf(".htm");
                                s = s.Substring(j + 9, k - j - 9);
                                pic.ArtId = int.Parse(s);


                                ctx.Pics.Add(pic);
                                ctx.SaveChanges();

                                grp = "";
                            }

                            i++;
                        }
                    }


                    var text = $"{mainCraft.CraftId}~{mainCraft.Country}~{mainCraft.Construct}~{mainCraft.Name}";
                    if (text != _lInfo.Text)
                    {
                        _lInfo.Text = text;
                        Application.DoEvents();
                    }
                    Util.DetachAllEntities(ctx);
                }

            }

            _lInfo.Text = "";
            MessageBox.Show("OK!");
        }

        public void Load7(AiKEntities ctx, string _imagesPath, Label lInfo)
        {
            _ctx = ctx;
            imagesPath = _imagesPath;

            _lInfo = lInfo;
            if (!(MessageBox.Show("Load FlyingMachines?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;


            _lInfo.Text = "Deleting...";
            Application.DoEvents();

            ctx.Database.ExecuteSqlCommand("delete from WordLinks where PicId in (select PicId from Pics where CraftId in (select CraftId from Crafts where source = '7'))");
            ctx.Database.ExecuteSqlCommand("delete from Links where Pict1 in (select PicId from Pics where CraftId in (select CraftId from Crafts where source = '7'))" +
              " or Pict2 in (select PicId from Pics where CraftId in (select CraftId from Crafts where source = '7'))");
            ctx.Database.ExecuteSqlCommand("delete from WordLinks where CraftId in (select CraftId from Crafts where source = '7')");
            ctx.Database.ExecuteSqlCommand("delete from pics where CraftId in (select CraftId from Crafts where source = '7')");
            ctx.Database.ExecuteSqlCommand("update Crafts set FlyingM = null");
            ctx.Database.ExecuteSqlCommand("delete from Crafts where source = '7'");
            ctx.Database.ExecuteSqlCommand("delete from arts where artid in (select artid from vwArts where cnt = 0)");
            ctx.Database.Connection.Close();
            ctx.Database.Connection.Open();
            Util.DetachAllEntities(ctx);

            _lInfo.Text = "Preparing...";
            Application.DoEvents();

            var datFile = File.ReadAllLines("D:\\Restore\\Site2\\Indexes\\Crafts1.htm");
            var datList = new List<string>(datFile);

            var i = 0;
            while (i < datList.Count)
            {
                if (datList[i] == "<div class=\"tablerow\">")
                {

                    var craft = new Crafts()
                    {
                        CraftId = GetCraftId(datList[i + 1]),
                        Source = "7",
                        Country = UntagString(datList[i + 1]),
                        Construct = UntagString(datList[i + 2]),
                        Name = UntagString(datList[i + 3]),
                        Vert = datList[i + 5].Contains("Вертолет"),
                        Glider = datList[i + 5].Contains("Планер"),
                        Type = UntagString(datList[i + 6]).Replace("&nbsp;", "")
                    };

                    var s = UntagString(datList[i + 4]);
                    if (s != "&nbsp;")
                        craft.IYear = int.Parse(s);

                    _ctx.Crafts.Add(craft);
                    _ctx.SaveChanges();
                    Util.DetachAllEntities(ctx);

                    var text = $"{craft.Country}~{craft.Construct}";
                    if (text != _lInfo.Text)
                    {
                        _lInfo.Text = text;
                        Application.DoEvents();
                    }
                }
                i++;
            }

            var srcFile = File.ReadAllLines("D:\\Restore\\Site2\\Sources.txt");
            var srcList = new List<string>(srcFile);
            var srcNamesFile = File.ReadAllLines("D:\\Restore\\Site2\\SourceNames.txt");
            var srcNamesList = new List<string>(srcNamesFile);
            var srcNames2File = File.ReadAllLines("D:\\Restore\\Site2\\SourceNames2.txt");
            var srcNames2List = new List<string>(srcNames2File);
            var srcIdsFile = File.ReadAllLines("D:\\Restore\\Site2\\SourceIds.txt");
            var srcIdsList = new List<string>(srcIdsFile);
            var srcMagFile = File.ReadAllLines("D:\\Restore\\Site2\\SourceMag.txt");
            var srcMagList = new List<string>(srcMagFile);

            var artFile = File.ReadAllLines("D:\\Restore\\Site2\\Indexes\\Arts2.htm");
            var artList = new List<string>(artFile);
            i = 0;
            while (i < artList.Count)
            {
                if (artList[i] == "<div class=\"tablerow\">")
                {
                    s = artList[i + 1];
                    var j = s.IndexOf("/Arts/Art");
                    var k = s.IndexOf(".htm");
                    var artId = int.Parse(s.Substring(j + 9, k - j - 9));
                    var serie = UntagString(artList[i + 1]).Replace("&nbsp;", "");
                    var serie0 = serie;
                    var author = UntagString(artList[i + 2]).Replace("&nbsp;", "");
                    var name = UntagString(artList[i + 3]).Replace("&nbsp;", "");
                    var fullName = $"{author} {name}".Trim();
                    var iyear = 0;
                    if (name.EndsWith(" г."))
                    {
                        var iy = int.Parse(name.Substring(name.Length - 7, 4));
                        if (iy < 1920) iyear = iy;
                    }
                    if (serie == "Centennial Perspective")
                    {
                        for (j = 1; j < 60; j++)
                        {
                            var serx = $"A Centennial Perspective on Great War Airplanes {j}";
                            s = fullName + $" ({serx})";
                            if (srcNamesList.Contains(s))
                            {
                                serie = serx;
                                iyear = j;
                                break;
                            }
                        }
                    }
                    if (serie != "")
                    {
                        fullName += $" ({serie})";
                    }

                    var art = new Arts()
                    {
                        ArtId = artId,
                        Name = name,
                        Author = author,
                        Serie = serie0
                    };

                    if (iyear > 0) art.IYear = iyear;

                    j = srcNamesList.IndexOf(fullName);
                    if (j < 0 && fullName != "")
                    {
                        MessageBox.Show($"Art not found: {fullName}");
                    }
                    else if (j >= 0)
                    {
                        art.Mag = srcMagList[j];
                    }

                    _ctx.Arts.Add(art);
                    _ctx.SaveChanges();
                    Util.DetachAllEntities(ctx);

                    var text = $"{art.Mag}~{art.IYear}";
                    if (text != _lInfo.Text)
                    {
                        _lInfo.Text = text;
                        Application.DoEvents();
                    }
                }
                i++;
            }

            
            var nnn = ctx.Pics.Max(p => p.PicId) + 1;
            var nx = ctx.Pics.Max(p => p.NNN ?? 0) + 1;
            if (nnn < nx) nnn = nx;

            var files = Directory.GetFiles("D:\\Restore\\Site2\\Crafts").OrderByDescending(x => x).ToList();
            var bads = new List<string>();
            foreach (var file in files)
            {
                var craftFile = File.ReadAllLines(file);
                var craftPage = new List<string>(craftFile);

                var craftId = GetCraftId(file);

                var mainCraft = ctx.Crafts.FirstOrDefault(c => c.CraftId == craftId);

                if (mainCraft != null)
                {

                    var kk = 0;
                    i = 0;
                    var fullDescr = "";
                    while (FindString($"<div id='tabs-{kk}'", craftPage, ref i, false, true))
                    {
                        var i0 = i;
                        FindString($"</div>", craftPage, ref i);
                        var descr = "";
                        while (i0 < i)
                        {
                            descr += craftPage[i0];
                            i0++;
                        }

                        j = descr.IndexOf("<h3>");
                        var k = descr.IndexOf("</h3>");
                        var artName = UntagString(descr.Substring(j + 4, k - j - 4));
                        j = srcNames2List.IndexOf(artName);
                        if (j < 0)
                        {
                            if (!bads.Contains(artName)) bads.Add(artName);
                        } else
                        {
                            var descrx = descr.Substring(k + 5);
                            if (descrx.StartsWith("<br>")) descrx = descrx.Substring(4);
                            descr = $"{{{srcList[j]}}}\n\n/*{descrx}*/\n\n\n\n";
                        }

                        fullDescr += descr;

                        kk++;
                        i = 0;
                    }
                    fullDescr = fullDescr
                        .Replace("<br>", "\n")
                        .Replace("&nbsp;", " ");
                    fullDescr = UntagString(fullDescr);

                    if (!string.IsNullOrEmpty(fullDescr))
                    {
                        mainCraft.CText = fullDescr;
                        _ctx.SaveChanges();
                    }


                    i = 0;
                    if (FindString("<table id=\"all_pics\"", craftPage, ref i, true, true))
                    {
                        var grp = "";
                        while (i < craftPage.Count)
                        {
                            if (craftPage[i].Contains("<td class=\"preview\""))
                            {
                                s = craftPage[i];
                                j = s.LastIndexOf("Images7/");
                                var k = s.IndexOf(".jpg", j);
                                if (k < 0) k = s.IndexOf(".gif", j);
                                if (k < 0) k = s.IndexOf(".GIF", j);
                                if (k < 0) k = s.IndexOf(".JPG", j);
                                var picPath = s.Substring(j + 8, k - j - 4);

                                var pic = new Pics()
                                {
                                    CraftId = craftId,
                                    Path = picPath.Replace("/", "\\"),
                                    NNN = nnn++,
                                    Grp = grp
                                };

                                j = picPath.LastIndexOf("/");
                                picPath = picPath.Substring(j + 1);
                                picPath = picPath.Substring(0, picPath.Length - 4);
                                j = picPath.IndexOf("-");
                                if (j >= 1 && j <= 4 && (picPath.Length - j - 1) <= 5)
                                {
                                    pic.XPage = picPath.Substring(0, j);
                                    pic.NN = picPath.Substring(j + 1);
                                }

                                var type = "f";
                                pic.Type = type;
                                pic.NType = Util.GetNType(type);

                                i += 2;
                                s = craftPage[i];
                                j = s.IndexOf("/Arts/Art");
                                k = s.IndexOf(".htm");
                                s = s.Substring(j + 9, k - j - 9);
                                pic.ArtId = int.Parse(s);

                                i += 4;
                                s = "";
                                while (craftPage[i].Trim() != "</td>")
                                {
                                    s += craftPage[i];
                                    i++;
                                }

                                s = s
                                    .Replace("<br>", "\n")
                                    .Replace("&nbsp;", " ")
                                    .Trim();
                                pic.Text = UntagString(s);


                                ctx.Pics.Add(pic);
                                ctx.SaveChanges();

                                grp = "";
                            }

                            i++;
                        }
                    }


                    var text = $"{mainCraft.CraftId}~{mainCraft.Country}~{mainCraft.Construct}~{mainCraft.Name}";
                    if (text != _lInfo.Text)
                    {
                        _lInfo.Text = text;
                        Application.DoEvents();
                    }
                    Util.DetachAllEntities(ctx);
                }

            }

            _lInfo.Text = "";
            MessageBox.Show($"OK!\n{string.Join("\n", bads)}");
        }


        private bool FindString(string str, List<string> page, ref int i, bool isTrim = false, bool isStarting = false)
        {
            while (i < page.Count)
            {
                var s = page[i];
                if (isTrim) s = s.Trim();
                if ((isStarting && s.StartsWith(str)) || (!isStarting && s == str)) return true;
                i++;
            }
            return false;
        }

        private string UntagString(string str)
        {
            var res = "";
            var inTag = false;
            var i0 = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == '<' && (str.Length < (i + 5) || str.Substring(i, 5) != "<...>"))
                {
                    inTag = true;
                    if (i0 != i)
                    {
                        res += str.Substring(i0, i - i0);
                    }
                }
                else if (inTag && str[i] == '>')
                {
                    inTag = false;
                    i0 = i + 1;
                };
            }
            if (i0 < str.Length) res += str.Substring(i0);
            return res;
        }

        private string DecodeMag(string str)
        {
            if (str.StartsWith("Air Enthusiast")) return "AE";
            if (str.StartsWith("АэроХобби")) return "AH";
            if (str.StartsWith("Air Pictorial")) return "AI";
            if (str.StartsWith("Авиация и Космонавтика")) return "AK";
            if (str.StartsWith("АвиаМастер")) return "AM";
            if (str.StartsWith("Air International")) return "AN";
            if (str.StartsWith("АвиаПарк")) return "AP";
            if (str.StartsWith("АС")) return "AS";
            if (str.StartsWith("Авиация и Время")) return "AV";
            if (str.StartsWith("Flight")) return "FT";
            if (str.StartsWith("Aviation Historian")) return "HI";
            if (str.StartsWith("История Авиации")) return "IA";
            if (str.StartsWith("In Action")) return "IN";
            if (str.StartsWith("Jane's All the World Aircraft")) return "JS";
            if (str.StartsWith("Мир Авиации")) return "MA";
            if (str.StartsWith("My photos")) return "ME";
            if (str.StartsWith("Моделист-Конструктор")) return "MK";
            if (str.StartsWith("Мировая Авиация")) return "MM";
            if (str.StartsWith("Aeroplane Monthly")) return "MY";
            if (str.StartsWith("Изд-во Osprey")) return "OS";
            if (str.StartsWith("Изд-во Schiffer")) return "SC";

            MessageBox.Show($"Unknown mag {str}");
            return "";
        }

        private int GetCraftId(string str)
        {
            var j = str.IndexOf(".htm");
            str = str.Substring(0, j + 4);
            var i = str.LastIndexOf("Craft");
            var s = str.Substring(i + 5, j - i - 5);
            return int.Parse(s);
        }
    }
}
