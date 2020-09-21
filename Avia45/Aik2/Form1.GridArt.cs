using Aik2.Dto;
using AutoMapper;
using Newtonsoft.Json;
using SourceGrid;
using SourceGrid.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsArt;
        private Position _artPosition = Position.Empty;
        private GridArtController _gridArtController = null;
        private List<Pair<int>> _arts;
        private List<ArtDto> _artDtos;
        private List<int> _filteredMags;
        private List<int> _filteredMagCrafts;
        private int _filteredArtId = -1;
        private SourceGrid.Cells.Editors.ComboBox _editArt;
        private ArtDto _selectedArt = null;

        public void InitArtGrid()
        {
            _editorsArt = new List<SourceGrid.Cells.Editors.EditorBase>();

            var editStr5 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr5.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 5); };

            var editStr50 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr50.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 50); };

            var editStr100 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr100.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 100); };

            var editInt = SourceGrid.Cells.Editors.Factory.Create(typeof(int));

            var editIntNull = SourceGrid.Cells.Editors.Factory.Create(typeof(int));
            editIntNull.AllowNull = true;

            _editArt = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editArt.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editArt.Control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _editArt.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _arts.All(x => x.Name != text)) cancelEvent.Cancel = true;
            };

            _editorsArt.Add(null);
            _editorsArt.Add(editStr5);
            _editorsArt.Add(editInt);
            _editorsArt.Add(editStr5);
            _editorsArt.Add(editStr50);
            _editorsArt.Add(editStr100);
            _editorsArt.Add(editIntNull);
            _editorsArt.Add(editStr50);
            _editorsArt.Add(null);
            _editorsArt.Add(null);

            gridArt.ColumnsCount = 10;
            gridArt.RowsCount = 1;
            gridArt.FixedRows = 1;

            gridArt[0, 0] = new SourceGrid.Cells.ColumnHeader("ArtId");
            gridArt.Columns[0].Visible = false;
            gridArt[0, 1] = new SourceGrid.Cells.ColumnHeader("Mag");
            gridArt[0, 2] = new SourceGrid.Cells.ColumnHeader("IYear");
            gridArt[0, 3] = new SourceGrid.Cells.ColumnHeader("IMonth");
            gridArt[0, 4] = new SourceGrid.Cells.ColumnHeader("Author");
            gridArt[0, 5] = new SourceGrid.Cells.ColumnHeader("Name");
            gridArt[0, 6] = new SourceGrid.Cells.ColumnHeader("NN");
            gridArt[0, 7] = new SourceGrid.Cells.ColumnHeader("Serie");
            gridArt[0, 8] = new SourceGrid.Cells.ColumnHeader("Pics");
            gridArt[0, 9] = new SourceGrid.Cells.ColumnHeader("Art");

            for (var i = 1; i < gridArt.ColumnsCount; i++)
            {
                var key = $"gridArt:ColumnWidth:{i}";
                if (_config.ContainsKey(key))
                {
                    gridArt.Columns[i].Width = int.Parse(_config[key]);
                }
            }

            _gridArtController = new GridArtController(this);

            gridArt.Selection.CellGotFocus += ArtCellGotFocus;
            gridArt.Columns.ColumnWidthChanged += ArtColumnWidthChanged;

        }

        public void LoadArts()
        {
            var artsQry = _ctx.vwArts.AsNoTracking().AsQueryable();
            if (chArtSortAuthor.Checked)
                artsQry = artsQry.OrderBy(x => x.Author).ThenBy(x => x.Name).ThenBy(x => x.IYear).ThenBy(x => x.IMonth).ThenBy(x => x.Mag);
            else if (chArtSortSerie.Checked)
                artsQry = artsQry.OrderBy(x => x.Serie).ThenBy(x => x.Name).ThenBy(x => x.IYear).ThenBy(x => x.IMonth);
            else if (chArtSortYear.Checked)
                artsQry = artsQry.OrderBy(x => x.IYear).ThenBy(x => x.IMonth).ThenBy(x => x.Mag).ThenBy(x => x.Author).ThenBy(x => x.Name);
            var arts = artsQry.ToList();
            _artDtos = Mapper.Map<List<ArtDto>>(arts);


            var saved = -1;
            if (_artPosition != Position.Empty)
            {
                saved = (int)gridArt[_artPosition.Row, Const.Columns.Art.ArtId].Value;
            }

            gridArt.RowsCount = _artDtos.Count + 1;
            var focused = false;
            for (var r = 1; r <= _artDtos.Count; r++)
            {
                var art = _artDtos[r - 1];
                UpdateArtRow(art, r);

                if (art.ArtId == saved && _artPosition != Position.Empty)
                {
                    var focusPosn = new Position(r, _artPosition.Column);
                    gridArt.Selection.Focus(focusPosn, true);
                    _artPosition = focusPosn;
                    focused = true;
                }
            }
            if (!focused && _artDtos.Any())
            {
                _artPosition = new Position(1, 1);
                gridArt.Selection.Focus(_artPosition, true);
                ArtCellGotFocus(null, new ChangeActivePositionEventArgs(_artPosition, _artPosition));
            }

            _arts = _artDtos.OrderBy(x => x.FullName).Select(x => new Pair<int>() { Id = x.ArtId, Name = x.FullName }).ToList();

            _editArt.Control.Items.Clear();
            _editArt.Control.Items.AddRange(_arts.Select(x => x.Name).ToArray());

            gridArt.Refresh();
        }

        public void UpdateArtRow(ArtDto art, int r)
        {
            gridArt[r, 0] = new SourceGrid.Cells.Cell(art.ArtId, _editorsArt[0]);
            gridArt[r, 1] = new SourceGrid.Cells.Cell(art.Mag, _editorsArt[1]);
            gridArt[r, 1].AddController(_gridArtController);
            gridArt[r, 2] = new SourceGrid.Cells.Cell(art.IYear, _editorsArt[2]);
            gridArt[r, 2].AddController(_gridArtController);
            gridArt[r, 3] = new SourceGrid.Cells.Cell(art.IMonth, _editorsArt[3]);
            gridArt[r, 3].AddController(_gridArtController);
            gridArt[r, 4] = new SourceGrid.Cells.Cell(art.Author, _editorsArt[4]);
            gridArt[r, 4].AddController(_gridArtController);
            gridArt[r, 5] = new SourceGrid.Cells.Cell(art.Name, _editorsArt[5]);
            gridArt[r, 5].AddController(_gridArtController);
            gridArt[r, 6] = new SourceGrid.Cells.Cell(art.NN, _editorsArt[6]);
            gridArt[r, 6].AddController(_gridArtController);
            gridArt[r, 7] = new SourceGrid.Cells.Cell(art.Serie, _editorsArt[7]);
            gridArt[r, 7].AddController(_gridArtController);
            gridArt[r, 8] = new SourceGrid.Cells.Cell(art.cnt, _editorsArt[8]);
            gridArt[r, 9] = new SourceGrid.Cells.Cell(art.FullName, _editorsArt[9]);
        }

        public ArtDto GetArtFromTable(int row)
        {
            var art = new ArtDto()
            {
                ArtId = (int)gridArt[row, Const.Columns.Art.ArtId].Value,
                Mag = (string)gridArt[row, Const.Columns.Art.Mag].Value,
                IYear = (int?)gridArt[row, Const.Columns.Art.IYear].Value,
                IMonth = (string)gridArt[row, Const.Columns.Art.IMonth].Value,
                Author = (string)gridArt[row, Const.Columns.Art.Author].Value,
                Name = (string)gridArt[row, Const.Columns.Art.Name].Value,
                NN = (int?)gridArt[row, Const.Columns.Art.NN].Value,
                Serie = (string)gridArt[row, Const.Columns.Art.Serie].Value
            };
            return art;
        }

        private class GridArtController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly Form1 _form;

            public GridArtController(Form1 form)
            {
                _form = form;
            }

            public override void OnEditStarted(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnEditStarted(sender, e);
                _form.SearchModeOff();
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                int row = sender.Position.Row;
                SourceGrid.Cells.Cell cell = (SourceGrid.Cells.Cell)sender.Cell;
                var art = _form.GetArtFromTable(row);
                var isForce = _form.UpdateArtEntity(art, row);
                _form.gridArt[row, Const.Columns.Art.FullName].Value = art.FullName;


                _form.CheckArtSort(sender.Position, isForce);
            }
        }

        private void CheckArtSort(Position pos, bool isForce)
        {
            int[] sortIndexes;
            if (chArtSortAuthor.Checked)
                sortIndexes = Const.Columns.Art.SortAuthor;
            else if (chArtSortSerie.Checked)
                sortIndexes = Const.Columns.Art.SortSerie;
            else //if (_form.chArtSortYear.Checked)
                sortIndexes = Const.Columns.Art.SortYear;

            if (isForce || sortIndexes.Contains(pos.Column))
            {
                var sortVal = "";
                var rNew = pos.Row;
                foreach (var col in sortIndexes) sortVal += gridArt[pos.Row, col].DisplayText + " ";
                if (pos.Row < gridArt.RowsCount)
                {
                    var found = false;
                    for (var r = pos.Row + 1; r < gridArt.RowsCount; r++)
                    {
                        var sortRVal = "";
                        foreach (var col in sortIndexes) sortRVal += gridArt[r, col].DisplayText + " ";
                        if (string.Compare(sortVal, sortRVal) <= 0)
                        {
                            rNew = r - 1;
                            found = true;
                            break;
                        }
                    }
                    if (!found) rNew = gridArt.RowsCount - 1;
                }
                if (rNew == pos.Row && pos.Row > 1)
                {
                    var found = false;
                    for (var r = pos.Row - 1; r >= 1; r--)
                    {
                        var sortRVal = "";
                        foreach (var col in sortIndexes) sortRVal += gridArt[r, col].DisplayText + " ";
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
                    gridArt.Rows.Move(pos.Row, rNew);
                    _artDtos.Move(pos.Row - 1, rNew - 1);
                    var focusPosn = new Position(rNew, pos.Column);
                    gridArt.Selection.Focus(focusPosn, true);
                }

            }
        }

        private bool UpdateArtEntity(ArtDto art, int row)
        {
            var isAdded = false;
            if (art.ArtId == 0)
            {
                var entity = Mapper.Map<Arts>(art);
                _ctx.Arts.Add(entity);
                _ctx.SaveChanges();
                art.ArtId = entity.ArtId;
                gridArt[row, Const.Columns.Art.ArtId].Value = entity.ArtId;
                isAdded = true;
            }
            else
            {
                var entity = _ctx.Arts.Single(x => x.ArtId == art.ArtId);
                Mapper.Map(art, entity);
                _ctx.SaveChanges();
            }

            _selectedArt = art;
            _artDtos[row - 1] = art;

            var pArt = _arts.FirstOrDefault(x => x.Id == art.ArtId);
            if (pArt != null) _arts.Remove(pArt);
            _arts.InsertPairSorted(new Pair<int>() { Id = art.ArtId, Name = art.FullName });

            _editArt.Control.Items.Clear();
            _editArt.Control.Items.AddRange(_arts.Select(x => x.Name).ToArray());

            return isAdded;
        }

        private void SelectArt(int? artId)
        {
            if (!artId.HasValue) return;
            var art = _artDtos.Where(x => x.ArtId == artId).SingleOrDefault();
            if (art != null)
            {
                var ind = _artDtos.IndexOf(art);
                var pos = gridArt.Selection.ActivePosition;
                var newPos = new Position(ind + 1, pos == Position.Empty ? 1 : pos.Column);
                gridArt.Selection.Focus(newPos, true);
            }
            _needLoadArt = false;
        }

        private void ArtCellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            _needLoadArt = false;
            _artPosition = e.NewFocusPosition;
            _selectedArt = _artDtos[_artPosition.Row - 1];
            lArt.Text = _selectedArt.FullName;
            if (_searchMode && !_searchChanging)
            {
                _searchString = "";
                lInfo.Text = $"Поиск: {_searchString}";
            }
        }

        private void ArtColumnWidthChanged(object sender, ColumnInfoEventArgs e)
        {
            if (sender == gridArt.Columns)
            {
                _config[$"gridArt:ColumnWidth:{e.Column.Index}"] = e.Column.Width.ToString();
                File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
            }
        }

        private void DoArtSearch(Keys c, bool isText)
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

            var pos = gridArt.Selection.ActivePosition;
            if (pos != Position.Empty)
            {
                var found = false;
                for (var r = pos.Row; r < gridArt.RowsCount; r++)
                {
                    if (gridArt[r, pos.Column].DisplayText.ToUpper().StartsWith(_searchString.ToUpper()))
                    {
                        _searchChanging = true;
                        var focusPosn = new Position(r, pos.Column);
                        gridArt.Selection.Focus(focusPosn, true);
                        found = true;
                        _searchChanging = false;
                        break;
                    }
                }
                if (!found)
                {
                    for (var r = 1; r < pos.Row; r++)
                    {
                        if (gridArt[r, pos.Column].DisplayText.ToUpper().StartsWith(_searchString.ToUpper()))
                        {
                            _searchChanging = true;
                            var focusPosn = new Position(r, pos.Column);
                            gridArt.Selection.Focus(focusPosn, true);
                            found = true;
                            _searchChanging = false;
                            break;
                        }
                    }
                }
            }
        }

        private void gridArt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (gridArt.Focused)
            {
                var pos = gridArt.Selection.ActivePosition;
                if (pos != Position.Empty)
                {
                    if (e.KeyCode == Keys.Insert)
                    {
                        SearchModeOff();
                        var art = new ArtDto()
                        {
                            Mag = (string)gridArt[pos.Row, Const.Columns.Art.Mag].Value,
                            IYear = (int?)gridArt[pos.Row, Const.Columns.Art.IYear].Value,
                            IMonth = (string)gridArt[pos.Row, Const.Columns.Art.IMonth].Value
                        };
                        _artDtos.Insert(pos.Row - 1, Mapper.Map<ArtDto>(art));
                        gridArt.Rows.Insert(pos.Row);
                        UpdateArtRow(art, pos.Row);
                        CheckArtSort(pos, true);
                    }
                    else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                    {
                        if (MessageBox.Show("Delete art?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                var art = _artDtos[pos.Row - 1];
                                if (art.ArtId > 0)
                                {
                                    var entity = _ctx.Arts.Single(x => x.ArtId == art.ArtId);
                                    _ctx.Arts.Remove(entity);
                                    _ctx.SaveChanges();
                                }

                                gridArt.Rows.Remove(pos.Row);
                                _artDtos.RemoveAt(pos.Row - 1);
                                var pArt = _arts.FirstOrDefault(x => x.Id == art.ArtId);
                                if (pArt != null) _arts.Remove(pArt);

                                _editArt.Control.Items.Clear();
                                _editArt.Control.Items.AddRange(_arts.Select(x => x.Name).ToArray());
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
                        while (!gridArt.Columns[col].Visible) col++;
                        var focusPosn = new Position(pos.Row, col);
                        gridArt.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.None)
                    {
                        var col = gridArt.Columns.Count - 1;
                        while (!gridArt.Columns[col].Visible) col--;
                        var focusPosn = new Position(pos.Row, col);
                        gridArt.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.Home && e.Modifiers == Keys.Control)
                    {
                        var col = 0;
                        while (!gridArt.Columns[col].Visible) col++;
                        var focusPosn = new Position(1, col);
                        gridArt.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.Control)
                    {
                        var col = gridArt.Columns.Count - 1;
                        while (!gridArt.Columns[col].Visible) col--;
                        var focusPosn = new Position(gridArt.RowsCount - 1, col);
                        gridArt.Selection.Focus(focusPosn, true);
                    }
                }
            }
        }

        private void chArtSortYear_Click(object sender, EventArgs e)
        {
            LoadArts();
        }

    }
}
