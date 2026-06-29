using System.Drawing.Drawing2D;

namespace OHelper.UI
{
    public class RBadgeButton : RButton
    {
        private int badge = 0;
        public int Badge
        {
            get => badge;
            set
            {
                if (badge != value)
                {
                    badge = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (badge <= 0) return;

            float ratio = pevent.Graphics.DpiX / 192.0f;
            var rectSurface = ClientRectangle;

            using (Brush brush = new SolidBrush(BorderColor))
            {
                var radius = ratio * 14;
                var badgeRect = new RectangleF(
                    rectSurface.Width - rectSurface.Height / 2f - radius,
                    rectSurface.Height / 2f - radius,
                    radius + radius,
                    radius + radius
                );

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.FillEllipse(brush, badgeRect);

                using (GraphicsPath path = new GraphicsPath())
                using (FontFamily family = new FontFamily("Segoe UI"))
                using (StringFormat sf = StringFormat.GenericTypographic)
                {
                    path.AddString(badge.ToString(), family, (int)FontStyle.Bold, 100f, PointF.Empty, sf);
                    path.Flatten();

                    RectangleF ink = path.GetBounds();
                    float scale = radius * 1.1f / ink.Height;
                    float anchorX = ink.X + ink.Width / 2f + (badge == 1 ? ink.Width * 0.10f : 0f);

                    using System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                    matrix.Translate(badgeRect.X + badgeRect.Width / 2f, badgeRect.Y + badgeRect.Height / 2f);
                    matrix.Scale(scale, scale);
                    matrix.Translate(-anchorX, -(ink.Y + ink.Height / 2f));
                    path.Transform(matrix);

                    using Brush textBrush = new SolidBrush(RForm.foreMain);
                    pevent.Graphics.FillPath(textBrush, path);
                }
            }
        }
    }
}
