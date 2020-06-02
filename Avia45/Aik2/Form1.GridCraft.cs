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
        private List<SourceGrid.Cells.Editors.EditorBase> _editorsCraft;
        private Position _craftPosition = Position.Empty;
        private GridCraftController _gridCraftController = null;
        private CraftDto _selectedCraft = null;
        private SourceGrid.Cells.Editors.ComboBox _editCraft6;
        private SourceGrid.Cells.Editors.ComboBox _editCraft7;
        private SourceGrid.Cells.Editors.ComboBox _editCraft678;
        private SourceGrid.Cells.Editors.ComboBox _editCraftWings;
        private SourceGrid.Cells.Editors.ComboBox _editCraftEngs;
        private SourceGrid.Cells.Views.Cell _normCraftCellView;
        private SourceGrid.Cells.Views.CheckBox _normCraftCheckView;
        private SourceGrid.Cells.Views.Cell _redCraftCellView;
        private SourceGrid.Cells.Views.CheckBox _redCraftCheckView;
        private SourceGrid.Cells.Views.Cell _grayCraftCellView;
        private SourceGrid.Cells.Views.CheckBox _grayCraftCheckView;
        private SourceGrid.Cells.Views.Cell _navyCraftCellView;
        private SourceGrid.Cells.Views.CheckBox _navyCraftCheckView;
        private string _editingCraftFullName;
        private List<CraftDto> _crafts;
        private List<CraftDto> _crafts6;
        private List<CraftDto> _crafts7;
        private List<CraftDto> _crafts678;
        private List<CraftDto> _craftDtos;
        private List<PicDto> _craftPicDtos;
        private int _craftPicDtoIndex;
        private string _oldCraftText;
        private bool _craftTextChanging;
        private bool _craftTextChanged;

        private FilterDto _filter = null;
        private bool _filterOn = false;

        public void InitCraftGrid()
        {
            _editorsCraft = new List<SourceGrid.Cells.Editors.EditorBase>();

            var editStr1 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr1.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 1); };

            var editStr20 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr20.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 20); };

            var editStr50 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr50.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 50); };

            var editStr100 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr100.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 100); };

            var editStr255 = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            editStr255.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) { StringMaxLen((TextBox)sender, cancelEvent, 255); };

            var editIntNull = SourceGrid.Cells.Editors.Factory.Create(typeof(int));
            editIntNull.AllowNull = true;

            var editBool = SourceGrid.Cells.Editors.Factory.Create(typeof(bool));

            _editCraft6 = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraft6.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraft6.Control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _editCraft6.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _crafts6.All(x => x.FullName != text)) cancelEvent.Cancel = true;
            };

            _editCraft7 = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraft7.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraft7.Control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _editCraft7.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _crafts7.All(x => x.FullName != text)) cancelEvent.Cancel = true;
            };

            _editCraft678 = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraft678.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraft678.Control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _editCraft678.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && _crafts678.All(x => x.FullName != text)) cancelEvent.Cancel = true;
            };

            _editCraftWings = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraftWings.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraftWings.Control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _editCraftWings.Control.Items.AddRange(_ctx.Crafts.Select(x => x.Wings).Distinct().OrderBy(x => x).Where(x => !string.IsNullOrEmpty(x)).ToArray());
            _editCraftWings.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && text.Length > 20) cancelEvent.Cancel = true;
            };

            _editCraftEngs = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            _editCraftEngs.Control.AutoCompleteSource = AutoCompleteSource.ListItems;
            _editCraftEngs.Control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _editCraftEngs.Control.Items.AddRange(_ctx.Crafts.Select(x => x.Engines).Distinct().OrderBy(x => x).Where(x => !string.IsNullOrEmpty(x)).ToArray());
            _editCraftEngs.Control.Validating += delegate (object sender, CancelEventArgs cancelEvent) {
                var text = ((ComboBox)sender).Text;
                if (!string.IsNullOrEmpty(text) && text.Length > 20) cancelEvent.Cancel = true;
            };

            gridCraft.ColumnsCount = 25;

            gridCraft.RowsCount = 1;
            gridCraft.FixedRows = 1;

            gridCraft[0, 0] = new SourceGrid.Cells.ColumnHeader("CraftId");
            gridCraft.Columns[0].Visible = false;
            _editorsCraft.Add(null);
            gridCraft[0, 1] = new SourceGrid.Cells.ColumnHeader("Construct");
            _editorsCraft.Add(editStr50);
            gridCraft[0, 2] = new SourceGrid.Cells.ColumnHeader("Name");
            _editorsCraft.Add(editStr100);
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
            _editorsCraft.Add(_editCraftWings);
            gridCraft[0, 12] = new SourceGrid.Cells.ColumnHeader("Engines");
            _editorsCraft.Add(_editCraftEngs);
            gridCraft[0, 13] = new SourceGrid.Cells.ColumnHeader("Source");
            _editorsCraft.Add(editStr1);
            gridCraft[0, 14] = new SourceGrid.Cells.ColumnHeader("PicCnt");
            _editorsCraft.Add(null);
            gridCraft[0, 15] = new SourceGrid.Cells.ColumnHeader("Type");
            _editorsCraft.Add(editStr255);
            gridCraft[0, 16] = new SourceGrid.Cells.ColumnHeader("SeeAlso");
            _editorsCraft.Add(_editCraft6);
            gridCraft[0, 17] = new SourceGrid.Cells.ColumnHeader("FullName");
            _editorsCraft.Add(null);
            gridCraft[0, 18] = new SourceGrid.Cells.ColumnHeader("Wiki");
            _editorsCraft.Add(editStr255);
            gridCraft[0, 19] = new SourceGrid.Cells.ColumnHeader("Airwar");
            _editorsCraft.Add(editStr255);
            gridCraft[0, 20] = new SourceGrid.Cells.ColumnHeader("FlyingM");
            _editorsCraft.Add(_editCraft7);
            gridCraft[0, 21] = new SourceGrid.Cells.ColumnHeader("Same");
            _editorsCraft.Add(_editCraft6);
            gridCraft[0, 22] = new SourceGrid.Cells.ColumnHeader("SeeAlsoId");
            gridCraft.Columns[22].Visible = false;
            gridCraft[0, 23] = new SourceGrid.Cells.ColumnHeader("FlyingMId");
            gridCraft.Columns[23].Visible = false;
            gridCraft[0, 24] = new SourceGrid.Cells.ColumnHeader("SameId");
            gridCraft.Columns[24].Visible = false;

            for (var i = 1; i < gridCraft.ColumnsCount; i++)
            {
                var key = $"gridCraft:ColumnWidth:{i}";
                if (_config.ContainsKey(key))
                {
                    gridCraft.Columns[i].Width = int.Parse(_config[key]);
                }
            }

            _normCraftCellView = new SourceGrid.Cells.Views.Cell();
            _normCraftCheckView = new SourceGrid.Cells.Views.CheckBox();
            _redCraftCellView = new SourceGrid.Cells.Views.Cell();
            _redCraftCellView.BackColor = Color.FromArgb(0xff, 0xcc, 0xcc);
            _redCraftCheckView = new SourceGrid.Cells.Views.CheckBox();
            _redCraftCheckView.BackColor = Color.FromArgb(0xff, 0xcc, 0xcc);
            _grayCraftCellView = new SourceGrid.Cells.Views.Cell();
            _grayCraftCellView.BackColor = Color.FromArgb(0xdd, 0xdd, 0xdd);
            _grayCraftCheckView = new SourceGrid.Cells.Views.CheckBox();
            _grayCraftCheckView.BackColor = Color.FromArgb(0xdd, 0xdd, 0xdd);
            _navyCraftCellView = new SourceGrid.Cells.Views.Cell();
            _navyCraftCellView.BackColor = Color.FromArgb(0xdd, 0xdd, 0xff);
            _navyCraftCheckView = new SourceGrid.Cells.Views.CheckBox();
            _navyCraftCheckView.BackColor = Color.FromArgb(0xdd, 0xdd, 0xff);

            _gridCraftController = new GridCraftController(this);

            gridCraft.Selection.CellGotFocus += CraftCellGotFocus;
            gridCraft.Columns.ColumnWidthChanged += CraftColumnWidthChanged;
        }

        public void LoadCrafts()
        {
            lWorking.Visible = true;
            Application.DoEvents();

            var CraftsQry = _ctx.vwCrafts.AsNoTracking().AsQueryable();

            _filteredMags = null;
            _filteredArtId = -1;
            if (chFilterCraftsByMag.Checked && _selectedArt != null)
            {
                if (_filteredArtId != _selectedArt.ArtId) LoadFilteredMags();
                CraftsQry = CraftsQry.Where(x => _filteredMagCrafts.Contains(x.CraftId));
            }

            var filterText = false;
            List<string> lTxt = null;
            List<string> lTxt2 = null;
            IQueryable<Pics> xPicsQry = null;
            IQueryable<Pics> xPicsQry2 = null;

            if (_filterOn)
            {
                if (_filter.CountriesNo.Count > 0 && _filter.CountriesYes.Count > 0)
                {
                    if (_filter.CountriesNo.Count > _filter.CountriesYes.Count)
                    {
                        CraftsQry = CraftsQry.Where(x => _filter.CountriesYes.Contains(x.Country));
                    }else
                    {
                        CraftsQry = CraftsQry.Where(x => !_filter.CountriesNo.Contains(x.Country));
                    }
                }
                if (_filter.SourcesNo.Any())
                {
                    CraftsQry = CraftsQry.Where(x => !_filter.SourcesNo.Contains(x.Source));
                }

                if (_filter.VertYes != _filter.VertNo)
                {
                    CraftsQry = CraftsQry.Where(x => (x.Vert ?? false) == _filter.VertYes);
                };
                if (_filter.UavYes != _filter.UavNo)
                {
                    CraftsQry = CraftsQry.Where(x => (x.Uav ?? false) == _filter.UavYes);
                };
                if (_filter.GlidYes != _filter.GlidNo)
                {
                    CraftsQry = CraftsQry.Where(x => (x.Glider ?? false) == _filter.GlidYes);
                };
                if (_filter.LlYes != _filter.LlNo)
                {
                    CraftsQry = CraftsQry.Where(x => (x.LL ?? false) == _filter.LlYes);
                };
                if (_filter.SinglYes != _filter.SinglNo)
                {
                    CraftsQry = CraftsQry.Where(x => (x.Single ?? false) == _filter.SinglYes);
                };
                if (_filter.ProjYes != _filter.ProjNo)
                {
                    CraftsQry = CraftsQry.Where(x => (x.Proj ?? false) == _filter.ProjYes);
                };

                if (_filter.YearFrom > 0)
                {
                    CraftsQry = CraftsQry.Where(x => x.IYear.HasValue && x.IYear >= _filter.YearFrom);
                }
                if (_filter.YearTo > 0)
                {
                    CraftsQry = CraftsQry.Where(x => x.IYear.HasValue && x.IYear <= _filter.YearTo);
                }

                if (!string.IsNullOrEmpty(_filter.Wings))
                {
                    var lWings = _filter.Wings.Split(' ');
                    foreach(var wng in lWings)
                    {
                        if (!string.IsNullOrEmpty(wng))
                        CraftsQry = CraftsQry.Where(x => x.Wings.Contains(wng));
                    }
                }
                if (!string.IsNullOrEmpty(_filter.Engines))
                {
                    var lEngs = _filter.Engines.Split(' ');
                    foreach (var eng in lEngs)
                    {
                        if (!string.IsNullOrEmpty(eng))
                            CraftsQry = CraftsQry.Where(x => x.Engines.Contains(eng));
                    }
                }

                if (!string.IsNullOrEmpty(_filter.Text) || !string.IsNullOrEmpty(_filter.Text2))
                {
                    lTxt = ExtractFilters(_filter.Text, _filter.CaseSensitive);
                    lTxt2 = ExtractFilters(_filter.Text2, _filter.CaseSensitive);
                    if (lTxt.Any() || lTxt2.Any())
                    {
                        filterText = true;

                        var xCraftsQry = _ctx.Crafts.AsNoTracking().AsQueryable();
                        var xCraftsQry2 = _ctx.Crafts.AsNoTracking().AsQueryable();
                        if (_filter.InText)
                        {
                            xPicsQry = _ctx.Pics.AsNoTracking().AsQueryable();
                            xPicsQry2 = _ctx.Pics.AsNoTracking().AsQueryable();
                        }
                        if (lTxt.Any())
                        {
                            foreach (var t in lTxt)
                            {
                                if (_filter.InText)
                                    xCraftsQry = xCraftsQry.Where(x => (x.Construct + " " + x.Name + " " + x.Type + " " + x.CText).Contains(t));
                                else
                                    xCraftsQry = xCraftsQry.Where(x => (x.Construct + " " + x.Name + " " + x.Type).Contains(t));
                            }
                            if (_filter.InText)
                            {
                                foreach (var t in lTxt)
                                {
                                    xPicsQry = xPicsQry.Where(x => (x.Text).Contains(t));
                                }
                            }
                        }
                        if (lTxt2.Any())
                        {
                            foreach (var t in lTxt2)
                            {
                                if (_filter.InText)
                                    xCraftsQry2 = xCraftsQry2.Where(x => (x.Construct + " " + x.Name + " " + x.Type + " " + x.CText).Contains(t));
                                else
                                    xCraftsQry2 = xCraftsQry2.Where(x => (x.Construct + " " + x.Name + " " + x.Type).Contains(t));
                            }
                            if (_filter.InText)
                            {
                                foreach (var t in lTxt)
                                {
                                    xPicsQry2 = xPicsQry2.Where(x => (x.Text).Contains(t));
                                }
                            }
                        }
                        if (lTxt.Any() && lTxt2.Any())
                            if (_filter.InText)
                                CraftsQry = CraftsQry
                                    .Where(x => xCraftsQry.Select(y => y.CraftId).Contains(x.CraftId) || xPicsQry.Select(y => y.CraftId).Contains(x.CraftId) ||
                                                xCraftsQry2.Select(y => y.CraftId).Contains(x.CraftId) || xPicsQry2.Select(y => y.CraftId).Contains(x.CraftId));
                            else
                                CraftsQry = CraftsQry
                                    .Where(x => xCraftsQry.Select(y => y.CraftId).Contains(x.CraftId) ||
                                                xCraftsQry2.Select(y => y.CraftId).Contains(x.CraftId));
                        else if (lTxt.Any())
                            if (_filter.InText)
                                CraftsQry = CraftsQry.Where(x => xCraftsQry.Select(y => y.CraftId).Contains(x.CraftId) || xPicsQry.Select(y => y.CraftId).Contains(x.CraftId));
                            else
                                CraftsQry = CraftsQry.Where(x => xCraftsQry.Select(y => y.CraftId).Contains(x.CraftId));
                        else //if (lTxt2.Any())
                            if (_filter.InText)
                                CraftsQry = CraftsQry.Where(x => xCraftsQry2.Select(y => y.CraftId).Contains(x.CraftId) || xPicsQry2.Select(y => y.CraftId).Contains(x.CraftId));
                            else
                                CraftsQry = CraftsQry.Where(x => xCraftsQry2.Select(y => y.CraftId).Contains(x.CraftId));
                    }
                }
            } else
            {
                CraftsQry = CraftsQry.Where(x => !Const.Sources.ReadOnly.Contains(x.Source));
            }


            if (chCraftSortConstruct.Checked)
                CraftsQry = CraftsQry.OrderBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name);
            else if (chCraftSortCountry.Checked)
                CraftsQry = CraftsQry.OrderBy(x => x.Country).ThenBy(x => x.Construct).ThenBy(x => x.IYear).ThenBy(x => x.Name);
            else if (chCraftSortYear.Checked)
                CraftsQry = CraftsQry.OrderBy(x => x.IYear).ThenBy(x => x.Country).ThenBy(x => x.Construct).ThenBy(x => x.Name);
            var Crafts = CraftsQry.ToList();

            if (filterText && (_filter.WholeWords || _filter.CaseSensitive))
            {
                lWorkingText.Text = "********************";
                lWorkingText.Visible = true;
                Application.DoEvents();

                var cnt = 0;
                var all = Crafts.Count;
                var v = 0;
                Crafts = Crafts.Where(x => {
                    cnt++;
                    var v0 = 20 * cnt / all;
                    if (v != v0)
                    {
                        v = v0;
                        lWorkingText.Text = new String('-', v) + (v < 20 ? new String('*', 20 - v) : "");
                        Application.DoEvents();
                    }

                    var s = $" {x.Construct} {x.Name} {x.Type} ";
                    if (_filter.InText)
                    {
                        var crf = _ctx.Crafts.AsNoTracking().Single(y => y.CraftId == x.CraftId);
                        s += $"{crf.CText} ";
                    }
                    if (!_filter.CaseSensitive) s = s.ToLower();
                    var isOk = false;
                    foreach(var t in lTxt)
                    {
                        var found = false;
                        var i = s.IndexOf(t);
                        while (i >= 0)
                        {
                            if (!Char.IsLetterOrDigit(s[i - 1]) && !Char.IsLetterOrDigit(s[i + t.Length])) { found = true; break; };
                            i = s.IndexOf(t, i + 1);
                        }
                        if (found) isOk = true;
                        else break;
                    }
                    if (isOk) return true;

                    isOk = false;
                    foreach (var t in lTxt2)
                    {
                        var found = false;
                        var i = s.IndexOf(t);
                        while (i >= 0)
                        {
                            if (!Char.IsLetterOrDigit(s[i - 1]) && !Char.IsLetterOrDigit(s[i + t.Length])) { found = true; break; };
                            i = s.IndexOf(t, i + 1);
                        }
                        if (found) isOk = true;
                        else break;
                    }
                    if (isOk) return true;
                    if (!_filter.InText) return false;

                    var pics = xPicsQry.Where(y => y.CraftId == x.CraftId).ToList();
                    s = " ";
                    foreach (var p in pics) s += p.Text + " ";
                    if (!_filter.CaseSensitive) s = s.ToLower();
                    isOk = false;
                    foreach (var t in lTxt)
                    {
                        var found = false;
                        var i = s.IndexOf(t);
                        while (i >= 0)
                        {
                            if (!Char.IsLetterOrDigit(s[i - 1]) && !Char.IsLetterOrDigit(s[i + t.Length])) { found = true; break; };
                            i = s.IndexOf(t, i + 1);
                        }
                        if (found) isOk = true;
                        else break;
                    }
                    if (isOk) return true;

                    pics = xPicsQry2.Where(y => y.CraftId == x.CraftId).ToList();
                    s = " ";
                    foreach (var p in pics) s += p.Text + " ";
                    if (!_filter.CaseSensitive) s = s.ToLower();
                    isOk = false;
                    foreach (var t in lTxt2)
                    {
                        var found = false;
                        var i = s.IndexOf(t);
                        while (i >= 0)
                        {
                            if (!Char.IsLetterOrDigit(s[i - 1]) && !Char.IsLetterOrDigit(s[i + t.Length])) { found = true; break; };
                            i = s.IndexOf(t, i + 1);
                        }
                        if (found) isOk = true;
                        else break;
                    }
                    return isOk;
                }).ToList();

                lWorkingText.Visible = false;
                Application.DoEvents();
            }

            _craftDtos = Mapper.Map<List<CraftDto>>(Crafts);
            lCraftCnt.Text = _craftDtos.Count.ToString();

            var saved = -1;
            if (_craftPosition != Position.Empty)
            {
                saved = (int)gridCraft[_craftPosition.Row, Const.Columns.Craft.CraftId].Value;
            }

            gridCraft.RowsCount = _craftDtos.Count + 1;

            _crafts = _craftDtos.OrderBy(x => x.Sort).Select(x => Mapper.Map<CraftDto>(x)).ToList();

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
            } else
            {
                _craftPosition = Position.Empty;
            }

            _crafts6 = _craftDtos.Where(x => x.Source == "6").OrderBy(x => x.Sort).ToList();
            _editCraft6.Control.Items.Clear();
            _editCraft6.Control.Items.AddRange(_crafts6.Select(x => x.FullName).ToArray());

            _crafts7 = _craftDtos.Where(x => x.Source == "7").OrderBy(x => x.Sort).ToList();
            _editCraft7.Control.Items.Clear();
            _editCraft7.Control.Items.AddRange(_crafts7.Select(x => x.FullName).ToArray());

            _crafts678 = _craftDtos.Where(x => x.Source == "6" || x.Source == "7" || x.Source == "8").OrderBy(x => x.Sort).ToList();
            _editCraft678.Control.Items.Clear();
            _editCraft678.Control.Items.AddRange(_crafts678.Select(x => x.FullName).ToArray());

            gridCraft.Refresh();

            lWorking.Visible = false;
            Application.DoEvents();
        }

        private List<string> ExtractFilters(string txt, bool caseSensitive)
        {
            var res = new List<string>();
            var i = txt.IndexOf('\'');
            while (i >= 0)
            {
                var j = txt.IndexOf('\'', i + 1);
                if (j < 0) j = txt.Length;
                var s = txt.Substring(i + 1, j - i - 1);
                if (!caseSensitive) s = s.ToLower();
                res.Add(s);
                txt = (i > 0 ? txt.Substring(0, i - 1) : "") + " " + (j < txt.Length ? txt.Substring(j + 1) : "");
                i = txt.IndexOf('\'');
            }
            i = txt.IndexOf('"');
            while (i >= 0)
            {
                var j = txt.IndexOf('"', i + 1);
                if (j < 0) j = txt.Length;
                var s = txt.Substring(i + 1, j - i - 1);
                if (!caseSensitive) s = s.ToLower();
                res.Add(s);
                txt = (i > 0 ? txt.Substring(0, i - 1) : "") + " " + (j < txt.Length ? txt.Substring(j + 1) : "");
                i = txt.IndexOf('"');
            }
            var lTxt = txt.Split(' ');
            foreach(var t in lTxt)
            {
                if (!string.IsNullOrEmpty(t.Trim()))
                {
                    var s = t.Trim();
                    if (!caseSensitive) s = s.ToLower();
                    res.Add(s);
                }
            }
            return res;
        }

        public void UpdateCraftRow(CraftDto Craft, int r)
        {
            var isReadOnly = Const.Sources.ReadOnly.Contains(Craft.Source);
            gridCraft[r, 0] = new SourceGrid.Cells.Cell(Craft.CraftId, isReadOnly ? null : _editorsCraft[0]);
            gridCraft[r, 0].View = _normCraftCellView;
            gridCraft[r, 1] = new SourceGrid.Cells.Cell(Craft.Construct, isReadOnly ? null : _editorsCraft[1]);
            gridCraft[r, 1].AddController(_gridCraftController);
            gridCraft[r, 2] = new SourceGrid.Cells.Cell(Craft.Name, isReadOnly ? null : _editorsCraft[2]);
            gridCraft[r, 2].AddController(_gridCraftController);
            gridCraft[r, 3] = new SourceGrid.Cells.Cell(Craft.Country, isReadOnly ? null : _editorsCraft[3]);
            gridCraft[r, 3].AddController(_gridCraftController);
            gridCraft[r, 4] = new SourceGrid.Cells.Cell(Craft.IYear, isReadOnly ? null : _editorsCraft[4]);
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
            gridCraft[r, 11] = new SourceGrid.Cells.Cell(Craft.Wings, isReadOnly ? null : _editorsCraft[11]);
            gridCraft[r, 11].AddController(_gridCraftController);
            gridCraft[r, 12] = new SourceGrid.Cells.Cell(Craft.Engines, isReadOnly ? null : _editorsCraft[12]);
            gridCraft[r, 12].AddController(_gridCraftController);
            gridCraft[r, 13] = new SourceGrid.Cells.Cell(Craft.Source, isReadOnly ? null : _editorsCraft[13]);
            gridCraft[r, 13].AddController(_gridCraftController);
            gridCraft[r, 14] = new SourceGrid.Cells.Cell(Craft.PicCnt, isReadOnly ? null : _editorsCraft[14]);
            gridCraft[r, 15] = new SourceGrid.Cells.Cell(Craft.Type, isReadOnly ? null : _editorsCraft[15]);
            gridCraft[r, 15].AddController(_gridCraftController);
            var craftName = "";
            if (Craft.SeeAlso.HasValue)
            {
                var craft = _crafts.FirstOrDefault(x => x.CraftId == Craft.SeeAlso);
                if (craft != null) craftName = craft.FullName;
            }
            gridCraft[r, 16] = new SourceGrid.Cells.Cell(craftName, Craft.Source == "6" ? _editorsCraft[16] : null);
            gridCraft[r, 16].AddController(_gridCraftController);
            gridCraft[r, 17] = new SourceGrid.Cells.Cell(Craft.FullName, isReadOnly ? null : _editorsCraft[17]);
            gridCraft[r, 18] = new SourceGrid.Cells.Cell(Craft.Wiki, isReadOnly ? null : _editorsCraft[18]);
            gridCraft[r, 18].AddController(_gridCraftController);
            gridCraft[r, 19] = new SourceGrid.Cells.Cell(Craft.Airwar, isReadOnly ? null : _editorsCraft[18]);
            gridCraft[r, 19].AddController(_gridCraftController);
            craftName = "";
            if (Craft.FlyingM.HasValue)
            {
                var craft = _crafts.FirstOrDefault(x => x.CraftId == Craft.FlyingM);
                if (craft != null) craftName = craft.FullName;
            }
            gridCraft[r, 20] = new SourceGrid.Cells.Cell(craftName, Craft.Source == "6" ? _editorsCraft[20] : null);
            gridCraft[r, 20].AddController(_gridCraftController);
            craftName = "";
            if (Craft.Same.HasValue)
            {
                var craft = _crafts.FirstOrDefault(x => x.CraftId == Craft.Same);
                if (craft != null) craftName = craft.FullName;
            }
            gridCraft[r, 21] = new SourceGrid.Cells.Cell(craftName, Craft.Source == "6" ? _editorsCraft[21] : null);
            gridCraft[r, 21].AddController(_gridCraftController);
            gridCraft[r, 22] = new SourceGrid.Cells.Cell(Craft.SeeAlso);
            gridCraft[r, 23] = new SourceGrid.Cells.Cell(Craft.FlyingM);
            gridCraft[r, 24] = new SourceGrid.Cells.Cell(Craft.Same);

            SetCraftRowColor(Craft, r);
        }

        private void SetCraftRowColor(CraftDto Craft, int r)
        {
            var view = _normCraftCellView;
            var viewCheck = _normCraftCheckView;
            if (Const.Sources.ReadOnly.Contains(Craft.Source))
            {
                view = _navyCraftCellView;
                viewCheck = _navyCraftCheckView;
            }
            else if (Craft.Same.HasValue)
            {
                view = _grayCraftCellView;
                viewCheck = _grayCraftCheckView;
            }
            else if (Craft.Source == "6" && Craft.IYear > 0 && Craft.IYear < 1920 && !Craft.FlyingM.HasValue)
            {
                view = _redCraftCellView;
                viewCheck = _redCraftCheckView;
            }

            if (gridCraft[r, 0].View != view)
            {
                for (var i = 0; i < gridCraft.ColumnsCount; i++)
                {
                    gridCraft[r, i].View = gridCraft[r, i].View is SourceGrid.Cells.Views.CheckBox ?
                        viewCheck : view;
                }
            }
        }


        public void UpdateCraftRow2(CraftDto Craft, int r)
        {
            if (!string.IsNullOrEmpty(gridCraft[r, Const.Columns.Craft.FlyingMId].DisplayText))
            {
                var craft = _crafts.FirstOrDefault(x => x.CraftId == (int)gridCraft[r, Const.Columns.Craft.FlyingMId].Value);
                if (craft != null)
                {
                    gridCraft[r, 20] = new SourceGrid.Cells.Cell(Craft.FlyingM, _editorsCraft[20]);
                    gridCraft[r, 20].AddController(_gridCraftController);
                }
            }
            if (!string.IsNullOrEmpty(gridCraft[r, Const.Columns.Craft.SameId].DisplayText))
            {
                var craft = _crafts.FirstOrDefault(x => x.CraftId == (int)gridCraft[r, Const.Columns.Craft.SameId].Value);
                if (craft != null)
                {
                    gridCraft[r, 21] = new SourceGrid.Cells.Cell(Craft.Same, _editorsCraft[21]);
                    gridCraft[r, 21].AddController(_gridCraftController);
                }
            }

        }

        public CraftDto GetCraftFromTable(int r)
        {
            var craft = new CraftDto()
            {
                CraftId = (int)gridCraft[r, 0].Value,
                Construct = (string)gridCraft[r, 1].Value,
                Name = (string)gridCraft[r, 2].Value,
                Country = (string)gridCraft[r, 3].Value,
                IYear = (int?)gridCraft[r, 4].Value,
                Vert = (bool)gridCraft[r, 5].Value,
                Uav = (bool)gridCraft[r, 6].Value,
                Glider = (bool)gridCraft[r, 7].Value,
                LL = (bool)gridCraft[r, 8].Value,
                Single = (bool)gridCraft[r, 9].Value,
                Proj = (bool)gridCraft[r, 10].Value,
                Wings = (string)gridCraft[r, 11].Value,
                Engines = (string)gridCraft[r, 12].Value,
                Source = (string)gridCraft[r, 13].Value,
                Type = (string)gridCraft[r, 15].Value,
                Wiki = (string)gridCraft[r, 18].Value,
                Airwar = (string)gridCraft[r, 19].Value,
                SeeAlso = (int?)gridCraft[r, 22].Value,
                FlyingM = (int?)gridCraft[r, 23].Value,
                Same = (int?)gridCraft[r, 24].Value
            };
            return craft;
        }

       private class GridCraftController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly Form1 _form;

            public GridCraftController(Form1 form)
            {
                _form = form;
            }

            public override void OnEditStarting(CellContext sender, System.ComponentModel.CancelEventArgs e)
            {
                base.OnEditStarting(sender, e);

                var src = _form.gridCraft[sender.Position.Row, Const.Columns.Craft.Source].DisplayText;
                if (Const.Sources.ReadOnly.Contains(src))
                {
                    e.Cancel = true;
                }
            }

            public override void OnEditStarted(SourceGrid.CellContext sender, EventArgs e)
            {
                _form._editingCraftFullName = _form.gridCraft[sender.Position.Row, Const.Columns.Craft.FullName].DisplayText;
                base.OnEditStarted(sender, e);
                _form.SearchModeOff();
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
                        _form.gridCraft[row, Const.Columns.Craft.SeeAlsoId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._crafts6.SingleOrDefault(x => x.FullName == val)?.CraftId;
                        break;
                    case Const.Columns.Craft.Same:
                        _form.gridCraft[row, Const.Columns.Craft.SameId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._crafts6.SingleOrDefault(x => x.FullName == val)?.CraftId;
                        break;
                    case Const.Columns.Craft.FlyingM:
                        _form.gridCraft[row, Const.Columns.Craft.FlyingMId].Value = string.IsNullOrEmpty(val) ? null :
                            _form._crafts7.SingleOrDefault(x => x.FullName == val)?.CraftId;
                        break;
                }

                var isForce = false;
                var craftDto = _form.GetCraftFromTable(row);
                if (craftDto.CraftId == 0)
                {
                    var entity = Mapper.Map<Crafts>(craftDto);
                    _form._ctx.Crafts.Add(entity);
                    _form._ctx.SaveChanges();
                    craftDto.CraftId = entity.CraftId;
                    _form.gridCraft[row, Const.Columns.Craft.CraftId].Value = entity.CraftId;
                    isForce = true;
                }
                else
                {
                    var entity = _form._ctx.Crafts.Single(x => x.CraftId == craftDto.CraftId);
                    Mapper.Map(craftDto, entity);
                    _form._ctx.SaveChanges();
                }
                _form._selectedCraft = craftDto;
                _form._craftDtos[row - 1] = craftDto;
                var newCraft = craftDto;

                if (newCraft.FullName != _form._editingCraftFullName)
                {
                    var isRefresh = false;
                    _form.gridCraft[row, Const.Columns.Craft.FullName].Value = newCraft.FullName;
                    var craft = _form._crafts.FirstOrDefault(x => x.CraftId == newCraft.CraftId);
                    if (craft != null) _form._crafts.Remove(craft);
                    craft = _form._crafts6.FirstOrDefault(x => x.CraftId == newCraft.CraftId);
                    if (craft != null) { _form._crafts6.Remove(craft); isRefresh = true; }
                    craft = _form._crafts7.FirstOrDefault(x => x.CraftId == newCraft.CraftId);
                    if (craft != null) { _form._crafts7.Remove(craft); isRefresh = true; }
                    var source = (string)_form.gridCraft[row, Const.Columns.Craft.Source].Value;
                    var list = source == "6" ? _form._crafts6 : source == "7" ? _form._crafts7 : null;
                    if (list != null)
                    {
                        list.InsertCraftSorted(newCraft);
                        isRefresh = true;
                    }
                    if ((new string[] { "6", "7", "8" }).Contains(source))
                    {
                        craft = _form._crafts678.FirstOrDefault(x => x.CraftId == newCraft.CraftId);
                        if (craft != null) _form._crafts678.Remove(craft);
                        _form._crafts678.InsertCraftSorted(newCraft);
                        isRefresh = true; 
                    }
                    if (isRefresh)
                    {
                        _form._editCraft6.Control.Items.Clear();
                        _form._editCraft6.Control.Items.AddRange(_form._crafts6.Select(x => x.FullName).ToArray());

                        _form._editCraft7.Control.Items.Clear();
                        _form._editCraft7.Control.Items.AddRange(_form._crafts7.Select(x => x.FullName).ToArray());

                        _form._editCraft678.Control.Items.Clear();
                        _form._editCraft678.Control.Items.AddRange(_form._crafts678.Select(x => x.FullName).ToArray());
                    }
                }

                _form.SetCraftRowColor(_form._selectedCraft, row);

                _form.CheckCraftSort(sender.Position, isForce);
            }
        }

        private void CheckCraftSort(Position pos, bool isForce)
        {
            int[] sortIndexes;
            if (chCraftSortConstruct.Checked)
                sortIndexes = Const.Columns.Craft.SortConstruct;
            else if (chCraftSortCountry.Checked)
                sortIndexes = Const.Columns.Craft.SortCountry;
            else //if (_form.chCraftSortYear.Checked)
                sortIndexes = Const.Columns.Craft.SortYear;

            if (isForce || sortIndexes.Contains(pos.Column))
            {
                var sortVal = "";
                var rNew = pos.Row;
                foreach (var col in sortIndexes) sortVal += gridCraft[pos.Row, col].DisplayText + " ";
                if (pos.Row < gridCraft.RowsCount)
                {
                    var found = false;
                    for (var r = pos.Row + 1; r < gridCraft.RowsCount; r++)
                    {
                        var sortRVal = "";
                        foreach (var col in sortIndexes) sortRVal += gridCraft[r, col].DisplayText + " ";
                        if (string.Compare(sortVal, sortRVal) <= 0)
                        {
                            rNew = r - 1;
                            found = true;
                            break;
                        }
                    }
                    if (!found) rNew = gridCraft.RowsCount - 1;
                }
                if (rNew == pos.Row && pos.Row > 1)
                {
                    var found = false;
                    for (var r = pos.Row - 1; r >= 1; r--)
                    {
                        var sortRVal = "";
                        foreach (var col in sortIndexes) sortRVal += gridCraft[r, col].DisplayText + " ";
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
                    gridCraft.Rows.Move(pos.Row, rNew);
                    _craftDtos.Move(pos.Row - 1, rNew - 1);
                    var focusPosn = new Position(rNew, pos.Column);
                    gridCraft.Selection.Focus(focusPosn, true);
                }

            }
        }

        private void SelectCraft(int? craftId)
        {
            if (!craftId.HasValue) return;
            int row;
            var craftDto = _craftDtos.Where(x => x.CraftId == craftId).SingleOrDefault();
            if (craftDto == null)
            {
                craftDto = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().Single(x => x.CraftId == craftId));
                _craftDtos.Add(craftDto);
                gridCraft.RowsCount++;
                row = gridCraft.RowsCount - 1;
                UpdateCraftRow(craftDto, row);
                gridCraft.Refresh();
            }
            else
            {
                row = _craftDtos.IndexOf(craftDto) + 1;
            }
            var pos = gridCraft.Selection.ActivePosition;
            var newPos = new Position(row, pos == Position.Empty ? 1 : pos.Column);
            gridCraft.Selection.Focus(newPos, true);
            _needLoadCraft = false;
        }

        private void CraftCellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            DoCraftCellGotFocus(e.NewFocusPosition);
        }

        private void StoreEditedCraftText()
        {
            if (_selectedCraft != null && _craftTextChanged)
            {
                var craftOld = _ctx.Crafts.SingleOrDefault(x => x.CraftId == _selectedCraft.CraftId);
                if (craftOld != null)
                {
                    craftOld.CText = _oldCraftText;
                    _ctx.SaveChanges();

                    _ctx.Database.ExecuteSqlCommand($"delete from WordLinks where CraftId ={craftOld.CraftId}");
                    var wordsList = GetWords(_oldCraftText, true);
                    foreach (var word in wordsList)
                    {
                        _ctx.Database.ExecuteSqlCommand($"insert into WordLinks (WordId, CraftId) Values ({word.Id}, {craftOld.CraftId})");
                    }
                    _ctx.Database.ExecuteSqlCommand($"delete from Words where Cnt = 0");
                }
            }
            _craftTextChanged = false;
        }

        private void DoCraftCellGotFocus(Position pos)
        {
            _craftPosition = pos;
            _needLoadCraft = false;
            var craft = _craftDtos[_craftPosition.Row - 1];
            lCraft.Text = craft.FullName;

            StoreEditedCraftText();

            if (_selectedCraft == null || _selectedCraft.CraftId != craft.CraftId)
            {
                _craftTextChanging = true;
                edCraftText.Text = "";
                _oldCraftText = edCraftText.Text;
                edCraftText.SelectionStart = 0;
                edCraftText.SelectionLength = 1;
                edCraftText.SelectionFont = new Font(edPicText.SelectionFont, FontStyle.Regular);
                edCraftText.SelectionColor = Color.Black;
                edCraftText.SelectionBackColor = Color.White;
                edCraftText.ReadOnly = Const.Sources.ReadOnly.Contains(craft.Source);
                _craftTextChanging = false;
                _craftTextChanged = false;

                _selectedCraft = craft;
                _craftTextChanging = true;
                var craftText = _ctx.Crafts.SingleOrDefault(x => x.CraftId == craft.CraftId);
                if (craftText != null)
                {
                    edCraftText.Text = craftText.CText;
                    _oldCraftText = edCraftText.Text;
                    if (!chFilterCraftsByMag.Checked) ColorizeText(edCraftText, true);
                    _craftTextChanging = false;
                    _craftTextChanged = false;
                }

                if (craft.CraftId > 0 || craft.Same.HasValue)
                {
                    var picQry = _ctx.vwPics.AsNoTracking().Where(x => x.CraftId == craft.CraftId || x.CraftId == craft.Same);
                    if (chFilterCraftsByMag.Checked && _filteredMags != null)
                    {
                        picQry = picQry.Where(x => _filteredMags.Contains(x.ArtId));
                    }
                    _craftPicDtos = picQry.OrderBy(x => x.NType).ThenBy(x => x.NNN)
                        .Select(Mapper.Map<PicDto>).ToList();
                    if (_craftPicDtos.Any())
                    {
                        imgCraftPic.Visible = true;
                        sbCraftPics.Visible = false;
                        sbCraftPics.Value = 0;
                        _craftPicDtoIndex = 0;
                        if (_craftPicDtos.Count > 1)
                        {
                            sbCraftPics.Maximum = _craftPicDtos.Count - 1;
                            while (_craftPicDtoIndex < _craftPicDtos.Count && _craftPicDtos[_craftPicDtoIndex].Type?.Trim() == "s")
                                _craftPicDtoIndex++;
                            if (_craftPicDtoIndex >= _craftPicDtos.Count)
                                _craftPicDtoIndex = 0;
                            sbCraftPics.Value = _craftPicDtoIndex;
                            sbCraftPics.Visible = true;
                        }
                        ShowCraftPic();
                    }
                    else
                    {
                        imgCraftPic.Visible = false;
                        sbCraftPics.Visible = false;
                        tCraftPicTxt.Text = "";
                        _craftPicDtoIndex = -1;
                    }

                    if (string.IsNullOrEmpty(_selectedCraft.Airwar))
                    {
                        bCraftAirwarLink.ForeColor = Color.Gray;
                        bCraftAirwarLink.BackColor = Color.White;
                    }
                    else
                    {
                        bCraftAirwarLink.ForeColor = Color.Black;
                        bCraftAirwarLink.BackColor = Color.LightGray;
                    }
                    if (string.IsNullOrEmpty(_selectedCraft.Wiki))
                    {
                        bCraftWikiLink.ForeColor = Color.Gray;
                        bCraftWikiLink.BackColor = Color.White;
                    }
                    else
                    {
                        bCraftWikiLink.ForeColor = Color.Black;
                        bCraftWikiLink.BackColor = Color.LightGray;
                    }
                    if (!_selectedCraft.FlyingM.HasValue)
                    {
                        bCraftFlyingLink.Enabled = false;
                        bCraftFlyingLink.ForeColor = Color.Gray;
                        bCraftFlyingLink.BackColor = Color.White;
                    }
                    else
                    {
                        bCraftFlyingLink.Enabled = true;
                        bCraftFlyingLink.ForeColor = Color.Black;
                        bCraftFlyingLink.BackColor = Color.LightGray;
                    }

                    lstCraftSeeAlso.Items.Clear();
                    var crft = _selectedCraft;
                    var crafts = new List<CraftDto>();
                    if (crft.SeeAlso.HasValue)
                    {
                        crafts.Add(crft);
                        while (crft.SeeAlso.HasValue && !crafts.Any(x => x.CraftId == crft.SeeAlso))
                        {
                            crft = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().Single(x => x.CraftId == crft.SeeAlso));
                            crafts.Add(crft);
                        }
                    }
                    var isOk = !crafts.Any() || crafts.First().CraftId == crafts.Last().SeeAlso;

                    var sameCrafts = _ctx.vwCrafts.AsNoTracking().Where(x => x.Same == _selectedCraft.CraftId).Select(Mapper.Map<CraftDto>).ToList();
                    crafts.AddRange(sameCrafts);

                    if (_selectedCraft.Same.HasValue)
                    {
                        crft = Mapper.Map<CraftDto>(_ctx.vwCrafts.AsNoTracking().Single(x => x.CraftId == _selectedCraft.Same));
                        crafts.Add(crft);
                    }

                    crafts.Sort((f1, f2) =>
                    {
                        return (f1.IYear ?? 0).CompareTo(f2.IYear ?? 0);
                    });
                    foreach (var c in crafts)
                    {
                        lstCraftSeeAlso.Items.Add(new Pair<int>() { Id = c.CraftId, Name = (c.Same.HasValue ? "- " : "") + c.FullName });
                    }
                    lstCraftSeeAlso.ForeColor = isOk ? Color.Black : Color.Red;
                }
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
                    isDef = true;
                    break;
            }

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
                if (!found && isDef) _searchString = oldSearch;
            }
            lInfo.Text = $"Поиск: {_searchString}";
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
                        _craftDtos.Insert(pos.Row - 1, Mapper.Map<CraftDto>(Craft));
                        gridCraft.Rows.Insert(pos.Row);
                        UpdateCraftRow(Craft, pos.Row);
                        DoCraftCellGotFocus(pos);
                        CheckCraftSort(pos, true);
                    }
                    else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
                    {
                        if (MessageBox.Show("Delete craft?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                var craftId = (int)gridCraft[pos.Row, Const.Columns.Craft.CraftId].Value;
                                if (craftId > 0)
                                {
                                    _ctx.Database.ExecuteSqlCommand($"delete from WordLinks where CraftId ={craftId}");
                                    var entity = _ctx.Crafts.Single(x => x.CraftId == craftId);
                                    _ctx.Crafts.Remove(entity);
                                    _ctx.SaveChanges();
                                }

                                var craft = _crafts.FirstOrDefault(x => x.CraftId == craftId);
                                if (craft != null) _crafts.Remove(craft);
                                craft = _crafts6.FirstOrDefault(x => x.CraftId == craftId);
                                if (craft != null)
                                {
                                    _crafts6.Remove(craft);
                                    _editCraft6.Control.Items.Clear();
                                    _editCraft6.Control.Items.AddRange(_crafts6.Select(x => x.FullName).ToArray());
                                }
                                craft = _crafts7.FirstOrDefault(x => x.CraftId == craftId);
                                if (craft != null)
                                {
                                    _crafts7.Remove(craft);
                                    _editCraft7.Control.Items.Clear();
                                    _editCraft7.Control.Items.AddRange(_crafts7.Select(x => x.FullName).ToArray());
                                }
                                craft = _crafts678.FirstOrDefault(x => x.CraftId == craftId);
                                if (craft != null)
                                {
                                    _crafts678.Remove(craft);
                                    _editCraft678.Control.Items.Clear();
                                    _editCraft678.Control.Items.AddRange(_crafts678.Select(x => x.FullName).ToArray());
                                }

                                gridCraft.Rows.Remove(pos.Row);
                                _craftDtos.RemoveAt(pos.Row - 1);
                                if (pos.Row >= gridCraft.RowsCount) pos = new Position(pos.Row - 1, pos.Column);
                                DoCraftCellGotFocus(pos);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(GetInnerestException(ex));
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Right && e.Modifiers == Keys.Control)
                    {
                        if (_craftPicDtos != null && _craftPicDtos.Any() && (_craftPicDtoIndex + 1) < _craftPicDtos.Count)
                        {
                            _craftPicDtoIndex++;
                            sbCraftPics.Value = _craftPicDtoIndex;
                        }
                    }
                    else if (e.KeyCode == Keys.Left && e.Modifiers == Keys.Control)
                    {
                        if (_craftPicDtos != null && _craftPicDtos.Any() && _craftPicDtoIndex > 0)
                        {
                            _craftPicDtoIndex--;
                            sbCraftPics.Value = _craftPicDtoIndex;
                        }
                    }
                    else if (e.KeyCode == Keys.Home && e.Modifiers == Keys.None)
                    {
                        var col = 0;
                        while (!gridCraft.Columns[col].Visible) col++;
                        var focusPosn = new Position(pos.Row, col);
                        gridCraft.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.None)
                    {
                        var col = gridCraft.Columns.Count - 1;
                        while (!gridCraft.Columns[col].Visible) col--;
                        var focusPosn = new Position(pos.Row, col);
                        gridCraft.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.Home && e.Modifiers == Keys.Control)
                    {
                        var focusPosn = new Position(1, pos.Column);
                        gridCraft.Selection.Focus(focusPosn, true);
                    }
                    else if (e.KeyCode == Keys.End && e.Modifiers == Keys.Control)
                    {
                        var focusPosn = new Position(gridCraft.RowsCount - 1, pos.Column);
                        gridCraft.Selection.Focus(focusPosn, true);
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
            if (_init) return;
            _config["pCraftTextWidth"] = ((Panel)sender).Width.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void edCraftText_TextChanged(object sender, EventArgs e)
        {
            if (_craftTextChanging) return;
            _craftTextChanging = true;

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
                j = newText.Length - j;

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
            _craftTextChanging = false;
            _craftTextChanged = true;
        }

        private void pCraftPic_Resize(object sender, EventArgs e)
        {
            if (_init) return;
            _config["pCraftPicHeight"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void pCraftSeeAlso_Resize(object sender, EventArgs e)
        {
            if (_init) return;
            _config["pCraftSeeAlsoHeight"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void pCraftPicTxt_Resize(object sender, EventArgs e)
        {
            if (_init) return;
            _config["pCraftPicTxt"] = ((Panel)sender).Height.ToString();
            File.WriteAllText(_confPath, JsonConvert.SerializeObject(_config));
        }

        private void sbCraftPics_ValueChanged(object sender, EventArgs e)
        {
            var sb = (HScrollBar)sender;
            _craftPicDtoIndex = sb.Value;
            ShowCraftPic();
        }

        private void ShowCraftPic()
        {
            lCraftPicNum.Text = $"{_craftPicDtoIndex + 1}/{_craftPicDtos.Count}";
            var picDto = _craftPicDtos[_craftPicDtoIndex];
            tCraftPicTxt.Text = picDto.SText;
            try
            {
                var picPath = $"{_imagesPath}Images{_selectedCraft.Source}\\{picDto.Path}";
                using (var bmpTemp = new Bitmap(picPath))
                {
                    imgCraftPic.Image = new Bitmap(bmpTemp);
                }
                //imgCraftPic.Load(picPath);
            } catch
            {

            }
        }

        private void bCraftAirwarLink_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCraft.Airwar))
            {
                var url =
                    "http://www.google.ru/search?q=" +
                    $"{_selectedCraft.Construct} {_selectedCraft.Name} site%3Aairwar.ru"
                      .Replace(" ", "+").Replace("/", "+").Replace("&", "+");
                System.Diagnostics.Process.Start(url);
            }
            else
            {
                System.Diagnostics.Process.Start(_selectedCraft.Airwar);
            }
        }

        private void bCraftWikiLink_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCraft.Wiki))
            {
                var url =
                  "http://www.google.ru/search?q=" + 
                  $"{_selectedCraft.Construct} {_selectedCraft.Name} site%3Awikipedia.org"
                    .Replace(" ", "+").Replace("/", "+").Replace("&", "+");
                System.Diagnostics.Process.Start(url);
            }
            else
            {
                System.Diagnostics.Process.Start(_selectedCraft.Wiki);
            }
        }

        private void bCraftFlyingLink_Click(object sender, EventArgs e)
        {
            SelectCraft(_selectedCraft.FlyingM.Value);
        }


        private void lstCraftSeeAlso_DoubleClick(object sender, EventArgs e)
        {
            var item = (Pair<int>)lstCraftSeeAlso.SelectedItem;
            if (item != null)
            {
                SelectCraft(item.Id);
            }
        }

        private void nextCraftInMagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && !chFilterCraftsByMag.Checked && _selectedArt != null && _craftPosition != Position.Empty)
            {
                if (_filteredArtId != _selectedArt.ArtId) LoadFilteredMags();

                var r = _craftPosition.Row + 1;
                while (r < gridCraft.RowsCount)
                {
                    if (_filteredMagCrafts.Contains((int)gridCraft[r, Const.Columns.Craft.CraftId].Value))
                    {
                        var focusPosn = new Position(r, _craftPosition.Column);
                        gridCraft.Selection.Focus(focusPosn, true);
                        _craftPosition = focusPosn;
                        break;
                    }
                    r++;
                }
            }
        }

        private void priorCraftInMagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && !chFilterCraftsByMag.Checked && _selectedArt != null && _craftPosition != Position.Empty)
            {
                if (_filteredArtId != _selectedArt.ArtId) LoadFilteredMags();

                var r = _craftPosition.Row - 1;
                while (r > 0)
                {
                    if (_filteredMagCrafts.Contains((int)gridCraft[r, Const.Columns.Craft.CraftId].Value))
                    {
                        var focusPosn = new Position(r, _craftPosition.Column);
                        gridCraft.Selection.Focus(focusPosn, true);
                        _craftPosition = focusPosn;
                        break;
                    }
                    r--;
                }
            }
        }

        private void LoadFilteredMags()
        {
            _filteredMags = _ctx.Arts
                .Where(x => x.Mag == _selectedArt.Mag && x.IYear == _selectedArt.IYear && x.IMonth == _selectedArt.IMonth)
                .Select(x => x.ArtId)
                .ToList();

            _filteredMagCrafts = _ctx.vwPics.AsNoTracking().Where(x => _filteredMags.Contains(x.ArtId)).Select(x => x.CraftId).Distinct().ToList();

            _filteredArtId = _selectedArt.ArtId;
        }

        private void chCraftSortConstruct_Click(object sender, EventArgs e)
        {
            LoadCrafts();
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fltDlg = new fFilter();
            var countries = _ctx.Crafts.Select(x => (x.Country ?? "")).Distinct().OrderBy(x => x).ToArray();
            fltDlg.ClCountries.Items.AddRange(countries);
            for (int i = 0; i < fltDlg.ClCountries.Items.Count; i++) 
            {
                fltDlg.ClCountries.SetItemChecked(i, true);
            }
            for (int i = 0; i < fltDlg.ClSources.Items.Count; i++)
            {
                fltDlg.ClSources.SetItemChecked(i, Const.Sources.Writeable.Contains(fltDlg.ClSources.Items[i].ToString().Substring(0, 1)));
            }

            if (_filter != null)
            {
                for (int i = 0; i < fltDlg.ClCountries.Items.Count; i++) 
                {
                    if (_filter.CountriesNo.Contains(fltDlg.ClCountries.Items[i]))
                    {
                        fltDlg.ClCountries.SetItemChecked(i, false);
                    }
                }
                for (int i = 0; i < fltDlg.ClSources.Items.Count; i++)
                {
                    fltDlg.ClSources.SetItemChecked(i, !_filter.SourcesNo.Contains(fltDlg.ClSources.Items[i].ToString().Substring(0, 1)));
                }
                fltDlg.CVertYes.Checked = _filter.VertYes;
                fltDlg.CUavYes.Checked = _filter.UavYes;
                fltDlg.CGlidYes.Checked = _filter.GlidYes;
                fltDlg.CLlYes.Checked = _filter.LlYes;
                fltDlg.CSinglYes.Checked = _filter.SinglYes;
                fltDlg.CProjYes.Checked = _filter.ProjYes;
                fltDlg.CVertNo.Checked = _filter.VertNo;
                fltDlg.CUavNo.Checked = _filter.UavNo;
                fltDlg.CGlidNo.Checked = _filter.GlidNo;
                fltDlg.CLlNo.Checked = _filter.LlNo;
                fltDlg.CSinglNo.Checked = _filter.SinglNo;
                fltDlg.CProjNo.Checked = _filter.ProjNo;

                fltDlg.EWings.Text = _filter.Wings;
                fltDlg.EEngines.Text = _filter.Engines;
                fltDlg.EText.Text = _filter.Text;
                fltDlg.EText2.Text = _filter.Text2;

                fltDlg.CWholeWords.Checked = _filter.WholeWords;
                fltDlg.CCaseSensitive.Checked = _filter.CaseSensitive;
                fltDlg.CInText.Checked = _filter.InText;

                fltDlg.NYearFrom.Value = _filter.YearFrom;
                fltDlg.NYearTo.Value = _filter.YearTo;
            }

            if (fltDlg.ShowDialog() == DialogResult.OK)
            {
                if (_filter == null) _filter = new FilterDto();
                _filter.CountriesYes = new List<string>();
                _filter.CountriesNo = new List<string>();
                _filter.SourcesNo = new List<string>();
                for (int i = 0; i < fltDlg.ClCountries.Items.Count; i++) 
                {
                    if (fltDlg.ClCountries.GetItemChecked(i))
                    {
                        _filter.CountriesYes.Add(fltDlg.ClCountries.Items[i].ToString());
                    }
                    else
                    {
                        _filter.CountriesNo.Add(fltDlg.ClCountries.Items[i].ToString());
                    }
                }
                for (int i = 0; i < fltDlg.ClSources.Items.Count; i++)
                {
                    if (!fltDlg.ClSources.GetItemChecked(i))
                    {
                        _filter.SourcesNo.Add(fltDlg.ClSources.Items[i].ToString().Substring(0, 1));
                    }
                }

                _filter.VertYes = fltDlg.CVertYes.Checked;
                _filter.UavYes = fltDlg.CUavYes.Checked;
                _filter.GlidYes = fltDlg.CGlidYes.Checked;
                _filter.LlYes = fltDlg.CLlYes.Checked;
                _filter.SinglYes = fltDlg.CSinglYes.Checked;
                _filter.ProjYes = fltDlg.CProjYes.Checked;
                _filter.VertNo = fltDlg.CVertNo.Checked;
                _filter.UavNo = fltDlg.CUavNo.Checked;
                _filter.GlidNo = fltDlg.CGlidNo.Checked;
                _filter.LlNo = fltDlg.CLlNo.Checked;
                _filter.SinglNo = fltDlg.CSinglNo.Checked;
                _filter.ProjNo = fltDlg.CProjNo.Checked;

                _filter.Wings = fltDlg.EWings.Text;
                _filter.Engines = fltDlg.EEngines.Text;
                _filter.Text = fltDlg.EText.Text;
                _filter.Text2 = fltDlg.EText2.Text;

                _filter.WholeWords = fltDlg.CWholeWords.Checked;
                _filter.CaseSensitive = fltDlg.CCaseSensitive.Checked;
                _filter.InText = fltDlg.CInText.Checked;

                _filter.YearFrom = (int)fltDlg.NYearFrom.Value;
                _filter.YearTo = (int)fltDlg.NYearTo.Value;

                _filterOn = true;
                LoadCrafts();
            }
        }

        private void filterOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _filterOn = false;
            LoadCrafts();
        }
    }
}
