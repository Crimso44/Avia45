using Aik2.Dto;
using AutoMapper;
using Newtonsoft.Json;
using SourceGrid;
using SourceGrid.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Aik2.Util;

namespace Aik2
{
    public partial class Form1 : Form
    {
        private AiKEntities _ctx;

        private bool _shiftPressed = false;
        private bool _searchMode = false;
        private bool _searchChanging = false;
        private string _searchString = "";
        private bool _init = false;

        private Dictionary<string, string> _config;
        private string _confPath;
        private string _imagesPath;
        private List<WordLinks> _menuLinks;

        public Form1()
        {
            LoadConfig();
            InitializeComponent();
            SetConfig();

            InitArtGrid();
            InitCraftGrid();
            InitPicGrid();
            InitLinkGrid();

            _ctx = new AiKEntities();
            LoadArts();
            LoadCrafts();
            LoadPics(true);

            tabControl1.SelectedIndex = 1;
        }

        public void LoadConfig()
        {
            _imagesPath = ConfigurationManager.AppSettings["AikPath"] + "\\";

            _confPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Aik2.config";
            if (File.Exists(_confPath))
            {
                var confData = File.ReadAllText(_confPath);
                _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(confData);
            } else
            {
                _config = new Dictionary<string, string>();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }

        }

        public void SetConfig() {
            _init = true;
            if (_config.ContainsKey("pCraftTextWidth"))
            {
                pCraftText.Width = int.Parse(_config["pCraftTextWidth"]);
            }
            if (_config.ContainsKey("pCraftPicHeight"))
            {
                pCraftPic.Height = int.Parse(_config["pCraftPicHeight"]);
            }
            if (_config.ContainsKey("pPicTextHeight"))
            {
                pPicText.Height = int.Parse(_config["pPicTextHeight"]);
            }
            if (_config.ContainsKey("pCraftSeeAlsoHeight"))
            {
                pCraftSeeAlso.Height = int.Parse(_config["pCraftSeeAlsoHeight"]);
            }
            if (_config.ContainsKey("pPicImgHeight"))
            {
                pPicImg.Height = int.Parse(_config["pPicImgHeight"]);
            }
            if (_config.ContainsKey("pLinkImgWidth"))
            {
                pLinkImage.Width = int.Parse(_config["pLinkImgWidth"]);
            }
            _init = false;
        }


