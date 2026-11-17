using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace DesignSurface
{
    /// <summary>
    /// Describes a label sheet template (e.g. Avery 5160): the paper size and
    /// the grid of identical labels printed on it. All dimensions are stored in
    /// 1/100 inch units, matching SurfaceBounds/SurfaceMargins.
    /// </summary>
    public class LabelSheet
    {
        public string Name { get; private set; }

        /// <summary>Sheet width in 1/100 inch.</summary>
        public int SheetWidth { get; private set; }

        /// <summary>Sheet height in 1/100 inch.</summary>
        public int SheetHeight { get; private set; }

        public int Columns { get; private set; }

        public int Rows { get; private set; }

        /// <summary>Single label width in 1/100 inch.</summary>
        public int LabelWidth { get; private set; }

        /// <summary>Single label height in 1/100 inch.</summary>
        public int LabelHeight { get; private set; }

        /// <summary>Distance from the left sheet edge to the first label, 1/100 inch.</summary>
        public int MarginLeft { get; private set; }

        /// <summary>Distance from the top sheet edge to the first label, 1/100 inch.</summary>
        public int MarginTop { get; private set; }

        /// <summary>Left edge to left edge distance between columns, 1/100 inch.</summary>
        public int PitchX { get; private set; }

        /// <summary>Top edge to top edge distance between rows, 1/100 inch.</summary>
        public int PitchY { get; private set; }

        public LabelSheet(string name, double sheetWidth, double sheetHeight, int columns, int rows,
            double labelWidth, double labelHeight, double marginLeft, double marginTop,
            double pitchX, double pitchY)
        {
            Name = name;
            SheetWidth = ToUnits(sheetWidth);
            SheetHeight = ToUnits(sheetHeight);
            Columns = columns;
            Rows = rows;
            LabelWidth = ToUnits(labelWidth);
            LabelHeight = ToUnits(labelHeight);
            MarginLeft = ToUnits(marginLeft);
            MarginTop = ToUnits(marginTop);
            PitchX = ToUnits(pitchX);
            PitchY = ToUnits(pitchY);
        }

        private static int ToUnits(double inches)
        {
            return (int)Math.Round(inches * 100.0);
        }

        /// <summary>
        /// Returns the bounds of the label cell at (column, row) in 1/100 inch
        /// surface units. Column/row are zero-based.
        /// </summary>
        public Rectangle GetCellBounds(int column, int row)
        {
            return new Rectangle(
                MarginLeft + column * PitchX,
                MarginTop + row * PitchY,
                LabelWidth,
                LabelHeight);
        }

        public IEnumerable<Rectangle> GetAllCells()
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                {
                    yield return GetCellBounds(c, r);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>Sentinel value meaning "no template".</summary>
        public static readonly LabelSheet NoTemplate = new LabelSheet("(No template)", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        // Template dimensions from Microsoft's published Avery template table
        // (same data Word's label templates are built from). Inches.
        public static readonly LabelSheet[] Presets =
        {
            // Address labels
            new LabelSheet("5160 Address (30/sheet)", 8.501, 11.000, 3, 10, 2.625, 1.000, 0.188, 0.500, 2.750, 1.000),
            new LabelSheet("5161 Address (20/sheet)", 8.500, 11.000, 2, 10, 4.000, 1.000, 0.156, 0.500, 4.188, 1.000),
            new LabelSheet("5162 Address (14/sheet)", 8.500, 10.997, 2, 7, 4.000, 1.333, 0.156, 0.833, 4.188, 1.333),
            new LabelSheet("5163 Shipping (10/sheet)", 8.500, 11.000, 2, 5, 4.000, 2.000, 0.156, 0.500, 4.188, 2.000),
            new LabelSheet("5164 Shipping (6/sheet)", 8.500, 10.999, 2, 3, 4.000, 3.333, 0.156, 0.500, 4.188, 3.333),
            new LabelSheet("5167 Return address (80/sheet)", 8.501, 11.000, 4, 20, 1.750, 0.500, 0.281, 0.500, 2.063, 0.500),
            new LabelSheet("5660 Address (30/sheet)", 8.499, 11.000, 3, 10, 2.833, 1.000, 0.000, 0.500, 2.833, 1.000),
            new LabelSheet("5661 Address (20/sheet)", 8.500, 11.000, 2, 10, 4.250, 1.000, 0.000, 0.500, 4.250, 1.000),
            new LabelSheet("5662 Address (14/sheet)", 8.500, 11.001, 2, 7, 4.250, 1.333, 0.000, 0.835, 4.250, 1.333),
            new LabelSheet("5663 Shipping (10/sheet)", 8.500, 11.000, 2, 5, 4.250, 2.000, 0.000, 0.500, 4.250, 2.000),
            new LabelSheet("5664 Shipping (6/sheet)", 8.500, 10.999, 2, 3, 4.250, 3.333, 0.000, 0.500, 4.250, 3.333),

            // Specialty labels
            new LabelSheet("5165 Full sheet", 8.500, 11.000, 1, 1, 8.500, 11.000, 0.000, 0.000, 8.500, 11.000),
            new LabelSheet("5266 File folder (30/sheet)", 8.500, 11.005, 2, 15, 3.438, 0.667, 0.531, 0.500, 4.000, 0.667),
            new LabelSheet("5295 Round 3-1/3 in (6/sheet)", 8.520, 10.999, 2, 3, 3.333, 3.333, 0.625, 0.500, 3.937, 3.333),
            new LabelSheet("5196/5096 Square 2-3/4 in (9/sheet)", 8.500, 9.750, 3, 3, 2.750, 2.750, 0.125, 0.500, 2.750, 3.000),
            new LabelSheet("5197 Name badge (12/sheet)", 8.500, 11.000, 2, 6, 4.000, 1.500, 0.156, 1.000, 4.188, 1.500),
            new LabelSheet("5198 Name badge (12/sheet)", 8.500, 11.002, 2, 6, 3.500, 1.667, 0.500, 0.500, 4.000, 1.667),
            new LabelSheet("5199 Name badge (10/sheet)", 8.501, 10.999, 2, 5, 3.063, 1.833, 1.069, 0.917, 3.300, 1.833),
            new LabelSheet("5395 Name badge (8/sheet)", 8.501, 11.000, 2, 4, 3.375, 2.333, 0.688, 0.583, 3.750, 2.500),

            // Card products
            new LabelSheet("5371 Cards (10/sheet)", 8.500, 11.000, 2, 5, 3.500, 2.000, 0.750, 0.500, 3.500, 2.000),
            new LabelSheet("5383 Cards (8/sheet)", 8.500, 11.000, 2, 4, 3.500, 2.167, 0.750, 1.167, 3.500, 2.167),
            new LabelSheet("5384 Cards (6/sheet)", 8.500, 11.250, 2, 3, 4.000, 3.000, 0.250, 1.125, 4.000, 3.000),
            new LabelSheet("5385 Cards (8/sheet)", 8.500, 11.000, 2, 4, 4.000, 2.167, 0.250, 1.167, 4.000, 2.167),
            new LabelSheet("5386/5388 Cards (3/sheet)", 8.500, 11.000, 1, 3, 5.000, 3.000, 1.750, 1.000, 5.000, 3.000),
            new LabelSheet("5389 Cards (2/sheet)", 8.500, 11.000, 1, 2, 6.000, 4.000, 1.250, 1.500, 6.000, 4.000),
            new LabelSheet("5305 Half sheet (2/sheet)", 8.500, 11.000, 1, 2, 8.500, 5.000, 0.000, 0.500, 8.500, 5.000)
        };

        public static LabelSheet FindByName(string name)
        {
            foreach (var preset in Presets)
            {
                if (string.Equals(preset.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return preset;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// TypeConverter so the PropertyGrid shows Template as a dropdown of the
    /// preset names instead of an editable object.
    /// </summary>
    public class LabelSheetConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string name)
            {
                if (string.IsNullOrWhiteSpace(name) || name == LabelSheet.NoTemplate.Name)
                {
                    return LabelSheet.NoTemplate;
                }

                return LabelSheet.FindByName(name) ?? LabelSheet.NoTemplate;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is LabelSheet sheet)
            {
                return sheet.Name;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = new List<LabelSheet>(LabelSheet.Presets.Length + 1) { LabelSheet.NoTemplate };
            values.AddRange(LabelSheet.Presets);
            return new StandardValuesCollection(values);
        }
    }
}
