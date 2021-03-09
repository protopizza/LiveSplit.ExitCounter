using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LiveSplit.Model.Input;

namespace LiveSplit.UI.Components
{
    public class ExitCounterComponent : IComponent
    {
        public ExitCounterComponent()
        {
            VerticalHeight = 10;
            Settings = new ExitCounterComponentSettings();
            Cache = new GraphicsCache();
            ExitCounterLabel = new SimpleLabel();
        }

        public ExitCounterComponentSettings Settings { get; set; }

        public GraphicsCache Cache { get; set; }

        public float VerticalHeight { get; set; }

        public float MinimumHeight { get; set; }

        public float MinimumWidth 
        { 
            get
            {
                return ExitCounterLabel.X + ExitCounterLabel.ActualWidth;
            } 
        }

        public float HorizontalWidth { get; set; }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return null; }
        }

        public float PaddingTop { get; set; }
        public float PaddingLeft { get { return 7f; } }
        public float PaddingBottom { get; set; }
        public float PaddingRight { get { return 7f; } }

        protected SimpleLabel ExitCounterLabel = new SimpleLabel();

        protected Font CounterFont { get; set; }

        private void DrawGeneral(Graphics g, Model.LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // Set Background colour.
            if (Settings.BackgroundColor.A > 0
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.A > 0)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);

                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }

            // Set Font.
            CounterFont = Settings.OverrideCounterFont ? Settings.CounterFont : state.LayoutSettings.TextFont;

            // Calculate Height from Font.
            var textHeight = g.MeasureString("A", CounterFont).Height;
            VerticalHeight = 1.2f * textHeight;
            MinimumHeight = MinimumHeight;

            PaddingTop = Math.Max(0, ((VerticalHeight - 0.75f * textHeight) / 2f));
            PaddingBottom = PaddingTop;

            HorizontalWidth = ExitCounterLabel.X + ExitCounterLabel.ActualWidth + 5;

            // Set Counter Label.
            ExitCounterLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Center : StringAlignment.Center;
            ExitCounterLabel.VerticalAlignment = StringAlignment.Center;
            ExitCounterLabel.X = 10;
            ExitCounterLabel.Y = 0;
            ExitCounterLabel.Width = (width - 10);
            ExitCounterLabel.Height = height;
            ExitCounterLabel.Font = CounterFont;
            ExitCounterLabel.Brush = new SolidBrush(Settings.OverrideTextColor ? Settings.CounterTextColor : state.LayoutSettings.TextColor);
            ExitCounterLabel.HasShadow = state.LayoutSettings.DropShadows;
            ExitCounterLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            ExitCounterLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            
            ExitCounterLabel.Draw(g);
        }

        public void DrawHorizontal(Graphics g, Model.LiveSplitState state, float height, Region clipRegion)
        {
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        }

        public void DrawVertical(System.Drawing.Graphics g, Model.LiveSplitState state, float width, Region clipRegion)
        {
            DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
        }

        public string ComponentName
        {
            get { return "Exit Counter"; }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, Model.LiveSplitState state, float width, float height, LayoutMode mode)
        {
            int splitMinusOne = 0;
            try
            {
                splitMinusOne = int.Parse(state.CurrentSplit.Name) - 1;
            }
            catch {
                try
                {
                    splitMinusOne = Math.Max(0, state.CurrentSplitIndex);
                }
                catch { }
            }

            if (Settings.AutoTotalCount)
            {
                ExitCounterLabel.Text = Settings.ExitCounterText + " " + splitMinusOne.ToString() + "/" + state.Run.Count;
            }
            else
            {
                ExitCounterLabel.Text = Settings.ExitCounterText + " " + splitMinusOne.ToString() + "/" + Settings.TotalExitCount.ToString();
            }

            Cache.Restart();
            Cache["ExitCounterLabel"] = ExitCounterLabel.Text;

            if (invalidator != null && Cache.HasChanged)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        public void Dispose()
        {

        }

        public int GetSettingsHashCode()
        {
            return Settings.GetSettingsHashCode();
        }

    }
}
