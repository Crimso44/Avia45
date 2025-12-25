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
using System.Windows.Forms;
using static Aik2.Util;

namespace Aik2
{
    public partial class Form1 : Form
    {
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsPic;
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsLink;
        private Position _picPosition = Position.Empty;
        private Position _linkPosition = Position.Empty;
        private GridPicController _gridPicController = null;
        private GridLinkController _gridLinkController = null;
        private PicDto _selectedPic = null;
        private LinkDto _selectedLink = null;
        private string _oldPicText;
        private bool _picChanging = false;
        private bool _picTextChanging;
        private bool _picTextChanged;
        private bool _needLoadArt = false;
        private bool _needLoadCraft = false;
        private List<PicDto> _pics;
        private List<LinkDto> _links;
        private int? _lockedPicId = null;
        private bool _isPicReadonly = false;
        private string serialFilter = "";

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

            var editBool = SourceGrid.Cells.Editors.Factory.Create(typeof(bool));

            gridPic.ColumnsCount = 18;

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
            gridPic[0, 7] = new SourceGrid.Cells.ColumnHeader("Links");
            _editorsPic.Add(null);
            gridPic[0, 8] = new SourceGrid.Cells.ColumnHeader("Art");
            _editorsPic.Add(_editArt);
            gridPic[0, 9] = new SourceGrid.Cells.ColumnHeader("Craft");
            _editorsPic.Add(_editCraft678);
            gridPic[0, 10] = new SourceGrid.Cells.ColumnHeader("Grp");
            _editorsPic.Add(editStr100);
            gridPic[0, 11] = new SourceGrid.Cells.ColumnHeader("Serial");
            _editorsPic.Add(null);
            gridPic[0, 12] = new SourceGrid.Cells.ColumnHeader("Serial2");
            _editorsPic.Add(null);
            gridPic[0, 13] = new SourceGrid.Cells.ColumnHeader("SText");
            _editorsPic.Add(null);
            gridPic[0, 14] = new SourceGrid.Cells.ColumnHeader("Copyright");
            _editorsPic.Add(editBool);
            gridPic[0, 15] = new SourceGrid.Cells.ColumnHeader("MultiCraft");
            _editorsPic.Add(editBool);
            gridPic[0, 16] = new SourceGrid.Cells.ColumnHeader("ArtId");
            gridPic.Columns[16].Visible = false;
            gridPic[0, 17] = new SourceGrid.Cells.ColumnHeader("CraftId");
            gridPic.Columns[17].Visible = false;

            for (var i = 1; i < gridPic.ColumnsCount; i++)
            {
                var key = $"gridPic:ColumnWidth:{i}";
                if (_config.ContainsKey(key))
                {
                    gridPic.Columns[i].Width = int.Parse(_config[key]);
                }
            }

            _gridPicController = new GridPicController(this);

            gridPic.Selection.CellGotFocus += PicCellGotFocus;
            gridPic.Columns.ColumnWidthChanged += PicColumnWidthChanged;

        }


        public void LoadPics(bool isFromOtherTabs)
        {
            lWorking.Visible = true;
            Application.DoEvents();

            var PicsQry = _ctx.vwPics.AsNoTracking().AsQueryable();

            var flt = false;
            if (chPicSelArt.Checked && (_selectedPic != null || _selectedArt != null))
            {
                var artId = isFromOtherTabs ? _selectedArt?.ArtId ?? _selectedPic.ArtId : _selectedPic?.ArtId ?? _selectedArt.ArtId;
                PicsQry = PicsQry.Where(x => x.ArtId == artId);
                flt = true;
            }
            if (chPicSelCraft.Checked && (_selectedPic != null || _selectedCraft != null))
            {
                var craftId = isFromOtherTabs ? _selectedCraft?.CraftId ?? _selectedPic.CraftId : _selectedPic?.CraftId ?? _selectedCraft.CraftId;
                PicsQry = PicsQry.Where(x => x.CraftId == craftId);
                flt = true;
            }

            List<string> lTxt = null;
            List<string> lTxt2 = null;
            IQueryable<Pics> xPicsQry = null;
            IQueryable<Pics> xPicsQry2 = null;

            if (_filterOn && (!string.IsNullOrEmpty(_filter.Text) || !string.IsNullOrEmpty(_filter.Text2)) && _filter.InText)
            {
                lTxt = ExtractFilters(_filter.Text, _filter.CaseSensitive);
                lTxt2 = ExtractFilters(_filter.Text2, _filter.CaseSensitive);
                if (lTxt.Any() || lTxt2.Any())
                {
                    xPicsQry = _ctx.Pics.AsNoTracking().AsQueryable();
                    xPicsQry2 = _ctx.Pics.AsNoTracking().AsQueryable();
                    if (lTxt.Any())
                    {
                        foreach (var t in lTxt)
                        {
                            xPicsQry = xPicsQry.Where(x => (x.Text).Contains(t));
                        }
                    }
                    if (lTxt2.Any())
                    {
                        foreach (var t in lTxt2)
                        {
                            xPicsQry2 = xPicsQry2.Where(x => (x.Text).Contains(t));
                        }
                    }
                    if (lTxt.Any() && lTxt2.Any())
                        PicsQry = PicsQry
                            .Where(x => xPicsQry.Select(y => y.PicId).Contains(x.PicId) ||
                                        xPicsQry2.Select(y => y.PicId).Contains(x.PicId));
                    else if (lTxt.Any())
                        PicsQry = PicsQry.Where(x => xPicsQry.Select(y => y.PicId).Contains(x.PicId));
                    else //if (lTxt2.Any())
                        PicsQry = PicsQry.Where(x => xPicsQry2.Select(y => y.PicId).Contains(x.PicId));
                }
            }

            if (!flt)
            {
                PicsQry = PicsQry.Where(x => false);
            }
            PicsQry = PicsQry.OrderBy(x => x.CraftId).ThenBy(x => x.NType).ThenBy(x => x.NNN);

            var Pics = PicsQry.ToList();

            if (_filterOn && (!string.IsNullOrEmpty(_filter.Text) || !string.IsNullOrEmpty(_filter.Text2)) && _filter.InText && _filter.CaseSensitive)
            {
                Pics = Pics.Where(x =>
                {
                    var pic = xPicsQry.First(y => y.PicId == x.PicId);
                    var s = $" {pic.Text} ";
                    if (!_filter.CaseSensitive) s = s.ToLower();
                    var isOk = false;
                    foreach (var t in lTxt)
                    {
                        var found = false;
                        var i = s.IndexOf(t);
                        while (i >= 0)
                        {
                            if (!_filter.WholeWords) { found = true; break; };
                            if (!Char.IsLetterOrDigit(s[i - 1]) && !Char.IsLetterOrDigit(s[i + t.Length])) { found = true; break; };
                            i = s.IndexOf(t, i + 1);
                        }
                        if (found) isOk = true;
                        else break;
                    }
                    if (isOk) return true;

                    pic = xPicsQry2.First(y => y.PicId == x.PicId);
                    s = $" {pic.Text} ";
                    isOk = false;
                    foreach (var t in lTxt2)
                    {
                        var found = false;
                        var i = s.IndexOf(t);
                        while (i >= 0)
                        {
                            if (!_filter.WholeWords) { found = true; break; };
                            if (!Char.IsLetterOrDigit(s[i - 1]) && !Char.IsLetterOrDigit(s[i + t.Length])) { found = true; break; };
                            i = s.IndexOf(t, i + 1);
                        }
                        if (found) isOk = true;
                        else break;
                    }
                    return isOk;
                }).ToList();
            }

            if (!string.IsNullOrEmpty(serialFilter))
            {
                Pics = Pics.Where(x =>
                {
                    var isOk = _ctx.Serials.Any(y => y.PicId == x.PicId && y.Serial == serialFilter);
                    return isOk;
                }).ToList();
            }

            _pics = Mapper.Map<List<PicDto>>(Pics);
            lPicCnt.Text = _pics.Count.ToString();

            var saved = -1;
            var savedCol = 1;
            var empty = false;
            if (_picPosition != Position.Empty && _picPosition.Row > 0)
            {
                saved = (int)gridPic[_picPosition.Row, Const.Columns.Pic.PicId].Value;
                savedCol = _picPosition.Column;
            }

            gridPic.RowsCount = _pics.Count + 1;

            var focused = false;
            if (_pics.Any())
            {
                for (var r = 1; r <= _pics.Count; r++)
                {
                    var Pic = _pics[r - 1];
                    UpdatePicRow(Pic, r, false);

                    if (Pic.PicId == saved && _picPosition != Position.Empty)
                    {
                        var focusPosn = new Position(r, _picPosition.Column);
                        gridPic.Selection.Focus(focusPosn, true);
                        _picPosition = focusPosn;
                        focused = true;
                    }
                }
                ShowPicImage();
            }
            else
            {
                gridPic.RowsCount = 2;
                var Pic = new PicDto() { PicId = -1 };
                _pics.Add(Pic);
                UpdatePicRow(Pic, 1, true);
                empty = true;
            }
            if (!focused)
            {
                if (empty) savedCol = 1;
                _picPosition = new Position(1, savedCol);
                gridPic.Selection.Focus(_picPosition, true);
            }

            gridPic.Refresh();

            bUp.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bUp10.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bDown.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bDown10.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bHere.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;

            lWorking.Visible = false;
            Application.DoEvents();
        }

