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
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsLink;
        private Position _picPosition = Position.Empty;
        private Position _linkPosition = Position.Empty;
        private GridPicController _gridPicController = null;
        private GridLinkController _gridLinkController = null;
        private PicDto _selectedPic = null;
        private LinkDto _selectedLink = null;
        private string _oldPicText;
        private bool _picTextChanging;
        private bool _picTextChanged;
        private bool _needLoadArt = false;
        private bool _needLoadCraft = false;
        private List<PicDto> _pics;
        private List<LinkDto> _links;
        private int? _lockedPicId = null;

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

            gridPic.ColumnsCount = 14;

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
            gridPic[0, 11] = new SourceGrid.Cells.ColumnHeader("SText");
            _editorsPic.Add(null);
            gridPic[0, 12] = new SourceGrid.Cells.ColumnHeader("ArtId");
            gridPic.Columns[12].Visible = false;
            gridPic[0, 13] = new SourceGrid.Cells.ColumnHeader("CraftId");
            gridPic.Columns[13].Visible = false;

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
            if (!flt)
            {
                PicsQry = PicsQry.Where(x => false);
            }
            PicsQry = PicsQry.OrderBy(x => x.CraftId).ThenBy(x => x.NType).ThenBy(x => x.NNN);

            var Pics = PicsQry.ToList();
            _pics = Mapper.Map<List<PicDto>>(Pics);
            lPicCnt.Text = _pics.Count.ToString();

            var saved = -1;
            if (_picPosition != Position.Empty && _picPosition.Row > 0)
            {
                saved = (int)gridPic[_picPosition.Row, Const.Columns.Pic.PicId].Value;
            }

            gridPic.RowsCount = _pics.Count + 1;

            var focused = false;
            if (_pics.Any())
            {
                for (var r = 1; r <= _pics.Count; r++)
                {
                    var Pic = _pics[r - 1];
                    UpdatePicRow(Pic, r);

                    if (Pic.PicId == saved && _picPosition != Position.Empty)
                    {
                        var focusPosn = new Position(r, _picPosition.Column);
                        gridPic.Selection.Focus(focusPosn, true);
                        _picPosition = focusPosn;
                        focused = true;
                    }
                }
            }
            else
            {
                gridPic.RowsCount = 2;
                var Pic = new PicDto() { PicId = -1 };
                _pics.Add(Pic);
                UpdatePicRow(Pic, 1);
            }
            if (!focused)
            {
                _picPosition = new Position(1, 1);
                gridPic.Selection.Focus(_picPosition, true);
            }

            gridPic.Refresh();

            bUp.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bUp10.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bDown.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bDown10.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
            bHere.Enabled = chPicSelCraft.Checked && !chPicSelArt.Checked;
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
        }



        public void UpdatePicRow(PicDto Pic, int r)
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
            gridPic[r, 11] = new SourceGrid.Cells.Cell(Pic.SText, _editorsPic[11]);
            gridPic[r, 11].AddController(_gridPicController);
            gridPic[r, 12] = new SourceGrid.Cells.Cell(Pic.ArtId);
            gridPic[r, 13] = new SourceGrid.Cells.Cell(Pic.CraftId);

            ShowPicImage();
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
                ArtId = (int)gridPic[r, 12].Value,
                CraftId = (int)gridPic[r, 13].Value
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

            public override void OnEditStarted(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnEditStarted(sender, e);
                _form.SearchModeOff();
                if (sender.Position.Column == Const.Columns.Pic.Craft)
                {
                    _form._editCraft678.Control.DroppedDown = true;
                }
                if (sender.Position.Column == Const.Columns.Pic.Art)
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
                            _form._crafts678.SingleOrDefault(x => x.FullName == val)?.CraftId;
                        break;
                    case Const.Columns.Pic.Art:
                        _form.gridPic[row, Const.Columns.Pic.ArtId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._arts.SingleOrDefault(x => x.Name == val)?.Id;
                        break;
                    case Const.Columns.Pic.Type:
                        _form.gridPic[row, Const.Columns.Pic.NType].Value = _form.GetNType(val);
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
                    edPicText.Text = picText.Text;
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

                try
                {
                    var picPath = $"{_imagesPath}Images{craft.Source}\\{(string)gridPic[_picPosition.Row, Const.Columns.Pic.Path].Value}";
                    picPicImage.Load(picPath);
                    picPicImage.Visible = true;
                }
                catch
                {

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
                    picLinkImage.Load(picPath);
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
                            Pic.NN = (int.Parse((string)gridPic[pos.Row, Const.Columns.Pic.NN].Value) + 1).ToString();
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
                        UpdatePicRow(Pic, pos.Row);
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
                                    UpdatePicRow(Pic, 1);
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
                        var col = 0;
                        while (!gridPic.Columns[col].Visible) col++;
                        var focusPosn = new Position(1, col);
                        gridPic.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.Control)
                    {
                        var col = gridPic.Columns.Count - 1;
                        while (!gridPic.Columns[col].Visible) col--;
                        var focusPosn = new Position(gridPic.RowsCount - 1, col);
                        gridPic.Selection.Focus(focusPosn, true);
                    }
                    else if (_searchMode)
                    {
                        DoPicSearch(e.KeyCode, false);
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
                            var link = _links[pos.Row - 1];
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
                    else if (_searchMode)
                    {
                        DoPicSearch(e.KeyCode, false);
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
                    else
                    {
                        int.TryParse(art.IMonth.Trim(), out int artMonth);
                        var year = art.IYear == 0 ? "" : (art.IYear % 100).ToString();
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
            var nnn = (int)gridPic[pos.Row, Const.Columns.Pic.NType].Value;
            if (pos.Row > 1 && nnn == (int)gridPic[pos.Row - 1, Const.Columns.Pic.NType].Value)
            {
                var picFrom = _pics[pos.Row - 1];
                var nnnFrom = picFrom.NNN;
                var picTo = _pics[pos.Row - 2];
                var nnnTo = picTo.NNN;
                picFrom.NNN = nnnTo;
                picTo.NNN = nnnFrom;

                var entFrom = _ctx.Pics.FirstOrDefault(x => x.PicId == picFrom.PicId);
                var entTo = _ctx.Pics.FirstOrDefault(x => x.PicId == picTo.PicId);
                if (entFrom != null) Mapper.Map(picFrom, entFrom);
                if (entTo != null) Mapper.Map(picTo, entTo);
                _ctx.SaveChanges();

                _pics.Move(pos.Row - 1, pos.Row - 2);
                gridPic.Rows.Move(pos.Row, pos.Row - 1);
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
                    picFrom.NNN = nnnTo;
                    picTo.NNN = nnnFrom;

                    var entFrom = _ctx.Pics.FirstOrDefault(x => x.PicId == picFrom.PicId);
                    var entTo = _ctx.Pics.FirstOrDefault(x => x.PicId == picTo.PicId);
                    if (entFrom != null) Mapper.Map(picFrom, entFrom);
                    if (entTo != null) Mapper.Map(picTo, entTo);
                    _ctx.SaveChanges();

                    _pics.Move(pos.Row - 1, pos.Row);
                    gridPic.Rows.Move(pos.Row, pos.Row + 1);
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
            }
        }

        private void chPicSelArt_Click(object sender, EventArgs e)
        {
            LoadPics(false);
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StoreEditedPicText();
            LoadPics(false);
            edPicText.SelectionStart = 0;
            edPicText.SelectionLength = edPicText.TextLength;
            edPicText.SelectionFont = new Font(edPicText.SelectionFont, FontStyle.Regular);
            edPicText.SelectionColor = Color.Black;
            edPicText.SelectionBackColor = Color.White;
        }
    }
}
