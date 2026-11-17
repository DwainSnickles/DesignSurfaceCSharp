using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace DesignSurface
{
    /// <summary>
    /// Imports label/template layouts from Excel OpenXML files (.xlsx/.xlsm).
    /// Column widths and row heights determine cell geometry; a uniform grid
    /// becomes a LabelSheet, anything else falls back to cell outlines.
    /// </summary>
    public static class SpreadsheetTemplateImporter
    {
        private const string SpreadsheetNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        private const string OfficeRelationshipsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        private const string PackageRelationshipsNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";

        /// <summary>Excel column width unit approximates to this many pixels (Calibri 11 MDW).</summary>
        private const double ColumnWidthPixelsPerUnit = 7.0;

        /// <summary>1pt = 96/72 px at 96dpi.</summary>
        private const double PointsToPixels = 96.0 / 72.0;

        /// <summary>Converts 96-dpi pixels to surface units (1/100 inch).</summary>
        private static int PixelsToUnits(double pixels)
        {
            return (int)Math.Round(pixels * 100.0 / 96.0);
        }

        /// <summary>Excel column width (chars) to surface units, incl. cell padding.</summary>
        private static int ColWidthToUnits(double widthChars)
        {
            return PixelsToUnits(widthChars * ColumnWidthPixelsPerUnit + 5.0);
        }

        /// <summary>Row height (points) to surface units.</summary>
        private static int RowHeightToUnits(double heightPoints)
        {
            return PixelsToUnits(heightPoints * PointsToPixels);
        }

        public static OfficeTemplateResult Import(string path)
        {
            var result = new OfficeTemplateResult { SourceName = Path.GetFileNameWithoutExtension(path) };

            List<string> sharedStrings;
            List<CellInfo> cells;
            List<MergeInfo> merges;
            Dictionary<int, double> colWidths;   // 1-based column -> width chars
            Dictionary<int, double> rowHeights;  // 1-based row -> height points
            double defaultColWidth, defaultRowHeight;

            try
            {
                using (var zip = ZipFile.OpenRead(path))
                {
                    sharedStrings = ReadSharedStrings(zip);
                    var sheetPath = ResolveFirstSheetPath(zip);
                    if (sheetPath == null)
                    {
                        return null;
                    }

                    var entry = zip.GetEntry(sheetPath);
                    if (entry == null)
                    {
                        return null;
                    }

                    var doc = new XmlDocument();
                    using (var stream = entry.Open())
                    {
                        doc.Load(stream);
                    }

                    var nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("s", SpreadsheetNamespace);

                    var sheetFormat = doc.SelectSingleNode("//s:worksheet/s:sheetFormatPr", nsmgr) as XmlElement;
                    defaultColWidth = GetDoubleAttr(sheetFormat, "defaultColWidth", 8.43);
                    defaultRowHeight = GetDoubleAttr(sheetFormat, "defaultRowHeight", 15.0);

                    colWidths = ReadColumnWidths(doc, nsmgr, defaultColWidth);
                    rowHeights = ReadRowHeights(doc, nsmgr, defaultRowHeight);
                    merges = ReadMerges(doc, nsmgr);
                    cells = new List<CellInfo>();
                    ReadCells(doc, nsmgr, sharedStrings, cells);
                }
            }
            catch
            {
                return null;
            }

            if (cells.Count == 0)
            {
                return result;
            }

            // Absolute x/y origins (surface units) for every used column/row.
            var colX = new Dictionary<int, int>();
            var rowY = new Dictionary<int, int>();
            {
                var x = 0;
                var maxCol = 0;
                foreach (var cell in cells)
                {
                    maxCol = Math.Max(maxCol, cell.Col);
                }
                for (var c = 1; c <= maxCol; c++)
                {
                    colX[c] = x;
                    x += ColWidthToUnits(GetOrDefault(colWidths, c, defaultColWidth));
                }

                var y = 0;
                var maxRow = 0;
                foreach (var cell in cells)
                {
                    maxRow = Math.Max(maxRow, cell.Row);
                }
                for (var r = 1; r <= maxRow; r++)
                {
                    rowY[r] = y;
                    y += RowHeightToUnits(GetOrDefault(rowHeights, r, defaultRowHeight));
                }
            }

            // Used range in surface units.
            var minCol = int.MaxValue;
            var maxColUsed = int.MinValue;
            var minRow = int.MaxValue;
            var maxRowUsed = int.MinValue;
            foreach (var cell in cells)
            {
                minCol = Math.Min(minCol, cell.Col);
                maxColUsed = Math.Max(maxColUsed, cell.Col);
                minRow = Math.Min(minRow, cell.Row);
                maxRowUsed = Math.Max(maxRowUsed, cell.Row);
            }

            var rangeRight = colX[maxColUsed] + ColWidthToUnits(GetOrDefault(colWidths, maxColUsed, defaultColWidth));
            var rangeBottom = rowY[maxRowUsed] + RowHeightToUnits(GetOrDefault(rowHeights, maxRowUsed, defaultRowHeight));

            var rangeWidth = rangeRight;
            var rangeHeight = rangeBottom;
            result.Landscape = rangeWidth > rangeHeight;

            // Portrait-normalized page size (matches OfficeTemplateImporter),
            // at least US Letter so small grids still get a full page.
            var portraitWidth = result.Landscape ? rangeHeight : rangeWidth;
            var portraitHeight = result.Landscape ? rangeWidth : rangeHeight;
            result.PageSize = new Size(Math.Max(portraitWidth, 850), Math.Max(portraitHeight, 1100));

            // Fallback outlines: merged regions once, then each non-empty cell.
            var covered = new HashSet<string>();
            foreach (var merge in merges)
            {
                var x = colX[merge.Col];
                var y = rowY[merge.Row];
                var w = 0;
                var h = 0;
                for (var c = merge.Col; c < merge.Col + merge.ColSpan; c++)
                {
                    w += ColWidthToUnits(GetOrDefault(colWidths, c, defaultColWidth));
                }
                for (var r = merge.Row; r < merge.Row + merge.RowSpan; r++)
                {
                    h += RowHeightToUnits(GetOrDefault(rowHeights, r, defaultRowHeight));
                }
                result.Cells.Add(new Rectangle(x, y, w, h));
                for (var c = merge.Col; c < merge.Col + merge.ColSpan; c++)
                {
                    for (var r = merge.Row; r < merge.Row + merge.RowSpan; r++)
                    {
                        covered.Add(c + ":" + r);
                    }
                }
            }

            foreach (var cell in cells)
            {
                if (!cell.HasContent || covered.Contains(cell.Col + ":" + cell.Row))
                {
                    continue;
                }

                result.Cells.Add(new Rectangle(
                    colX[cell.Col],
                    rowY[cell.Row],
                    ColWidthToUnits(GetOrDefault(colWidths, cell.Col, defaultColWidth)),
                    RowHeightToUnits(GetOrDefault(rowHeights, cell.Row, defaultRowHeight))));
            }

            if (merges.Count > 0)
            {
                return result; // merged layouts can't be a uniform label grid
            }

            TryBuildUniformSheet(cells, colX, rowY, colWidths, rowHeights, defaultColWidth, defaultRowHeight, result);
            return result;
        }

        private static void TryBuildUniformSheet(List<CellInfo> cells, Dictionary<int, int> colX, Dictionary<int, int> rowY,
            Dictionary<int, double> colWidths, Dictionary<int, double> rowHeights,
            double defaultColWidth, double defaultRowHeight, OfficeTemplateResult result)
        {
            // Columns/rows that contain at least one non-empty cell.
            var labelCols = new SortedSet<int>();
            var labelRows = new SortedSet<int>();
            foreach (var cell in cells)
            {
                if (cell.HasContent)
                {
                    labelCols.Add(cell.Col);
                    labelRows.Add(cell.Row);
                }
            }

            if (labelCols.Count == 0 || labelRows.Count == 0)
            {
                return;
            }

            // All label columns must share ~the same width, all label rows ~the same height.
            var labelW = ColWidthToUnits(GetOrDefault(colWidths, labelCols.Min, defaultColWidth));
            var labelH = RowHeightToUnits(GetOrDefault(rowHeights, labelRows.Min, defaultRowHeight));
            foreach (var c in labelCols)
            {
                if (Math.Abs(ColWidthToUnits(GetOrDefault(colWidths, c, defaultColWidth)) - labelW) > 3)
                {
                    return;
                }
            }
            foreach (var r in labelRows)
            {
                if (Math.Abs(RowHeightToUnits(GetOrDefault(rowHeights, r, defaultRowHeight)) - labelH) > 3)
                {
                    return;
                }
            }

            var cols = new List<int>(labelCols);
            var rows = new List<int>(labelRows);
            var pitchX = cols.Count > 1 ? colX[cols[1]] - colX[cols[0]] : labelW;
            var pitchY = rows.Count > 1 ? rowY[rows[1]] - rowY[rows[0]] : labelH;

            result.Sheet = new LabelSheet(
                result.SourceName,
                result.PageSize.Width,
                result.PageSize.Height,
                cols.Count,
                rows.Count,
                labelW,
                labelH,
                colX[cols[0]],
                rowY[rows[0]],
                pitchX,
                pitchY);
        }

        private static double GetOrDefault(Dictionary<int, double> map, int key, double fallback)
        {
            double value;
            return map != null && map.TryGetValue(key, out value) ? value : fallback;
        }

        private static double GetDoubleAttr(XmlElement element, string name, double fallback)
        {
            double value;
            return element != null && double.TryParse(element.GetAttribute(name), out value) && value > 0 ? value : fallback;
        }

        private class CellInfo
        {
            public int Row;   // 1-based
            public int Col;   // 1-based
            public bool HasContent;
        }

        private class MergeInfo
        {
            public int Row;
            public int Col;
            public int RowSpan;
            public int ColSpan;
        }

        private static List<string> ReadSharedStrings(ZipArchive zip)
        {
            var strings = new List<string>();
            var entry = zip.GetEntry("xl/sharedStrings.xml");
            if (entry == null)
            {
                return strings;
            }

            var doc = new XmlDocument();
            using (var stream = entry.Open())
            {
                doc.Load(stream);
            }

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", SpreadsheetNamespace);
            var items = doc.SelectNodes("//s:sst/s:si", nsmgr);
            if (items == null)
            {
                return strings;
            }

            foreach (XmlElement si in items)
            {
                strings.Add(si.InnerText ?? string.Empty);
            }

            return strings;
        }

        private static string ResolveFirstSheetPath(ZipArchive zip)
        {
            var workbookEntry = zip.GetEntry("xl/workbook.xml");
            var relsEntry = zip.GetEntry("xl/_rels/workbook.xml.rels");
            if (workbookEntry == null || relsEntry == null)
            {
                return null;
            }

            string relId = null;
            var wbDoc = new XmlDocument();
            using (var stream = workbookEntry.Open())
            {
                wbDoc.Load(stream);
            }

            var wbNsmgr = new XmlNamespaceManager(wbDoc.NameTable);
            wbNsmgr.AddNamespace("s", SpreadsheetNamespace);
            wbNsmgr.AddNamespace("r", OfficeRelationshipsNamespace);
            var firstSheet = wbDoc.SelectSingleNode("//s:sheets/s:sheet", wbNsmgr) as XmlElement;
            if (firstSheet != null)
            {
                relId = firstSheet.GetAttribute("id", OfficeRelationshipsNamespace);
            }

            var relsDoc = new XmlDocument();
            using (var stream = relsEntry.Open())
            {
                relsDoc.Load(stream);
            }

            var relsNsmgr = new XmlNamespaceManager(relsDoc.NameTable);
            relsNsmgr.AddNamespace("pr", PackageRelationshipsNamespace);
            XmlNode target = null;
            if (relId != null)
            {
                target = relsDoc.SelectSingleNode("//pr:Relationship[@Id='" + relId + "']", relsNsmgr);
            }
            if (target == null)
            {
                target = relsDoc.SelectSingleNode("//pr:Relationship", relsNsmgr);
            }

            if (target == null || target.Attributes["Target"] == null)
            {
                return null;
            }

            var targetPath = target.Attributes["Target"].Value;
            if (string.IsNullOrEmpty(targetPath))
            {
                return null;
            }

            return targetPath.StartsWith("/")
                ? targetPath.TrimStart('/')
                : "xl/" + targetPath.TrimStart('/');
        }

        private static Dictionary<int, double> ReadColumnWidths(XmlDocument doc, XmlNamespaceManager nsmgr, double defaultWidth)
        {
            var widths = new Dictionary<int, double>();
            var cols = doc.SelectNodes("//s:worksheet/s:cols/s:col", nsmgr);
            if (cols == null)
            {
                return widths;
            }

            foreach (XmlElement col in cols)
            {
                int min, max;
                double width;
                if (!int.TryParse(col.GetAttribute("min"), out min) ||
                    !int.TryParse(col.GetAttribute("max"), out max) ||
                    !double.TryParse(col.GetAttribute("width"), out width) || width <= 0)
                {
                    continue;
                }
                for (var i = min; i <= max; i++)
                {
                    widths[i] = width;
                }
            }

            return widths;
        }

        private static Dictionary<int, double> ReadRowHeights(XmlDocument doc, XmlNamespaceManager nsmgr, double defaultHeight)
        {
            var heights = new Dictionary<int, double>();
            var rows = doc.SelectNodes("//s:worksheet/s:sheetData/s:row", nsmgr);
            if (rows == null)
            {
                return heights;
            }

            foreach (XmlElement row in rows)
            {
                int rowIndex;
                double height;
                if (!int.TryParse(row.GetAttribute("r"), out rowIndex))
                {
                    continue;
                }
                if (!double.TryParse(row.GetAttribute("ht"), out height) || height <= 0)
                {
                    height = defaultHeight;
                }
                heights[rowIndex] = height;
            }

            return heights;
        }

        private static List<MergeInfo> ReadMerges(XmlDocument doc, XmlNamespaceManager nsmgr)
        {
            var merges = new List<MergeInfo>();
            var nodes = doc.SelectNodes("//s:worksheet/s:mergeCells/s:mergeCell", nsmgr);
            if (nodes == null)
            {
                return merges;
            }

            foreach (XmlElement node in nodes)
            {
                var merge = ParseCellRange(node.GetAttribute("ref"));
                if (merge != null)
                {
                    merges.Add(merge);
                }
            }

            return merges;
        }

        private static MergeInfo ParseCellRange(string reference)
        {
            // Formats: "A1" or "A1:C3"
            var parts = reference.Split(':');
            var start = ParseCellRef(parts[0]);
            var end = parts.Length > 1 ? ParseCellRef(parts[1]) : start;
            if (start == null || end == null)
            {
                return null;
            }

            return new MergeInfo
            {
                Row = start.Value.Row,
                Col = start.Value.Col,
                RowSpan = end.Value.Row - start.Value.Row + 1,
                ColSpan = end.Value.Col - start.Value.Col + 1
            };
        }

        private static void ReadCells(XmlDocument doc, XmlNamespaceManager nsmgr, List<string> sharedStrings, List<CellInfo> cells)
        {
            var nodes = doc.SelectNodes("//s:worksheet/s:sheetData//s:c", nsmgr);
            if (nodes == null)
            {
                return;
            }

            var cellNsmgr = new XmlNamespaceManager(doc.NameTable);
            cellNsmgr.AddNamespace("s", SpreadsheetNamespace);

            foreach (XmlElement cell in nodes)
            {
                var pos = ParseCellRef(cell.GetAttribute("r"));
                if (pos == null)
                {
                    continue;
                }

                cells.Add(new CellInfo
                {
                    Row = pos.Value.Row,
                    Col = pos.Value.Col,
                    HasContent = CellHasContent(cell, sharedStrings, cellNsmgr)
                });
            }
        }

        private static bool CellHasContent(XmlElement cell, List<string> sharedStrings, XmlNamespaceManager nsmgr)
        {
            var type = cell.GetAttribute("t");
            var valueNode = cell.SelectSingleNode("s:v", nsmgr) as XmlElement;

            if (string.Equals(type, "inlineStr", StringComparison.Ordinal))
            {
                var text = cell.InnerText;
                return !string.IsNullOrWhiteSpace(text);
            }

            if (valueNode == null || string.IsNullOrWhiteSpace(valueNode.InnerText))
            {
                return false;
            }

            if (string.Equals(type, "s", StringComparison.Ordinal))
            {
                int index;
                if (int.TryParse(valueNode.InnerText, out index) && index >= 0 && index < sharedStrings.Count)
                {
                    return !string.IsNullOrWhiteSpace(sharedStrings[index]);
                }
                return false;
            }

            return true;
        }

        private struct CellPos
        {
            public int Row;
            public int Col;
        }

        /// <summary>Parses an A1-style cell reference into 1-based row/col.</summary>
        private static CellPos? ParseCellRef(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return null;
            }

            var col = 0;
            var i = 0;
            while (i < reference.Length && char.IsLetter(reference[i]))
            {
                col = col * 26 + (char.ToUpperInvariant(reference[i]) - 'A' + 1);
                i++;
            }

            int row;
            if (i >= reference.Length || !int.TryParse(reference.Substring(i), out row) || row <= 0 || col <= 0)
            {
                return null;
            }

            return new CellPos { Row = row, Col = col };
        }
    }
}
