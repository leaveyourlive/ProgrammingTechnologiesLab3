using System.Drawing.Drawing2D;

namespace MovingAverageLab;

/// <summary>
/// Кнопка с закруглёнными углами, эффектом наведения и тенью.
/// </summary>
public class RoundedButton : Control
{
    // ── Свойства ────────────────────────────────────────────────
    public Color BaseColor  { get; set; } = Color.FromArgb(52, 110, 235);
    public Color HoverColor { get; set; } = Color.FromArgb(30, 85, 200);
    public Color BorderColor{ get; set; } = Color.Transparent;
    public int   Radius     { get; set; } = 10;

    private bool _isHovered;
    private bool _isPressed;

    public RoundedButton()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        Cursor = Cursors.Hand;
        Font   = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
    }

    // ── Мышь ────────────────────────────────────────────────────
    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        _isPressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isPressed = true;
            Invalidate();
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _isPressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }

    // ── Отрисовка ───────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rc = new Rectangle(2, 2, Width - 5, Height - 5);

        // Тень (только в нормальном состоянии)
        if (!_isPressed)
        {
            using var shadow = new SolidBrush(Color.FromArgb(18, 0, 0, 0));
            using var shadowPath = RoundedRect(new Rectangle(3, 4, Width - 5, Height - 4), Radius);
            g.FillPath(shadow, shadowPath);
        }

        // Фон кнопки
        var fillColor = _isPressed
            ? ControlPaint.Dark(HoverColor, 0.05f)
            : _isHovered ? HoverColor : BaseColor;

        using var brush = new SolidBrush(fillColor);
        using var path  = RoundedRect(rc, Radius);
        g.FillPath(brush, path);

        // Рамка
        if (BorderColor != Color.Transparent)
        {
            using var pen = new Pen(BorderColor, 1.4f);
            g.DrawPath(pen, path);
        }

        // Текст
        var flags = TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter   |
                    TextFormatFlags.SingleLine;

        var offset = _isPressed ? 1 : 0;
        var textRc = new Rectangle(rc.X + offset, rc.Y + offset, rc.Width, rc.Height);
        TextRenderer.DrawText(g, Text, Font, textRc, ForeColor, flags);
    }

    // ── Helpers ─────────────────────────────────────────────────
    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