        public void InitLinkGrid()
        {
            _editorsLink = new List<SourceGrid.Cells.Editors.EditorBase>();

            var editStr20 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr20.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 20); };

            gridLink.ColumnsCount = 5;

            gridLink.RowsCount = 0;
            gridLink.FixedRows = 0;

            _editorsLink.Add(null); //LinkId
            gridLink.Columns[0].Visible = false;
            _editorsLink.Add(null); //Pic1
            gridLink.Columns[1].Visible = false;
            _editorsLink.Add(null); //Pic2
            gridLink.Columns[2].Visible = false;
            _editorsLink.Add(editStr20); //Type1
            _editorsLink.Add(editStr20); //Type2

            for (var i = 1; i < gridLink.ColumnsCount; i++)
            {
                var key = $"gridLink:ColumnWidth:{i}";
                if (_config.ContainsKey(key))
                {
                    gridLink.Columns[i].Width = int.Parse(_config[key]);
                }
            }

            _gridLinkController = new GridLinkController(this);

            gridLink.Selection.CellGotFocus += LinkCellGotFocus;
            gridLink.Columns.ColumnWidthChanged += LinkColumnWidthChanged;

        }

        public void LoadLinks()
        {
            picLinkImage.Visible = false;
            lArt2.Text = "";
            lCraft2.Text = "";

            var LinksQry = _ctx.Links.AsQueryable();

            var flt = false;
            if (_selectedPic != null)
            {
                LinksQry = LinksQry.Where(x => x.Pict1 == _selectedPic.PicId || x.Pict2 == _selectedPic.PicId);
                flt = true;
            }
            if (!flt)
            {
                LinksQry = LinksQry.Where(x => false);
            }

            var Links = LinksQry.ToList();
            _links = Mapper.Map<List<LinkDto>>(Links);

            gridLink.RowsCount = _links.Count;

            if (_links.Any())
            {
                for (var r = 0; r < _links.Count; r++)
                {
                    var Link = _links[r];
                    UpdateLinkRow(Link, r);
                }
            }
            else
            {
                gridLink.RowsCount = 1;
                var Link = new LinkDto() { LinkId = -1 };
                _links.Add(Link);
                UpdateLinkRow(Link, 0);
            }

            _linkPosition = new Position(0, 1);
            gridLink.Selection.Focus(_linkPosition, true);
            LinkCellGotFocus(null, new ChangeActivePositionEventArgs(_linkPosition, _linkPosition));

            gridLink.Refresh();

            RefreshSerials();
        }



        public void UpdatePicRow(PicDto Pic, int r, bool isShowImage)
        {
            gridPic[r, 0] = new SourceGrid.Cells.Cell(Pic.PicId, _editorsPic[0]);
            if (Pic.PicId == -1)
            {
                gridPic[r, 1] = new SourceGrid.Cells.Cell("");
                return;
            }
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
            gridPic[r, 7] = new SourceGrid.Cells.Cell(Pic.XType, _editorsPic[7]);
            var artName = "";
            var art = _arts.FirstOrDefault(x => x.Id == Pic.ArtId);
            if (art != null) artName = art.Name;
            gridPic[r, 8] = new SourceGrid.Cells.Cell(artName, _editorsPic[8]);
            gridPic[r, 8].AddController(_gridPicController);
            var craftName = "";
            var craft = _crafts678.FirstOrDefault(x => x.CraftId == Pic.CraftId);
            if (craft != null) craftName = craft.FullName;
            gridPic[r, 9] = new SourceGrid.Cells.Cell(craftName, _editorsPic[9]);
            gridPic[r, 9].AddController(_gridPicController);
            gridPic[r, 10] = new SourceGrid.Cells.Cell(Pic.Grp, _editorsPic[10]);
            gridPic[r, 10].AddController(_gridPicController);
            gridPic[r, 11] = new SourceGrid.Cells.Cell(Pic.Serial, _editorsPic[11]);
            gridPic[r, 11].AddController(_gridPicController);
            gridPic[r, 11].View = Pic.needCheck == 2 ? _redCraftCellView : Pic.needCheck == 1 ? _greenCraftCellView : _normCraftCellView;
            gridPic[r, 12] = new SourceGrid.Cells.Cell(Pic.Serial2, _editorsPic[12]);
            gridPic[r, 12].AddController(_gridPicController);
            gridPic[r, 12].View = Pic.needCheck == 2 ? _redCraftCellView : Pic.needCheck == 1 ? _greenCraftCellView : _normCraftCellView;
            gridPic[r, 13] = new SourceGrid.Cells.Cell(Pic.SText, _editorsPic[13]);
            gridPic[r, 13].AddController(_gridPicController);
            gridPic[r, 14] = new SourceGrid.Cells.CheckBox("", Pic.Copyright ?? false);
            gridPic[r, 14].AddController(_gridPicController);
            gridPic[r, 15] = new SourceGrid.Cells.CheckBox("", Pic.Multiplane ?? false);
            gridPic[r, 15].AddController(_gridPicController);
            gridPic[r, 16] = new SourceGrid.Cells.Cell(Pic.ArtId);
            gridPic[r, 17] = new SourceGrid.Cells.Cell(Pic.CraftId);

            if (isShowImage)
            {
                ShowPicImage();
            }
        }

        public void UpdateLinkRow(LinkDto Link, int r)
        {
            gridLink[r, 0] = new SourceGrid.Cells.Cell(Link.LinkId, _editorsLink[0]);
            if (Link.LinkId == -1)
            {
                gridLink[r, 3] = new SourceGrid.Cells.Cell("");
                return;
            }
            gridLink[r, 1] = new SourceGrid.Cells.Cell(_selectedPic.PicId == Link.Pict1 ? Link.Pict1 : Link.Pict2, _editorsLink[1]);
            gridLink[r, 2] = new SourceGrid.Cells.Cell(_selectedPic.PicId == Link.Pict1 ? Link.Pict2 : Link.Pict1, _editorsLink[2]);
            gridLink[r, 3] = new SourceGrid.Cells.Cell(_selectedPic.PicId == Link.Pict1 ? Link.Type1 : Link.Type2, _editorsLink[3]);
            gridLink[r, 3].AddController(_gridLinkController);
            gridLink[r, 4] = new SourceGrid.Cells.Cell(_selectedPic.PicId == Link.Pict1 ? Link.Type2 : Link.Type1, _editorsLink[4]);
            gridLink[r, 4].AddController(_gridLinkController);
        }


        public PicDto GetPicFromTable(int r)
        {
            var pic = new PicDto()
            {
                PicId = (int)gridPic[r, 0].Value,
                XPage = (string)gridPic[r, 1].Value,
                NN = (string)gridPic[r, 2].Value,
                NNN = (int?)gridPic[r, 3].Value,
                Type = (string)gridPic[r, 4].Value,
                NType = (int?)gridPic[r, 5].Value,
                Path = (string)gridPic[r, 6].Value,
                Grp = (string)gridPic[r, 10].Value,
                Copyright = (bool?)gridPic[r, 14].Value,
                Multiplane = (bool?)gridPic[r, 15].Value,
                ArtId = (int)gridPic[r, 16].Value,
                CraftId = (int)gridPic[r, 17].Value
            };
            return pic;
        }

        public LinkDto GetLinkFromTable(int r)
        {
            var link = new LinkDto()
            {
                LinkId = (int)gridLink[r, 0].Value,
                Pict1 = (int)gridLink[r, 1].Value,
                Pict2 = (int)gridLink[r, 2].Value,
                Type1 = (string)gridLink[r, 3].Value,
                Type2 = (string)gridLink[r, 4].Value
            };
            return link;
        }

        private class GridPicController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly Form1 _form;

            public GridPicController(Form1 form)
            {
                _form = form;
            }

            public override void OnEditStarting(CellContext sender, System.ComponentModel.CancelEventArgs e)
            {
                base.OnEditStarting(sender, e);

                if (_form._isPicReadonly)
                {
                    e.Cancel = true;
                }
            }

            public override void OnEditStarted(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnEditStarted(sender, e);
                _form.SearchModeOff();
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                if (_form._picChanging) return;
                try
                {
                    _form._picChanging = true;

                    _form.StoreEditedPicText();
                    var row = sender.Position.Row;
                    SourceGrid.Cells.Cell cell = (SourceGrid.Cells.Cell)sender.Cell;
                    var val = (string)cell.DisplayText;

                    switch (sender.Position.Column)
                    {
                        case Const.Columns.Pic.Craft:
                            _form.gridPic[row, Const.Columns.Pic.CraftId].Value = string.IsNullOrEmpty(val) ? null :
                                _form._crafts678.SingleOrDefault(x => x.FullName == val)?.CraftId;
                            break;
                        case Const.Columns.Pic.Art:
                            _form.gridPic[row, Const.Columns.Pic.ArtId].Value = string.IsNullOrEmpty(val) ? null :
                                _form._arts.SingleOrDefault(x => x.Name == val)?.Id;
                            break;
                        case Const.Columns.Pic.Type:
                            _form.gridPic[row, Const.Columns.Pic.NType].Value = Util.GetNType(val);
                            break;
                        case Const.Columns.Pic.Path:
                            _form.ShowPicImage();
                            break;
                    }

                    var isForce = false;
                    var picDto = _form.GetPicFromTable(row);
                    if (picDto.PicId == 0)
                    {
                        var entity = Mapper.Map<Pics>(picDto);
                        _form._ctx.Pics.Add(entity);
                        _form._ctx.SaveChanges();
                        picDto.PicId = entity.PicId;
                        _form.gridPic[row, Const.Columns.Pic.PicId].Value = entity.PicId;
                        isForce = true;
                    }
                    else
                    {
                        var entity = _form._ctx.Pics.Single(x => x.PicId == picDto.PicId);
                        Mapper.Map(picDto, entity);
                        _form._ctx.SaveChanges();
                    }

                    _form._selectedPic = picDto;
                    _form._pics[row - 1] = picDto;

                    _form.CheckPicSort(sender.Position, isForce);

                }
                finally
                {
                    _form._picChanging = false;
                }
            }
        }

        private void CheckPicSort(Position pos, bool isForce)
        {
            var sortIndexes = new int[] { Const.Columns.Pic.NType, Const.Columns.Pic.NNN };
            var sortXIndexes = new int[] { Const.Columns.Pic.Type, Const.Columns.Pic.NType, Const.Columns.Pic.NNN };

            if (isForce || sortXIndexes.Contains(pos.Column))
            {
                var sortVal = "";
                var rNew = pos.Row;
                foreach (var col in sortIndexes) sortVal += PadLeft(gridPic[pos.Row, col].DisplayText, '0', 15) + " ";
                if (pos.Row < gridPic.RowsCount)
                {
                    var found = false;
                    for (var r = pos.Row + 1; r < gridPic.RowsCount; r++)
                    {
                        var sortRVal = "";
                        foreach (var col in sortIndexes) sortRVal += PadLeft(gridPic[r, col].DisplayText, '0', 15) + " ";
                        if (string.Compare(sortVal, sortRVal) <= 0)
                        {
                            rNew = r - 1;
                            found = true;
                            break;
                        }
                    }
                    if (!found) rNew = gridPic.RowsCount - 1;
                }
                if (rNew == pos.Row && pos.Row > 1)
                {
                    var found = false;
                    for (var r = pos.Row - 1; r >= 1; r--)
                    {
                        var sortRVal = "";
                        foreach (var col in sortIndexes) sortRVal += PadLeft(gridPic[r, col].DisplayText, '0', 15) + " ";
                        if (string.Compare(sortRVal, sortVal) <= 0)
                        {
                            rNew = r + 1;
                            found = true;
                            break;
                        }
                    }
                    if (!found) rNew = 1;
                }

                if (rNew != pos.Row)
                {
                    gridPic.Rows.Move(pos.Row, rNew);
                    _pics.Move(pos.Row - 1, rNew - 1);
                    var focusPosn = new Position(rNew, pos.Column == Const.Columns.Pic.NType ? Const.Columns.Pic.Type : pos.Column);
                    gridPic.Selection.Focus(focusPosn, true);
                }

            }

        }

        private class GridLinkController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly Form1 _form;

            public GridLinkController(Form1 form)
            {
                _form = form;
            }

            public override void OnEditStarting(CellContext sender, System.ComponentModel.CancelEventArgs e)
            {
                base.OnEditStarting(sender, e);

                if (_form._isPicReadonly)
                {
                    e.Cancel = true;
                }
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                var row = sender.Position.Row;
                SourceGrid.Cells.Cell cell = (SourceGrid.Cells.Cell)sender.Cell;
                var val = (string)cell.DisplayText;

                var linkDto = _form.GetLinkFromTable(row);
                if (linkDto.LinkId == 0)
                {
                    var entity = Mapper.Map<Links>(linkDto);
                    _form._ctx.Links.Add(entity);
                    _form._ctx.SaveChanges();
                    linkDto.LinkId = entity.LinkId;
                    _form.gridLink[row, Const.Columns.Link.LinkId].Value = entity.LinkId;
                }
                else
                {
                    var entity = _form._ctx.Links.Single(x => x.LinkId == linkDto.LinkId);
                    Mapper.Map(linkDto, entity);
                    _form._ctx.SaveChanges();
                }
                _form._selectedLink = linkDto;
                _form._links[row] = linkDto;


            }
        }

        private void PicCellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            DoPicCellGotFocus(e.NewFocusPosition);
        }

        private void DoPicCellGotFocus(Position newPos)
        {
            _picPosition = newPos;
            var pic = _pics[_picPosition.Row - 1];
            if (pic.PicId == -1) return;

            _needLoadArt = true;
            _needLoadCraft = true;
            StoreEditedPicText();

            if (_selectedPic == null || _selectedPic.PicId == 0 || _selectedPic.PicId != pic.PicId)
            {
                _picTextChanging = true;
                edPicText.Text = "";
                _oldPicText = edPicText.Text;
                edPicText.SelectionStart = 0;
                edPicText.SelectionLength = 1;
                edPicText.SelectionFont = new Font(edPicText.SelectionFont, FontStyle.Regular);
                edPicText.SelectionColor = Color.Black;
                edPicText.SelectionBackColor = Color.White;
                _picTextChanging = false;
                _picTextChanged = false;

                var picText = _ctx.Pics.SingleOrDefault(x => x.PicId == pic.PicId);
                if (picText != null)
                {
                    _selectedPic = pic;
                    _picTextChanging = true;
                    edPicText.Text = picText.Text ?? "";
                    _oldPicText = edPicText.Text;
                    ColorizeText(edPicText, false);
                    _picTextChanging = false;
                    _picTextChanged = false;
                } else
                {
                    _selectedPic = GetPicFromTable(_picPosition.Row);
                }


                LoadLinks();

                if (_searchMode && !_searchChanging)
                {
                    _searchString = "";
                    lInfo.Text = $"Поиск: {_searchString}";
                }

                var artId = (int)gridPic[_picPosition.Row, Const.Columns.Pic.ArtId].Value;
                var art = Mapper.Map<ArtDto>(_ctx.Arts.FirstOrDefault(x => x.ArtId == artId));
                if (art != null) lArt.Text = art.FullName;
                var craftId = (int)gridPic[_picPosition.Row, Const.Columns.Pic.CraftId].Value;
                var craft = Mapper.Map<CraftDto>(_ctx.Crafts.FirstOrDefault(x => x.CraftId == craftId));
                if (craft != null) lCraft.Text = craft.FullName;

                _isPicReadonly = Const.Sources.ReadOnly.Contains(craft.Source);
                edPicText.ReadOnly = _isPicReadonly;

                ShowPicImage();
            }
        }

        private void StoreEditedPicText()
        {
            if (_selectedPic != null && _picTextChanged)
            {
                var picOld = _ctx.Pics.SingleOrDefault(x => x.PicId == _selectedPic.PicId);
                if (picOld != null)
                {
                    picOld.Text = _oldPicText;
                    _ctx.SaveChanges();

                    _ctx.Database.ExecuteSqlCommand($"delete from WordLinks where PicId ={picOld.PicId}");
                    var wordsList = GetWords(_oldPicText, true);
                    foreach (var word in wordsList)
                    {
                        _ctx.Database.ExecuteSqlCommand($"insert into WordLinks (WordId, PicId) Values ({word.Id}, {picOld.PicId})");
                    }
                    _ctx.Database.ExecuteSqlCommand($"delete from Words where Cnt = 0");
                }
            }
            _picTextChanged = false;
        }

        private void ShowPicImage()
        {
            picPicImage.Visible = false;
            lblV.ForeColor = Color.LightGray;
            lblU.ForeColor = Color.LightGray;
            lblG.ForeColor = Color.LightGray;
            lblL.ForeColor = Color.LightGray;
            lbl1.ForeColor = Color.LightGray;
            lblP.ForeColor = Color.LightGray;
            lblWingsEngs.Text = "";
            var craft = _craftDtos.FirstOrDefault(x => x.CraftId == _selectedPic?.CraftId);
            if (craft == null && _selectedPic != null)
            {
                craft = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().FirstOrDefault(x => x.CraftId == _selectedPic.CraftId));
            }
            if (craft != null)
            {
                lCraft.Text = craft.FullName;
                lblV.ForeColor = craft.Vert ?? false ? Color.Black : Color.LightGray;
                lblU.ForeColor = craft.Uav ?? false ? Color.Black : Color.LightGray;
                lblG.ForeColor = craft.Glider ?? false ? Color.Black : Color.LightGray;
                lblL.ForeColor = craft.LL ?? false ? Color.Black : Color.LightGray;
                lbl1.ForeColor = craft.Single ?? false ? Color.Black : Color.LightGray;
                lblP.ForeColor = craft.Proj ?? false ? Color.Black : Color.LightGray;
                lblWingsEngs.Text = $"{craft.Wings} / {craft.Engines}";

                if (gridPic[_picPosition.Row, Const.Columns.Pic.Path] != null && !string.IsNullOrEmpty((string)gridPic[_picPosition.Row, Const.Columns.Pic.Path].Value))
                {
                    try
                    {
                        var picPath = $"{_imagesPath}Images{craft.Source}\\{(string)gridPic[_picPosition.Row, Const.Columns.Pic.Path].Value}";
                        using (var bmpTemp = new Bitmap(picPath))
                        {
                            picPicImage.Image = new Bitmap(bmpTemp);
                        }
                        //picPicImage.Load(picPath);
                        picPicImage.Visible = true;
                    } catch { }
                }
            }
        }

        private void LinkCellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            _linkPosition = e.NewFocusPosition;
            var link = _links[_linkPosition.Row];
            if (link.LinkId == -1) return;

            if (_selectedLink == null || _selectedLink.LinkId != link.LinkId)
            {
                _selectedLink = link;
            }

            picLinkImage.Visible = false;
            try
            {
                var picId = link.Pict1 == _selectedPic.PicId ? link.Pict2 : link.Pict1;
                var pic2 = _ctx.Pics.Single(x => x.PicId == picId);
                var craft = _craftDtos.FirstOrDefault(x => x.CraftId == pic2.CraftId);
                if (craft != null)
                {
                    var picPath = $"{_imagesPath}Images{craft.Source}\\{pic2.Path}";
                    using (var bmpTemp = new Bitmap(picPath))
                    {
                        picLinkImage.Image = new Bitmap(bmpTemp);
                    }
                    //picLinkImage.Load(picPath);
                    picLinkImage.Visible = true;
                    lCraft2.Text = craft.FullName;
                }
                var art = _artDtos.FirstOrDefault(x => x.ArtId == pic2.ArtId);
                if (art != null)
                {
                    lArt2.Text = art.FullName;
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

        private void LinkColumnWidthChanged(object sender, ColumnInfoEventArgs e)
        {
            if (sender == gridPic.Columns)
            {
                _config[$"gridLink:ColumnWidth:{e.Column.Index}"] = e.Column.Width.ToString();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }
        }

        private void DoPicSearch(Keys c, bool isText)
        {
            var oldSearch = _searchString;
            var isDef = false;

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
                if (!found && isDef) _searchString = oldSearch;
            }
            lInfo.Text = $"Поиск: {_searchString}";
        }

        private void gridPic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (gridPic.Focused)
            {
                var pos = gridPic.Selection.ActivePosition;
                if (pos != Position.Empty)
                {
                    if (e.KeyCode == Keys.Insert && e.Modifiers == Keys.None)
                    {
                        SearchModeOff();
                        var fromEmpty = (int)gridPic[pos.Row, Const.Columns.Pic.PicId].Value == -1;
                        var Pic = new PicDto();
                        if (fromEmpty)
                        {
                            Pic.XPage = "1";
                            Pic.Type = "f";
                            Pic.NType = GetNType("f");
                            Pic.NN = "1";
                            Pic.NNN = _ctx.vwPics.AsNoTracking().OrderByDescending(x => x.NNN).First().NNN + 1;
                            Pic.CraftId = _selectedCraft?.CraftId ?? -1;
                            Pic.ArtId = _selectedArt?.ArtId ?? -1;
                        }
                        else
                        {
                            Pic.XPage = (string)gridPic[pos.Row, Const.Columns.Pic.XPage].Value;
                            Pic.Type = (string)gridPic[pos.Row, Const.Columns.Pic.Type].Value;
                            Pic.NType = (int?)gridPic[pos.Row, Const.Columns.Pic.NType].Value;
                            if (int.TryParse((string)gridPic[pos.Row, Const.Columns.Pic.NN].Value, out var nn))
                                Pic.NN = (nn + 1).ToString();
                            else
                                Pic.NN = "1";
                            Pic.NNN = _ctx.vwPics.AsNoTracking().OrderByDescending(x => x.NNN).First().NNN + 1;
                            Pic.CraftId = (int)gridPic[pos.Row, Const.Columns.Pic.CraftId].Value;
                            Pic.ArtId = (int)gridPic[pos.Row, Const.Columns.Pic.ArtId].Value;
                        };
                        _pics.Insert(pos.Row - 1, Mapper.Map<PicDto>(Pic));
                        gridPic.Rows.Insert(pos.Row);
                        if (fromEmpty)
                        {
                            _pics.RemoveAt(1);
                            gridPic.Rows.Remove(1);
                        }
                        UpdatePicRow(Pic, pos.Row, true);
                        DoPicCellGotFocus(pos);
                        CheckPicSort(pos, true);
                    }
                    else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                    {
                        if (MessageBox.Show("Delete pic?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                var pic = _pics[pos.Row - 1];
                                if (pic.PicId > 0)
                                {
                                    var links = _ctx.Links.Where(x => x.Pict1 == pic.PicId || x.Pict2 == pic.PicId).ToList();
                                    if (links.Any())
                                    {
                                        MessageBox.Show("Невозможно удалить: есть связи.");
                                        return;
                                    }

                                    _ctx.Database.ExecuteSqlCommand($"delete from WordLinks where PicId ={pic.PicId}");
                                    var entity = _ctx.Pics.Single(x => x.PicId == pic.PicId);
                                    _ctx.Pics.Remove(entity);
                                    _ctx.SaveChanges();
                                }
                                gridPic.Rows.Remove(pos.Row);
                                _pics.RemoveAt(pos.Row - 1);
                                if (_pics.Count == 0)
                                {
                                    gridPic.RowsCount = 2;
                                    var Pic = new PicDto() { PicId = -1 };
                                    _pics.Add(Pic);
                                    UpdatePicRow(Pic, 1, true);
                                    DoPicCellGotFocus(new Position(1, 0));
                                }
                                else
                                {
                                    var row = pos.Row < gridPic.RowsCount ? pos.Row : gridPic.RowsCount - 1;
                                    var focusPosn = new Position(row, _picPosition.Column);
                                    gridPic.Selection.Focus(focusPosn, true);
                                    ShowPicImage();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(GetInnerestException(ex));
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Home && e.Modifiers == Keys.None)
                    {
                        var col = 0;
                        while (!gridPic.Columns[col].Visible) col++;
                        var focusPosn = new Position(pos.Row, col);
                        gridPic.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.None)
                    {
                        var col = gridPic.Columns.Count - 1;
                        while (!gridPic.Columns[col].Visible) col--;
                        var focusPosn = new Position(pos.Row, col);
                        gridPic.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.Home && e.Modifiers == Keys.Control)
                    {
                        var focusPosn = new Position(1, pos.Column);
                        gridPic.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.Control)
                    {
                        var focusPosn = new Position(gridPic.RowsCount - 1, pos.Column);
                        gridPic.Selection.Focus(focusPosn, true);
                    }
                }
            }
        }

        private void gridLink_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (gridLink.Focused)
            {
                var pos = gridLink.Selection.ActivePosition;
                if (pos != Position.Empty)
                {
                    if (e.KeyCode == Keys.Insert)
                    {
                        if (_lockedPicId.HasValue)
                        {
                            var fromEmpty = (int)gridLink[pos.Row, Const.Columns.Link.LinkId].Value == -1;
                            var Link = new LinkDto()
                            {
                                Pict1 = _selectedPic.PicId,
                                Pict2 = _lockedPicId.Value
                            };
                            _links.Insert(pos.Row, Mapper.Map<LinkDto>(Link));
                            gridLink.Rows.Insert(pos.Row);
                            if (fromEmpty)
                            {
                                _links.RemoveAt(1);
                                gridLink.Rows.Remove(1);
                            }
                            UpdateLinkRow(Link, pos.Row);
                            LinkCellGotFocus(null, new ChangeActivePositionEventArgs(pos, pos));
                        }
                    }
                    else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                    {
                        if (MessageBox.Show("Delete link?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var link = _links[pos.Row];
                            if (link.LinkId > 0)
                            {
                                var entity = _ctx.Links.Single(x => x.LinkId == link.LinkId);
                                _ctx.Links.Remove(entity);
                                _ctx.SaveChanges();
                            }
                            gridLink.Rows.Remove(pos.Row);
                            _links.RemoveAt(pos.Row);
                        }
                    }
                }
            }
        }

        private void pPicText_Resize(object sender, EventArgs e)
        {
            if (_init) return;
            _config["pPicTextHeight"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void pPicImg_Resize(object sender, EventArgs e)
        {
            if (_init) return;
            _config["pPicImgHeight"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }


        private void edPicText_TextChanged(object sender, EventArgs e)
        {
            if (_picTextChanging) return;
            _picTextChanging = true;

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
                j = newText.Length - j;

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
            _picTextChanging = false;
            _picTextChanged = true;
        }

        private void bLockPic_Click(object sender, EventArgs e)
        {
            _lockedPicId = _selectedPic?.PicId;
        }

        private void pLinkImage_Resize(object sender, EventArgs e)
        {
            if (pLinkTable.Width != pLinkImage.Width)
            {
                pLinkTable.Width = pLinkImage.Width;
                if (_init) return;
                _config["pLinkImgWidth"] = ((Panel)sender).Width.ToString();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }
        }

        private void pLinkTable_Resize(object sender, EventArgs e)
        {
            if (pLinkImage.Width != pLinkTable.Width)
            {
                pLinkImage.Width = pLinkTable.Width;
                if (_init) return;
                _config["pLinkImgWidth"] = ((Panel)sender).Width.ToString();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }
        }

        private void mnuAlt1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                gridPic.Focus();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                gridCraft.Focus();
            }
        }

        private void mnuAlt2_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                edPicText.Focus();
            }
        }

        private void mnuAlt3_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                gridLink.Focus();
            }
        }

        private void mnuAltS_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                cbSerial.Focus();
            }
        }

        private void mnuAltZ_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                cbSerial2.Focus();
            }
        }


        private void mnuPicsFillPath_Click(object sender, EventArgs e)
        {
            if (_selectedPic != null)
            {
                var pos = gridPic.Selection.ActivePosition;
                if (pos != Position.Empty)
                {
                    var art = _artDtos.Single(x => x.ArtId == _selectedPic.ArtId);
                    string path;
                    if (art.Mag.Trim() == "AK")
                    {
                        path = $"{art.Mag.Trim()}\\{art.Mag.Trim()}{art.IYear}-{art.IMonth.Trim()}\\{_selectedPic.XPage?.Trim()}-{_selectedPic.NN?.Trim()}.jpg";
                    }
                    else if (art.Mag.Trim() == "FT")
                    {
                        path = $"{art.Mag.Trim()}\\{art.Mag.Trim()}{art.IYear}\\{art.IMonth.Trim()}\\{_selectedPic.XPage?.Trim()}-{_selectedPic.NN?.Trim()}.jpg";
                    }
                    else if (art.Mag.Trim() == "IN")
                    {
                        path = $"{art.Mag.Trim()}\\{art.Mag.Trim()}-{art.IYear}\\{_selectedPic.XPage?.Trim()}-{_selectedPic.NN?.Trim()}.jpg";
                    }
                    else if (art.Mag.Trim() == "HI")
                    {
                        path = $"{art.Mag.Trim()}\\{art.Mag.Trim()}-{art.IYear}\\{_selectedPic.XPage?.Trim()}-{_selectedPic.NN?.Trim()}.jpg";
                    }
                    else if (art.Mag.Trim() == "IA")
                    {
                        path = $"{art.Mag.Trim()}\\{art.Mag.Trim()}-{art.IYear}\\{_selectedPic.XPage?.Trim()}-{_selectedPic.NN?.Trim()}.jpg";
                    }
                    else
                    {
                        int.TryParse(art.IMonth?.Trim() ?? "", out int artMonth);
                        var year = (art.IYear ?? 0) == 0 ? "" : PadLeft((art.IYear % 100).ToString(), '0', 2).ToString();
                        path = $"{art.Mag.Trim()}\\{art.Mag.Trim()}{year}-{artMonth}\\{_selectedPic.XPage?.Trim()}-{_selectedPic.NN?.Trim()}.jpg";
                    }
                    _selectedPic.Path = path;
                    gridPic[pos.Row, Const.Columns.Pic.Path].Value = path;

                    ShowPicImage();
                }
            }
        }

        private void MovePicUp(Position pos)
        {
            var nType = (int)gridPic[pos.Row, Const.Columns.Pic.NType].Value;
            if (pos.Row > 1 && nType == (int)gridPic[pos.Row - 1, Const.Columns.Pic.NType].Value)
            {
                var picFrom = _pics[pos.Row - 1];
                var nnnFrom = picFrom.NNN;
                var picTo = _pics[pos.Row - 2];
                var nnnTo = picTo.NNN;
                if (nnnFrom == nnnTo) nnnTo--;
                picFrom.NNN = nnnTo;
                picTo.NNN = nnnFrom;

                var entFrom = _ctx.Pics.FirstOrDefault(x => x.PicId == picFrom.PicId);
                var entTo = _ctx.Pics.FirstOrDefault(x => x.PicId == picTo.PicId);
                if (entFrom != null) entFrom.NNN = nnnTo;
                if (entTo != null) entTo.NNN = nnnFrom;
                _ctx.SaveChanges();

                _pics.Move(pos.Row - 1, pos.Row - 2);
                _picChanging = true;
                gridPic[pos.Row, Const.Columns.Pic.NNN].Value = nnnTo;
                gridPic[pos.Row - 1, Const.Columns.Pic.NNN].Value = nnnFrom;
                gridPic.Rows.Move(pos.Row, pos.Row - 1);
                _picChanging = false;
                var newPos = new Position(pos.Row - 1, pos.Column);
                gridPic.Selection.Focus(newPos, true);

                Application.DoEvents();
            }
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            var pos = _picPosition;
            if (pos != Position.Empty)
            {
                MovePicUp(pos);
            }
        }

        private void bUp10_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < 10; i++)
            {
                bUp_Click(sender, e);
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            var pos = _picPosition;
            if (pos != Position.Empty)
            {
                var nnn = (int)gridPic[pos.Row, Const.Columns.Pic.NType].Value;
                if (pos.Row < (gridPic.RowsCount - 1) && nnn == (int)gridPic[pos.Row + 1, Const.Columns.Pic.NType].Value)
                {
                    var picFrom = _pics[pos.Row - 1];
                    var nnnFrom = picFrom.NNN;
                    var picTo = _pics[pos.Row];
                    var nnnTo = picTo.NNN;
                    if (nnnFrom == nnnTo) nnnTo++;
                    picFrom.NNN = nnnTo;
                    picTo.NNN = nnnFrom;

                    var entFrom = _ctx.Pics.FirstOrDefault(x => x.PicId == picFrom.PicId);
                    var entTo = _ctx.Pics.FirstOrDefault(x => x.PicId == picTo.PicId);
                    if (entFrom != null) entFrom.NNN = nnnTo;
                    if (entTo != null) entTo.NNN = nnnFrom;
                    _ctx.SaveChanges();

                    _pics.Move(pos.Row - 1, pos.Row);
                    _picChanging = true;
                    gridPic[pos.Row, Const.Columns.Pic.NNN].Value = nnnTo;
                    gridPic[pos.Row + 1, Const.Columns.Pic.NNN].Value = nnnFrom;
                    gridPic.Rows.Move(pos.Row, pos.Row + 1);
                    _picChanging = false;
                    var newPos = new Position(pos.Row + 1, pos.Column);
                    gridPic.Selection.Focus(newPos, true);

                    Application.DoEvents();
                }
            }
        }

        private void bDown10_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < 10; i++)
            {
                bDown_Click(sender, e);
            }
        }

        private void bHere_Click(object sender, EventArgs e)
        {
            var pos = _picPosition;
            if (pos != Position.Empty)
            {
                var nType = (int)gridPic[pos.Row, Const.Columns.Pic.NType].Value;
                var r = pos.Row + 1;
                while (r - 1 < _pics.Count && _pics[r - 1].NType == nType) r++;
                r--;
                while (pos.Row != r)
                {
                    var newPos = new Position(r, pos.Column);
                    MovePicUp(newPos);
                    r--;
                }
                _ctx.SaveChanges();
                Util.DetachAllEntities(_ctx);
            }
        }

        private void chPicSelArt_Click(object sender, EventArgs e)
        {
            LoadPics(false);
        }

        private void unusedPicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var existImg = new List<string>();
            var srcs = new string[] { "1", "2", "5", "6", "7" };
            foreach(var src in srcs)
            {
                LoadDirectories(_imagesPath + $"Images{src}\\", $"Images{src}\\", existImg);
            }
            var baseImg = (
                from c in _ctx.vwCrafts
                join p in _ctx.vwPics on c.CraftId equals p.CraftId
                where srcs.Contains(c.Source)
                select ("Images" + c.Source + "\\" + p.Path)/*.ToLower()*/)
                .Distinct().ToList();
            baseImg.Sort();

            var unusedImg = new List<string>();
            foreach (var img in existImg)
            {
                var i = baseImg.BinaryIndexOf(img);
                if (i < 0)
                {
                    unusedImg.Add(img);
                }
            }

            foreach (var img in baseImg)
            {
                var i = existImg.BinaryIndexOf(img);
                if (i < 0)
                {
                    unusedImg.Add("-" + img);
                }
            }

            var frep = new fReport(_ctx, _imagesPath);
            frep.SaveButton.Text = "Удалить";
            frep.Mode = 1;
            foreach(var unused in unusedImg)
            {
                frep.ReportList.Items.Add(unused);
            }
            frep.Show(this);
        }

        private void LoadDirectories(string path, string shortPath, List<string> imgs)
        {
            var files = Directory.GetFiles(path, "*.jpg");
            foreach(var file in files)
            {
                imgs.InsertStringSorted((shortPath + Path.GetFileName(file))/*.ToLower()*/);
            }
            files = Directory.GetFiles(path, "*.gif");
            foreach (var file in files)
            {
                imgs.InsertStringSorted((shortPath + Path.GetFileName(file))/*.ToLower()*/);
            }
            var dirs = Directory.GetDirectories(path);
            foreach(var dir in dirs)
            {
                var shPath = shortPath + Path.GetFileName(dir) + "\\";
                LoadDirectories(dir, shPath, imgs);
            }
        }

        private void bNextCraft_Click(object sender, EventArgs e)
        {

            var i = _craftDtos.IndexOf(_selectedCraft);
            if (i >= 0 && i < (_craftDtos.Count - 1))
            {
                SelectCraft(_craftDtos[i + 1].CraftId);
            }
            LoadPics(true);
        }

        private void bPrevCraft_Click(object sender, EventArgs e)
        {
            var i = _craftDtos.IndexOf(_selectedCraft);
            if (i > 0)
            {
                SelectCraft(_craftDtos[i - 1].CraftId);
            }
            LoadPics(true);
        }

        private void LoadSerials()
        {
            var serials = _ctx.vwSerials.OrderBy(x => x.SerialCraft).Select(x => x.SerialCraft).ToArray();
            cbSerial.Items.Clear();
            cbSerial.Items.AddRange(serials);
            cbSerial2.Items.Clear();
            cbSerial2.Items.AddRange(serials);
        }

        private void cbSerial_KeyDown(object sender, KeyEventArgs e)
        {
            var cb = (ComboBox)sender;
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(cb.Text) && _selectedPic != null)
                {
                    var sText = cb.Text.Trim().ToUpper();
                    var i = sText.IndexOf(' ');
                    if (i > 0)
                    {
                        sText = sText.Substring(0, i);
                    }
                    var ser = new Serials()
                    {
                        Serial = sText,
                        PicId = _selectedPic.PicId,
                        IsSecondary = sender == cbSerial ? false : true,
                    };
                    _ctx.Serials.Add(ser);
                    try
                    {
                        _ctx.SaveChanges();
                    }
                    catch 
                    {
                        _ctx.Serials.Remove(ser);
                    }
                    LoadSerials();
                    RefreshSerials();
                }
                cb.Text = "";

                e.Handled = true;
            }
        }


        private void lbSerials_KeyDown(object sender, KeyEventArgs e)
        {
            var lb = (ListBox)sender;
            if (e.KeyCode == Keys.Delete)
            {
                if (lb.SelectedItem != null)
                {
                    var ser = lb.SelectedItem.ToString();
                    if (ser.IndexOf(' ') > 0)
                    {
                        ser = ser.Substring(0, ser.IndexOf(' '));
                    }
                    var toDelete = _ctx.Serials.FirstOrDefault(x => x.Serial == ser && x.PicId == _selectedPic.PicId);
                    if (toDelete != null)
                    {
                        _ctx.Serials.Remove(toDelete);
                        _ctx.SaveChanges();
                        LoadSerials();
                        RefreshSerials();
                    }
                }
            }
        }


        private void RefreshSerials()
        {
            lbSerials.Items.Clear();
            if (_selectedPic != null)
            {
                var serials = _ctx.Serials.Where(x => x.PicId == _selectedPic.PicId).OrderBy(x => x.Serial).Select(x => new { x.Serial, x.IsSecondary }).ToArray();
                var slist = new List<string>();
                var slist2 = new List<string>();
                foreach (var serial in serials)
                {
                    var s = serial.Serial;
                    var sx = (from ser in _ctx.Serials
                              join pic in _ctx.Pics on ser.PicId equals pic.PicId
                              where ser.Serial == serial.Serial
                              group ser by new { ser.Serial, pic.CraftId }
                              into grp
                              select new {grp.Key.Serial, grp.Key.CraftId, cnt = grp.Count()}).ToList();
                    if (sx.Count == 1 && sx[0].cnt > 1)
                    {
                        s = $"{s} ({sx[0].cnt})";
                    }
                    else if (sx.Count > 1)
                    {
                        var cnt1 = sx.Where(x => x.CraftId == _selectedCraft?.CraftId).Sum(x => x.cnt);
                        var cnt2 = sx.Where(x => x.CraftId != _selectedCraft?.CraftId).Sum(x => x.cnt);
                        s = $"{s} ({cnt1}+{cnt2})";
                    }
                    if (serial.IsSecondary)
                    {
                        slist2.Add(s);
                    }
                    else
                    {
                        slist.Add(s);
                    }
                }
                lbSerials.Items.Clear();
                lbSerials.Items.AddRange(slist.ToArray());
                lbSerials2.Items.Clear();
                lbSerials2.Items.AddRange(slist2.ToArray());
            }

        }

        private void lbSerials_DoubleClick(object sender, EventArgs e)
        {
            var lb = (ListBox)sender;
            if (string.IsNullOrEmpty(serialFilter) && lb.SelectedItem != null)
            {
                var s = lb.SelectedItem.ToString();
                var i = s.IndexOf(" ");
                if (i <= 0) return;
                s = s.Substring(0, i);
                serialFilter = s;
            }
            else
            {
                serialFilter = "";
            }
            LoadPics(false);
        }

    }
}
