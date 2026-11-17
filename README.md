# DesignSurfaceCSharp

A scrollable, zoomable **design surface** for WinForms — the canvas half of a
label/graphic designer — plus the **graphic object model** that draws on it.
Drop the `DesignSurface` control into any WinForms app and you get a page with
margins, a dot grid, object selection, undo/redo and a print pipeline, ready to
host text, images, shapes and more.

<p align="center">
  <img src="assets/design-surface.svg" alt="DesignSurface canvas with grid, margins and graphic objects, beside a capabilities list" width="880">
</p>

## Projects

| Project | Output | Purpose |
| --- | --- | --- |
| `GraphicObjects` | `GraphicObjects.dll` | The drawable object model — text, images, shapes, lines, stars — plus collection, history and filters. |
| `GraphicsSurface` | `GraphicsSurface.dll` | The `DesignSurface` UserControl: painting, input, selection, zoom, print and the PropertyGrid pane. |

`DesignSurface.slnx` contains both library projects — no sample apps.

## Capabilities

**Canvas**

- Printable page bounds with configurable portrait/landscape setup
- Non-printing margin guides and a configurable dot grid
- Smooth zoom, scroll and pan with double-buffered custom painting

**Objects**

- `TextGraphic`, `ImageGraphic` (linked + embedded), `ShapeGraphic`
  (rectangle, ellipse), `LineGraphic`, star and background-image objects
- Drag, resize and rotate with grab handles; click or rubber-band select
- Bring to front / send to back, undo & redo, cut/copy/paste
- Built-in `PropertyGrid` pane edits whichever object is selected

**Output**

- Print and print-preview pipeline honouring the page margins
- Save/open design documents and export the surface as an image
- `StatusUpdate` / `SelectionChanged` events so a host app can react

## Usage

```csharp
var surface = new DesignSurface.DesignSurface
{
    Dock = DockStyle.Fill,
    PageSize = DesignSurface.PageSizeKind.Letter,
    Orientation = DesignSurface.GridOrientation.Portrait,
};

surface.drawingObjects.Add(new GraphicObjects.TextGraphic(
    new Point(100, 100), "Hello", new Font("Arial", 24), Color.Black));

surface.SelectionChanged += (s, e) => { /* react to selection */ };
surface.PrintPreview();
```

## Building

Open `GraphicsSurface/DesignSurface.slnx` in Visual Studio (2022 17.10+ or
VS 2026) — it builds both projects. Both target .NET Framework 4.8 and use
only framework assemblies, so the DLLs run on any Windows Forms app.

> **Note:** the object and surface namespaces are `GraphicObjects` and
> `DesignSurface` — the code was ported from the old
> `Microsoft.Msdn.Samples.*` VB sample, so expect names like
> `GraphicObjects.GraphicObjects` for the base object type.

## License

Derived from the Microsoft DesignSurface sample; see each project's
`AssemblyInfo` for origin notes.

---

<p align="center"><sub>Built for <a href="https://github.com/DwainSnickles">Dwain Snickles</a> · used by <a href="https://github.com/DwainSnickles/GraphicsWorkshop">GraphicsWorkshop</a></sub></p>
</file_path_path>
</invoke>
