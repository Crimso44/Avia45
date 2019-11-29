using Aik2.Dto;
using AutoMapper;
using Newtonsoft.Json;
using SourceGrid;
using SourceGrid.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsPic;
        private Position _picPosition = Position.Empty;
        private GridPicController _gridPicController = null;
        private PicDto _selectedPic = null;
        private int? _selectedPicArtId = null;
        private int? _selectedPicCraftId = null;
        private string _oldPicText;
        private bool _picTextChanging;
        private List<PicDto> _pics;

        public void LoadPics(bool isClear)
        {
            if (isClear)
            {
                _selectedPicArtId = null;
                _selectedPicCraftId = null;
            }

            var PicsQry = _ctx.vwPics.AsQueryable();

            var flt = false;
            if (chPicSelArt.Checked && (_selectedPicArtId.HasValue || _selectedArt != null))
            {
                var artId = _selectedPicArtId ?? _selectedArt.ArtId;
                PicsQry = PicsQry.Where(x => x.ArtId == artId);
                flt = true;
            }
            if (chPicSelCraft.Checked && (_selectedPicCraftId.HasValue || _selectedCraft != null))
            {
                var craftId = _selectedPicCraftId ?? _selectedCraft.CraftId;
                PicsQry = PicsQry.Where(x => x.CraftId == craftId);
                flt = true;
            }
            if (!flt)
            {
                PicsQry = PicsQry.Where(x => false);
            }
            PicsQry = PicsQry.OrderBy(x => x.NType).ThenBy(x => x.NNN);

            var Pics = PicsQry.ToList();
            _pics = Mapper.Map<List<PicDto>>(Pics);

            var saved = -1;
            if (_picPosition != Position.Empty && _picPosition.Row > 0)
            {
                saved = (int)gridPic[_picPosition.Row, Const.Columns.Pic.PicId].Value;
            }

            _selectedPicCraftId = null;
            _selectedPicArtId = null;

            _gridPicController = new GridPicController(this);
            gridPic.RowsCount = _pics.Count + 1;

            var focused = false;
            for (var r = 1; r <= _pics.Count; r++)
            {
                var Pic = _pics[r - 1];
                UpdatePicRow(Pic, r);

                if (Pic.PicId == saved && _picPosition != Position.Empty)
                {
                    var focusPosn = new Position(r, _picPosition.Column);
                    gridPic.Selection.Focus(focusPosn, true);
                    _selectedPicCraftId = Pic.CraftId;
                    _selectedPicArtId = Pic.ArtId;
                    _picPosition = focusPosn;
                    focused = true;
                }
            }
            if (!focused)
            {
                _picPosition = _pics.Any() ? new Position(1, 1) : new Position(0, 1);
                gridPic.Selection.Focus(_picPosition, true);
            }

            gridPic.Refresh();
        }



        public void UpdatePicRow(PicDto Pic, int r)
        {
            gridPic[r, 0] = new SourceGrid.Cells.Cell(Pic.PicId, _editorsPic[0]);
            gridPic[r, 1] = new SourceGrid.Cells.Cell(Pic.XPage, _editorsPic[1]);
            gridPic[r, 1].AddController(_gridPicController);
            gridPic[r, 2] = new SourceGrid.Cells.Cell(Pic.NN, _editorsPic[2]);
            gridPic[r, 2].AddController(_gridPicController);
            gridPic[r, 3] = new SourceGrid.Cells.Cell(Pic.NNN, _editorsPic[3]);
            gridPic[r, 3].AddController(_gridPicController);
            gridPic[r, 4] = new SourceGrid.Cells.Cell(Pic.Type, _editorsPic[4]);
            gridPic[r, 4].AddController(_gridPicController);
            gridPic[r, 5] = new SourceGrid.Cells.Cell(Pic.NType, _editorsPic[5]);
            gridPic[r, 5].AddController(_gridPicController);
            gridPic[r, 6] = new SourceGrid.Cells.Cell(Pic.Path, _editorsPic[6]);
            gridPic[r, 6].AddController(_gridPicController);
            var artName = "";
            var art = _arts.FirstOrDefault(x => x.Id == Pic.ArtId);
            if (art != null) artName = art.Name;
            gridPic[r, 7] = new SourceGrid.Cells.Cell(artName, _editorsPic[7]);
            gridPic[r, 7].AddController(_gridPicController);
            var craftName = "";
            var craft = _crafts678.FirstOrDefault(x => x.Id == Pic.CraftId);
            if (craft != null) craftName = craft.Name;
            gridPic[r, 8] = new SourceGrid.Cells.Cell(craftName, _editorsPic[8]);
            gridPic[r, 8].AddController(_gridPicController);
            gridPic[r, 9] = new SourceGrid.Cells.Cell(Pic.Grp, _editorsPic[9]);
            gridPic[r, 9].AddController(_gridPicController);
            gridPic[r, 10] = new SourceGrid.Cells.Cell(Pic.SText, _editorsPic[10]);
            gridPic[r, 10].AddController(_gridPicController);
            gridPic[r, 11] = new SourceGrid.Cells.Cell(Pic.ArtId);
            gridPic[r, 12] = new SourceGrid.Cells.Cell(Pic.CraftId);

        }


        private class GridPicController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly Form1 _form;

            public GridPicController(Form1 form)
            {
                _form = form;
            }

            public override void OnEditStarted(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnEditStarted(sender, e);
                _form.SearchModeOff();
                if (sender.Position.Column == Const.Columns.Pic.CraftId)
                {
                    _form._editCraft678.Control.DroppedDown = true;
                }
                if (sender.Position.Column == Const.Columns.Pic.ArtId)
                {
                    _form._editArt.Control.DroppedDown = true;
                }
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                var row = sender.Position.Row;
                SourceGrid.Cells.Cell cell = (SourceGrid.Cells.Cell)sender.Cell;
                var val = (string)cell.DisplayText;


                switch (sender.Position.Column)
                {
                    case Const.Columns.Pic.Craft:
                        _form.gridPic[row, Const.Columns.Pic.CraftId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._crafts678.SingleOrDefault(x => x.Name == val)?.Id;
                        break;
                    case Const.Columns.Pic.Art:
                        _form.gridPic[row, Const.Columns.Pic.ArtId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._arts.SingleOrDefault(x => x.Name == val)?.Id;
                        break;
                    case Const.Columns.Pic.Type:
                        _form.gridPic[row, Const.Columns.Pic.NType].Value = _form.GetNType(val);
                        break;
                }

                var sortIndexes = new int[] { Const.Columns.Pic.NType, Const.Columns.Pic.NNN };
                var sortXIndexes = new int[] { Const.Columns.Pic.Type, Const.Columns.Pic.NType, Const.Columns.Pic.NNN };

                if (sortXIndexes.Contains(cell.Column.Index))
                {
                    var sortVal = "";
                    var rNew = row;
                    foreach (var col in sortIndexes) sortVal += PadLeft(_form.gridPic[row, col].DisplayText, '0', 5) + " ";
                    if (row < _form.gridPic.RowsCount)
                    {
                        var found = false;
                        for (var r = row + 1; r < _form.gridPic.RowsCount; r++)
                        {
                            var sortRVal = "";
                            foreach (var col in sortIndexes) sortRVal += PadLeft(_form.gridPic[r, col].DisplayText, '0', 5) + " ";
                            if (string.Compare(sortVal, sortRVal) <= 0)
                            {
                                rNew = r - 1;
                                found = true;
                                break;
                            }
                        }
                        if (!found) rNew = _form.gridPic.RowsCount - 1;
                    }
                    if (rNew == row && row > 1)
                    {
                        var found = false;
                        for (var r = row - 1; r >= 1; r--)
                        {
                            var sortRVal = "";
                            foreach (var col in sortIndexes) sortRVal += PadLeft(_form.gridPic[r, col].DisplayText, '0', 5) + " ";
                            if (string.Compare(sortRVal, sortVal) <= 0)
                            {
                                rNew = r + 1;
                                found = true;
                                break;
                            }
                        }
                        if (!found) rNew = 1;
                    }

                    if (rNew != row)
                    {
                        _form.gridPic.Rows.Move(row, rNew);
                        var focusPosn = new Position(rNew, sender.Position.Column == Const.Columns.Pic.NType ? Const.Columns.Pic.Type : sender.Position.Column);
                        _form.gridPic.Selection.Focus(focusPosn, true);
                    }

                }

            }
        }

        public void InitPicGrid()
        {
            _editorsPic = new List<SourceGrid.Cells.Editors.EditorBase>();

            var editStr4 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr4.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 4); };

            var editStr5 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr5.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 5); };

            var editStr10 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr10.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 10); };

            var editStr50 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr50.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 50); };

            var editStr100 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr100.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 100); };

            gridPic.ColumnsCount = 13;

            gridPic.RowsCount = 1;
            gridPic.FixedRows = 1;

            gridPic[0, 0] = new SourceGrid.Cells.ColumnHeader("PicId");
            gridPic.Columns[0].Visible = false;
            _editorsPic.Add(null);
            gridPic[0, 1] = new SourceGrid.Cells.ColumnHeader("XPage");
            _editorsPic.Add(editStr4);
            gridPic[0, 2] = new SourceGrid.Cells.ColumnHeader("NN");
            _editorsPic.Add(editStr5);
            gridPic[0, 3] = new SourceGrid.Cells.ColumnHeader("NNN");
            gridPic.Columns[3].Visible = false;
            _editorsPic.Add(null);
            gridPic[0, 4] = new SourceGrid.Cells.ColumnHeader("Type");
            _editorsPic.Add(editStr10);
            gridPic[0, 5] = new SourceGrid.Cells.ColumnHeader("NType");
            gridPic.Columns[5].Visible = false;
            _editorsPic.Add(null);
            gridPic[0, 6] = new SourceGrid.Cells.ColumnHeader("Path");
            _editorsPic.Add(editStr50);
            gridPic[0, 7] = new SourceGrid.Cells.ColumnHeader("Art");
            _editorsPic.Add(_editArt);
            gridPic[0, 8] = new SourceGrid.Cells.ColumnHeader("Craft");
            _editorsPic.Add(_editCraft678);
            gridPic[0, 9] = new SourceGrid.Cells.ColumnHeader("Grp");
            _editorsPic.Add(editStr100);
            gridPic[0, 10] = new SourceGrid.Cells.ColumnHeader("SText");
            _editorsPic.Add(null);
            gridPic[0, 11] = new SourceGrid.Cells.ColumnHeader("ArtId");
            gridPic.Columns[11].Visible = false;
            gridPic[0, 12] = new SourceGrid.Cells.ColumnHeader("CraftId");
            gridPic.Columns[12].Visible = false;

            for (var i = 1; i < gridPic.ColumnsCount; i++)
            {
                var key = $"gridPic:ColumnWidth:{i}";
                if (_config.ContainsKey(key))
                {
                    gridPic.Columns[i].Width = int.Parse(_config[key]);
                }
            }

            gridPic.Selection.CellGotFocus += PicCellGotFocus;
            gridPic.Columns.ColumnWidthChanged += PicColumnWidthChanged;

        }

        private void PicCellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            _picPosition = e.NewFocusPosition;
            var picId = (int)gridPic[_picPosition.Row, Const.Columns.Pic.PicId].Value;
            if (picId > 0 &&
                (_selectedPic == null || _selectedPic.PicId != (int)gridPic[_picPosition.Row, Const.Columns.Pic.PicId].Value))
            {
                _selectedPic = Mapper.Map<PicDto>(_ctx.Pics.Single(x => x.PicId == picId));
                _picTextChanging = true;
                edPicText.Text = _selectedPic.Text;
                _oldPicText = edPicText.Text;
                ColorizeText(edPicText, false);
                _picTextChanging = false;
            }
            _selectedPicCraftId = (int)gridPic[_picPosition.Row, Const.Columns.Pic.CraftId].Value;
            _selectedPicArtId = (int)gridPic[_picPosition.Row, Const.Columns.Pic.ArtId].Value;
            if (_searchMode && !_searchChanging)
            {
                _searchString = "";
                lInfo.Text = $"Поиск: {_searchString}";
            }

            var art = Mapper.Map<ArtDto>(_ctx.Arts.FirstOrDefault(x => x.ArtId == _selectedPicArtId));
            if (art != null) lArt.Text = art.FullName;

            picPicImage.Visible = false;
            try
            {
                var craft = _craftDtos.FirstOrDefault(x => x.CraftId == _selectedPicCraftId);
                if (craft == null)
                {
                    craft = Mapper.Map<CraftDto>(_ctx.vwCrafts.FirstOrDefault(x => x.CraftId == _selectedPicCraftId));
                }
                if (craft != null)
                {
                    lCraft.Text = craft.FullName;

                    var picPath = $"{_imagesPath}Images{craft.Source}\\{(string)gridPic[_picPosition.Row, Const.Columns.Pic.Path].Value}";
                    picPicImage.Load(picPath);
                    picPicImage.Visible = true;
                }
            }
            catch
            {

            }
        }


        private void PicColumnWidthChanged(object sender, ColumnInfoEventArgs e)
        {
            if (sender == gridPic.Columns)
            {
                _config[$"gridPic:ColumnWidth:{e.Column.Index}"] = e.Column.Width.ToString();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }
        }

        private void DoPicSearch(Keys c, bool isText)
        {
            switch (c)
            {
                case Keys.Escape:
                    _searchString = "";
                    break;
                case Keys.Back:
                    _searchString = _searchString.Length == 0 ? "" : _searchString.Substring(0, _searchString.Length - 1);
                    break;
                default:
                    if (isText) _searchString += (char)c;
                    break;
            }
            lInfo.Text = $"Поиск: {_searchString}";

            var pos = gridPic.Selection.ActivePosition;
            if (pos != Position.Empty)
            {
                var found = false;
                for (var r = pos.Row; r < gridPic.RowsCount; r++)
                {
                    if (gridPic[r, pos.Column].DisplayText.ToUpper().StartsWith(_searchString.ToUpper()))
                    {
                        _searchChanging = true;
                        var focusPosn = new Position(r, pos.Column);
                        gridPic.Selection.Focus(focusPosn, true);
                        found = true;
                        _searchChanging = false;
                        break;
                    }
                }
                if (!found)
                {
                    for (var r = 1; r < pos.Row; r++)
                    {
                        if (gridPic[r, pos.Column].DisplayText.ToUpper().StartsWith(_searchString.ToUpper()))
                        {
                            _searchChanging = true;
                            var focusPosn = new Position(r, pos.Column);
                            gridPic.Selection.Focus(focusPosn, true);
                            found = true;
                            _searchChanging = false;
                            break;
                        }
                    }
                }
            }
        }

        private void gridPic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (gridPic.Focused)
            {
                var pos = gridPic.Selection.ActivePosition;
                if (pos != Position.Empty)
                {
                    if (e.KeyCode == Keys.Insert)
                    {
                        SearchModeOff();
                        var Pic = new PicDto()
                        {
                            XPage = (string)gridPic[pos.Row, Const.Columns.Pic.XPage].Value,
                            Type = (string)gridPic[pos.Row, Const.Columns.Pic.Type].Value,
                            NType = (int?)gridPic[pos.Row, Const.Columns.Pic.NType].Value,
                            NN = (int.Parse((string)gridPic[pos.Row, Const.Columns.Pic.NN].Value)+1).ToString(),
                            NNN = _ctx.vwPics.OrderByDescending(x => x.NNN).First().NNN + 1
                        };
                        gridPic.Rows.Insert(pos.Row);
                        UpdatePicRow(Pic, pos.Row);
                    }
                    else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                    {
                        if (MessageBox.Show("Delete pic?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            gridPic.Rows.Remove(pos.Row);
                    }
                    else if (_searchMode)
                    {
                        DoPicSearch(e.KeyCode, false);
                    }
                }
            }
        }


        private void pPicText_Resize(object sender, EventArgs e)
        {
            _config["pPicTextHeight"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void pPicImg_Resize(object sender, EventArgs e)
        {
            _config["pPicImgHeight"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }


        private void edPicText_TextChanged(object sender, EventArgs e)
        {
            if (_picTextChanging) return;

            var selStart = edPicText.SelectionStart;
            var selLength = edPicText.SelectionLength;
            var selChanged = false;
            var changed = "";
            var newText = edPicText.Text;
            if (string.IsNullOrEmpty(_oldPicText))
            {
                changed = newText;
            }
            else if (!string.IsNullOrEmpty(newText))
            {
                var i = 0;
                while (i < _oldPicText.Length && i < newText.Length && _oldPicText[i] == newText[i]) i++;
                if (i > 0) i--;
                while (i > 0 && (Char.IsLetterOrDigit(newText[i]) || newText[i] == '-')) i--;

                var oldTextRev = Util.Reverse(_oldPicText);
                var newTextRev = Util.Reverse(newText);
                var j = 0;
                while (j < oldTextRev.Length && j < newTextRev.Length && oldTextRev[j] == newTextRev[j]) j++;
                if (j > 0) j--;
                while (j > 0 && (Char.IsLetterOrDigit(newTextRev[j]) || newTextRev[j] == '-')) j--;
                j = newText.Length - j - 1;

                if (i < j) changed = newText.Substring(i, j - i);

                edPicText.Select(i, j - i);
                edPicText.SelectionColor = Color.Black;
                edPicText.SelectionBackColor = Color.White;

                selChanged = true;
            }

            var uText = edPicText.Text.ToUpper();
            var chWords = GetWords(changed, false);
            foreach (var chWord in chWords)
            {
                var words = _ctx.Database.SqlQuery<Words>(
                    $"select distinct w.* from Words w where w.Word = '{chWord.Name}'").ToList();
                if (words.Any())
                {
                    SetWordColor(edPicText, words.First(), uText);
                }
                else
                {
                    SetWordColor(edPicText, new Words() { Word = chWord.Name, Cnt = 0 }, uText);
                }
                selChanged = true;
            }
            if (selChanged)
            {
                edPicText.SelectionStart = selStart;
                edPicText.SelectionLength = selLength;
            }

            _oldPicText = newText;
        }

        private int GetNType(string type)
        {
            if (type == "s") return 10;
            if (type == "fc") return 20;
            if (type == "f") return 30;
            if (type == "c") return 40;
            if (type == "fd") return 50;
            if (type == "fr") return 60;
            if (type == "m") return 70;
            if (type == "dc") return 80;
            if (type == "d") return 90;
            if (type == "k") return 100;
            if (type == "p") return 110;
            return 120;
        }


    }


}
