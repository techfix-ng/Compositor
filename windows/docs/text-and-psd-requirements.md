# Windows Text System and PSD Compatibility

## Release requirement

The Windows edition must provide Photoshop-style editable text. The Type tool is not a placeholder and the next user-facing installer must not be published until the minimum release gate below passes.

## Text creation and editing

- Point text (click and type)
- Paragraph/area text (drag a resizable text box)
- Horizontal and vertical text
- Direct canvas editing with cursor and selection
- Standard clipboard, word selection and line selection
- Commit/cancel editing
- Undo/redo for text and formatting
- Editable text layers retained across save/reopen
- Unicode support, including Yoruba diacritics and emoji when supported by the selected font

## Character controls

- Searchable installed-font family picker with previews
- Font style and weight
- Font size
- Leading/line spacing
- Tracking/letter spacing and kerning
- Baseline shift
- Horizontal and vertical scale
- Bold, italic, underline and strikethrough
- Superscript and subscript
- All caps and small caps
- Anti-aliasing modes
- Text color, including formatting selected ranges

## PSD font discovery and resolution

PSD files normally reference fonts but do not contain installable font files. For every imported text layer, Compositor must preserve and inspect the PSD font descriptor, including PostScript name, family, style, weight and stretch where available.

Resolution order:

1. Exact installed PostScript/full-face name.
2. Exact installed family plus style, weight and stretch.
3. Normalized family alias plus the nearest matching face.
4. User-selected replacement from the installed Windows font catalog.
5. Rendered PSD composite fallback when no editable match is available.

Requirements:

- Enumerate fonts installed for all users and the current Windows user.
- Refresh the font catalog when Windows reports a font change.
- Resolve every distinct font run within a text layer, not only the first font.
- Mark missing fonts on both the text layer and the Character panel.
- Show the original missing font name and the proposed replacement.
- Allow replace-once, replace-in-document and replace-all choices.
- Never silently substitute or permanently overwrite the original PSD font identity.
- Store the original descriptor alongside any local substitution so reopening on another computer can resolve it again.
- Do not copy, extract or redistribute commercial font files.
- Preserve the PSD composite appearance as a visual fallback until the user chooses a replacement.
- Reflow paragraph text only after the user accepts a substitution because font metrics may change line breaks.

## Paragraph controls

- Left, center, right and justified alignment
- First-line and paragraph indentation
- Space before/after paragraphs
- Left-to-right and right-to-left layout where supported
- Bullets and numbered lists
- Text-box inset/padding
- Overflow indicator

## Layer and transform integration

- Text-layer thumbnails
- Rename, duplicate, reorder, hide, lock and group
- Opacity and common blend modes
- Layer masks and clipping masks
- Move, scale, rotate, skew and free transform without forced rasterization
- Exact numeric transform controls
- Smart raster fallback for unsupported effects

## PSD/PSB compatibility

- Always render a composite preview rather than a blank canvas
- Import supported Photoshop text layers as editable text
- Preserve font family, size, color, alignment, transform and text-box bounds
- Warn about missing fonts and offer substitution
- Preserve rendered appearance when an advanced Photoshop feature cannot be mapped
- Preserve original metadata needed for improved future compatibility
- Include text layers in the actual Layers panel hierarchy

## Windows UI

- Type tool in the complete tool rail
- Contextual options bar for family, size, alignment, color and commit/cancel
- Character and Paragraph panels
- Type menu and Windows keyboard shortcuts
- Compact dark styling matching the macOS application while retaining native Windows behavior

## Minimum release gate

Do not publish the next user-facing installer until:

1. Text can be created, edited, transformed, saved and reopened.
2. PSD/PSB files display their composite preview.
3. Supported PSD text layers appear in the real Layers panel.
4. Missing or unsupported fonts/features show a warning and visual fallback instead of a blank canvas.
5. Raster export correctly includes every visible text layer.
6. PSD fonts are checked against the local Windows font catalog using the resolution order above.
