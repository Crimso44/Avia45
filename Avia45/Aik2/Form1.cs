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

            _ctx = new AiKEntities();
            LoadArts();
            LoadCrafts();
            LoadPics(false);

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
            if (_config.ContainsKey("pCraftTextWidth"))
            {
                pCraftText.Width = int.Parse(_config["pCraftTextWidth"]);
            }
            if (_config.ContainsKey("pPicTextHeight"))
            {
                pPicText.Height = int.Parse(_config["pPicTextHeight"]);
            }
            if (_config.ContainsKey("pPicImgHeight"))
            {
                pPicImg.Height = int.Parse(_config["pPicImgHeight"]);
            }

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
        }

        private void calcWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ctx.Database.ExecuteSqlCommand("Truncate Table dbo.WordLinks");
            _ctx.Database.ExecuteSqlCommand("Delete From dbo.Words");

            foreach(var craft in _ctx.vwCrafts
                .OrderBy(x => x.Country).ThenBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name).ThenBy(x => x.Source)
                .ToList())
            {
                var crft = Mapper.Map<CraftDto>(craft);
                lInfo.Text = crft.FullName;
                Application.DoEvents();

                var text = _ctx.Crafts.Single(x => x.CraftId == craft.CraftId).CText;
                var wordsList = GetWords(text, true);
                foreach(var word in wordsList)
                {
                    _ctx.Database.ExecuteSqlCommand($"insert into WordLinks (WordId, CraftId) Values ({word.Id}, {craft.CraftId})");
                }

                var pics = _ctx.vwPics.Where(x => x.CraftId == crft.CraftId).ToList();
                var cnt = 0;
                foreach (var pic in pics)
                {
                    cnt++;
                    lInfo.Text = $"{crft.FullName} {cnt}/{pics.Count}";
                    Application.DoEvents();

                    var ptext = _ctx.Pics.Single(x => x.PicId == pic.PicId).Text;
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

            var sql = isCraft ?
                $"select distinct w.* from Words w join WordLinks wl on w.WordId = wl.WordId where w.cnt < 16 and wl.CraftId = {_selectedCraft.CraftId}" :
                $"select distinct w.* from Words w join WordLinks wl on w.WordId = wl.WordId where w.cnt < 16 and wl.PicId = {_selectedPic.PicId}";


            var words = _ctx.Database.SqlQuery<Words>(sql).ToList();

            var uText = textBox.Text.ToUpper();
            foreach (var word in words)
            {
                SetWordColor(textBox, word, uText);
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
                var ce = ' '; if (i + word.Word.Length < uText.Length) cb = uText[i + word.Word.Length];
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
                    SelectArt(_selectedPicArtId);
                    break;
                case 1:
                    SelectCraft(_selectedPicCraftId);
                    break;
                case 2:
                    LoadPics(true);
                    break;
            }
        }

        private void chPicSelArt_Click(object sender, EventArgs e)
        {
            LoadPics(false);
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
                                    var pic = _ctx.vwPics.Single(x => x.PicId == link.PicId);
                                    var craft = Mapper.Map<CraftDto>(_ctx.vwCrafts.Single(x => x.CraftId == pic.CraftId));
                                    captions.Add(new Pair<object>() { Id = (object)link, Name = $"{craft.FullName} {pic.Path}" });
                                } else
                                {
                                    var craft = Mapper.Map<CraftDto>(_ctx.vwCrafts.Single(x => x.CraftId == link.CraftId));
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
                _selectedPicCraftId = link.CraftId;
                if (tabControl1.SelectedIndex != 1)
                {
                    tabControl1.SelectedIndex = 1;
                } else
                {
                    SelectCraft(_selectedPicCraftId);
                }
            }
            else //if (link.PicId.HasValue)
            {
                int row;
                var craftId = _ctx.vwPics.Single(x => x.PicId == link.PicId.Value).CraftId;
                if (_selectedCraft == null || _selectedCraft.CraftId != craftId)
                {
                    var craftDto = _craftDtos.FirstOrDefault(x => x.CraftId == craftId);
                    if (craftDto == null)
                    {
                        craftDto = Mapper.Map<CraftDto>(_ctx.vwCrafts.Single(x => x.CraftId == craftId));
                        _craftDtos.Add(craftDto);
                        gridCraft.RowsCount++;
                        row = gridCraft.RowsCount - 1;
                        UpdateCraftRow(craftDto, row);
                    } else
                    {
                        row = _craftDtos.IndexOf(craftDto) + 1;
                    }
                    var focusPosn = new Position(row, _craftPosition.Column);
                    gridCraft.Selection.Focus(focusPosn, true);
                    _craftPosition = focusPosn;
                }
                _selectedPicCraftId = link.CraftId;
                if (tabControl1.SelectedIndex != 2)
                {
                    tabControl1.SelectedIndex = 2;
                }
                else
                {
                    LoadPics(true);
                }
                var pic = _pics.SingleOrDefault(x => x.PicId == link.PicId.Value);
                if (pic != null)
                {
                    row = _pics.IndexOf(pic) + 1;
                    var focusPic = new Position(row, _craftPosition.Column);
                    gridPic.Selection.Focus(focusPic, true);
                    _picPosition = focusPic;
                }
            }
        }
    }
}