        private void StringMaxLen(TextBox sender, CancelEventArgs cancelEvent, int maxLen)
        {
            string val = sender.Text;
            if (val != null && val.Length > maxLen)
                cancelEvent.Cancel = true;
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                _shiftPressed = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey && _shiftPressed)
            {
                _searchMode = !_searchMode;
                _searchString = "";
                lInfo.Text = _searchMode ? "Поиск: " : "";
                foreach (var editor in _editorsArt)
                {
                    if (editor != null) editor.EnableEdit = !_searchMode;
                }
                foreach (var editor in _editorsCraft)
                {
                    if (editor != null) editor.EnableEdit = !_searchMode;
                }
                foreach (var editor in _editorsPic)
                {
                    if (editor != null) editor.EnableEdit = !_searchMode;
                }
            }
            _shiftPressed = false;
        }


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_searchMode && gridArt.Focused) DoArtSearch((Keys)e.KeyChar, true);
            else
            if (_searchMode && gridCraft.Focused) DoCraftSearch((Keys)e.KeyChar, true);
            else
            if (_searchMode && gridPic.Focused) DoPicSearch((Keys)e.KeyChar, true);
        }


        private void SearchModeOff()
        {
            _searchMode = false;
            lInfo.Text = "";
            foreach (var editor in _editorsArt)
            {
                if (editor != null) editor.EnableEdit = !_searchMode;
            }
            foreach (var editor in _editorsCraft)
            {
                if (editor != null) editor.EnableEdit = !_searchMode;
            }
            foreach (var editor in _editorsPic)
            {
                if (editor != null) editor.EnableEdit = !_searchMode;
            }
        }

        private void calcWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reload all words?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _ctx.Database.ExecuteSqlCommand("Truncate Table dbo.WordLinks");
                _ctx.Database.ExecuteSqlCommand("Delete From dbo.Words");

                foreach (var craft in _ctx.vwCrafts.AsNoTracking()
                    .OrderBy(x => x.Country).ThenBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name).ThenBy(x => x.Source)
                    .ToList())
                {
                    var crft = Mapper.Map<CraftDto>(craft);
                    lInfo.Text = crft.FullName;
                    Application.DoEvents();

                    var text = _ctx.Crafts.Single(x => x.CraftId == craft.CraftId).CText;
                    var wordsList = GetWords(text, true);
                    foreach (var word in wordsList)
                    {
                        _ctx.Database.ExecuteSqlCommand($"insert into WordLinks (WordId, CraftId) Values ({word.Id}, {craft.CraftId})");
                    }

                    var pics = _ctx.vwPics.AsNoTracking().Where(x => x.CraftId == crft.CraftId).ToList();
                    var cnt = 0;
                    foreach (var pic in pics)
                    {
                        cnt++;
                        lInfo.Text = $"{crft.FullName} {cnt}/{pics.Count}";
                        Application.DoEvents();

                        var ptext = _ctx.Pics.Single(x => x.PicId == pic.PicId).Text ?? "";
                        wordsList = GetWords(ptext, true);
                        foreach (var word in wordsList)
                        {
                            _ctx.Database.ExecuteSqlCommand($"insert into WordLinks (WordId, PicId) Values ({word.Id}, {pic.PicId})");
                        }
                    }

                }
                lInfo.Text = "";

                _ctx.Database.Connection.Close();
                _ctx.Database.Connection.Open();

                MessageBox.Show("OK");
            }
        }

        private List<Pair<int>> GetWords(string text, bool isAppend)
        {
            var res = new List<Pair<int>>();
            if (string.IsNullOrEmpty(text)) return res;

            var txt = text.ToUpper() + ' ';
            var list = new List<string>();
            var s = "";
            foreach(var c in txt)
            {
                if (Char.IsLetterOrDigit(c) || c == '-')
                {
                    s += c;
                } else
                {
                    if (s.Length > 3)
                    {
                        s = s.Substring(0, Math.Min(s.Length, 50));
                        if (!list.Contains(s))
                        {
                            list.Add(s);
                        }
                    }
                    s = "";
                }
            }
            foreach(var w in list)
            {
                var words = _ctx.Database.SqlQuery<Words>($"select * from Words where Word = '{w}'").ToList();
                var word = words.Any() ? words.First() : null;
                if (word == null)
                {
                    var wordId = -1;
                    if (isAppend)
                    {
                        var wordIdDec = _ctx.Database.SqlQuery<decimal>(
                            $"SET NOCOUNT ON; insert into Words (word) values ('{w}'); select SCOPE_IDENTITY(); SET NOCOUNT OFF;"
                        ).ToList().First();
                        wordId = Convert.ToInt32(wordIdDec);
                    }
                    res.Add(new Pair<int>() { Id = wordId, Name = w });
                }
                else
                {
                    res.Add(new Pair<int>() { Id = word.WordId, Name = w });
                }
            }
            return res;
        }

        private void ColorizeText(RichTextBox textBox, bool isCraft)
        {
            textBox.Select(0, textBox.Text.Length);
            textBox.SelectionColor = Color.Black;
            textBox.SelectionBackColor = Color.White;

            if (_filterOn && _filter.InText && (!string.IsNullOrEmpty(_filter.Text) || !string.IsNullOrEmpty(_filter.Text2)))
            {
                var lTxt = ExtractFilters(_filter.Text.ToLower());
                lTxt.AddRange(ExtractFilters(_filter.Text2.ToLower()));
                var txtlw = textBox.Text.ToLower();
                foreach (var t in lTxt)
                {
                    var i = txtlw.IndexOf(t);
                    while (i >= 0)
                    {
                        textBox.Select(i, t.Length);
                        textBox.SelectionColor = Color.Red;

                        i = txtlw.IndexOf(t, i + 1);
                    }
                }
            }
            else
            {
                var sql = isCraft ?
                    $"select distinct w.* from Words w join WordLinks wl on w.WordId = wl.WordId where w.cnt < 16 and wl.CraftId = {_selectedCraft.CraftId}" :
                    $"select distinct w.* from Words w join WordLinks wl on w.WordId = wl.WordId where w.cnt < 16 and wl.PicId = {_selectedPic.PicId}";


                var words = _ctx.Database.SqlQuery<Words>(sql).ToList();

                var uText = textBox.Text.ToUpper();
                foreach (var word in words)
                {
                    SetWordColor(textBox, word, uText);
                }
            }

            textBox.Select(0, 0);

        }

        private void SetWordColor(RichTextBox textBox, Words word, string uText)
        {
            var selectedWord = word.Word;
            var i = uText.IndexOf(word.Word);
            while (i >= 0)
            {
                var cb = ' '; if (i > 0) cb = uText[i - 1];
                var ce = ' '; if (i + word.Word.Length < uText.Length) ce = uText[i + word.Word.Length];
                if (cb != '-' && ce != '-' && !Char.IsLetterOrDigit(cb) && !Char.IsLetterOrDigit(ce))
                {
                    Color col = Color.Black;
                    var isColor = false;
                    if (word.Cnt < 2)
                    {
                        col = Color.Red;
                        isColor = true;
                    }
                    else
                    {
                        var iCol = 255 - word.Cnt.Value * 16;
                        if (iCol >= 0)
                        {
                            col = Color.FromArgb(0, iCol, 0);
                            isColor = true;
                        }
                    }
                    if (isColor)
                    {
                        textBox.Select(i, word.Word.Length);
                        textBox.SelectionColor = col;
                    }

                    var is_bad = false;
                    var is_none = false;
                    var is_lat = false;
                    var is_rus = false;
                    var is_lat2 = false;
                    var is_rus2 = false;
                    var bcol = Color.White;
                    foreach (var cc in textBox.Text.Substring(i, word.Word.Length))
                    {
                        if (Const.lat.Contains(cc))
                            is_lat2 = true;
                        else if (Const.rus.Contains(cc))
                            is_rus2 = true;
                        if ("ABCEHKMOPTX".Contains(cc))
                            is_lat = true;
                        else if ("АВСЕНКМОРТХ".Contains(cc))
                            is_rus = true;
                        else if (!Char.IsDigit(cc) && cc != '-')
                            is_none = true;
                        if (is_rus2 && is_lat2)
                        {
                            is_bad = true;
                            break;
                        }

                    }

                    if (is_bad)
                        bcol = Color.FromArgb(0xFF, 0xDD, 0xDD);
                    else if (is_none)
                        bcol = Color.White;
                    else if (is_rus)
                        bcol = Color.FromArgb(0xE2, 0xFF, 0xE2);
                    else if (is_lat)
                        bcol = Color.FromArgb(0xE2, 0xE2, 0xFF);
                    /*else
                        bcol = Color.FromArgb(0xE2, 0xFF, 0xFF);*/
                    if (bcol != Color.White)
                    {
                        textBox.Select(i, word.Word.Length);
                        textBox.SelectionBackColor = bcol;
                    }
                }

                i = uText.IndexOf(word.Word, i + 1);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tab = (TabControl)sender;
            switch (tab.SelectedIndex)
            {
                case 0:
                    if (_needLoadArt) SelectArt(_selectedPic?.ArtId);
                    break;
                case 1:
                    if (_needLoadCraft) SelectCraft(_selectedPic?.CraftId);
                    break;
                case 2:
                    LoadPics(true);
                    break;
            }
        }

        private void edPicText_DoubleClick(object sender, EventArgs e)
        {
            var richText = (RichTextBox)sender;
            var text = richText.Text;
            var i0 = richText.SelectionStart;
            while (i0 >= 0 && (text[i0] == '-' || char.IsLetterOrDigit(text[i0]))) i0--;
            i0++;
            var i1 = richText.SelectionStart;
            while (i1 < text.Length && (text[i1] == '-' || char.IsLetterOrDigit(text[i1]))) i1++;
            i1--;
            if (i1 > i0)
            {
                var word = richText.Text.Substring(i0, i1 - i0 + 1).ToUpper();
                if (word.Length > 3)
                {
                    var linkCnt = _ctx.Database.SqlQuery<int>(
                        $"select count(*) from Words w join WordLinks wl on w.WordId = wl.WordId where w.Word = '{word}'").ToList().First();
                    if (linkCnt > 0)
                    {
                        var menu = new ContextMenu();
                        if (linkCnt > 15)
                        {
                            menu.MenuItems.Add($"{linkCnt} слов");
                        }
                        else
                        {
                            _menuLinks = _ctx.Database.SqlQuery<WordLinks>(
                                $"select wl.* from Words w join WordLinks wl on w.WordId = wl.WordId where w.Word = '{word}'").ToList();
                            var captions = new List<Pair<object>>();
                            foreach (var link in _menuLinks)
                            {
                                if (link.PicId.HasValue)
                                {
                                    var pic = _ctx.vwPics.AsNoTracking().Single(x => x.PicId == link.PicId);
                                    var craft = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().Single(x => x.CraftId == pic.CraftId));
                                    captions.Add(new Pair<object>() { Id = (object)link, Name = $"{craft.FullName} {pic.Path}" });
                                } else
                                {
                                    var craft = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().Single(x => x.CraftId == link.CraftId));
                                    captions.Add(new Pair<object>() { Id = (object)link, Name = $"{craft.FullName}" });
                                }
                            }
                            captions.Sort((f1, f2) =>
                            {
                                return f1.Name.CompareTo(f2.Name);
                            });
                            foreach(var cap in captions)
                            {
                                var mnuItem = new MenuItem() { Text = cap.Name, Tag = cap.Id };
                                if ((sender == edCraftText && _selectedCraft.CraftId == ((WordLinks)cap.Id).CraftId) ||
                                    (sender == edPicText && _selectedPic.PicId == ((WordLinks)cap.Id).PicId))
                                {
                                    mnuItem.Checked = true;
                                }
                                mnuItem.Click += OnWordMenuClick;
                                menu.MenuItems.Add(mnuItem);
                            }
                        }
                        menu.Show(richText, richText.PointToClient(Control.MousePosition));
                    }
                }
            }

        }

        private void OnWordMenuClick(object sender, EventArgs e)
        {
            var mnuItem = (MenuItem)sender;
            var link = (WordLinks)mnuItem.Tag;
            if (link.CraftId.HasValue)
            {
                if (tabControl1.SelectedIndex != 1)
                {
                    tabControl1.SelectedIndex = 1;
                }
                _selectedCraft = null;
                SelectCraft(link.CraftId);
            }
            else //if (link.PicId.HasValue)
            {
                ShowPictureById(link.PicId.Value);
            }
        }

        private void ShowPictureById(int picId)
        {
            int row;
            if (chPicSelCraft.Checked)
            {
                var craftId = _ctx.vwPics.AsNoTracking().Single(x => x.PicId == picId).CraftId;
                if (_selectedCraft == null || _selectedCraft.CraftId != craftId)
                {
                    var craftDto = _craftDtos.FirstOrDefault(x => x.CraftId == craftId);
                    if (craftDto == null)
                    {
                        craftDto = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().Single(x => x.CraftId == craftId));
                        _craftDtos.Add(craftDto);
                        gridCraft.RowsCount++;
                        row = gridCraft.RowsCount - 1;
                        UpdateCraftRow(craftDto, row);
                    }
                    else
                    {
                        row = _craftDtos.IndexOf(craftDto) + 1;
                    }
                    var focusPosn = new Position(row, _craftPosition.Column);
                    gridCraft.Selection.Focus(focusPosn, true);
                    DoCraftCellGotFocus(focusPosn);
                    _craftPosition = focusPosn;
                    _selectedCraft = craftDto;
                }
            }
            else
            {
                var artId = _ctx.vwPics.AsNoTracking().Single(x => x.PicId == picId).ArtId;
                if (_selectedArt == null || _selectedArt.ArtId != artId)
                {
                    var artDto = _artDtos.FirstOrDefault(x => x.ArtId == artId);
                    if (artDto != null)
                    {
                        row = _artDtos.IndexOf(artDto) + 1;
                        var focusPosn = new Position(row, _artPosition.Column);
                        gridArt.Selection.Focus(focusPosn, true);
                        _artPosition = focusPosn;
                        _selectedArt = artDto;
                    }
                }
            }
            PicDto pic = null;
            if (tabControl1.SelectedIndex == 2)
            {
                pic = _pics.SingleOrDefault(x => x.PicId == picId);
            }
            else
            {
                tabControl1.SelectedIndex = 2;
                pic = _pics.SingleOrDefault(x => x.PicId == picId);
            }
            if (pic == null)
            {
                LoadPics(true);
                pic = _pics.SingleOrDefault(x => x.PicId == picId);
            }
            if (pic != null)
            {
                row = _pics.IndexOf(pic) + 1;
                var focusPic = new Position(row, _craftPosition.Column);
                gridPic.Selection.Focus(focusPic, true);
                _picPosition = focusPic;
            }
        }

        private string GetInnerestException(Exception e)
        {
            while (e.InnerException != null) e = e.InnerException;
            return e.Message;
        }

        private bool CompareReport(ReportDto o, ReportDto n)
        {
            if (o == null || n == null) return true;
            return o.pics != n.pics || o.crafts != n.crafts || o.uniq != n.uniq;
        }

        private void reportNewPicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var all = (
                    from r in _ctx.Report
                    select new { r.Mag, IYear = r.IYear ?? -1, r.IMonth, r.Source }
                ).Concat(
                    from v in _ctx.vwReport
                    select new { v.Mag, v.IYear, v.IMonth, v.Source }
                ).Distinct();
            var rep = (
                from m in all
                from n in _ctx.vwReport.Where(n =>
                    m.Mag == n.Mag && m.IYear == n.IYear && m.IMonth == n.IMonth && m.Source == n.Source).DefaultIfEmpty()
                from o in _ctx.Report.Where(o =>
                    m.Mag == o.Mag && m.IYear == o.IYear && m.IMonth == o.IMonth && m.Source == o.Source).DefaultIfEmpty()
                orderby m.Mag, m.IYear, m.IMonth, m.Source
                select new { m, o, n }).ToList();

            var frep = new fReport(_ctx, _imagesPath);
            var wasDivider = true;
            for(var i = 0; i < rep.Count; i++)
            {
                var r = rep[i];
                var diff = CompareReport(Mapper.Map<ReportDto>(r.o), Mapper.Map<ReportDto>(r.n));
                var diffPrev = i > 0 && CompareReport(Mapper.Map<ReportDto>(rep[i - 1].o), Mapper.Map<ReportDto>(rep[i - 1].n));
                var diffNext = i < rep.Count - 1 && CompareReport(Mapper.Map<ReportDto>(rep[i + 1].o), Mapper.Map<ReportDto>(rep[i + 1].n));

                if (diff || diffPrev || diffNext)
                {
                    var s =
                        $"{PadLeft(r.m.Mag.Trim(), ' ', 5)} {PadLeft(r.m.IYear.ToString(), ' ', 5)} {PadLeft(r.m.IMonth.Trim(), ' ', 5)} {r.o?.Source ?? r.n?.Source}: " +
                        $"{PadLeft(r.o?.pics.ToString(), ' ', 6)} {PadLeft(r.o?.crafts.ToString(), ' ', 6)} {PadLeft(r.o?.uniq.ToString(), ' ', 6)}   ";
                    if (diff)
                    {
                        s += $"{PadLeft(r.n?.pics.ToString(), ' ', 6)} {PadLeft(r.n?.crafts.ToString(), ' ', 6)} {PadLeft(r.n?.uniq.ToString(), ' ', 6)}  =>  ";
                        var dpics = (r.n?.pics ?? 0) - (r.o?.pics ?? 0);
                        var dcrafts = (r.n?.crafts ?? 0) - (r.o?.crafts ?? 0);
                        var duniq = (r.n?.uniq ?? 0) - (r.o?.uniq ?? 0);
                        s += $"{(dpics > 0 ? "+" : "")}{dpics} {(dcrafts > 0 ? "+" : "")}{dcrafts} {(duniq > 0 ? "+" : "")}{duniq}";
                    }
                    frep.ReportList.Items.Add(s);
                    wasDivider = false;
                }
                else
                {
                    if (!wasDivider)
                    {
                        frep.ReportList.Items.Add("...");
                        wasDivider = true;
                    }
                }
            }
            frep.ShowDialog(this);
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    StoreEditedCraftText();
                    LoadCrafts();
                    edCraftText.SelectionStart = 0;
                    edCraftText.SelectionLength = edCraftText.TextLength;
                    edCraftText.SelectionFont = new Font(edCraftText.SelectionFont, FontStyle.Regular);
                    edCraftText.SelectionColor = Color.Black;
                    edCraftText.SelectionBackColor = Color.White;
                    break;
                case 2:
                    StoreEditedPicText();
                    LoadPics(false);
                    edPicText.SelectionStart = 0;
                    edPicText.SelectionLength = edPicText.TextLength;
                    edPicText.SelectionFont = new Font(edPicText.SelectionFont, FontStyle.Regular);
                    edPicText.SelectionColor = Color.Black;
                    edPicText.SelectionBackColor = Color.White;
                    break;
            }
        }

        private void make6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var wm = new WebMaker();
            wm.PrepareWeb6(_ctx, _imagesPath, lInfo);
        }

        private void showBookmarkedPicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lockedPicId.HasValue)
            {
                ShowPictureById(_lockedPicId.Value);
            }
        }

    }
}
