﻿using Aik2.Dto;
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
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsCraft;
        private Position _craftPosition = Position.Empty;
        private GridCraftController _gridCraftController = null;
        private CraftDto _selectedCraft = null;
        private SourceGrid.Cells.Editors.ComboBox _editCraft6;
        private SourceGrid.Cells.Editors.ComboBox _editCraft7;
        private SourceGrid.Cells.Editors.ComboBox _editCraft678;
        private string _editingCraftFullName;
        private List<Pair<int>> _crafts;
        private List<Pair<int>> _crafts6;
        private List<Pair<int>> _crafts7;
        private List<Pair<int>> _crafts678;
        private List<CraftDto> _craftDtos;
        private string _oldCraftText;
        private bool _craftTextChanging;

        public void LoadCrafts()
        {
            var CraftsQry = _ctx.vwCrafts.AsQueryable();
            CraftsQry = CraftsQry.OrderBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name);
            /*if (chCraftSortAuthor.Checked)
                CraftsQry = CraftsQry.OrderBy(x => x.Author).ThenBy(x => x.Name).ThenBy(x => x.IYear).ThenBy(x => x.IMonth);
            else if (chCraftSortSerie.Checked)
                CraftsQry = CraftsQry.OrderBy(x => x.Serie).ThenBy(x => x.Name).ThenBy(x => x.IYear).ThenBy(x => x.IMonth);
            else if (chCraftSortYear.Checked)
                CraftsQry = CraftsQry.OrderBy(x => x.IYear).ThenBy(x => x.IMonth).ThenBy(x => x.Author).ThenBy(x => x.Name);*/
            var Crafts = CraftsQry.ToList();
            _craftDtos = Mapper.Map<List<CraftDto>>(Crafts);

            var saved = -1;
            if (_craftPosition != Position.Empty)
            {
                saved = (int)gridCraft[_craftPosition.Row, Const.Columns.Craft.CraftId].Value;
            }

            _gridCraftController = new GridCraftController(this);
            gridCraft.RowsCount = _craftDtos.Count + 1;

            _crafts = _craftDtos.OrderBy(x => x.FullName).Select(x => new Pair<int>() { Id = x.CraftId, Name = x.FullName }).ToList();

            var focused = false;
            for (var r = 1; r <= _craftDtos.Count; r++)
            {
                var Craft = _craftDtos[r - 1];
                UpdateCraftRow(Craft, r);

                if (Craft.CraftId == saved && _craftPosition != Position.Empty)
                {
                    var focusPosn = new Position(r, _craftPosition.Column);
                    gridCraft.Selection.Focus(focusPosn, true);
                    _craftPosition = focusPosn;
                    focused = true;
                }
            }
            if (!focused && _craftDtos.Any())
            {
                _craftPosition = new Position(1, 1);
                gridCraft.Selection.Focus(_craftPosition, true);
                CraftCellGotFocus(null, new ChangeActivePositionEventArgs(_craftPosition, _craftPosition));
            }

            _crafts6 = _craftDtos.Where(x => x.Source == "6").OrderBy(x => x.FullName).Select(x => new Pair<int>() { Id = x.CraftId, Name = x.FullName }).ToList();
            _editCraft6.Control.Items.Clear();
            _editCraft6.Control.Items.AddRange(_crafts6.Select(x => x.Name).ToArray());

            _crafts7 = _craftDtos.Where(x => x.Source == "7").OrderBy(x => x.FullName).Select(x => new Pair<int>() { Id = x.CraftId, Name = x.FullName }).ToList();
            _editCraft7.Control.Items.Clear();
            _editCraft7.Control.Items.AddRange(_crafts7.Select(x => x.Name).ToArray());

            _crafts678 = _craftDtos.Where(x => x.Source == "6" || x.Source == "7" || x.Source == "8")
                .OrderBy(x => x.FullName).Select(x => new Pair<int>() { Id = x.CraftId, Name = x.FullName }).ToList();
            _editCraft678.Control.Items.Clear();
            _editCraft678.Control.Items.AddRange(_crafts678.Select(x => x.Name).ToArray());

            gridCraft.Refresh();
        }

        public void UpdateCraftRow(CraftDto Craft, int r)
        {
            gridCraft[r, 0] = new SourceGrid.Cells.Cell(Craft.CraftId, _editorsCraft[0]);
            gridCraft[r, 1] = new SourceGrid.Cells.Cell(Craft.Construct, _editorsCraft[1]);
            gridCraft[r, 1].AddController(_gridCraftController);
            gridCraft[r, 2] = new SourceGrid.Cells.Cell(Craft.Name, _editorsCraft[2]);
            gridCraft[r, 2].AddController(_gridCraftController);
            gridCraft[r, 3] = new SourceGrid.Cells.Cell(Craft.Country, _editorsCraft[3]);
            gridCraft[r, 3].AddController(_gridCraftController);
            gridCraft[r, 4] = new SourceGrid.Cells.Cell(Craft.IYear, _editorsCraft[4]);
            gridCraft[r, 4].AddController(_gridCraftController);
            gridCraft[r, 5] = new SourceGrid.Cells.CheckBox("", Craft.Vert ?? false);
            gridCraft[r, 5].AddController(_gridCraftController);
            gridCraft[r, 6] = new SourceGrid.Cells.CheckBox("", Craft.Uav ?? false);
            gridCraft[r, 6].AddController(_gridCraftController);
            gridCraft[r, 7] = new SourceGrid.Cells.CheckBox("", Craft.Glider ?? false);
            gridCraft[r, 7].AddController(_gridCraftController);
            gridCraft[r, 8] = new SourceGrid.Cells.CheckBox("", Craft.LL ?? false);
            gridCraft[r, 8].AddController(_gridCraftController);
            gridCraft[r, 9] = new SourceGrid.Cells.CheckBox("", Craft.Single ?? false);
            gridCraft[r, 9].AddController(_gridCraftController);
            gridCraft[r, 10] = new SourceGrid.Cells.CheckBox("", Craft.Proj ?? false);
            gridCraft[r, 10].AddController(_gridCraftController);
            gridCraft[r, 11] = new SourceGrid.Cells.Cell(Craft.Wings, _editorsCraft[11]);
            gridCraft[r, 11].AddController(_gridCraftController);
            gridCraft[r, 12] = new SourceGrid.Cells.Cell(Craft.Engines, _editorsCraft[12]);
            gridCraft[r, 12].AddController(_gridCraftController);
            gridCraft[r, 13] = new SourceGrid.Cells.Cell(Craft.Source, _editorsCraft[13]);
            gridCraft[r, 13].AddController(_gridCraftController);
            gridCraft[r, 14] = new SourceGrid.Cells.Cell(Craft.Type, _editorsCraft[14]);
            gridCraft[r, 14].AddController(_gridCraftController);
            var craftName = "";
            if (Craft.SeeAlso.HasValue)
            {
                var craft = _crafts.FirstOrDefault(x => x.Id == Craft.SeeAlso);
                if (craft != null) craftName = craft.Name;
            }
            gridCraft[r, 15] = new SourceGrid.Cells.Cell(craftName, Craft.Source == "6" ? _editorsCraft[15] : null);
            gridCraft[r, 15].AddController(_gridCraftController);
            gridCraft[r, 16] = new SourceGrid.Cells.Cell(Craft.FullName, _editorsCraft[16]);
            gridCraft[r, 17] = new SourceGrid.Cells.Cell(Craft.Wiki, _editorsCraft[17]);
            gridCraft[r, 17].AddController(_gridCraftController);
            gridCraft[r, 18] = new SourceGrid.Cells.Cell(Craft.Airwar, _editorsCraft[18]);
            gridCraft[r, 18].AddController(_gridCraftController);
            craftName = "";
            if (Craft.FlyingM.HasValue)
            {
                var craft = _crafts.FirstOrDefault(x => x.Id == Craft.FlyingM);
                if (craft != null) craftName = craft.Name;
            }
            gridCraft[r, 19] = new SourceGrid.Cells.Cell(craftName, Craft.Source == "6" ? _editorsCraft[19] : null);
            gridCraft[r, 19].AddController(_gridCraftController);
            craftName = "";
            if (Craft.Same.HasValue)
            {
                var craft = _crafts.FirstOrDefault(x => x.Id == Craft.Same);
                if (craft != null) craftName = craft.Name;
            }
            gridCraft[r, 20] = new SourceGrid.Cells.Cell(craftName, Craft.Source == "6" ? _editorsCraft[20] : null);
            gridCraft[r, 20].AddController(_gridCraftController);
            gridCraft[r, 21] = new SourceGrid.Cells.Cell(Craft.SeeAlso);
            gridCraft[r, 22] = new SourceGrid.Cells.Cell(Craft.FlyingM);
            gridCraft[r, 23] = new SourceGrid.Cells.Cell(Craft.Same);

        }

        public void UpdateCraftRow2(CraftDto Craft, int r)
        {
            if (!string.IsNullOrEmpty(gridCraft[r, Const.Columns.Craft.FlyingMId].DisplayText))
            {
                var craft = _crafts.FirstOrDefault(x => x.Id == (int)gridCraft[r, Const.Columns.Craft.FlyingMId].Value);
                if (craft != null)
                {
                    gridCraft[r, 19] = new SourceGrid.Cells.Cell(Craft.FlyingM, _editorsCraft[19]);
                    gridCraft[r, 19].AddController(_gridCraftController);
                }
            }
            if (!string.IsNullOrEmpty(gridCraft[r, Const.Columns.Craft.SameId].DisplayText))
            {
                var craft = _crafts.FirstOrDefault(x => x.Id == (int)gridCraft[r, Const.Columns.Craft.SameId].Value);
                if (craft != null)
                {
                    gridCraft[r, 20] = new SourceGrid.Cells.Cell(Craft.Same, _editorsCraft[20]);
                    gridCraft[r, 20].AddController(_gridCraftController);
                }
            }

        }

        public string GetCraftFullName(int row)
        {
            var craft = new CraftDto()
            {
                Construct = (string)gridCraft[row, Const.Columns.Craft.Construct].Value,
                Name = (string)gridCraft[row, Const.Columns.Craft.Name].Value,
                Source = (string)gridCraft[row, Const.Columns.Craft.Source].Value,
                IYear = (int?)gridCraft[row, Const.Columns.Craft.IYear].Value,
                Country = (string)gridCraft[row, Const.Columns.Craft.Country].Value
            };
            return craft.FullName;
        }

        private class GridCraftController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly Form1 _form;

            public GridCraftController(Form1 form)
            {
                _form = form;
            }

            public override void OnEditStarted(SourceGrid.CellContext sender, EventArgs e)
            {
                _form._editingCraftFullName = _form.GetCraftFullName(sender.Position.Row);
                base.OnEditStarted(sender, e);
                _form.SearchModeOff();
                if (sender.Position.Column == Const.Columns.Craft.SeeAlso || sender.Position.Column == Const.Columns.Craft.Same)
                {
                    _form._editCraft6.Control.DroppedDown = true;
                }
                if (sender.Position.Column == Const.Columns.Craft.FlyingM)
                {
                    _form._editCraft7.Control.DroppedDown = true;
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
                    case Const.Columns.Craft.SeeAlso:
                    case Const.Columns.Craft.Same:
                        _form.gridCraft[row, Const.Columns.Craft.SeeAlsoId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._crafts6.SingleOrDefault(x => x.Name == val)?.Id;
                        break;
                    case Const.Columns.Craft.FlyingM:
                        _form.gridCraft[row, Const.Columns.Craft.SeeAlsoId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._crafts7.SingleOrDefault(x => x.Name == val)?.Id;
                        break;
                    default:
                        var newCraft = new Pair<int>()
                        {
                            Id = (int)_form.gridCraft[row, Const.Columns.Craft.CraftId].Value,
                            Name = _form.GetCraftFullName(row)
                        };

                        if (newCraft.Name != _form._editingCraftFullName)
                        {
                            var isRefresh = false;
                            _form.gridCraft[row, Const.Columns.Craft.FullName].Value = newCraft.Name;
                            var craft = _form._crafts.FirstOrDefault(x => x.Id == newCraft.Id);
                            if (craft != null) _form._crafts.Remove(craft);
                            craft = _form._crafts6.FirstOrDefault(x => x.Id == newCraft.Id);
                            if (craft != null) { _form._crafts6.Remove(craft); isRefresh = true; }
                            craft = _form._crafts7.FirstOrDefault(x => x.Id == newCraft.Id);
                            if (craft != null) { _form._crafts7.Remove(craft); isRefresh = true; }
                            var source = (string)_form.gridCraft[row, Const.Columns.Craft.Source].Value;
                            var list = source == "6" ? _form._crafts6 : source == "7" ? _form._crafts7 : null;
                            if (list != null)
                            {
                                list.InsertCraftSorted(newCraft);
                                isRefresh = true;
                            }
                            if (isRefresh)
                            {
                                _form._editCraft6.Control.Items.Clear();
                                _form._editCraft6.Control.Items.AddRange(_form._crafts6.Select(x => x.Name).ToArray());

                                _form._editCraft7.Control.Items.Clear();
                                _form._editCraft7.Control.Items.AddRange(_form._crafts7.Select(x => x.Name).ToArray());
                            }
                        }
                        break;
                }

                int[] sortIndexes;
                sortIndexes = new int[] { Const.Columns.Craft.Construct, Const.Columns.Craft.IYear, Const.Columns.Craft.Name };
                /*if (_form.chCraftSortAuthor.Checked)
                    sortIndexes = Const.Columns.Craft.SortAuthor;
                else if (_form.chCraftSortSerie.Checked)
                    sortIndexes = Const.Columns.Craft.SortSerie;
                else //if (_form.chCraftSortYear.Checked)
                    sortIndexes = Const.Columns.Craft.SortYear;*/

                if (sortIndexes.Contains(cell.Column.Index))
                {
                    var sortVal = "";
                    var rNew = row;
                    foreach (var col in sortIndexes) sortVal += _form.gridCraft[row, col].DisplayText + " ";
                    if (row < _form.gridCraft.RowsCount)
                    {
                        var found = false;
                        for (var r = row + 1; r < _form.gridCraft.RowsCount; r++)
                        {
                            var sortRVal = "";
                            foreach (var col in sortIndexes) sortRVal += _form.gridCraft[r, col].DisplayText + " ";
                            if (string.Compare(sortVal, sortRVal) <= 0)
                            {
                                rNew = r - 1;
                                found = true;
                                break;
                            }
                        }
                        if (!found) rNew = _form.gridCraft.RowsCount - 1;
                    }
                    if (rNew == row && row > 1)
                    {
                        var found = false;
                        for (var r = row - 1; r >= 1; r--)
                        {
                            var sortRVal = "";
                            foreach (var col in sortIndexes) sortRVal += _form.gridCraft[r, col].DisplayText + " ";
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
                        _form.gridCraft.Rows.Move(row, rNew);
                        var focusPosn = new Position(rNew, sender.Position.Column);
                        _form.gridCraft.Selection.Focus(focusPosn, true);
                    }

                }

            }
        }

        public void InitCraftGrid()
        {
            _editorsCraft = new List<SourceGrid.Cells.Editors.EditorBase>();

            var editStr1 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr1.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 1); };

            var editStr20 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr20.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 20); };

            var editStr50 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr50.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 50); };

            var editStr255 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr255.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 255); };

            var editIntNull = SourceGrid.Cells.Editors.Factory.Create(typeof(int));
            editIntNull.AllowNull = true;

            var editBool = SourceGrid.Cells.Editors.Factory.Create(typeof(bool));

            _editCraft6 = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraft6.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraft6.Control.AutoCompleteMode = AutoCompleteMode.Append;
            _editCraft6.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _crafts6.All(x => x.Name != text)) cancelEvent.Cancel = true;
            };

            _editCraft7 = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraft7.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraft7.Control.AutoCompleteMode = AutoCompleteMode.Append;
            _editCraft7.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _crafts7.All(x => x.Name != text)) cancelEvent.Cancel = true;
            };

            _editCraft678 = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraft678.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraft678.Control.AutoCompleteMode = AutoCompleteMode.Append;
            _editCraft678.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _crafts678.All(x => x.Name != text)) cancelEvent.Cancel = true;
            };

            gridCraft.ColumnsCount = 24;

            gridCraft.RowsCount = 1;
            gridCraft.FixedRows = 1;

            gridCraft[0, 0] = new SourceGrid.Cells.ColumnHeader("CraftId");
            gridCraft.Columns[0].Visible = false;
            _editorsCraft.Add(null);
            gridCraft[0, 1] = new SourceGrid.Cells.ColumnHeader("Construct");
            _editorsCraft.Add(editStr50);
            gridCraft[0, 2] = new SourceGrid.Cells.ColumnHeader("Name");
            _editorsCraft.Add(editStr50);
            gridCraft[0, 3] = new SourceGrid.Cells.ColumnHeader("Country");
            _editorsCraft.Add(editStr20);
            gridCraft[0, 4] = new SourceGrid.Cells.ColumnHeader("IYear");
            _editorsCraft.Add(editIntNull);
            gridCraft[0, 5] = new SourceGrid.Cells.ColumnHeader("Vert");
            _editorsCraft.Add(editBool);
            gridCraft[0, 6] = new SourceGrid.Cells.ColumnHeader("Uav");
            _editorsCraft.Add(editBool);
            gridCraft[0, 7] = new SourceGrid.Cells.ColumnHeader("Glider");
            _editorsCraft.Add(editBool);
            gridCraft[0, 8] = new SourceGrid.Cells.ColumnHeader("LL");
            _editorsCraft.Add(editBool);
            gridCraft[0, 9] = new SourceGrid.Cells.ColumnHeader("Single");
            _editorsCraft.Add(editBool);
            gridCraft[0, 10] = new SourceGrid.Cells.ColumnHeader("Proj");
            _editorsCraft.Add(editBool);
            gridCraft[0, 11] = new SourceGrid.Cells.ColumnHeader("Wings");
            _editorsCraft.Add(editStr20);
            gridCraft[0, 12] = new SourceGrid.Cells.ColumnHeader("Engines");
            _editorsCraft.Add(editStr20);
            gridCraft[0, 13] = new SourceGrid.Cells.ColumnHeader("Source");
            _editorsCraft.Add(editStr1);
            gridCraft[0, 14] = new SourceGrid.Cells.ColumnHeader("Type");
            _editorsCraft.Add(editStr255);
            gridCraft[0, 15] = new SourceGrid.Cells.ColumnHeader("SeeAlso");
            _editorsCraft.Add(_editCraft6);
            gridCraft[0, 16] = new SourceGrid.Cells.ColumnHeader("FullName");
            _editorsCraft.Add(null);
            gridCraft[0, 17] = new SourceGrid.Cells.ColumnHeader("Wiki");
            _editorsCraft.Add(editStr255);
            gridCraft[0, 18] = new SourceGrid.Cells.ColumnHeader("Airwar");
            _editorsCraft.Add(editStr255);
            gridCraft[0, 19] = new SourceGrid.Cells.ColumnHeader("FlyingM");
            _editorsCraft.Add(_editCraft7);
            gridCraft[0, 20] = new SourceGrid.Cells.ColumnHeader("Same");
            _editorsCraft.Add(_editCraft6);
            gridCraft[0, 21] = new SourceGrid.Cells.ColumnHeader("SeeAlsoId");
            gridCraft.Columns[21].Visible = false;
            gridCraft[0, 22] = new SourceGrid.Cells.ColumnHeader("FlyingMId");
            gridCraft.Columns[22].Visible = false;
            gridCraft[0, 23] = new SourceGrid.Cells.ColumnHeader("SameId");
            gridCraft.Columns[23].Visible = false;

            for (var i = 1; i < gridCraft.ColumnsCount; i++)
            {
                var key = $"gridCraft:ColumnWidth:{i}";
                if (_config.ContainsKey(key))
                {
                    gridCraft.Columns[i].Width = int.Parse(_config[key]);
                }
            }

            gridCraft.Selection.CellGotFocus += CraftCellGotFocus;
            gridCraft.Columns.ColumnWidthChanged += CraftColumnWidthChanged;

        }

        private void SelectCraft(int? craftId)
        {
            if (!craftId.HasValue) return;
            var craft = _craftDtos.Where(x => x.CraftId == craftId).SingleOrDefault();
            if (craft != null)
            {
                var ind = _craftDtos.IndexOf(craft);
                var pos = gridCraft.Selection.ActivePosition;
                var newPos = new Position(ind + 1, pos == Position.Empty ? 1 : pos.Column);
                gridCraft.Selection.Focus(newPos, true);
            }
        }

        private void CraftCellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            _craftPosition = e.NewFocusPosition;
            var craft = _craftDtos[_craftPosition.Row - 1];
            lCraft.Text = craft.FullName;
            if (craft.CraftId > 0 && 
                (_selectedCraft == null || _selectedCraft.CraftId != craft.CraftId))
            {
                _selectedCraft = Mapper.Map<CraftDto>(_ctx.Crafts.Single(x => x.CraftId == craft.CraftId));
                _craftTextChanging = true;
                edCraftText.Text = _selectedCraft.CText;
                _oldCraftText = edCraftText.Text;
                ColorizeText(edCraftText, true);
                _craftTextChanging = false;
            }
            if (_searchMode && !_searchChanging)
            {
                _searchString = "";
                lInfo.Text = $"Поиск: {_searchString}";
            }
        }

        private void CraftColumnWidthChanged(object sender, ColumnInfoEventArgs e)
        {
            if (sender == gridCraft.Columns)
            {
                _config[$"gridCraft:ColumnWidth:{e.Column.Index}"] = e.Column.Width.ToString();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }
        }

        private void DoCraftSearch(Keys c, bool isText)
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

            var pos = gridCraft.Selection.ActivePosition;
            if (pos != Position.Empty)
            {
                var found = false;
                for (var r = pos.Row; r < gridCraft.RowsCount; r++)
                {
                    if (gridCraft[r, pos.Column].DisplayText.ToUpper().StartsWith(_searchString.ToUpper()))
                    {
                        _searchChanging = true;
                        var focusPosn = new Position(r, pos.Column);
                        gridCraft.Selection.Focus(focusPosn, true);
                        found = true;
                        _searchChanging = false;
                        break;
                    }
                }
                if (!found)
                {
                    for (var r = 1; r < pos.Row; r++)
                    {
                        if (gridCraft[r, pos.Column].DisplayText.ToUpper().StartsWith(_searchString.ToUpper()))
                        {
                            _searchChanging = true;
                            var focusPosn = new Position(r, pos.Column);
                            gridCraft.Selection.Focus(focusPosn, true);
                            found = true;
                            _searchChanging = false;
                            break;
                        }
                    }
                }
            }
        }

        private void gridCraft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (gridCraft.Focused)
            {
                var pos = gridCraft.Selection.ActivePosition;
                if (pos != Position.Empty)
                {
                    if (e.KeyCode == Keys.Insert)
                    {
                        SearchModeOff();
                        var Craft = new CraftDto()
                        {
                            Construct = (string)gridCraft[pos.Row, Const.Columns.Craft.Construct].Value,
                            Country = (string)gridCraft[pos.Row, Const.Columns.Craft.Country].Value,
                            Source = (string)gridCraft[pos.Row, Const.Columns.Craft.Source].Value
                        };
                        gridCraft.Rows.Insert(pos.Row);
                        UpdateCraftRow(Craft, pos.Row);
                    }
                    else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                    {
                        if (MessageBox.Show("Delete pic?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var craftId = (int)gridCraft[pos.Row, Const.Columns.Craft.CraftId].Value;
                            var craft = _crafts.FirstOrDefault(x => x.Id == craftId);
                            if (craft != null) _crafts.Remove(craft);
                            craft = _crafts6.FirstOrDefault(x => x.Id == craftId);
                            if (craft != null)
                            {
                                _crafts6.Remove(craft);
                                _editCraft6.Control.Items.Clear();
                                _editCraft6.Control.Items.AddRange(_crafts6.Select(x => x.Name).ToArray());
                            }
                            craft = _crafts7.FirstOrDefault(x => x.Id == craftId);
                            if (craft != null)
                            {
                                _crafts7.Remove(craft);
                                _editCraft7.Control.Items.Clear();
                                _editCraft7.Control.Items.AddRange(_crafts7.Select(x => x.Name).ToArray());
                            }

                            gridCraft.Rows.Remove(pos.Row);
                        }
                    }
                    else if (_searchMode)
                    {
                        DoCraftSearch(e.KeyCode, false);
                    }
                }
            }
        }


        private void chCraftSortYear_Click(object sender, EventArgs e)
        {
            LoadCrafts();
        }

        private void pCraftText_Resize(object sender, EventArgs e)
        {
            _config["pCraftTextWidth"] = ((Panel)sender).Width.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void edCraftText_TextChanged(object sender, EventArgs e)
        {
            if (_craftTextChanging) return;

            var selStart = edCraftText.SelectionStart;
            var selLength = edCraftText.SelectionLength;
            var selChanged = false;
            var changed = "";
            var newText = edCraftText.Text;
            if (string.IsNullOrEmpty(_oldCraftText))
            {
                changed = newText;
            }
            else if (!string.IsNullOrEmpty(newText))
            {
                var i = 0;
                while (i < _oldCraftText.Length && i < newText.Length && _oldCraftText[i] == newText[i]) i++;
                if (i > 0) i--;
                while (i > 0 && (Char.IsLetterOrDigit(newText[i]) || newText[i] == '-')) i--;

                var oldTextRev = Util.Reverse(_oldCraftText);
                var newTextRev = Util.Reverse(newText);
                var j = 0;
                while (j < oldTextRev.Length && j < newTextRev.Length && oldTextRev[j] == newTextRev[j]) j++;
                if (j > 0) j--;
                while (j > 0 && (Char.IsLetterOrDigit(newTextRev[j]) || newTextRev[j] == '-')) j--;
                j = newText.Length - j - 1;

                if (i < j) changed = newText.Substring(i, j - i);

                edCraftText.Select(i, j - i);
                edCraftText.SelectionColor = Color.Black;
                edCraftText.SelectionBackColor = Color.White;

                selChanged = true;
            }

            var uText = edCraftText.Text.ToUpper();
            var chWords = GetWords(changed, false);
            foreach(var chWord in chWords)
            {
                var words = _ctx.Database.SqlQuery<Words>(
                    $"select distinct w.* from Words w where w.Word = '{chWord.Name}'").ToList();
                if (words.Any())
                {
                    SetWordColor(edCraftText, words.First(), uText);
                } else
                {
                    SetWordColor(edCraftText, new Words() { Word = chWord.Name, Cnt = 0 }, uText);
                }
                selChanged = true;
            }
            if (selChanged)
            {
                edCraftText.SelectionStart = selStart;
                edCraftText.SelectionLength = selLength;
            }

            _oldCraftText = newText;
        }

    }


}