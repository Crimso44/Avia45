using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

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
                .Replace("\r\n"," ")
                .Replace("<br>","\n\n")
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

            ctx.Database.ExecuteSqlCommand("Delete From dbo.Pics From dbo.Pics p join dbo.Crafts c on c.CraftId = p.CraftId where c.Source = '1'");
            ctx.Database.ExecuteSqlCommand("Delete From dbo.Crafts where Source = '1'");
            ctx.Database.Connection.Close();
            ctx.Database.Connection.Open();

            var artId = ctx.Arts.Where(x => x.Name == "Airwar").Single().ArtId;

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
                            s = File.ReadAllText(file, Encoding.GetEncoding("KOI8-R"));
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
    }
}
