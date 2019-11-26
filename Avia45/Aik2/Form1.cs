using Aik2.Dto;
using AutoMapper;
using Newtonsoft.Json;
using SourceGrid;
using SourceGrid.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            LoadPics();
        }

        public void LoadConfig()
        {
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
            }
            _shiftPressed = false;
        }


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_searchMode && gridArt.Focused) DoArtSearch((Keys)e.KeyChar, true);
            else
            if (_searchMode && gridCraft.Focused) DoCraftSearch((Keys)e.KeyChar, true);
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
                        wordId = Convert.ToInt32(wordId);
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
                SetWordColor(edCraftText, word, uText);
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
            if (tab.SelectedIndex == 2) LoadPics();
        }

        private void chPicSelArt_Click(object sender, EventArgs e)
        {
            LoadPics();
        }

    }
}
