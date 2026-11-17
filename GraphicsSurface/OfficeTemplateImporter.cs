using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace DesignSurface
{
    /// <summary>
    /// Result of importing an Office/OpenXML template (.docx/.dotx).
    /// </summary>
    public class OfficeTemplateResult
    {
        /// <summary>Uniform label grid, when the table is a regular grid.</summary>
        public LabelSheet Sheet;

        /// <summary>Label cell rectangles in 1/100 inch surface units (non-uniform fallback).</summary>
        public List<Rectangle> Cells = new List<Rectangle>();

        /// <summary>Sheet size in 1/100 inch (portrait orientation).</summary>
        public Size PageSize;

        /// <summary>True when the source document is landscape.</summary>
        public bool Landscape;

        /// <summary>Page margins in twips (1/1440 inch).</summary>
        public double MarginLeftTwips;

        public double MarginTopTwips;

        /// <summary>Source file name, used to name imported sheets.</summary>
        public string SourceName;

        public bool IsUniform
        {
            get { return Sheet != null; }
        }
    }

    /// <summary>
    /// Imports label templates from WordprocessingML files (.docx/.dotx).
    /// These are ZIP archives containing word/document.xml; tables in the
    /// document describe the label grid.
    /// </summary>
    public static class OfficeTemplateImporter
    {
        private const string WordprocessingNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        private const double TwipsPerInch = 1440.0;

        /// <summary>Converts twips (1/1440 inch) to surface units (1/100 inch).</summary>
        private static int TwipsToUnits(double twips)
        {
            return (int)Math.Round(twips * 100.0 / TwipsPerInch);
        }

        public static OfficeTemplateResult Import(string path)
        {
            XmlDocument doc;
            try
            {
                using (var zip = ZipFile.OpenRead(path))
                {
                    var entry = zip.GetEntry("word/document.xml");
                    if (entry == null)
                    {
                        return null;
                    }

                    doc = new XmlDocument();
                    using (var stream = entry.Open())
                    {
                        doc.Load(stream);
                    }
                }
            }
            catch
            {
                return null;
            }

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("w", WordprocessingNamespace);

            var result = new OfficeTemplateResult { SourceName = Path.GetFileNameWithoutExtension(path) };
            ReadPageSetup(doc, nsmgr, result);

            // Pick the table with the most direct cells. Documents often wrap
            // their real layout (e.g. an invoice grid) in nested tables, so
            // nested tables are eligible and win on cell count.
            var tables = doc.SelectNodes("//w:tbl", nsmgr);
            XmlElement table = null;
            var bestScore = -1;
            if (tables != null)
            {
                foreach (XmlElement candidate in tables)
                {
                    // w:tr/w:tc counts only this table's own rows/cells —
                    // cells inside nested tables belong to the nested tbl.
                    var score = candidate.SelectNodes("w:tr/w:tc", nsmgr)?.Count ?? 0;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        table = candidate;
                    }
                }
            }

            if (table == null)
            {
                return null;
            }

            double originOffsetX, originOffsetY;
            GetAncestorOffsetTwips(table, nsmgr, out originOffsetX, out originOffsetY);
            ReadTable(table, nsmgr, result, originOffsetX, originOffsetY);
            return result;
        }

        /// <summary>
        /// Computes the position of a (possibly nested) table within the page
        /// by walking up through ancestor table cells, accumulating each
        /// parent cell's offset. Returns twips relative to the page margin.
        /// </summary>
        private static void GetAncestorOffsetTwips(XmlNode node, XmlNamespaceManager nsmgr, out double offsetX, out double offsetY)
        {
            offsetX = 0;
            offsetY = 0;

            var current = node.ParentNode;
            while (current != null)
            {
                var tc = current as XmlElement;
                if (tc == null || tc.LocalName != "tc" || tc.NamespaceURI != WordprocessingNamespace)
                {
                    current = current.ParentNode;
                    continue;
                }

                // The row and table that contain this cell.
                var tr = tc.ParentNode as XmlElement;
                while (tr != null && (tr.LocalName != "tr" || tr.NamespaceURI != WordprocessingNamespace))
                {
                    tr = tr.ParentNode as XmlElement;
                }
                if (tr == null || tr.ParentNode == null)
                {
                    break;
                }

                var parentTbl = tr.ParentNode as XmlElement;
                while (parentTbl != null && (parentTbl.LocalName != "tbl" || parentTbl.NamespaceURI != WordprocessingNamespace))
                {
                    parentTbl = parentTbl.ParentNode as XmlElement;
                }
                if (parentTbl == null || tr.ParentNode != parentTbl)
                {
                    break; // unexpected wrapping; give up on precise offset
                }

                // Y: heights of the rows above our row in the parent table.
                foreach (XmlElement siblingRow in parentTbl.SelectNodes("w:tr", nsmgr))
                {
                    if (ReferenceEquals(siblingRow, tr))
                    {
                        break;
                    }
                    var rowHeight = siblingRow.SelectSingleNode("w:trPr/w:trHeight", nsmgr) as XmlElement;
                    var h = GetTwipsAttribute(rowHeight, "val");
                    offsetY += h > 0 ? h : 240;
                }

                // X: widths of the cells to the left of ours in the same row.
                var gridCols = parentTbl.SelectNodes("w:tblGrid/w:gridCol", nsmgr);
                var gridWidths = new List<double>();
                if (gridCols != null)
                {
                    foreach (XmlElement gc in gridCols)
                    {
                        gridWidths.Add(GetTwipsAttribute(gc, "w"));
                    }
                }

                var gridPos = 0;
                foreach (XmlElement siblingCell in tr.SelectNodes("w:tc", nsmgr))
                {
                    if (ReferenceEquals(siblingCell, tc))
                    {
                        break;
                    }

                    var span = 1;
                    var tcPr = siblingCell.SelectSingleNode("w:tcPr", nsmgr) as XmlElement;
                    var spanEl = tcPr != null ? tcPr.SelectSingleNode("w:gridSpan", nsmgr) as XmlElement : null;
                    if (spanEl != null)
                    {
                        int.TryParse(GetAttr(spanEl, "val"), out span);
                        if (span < 1) span = 1;
                    }

                    var cellW = 0.0;
                    var tcW = tcPr != null ? tcPr.SelectSingleNode("w:tcW", nsmgr) as XmlElement : null;
                    if (tcW != null && GetAttr(tcW, "type") != "pct")
                    {
                        cellW = GetTwipsAttribute(tcW, "w");
                    }
                    if (cellW <= 0)
                    {
                        for (var g = 0; g < span && gridPos + g < gridWidths.Count; g++)
                        {
                            cellW += gridWidths[gridPos + g];
                        }
                    }
                    offsetX += cellW;
                    gridPos += span;
                }

                // The parent table's own indent shifts its whole grid.
                var parentPr = parentTbl.SelectSingleNode("w:tblPr", nsmgr) as XmlElement;
                var parentInd = parentPr != null ? parentPr.SelectSingleNode("w:tblInd", nsmgr) as XmlElement : null;
                offsetX += GetTwipsAttribute(parentInd, "w");

                current = parentTbl;
            }
        }

        private static void ReadPageSetup(XmlDocument doc, XmlNamespaceManager nsmgr, OfficeTemplateResult result)
        {
            // Last sectPr in the document holds the final section's page setup.
            var sectPrs = doc.SelectNodes("//w:sectPr", nsmgr);
            if (sectPrs == null || sectPrs.Count == 0)
            {
                result.PageSize = new Size(850, 1100);
                return;
            }

            var sectPr = sectPrs[sectPrs.Count - 1];
            var pgSz = sectPr.SelectSingleNode("w:pgSz", nsmgr) as XmlElement;
            var pgMar = sectPr.SelectSingleNode("w:pgMar", nsmgr) as XmlElement;
            if (pgMar != null)
            {
                result.MarginLeftTwips = GetTwipsAttribute(pgMar, "left");
                if (result.MarginLeftTwips <= 0)
                {
                    result.MarginLeftTwips = GetTwipsAttribute(pgMar, "start");
                }
                result.MarginTopTwips = GetTwipsAttribute(pgMar, "top");
            }

            double pageW = 12240; // 8.5in default
            double pageH = 15840; // 11in default
            if (pgSz != null)
            {
                double w, h;
                if (double.TryParse(GetAttr(pgSz, "w"), out w) && w > 0)
                {
                    pageW = w;
                }
                if (double.TryParse(GetAttr(pgSz, "h"), out h) && h > 0)
                {
                    pageH = h;
                }
                result.Landscape = string.Equals(GetAttr(pgSz, "orient"), "landscape", StringComparison.OrdinalIgnoreCase);
            }

            // Normalize to portrait storage; Landscape flag reports the source orientation.
            result.PageSize = result.Landscape
                ? new Size(TwipsToUnits(pageH), TwipsToUnits(pageW))
                : new Size(TwipsToUnits(pageW), TwipsToUnits(pageH));
        }

        /// <summary>
        /// Reads a WordprocessingML attribute. Attributes like w:w and w:val
        /// are namespace-qualified, so the plain GetAttribute(name) overload
        /// returns empty — check the namespace first, then fall back.
        /// </summary>
        private static string GetAttr(XmlElement element, string name)
        {
            if (element == null)
            {
                return string.Empty;
            }
            var value = element.GetAttribute(name, WordprocessingNamespace);
            return string.IsNullOrEmpty(value) ? element.GetAttribute(name) : value;
        }

        private static double GetTwipsAttribute(XmlElement element, string name)
        {
            double value;
            return element != null && double.TryParse(GetAttr(element, name), out value) ? value : 0;
        }

        private static bool CellHasContent(XmlElement tc, XmlNamespaceManager nsmgr)
        {
            var texts = tc.SelectNodes(".//w:t", nsmgr);
            if (texts == null)
            {
                return false;
            }

            foreach (XmlNode node in texts)
            {
                if (!string.IsNullOrWhiteSpace(node.InnerText))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ReadTable(XmlElement table, XmlNamespaceManager nsmgr, OfficeTemplateResult result, double originOffsetX = 0, double originOffsetY = 0)
        {
            var tblPr = table.SelectSingleNode("w:tblPr", nsmgr) as XmlElement;
            double tableIndent = 0;
            double cellSpacing = 0;
            if (tblPr != null)
            {
                var tblInd = tblPr.SelectSingleNode("w:tblInd", nsmgr) as XmlElement;
                tableIndent = GetTwipsAttribute(tblInd, "w");
                var spacing = tblPr.SelectSingleNode("w:tblCellSpacing", nsmgr) as XmlElement;
                cellSpacing = GetTwipsAttribute(spacing, "w");
            }

            // Column widths from tblGrid when available.
            var gridCols = table.SelectNodes("w:tblGrid/w:gridCol", nsmgr);
            var colWidths = new List<double>();
            if (gridCols != null)
            {
                foreach (XmlElement col in gridCols)
                {
                    var w = GetTwipsAttribute(col, "w");
                    if (w <= 0)
                    {
                        colWidths = new List<double>();
                        break;
                    }
                    colWidths.Add(w);
                }
            }

            var rows = table.SelectNodes("w:tr", nsmgr);
            if (rows == null || rows.Count == 0)
            {
                return;
            }

            // Table origin = page margins + optional table indent.
            var marginLeftTwips = result.MarginLeftTwips;
            var marginTopTwips = result.MarginTopTwips;

            var cellRecords = new List<CellRecord>();
            var rowHeights = new List<double>();
            var complex = false;

            foreach (XmlElement tr in rows)
            {
                var trHeight = tr.SelectSingleNode("w:trPr/w:trHeight", nsmgr) as XmlElement;
                var rowH = GetTwipsAttribute(trHeight, "val");
                rowHeights.Add(rowH);

                var cells = tr.SelectNodes("w:tc", nsmgr);
                if (cells == null)
                {
                    continue;
                }

                int gridCol = 0;
                foreach (XmlElement tc in cells)
                {
                    var tcPr = tc.SelectSingleNode("w:tcPr", nsmgr) as XmlElement;
                    var tcW = tcPr != null ? tcPr.SelectSingleNode("w:tcW", nsmgr) as XmlElement : null;
                    var wType = tcW != null ? GetAttr(tcW, "type") : "";
                    var cellW = GetTwipsAttribute(tcW, "w");
                    var gridSpan = 1;
                    var vMerge = false;
                    if (tcPr != null)
                    {
                        var span = tcPr.SelectSingleNode("w:gridSpan", nsmgr) as XmlElement;
                        if (span != null)
                        {
                            int.TryParse(GetAttr(span, "val"), out gridSpan);
                        }
                        vMerge = tcPr.SelectSingleNode("w:vMerge", nsmgr) != null;
                    }

                    if (gridSpan > 1 || vMerge || wType == "pct" || cellW <= 0)
                    {
                        complex = true;
                    }

                    cellRecords.Add(new CellRecord
                    {
                        Row = rowHeights.Count - 1,
                        Col = gridCol,
                        Width = cellW,
                        Height = rowH,
                        HasContent = CellHasContent(tc, nsmgr)
                    });

                    gridCol += Math.Max(1, gridSpan);
                }
            }

            if (cellRecords.Count == 0)
            {
                return;
            }

            // Column count = max cells in any row.
            var columnCount = 0;
            foreach (var rec in cellRecords)
            {
                columnCount = Math.Max(columnCount, rec.Col + 1);
            }

            // Fall back to first-row cell widths when tblGrid is unusable.
            if (colWidths.Count == 0)
            {
                foreach (var rec in cellRecords)
                {
                    if (rec.Row == 0 && rec.Width > 0)
                    {
                        while (colWidths.Count <= rec.Col)
                        {
                            colWidths.Add(rec.Width);
                        }
                    }
                }
                while (colWidths.Count < columnCount)
                {
                    colWidths.Add(colWidths.Count > 0 ? colWidths[colWidths.Count - 1] : 240);
                }
            }

            // Row heights: fill missing values with the max known cell height.
            var maxRowH = 240.0;
            foreach (var h in rowHeights)
            {
                maxRowH = Math.Max(maxRowH, h);
            }
            for (var i = 0; i < rowHeights.Count; i++)
            {
                if (rowHeights[i] <= 0)
                {
                    rowHeights[i] = maxRowH;
                }
            }

            // Left edge of each column (twips), including cell spacing.
            var colLeft = new double[columnCount];
            var x = marginLeftTwips + tableIndent + originOffsetX;
            for (var c = 0; c < columnCount; c++)
            {
                colLeft[c] = x;
                x += (c < colWidths.Count ? colWidths[c] : 240) + cellSpacing;
            }

            var rowTop = new double[rowHeights.Count];
            var y = marginTopTwips + originOffsetY;
            for (var r = 0; r < rowHeights.Count; r++)
            {
                rowTop[r] = y;
                y += rowHeights[r] + cellSpacing;
            }

            // A cell is a "label" when it has content; when the whole table is
            // blank every cell counts as a label.
            var anyContent = false;
            foreach (var rec in cellRecords)
            {
                if (rec.HasContent)
                {
                    anyContent = true;
                    break;
                }
            }

            var labelCols = new bool[columnCount];
            var labelRows = new bool[rowHeights.Count];
            foreach (var rec in cellRecords)
            {
                if (!anyContent || rec.HasContent)
                {
                    if (rec.Col < columnCount)
                    {
                        labelCols[rec.Col] = true;
                    }
                    if (rec.Row < labelRows.Length)
                    {
                        labelRows[rec.Row] = true;
                    }
                }
            }

            // Uniform grid check: every label column/row must be populated and
            // all cells the same size, with no merged/spanning cells.
            var uniform = !complex && columnCount > 0 && rowHeights.Count > 0;
            if (uniform)
            {
                var firstW = 0.0;
                var firstH = 0.0;
                var seen = false;
                foreach (var rec in cellRecords)
                {
                    if (anyContent && !rec.HasContent)
                    {
                        continue;
                    }
                    if (!seen)
                    {
                        firstW = rec.Width;
                        firstH = rec.Height;
                        seen = true;
                    }
                    else if (Math.Abs(rec.Width - firstW) > 2 || Math.Abs(rec.Height - firstH) > 2)
                    {
                        uniform = false;
                        break;
                    }
                }

                // Every label col/row must have exactly the expected cell count
                // and first/last columns must be labels (no leading/trailing spacers).
                var labelColCount = 0;
                var labelRowCount = 0;
                var firstLabelCol = -1;
                var lastLabelCol = -1;
                var firstLabelRow = -1;
                var lastLabelRow = -1;
                for (var c = 0; c < columnCount; c++)
                {
                    if (labelCols[c])
                    {
                        labelColCount++;
                        if (firstLabelCol < 0) firstLabelCol = c;
                        lastLabelCol = c;
                    }
                }
                for (var r = 0; r < labelRows.Length; r++)
                {
                    if (labelRows[r])
                    {
                        labelRowCount++;
                        if (firstLabelRow < 0) firstLabelRow = r;
                        lastLabelRow = r;
                    }
                }

                if (firstLabelCol != 0 || lastLabelCol != columnCount - 1 ||
                    firstLabelRow != 0 || lastLabelRow != labelRows.Length - 1)
                {
                    uniform = false;
                }
            }

            // Build cell rectangles in surface units regardless (fallback data).
            foreach (var rec in cellRecords)
            {
                if (anyContent && !rec.HasContent)
                {
                    continue; // spacer cell
                }
                if (rec.Col >= columnCount || rec.Row >= rowHeights.Count)
                {
                    continue;
                }

                result.Cells.Add(new Rectangle(
                    TwipsToUnits(colLeft[rec.Col]),
                    TwipsToUnits(rowTop[rec.Row]),
                    TwipsToUnits(rec.Width > 0 ? rec.Width : (colWidths.Count > rec.Col ? colWidths[rec.Col] : 240)),
                    TwipsToUnits(rec.Height)));
            }

            if (!uniform || result.Cells.Count == 0)
            {
                return;
            }

            // Derive the LabelSheet from the regular grid.
            var labelCellW = result.Cells[0].Width;
            var labelCellH = result.Cells[0].Height;
            var firstCell = result.Cells[0];

            // Distinct cell origins give the label grid dimensions.
            var xs = new SortedSet<int>();
            var ys = new SortedSet<int>();
            foreach (var cell in result.Cells)
            {
                xs.Add(cell.X);
                ys.Add(cell.Y);
            }
            var xsList = new List<int>(xs);
            var ysList = new List<int>(ys);
            var gridColCount = xsList.Count;
            var gridRowCount = ysList.Count;

            var pitchX = gridColCount > 1 ? xsList[1] - xsList[0] : labelCellW;
            var pitchY = gridRowCount > 1 ? ysList[1] - ysList[0] : labelCellH;

            result.Sheet = new LabelSheet(
                result.SourceName,
                result.PageSize.Width,
                result.PageSize.Height,
                gridColCount,
                gridRowCount,
                labelCellW,
                labelCellH,
                firstCell.X,
                firstCell.Y,
                pitchX,
                pitchY);
        }

        private class CellRecord
        {
            public int Row;
            public int Col;
            public double Width;
            public double Height;
            public bool HasContent;
        }
    }
}
