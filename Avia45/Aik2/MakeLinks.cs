using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aik2
{
    public class TRec
    {
        public int id;
        public int iyear;
        public int iconstr;
        public int isrc;
        public string name;
        public string scntr;
        public List<string> ls;
        public bool bv;
        public bool bc;

        public TRec(Crafts T)
        {
            ls = new List<string>();
            id = T.CraftId;
            iyear = T.IYear ?? 0;
            scntr = T.Country;
            isrc = int.Parse(T.Source);
            bv = T.Vert ?? false;
            bc = !bv && isrc != 5;
            name = LoadNameStr(T);
            var s = $" {T.Construct?.ToUpper()} ".Replace(" ОКБ ", " ").Trim();
            GetTokens(s, ls);
            iconstr = ls.Count;
            s = T.Name.ToUpper();
            GetTokens(s, ls);
        }

        private string LoadNameStr(Crafts T) {
            var s = T.Name;
            var ss = T.Construct;
            if (!string.IsNullOrEmpty(ss)) s = $"{ss} - {s}";
            ss = T.IYear.ToString();
            if (!string.IsNullOrEmpty(ss)) s += $" - {ss}";
            ss = T.Country;
            if (!string.IsNullOrEmpty(ss)) s += $" - {ss}";
            ss = T.Type;
            if (!string.IsNullOrEmpty(ss)) s += $" - {ss}";
            s = $"{T.Source} - {s}";
            return s;
        }

        private void GetTokens(string s, List<string> ls) {
            var ss = "";
            for (var i = 0; i < s.Length; i++) {
                if (Char.IsLetterOrDigit(s[i])) {
                    ss += s[i];
                }
                else if (!string.IsNullOrEmpty(ss)) {
                    ss = RusToLatin(ss);
                    if (!ls.Contains(ss)) ls.Add(ss);
                    ss = "";
                }
            }
            if (!string.IsNullOrEmpty(ss)) {
                ss = RusToLatin(ss);
                if (!ls.Contains(ss)) ls.Add(ss);
            }
        }

        public static string RusToLatin(string s, bool kill_spaces = true) {
            const string rus = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            //const string lat = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghujklmnopqrstuvwxyz";
            const string rus1 = "АБВГДЕЗИЙКЛМНОПРСТУФЫЭабвгдезийклмнопрстуфыэ";
            const string lat1 = "ABVGDEZIJKLMNOPRSTUFYEabvgdezijklmnoprstufye";
            const string rus2 = "ЖХЦЧШЩЮЯжхцчшщюя";
            const string lat2 = "ZH;KH;TS;CH;SH;SCH;IU;YA;zh;kh;ts;ch;sh;sch;iu;ya";
            var latX = lat2.Split(';');
        
            var Result = "";
            for (var i = 0; i < s.Length; i++)
            {
                if (rus.Contains(s[i]))
                {
                    var j = rus1.IndexOf(s[i]);
                    if (j >= 0)
                    {
                        Result += lat1[j];
                    }
                    else
                    {
                        j = rus2.IndexOf(s[i]);
                        if (j >= 0)
                        {
                            Result += latX[j];
                        }
                    }
                }
                else
                {
                    if ((!kill_spaces) || Char.IsLetterOrDigit(s[i]))
                    {
                        Result += s[i];
                    }
                }
            }
            return Result;
        }
    }

    public class MakeLinks
    {
        private List<TRec>[] Recs = new List<TRec>[8];

        public void DoMakeLinks(AiKEntities ctx, Label lInfo, int ind)
        {
            if (!(MessageBox.Show("Load Links?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)) return;

            var ld = new List<string>();

            for (var i = 1; i <= 7; i++) Recs[i] = new List<TRec>();

            lInfo.Text = "Loading...";
            Application.DoEvents();

            var crafts = ctx.Crafts.Where(x => "1234567".Contains(x.Source)).ToList();
            foreach (var craft in crafts)
            {
                var r = new TRec(craft);
                Recs[r.isrc].Add(r);
            }

            var icnt = 0;
            var cnt = Recs[1].Count +
                Recs[2].Count +
                Recs[3].Count +
                Recs[4].Count +
                Recs[5].Count +
                Recs[6].Count +
                Recs[7].Count;
            for (var i = 1; i <= 7; i++) {
                foreach (var r in Recs[i]) {
                    icnt++;
                    lInfo.Text = $"{icnt} of {cnt}";
                    Application.DoEvents();

                    var craft = ctx.Crafts.Single(x => x.CraftId == r.id);
                    for (var k = 1; k <= 7; k++) {
                        if ((ind == 0) || (ind == i) || (ind == k)) {
                            if (i == k) {
                                craft.GetType().GetProperty($"s{k}").SetValue(craft, r.id);
                            } else {
                                craft.GetType().GetProperty($"s{k}").SetValue(craft, null);
                                var maxcmp = Int32.MinValue;
                                var maxyr = 0;
                                var maxid = 0;
                                ld.Clear();
                                foreach (var r1 in Recs[k]) {
                                    var m = CompareRecs(r, r1);
                                    if (m > Int32.MinValue) {
                                        if (m > maxcmp) {
                                            maxcmp = m;
                                            maxid = r1.id;
                                            maxyr = r1.iyear;
                                            ld.Clear();
                                        } else if (m == maxcmp) {
                                            ld.Add($"{r1.name} ({m})");
                                            if ((r1.iyear > 0) && ((maxyr > r1.iyear) || (maxyr == 0))) {
                                                maxcmp = m;
                                                maxid = r1.id;
                                                maxyr = r1.iyear;
                                                ld.Move(ld.Count - 1, 0);
                                            }
                                        }
                                    }
                                }
                                if (maxcmp > Int32.MinValue) {
                                    craft.GetType().GetProperty($"s{k}").SetValue(craft, maxid);
                                }
                            }
                        }
                    }
                    ctx.SaveChanges();
                    Util.DetachAllEntities(ctx);
                }
            }

            lInfo.Text = "";
            MessageBox.Show("OK!");
        }

        private bool is_constr;
        private bool is_name;
        private int res;
        private bool is_was_was;

        private int CompareRecs(TRec r, TRec r1) {
            var Result = Int32.MinValue;
            if ((r.iyear > 0) && (r1.iyear > 0) && (Math.Abs(r.iyear - r1.iyear) > 9)) return Result;
            if ((r.bv && r1.bc) || (r.bc && r1.bv)) return Result;
            var s1 = r.scntr; var s2 = r1.scntr;
            if (!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2)) {
                if (s1 == "International") s1 = "";
                if (s2 == "International") s2 = "";
                if (s1.Length > 3 && ("СC".IndexOf(s1[0]) >= 0) && ("СC".IndexOf(s1[1]) >= 0) && ("СC".IndexOf(s1[2]) >= 0) && ("РP".IndexOf(s1[3]) >= 0)) {
                    s1 = "Россия";
                }
                if (s2.Length > 3 && ("СC".IndexOf(s2[0]) >= 0) && ("СC".IndexOf(s2[1]) >= 0) && ("СC".IndexOf(s2[2]) >= 0) && ("РP".IndexOf(s2[3]) >= 0))
                    s2 = "Россия";
                if (s1 == "СССР") s1 = "Россия";
                if (s2 == "СССР") s2 = "Россия";
                if (s1 == "Украина") s1 = "Россия";
                if (s2 == "Украина") s2 = "Россия";
                if (s1 != s2) return Result;
            }
            is_constr = false;
            is_name = false;
            res = 0;
            is_was_was = false;
            CompIt(r.ls, r1.ls, r.iconstr, r1.iconstr);
            CompIt(r1.ls, r.ls, r1.iconstr, r.iconstr);
            if (is_constr) res += 1000;
            if (!is_name && !is_constr) res = Int32.MinValue;
            if (!is_was_was) res = Int32.MinValue;
            return res;
        }


        private void CompIt(List<string> ls1, List<string> ls2, int c1, int c2) {
            var jprev = -2;
            for (var i = 0; i < ls1.Count; i++) {
                var s = ls1[i];
                var l = s.Length;
                var is_was = false;
                for (var j = ls2.Count - 1; j >= 0; j--) {
                    var s2 = ls2[j];
                    if (s2.StartsWith(s)) {
                        res += 2;
                        is_was = true;
                        is_was_was = true;
                        if ((i >= c1) && (j >= c2)) is_name = true;
                        if (s == s2) {
                            res++;
                            if (j == (jprev + 1)) res++;
                            jprev = j;
                            if ((l > 2) && ((i < c1) || (j < c2))) {
                                is_constr = true;
                            }
                        } else {
                            jprev = -2;
                        }
                        break;
                    } else {
                        if (Char.IsDigit(s[0]) && Char.IsDigit(s2[0])) {
                            var k = s2.Length;
                            if (k < l) l = k;
                            k = 1;
                            while ((k < l) && Char.IsDigit(s[k]) && Char.IsDigit(s2[k])) k++;
                            if ((k < l) &&
                                (!Char.IsDigit(s[k])) && (!Char.IsDigit(s2[k])) &&
                                (s.Substring(0, k) == s2.Substring(0, k))) {
                                res += 2;
                                jprev = j;
                            } else {
                                jprev = -2;
                            }
                        } else {
                            jprev = -2;
                        }
                    }
                }
                if (!is_was) res--;
            }
        }

    }
}
